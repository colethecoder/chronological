using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Environment
    {
        internal string DisplayName { get; }
        internal string EnvironmentFqdn { get; }
        internal string EnvironmentId { get; }
        internal string ResourceId { get; }
        internal string AccessToken { get; }

        public Environment(string fqdn, string accessToken)
        {
            EnvironmentFqdn = fqdn;
            AccessToken = accessToken;
        }

        internal Environment(string displayName, string environmentId, string resourceId, string environmentFqdn, string accessToken)
        {
            DisplayName = displayName;
            EnvironmentFqdn = environmentFqdn;
            EnvironmentId = environmentId;
            ResourceId = resourceId;
            AccessToken = accessToken;
        }

        public async Task<Availability> GetAvailabilityAsync(string queryName = "TimeSeriesInsightsAvailabilityQuery")
        {
            DateTime fromAvailabilityTimestamp;
            DateTime toAvailabilityTimestamp;
            {
                Uri uri = new UriBuilder("https", EnvironmentFqdn)
                {
                    Path = "availability",
                    Query = "api-version=2016-12-12"
                }.Uri;
                HttpWebRequest request = WebRequest.CreateHttp(uri);
                request.Method = "GET";
                request.Headers["x-ms-client-application-name"] = queryName;
                request.Headers["Authorization"] = "Bearer " + AccessToken;

                using (WebResponse webResponse = await request.GetResponseAsync())
                using (var sr = new StreamReader(webResponse.GetResponseStream()))
                {
                    string responseJson = await sr.ReadToEndAsync();

                    JObject result = JsonConvert.DeserializeObject<JObject>(responseJson);
                    JObject range = (JObject)result["range"];
                    fromAvailabilityTimestamp = range["from"].Value<DateTime>();
                    toAvailabilityTimestamp = range["to"].Value<DateTime>();
                }
            }

            return new Availability(fromAvailabilityTimestamp, toAvailabilityTimestamp);
        }

        public AggregatesQuery AggregatesQuery(string queryName)
        {
            return new AggregatesQuery(queryName, this);
        }

        public EventsQuery EventsQuery(string queryName)
        {
            return new EventsQuery(queryName, this);
        }

    }
}
