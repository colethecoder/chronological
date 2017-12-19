using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace Chronological
{
    public abstract class Aggregate<T, TX, TY> : Dictionary<TX,TY>
    {
        internal abstract string AggregateType { get; }

        internal Aggregate() : base()
        {
        }

        internal abstract JProperty ToAggregateJProperty();

        internal JObject ToJObject()
        {

            return null;
        }
    }    

    public class Aggregate
    {
        private Dimension _dimension;
        private List<Measure> _measures;
        private Aggregate _aggregate;

        private Aggregate()
        {
        }

        private Aggregate WithDimension(Dimension dimension)
        {
            _dimension = dimension;
            return this;
        }

        
        public Aggregate WithMeasure(Measure measure)
        {
            if (_measures == null)
            {
                _measures = new List<Measure>();
            }
            _measures.Add(measure);
            return this;
        }

        public Aggregate WithAggregate(Aggregate aggregate)
        {
            _aggregate = aggregate;
            return this;
        }

        private JArray GetMeasuresJArray()
        {
            var array = new JArray();
            foreach (var measure in _measures)
            {
                array.Add(new JObject(measure.ToJProperty()));
            }
            return array;
        }


        internal JObject ToJObject()
        {
            if (_aggregate != null)
            {
                return new JObject(
                    new JProperty("aggregate", _aggregate.ToJObject()),
                    _dimension.ToJProperty());
            }
            return new JObject(
                 new JProperty("measures", GetMeasuresJArray()),
                 _dimension.ToJProperty());
        }
    }
}
