using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chronological
{
    internal interface IAggregate
    {
        JObject ToJObject();
    }

    public abstract class Aggregate<TX, TY, TZ> : Dictionary<TY,TZ>, IAggregate
    {
        internal abstract string AggregateType { get; }
        public abstract TZ Child { get; }

        internal Aggregate() : base()
        {
        }

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
            throw new NotImplementedException();
        }

        public JObject ToJObject()
        {
            var result = new JObject(new JProperty("dimension", ToAggregateJProperty()), ToChildJProperty());
            return null;
        }
    }    

    
}
