using System;
using Newtonsoft.Json.Linq;

namespace Chronological.Tests
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
        public double Value { get; set; }
    }

    public class TestType1JProperties
    {
        public static JProperty Value => new JProperty("input",
            new JObject(new JProperty("property", "data.value"), new JProperty("type", "Double")));
    }
}
