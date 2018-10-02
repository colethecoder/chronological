using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological.Tests
{
    internal class MockWebSocketRepository : IWebSocketRepository
    {
        private readonly List<string> _results;

        public MockWebSocketRepository(string result) : this(new List<string> { result })
        {
        }

        public MockWebSocketRepository(List<string> results)
        {
            _results = results;
        }

        async Task<IReadOnlyList<JToken>> IWebSocketRepository.ReadWebSocketResponseAsync(string query, string resourcePath, CancellationToken cancellationToken)
        {
            return new List<JToken> { JToken.Parse(_results.First())["content"] };
        }
    }
}
