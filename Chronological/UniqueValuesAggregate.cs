 using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class UniqueValuesAggregate<TX, TY, TZ> : Aggregate<TX, TY, TZ>
    {
        internal override string AggregateType => "uniqueValues";

        public readonly Property Property;
        public readonly Limit Limit;
        public override TZ Child { get; }

        internal UniqueValuesAggregate(Property property, Limit limit, TZ child)
        {
            Property = property;
            Limit = limit;
            Child = child;
        }

        public override void Populate(JObject jObject)
        {
            var aggregatePopulated = false;
            foreach (var dimension in jObject["dimension"])
            {
                var child = Child;
                if (ChildIsAggregate())
                {
                    if (!aggregatePopulated)
                    {
                        ((IAggregate)child).Populate((JObject)jObject["aggregate"]);
                        aggregatePopulated = true;
                    }
                    this.Add(dimension.ToObject<TY>(), child);
                }
                else
                {
                    this.Add(dimension.ToObject<TY>(), default(TZ));
                }
            }            
        }

        internal override JProperty ToAggregateJProperty()
        {
            return new JProperty(AggregateType, new JObject(Property.ToInputJProperty(), Limit.ToJProperty()));
        }
 
    }

}
