using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronological.QueryResults.Aggregates
{
    public class AggregateQueryResultAggregateJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private List<AggregateQueryMeasure> GetMeasures(JArray measuresJArray)
        {
            var measures = new List<AggregateQueryMeasure>();

            foreach (var item in measuresJArray)
            {
                var measure = new AggregateQueryMeasure();
                if (item is JArray)
                {
                    measure.Measures = GetMeasures((JArray) item);
                }
                else
                {
                    measure.Measure = ((JValue)item).ToObject<double>();
                }
                measures.Add(measure);
            }

            return measures;
        }

        private List<AggregateQueryResultAggregate> GetAggregates(JArray aggregatesJArray)
        {
            var aggregates = new List<AggregateQueryResultAggregate>();

            foreach (var item in aggregatesJArray)
            {
                var aggregate = GetAggregate((JObject) item);
                aggregates.Add(aggregate);
            }

            return aggregates;
        }

        private AggregateQueryResultAggregate GetAggregate(JObject aggregateJObject)
        {
            var aggregateResult = new AggregateQueryResultAggregate();
            aggregateResult.Dimension = aggregateJObject["dimension"].ToObject<List<string>>();
            if (aggregateJObject["measures"] != null)
            {
                var measuresJArray = (JArray)aggregateJObject["measures"];
                var measures = GetMeasures(measuresJArray);
                aggregateResult.Measures = measures;
            }
            if (aggregateJObject["aggregate"] != null)
            {
                var subAggregateJObject = (JObject) aggregateJObject["aggregate"];
                var subAggregate = GetAggregate(subAggregateJObject);
                aggregateResult.Aggregate = subAggregate;
            }
            return aggregateResult;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jArray = JArray.Load(reader);
            return GetAggregates(jArray);
        }

        public override bool CanRead {
            get { return true; }
        }

        public override bool CanWrite {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            //Shouldn't be called if used as a property decorator
            return false;
        }
    }
}
