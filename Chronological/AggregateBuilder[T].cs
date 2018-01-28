using System;
using System.Linq.Expressions;

namespace Chronological
{
    public class AggregateBuilder<T> where T : new()
    {
        internal AggregateBuilder()
        {
        }        

        public Aggregate<T, TY, TZ> UniqueValues<TY, TZ>(Expression<Func<T, TY>> property, Limit limit, TZ child)
        {
            return new UniqueValuesAggregate<T, TY, TZ>(Property<TY>.Create(property), limit, child);
        }

        public Aggregate<T, DateTime, TZ> DateHistogram<TZ>(Expression<Func<T, DateTime>> property, DateBreaks breaks, TZ child)
        {
            return new DateHistogramAggregate<T, TZ>(Property<DateTime>.Create(property), breaks, child);
        }

        public Aggregate<T, NumericRange, TZ> NumericHistogram<TZ>(Expression<Func<T, double>> property, NumericBreaks breaks, TZ child)
        {
            return new NumericHistogramAggregate<T, TZ>(Property<double>.Create(property), breaks, child);
        }

        public Measure<TY> Maximum<TY>(Expression<Func<T, TY>> property)
        {
            return Measure<TY>.Create(property, Measure.MaximumMeasureExpression);
        }

        public Measure<TY> Minimum<TY>(Expression<Func<T, TY>> property)
        {
            return Measure<TY>.Create(property, Measure.MinimumMeasureExpression);
        }

        public Measure<TY> Sum<TY>(Expression<Func<T, TY>> property)
        {
            return Measure<TY>.Create(property, Measure.SumMeasureExpression);
        }

        public Measure<TY> Average<TY>(Expression<Func<T, TY>> property)
        {
            return Measure<TY>.Create(property, Measure.AverageMeasureExpression);
        }

        public Measure<int> Count()
        {
            return Measure<int>.Create(Measure.CountMeasureExpression);
        }

    }
}
