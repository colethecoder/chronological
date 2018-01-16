using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chronological
{
    internal interface IAggregateWebSocketRepository
    {
        Task<IEnumerable<T>> Execute<T>(string query, IEnumerable<T> aggregates);
    }
}