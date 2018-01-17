using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class DateBreaks : IBreaks
    {
        private readonly string _size;

        internal DateBreaks(string size)
        {
            _size = size;
        }

        public JProperty ToJProperty()
        {
            return new JProperty("breaks", new JObject(
                new JProperty("size", _size)));
        }
    }
}
