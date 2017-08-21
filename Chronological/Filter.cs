using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Filter
    {
        private readonly bool _singlular;
        private readonly List<Filter> _filters;

        private readonly Property _left;
        private readonly Property _right;

        private readonly string _operator;

        private Filter(bool singular, List<Filter> filters, Property left, Property right, string filterOperator)
        {
            _singlular = singular;
            _filters = filters;
            _left = left;
            _right = right;
            _operator = filterOperator;
        }

        public static Filter Equal(Property left, Property right)
        {
            return new Filter(true, null, left, right, "eq");
        }

        public static Filter Equal(Property left, string right)
        {
            return Equal(left, Property.Custom(right));
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
            return new JProperty(_operator, new JObject(
                _left.ToLeftJProperty(),
                _right.ToRightJProperty()));
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