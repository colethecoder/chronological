using Newtonsoft.Json.Linq;

namespace Chronological
{
    public abstract class EventQuery
    {
        private readonly string _queryName;
        private readonly Environment _environment;
        private readonly IEventApiRepository _eventApiRepository;

        internal EventQuery(string queryName, Environment environment,
            IEventApiRepository eventApiRepository)
        {
            _queryName = queryName;
            _environment = environment;
            _eventApiRepository = eventApiRepository;
        }

        internal EventQuery(string queryName, Environment environment) : this(queryName, environment, new EventApiRepository(new WebSocketRepository(environment)))
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
