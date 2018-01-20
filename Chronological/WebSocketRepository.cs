using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chronological.Exceptions;
using Chronological.QueryResults;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    internal interface IWebSocketRepository
    {        
        Task<IReadOnlyList<JToken>> ReadWebSocketResponseAsync(string query, string resourcePath);
    }

    internal class WebSocketRepository : IWebSocketRepository
    {
        private readonly Environment _environment;

        internal WebSocketRepository(Environment environment)
        {
            _environment = environment;
        }

        async Task<IReadOnlyList<JToken>> IWebSocketRepository.ReadWebSocketResponseAsync(string query, string resourcePath)
        {
            var webSocket = new ClientWebSocket();

            Uri uri = new UriBuilder("wss", _environment.EnvironmentFqdn)
            {
                Path = resourcePath,
                Query = "api-version=2016-12-12"
            }.Uri;

            await webSocket.ConnectAsync(uri, CancellationToken.None);

            byte[] inputPayloadBytes = Encoding.UTF8.GetBytes(query);
            await webSocket.SendAsync(
                new ArraySegment<byte>(inputPayloadBytes),
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);

            List<JToken> responseMessagesContent = new List<JToken>();
            using (webSocket)
            {
                while (true)
                {
                    string message;
                    using (var ms = new MemoryStream())
                    {
                        const int bufferSize = 16 * 1024;
                        var temporaryBuffer = new byte[bufferSize];
                        while (true)
                        {
                            WebSocketReceiveResult response = await webSocket.ReceiveAsync(
                                new ArraySegment<byte>(temporaryBuffer),
                                CancellationToken.None);

                            ms.Write(temporaryBuffer, 0, response.Count);
                            if (response.EndOfMessage)
                            {
                                break;
                            }
                        }

                        ms.Position = 0;

                        using (var sr = new StreamReader(ms))
                        {
                            message = sr.ReadToEnd();
                        }
                    }

                    JObject messageObj = JsonConvert.DeserializeObject<JObject>(message);

                    if (messageObj["error"] != null)
                    {
                        var errorObj = messageObj["error"].ToObject<ErrorResult>();

                        if (errorObj.Code == "AuthenticationFailed")
                        {
                            if (errorObj.InnerError?.Code == "TokenExpired")
                            {
                                throw new ChronologicalExpiredAccessTokenException(errorObj.InnerError.Message);
                            }
                        }
                        var errorMessage = $"Error Code: {errorObj.Code}, Error Message: {errorObj.Message}";
                        if (errorObj.InnerError != null)
                        {
                            errorMessage +=
                                $", Inner Error Code: {errorObj.InnerError.Code}, Inner Error Message: {errorObj.InnerError.Message}";
                        }
                        throw new ChronologicalUnexpectedException(errorMessage);                        
                    }

                    // Actual response contents is wrapped into "content" object.
                    responseMessagesContent.Add(messageObj["content"]);

                    // Stop reading if 100% of completeness is reached.
                    if (messageObj["percentCompleted"] != null &&
                        Math.Abs((double)messageObj["percentCompleted"] - 100d) < 0.01)
                    {
                        break;
                    }
                }

                // Close web socket connection.
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "CompletedByClient",
                        CancellationToken.None);
                }
            }

            return responseMessagesContent;
        }
    }
}
