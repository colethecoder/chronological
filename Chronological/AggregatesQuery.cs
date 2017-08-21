using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class AggregatesQuery
    {
        private readonly string _queryName;

        private Search _search;
        private List<Aggregate> _aggregates;
        private Filter _filter;
        private readonly Environment _environment;

        internal AggregatesQuery(string queryName, Environment environment)
        {
            _queryName = queryName;
            _environment = environment;
        }

        public AggregatesQuery WithSearch(Search search)
        {
            _search = search;

            return this;
        }

        public AggregatesQuery WithAggregate(Aggregate aggregate)
        {
            if (_aggregates == null)
            {
                _aggregates = new List<Aggregate>();
            }
            _aggregates.Add(aggregate);
            return this;
        }

        public AggregatesQuery Where(Filter filter)
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

        private JArray GetAggregatesJArray()
        {
            var array = new JArray();
            foreach (var aggregate in _aggregates)
            {
                array.Add(aggregate.ToJObject());
            }
            return array;
        }

        private JProperty GetContent()
        {
            return new JProperty("content", new JObject(
                _search.ToJProperty(),
                _filter.ToPredicateJProperty(),
                new JProperty("aggregates", GetAggregatesJArray())
            ));
        }

        public async Task<JObject> ResultsToJObjectAsync()
        {
            var inputPayload = ToJObject(_environment.AccessToken);
            var webSocket = new ClientWebSocket();

            var test = inputPayload.ToString();

            Uri uri = new UriBuilder("wss", _environment.EnvironmentFqdn)
            {
                Path = "aggregates",
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

                    JArray currentContents = (JArray)messageObj["content"];

                    responseContent = (JObject)currentContents[0];

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
    }
}
