using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Chronological.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class HttpRepository : IWebRequestRepository
    {
        private readonly Environment _environment;
        private readonly IErrorToExceptionConverter _errorToExceptionConverter;

        public HttpRepository(Environment environment) : this(environment, new ErrorToExceptionConverter())
        {
        }

        internal HttpRepository(Environment environment, IErrorToExceptionConverter errorToExceptionConverter)
        {
            _environment = environment;
            _errorToExceptionConverter = errorToExceptionConverter;
        }

        async Task<IReadOnlyList<JToken>> IWebRequestRepository.ExecuteRequestAsync(string query, string resourcePath, CancellationToken cancellationToken)
        {
            var httpQuery = JToken.Parse(query)["content"].ToString();

            var request = CreateHttpsWebRequest(_environment.EnvironmentFqdn, "POST", resourcePath, _environment.AccessToken, new[] { "timeout=PT20S" });
            await WriteRequestStreamAsync(request, httpQuery);
            var responseContent = await GetResponseAsync(request);

            if ("aggregates".Equals(resourcePath, StringComparison.OrdinalIgnoreCase))
            {
                var results = new List<JToken>() { responseContent };
                return new List<JToken>() { results.First()["aggregates"] };
            }

            return new List<JToken>() { responseContent };
        }

        private static HttpWebRequest CreateHttpsWebRequest(string host, string method, string path, string accessToken, string[] queryArgs = null)
        {
            string query = "api-version=2016-12-12";
            if (queryArgs != null && queryArgs.Any())
            {
                query += "&" + String.Join("&", queryArgs);
            }

            Uri uri = new UriBuilder("https", host)
            {
                Path = path,
                Query = query
            }.Uri;
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.Method = method;
            request.Headers["Authorization"] = $"Bearer {accessToken}";
            return request;
        }

        private static async Task WriteRequestStreamAsync(HttpWebRequest request, string inputPayload)
        {
            using (var stream = await request.GetRequestStreamAsync())
            using (var streamWriter = new StreamWriter(stream))
            {
                await streamWriter.WriteAsync(inputPayload);
                await streamWriter.FlushAsync();
                //streamWriter.Close();
            }
        }

        private static async Task<JToken> GetResponseAsync(HttpWebRequest request)
        {
            try
            {
                using (WebResponse webResponse = await request.GetResponseAsync())
                using (var sr = new StreamReader(webResponse.GetResponseStream()))
                {
                    string result = await sr.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<JToken>(result);
                }
            }
            catch (WebException e)
            {
                if (e.Response == null)
                    throw e;

                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        throw new Exception(reader.ReadToEnd(), e);
                    }
                }
            }
        }
    }
}
