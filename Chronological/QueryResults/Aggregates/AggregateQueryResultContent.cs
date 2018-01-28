using System.Collections.Generic;

namespace Chronological.QueryResults.Aggregates
{
    public class AggregateQueryResultContent
    {
        //[JsonProperty("aggregates")]
        //[JsonConverter(typeof(AggregateQueryMeasureJsonConverter))]
        public List<AggregateQueryResultAggregate> Aggregates { get; set; }
    }
}
