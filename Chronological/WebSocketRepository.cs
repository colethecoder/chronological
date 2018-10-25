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
    internal class WebSocketRepository : IWebRequestRepository
    {
        private readonly Environment _environment;
        private readonly IErrorToExceptionConverter _errorToExceptionConverter;

        internal WebSocketRepository(Environment environment) : this(environment, new ErrorToExceptionConverter())
        {            
        }

        internal WebSocketRepository(Environment environment, IErrorToExceptionConverter errorToExceptionConverter)
        {
            _environment = environment;
            _errorToExceptionConverter = errorToExceptionConverter;
        }

        async Task<IReadOnlyList<JToken>> IWebRequestRepository.ExecuteRequestAsync(string query, string resourcePath, CancellationToken cancellationToken)
        {
            var webSocket = new ClientWebSocket();

            Uri uri = new UriBuilder("wss", _environment.EnvironmentFqdn)
            {
                Path = resourcePath,
                Query = "api-version=2016-12-12"
            }.Uri;

            await webSocket.ConnectAsync(uri, cancellationToken);

            byte[] inputPayloadBytes = Encoding.UTF8.GetBytes(query);
            await webSocket.SendAsync(
                new ArraySegment<byte>(inputPayloadBytes),
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: cancellationToken);

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
                                cancellationToken);

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

                    JObject messageObj = JsonConvert.DeserializeObject<JObject>(message, new JsonSerializerSettings
                    {
                        DateParseHandling = DateParseHandling.None
                    });

                    if (messageObj["error"] != null)
                    {
                        var error = messageObj["error"].ToObject<ErrorResult>();

                        // Close web socket connection.
                        if (webSocket.State == WebSocketState.Open)
                        {
                            await webSocket.CloseAsync(
                                WebSocketCloseStatus.NormalClosure,
                                "CompletedByClient",
                                cancellationToken);
                        }

                        throw _errorToExceptionConverter.ConvertTimeSeriesErrorToException(error);                                               
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
                        cancellationToken);
                }
            }

            return responseMessagesContent;
        }
    }
}
