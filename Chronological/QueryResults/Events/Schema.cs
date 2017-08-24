using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chronological.QueryResults.Events
{
    public class Schema
    {
        public int Rid { get; set; }
        [JsonProperty("$esn")]
        public string Esn { get; set; }
        public List<SchemaProperty> Properties { get; set; }
    }
}
