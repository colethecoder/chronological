using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Chronological.Tests
{
    public class UniqueValuesAggregateTests
    {
        public JProperty ExpectedResult()
        {
            var measureJArray = new JArray();
            measureJArray.Add(new JObject(new JProperty("max", new JObject(TestType1JProperties.Value))));
            return new JProperty("measures", measureJArray);
        }

        [Fact]
        public void Test1()
        {
            var builder = new AggregateBuilder<TestType1>();
            var aggregate = builder.UniqueValues(x => x.Value, Limit.Take(10), new { Maximum = builder.Maximum(x => x.Value) });

            var test = aggregate.ToChildJProperty();
            var expected = ExpectedResult();
            Assert.True(JToken.DeepEquals(test, expected));
        }
    }
}
