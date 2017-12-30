using System;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class DateHistogramAggregate<TX, TZ> : Aggregate<TX, DateTime, TZ>
    {
        internal override string AggregateType => "dateHistogram";

        public readonly Property Property;
        public readonly DateBreaks DateBreaks;
        public override TZ Child { get; }

        internal DateHistogramAggregate(Property property, DateBreaks dateBreaks, TZ child)
        {
            Property = property;
            DateBreaks = dateBreaks;
            Child = child;
        }

        public override void Populate(JObject jObject)
        {            
            foreach (var dimension in jObject["dimension"])
            {
                var child = Child;
                if (ChildIsAggregate())
                {
                    ((IAggregate)child).Populate((JObject)jObject["aggregate"]);
                    this.Add(dimension.ToObject<DateTime>(), child);
                }
                else
                {
                    this.Add(dimension.ToObject<DateTime>(), default(TZ));
                }
            }
        }

        internal override JProperty ToAggregateJProperty()
        {
            return new JProperty(AggregateType, new JObject(Property.ToInputJProperty(), DateBreaks.ToJProperty()));
        }
 
    }

}
