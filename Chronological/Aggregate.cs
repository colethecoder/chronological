using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection;

namespace Chronological
{
    public class Aggregate<T>
    {
        public Aggregate<T, TY, TZ> UniqueValues<TY, TZ>(Expression<Func<T, TY>> property, Limit limit, TZ aggregate)
        {
            return new Aggregate<T, TY, TZ>();
        }

        public Aggregate<T, DateTime, TZ> DateHistogram<TY, TZ>(Expression<Func<T, TY>> property, DateBreaks breaks, TZ aggregate)
        {
            return new Aggregate<T, DateTime, TZ>();
        }

        public Aggregate<T, NumericRange, TZ> NumericHistogram<TY, TZ>(Expression<Func<T, TY>> property, NumericBreaks limit, TZ aggregate)
        {
            return new Aggregate<T, NumericRange, TZ>();
        }

        public Measure<TY> Maximum<TY>(Expression<Func<T, TY>> property) where TY : new()
        {
            var memberExpression = property.Body as MemberExpression;
            var attributes = memberExpression?.Member.GetCustomAttributes(typeof(ChronologicalEventFieldAttribute), true);
            var attribute = (ChronologicalEventFieldAttribute) attributes?.FirstOrDefault();
            if (attribute != null)
            {
                return new Measure<TY>(attribute.EventFieldName);
            }
            //Todo: attempt to get field name
            throw new NotImplementedException();
        }
    }


    public class NumericRange
    {
        public double From { get; set; }
        public double To { get; set; }
    }

    public class Aggregate<T, TX, TY>
    {
        public Aggregate<T, TX,TZ> WithAggregate<TZ>(TZ aggregate)
        {
            throw new NotImplementedException();
        }        
    }

    public class Aggregate<T, TX>
    {
        public Aggregate<T, TX, TY> WithAggregate<TY>(TY aggregate)
        {
            throw new NotImplementedException();
        }

        public Aggregate<T, TX, TY> WithMeasure<TY>(TY measure)
        {
            throw new NotImplementedException();
        }

        public Aggregate<T, TX, Tuple<TA,TB>> WithMeasures<TA,TB>(TA measureA, TB measureB)
        {
            throw new NotImplementedException();
        }

        public Aggregate<T, TX, Tuple<TA, TB, TC>> WithMeasures<TA, TB, TC>(TA measureA, TB measureB, TC measureC)
        {
            throw new NotImplementedException();
        }

        public Aggregate<TX, Tuple<TA, TB, TC, TD>> WithMeasures<TA, TB, TC, TD>(TA measureA, TB measureB, TC measureC, TD measureD)
        {
            throw new NotImplementedException();
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

        public static Aggregate<DateTime> DateHistogram(object property, DateBreaks breaks)
        {
            return new Aggregate<DateTime>();
        }

        public static Aggregate<NumericRange> NumericHistogram(object property, NumericBreaks limit)
        {
            return new Aggregate<NumericRange>();
        }
        
        public static Aggregate<TY> UniqueValues<TX, TY>(Expression<Func<TX,TY>> property, Limit limit)
        {
            return new Aggregate<TY>();
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
