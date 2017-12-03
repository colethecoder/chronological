using System;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Chronological.Tests
{
    public class TestType
    {
        [ChronologicalEventField("id")]
        public string Id { get; set; }

        [ChronologicalEventField("timestamp")]
        public DateTime Date { get; set; }

        [ChronologicalEventField("data.type")]
        public string DataType { get; set; }
        [ChronologicalEventField("data.value")]
        public double Value { get; set; }
    }

    public class MeasureExpressionTests
    {
        // Broken test to fiddle with build
        [Fact]
        public void Test1()
        {            
            var aggregate = new Aggregate<TestType>();
            var measure = aggregate.Maximum(x => x.Value);
            var measureProperty = measure.ToJProperty();
            Assert.Equal(measureProperty, new JProperty("Test"));
        }
    }
}
