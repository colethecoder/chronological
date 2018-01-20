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
        private readonly IEventWebSocketRepository _eventWebSocketRepository;

        internal EventQuery(string queryName, Environment environment,
            IEventWebSocketRepository eventWebSocketRepository)
        {
            _queryName = queryName;
            _environment = environment;
            _eventWebSocketRepository = eventWebSocketRepository;
        }

        internal EventQuery(string queryName, Environment environment) : this(queryName, environment, new EventWebSocketRepository(new WebSocketRepository(environment)))
        {            
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

    }
}
