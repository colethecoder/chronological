using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Chronological
{
    public class EventFieldMemberExpression
    {
        public readonly string UnescapedEventFieldName;
        public readonly string EscapedEventFieldName;
        public readonly DataType EventFieldDataType;

        public EventFieldMemberExpression(MemberExpression memberExpression)
        {
            var attributes = memberExpression?.Member.GetCustomAttributes(typeof(ChronologicalEventFieldAttribute), true);
            var attribute = (ChronologicalEventFieldAttribute)attributes?.FirstOrDefault();
            if (attribute != null)
            {
                EscapedEventFieldName = EscapeEventFieldName(attribute.EventFieldName);
                UnescapedEventFieldName = UnescapeEventFieldName(attribute.EventFieldName);
                EventFieldDataType = DataType.FromType(memberExpression.Type);
            }
            else
            {
                //TODO deal with problems
                throw new NotImplementedException();
            }
        }

        private string EscapeEventFieldName(string eventFieldName)
        {
            if (BuiltIn.All().Contains(eventFieldName))
            {
                return eventFieldName;
            }

            if (eventFieldName.StartsWith("[") && eventFieldName.EndsWith("]"))
            {
                return eventFieldName;
            }

            return $"[{eventFieldName}]";
        }

        private string UnescapeEventFieldName(string eventFieldName)
        {
            if (BuiltIn.All().Contains(eventFieldName))
            {
                return eventFieldName;
            }

            if (eventFieldName.StartsWith("[") && eventFieldName.EndsWith("]"))
            {
                return eventFieldName.Substring(1, EscapedEventFieldName.Length - 2);
            }

            return eventFieldName;
        }
    }
}
