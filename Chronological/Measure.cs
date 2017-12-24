using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public interface IMeasure
    {
        JProperty ToJProperty();
    }

    public class Measure<T> : IMeasure where T : new()
    {
        internal readonly string MeasureType;
        internal readonly Property Property;

        internal Measure (Property property, string measureType)
        {
            MeasureType = measureType;
            Property = property;
        }

        internal static Measure<T>Create<TY>(Expression<Func<TY, T>> propertyExpression, string measureType) where TY : new()
        {
            var property = Property<T>.Create(propertyExpression);
            return new Measure<T>(property, measureType);            
        }

        public JProperty ToJProperty()
        {
            return new JProperty(MeasureType, new JObject(Property.ToInputJProperty()));
        }
    }

    public class Measure
    {
        internal const string MaximumMeasureExpression = "max";

        internal readonly string _measureType;
        internal readonly Property _property;

        internal Measure(string measureType, Property property)
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
