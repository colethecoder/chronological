using System;
using System.Collections.Generic;
using System.Text;

namespace Chronological.QueryResults.Aggregates
{
    public class AggregateResult
    {
        public List<string> Dimension { get; set; }
        public List<AggregateResult> Aggregates { get; set; }

        public List<List<AggregateQueryMeasure>> Measures { get; set; }
    }
}
