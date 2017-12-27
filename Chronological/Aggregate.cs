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

    public abstract class Aggregate<TX, TY, TZ> : Dictionary<TY,TZ>, IAggregate
    {
        internal abstract string AggregateType { get; }
        public abstract TZ Child { get; }         

        internal abstract JProperty ToAggregateJProperty();

        internal bool ChildIsAggregate()
        {
            return typeof(IAggregate).GetTypeInfo().IsAssignableFrom(typeof(TZ).GetTypeInfo());
            //Below doesn't work in Standard 1.3
            //return typeof(IAggregate).IsAssignableFrom(typeof(TZ));
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
