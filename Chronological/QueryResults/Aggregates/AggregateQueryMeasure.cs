using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Chronological.QueryResults.Aggregates
{
    [JsonConverter(typeof(AggregateQueryMeasureJsonConverter))]
    public class AggregateQueryMeasure
    {
        public List<AggregateQueryMeasure> Measures { get; set; }

        public double Measure { get; set; }
    }
}
