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

        public async Task<EnvironmentMetadata> GetMetadataAsync(DateTime from, DateTime to, string queryName = "TimeSeriesInsightsMetadataQuery")
        {
            Uri uri = new UriBuilder("https", EnvironmentFqdn)
            {
                Path = "metadata",
                Query = "api-version=2016-12-12"
            }.Uri;

            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.Method = "POST";
            request.Headers["x-ms-client-application-name"] = queryName;
            request.Headers["Authorization"] = "Bearer " + AccessToken;

            var requestStream = await request.GetRequestStreamAsync();

            using (var streamWriter = new StreamWriter(requestStream))
            {
                string json = "{\"searchSpan\": { \"from\": { \"dateTime\":\"2017-08-01T00:00:00.000Z\"},  \"to\": { \"dateTime\":\"2018-08-31T00:00:00.000Z\"} } }";

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            using (WebResponse webResponse = await request.GetResponseAsync())
            using (var sr = new StreamReader(webResponse.GetResponseStream()))
            {
                string responseJson = await sr.ReadToEndAsync();

                var json = JsonConvert.DeserializeObject<JToken>(responseJson);

                var result = new EnvironmentMetadata((JArray)json["properties"]);

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

        public GenericFluentEventQuery<T> EventQuery<T>(string queryName, Search search, Limit limit) where T:new()
        {
            return new GenericFluentEventQuery<T>(queryName, search, limit, this);
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
