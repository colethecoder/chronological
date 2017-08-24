using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chronological.QueryResults.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class EventsQuery
    {
        private readonly string _queryName;

        private Search _search;
        private Filter _filter;
        private Limit _limit;
        private readonly Environment _environment;

        internal EventsQuery(string queryName, Environment environment)
        {
            _queryName = queryName;
            _environment = environment;
        }

        public EventsQuery WithSearch(Search search)
        {
            _search = search;

            return this;
        }

        public EventsQuery WithLimit(Limit limit)
        {
            _limit = limit;

            return this;
        }

        public EventsQuery Where(Filter filter)
        {
            _filter = filter;
            return this;
        }

        public JObject ToJObject(string accessToken)
        {
            return new JObject(
                GetHeaders(accessToken),
                GetContent()
            );
        }

        private JProperty GetHeaders(string accessToken)
        {
            return new JProperty("headers", new JObject(
                new JProperty("x-ms-client-application-name", _queryName),
                new JProperty("Authorization", "Bearer " + accessToken)));
        }

        private JProperty GetContent()
        {
            return new JProperty("content", new JObject(
                _search.ToJProperty(),
                _filter.ToPredicateJProperty(),
                _limit.ToJProperty()
            ));
        }

        public async Task<JObject> ResultsToJObjectAsync()
        {
            var inputPayload = ToJObject(_environment.AccessToken);
            var webSocket = new ClientWebSocket();

            var test = inputPayload.ToString();

            Uri uri = new UriBuilder("wss", _environment.EnvironmentFqdn)
            {
                Path = "events",
                Query = "api-version=2016-12-12"
            }.Uri;

            await webSocket.ConnectAsync(uri, CancellationToken.None);

            byte[] inputPayloadBytes = Encoding.UTF8.GetBytes(inputPayload.ToString());
            await webSocket.SendAsync(
                new ArraySegment<byte>(inputPayloadBytes),
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);

            JObject responseContent = null;
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
                        break;
                    }

                    var currentContents = (JObject)messageObj["content"];
                    var currentEvents = (JArray) currentContents["events"];

                    responseContent = (JObject)currentContents;

                    if (messageObj["percentCompleted"] != null &&
                        Math.Abs((double)messageObj["percentCompleted"] - 100d) < 0.01)
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

            return responseContent;
        }

        public async Task<EventQueryResult> ResultsToEventQueryResultAsync()
        {
            var inputPayload = ToJObject(_environment.AccessToken);
            var webSocket = new ClientWebSocket();

            var test = inputPayload.ToString();

            Uri uri = new UriBuilder("wss", _environment.EnvironmentFqdn)
            {
                Path = "events",
                Query = "api-version=2016-12-12"
            }.Uri;

            await webSocket.ConnectAsync(uri, CancellationToken.None);

            byte[] inputPayloadBytes = Encoding.UTF8.GetBytes(inputPayload.ToString());
            await webSocket.SendAsync(
                new ArraySegment<byte>(inputPayloadBytes),
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);

            EventQueryResult responseContent = null;
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

                    responseContent = JsonConvert.DeserializeObject<EventQueryResult>(message);

                    //Todo error handling


                    if (Math.Abs(responseContent.PercentCompleted - 100d) < 0.01)
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

            return responseContent;
        }

        //public async Task<dynamic> ResultsToDynamic()
        //{
        //    var results = await ResultsToJObjectAsync();

        //    throw new NotImplementedException();


        //}
    }
}
