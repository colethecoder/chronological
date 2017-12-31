using System;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class NumericHistogramAggregate<TX, TZ> : Aggregate<TX, NumericRange, TZ>
    {
        internal override string AggregateType => "numericHistogram";

        public readonly Property Property;
        public readonly NumericBreaks NumericBreaks;
        internal override TZ Child { get; }

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
