using System;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Search
    {
        private readonly DateTime _from;
        private readonly DateTime _to;

        private Search(DateTime from, DateTime to)
        {
            _from = from;
            _to = to;
        }

        public static Search Span(DateTime from, DateTime to)
        {
            return new Search(from, to);
        }

        internal JProperty ToJProperty()
        {
            return new JProperty("searchSpan", new JObject(
                new JProperty("from", _from.ToUniversalTime()),
                new JProperty("to", _to.ToUniversalTime())));
        }
    }
}
