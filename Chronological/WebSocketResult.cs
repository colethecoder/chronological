using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Chronological.Exceptions;
using Chronological.QueryResults;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class WebSocketResult<T>
    {
        public class WebSocketSuccess : WebSocketResult<T>
        {
            public bool Continue { get; }
            public IEnumerable<T> Results { get; }

            public WebSocketSuccess(IEnumerable<T> results, bool cont)
            {
                Results = results;
                Continue = cont;
            }
        }

        public class WebSocketFailure : WebSocketResult<T>
        {
            public Exception Exception { get; }

            public WebSocketFailure(Exception ex)
            {
                Exception = ex;
            }
        }

    }

    public class WebSocketReader<T>
    {
        private Func<JToken, IEnumerable<T>> _parser { get; }

        public WebSocketReader(Func<JToken, IEnumerable<T>> parser)
        {
            _parser = parser;
        }

        public WebSocketResult<T> Read(StreamReader streamReader)
        {
            var message = streamReader.ReadToEnd();

            JObject messageObj = JsonConvert.DeserializeObject<JObject>(message, new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None
            });

            if (messageObj["error"] != null)
            {
                var error = messageObj["error"].ToObject<ErrorResult>();

                return new WebSocketResult<T>
                            .WebSocketFailure(
                                new ErrorToExceptionConverter()
                                    .ConvertTimeSeriesErrorToException(error));
            }

            // Actual response contents is wrapped into "content" object.
            var results = _parser(messageObj["content"]);

            // Stop reading if 100% of completeness is reached.
            var cont = !(messageObj["percentCompleted"] != null &&
                Math.Abs((double)messageObj["percentCompleted"] - 100d) < 0.01);

            return new WebSocketResult<T>
                            .WebSocketSuccess(results, cont);
        }
    }
}
