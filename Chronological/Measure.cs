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
        internal readonly Property OrderBy;
        public readonly T Value;

        internal Measure(Property property, string measureType)
        {
            MeasureType = measureType;
            Property = property;
        }

        private Measure(Property property, string measureType, T value) : this(property, measureType)
        {
            Value = value;
        }

        private Measure(Property property, string measureType, Property orderBy) : this(property, measureType)
        {
            OrderBy = orderBy;
        }

        IMeasure IInternalMeasure.GetPopulatedMeasure(JValue value)
        {
            return new Measure<T>(Property, MeasureType, value.ToObject<T>());
        }

        internal static Measure<T> Create<TY>(Expression<Func<TY, T>> propertyExpression, string measureType, Expression<Func<TY, T>> orderByExpression = null)
        {
            var property = Property<T>.Create(propertyExpression);

            if (orderByExpression == null)
                return new Measure<T>(property, measureType);

            if (measureType != Measure.LastMeasureExpression || measureType != Measure.FirstMeasureExpression)
            {
                throw new NotSupportedException($"Cannot use OrderBy clause with the measure type {measureType}. Make sure to use first or last");
            }
            var orderBy = Property<T>.Create(orderByExpression);
            return new Measure<T>(property, measureType, orderBy);
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
                case (Measure.FirstMeasureExpression):
                case (Measure.LastMeasureExpression):
                    return new JProperty(MeasureType,
                        new JObject(Property.ToInputJProperty()),
                        OrderBy == null ? null : new JObject(OrderBy.ToInputJProperty()));
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
