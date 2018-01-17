using System;

namespace Chronological
{
    public class ChronologicalEventFieldAttribute : Attribute
    {
        public string EventFieldName { get; }

        public ChronologicalEventFieldAttribute(string eventFieldName)
        {
            EventFieldName = eventFieldName;
        }
    }
}
