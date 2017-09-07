using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class StringAggregateQuery : AggregateQuery
    {

        private readonly string _query;

        internal StringAggregateQuery(string queryName, string query, Environment environment) : base(queryName, environment)
        {
            _query = query;
        }

        protected override JProperty GetContent()
        {
            return new JProperty("content", JObject.Parse(_query));
        }
    }
}
