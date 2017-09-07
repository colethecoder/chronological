using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronological.QueryResults.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public abstract class EventQuery
    {
        private readonly string _queryName;
        private readonly Environment _environment;
        private readonly WebSocketRepository _webSocketRepository;


        internal EventQuery(string queryName, Environment environment)
        {
            _queryName = queryName;
            _environment = environment;
            _webSocketRepository = new WebSocketRepository(environment);
        }

        protected abstract JProperty GetContent();

        private JObject ToJObject(string accessToken)
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

        public new string ToString()
        {
            return ToJObject(_environment.AccessToken).ToString();
        }

        public async Task<JObject> ResultsToJObjectAsync()
        {
            var results = await _webSocketRepository.QueryWebSocket(ToString(), "events");

            if (results != null && results.Any())
            {
                return JsonConvert.DeserializeObject<JObject>(results.First());
            }

            return null;
        }

        public async Task<EventQueryResult> ResultsToEventQueryResultAsync()
        {
            var results = await _webSocketRepository.QueryWebSocket(ToString(), "events");

            if (results != null && results.Any())
            {
                return JsonConvert.DeserializeObject<EventQueryResult>(results.First());
            }

            return null;
        }

        public async Task<IEnumerable<T>> ResultsToTypeAsync<T>()
        {
            var results = await _webSocketRepository.QueryWebSocket(ToString(), "events");

            if (results != null && results.Any())
            {
                var eventQueryResult = JsonConvert.DeserializeObject<EventQueryResult>(results.First());
                return new EventQueryResultToTypeMapper().Map<T>(eventQueryResult);
            }

            return null;
        }

    }
}
