using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronological.QueryResults.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological
{

    internal interface IEventWebSocketRepository
    {

        Task<IEnumerable<T>> Execute<T>(string query);
    }

;   internal class EventWebSocketRepository : IEventWebSocketRepository
    {
        private readonly IWebSocketRepository _webSocketRepository;

        internal EventWebSocketRepository(IWebSocketRepository webSocketRepository)
        {
            _webSocketRepository = webSocketRepository;
        }

        async Task<IEnumerable<T>> IEventWebSocketRepository.Execute<T>(string query)
        {
            return await _webSocketRepository.ReadWebSocketResponseAsync(query, "events", new WebSocketReader<T>(ParseEvents<T>).Read);      
        }

        internal IEnumerable<T> ParseEvents<T>(JToken results)
        {
            // According to samples here: https://github.com/Azure-Samples/Azure-Time-Series-Insights/blob/master/C-%20Hello%20World%20App%20Sample/Program.cs
            // Events should combine all results recevied
            var jArray = (JArray)results["events"];

            var eventResults = jArray.ToObject<List<EventResult>>(new JsonSerializer
            {
                DateParseHandling = DateParseHandling.None
            });

            return new EventQueryResultToTypeMapper().Map<T>(eventResults);
        }

        

    }
}
