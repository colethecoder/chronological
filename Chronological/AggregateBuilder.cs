using System;
using System.Linq.Expressions;

namespace Chronological
{
    public class AggregateBuilder<T> where T : new()
    {
        internal AggregateBuilder()
        {
        }

        public Aggregate<T, TY, TZ> UniqueValues<TY, TZ>(Expression<Func<T, TY>> property, Limit limit, TZ aggregate) where TY : new()
        {
            return new UniqueValuesAggregate<T, TY, TZ>(Property<TY>.Create<T>(property), limit, aggregate);
        }

        public Aggregate<T, DateTime, TZ> DateHistogram<TY, TZ>(Expression<Func<T, TY>> property, DateBreaks breaks, TZ aggregate)
        {
            throw new NotImplementedException();
            //return new Aggregate<T, DateTime, TZ>();
        }

        public Aggregate<T, NumericRange, TZ> NumericHistogram<TY, TZ>(Expression<Func<T, TY>> property, NumericBreaks limit, TZ aggregate)
        {
            throw new NotImplementedException();
            //return new Aggregate<T, NumericRange, TZ>();
        }

        public Measure<TY> Maximum<TY>(Expression<Func<T, TY>> property) where TY : new()
        {
            return Measure<TY>.Create(property, Measure.MaximumMeasureExpression);
        }
    }
}
