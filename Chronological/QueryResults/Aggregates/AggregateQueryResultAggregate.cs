using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Chronological.QueryResults.Aggregates
{
    public class AggregateQueryResultAggregate
    {
        public List<string> Dimension { get; set; }
        public AggregateQueryResultAggregate Aggregate { get; set; }
        public List<AggregateQueryMeasure> Measures { get; set; }
    }
}
