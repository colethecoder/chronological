using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public interface IMeasure
    {
        JProperty ToJProperty();
    }

    public class Measure<T> : IMeasure
    {
        internal readonly string MeasureType;
        internal readonly Property Property;

        internal Measure (Property property, string measureType)
        {
            MeasureType = measureType;
            Property = property;
        }

        internal static Measure<T>Create<TY>(Expression<Func<TY, T>> propertyExpression, string measureType)
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
        internal const string MinimumMeasureExpression = "min";
        internal const string AverageMeasureExpression = "avg";
        internal const string SumMeasureExpression = "sum";
    }
}
