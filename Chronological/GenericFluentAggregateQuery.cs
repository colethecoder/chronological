using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class GenericFluentAggregateQuery<T> where T : new()
    {
        private readonly string _queryName;
        private readonly Search _search;

        internal GenericFluentAggregateQuery(string queryName, Search search)
        {
            _queryName = queryName;
            _search = search;
        }

        public GenericFluentAggregateQuery<T, Aggregate<T, TX, TY>> Select<TX, TY>(Func<AggregateBuilder<T>, Aggregate<T, TX, TY>> predicate) 
            => new GenericFluentAggregateQuery<T, Aggregate<T, TX, TY>>(_queryName, _search, predicate(new AggregateBuilder<T>()));

        public GenericFluentAggregateQuery<T, Aggregate<T, TX, TY>> Select<TX, TY>(IEnumerable<Func<AggregateBuilder<T>, Aggregate<T, TX, TY>>> predicates)
            => new GenericFluentAggregateQuery<T, Aggregate<T, TX, TY>>(_queryName, _search, from predicate in predicates select predicate(new AggregateBuilder<T>()));
    }

    public class GenericFluentAggregateQuery<TX, TY> where TX : new() where TY : IAggregate
    {
        private readonly IEnumerable<TY> _aggregates;
        private Filter _filter;
        private readonly string _queryName;
        private readonly Search _search;

        internal GenericFluentAggregateQuery(string queryName, Search search, TY aggregate) : this(queryName, search,
            new List<TY>
            {
                aggregate
            })
        {
        }

        internal GenericFluentAggregateQuery(string queryName, Search search, IEnumerable<TY> aggregates)
        {
            _queryName = queryName;
            _search = search;
            _aggregates = aggregates;
        }


        public GenericFluentAggregateQuery<TX, TY> Where(Expression<Func<TX,bool>> predicate)
        {
            _filter = Filter.Create(predicate);
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

        private JProperty GetContent()
        {
            if (_filter != null)
            {
                return new JProperty("content", new JObject(
                    _search.ToJProperty(),
                    _filter.ToPredicateJProperty(),
                    new JProperty("aggregates", GetAggregatesJArray())
                ));
            }
            return new JProperty("content", new JObject(
                _search.ToJProperty(),
                new JProperty("aggregates", GetAggregatesJArray())
            ));
        }

        public async Task<TY> Execute()
        {
            await Task.FromResult(0); //Just to stop warnings for now
            throw new NotImplementedException();
        }

        public new string ToString()
        {
            return GetContent().ToString();
        }
    }

    
}
