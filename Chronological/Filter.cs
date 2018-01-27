using System;
using System.Collections.Generic;
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

        internal static Filter Create<T>(Expression<Func<T, bool>> predicate) where T : new()
        {            
            return new Filter(ExpressionToString(predicate.Body));            
        }

        private static string ExpressionToString(Expression expression)
        {
            switch (expression)
            {
                case MemberExpression memberExpression:
                    return MemberExpressionToString(memberExpression);
                case BinaryExpression binaryExpression:
                    return BinaryExpressionToString(binaryExpression);
                case NewExpression newExpression:
                    return NewExpressionToString(newExpression);
                case ConstantExpression constantExpression:
                    return ConstantExpressionToString(constantExpression);
                case MethodCallExpression methodCallExpression:
                    return MethodCallExpressionToString(methodCallExpression);
                default:
                    throw new NotImplementedException();
            }
        }

        private static string MethodCallExpressionToString(MethodCallExpression methodCallExpression)
        {
            //TODO: more tests around inline methods
            object result = Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();
            return ExpressionToString(Expression.Constant(result));
        }

        private static string BinaryExpressionToString(BinaryExpression binaryExpression)
        {
            var leftSide = binaryExpression.Left;
            var rightSide = binaryExpression.Right;
            var comparison = binaryExpression.NodeType;

            return $"({ExpressionToString(leftSide)} {ComparisonToString(comparison)} {ExpressionToString(rightSide)})";
        }

        private static string MemberExpressionToString(MemberExpression memberExpression)
        {
            if (memberExpression.Type == typeof(DateTime))
            {
                if (memberExpression.Member.Name == "UtcNow")
                {
                    return BuiltIn.Function.UtcNow;
                }
                // TODO: throw exception for any other DateTime type as not supported
            }            

            var eventFieldMemberExpression = new EventFieldMemberExpression(memberExpression);

            return eventFieldMemberExpression.EscapedEventFieldName;
        }

        private static string NewExpressionToString(NewExpression newExpression)
        {
            object result = Expression.Lambda(newExpression).Compile().DynamicInvoke();
            return ConvertObjectToString(result);
        }

        private static string ConstantExpressionToString(ConstantExpression constantExpression)
        {
            return ConvertObjectToString(constantExpression.Value);
        }

        private static string ConvertObjectToString(object toConvert)
        {
            switch (toConvert)
            {
                case double d:
                    return d.ToString();
                case string s:
                    return $"'{s}'";
                case bool b:
                    return b ? "TRUE" : "FALSE";
                case TimeSpan ts:
                    return $"ts'P0Y0M{ts.Days}DT{ts.Hours}H{ts.Minutes}M{ts.Seconds}.{ts.Milliseconds}S'";
                case DateTime dt:
                    return $"dt'{dt:O}'";
                default:
                    throw new NotImplementedException();
            }
        }

        private static string ComparisonToString(ExpressionType comparison)
        {
            switch (comparison)
            {
                case (ExpressionType.LessThan):
                    return "<";
                case (ExpressionType.LessThanOrEqual):
                    return "<=";
                case (ExpressionType.GreaterThan):
                    return ">";
                case (ExpressionType.GreaterThanOrEqual):
                    return ">=";
                case (ExpressionType.Equal):
                    return "=";
                case (ExpressionType.NotEqual):
                    return "!=";
                case (ExpressionType.AndAlso):
                    return "and";
                case (ExpressionType.Subtract):
                    return "-";
                case (ExpressionType.Add):
                    return "+";
                //TODO: multiplication / division / or figure out IN / HAS
                default:
                    throw new NotImplementedException();
            }
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
