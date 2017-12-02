using System;
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
        [Fact]
        public void Test1()
        {
            var aggregate = new Aggregate<TestType>();
            var measure = aggregate.Maximum(x => x.Value);

        }
    }
}
