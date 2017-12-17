using System;

namespace Chronological.Tests
{
    public class TestType1
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
}