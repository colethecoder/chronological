using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class FluentEventQuery : EventQuery
    {
        private Search _search;
        private Filter _filter;
        private Limit _limit;

        internal FluentEventQuery(string queryName, Environment environment) : base(queryName, environment)
        {
        }

        public FluentEventQuery WithSearch(Search search)
        {
            _search = search;

            return this;
        }

        public FluentEventQuery WithLimit(Limit limit)
        {
            _limit = limit;

            return this;
        }

        public FluentEventQuery Where(Filter filter)
        {
            _filter = filter;
            return this;
        }

        public FluentEventQuery Where(string predicateString)
        {
            _filter = Filter.FromString(predicateString);
            return this;
        }

        protected override JProperty GetContent()
        {
            return new JProperty("content", new JObject(
                _search.ToJProperty(),
                _filter.ToPredicateJProperty(),
                _limit.ToJProperty()
            ));
        }
    }
}
