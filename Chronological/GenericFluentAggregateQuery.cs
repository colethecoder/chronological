using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class GenericFluentAggregateQuery<T> where T : new()
    {        

        public GenericFluentAggregateQuery<T, Aggregate<T,TX,TY>> Select<TX,TY>(Func<AggregateBuilder<T>,Aggregate<T,TX,TY>> predicate)
        {
            var aggregate = predicate(new AggregateBuilder<T>());

            return new GenericFluentAggregateQuery<T, Aggregate<T,TX,TY>>();
        }
        
    }

    public class GenericFluentAggregateQuery<TX, TY> where TX : new()
    {
        public GenericFluentAggregateQuery<TX, TY> Where(Expression<Func<TX,bool>> predicate)
        {
            var binaryExpression = (BinaryExpression)predicate.Body;
            var leftSide = binaryExpression.Left;
            var rightSide = binaryExpression.Right;
            var comparison = binaryExpression.NodeType;


            throw new NotImplementedException();
        }

        public async Task<TY> Execute()
        {
            throw new NotImplementedException();
        }
    }

    public class GenericFluentAggregateQuery : AggregateQuery
    {
        private Search _search;
        private List<Aggregate> _aggregates;
        private Filter _filter;

        internal GenericFluentAggregateQuery(string queryName, Environment environment) : base(queryName, environment)
        {
        }

        public GenericFluentAggregateQuery WithSearch(Search search)
        {
            _search = search;
            return this;
        }

        public GenericFluentAggregateQuery WithAggregate(Aggregate aggregate)
        {
            if (_aggregates == null)
            {
                _aggregates = new List<Aggregate>();
            }
            _aggregates.Add(aggregate);
            return this;
        }

        public GenericFluentAggregateQuery Where(Filter filter)
        {
            _filter = filter;
            return this;
        }

        public GenericFluentAggregateQuery Where(string predicateString)
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
    }
}
