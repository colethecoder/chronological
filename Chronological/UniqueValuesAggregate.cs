 using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class UniqueValuesAggregate<TX, TY, TZ> : Aggregate<TX, TY, TZ>
    {
        internal override string AggregateType => "uniqueValues";

        public readonly Property Property;
        public readonly Limit Limit;
        internal override TZ Child { get; }

        internal UniqueValuesAggregate(Property property, Limit limit, TZ child)
        {
            Property = property;
            Limit = limit;
            Child = child;
        }

        internal override JProperty ToAggregateJProperty()
        {
            return new JProperty(AggregateType, new JObject(Property.ToInputJProperty(), Limit.ToJProperty()));
        }
 
    }

}
