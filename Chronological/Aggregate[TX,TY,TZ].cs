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

        IAggregate IInternalAggregate.GetPopulatedAggregate(JObject jObject, Func<JArray, JArray> measureAccessFunc)
        {
            var aggregate = Clone();
            
            foreach (var dimension in jObject["dimension"].Select((x, y) => new { x, y }))
            {
                Func<JArray, JArray> revisedMeasureAccessFunc = x => {
                    if (x == null)
                    {
                        return null;
                    }
                    var temp = measureAccessFunc(x);
                    if (temp == null)
                    {
                        return null;
                    }
                    JToken value = temp[dimension.y];
                    return value.Type == JTokenType.Null ? null : (JArray)value;
                };
                if (ChildIsAggregate())
                {
                    var child = ((IInternalAggregate)Child).GetPopulatedAggregate((JObject)jObject["aggregate"], revisedMeasureAccessFunc);
                    aggregate.Add(dimension.x.ToObject<TY>(), (TZ)child);
                }
                else
                {
                    var measures = new List<IMeasure>();
                    foreach (var property in typeof(TZ).GetTypeInfo().DeclaredProperties.Select((x, y) => new { x, y }))
                    {
                        if (typeof(IMeasure).GetTypeInfo().IsAssignableFrom(property.x.PropertyType.GetTypeInfo()))
                        {
                            var m = (IInternalMeasure)property.x.GetValue(Child);
                            var measuresJArray = (JArray)jObject["measures"];
                            var measureIndexJArray = revisedMeasureAccessFunc(measuresJArray);
                            JValue value;
                            if (measureIndexJArray != null && measureIndexJArray.Type == JTokenType.Array)
                            {
                                value = (JValue)(measureIndexJArray[property.y]);
                            }
                            else
                            {
                                value = null;
                            }
                            if (value != null)
                            {
                                var m1 = m.GetPopulatedMeasure((JValue)value);
                                measures.Add(m1);
                            }
                            else
                            {
                                measures.Add(null);
                            }
                        }
                    }

                    object[] objects = (from measure in measures
                        select measure).ToArray();
                    TZ newAnon = (TZ)Activator.CreateInstance(typeof(TZ), objects);
                    aggregate.Add(dimension.x.ToObject<TY>(), newAnon);
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
