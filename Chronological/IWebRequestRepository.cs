using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    internal interface IWebRequestRepository
    {        
        Task<IReadOnlyList<JToken>> ExecuteRequestAsync(string query, string resourcePath, CancellationToken cancellationToken = default);
    }
}
