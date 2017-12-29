using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Filter
    {
        private readonly bool _singlular;
        private readonly bool _isPredicateString;

        private readonly string _predicateString;

        private readonly List<Filter> _filters;

        private readonly Property _left;
        private readonly string _rightAsString;
        private readonly double? _rightAsDouble;
        private readonly DateTime? _rightAsDateTime;
        
        private readonly string _operator;

        private Filter(bool singular, List<Filter> filters, Property left, string right, string filterOperator)
        {
            _isPredicateString = false;
            _singlular = singular;
            _filters = filters;
            _left = left;
            _rightAsString = right;
            _operator = filterOperator;
        }

        private Filter(bool singular, List<Filter> filters, Property left, double right, string filterOperator)
        {
            _isPredicateString = false;
            _singlular = singular;
            _filters = filters;
            _left = left;
            _rightAsDouble = right;
            _operator = filterOperator;
        }

        private Filter(bool singular, List<Filter> filters, Property left, DateTime right, string filterOperator)
        {
            _isPredicateString = false;
            _singlular = singular;
            _filters = filters;
            _left = left;
            _rightAsDateTime = right;
            _operator = filterOperator;
        }

        internal static Filter Create<T>(Expression<Func<T, bool>> predicate)
        {
            var binaryExpression = (BinaryExpression)predicate.Body;

            var leftString = string.Empty;

            var leftSide = binaryExpression.Left;

            if (leftSide.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = leftSide as MemberExpression;
                var attributes = memberExpression?.Member.GetCustomAttributes(typeof(ChronologicalEventFieldAttribute), true);
                var attribute = (ChronologicalEventFieldAttribute)attributes?.FirstOrDefault();
                if (attribute != null)
                {
                    leftString = "[" + attribute.EventFieldName + "]";
                }
            }

            var rightSide = binaryExpression.Right;

            var rightString = string.Empty;

            if (rightSide.NodeType == ExpressionType.Constant)
            {
                var constantExpression = ((ConstantExpression)rightSide);

                switch (constantExpression.Value)
                {
                    case double d:
                        rightString = d.ToString();
                        break;
                }
                
            }

            var comparison = binaryExpression.NodeType;

            var comparisonString = string.Empty;

            switch (comparison)
            {
                case (ExpressionType.GreaterThan):
                    comparisonString = ">";
                    break;                
            }

            return new Filter($"{leftString} {comparisonString} {rightString}");
        }

        private Filter(string predicateString)
        {
            _isPredicateString = true;
            _predicateString = predicateString;
        }

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

        public static Filter LessThan(Property left, double right)
        {
            return new Filter(true, null, left, right, "lt");
        }

        public static Filter LessThan(Property left, DateTime right)
        {
            return new Filter(true, null, left, right, "lt");
        }

        public static Filter LessThanOrEqual(Property left, double right)
        {
            return new Filter(true, null, left, right, "lte");
        }

        public static Filter LessThanOrEqual(Property left, DateTime right)
        {
            return new Filter(true, null, left, right, "lte");
        }

        public static Filter GreaterThan(Property left, double right)
        {
            return new Filter(true, null, left, right, "gt");
        }

        public static Filter GreaterThan(Property left, DateTime right)
        {
            return new Filter(true, null, left, right, "gt");
        }

        public static Filter GreaterThanOrEqual(Property left, double right)
        {
            return new Filter(true, null, left, right, "gte");
        }

        public static Filter GreaterThanOrEqual(Property left, DateTime right)
        {
            return new Filter(true, null, left, right, "gte");
        }

        public static Filter And(Filter filter1, Filter filter2, params Filter[] additionalFilters)
        {
            var filters = new List<Filter>() {filter1, filter2};
            filters.AddRange(additionalFilters);
            return new Filter(false, filters, null, null, "and");
        }

        public static Filter Or(Filter filter1, Filter filter2, params Filter[] additionalFilters)
        {
            var filters = new List<Filter>() { filter1, filter2 };
            filters.AddRange(additionalFilters);
            return new Filter(false, filters, null, null, "or");
        }

        public static Filter FromString(string predicateString)
        {
            return new Filter(predicateString);
        }

        internal JProperty ToPredicateJProperty()
        {
            if (_isPredicateString)
            {
                return new JProperty("predicate", new JObject(new JProperty("predicateString", _predicateString)));
            }
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
                    new JProperty("right", _rightAsDateTime.Value)));
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
