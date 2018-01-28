using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class StringEventQuery : EventQuery
    {

        private readonly string _query;

        internal StringEventQuery(string queryName, string query, Environment environment) : base(queryName, environment)
        {
            _query = query;
        }

        protected override JProperty GetContent()
        {
            return new JProperty("content", JObject.Parse(_query));
        }
    }
}
