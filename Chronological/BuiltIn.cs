using System.Collections.Generic;

namespace Chronological
{
    public static class BuiltIn
    {
        public const string EventTimeStamp = "$ts";
        public const string EventSourceName = "$esn";

        internal static List<string> All() => new List<string>
        {
            EventTimeStamp,
            EventSourceName
        };

        public static class Function
        {
            public const string UtcNow = "utcNow()";
        }
    }
}
