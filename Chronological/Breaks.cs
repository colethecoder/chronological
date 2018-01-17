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
}
