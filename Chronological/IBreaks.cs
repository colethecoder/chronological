using Newtonsoft.Json.Linq;

namespace Chronological
{
    public interface IBreaks
    {
        JProperty ToJProperty();
    }
}
