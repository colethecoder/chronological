using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chronological.QueryResults.Events
{
    public class EventResult
    {
        public Schema schema { get; set; }
        [JsonProperty("$ts")]
        public string Timestamp { get; set; }
        public List<string> Values { get; set; }
        public int? SchemaRid { get; set; }
    }
}
