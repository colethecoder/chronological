using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Chronological.QueryResults.Events;
using Newtonsoft.Json;
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
        private readonly IEventWebSocketRepository _eventWebSocketRepository;

        internal GenericFluentEventQuery(string queryName, Search search, Limit limit, Environment environment) : this(queryName,search,limit,environment,new EventWebSocketRepository(new WebSocketRepository(environment)))
        {            
        }

        internal GenericFluentEventQuery(string queryName, Search search, Limit limit, Environment environment,
            IEventWebSocketRepository eventWebSocketRepository)
        {
            _queryName = queryName;
            _search = search;
            _limit = limit;
            _environment = environment;
            _eventWebSocketRepository = eventWebSocketRepository;
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
            return new JProperty("content", new JObject(
                _search.ToJProperty(),
                _filter.ToPredicateJProperty(),
                _limit.ToJProperty()
            ));
        }

        public async Task<IEnumerable<T>> ExecuteAsync()
        {
            return await _eventWebSocketRepository.Execute<T>(ToString());
        }
    }
}
