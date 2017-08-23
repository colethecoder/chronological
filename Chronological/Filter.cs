using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System;

namespace Chronological
{
    public class Filter
    {
        private readonly bool _singlular;
        private readonly List<Filter> _filters;

        private readonly Property _left;
        private readonly string _rightAsString;
        private readonly double? _rightAsDouble;
        private readonly DateTime? _rightAsDateTime;
        
        private readonly string _operator;

        private Filter(bool singular, List<Filter> filters, Property left, string right, string filterOperator)
        {
            _singlular = singular;
            _filters = filters;
            _left = left;
            _rightAsString = right;
            _operator = filterOperator;
        }

        private Filter(bool singular, List<Filter> filters, Property left, double right, string filterOperator)
        {
            _singlular = singular;
            _filters = filters;
            _left = left;
            _rightAsDouble = right;
            _operator = filterOperator;
        }

        private Filter(bool singular, List<Filter> filters, Property left, DateTime right, string filterOperator)
        {
            _singlular = singular;
            _filters = filters;
            _left = left;
            _rightAsDateTime = right;
            _operator = filterOperator;
        }

        //public static Filter Equal(Property left, Property right)
        //{
        //    return new Filter(true, null, left, right, "eq");
        //}

        public static Filter Equal(Property left, string right)
        {
            return new Filter(true, null, left, right, "eq");
        }

        public static Filter Equal(Property left, double right)
        {
            return new Filter(true, null, left, right, "eq");
        }

        public static Filter Equal(Property left, DateTime right)
        {
            return new Filter(true, null, left, right, "eq");
        }

        public static Filter And(Filter filter1, Filter filter2, params Filter[] additionalFilters)
        {
            var filters = new List<Filter>() {filter1, filter2};
            filters.AddRange(additionalFilters);
            return new Filter(false, filters, null, null, "and");
        }

        internal JProperty ToPredicateJProperty()
        {
            return new JProperty("predicate", new JObject(ToOperationJProperty()));
        }

        internal JProperty ToOperationJProperty()
        {
            return _singlular ? ToSingularJProperty() : ToNestedJProperty();
        }

        private JProperty ToSingularJProperty()
        {
            if (_rightAsString != null)
            {
                return new JProperty(_operator, new JObject(
                    _left.ToLeftJProperty(),
                    new JProperty("right", _rightAsString)));
            }
            if (_rightAsDouble.HasValue)
            {
                return new JProperty(_operator, new JObject(
                    _left.ToLeftJProperty(),
                    new JProperty("right", _rightAsDouble.Value)));
            }

            return new JProperty(_operator, new JObject(
                    _left.ToLeftJProperty(),
                    new JProperty("right", new JObject(_rightAsDateTime.Value))));
        }

        private JProperty ToNestedJProperty()
        {
            var array = new JArray();
            foreach (var filter in _filters)
            {
                array.Add(new JObject(filter.ToOperationJProperty()));
            }
            return new JProperty(_operator, array);
        }
    }
}