using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Breaks
    {
        private readonly string _size;

        private Breaks(string size)
        {
            _size = size;
        }

        public static Breaks InDays(int days)
        {
            var size = $"{days}d";
            return new Breaks(size);
        }

        public static Breaks InHours(int hours)
        {
            var size = $"{hours}h";
            return new Breaks(size);
        }

        public static Breaks InMinutes(int minutes)
        {
            var size = $"{minutes}m";
            return new Breaks(size);
        }

        public static Breaks InSeconds(int seconds)
        {
            var size = $"{seconds}s";
            return new Breaks(size);
        }

        public static Breaks InMilliseconds(int milliseconds)
        {
            var size = $"{milliseconds}ms";
            return new Breaks(size);
        }

        internal JProperty ToJProperty()
        {
            return new JProperty("breaks", new JObject(
                new JProperty("size", _size)));
        }
    }
}