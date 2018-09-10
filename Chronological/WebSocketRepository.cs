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
    internal class WebSocketRepository : IWebSocketRepository
    {
        private readonly Environment _environment;
        private readonly ErrorToExceptionConverter _errorToExceptionConverter;

        internal WebSocketRepository(Environment environment) : this(environment, new ErrorToExceptionConverter())
        {            
        }

        internal WebSocketRepository(Environment environment, ErrorToExceptionConverter errorToExceptionConverter)
        {
            _environment = environment;
            _errorToExceptionConverter = errorToExceptionConverter;
        }

        async Task<IEnumerable<T>> IWebSocketRepository.ReadWebSocketResponseAsync<T>(string query, string resourcePath, Func<StreamReader, WebSocketResult<T>> parseFunc)
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

            List<T> responseMessagesContent = new List<T>();
            using (webSocket)
            {

                var complete = false;
                while (!complete)
                {
                    WebSocketResult<T> result;
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
                            result = parseFunc(sr);
                            //ms.Position = 0;
                            //var message = sr.ReadToEnd();
                        }
                    }

                    switch (result)
                    {
                        case WebSocketResult<T>.WebSocketSuccess success:
                            responseMessagesContent.AddRange(success.Results);
                            complete = !success.Continue;
                            break;
                        case WebSocketResult<T>.WebSocketFailure failure:
                            if (webSocket.State == WebSocketState.Open)
                            {
                                await webSocket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    "CompletedByClient",
                                    CancellationToken.None);
                            }
                            throw failure.Exception;
                        default:
                            // Should never get here
                            throw new NotSupportedException("Chronological Web Socket Issue");
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
