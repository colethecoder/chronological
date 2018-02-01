using System;
using System.Linq.Expressions;

namespace Chronological
{
    public class AggregateBuilder<T> where T : new()
    {
        internal AggregateBuilder()
        {
        }

        public Aggregate<T, TUniqueValue, TChild> UniqueValues<TUniqueValue, TChild, TSort>(Expression<Func<T, TUniqueValue>> property, ISortableLimit limit, int limitCount, ISortOrder sortOrder, Expression<Func<T, TSort>> sortProperty, TChild child)
        {
            var populatedSortProperty = Property<TSort>.Create(sortProperty);
            var populatedSort = Sort.Create(sortOrder, populatedSortProperty);
            var populatedLimit = Limit.CreateLimit(limit, limitCount, populatedSort);
            return new UniqueValuesAggregate<T, TUniqueValue, TChild>(Property<TUniqueValue>.Create(property), populatedLimit, child);
        }

        public Aggregate<T, TY, TZ> UniqueValues<TY, TZ>(Expression<Func<T, TY>> property, INonSortableLimit limit, int limitCount, TZ child)
        {
            var populatedLimit = Limit.CreateLimit(limit, limitCount);
            return new UniqueValuesAggregate<T, TY, TZ>(Property<TY>.Create(property), populatedLimit, child);
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
