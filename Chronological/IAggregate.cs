using Newtonsoft.Json.Linq;

namespace Chronological
{
    public interface IAggregate
    {
        JObject ToJObject();
    }

    
}
