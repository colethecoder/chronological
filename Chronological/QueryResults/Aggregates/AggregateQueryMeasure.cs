using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chronological.QueryResults.Aggregates
{
    [JsonConverter(typeof(AggregateQueryResultAggregateJsonConverter))]
    public class AggregateQueryMeasure
    {
        
        public List<AggregateQueryMeasure> Measures { get; set; }

        public double? Measure { get; set; }
    }
}
