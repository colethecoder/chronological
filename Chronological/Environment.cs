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

                var result = JsonConvert.DeserializeObject<Availability>(responseJson);
                return result;
            }

        }

        public GenericFluentAggregateQuery<T> AggregateQuery<T>(string queryName, Search search) where T: new()
        {
            return new GenericFluentAggregateQuery<T>(queryName, search, this);
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
