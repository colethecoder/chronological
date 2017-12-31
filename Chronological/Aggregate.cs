using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chronological
{
    public interface IAggregate
    {
        JObject ToJObject();        
    }

    internal interface IInternalAggregate
    {
        void Populate(JObject jObject);
    }

    public abstract class Aggregate<TX, TY, TZ> : Dictionary<TY,TZ>, IAggregate, IInternalAggregate
    {
        internal abstract string AggregateType { get; }
        internal abstract TZ Child { get; }

        void IInternalAggregate.Populate(JObject jObject)
        {
            var temp = false;
            foreach (var dimension in jObject["dimension"])
            {                
                if (ChildIsAggregate())
                {
                    if (!temp)
                    {
                        ((IInternalAggregate)Child).Populate((JObject)jObject["aggregate"]);
                        temp = true;
                    }
                    this.Add(dimension.ToObject<TY>(), Child);
                }
                else
                {
                    this.Add(dimension.ToObject<TY>(), default(TZ));
                }
            }
        }

        internal abstract JProperty ToAggregateJProperty();

        internal bool ChildIsAggregate()
        {
            return typeof(IAggregate).GetTypeInfo().IsAssignableFrom(typeof(TZ).GetTypeInfo());            
        }

        internal JProperty ToChildJProperty()
        {
            if (ChildIsAggregate())
            {
                return new JProperty("aggregate", ((IAggregate)Child).ToJObject());
            }

            var measures = new List<IMeasure>();
            foreach (var property in typeof(TZ).GetTypeInfo().DeclaredProperties)
            {
                if (typeof(IMeasure).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()))
                {
                    measures.Add((IMeasure)property.GetValue(Child));
                }
            }
            return new JProperty("measures", new JArray(from measure in measures select new JObject(measure.ToJProperty())));
        }

        public JObject ToJObject()
        {
            var childProperty = ToChildJProperty();
            var dimensionProperty = new JProperty("dimension", new JObject(ToAggregateJProperty()));
            var result = new JObject(dimensionProperty, childProperty);
            return result;
        }
    }    

    
}
