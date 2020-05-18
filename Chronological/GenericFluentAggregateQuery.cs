﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class GenericFluentAggregateQuery<T> where T : new()
    {
        private readonly string _queryName;
        private readonly Search _search;
        private readonly Environment _environment;
        private readonly IAggregateApiRepository _webSocketRepository;

        internal GenericFluentAggregateQuery(string queryName, Search search, Environment environment) : this(queryName,
            search, environment, new AggregateApiRepository(new WebSocketRepository(environment)))
        {
        }

        internal GenericFluentAggregateQuery(string queryName, Search search, Environment environment, IAggregateApiRepository webSocketRepository)
        {
            _queryName = queryName;
            _search = search;
            _environment = environment;
            _webSocketRepository = webSocketRepository;
        }

        public GenericFluentAggregateQuery<T, Aggregate<T, TX, TY>> Select<TX, TY>(Func<AggregateBuilder<T>, Aggregate<T, TX, TY>> predicate) 
            => new GenericFluentAggregateQuery<T, Aggregate<T, TX, TY>>(_queryName, _search, predicate(new AggregateBuilder<T>()), _environment, _webSocketRepository);

        public GenericFluentAggregatesQuery<T, Aggregate<T, TX, TY>> Select<TX, TY>(IEnumerable<Func<AggregateBuilder<T>, Aggregate<T, TX, TY>>> predicates)
            => new GenericFluentAggregatesQuery<T, Aggregate<T, TX, TY>>(_queryName, _search, from predicate in predicates select predicate(new AggregateBuilder<T>()), _environment, _webSocketRepository);
    }

    public class GenericFluentAggregateQuery<TX, TY> where TX : new() where TY : IAggregate
    {
        private GenericFluentAggregatesQuery<TX, TY> _multiQuery;

        internal GenericFluentAggregateQuery(string queryName, Search search, TY aggregate, Environment environment, IAggregateApiRepository webSocketRepository)
        {
            _multiQuery = new GenericFluentAggregatesQuery<TX, TY>(queryName, search, new List<TY> {aggregate},
                environment, webSocketRepository);
        }

        public GenericFluentAggregateQuery<TX, TY> Where(Expression<Func<TX, bool>> predicate)
        {
            _multiQuery = _multiQuery.Where(predicate);
            return this;
        }

        public GenericFluentAggregateQuery<TX, TY> Where(Filter filter)
        {
            _multiQuery = _multiQuery.Where(filter);
            return this;
        }

        public GenericFluentAggregateQuery<TX, TY> Where(string predicateString)
        {
            _multiQuery = _multiQuery.Where(predicateString);
            return this;
        }

        public new string ToString() => _multiQuery.ToString();

        public async Task<TY> ExecuteAsync(CancellationToken cancellationToken = default) => 
            (await _multiQuery.ExecuteAsync(cancellationToken)).First();

    }

    public class GenericFluentAggregatesQuery<TX, TY> where TX : new() where TY : IAggregate
    {
        private readonly IEnumerable<TY> _aggregates;
        private Filter _filter;
        private readonly string _queryName;
        private readonly Search _search;
        private readonly Environment _environment;
        private readonly IAggregateApiRepository _webSocketRepository;

        

        internal GenericFluentAggregatesQuery(string queryName, Search search, IEnumerable<TY> aggregates, Environment environment, IAggregateApiRepository webSocketRepository)
        {
            _queryName = queryName;
            _search = search;
            _aggregates = aggregates;
            _environment = environment;
            _webSocketRepository = webSocketRepository;
        }


        public GenericFluentAggregatesQuery<TX, TY> Where(Expression<Func<TX,bool>> predicate)
        {
            _filter = Filter.Create(predicate);
            return this;
        }

        public GenericFluentAggregatesQuery<TX, TY> Where(Filter filter)
        {
            _filter = filter;
            return this;
        }

        public GenericFluentAggregatesQuery<TX, TY> Where(string predicateString)
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

        public async Task<IEnumerable<TY>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var query = ToJObject(_environment.AccessToken);

            return await _webSocketRepository.Execute(query.ToString(), _aggregates, cancellationToken);
            
        }

    }

    
}
