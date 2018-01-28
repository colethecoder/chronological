using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class NumericHistogramAggregate<TX, TZ> : Aggregate<TX, NumericRange, TZ>
    {
        internal override string AggregateType => "numericHistogram";

        public readonly Property Property;
        public readonly NumericBreaks NumericBreaks;
        internal override TZ Child { get; }

        internal override Aggregate<TX, NumericRange, TZ> Clone()
        {
            return new NumericHistogramAggregate<TX, TZ>(Property, NumericBreaks, Child);
        }

        internal NumericHistogramAggregate(Property property, NumericBreaks numericBreaks, TZ child)
        {
            Property = property;
            NumericBreaks = numericBreaks;
            Child = child;
        }
       
        internal override JProperty ToAggregateJProperty()
        {
            return new JProperty(AggregateType, new JObject(Property.ToInputJProperty(), NumericBreaks.ToJProperty()));
        }
 
    }

}
