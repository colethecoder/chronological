using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    internal interface IWebSocketRepository
    {        
        Task<IReadOnlyList<JToken>> ReadWebSocketResponseAsync(string query, string resourcePath);
    }
}
