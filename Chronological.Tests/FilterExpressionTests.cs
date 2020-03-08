﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using Xunit;
using System.Linq;

namespace Chronological.Tests
{
    public class FilterExpressionTests
    {
        private string GetPredicateString(JProperty jProperty)
        {
            return jProperty.SelectToken("$..predicateString").Value<string>();
        }

        [Theory]
        [MemberData(nameof(FilterPredicateTestDataProvider.TestCases), MemberType = typeof(FilterPredicateTestDataProvider))]
        public void FilterPredicateTests(Expression<Func<TestType1, bool>> predicate, string expected)
        {
            var filter = Filter.Create(predicate);
            var result = filter.ToPredicateJProperty();

            var predicateString = GetPredicateString(result);

            Assert.Equal(expected, predicateString);
        }
    }

    public class FilterPredicateTestDataProvider
    {
        private static string[] ContainsTest = new[] { "Hello", "World" };

        private static IEnumerable<(Expression<Func<TestType1, bool>>, string)> _testCases = new List<(Expression<Func<TestType1, bool>>, string)>
        {
            (x => x.Value > 5,                                                                  "([data.value] > 5)"),
            (x => x.Value < 5,                                                                  "([data.value] < 5)"),
            (x => x.Value == 5,                                                                 "([data.value] = 5)"),
            (x => x.Value != 5,                                                                 "([data.value] != 5)"),
            (x => x.Value >= 5,                                                                 "([data.value] >= 5)"),
            (x => x.Value <= 5,                                                                 "([data.value] <= 5)"),
            (x => 4 > x.Value,                                                                  "(4 > [data.value])"),
            (x => 4 < x.Value,                                                                  "(4 < [data.value])"),
            (x => 4 == x.Value,                                                                 "(4 = [data.value])"),
            (x => 4 != x.Value,                                                                 "(4 != [data.value])"),
            (x => 4 >= x.Value,                                                                 "(4 >= [data.value])"),
            (x => 4 <= x.Value,                                                                 "(4 <= [data.value])"),
            (x => 4 > x.Value && x.DataType == "AString",                                       "((4 > [data.value]) and ([data.type] = 'AString'))"),
            (x => x.DeviceDate > DateTime.UtcNow,                                               "([data.devicedate] > utcNow())"),
            (x => x.Date > DateTime.UtcNow,                                                     "($ts > utcNow())"),
            (x => x.Date > (DateTime.UtcNow - TimeSpan.FromMinutes(55)),                        "($ts > (utcNow() - ts'P0Y0M0DT0H55M0.0S'))"),
            (x => x.Date > new DateTime(2018, 01, 27, 0, 0, 0, DateTimeKind.Utc),               "($ts > dt'2018-01-27T00:00:00.0000000Z')"),
            (x => x.IsSimulated == true,                                                        "([data.isSimulated] = TRUE)"),
            (x => 4 > x.Value && (x.DataType == "AStr" || x.DataType == "AStr1"),               "((4 > [data.value]) and (([data.type] = 'AStr') or ([data.type] = 'AStr1')))"),
            (x => new[] {"Hello", "World"}.Contains(x.DataType),                               "([data.type] IN ('Hello', 'World'))"),
            (x => ContainsTest.Contains(x.DataType),                                            "([data.type] IN ('Hello', 'World'))"),
            (x => x.DataType.StartsWith("Hello"),                                               "(startsWith_cs([data.type], 'Hello'))"),
            (x => x.DataType.StartsWith("Hello", StringComparison.CurrentCulture),              "(startsWith_cs([data.type], 'Hello'))"),
            (x => x.DataType.StartsWith("Hello", StringComparison.Ordinal),                     "(startsWith_cs([data.type], 'Hello'))"),
            (x => x.DataType.StartsWith("Hello", StringComparison.OrdinalIgnoreCase),           "(startsWith([data.type], 'Hello'))"),
            (x => x.DataType.StartsWith("Hello", StringComparison.CurrentCultureIgnoreCase),    "(startsWith([data.type], 'Hello'))"),
            (x => x.DataType.EndsWith("Hello"),                                                 "(endsWith_cs([data.type], 'Hello'))"),
            (x => x.DataType.EndsWith("Hello", StringComparison.CurrentCulture),                "(endsWith_cs([data.type], 'Hello'))"),
            (x => x.DataType.EndsWith("Hello", StringComparison.Ordinal),                       "(endsWith_cs([data.type], 'Hello'))"),
            (x => x.DataType.EndsWith("Hello", StringComparison.OrdinalIgnoreCase),             "(endsWith([data.type], 'Hello'))"),
            (x => x.DataType.EndsWith("Hello", StringComparison.CurrentCultureIgnoreCase),      "(endsWith([data.type], 'Hello'))"),
            (x => x.DataType.Contains("ello"),                                                  "(matchesRegex([data.type], '^.*ello.*'))"),
            (x => x.DataType != null,                                                           "([data.type] != NULL)"),
            (x => x.IsSimulated == null,                                                        "([data.isSimulated] = NULL)")
        };

        public static IEnumerable<object[]> TestCases
        {
            get
            {
                (var sc1Expr, var sc1Expect) = SpecialCase1();
                yield return new object[] { sc1Expr, sc1Expect };

                foreach (var (expr, expect) in _testCases)
                {
                    yield return new object[] { expr, expect };
                }
            }
        }

        static (Expression<Func<TestType1, bool>> Expr, string Expect) SpecialCase1()
        {
            var firstObj = new { Prop1 = "Val1", Prop2 = "TestValue" };
            var secondObj = new { PropA = firstObj, PropB = "Test"};
            return (x => x.DataType == secondObj.PropA.Prop2, "([data.type] = 'TestValue')");
        }

    }
}
