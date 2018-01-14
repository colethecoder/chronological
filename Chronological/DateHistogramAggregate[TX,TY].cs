using System;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class DateHistogramAggregate<TX, TY> : Aggregate<TX, DateTime, TY>
    {
        internal override string AggregateType => "dateHistogram";

        public readonly Property Property;
        public readonly DateBreaks DateBreaks;
        internal override TY Child { get; }
        internal override Aggregate<TX, DateTime, TY> Clone()
        {
            return new DateHistogramAggregate<TX, TY>(Property, DateBreaks, Child);
        }

        internal DateHistogramAggregate(Property property, DateBreaks dateBreaks, TY child)
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
