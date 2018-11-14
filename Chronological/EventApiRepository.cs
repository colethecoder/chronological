using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chronological.QueryResults.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological
{

    internal interface IEventApiRepository
    {

        Task<IEnumerable<T>> Execute<T>(string query, CancellationToken cancellationToken = default);
    }

;   internal class EventApiRepository : IEventApiRepository
    {
        private readonly IWebRequestRepository _webRequestRepository;

        internal EventApiRepository(IWebRequestRepository webRequestRepository)
        {
            _webRequestRepository = webRequestRepository;
        }

        async Task<IEnumerable<T>> IEventApiRepository.Execute<T>(string query, CancellationToken cancellationToken)
        {
            var results = await _webRequestRepository.ExecuteRequestAsync(query, "events", cancellationToken);

            // According to samples here: https://github.com/Azure-Samples/Azure-Time-Series-Insights/blob/master/C-%20Hello%20World%20App%20Sample/Program.cs
            // Events should combine all results recevied
            var jArray = new JArray(results.SelectMany(x => (JArray)x["events"]));

            var eventResults = jArray.ToObject<List<EventResult>>(new JsonSerializer
            {
                DateParseHandling = DateParseHandling.None
            });

            return new EventQueryResultToTypeMapper().Map<T>(eventResults);
                        
        }

        

    }
}
