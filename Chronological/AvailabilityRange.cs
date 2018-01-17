using System;

namespace Chronological
{
    public class AvailabilityRange
    {
        public DateTime From { get; }
        public DateTime To { get; }

        public AvailabilityRange(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }
    }
}
