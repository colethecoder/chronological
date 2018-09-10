using System;

namespace Chronological.Samples
{
    public class TestType1
    {
        [ChronologicalEventField("id")]
        public string Id { get; set; }

        [ChronologicalEventField(BuiltIn.EventTimeStamp)]
        public DateTime Date { get; set; }

        [ChronologicalEventField("data.devicedate")]
        public DateTime DeviceDate { get; set; }

        [ChronologicalEventField("data.type")]
        public string DataType { get; set; }

        [ChronologicalEventField("data.value")]
        public double? Value { get; set; }

        [ChronologicalEventField("data.isSimulated")]
        public bool? IsSimulated { get; set; }
    }
}
