using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Chronological.Events
{
    public class Headers
    {
        [JsonProperty("x-ms-request-id")]
        public string RequestId { get; set; }
    }

    public class Property
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Schema
    {
        public int Rid { get; set; }
        [JsonProperty("$esn")]
        public string Esn { get; set; }
        public List<Property> Properties { get; set; }
    }

    public class Event
    {
        public Schema schema { get; set; }
        [JsonProperty("$ts")]
        public string Ts { get; set; }
        public List<string> Values { get; set; }
        public int? SchemaRid { get; set; }
    }

    public class Content
    {
        public List<Event> Events { get; set; }
    }

    public class EventQueryResults
    {
        public Headers Headers { get; set; }
        public Content Content { get; set; }
        public List<object> Warnings { get; set; }
        public double PercentCompleted { get; set; }
    }
}
