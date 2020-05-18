using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class GenericFluentEventQuery<T> where T : new()
    {
        private readonly string _queryName;
        private readonly Search _search;
        private Filter _filter;
        private readonly Limit _limit;
        private readonly Environment _environment;
        private readonly IEventApiRepository _eventApiRepository;

        internal GenericFluentEventQuery(string queryName, Search search, Limit limit, Environment environment) 
            : this(queryName,search,limit,environment,new EventApiRepository(new WebSocketRepository(environment)))
        {            
        }

        internal GenericFluentEventQuery(string queryName, Search search, Limit limit, Environment environment,
            IEventApiRepository eventApiRepository)
        {
            _queryName = queryName;
            _search = search;
            _limit = limit;
            _environment = environment;
            _eventApiRepository = eventApiRepository;
        }

        public GenericFluentEventQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            _filter = Filter.Create(predicate);
            return this;
        }

        public GenericFluentEventQuery<T> Where(Filter filter)
        {
            _filter = filter;
            return this;
        }

        public GenericFluentEventQuery<T> Where(string predicateString)
        {
            _filter = Filter.FromString(predicateString);
            return this;
        }

        protected JProperty GetContent()
        {
            if (_filter != null)
            {
                return new JProperty("content", new JObject(
                    _search.ToJProperty(),
                    _filter.ToPredicateJProperty(),
                    _limit.ToJProperty()
                ));
            }
            return new JProperty("content", new JObject(
                _search.ToJProperty(),
                _limit.ToJProperty()
            ));
        }

        private JProperty GetHeaders(string accessToken)
        {
            return new JProperty("headers", new JObject(
                new JProperty("x-ms-client-application-name", _queryName),
                new JProperty("Authorization", "Bearer " + accessToken)));
        }

        private JObject ToJObject(string accessToken)
        {
            return new JObject(
                GetHeaders(accessToken),
                GetContent()
            );
        }

        public new string ToString()
        {
            return ToJObject(_environment.AccessToken).ToString();
        }

        public async Task<IEnumerable<T>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return await _eventApiRepository.Execute<T>(ToString(), cancellationToken);
        }
    }
}
