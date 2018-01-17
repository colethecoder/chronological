using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    internal class AggregateWebSocketRepository : IAggregateWebSocketRepository
    {
        private readonly IWebSocketRepository _webSocketRepository;

        internal AggregateWebSocketRepository(IWebSocketRepository webSocketRepository)
        {
            _webSocketRepository = webSocketRepository;
        }

        async Task<IEnumerable<T>> IAggregateWebSocketRepository.Execute<T>(string query, IEnumerable<T> aggregates)
        {
            var executionResults = new List<T>();
            
            var results = await _webSocketRepository.ReadWebSocketResponseAsync(query, "aggregates");

            // According to samples here: https://github.com/Azure-Samples/Azure-Time-Series-Insights/blob/master/C-%20Hello%20World%20App%20Sample/Program.cs
            // Aggregates should only use the final result set
            var jArray = (JArray)results.First();

            foreach (var aggregate in aggregates.Select((x, y) => new { x, y }))
            {
                var typedAggregate = (IInternalAggregate)aggregate.x;
                var aggregateJObject = (JObject)jArray[aggregate.y];

                var populatedAggregate = typedAggregate.GetPopulatedAggregate(aggregateJObject, x => x);
                executionResults.Add((T)populatedAggregate);
            }

            return executionResults;
        }
    }
}
