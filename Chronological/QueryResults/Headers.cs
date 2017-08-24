using Newtonsoft.Json;

namespace Chronological.QueryResults
{
    public class Headers
    {
        [JsonProperty("x-ms-request-id")]
        public string RequestId { get; set; }
    }
}
