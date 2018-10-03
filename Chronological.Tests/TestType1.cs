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
        public double? Value { get; set; }

        [ChronologicalEventField("data.isSimulated")]
        public bool? IsSimulated { get; set; }
    }

    public static class TestType1JProperties
    {
        public static JProperty Value => new JProperty("input",
            new JObject(new JProperty("property", "data.value"), new JProperty("type", "Double")));

        public static JProperty Date => new JProperty("input",
            new JObject(new JProperty("builtInProperty", "$ts")));

        public static JProperty LastMeasureWithoutOrderBy => new JProperty("last",
            new JObject(Value));

        public static JProperty FirstMeasureWithoutOrderBy => new JProperty("first",
            new JObject(Value));

        public static JProperty LastMeasureWithOrderBy => new JProperty("last",
            new JObject(Value, new JProperty("orderBy", new JObject(new JProperty("builtInProperty", "$ts")))));

        public static JProperty FirstMeasureWithOrderBy => new JProperty("first",
            new JObject(Value, new JProperty("orderBy", new JObject(new JProperty("builtInProperty", "$ts")))));
    }

    public class TestType2
    {
        [ChronologicalEventField("id")]
        public string Id { get; set; }

        [ChronologicalEventField(BuiltIn.EventTimeStamp)]
        public DateTime Date { get; set; }

        [ChronologicalEventField("data.devicedate")]
        public DateTime DeviceDate { get; set; }

        [ChronologicalEventField("data.type")]
        public string DataType { get; set; }

        [ChronologicalEventField("Value")]
        public double Value { get; set; }

        [ChronologicalEventField("data.isSimulated")]
        public bool? IsSimulated { get; set; }
    }
}
