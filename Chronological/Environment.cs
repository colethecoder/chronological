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
        public string DisplayName { get; }
        public string EnvironmentFqdn { get; }
        public string EnvironmentId { get; }
        public string ResourceId { get; }
        public string AccessToken { get; }

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

                    //TODO - response looks like below, should add other values in 
                    //{
                    //    "range": {
                    //        "from": "2016-08-01T01:02:03Z",
                    //        "to": "2016-08-31T03:04:05Z"
                    //    },
                    //    "intervalSize": "1h",
                    //    "distribution": {
                    //        "2016-08-01T00:00:00Z": 123,
                    //        "2016-08-31T03:00:00Z": 345
                    //    }
                    //}

                    JObject result = JsonConvert.DeserializeObject<JObject>(responseJson);
                    JObject range = (JObject)result["range"];
                    fromAvailabilityTimestamp = range["from"].Value<DateTime>();
                    toAvailabilityTimestamp = range["to"].Value<DateTime>();
                }
            }

            return new Availability(fromAvailabilityTimestamp, toAvailabilityTimestamp);
        }

        public GenericFluentAggregateQuery<T> AggregateQuery<T>(string queryName, Search search) where T: new()
        {
            return new GenericFluentAggregateQuery<T>(queryName, search);
        }        

        public StringAggregateQuery AggregateQuery(string queryName, string query)
        {
            return new StringAggregateQuery(queryName, query, this);
        }

        public FluentEventQuery EventQuery(string queryName)
        {
            return new FluentEventQuery(queryName, this);
        }

        /// <summary>
        /// Create a new EventQuery from a string of json
        /// </summary>
        /// <param name="queryName">Name of the query</param>
        /// <param name="query">Json query string</param>
        /// <returns></returns>
        public StringEventQuery EventQuery(string queryName, string query)
        {
            return new StringEventQuery(queryName, query, this);
        }

    }
}
