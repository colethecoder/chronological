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
    public class WebSocketRepository
    {
        private readonly Environment _environment;

        internal WebSocketRepository(Environment environment)
        {
            _environment = environment;
        }

        public async Task<List<string>> QueryWebSocket(string query, string resourcePath)
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

            var results = new List<string>();
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

                    // This may be slow but it lets us check the errors and percent complete
                    var messageObj = JsonConvert.DeserializeObject<QueryResult>(message);

                    if (messageObj.Error != null)
                    {
                        if (messageObj.Error.Code == "AuthenticationFailed")
                        {
                            if (messageObj.Error.InnerError?.Code == "TokenExpired")
                            {
                                throw new ChronologicalExpiredAccessTokenException(messageObj.Error.InnerError.Message);
                            }
                        }
                        var errorMessage = $"Error Code: {messageObj.Error.Code}, Error Message: {messageObj.Error.Message}";
                        if (messageObj.Error.InnerError != null)
                        {
                            errorMessage +=
                                $", Inner Error Code: {messageObj.Error.InnerError.Code}, Inner Error Message: {messageObj.Error.InnerError.Message}";
                        }
                        throw new ChronologicalUnexpectedException(errorMessage);
                    }

                    results.Add(message);

                    if (Math.Abs(messageObj.PercentCompleted - 100d) < 0.01)
                    {
                        break;
                    }
            }

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "CompletedByClient",
                    CancellationToken.None);
            }
        }

        return results;
    }
    }
}
