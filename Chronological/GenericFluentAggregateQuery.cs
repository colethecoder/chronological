﻿using System;
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
        private readonly Environment _environment;
        private readonly IWebSocketRepository _webSocketRepository;

        internal GenericFluentAggregateQuery(string queryName, Search search, Environment environment) : this(queryName,
            search, environment, new WebSocketRepository(environment))
        {
        }

        internal GenericFluentAggregateQuery(string queryName, Search search, Environment environment, IWebSocketRepository webSocketRepository)
        {
            _queryName = queryName;
            _search = search;
            _environment = environment;
            _webSocketRepository = webSocketRepository;
        }

        public GenericFluentAggregateQuery<T, Aggregate<T, TX, TY>> Select<TX, TY>(Func<AggregateBuilder<T>, Aggregate<T, TX, TY>> predicate) 
            => new GenericFluentAggregateQuery<T, Aggregate<T, TX, TY>>(_queryName, _search, predicate(new AggregateBuilder<T>()), _environment, _webSocketRepository);

        public GenericFluentAggregateQuery<T, Aggregate<T, TX, TY>> Select<TX, TY>(IEnumerable<Func<AggregateBuilder<T>, Aggregate<T, TX, TY>>> predicates)
            => new GenericFluentAggregateQuery<T, Aggregate<T, TX, TY>>(_queryName, _search, from predicate in predicates select predicate(new AggregateBuilder<T>()), _environment, _webSocketRepository);
    }

    public class GenericFluentAggregateQuery<TX, TY> where TX : new() where TY : IAggregate
    {
        private readonly IEnumerable<TY> _aggregates;
        private Filter _filter;
        private readonly string _queryName;
        private readonly Search _search;
        private readonly Environment _environment;
        private readonly IWebSocketRepository _webSocketRepository;

        internal GenericFluentAggregateQuery(string queryName, Search search, TY aggregate, Environment environment, IWebSocketRepository webSocketRepository) : this(queryName, search,
            new List<TY>
            {
                aggregate
            }, environment, webSocketRepository)
        {
        }

        internal GenericFluentAggregateQuery(string queryName, Search search, IEnumerable<TY> aggregates, Environment environment, IWebSocketRepository webSocketRepository)
        {
            _queryName = queryName;
            _search = search;
            _aggregates = aggregates;
            _environment = environment;
            _webSocketRepository = webSocketRepository;
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

        public new string ToString()
        {
            return ToJObject(_environment.AccessToken).ToString();
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

        public async Task<IEnumerable<TY>> Execute()
        {
            var executionResults = new List<TY>();
            var query = ToJObject(_environment.AccessToken);

            var results = await _webSocketRepository.QueryWebSocket(query.ToString(), "aggregates");

            var aggregates = _aggregates;

            //TODO: figure out what to do in situations were there is more than one result
            var jObject = JObject.Parse(results.First());

            foreach (var aggregate in aggregates.Select((x,y) => new {x,y}))
            {
                var typedAggregate = (IAggregate)aggregate.x;
                var aggregateJObject = (JObject)jObject["content"][aggregate.y];

                typedAggregate.Populate(aggregateJObject);
                executionResults.Add((TY)typedAggregate);
            }

            return executionResults;
        }

    }

    
}
