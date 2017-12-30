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
            if (BuiltIn.All().Contains(eventFieldMemberExpression.UnescapedEventFieldName))
            {
                return Property.BuiltIn(eventFieldMemberExpression.UnescapedEventFieldName);
            }
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

        public static Property EventTimeStamp => BuiltIn(Chronological.BuiltIn.EventTimeStamp);

        public static Property EventSourceName => BuiltIn(Chronological.BuiltIn.EventSourceName);

        internal static Property BuiltIn(string name) => new Property(true, name);

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
