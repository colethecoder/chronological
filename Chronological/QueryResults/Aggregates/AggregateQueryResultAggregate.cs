using System.Collections.Generic;

namespace Chronological.QueryResults.Aggregates
{
    public class AggregateQueryResultAggregate
    {
        public List<string> Dimension { get; set; }
        public AggregateQueryResultAggregate Aggregate { get; set; }
        public List<AggregateQueryMeasure> Measures { get; set; }
    }
}
