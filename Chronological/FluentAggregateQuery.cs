using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class FluentAggregateQuery : AggregateQuery
    {
        private Search _search;
        private List<Aggregate> _aggregates;
        private Filter _filter;

        internal FluentAggregateQuery(string queryName, Environment environment) : base(queryName, environment)
        {
        }

        public FluentAggregateQuery WithSearch(Search search)
        {
            _search = search;
            return this;
        }

        public FluentAggregateQuery WithAggregate(Aggregate aggregate)
        {
            if (_aggregates == null)
            {
                _aggregates = new List<Aggregate>();
            }
            _aggregates.Add(aggregate);
            return this;
        }

        public FluentAggregateQuery Where(Filter filter)
        {
            _filter = filter;
            return this;
        }

        public FluentAggregateQuery Where(string predicateString)
        {
            _filter = Filter.FromString(predicateString);
            return this;
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

        protected override JProperty GetContent()
        {
            return new JProperty("content", new JObject(
                _search.ToJProperty(),
                _filter.ToPredicateJProperty(),
                new JProperty("aggregates", GetAggregatesJArray())
            ));
        }
    }
}
