using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Chronological.QueryResults.Aggregates;

namespace Chronological
{
    public class AggregatesQuery
    {
        private readonly string _queryName;

        private Search _search;
        private List<Aggregate> _aggregates;
        private Filter _filter;
        private readonly Environment _environment;
        private readonly WebSocketRepository _webSocketRepository;

        internal AggregatesQuery(string queryName, Environment environment)
        {
            _queryName = queryName;
            _environment = environment;
            _webSocketRepository = new WebSocketRepository(environment);
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

        public new string ToString()
        {
            return ToJObject(_environment.AccessToken).ToString();
        }

        public async Task<JObject> ResultsToJObjectAsync()
        {
            var results = await _webSocketRepository.QueryWebSocket(ToString(),"aggregates");

            if (results != null && results.Any())
            {
                return JsonConvert.DeserializeObject<JObject>(results.First());
            }

            return null;
        }

        public async Task<AggregateQueryResult> ResultsToAggregateQueryResultAsync()
        {
            var results = await _webSocketRepository.QueryWebSocket(ToString(), "aggregates");

            if (results != null && results.Any())
            {
                return JsonConvert.DeserializeObject<AggregateQueryResult>(results.First());
            }

            return null;           
        }
    }
}
