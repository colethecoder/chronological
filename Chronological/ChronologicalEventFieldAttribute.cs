using System;
using System.Collections.Generic;
using System.Text;

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
