using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Property<T> where T : new()
    {
        internal static Property Create<TY>(Expression<Func<TY, T>> property) where TY : new()
        {
            var memberExpression = property.Body as MemberExpression;
            var attributes = memberExpression?.Member.GetCustomAttributes(typeof(ChronologicalEventFieldAttribute), true);
            var attribute = (ChronologicalEventFieldAttribute)attributes?.FirstOrDefault();
            if (attribute != null)
            {
                return Property.Custom(attribute.EventFieldName, DataType.FromType(new T()));
            }
            //TODO deal with problems
            throw new NotImplementedException();
        }


    }

    public class Property
    {
        private readonly bool _isBuiltIn;
        private readonly string _propertyName;
        private readonly DataType _dataType;

        private Property(bool isBuiltIn, string propertyName, DataType dataType = null)
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