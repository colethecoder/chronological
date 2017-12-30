using System;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class NumericHistogramAggregate<TX, TZ> : Aggregate<TX, NumericRange, TZ>
    {
        internal override string AggregateType => "numericHistogram";

        public readonly Property Property;
        public readonly NumericBreaks NumericBreaks;
        public override TZ Child { get; }

        internal NumericHistogramAggregate(Property property, NumericBreaks numericBreaks, TZ child)
        {
            Property = property;
            NumericBreaks = numericBreaks;
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
                    this.Add(dimension.ToObject<NumericRange>(), child);
                }
                else
                {
                    this.Add(dimension.ToObject<NumericRange>(), default(TZ));
                }
            }
        }

        internal override JProperty ToAggregateJProperty()
        {
            return new JProperty(AggregateType, new JObject(Property.ToInputJProperty(), NumericBreaks.ToJProperty()));
        }
 
    }

}
