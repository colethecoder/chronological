using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Connection
    {
        private readonly string _applicationClientId;

        private readonly string _applicationClientSecret;

        private readonly string _tenant;

        public Connection(string clientId, string secret, string tenant)
        {
            _applicationClientId = clientId;
            _applicationClientSecret = secret;
            _tenant = tenant;
        }

        public async Task<Environment> GetEnvironmentAsync(string environmentFqdn)
        {
            return new Environment(environmentFqdn, await GetAccessTokenAsync());
        }

        public async Task<IEnumerable<Environment>> GetEnvironmentsAsync()
        {
            var environments = new List<Environment>();

            string accessToken = await GetAccessTokenAsync();

            Uri uri = new UriBuilder("https", "api.timeseries.azure.com")
            {
                Path = "environments",
                Query = "api-version=2016-12-12"
            }.Uri;
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.Method = "GET";
            request.Headers["x-ms-client-application-name"] = "TimeSeriesInsightsQuerySample";
            request.Headers["Authorization"] = "Bearer " + accessToken;

            using (WebResponse webResponse = await request.GetResponseAsync())
            using (var sr = new StreamReader(webResponse.GetResponseStream()))
            {
                string responseJson = await sr.ReadToEndAsync();

                JObject result = JsonConvert.DeserializeObject<JObject>(responseJson);
                JArray environmentsList = (JArray)result["environments"];
                foreach (var environment in environmentsList)
                {
                    var environmentJObject = (JObject)environment;
                    var environmentFqdn = environmentJObject["environmentFqdn"].Value<string>();
                    var displayName = environmentJObject["displayName"].Value<string>();
                    var resourceId = environmentJObject["resourceId"].Value<string>();
                    var environmentId = environmentJObject["environmentId"].Value<string>();
                    environments.Add(new Environment(displayName, environmentId, resourceId, environmentFqdn, accessToken));
                }
            }

            return environments;
        }
        public async Task<string> GetAccessTokenAsync()
        {

            var authenticationContext = new AuthenticationContext(
                $"https://login.windows.net/{_tenant}",
                TokenCache.DefaultShared);


                AuthenticationResult token = await authenticationContext.AcquireTokenAsync(
                    resource: "https://api.timeseries.azure.com/",
                    clientCredential: new ClientCredential(
                        clientId: _applicationClientId,
                        clientSecret: _applicationClientSecret));
                return token.AccessToken;
        }

    }
}
