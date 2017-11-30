using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Measure<T>
    {
    }

    public class Measure
    {
        private readonly string _measureType;
        private readonly Property _property;

        public Measure(string measureType, Property property)
        {
            _measureType = measureType;
            _property = property;
        }

        public static Measure Sum(Property property)
        {
            return new Measure("sum", property);
        }

        public static Measure Average(Property property)
        {
            return new Measure("avg", property);
        }

        public static Measure Minimum(Property property)
        {
            return new Measure("min", property);
        }

        public static Measure Maximum(Property property)
        {
            return new Measure("max", property);
        }

        public static Measure<TY> Maximum<TX, TY>(Expression<Func<TX, TY>> property)
        {
            var memberExpression = property.Body as MemberExpression;
            var attr = memberExpression.Member.GetCustomAttributes(typeof(ChronologicalEventFieldAttribute), true);
            var test =  ((ChronologicalEventFieldAttribute)attr.First()).EventFieldName;
            throw new NotImplementedException();
        }

        public static Measure<DateTime> Maximum(DateTime property)
        {
            throw new NotImplementedException();
        }

        public static Measure<double> Minimum(double property)
        {
            throw new NotImplementedException();
        }

        public static Measure<DateTime> Minimum(DateTime property)
        {
            throw new NotImplementedException();
        }

        internal JProperty ToJProperty()
        {
            return new JProperty(_measureType, new JObject(_property.ToInputJProperty()));

        }
    }
}