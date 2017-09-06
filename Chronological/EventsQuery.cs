using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chronological.QueryResults.Aggregates;
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
        private readonly WebSocketRepository _webSocketRepository;

        private readonly string _query;

        internal EventsQuery(string queryName, Environment environment)
        {
            _queryName = queryName;
            _environment = environment;
            _webSocketRepository = new WebSocketRepository(environment);
        }

        internal EventsQuery(string queryName, string query, Environment environment)
        {
            _queryName = queryName;
            _query = query;
            _environment = environment;
            _webSocketRepository = new WebSocketRepository(environment);
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

            if (string.IsNullOrWhiteSpace(_query))
            {
                return new JProperty("content", new JObject(
                    _search.ToJProperty(),
                    _filter.ToPredicateJProperty(),
                    _limit.ToJProperty()
                ));
            }
            else
            {
                return new JProperty("content", JObject.Parse(_query));
            }
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
