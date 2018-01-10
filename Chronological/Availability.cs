using System;
using System.Collections.Generic;

namespace Chronological
{
    public class Availability
    {
        public AvailabilityRange Range { get; }
        public TimeSpan IntervalSize { get; }

        public Dictionary<DateTime, int> Distribution { get; }

        public Availability(AvailabilityRange range, string intervalSize, Dictionary<DateTime, int> distribution)
        {
            Range = range;
            IntervalSize = ParseIntervalFromString(intervalSize);
            Distribution = distribution;
        }

        internal TimeSpan ParseIntervalFromString(string interval)
        {
            if (interval.EndsWith("h"))
            {
                var hours = int.Parse(interval.Substring(0, interval.Length - 1));
                return TimeSpan.FromHours(hours);
            }
            return (default);
        }
    }

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
