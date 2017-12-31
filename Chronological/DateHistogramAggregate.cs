using System;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class DateHistogramAggregate<TX, TZ> : Aggregate<TX, DateTime, TZ>
    {
        internal override string AggregateType => "dateHistogram";

        public readonly Property Property;
        public readonly DateBreaks DateBreaks;
        internal override TZ Child { get; }

        internal DateHistogramAggregate(Property property, DateBreaks dateBreaks, TZ child)
        {
            Property = property;
            DateBreaks = dateBreaks;
            Child = child;
        }        

        internal override JProperty ToAggregateJProperty()
        {
            return new JProperty(AggregateType, new JObject(Property.ToInputJProperty(), DateBreaks.ToJProperty()));
        }
 
    }

}
