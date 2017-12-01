using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Chronological.QueryResults.Aggregates;

namespace Chronological
{

    public abstract class AggregateQuery
    {
        private readonly string _queryName;
       
        private readonly Environment _environment;
        private readonly WebSocketRepository _webSocketRepository;

        internal AggregateQuery(string queryName, Environment environment)
        {
            _queryName = queryName;
            _environment = environment;
            _webSocketRepository = new WebSocketRepository(environment);
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

        protected abstract JProperty GetContent();

        

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
