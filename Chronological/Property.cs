using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Property<T> where T : new()
    {
        internal static Property Create<TY>(Expression<Func<TY, T>> property)
        {
            var eventFieldMemberExpression = new EventFieldMemberExpression(property.Body as MemberExpression);
            return Property.Custom(eventFieldMemberExpression.UnescapedEventFieldName, eventFieldMemberExpression.EventFieldDataType);
        }


    }

    public class Property
    {
        internal readonly bool _isBuiltIn;
        internal readonly string _propertyName;
        internal readonly DataType _dataType;

        internal Property(bool isBuiltIn, string propertyName, DataType dataType = null)
        {
            _isBuiltIn = isBuiltIn;
            _propertyName = propertyName;
            _dataType = dataType;
        }

        public static Property TimeStamp => new Property(true, @"$ts");

        public static Property EventSourceName => new Property(true, @"$esn");
        public static Property Custom(string name, DataType type = null)
        {
            return new Property(false, name, type);
        }

        internal JProperty ToInputJProperty()
        {
            return _isBuiltIn ? ToBuiltInJProperty("input") : ToCustomJProperty("input");
        }

        internal JProperty ToLeftJProperty()
        {
            return _isBuiltIn ? ToBuiltInJProperty("left") : ToCustomJProperty("left");
        }

        internal JProperty ToRightJProperty()
        {
            return new JProperty("right", _propertyName);
        }

        private JProperty ToBuiltInJProperty(string outerName)
        {
            return new JProperty(outerName, new JObject(
                new JProperty("builtInProperty", _propertyName)));
        }

        private JProperty ToCustomJProperty(string outerName)
        {
            if (_dataType == null)
            {
                return new JProperty(outerName, new JObject(new JProperty("property", _propertyName)));
            }
            return new JProperty(outerName, new JObject(
                new JProperty("property", _propertyName),
                _dataType.ToJProperty()));
        }
    }
}
