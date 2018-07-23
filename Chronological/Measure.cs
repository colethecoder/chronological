using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public interface IMeasure
    {
        JProperty ToJProperty();
    }

    internal interface IInternalMeasure
    {
        IMeasure GetPopulatedMeasure(JValue value);
    }

    public class Measure<T> : IMeasure, IInternalMeasure
    {
        internal readonly string MeasureType;
        internal readonly Property Property;
        public readonly T Value;

        internal Measure (Property property, string measureType)
        {
            MeasureType = measureType;
            Property = property;
        }

        private Measure(Property property, string measureType, T value) : this(property, measureType)
        {
            Value = value;
        }

        IMeasure IInternalMeasure.GetPopulatedMeasure(JValue value)
        {
            return new Measure<T>(Property, MeasureType, value.ToObject<T>());
        }

        internal static Measure<T> Create<TY>(Expression<Func<TY, T>> propertyExpression, string measureType)
        {
            var property = Property<T>.Create(propertyExpression);
            return new Measure<T>(property, measureType);            
        }

        internal static Measure<T> Create(string measureType)
        {
            return new Measure<T>(null, measureType);
        }

        public JProperty ToJProperty()
        {
            switch (MeasureType)
            {
                case (Measure.CountMeasureExpression):
                    return new JProperty(MeasureType, new JObject());
                default:
                    return new JProperty(MeasureType, new JObject(Property.ToInputJProperty()));
            }
        }
    }

    public static class Measure
    {
        internal const string MaximumMeasureExpression = "max";
        internal const string MinimumMeasureExpression = "min";
        internal const string AverageMeasureExpression = "avg";
        internal const string SumMeasureExpression = "sum";
		internal const string CountMeasureExpression = "count";
        internal const string LastMeasureExpression = "last";
        internal const string FirstMeasureExpression = "first";
    }
}
