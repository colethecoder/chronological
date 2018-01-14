using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class NumericBreaks : IBreaks
    {
        private readonly int _count;

        internal NumericBreaks(int count)
        {
            _count = count;
        }

        public JProperty ToJProperty()
        {
            return new JProperty("breaks", new JObject(
                new JProperty("count", _count)));
        }
    }
}
