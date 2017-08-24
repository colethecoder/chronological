using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Chronological.QueryResults.Events
{

    public class EventQueryResult
    {
        public Headers Headers { get; set; }
        public EventQueryResultContent Content { get; set; }
        public List<object> Warnings { get; set; }
        public double PercentCompleted { get; set; }
    }
}
