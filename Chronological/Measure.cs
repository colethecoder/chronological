using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Measure
    {
        private readonly string _measureType;
        private readonly Property _property;

        public Measure(string measureType, Property property)
        {
            _measureType = measureType;
            _property = property;
        }

        public static Measure Count()
        {
            return new Measure("count", null);
        }

        public static Measure Sum(Property property)
        {
            return new Measure("sum", property);
        }

        public static Measure Average(Property property)
        {
            return new Measure("avg", property);
        }

        public static Measure Minimum(Property property)
        {
            return new Measure("min", property);
        }

        public static Measure Maximum(Property property)
        {
            return new Measure("max", property);
        }

        internal JProperty ToJProperty()
        {
            switch (_measureType)
            {
                case ("count"):
                    return new JProperty(_measureType, new JObject());
                default:
                    return new JProperty(_measureType, new JObject(_property.ToInputJProperty()));
            }

        }
    }
}