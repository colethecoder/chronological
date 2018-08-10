using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    internal interface IWebSocketRepository
    {        
        Task<IEnumerable<T>> ReadWebSocketResponseAsync<T>(string query, string resourcePath, Func<StreamReader, WebSocketResult<T>> parseFunc);
    }
}
