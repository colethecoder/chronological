using System;
using System.Linq.Expressions;

namespace Chronological
{
    public class AggregateBuilder<T> where T : new()
    {
        internal AggregateBuilder()
        {
        }        

        public Aggregate<T, TY, TZ> UniqueValues<TY, TZ>(Expression<Func<T, TY>> property, Limit limit, TZ child) where TY : new()
        {
            return new UniqueValuesAggregate<T, TY, TZ>(Property<TY>.Create<T>(property), limit, child);
        }

        public Aggregate<T, DateTime, TZ> DateHistogram<TZ>(Expression<Func<T, DateTime>> property, DateBreaks breaks, TZ child)
        {
            return new DateHistogramAggregate<T, TZ>(Property<DateTime>.Create<T>(property), breaks, child);
        }

        public Aggregate<T, NumericRange, TZ> NumericHistogram<TZ>(Expression<Func<T, double>> property, NumericBreaks breaks, TZ child)
        {
            return new NumericHistogramAggregate<T, TZ>(Property<double>.Create<T>(property), breaks, child);
        }

        public Measure<TY> Maximum<TY>(Expression<Func<T, TY>> property) where TY : new()
        {
            return Measure<TY>.Create(property, Measure.MaximumMeasureExpression);
        }
    }
}
