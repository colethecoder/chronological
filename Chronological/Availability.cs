using System;

namespace Chronological
{
    public class Availability
    {
        public DateTime FromDateTime { get; }
        public DateTime ToDateTime { get; }

        public Availability(DateTime from, DateTime to)
        {
            FromDateTime = from;
            ToDateTime = to;
        }
    }
}
