using System;
using System.Linq.Expressions;

namespace Chronological
{
    public class Property<T>
    {
        internal static Property Create<TY>(Expression<Func<TY, T>> property)
        {
            var eventFieldMemberExpression = new EventFieldMemberExpression(property.Body as MemberExpression);
            if (BuiltIn.All().Contains(eventFieldMemberExpression.UnescapedEventFieldName))
            {
                return Property.BuiltIn(eventFieldMemberExpression.UnescapedEventFieldName);
            }
            return Property.Custom(eventFieldMemberExpression.UnescapedEventFieldName, eventFieldMemberExpression.EventFieldDataType);
        }
    }
}
