using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Breaks
    {
        public static NumericBreaks Count(int count)
        {
            return new NumericBreaks(count);
        }

        public static DateBreaks InDays(int days)
        {
            var size = $"{days}d";
            return new DateBreaks(size);
        }

        public static DateBreaks InHours(int hours)
        {
            var size = $"{hours}h";
            return new DateBreaks(size);
        }

        public static DateBreaks InMinutes(int minutes)
        {
            var size = $"{minutes}m";
            return new DateBreaks(size);
        }

        public static DateBreaks InSeconds(int seconds)
        {
            var size = $"{seconds}s";
            return new DateBreaks(size);
        }

        public static DateBreaks InMilliseconds(int milliseconds)
        {
            var size = $"{milliseconds}ms";
            return new DateBreaks(size);
        }

        
    }

    public interface IBreaks
    {
        JProperty ToJProperty();
    }

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