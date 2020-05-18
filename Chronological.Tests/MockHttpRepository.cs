using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological.Tests
{
    internal class MockHttpRepository : IWebRequestRepository
    {
        private readonly List<string> _results;

        public MockHttpRepository(string result) : this(new List<string> { result })
        {
        }

        public MockHttpRepository(List<string> results)
        {
            _results = results;
        }

        async Task<IReadOnlyList<JToken>> IWebRequestRepository.ExecuteRequestAsync(string query, string resourcePath, CancellationToken cancellationToken)
        {
            if ("aggregates".Equals(resourcePath, StringComparison.OrdinalIgnoreCase))
            {
                return new List<JToken>() { JToken.Parse(_results.First())["aggregates"] };
            }

            return new List<JToken> { JToken.Parse(_results.First()) };
        }
    }
}
