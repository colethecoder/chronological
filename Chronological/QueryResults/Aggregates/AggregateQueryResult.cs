using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Chronological.QueryResults.Aggregates
{

    public class AggregateQueryResult : QueryResult
    {
        public Headers Headers { get; set; }
        [JsonProperty("content")]
        public List<AggregateResult> Content { get; set; }
        public List<object> Warnings { get; set; }
        
    }
}
