using Newtonsoft.Json.Linq;

namespace Chronological
{
    public interface IDimension
    {
        JProperty ToJProperty();

    }
}
