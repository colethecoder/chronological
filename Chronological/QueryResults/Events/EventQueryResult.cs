using System.Collections.Generic;

namespace Chronological.QueryResults.Events
{

    public class EventQueryResult : QueryResult
    {
        public Headers Headers { get; set; }
        public EventQueryResultContent Content { get; set; }
        public List<object> Warnings { get; set; }
    }
}
