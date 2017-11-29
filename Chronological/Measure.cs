using System;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Measure<T>
    {
    }

    public class Measure
    {
        private readonly string _measureType;
        private readonly Property _property;

        public Measure(string measureType, Property property)
        {
            _measureType = measureType;
            _property = property;
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

        public static Measure<double> Maximum(double property)
        {
            throw new NotImplementedException();
        }

        public static Measure<DateTime> Maximum(DateTime property)
        {
            throw new NotImplementedException();
        }

        public static Measure<double> Minimum(double property)
        {
            throw new NotImplementedException();
        }

        public static Measure<DateTime> Minimum(DateTime property)
        {
            throw new NotImplementedException();
        }

        internal JProperty ToJProperty()
        {
            return new JProperty(_measureType, new JObject(_property.ToInputJProperty()));

        }
    }
}