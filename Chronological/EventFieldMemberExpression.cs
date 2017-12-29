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
        public readonly string EventFieldName;
        public readonly DataType EventFieldDataType;

        public EventFieldMemberExpression(MemberExpression memberExpression)
        {
            var attributes = memberExpression?.Member.GetCustomAttributes(typeof(ChronologicalEventFieldAttribute), true);
            var attribute = (ChronologicalEventFieldAttribute)attributes?.FirstOrDefault();
            if (attribute != null)
            {
                EventFieldName = EscapeEventFieldName(attribute.EventFieldName);
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
            if (eventFieldName.StartsWith("[") && eventFieldName.EndsWith("]"))
            {
                return eventFieldName;
            }

            return $"[{eventFieldName}]";
        }
    }
}
