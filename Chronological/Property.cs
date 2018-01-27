using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Property
    {
        private readonly bool _isBuiltIn;
        public readonly string Name;
        public readonly DataType DataType;

        internal Property(bool isBuiltIn, string propertyName, DataType dataType = null)
        {
            _isBuiltIn = isBuiltIn;
            Name = propertyName;
            DataType = dataType;
        }

        internal static Property EventTimeStamp => BuiltIn(Chronological.BuiltIn.EventTimeStamp);

        internal static Property EventSourceName => BuiltIn(Chronological.BuiltIn.EventSourceName);

        internal static Property BuiltIn(string name) => new Property(true, name);

        internal static Property Custom(string name, DataType type = null)
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
            return new JProperty("right", Name);
        }

        private JProperty ToBuiltInJProperty(string outerName)
        {
            return new JProperty(outerName, new JObject(
                new JProperty("builtInProperty", Name)));
        }

        private JProperty ToCustomJProperty(string outerName)
        {
            if (DataType == null)
            {
                return new JProperty(outerName, new JObject(new JProperty("property", Name)));
            }
            return new JProperty(outerName, new JObject(
                new JProperty("property", Name),
                DataType.ToJProperty()));
        }
    }
}
