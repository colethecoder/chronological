using FastMember;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chronological
{

    public abstract class Aggregate<TX, TY, TZ> : Dictionary<TY,TZ>, IAggregate, IInternalAggregate
    {
        internal abstract string AggregateType { get; }
        internal abstract TZ Child { get; }

        internal abstract Aggregate<TX, TY, TZ> Clone();

        Func<JArray, JArray> MeasureAccess(Func<JArray, JArray> accessFunc, int index) => x =>
        {
            if (x == null)
            {
                return null;
            }

            var temp = accessFunc(x);
            if (temp == null)
            {
                return null;
            }

            JToken value = temp[index];
            return value.Type == JTokenType.Null ? null : (JArray)value;
        };

        IAggregate IInternalAggregate.GetPopulatedAggregate(JObject jObject, Func<JArray, JArray> measureAccessFunc)
        {
            var aggregate = Clone();
            var type = typeof(TZ);
            var accessor = TypeAccessor.Create(type);
            var props = type.GetTypeInfo().DeclaredProperties.Select((x, y) => new { x, y });
            foreach (var dimension in jObject["dimension"].Select((x, y) => new { x, y }))
            {               
                if (ChildIsAggregate())
                {
                    var child = ((IInternalAggregate)Child).GetPopulatedAggregate((JObject)jObject["aggregate"], MeasureAccess(measureAccessFunc, dimension.y));
                    aggregate.Add(dimension.x.ToObject<TY>(), (TZ)child);
                }
                else
                {
                    var zzz = accessor.CreateNew();
                    foreach (var property in props)
                    {
                        if (typeof(IMeasure).GetTypeInfo().IsAssignableFrom(property.x.PropertyType.GetTypeInfo()))
                        {
                            var m = (IInternalMeasure)property.x.GetValue(Child);
                            var measuresJArray = (JArray)jObject["measures"];
                            var measureIndexJArray = MeasureAccess(measureAccessFunc, dimension.y)(measuresJArray);
                            if (measureIndexJArray != null && measureIndexJArray.Type == JTokenType.Array)
                            {
                                accessor[zzz, property.x.Name] = 
                                    m.GetPopulatedMeasure(
                                        (JValue)measureIndexJArray[property.y]);
                            }
                            else
                            {
                                accessor[zzz, property.x.Name] = null;
                            }                            
                        }
                    }

                    aggregate.Add(dimension.x.ToObject<TY>(), (TZ)zzz);
                }
            }

            return aggregate;
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
