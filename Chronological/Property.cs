using Newtonsoft.Json.Linq;

namespace Chronological
{
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

        public static Property TimeSeries => new Property(true, @"$ts");

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
                return new JProperty(outerName, new JObject(new JProperty("property", "data.value")));
            }
            return new JProperty(outerName, new JObject(
                new JProperty("property", _propertyName),
                _dataType.ToJProperty()));
        }
    }
}