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
                    measure.Measure = ((JValue)item).ToObject<double?>();
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
            //aggregateResult.Dimension = aggregateJObject["dimension"].ToObject<List<string>>();
            var dimensionJArray = (JArray) aggregateJObject["dimension"];
            aggregateResult.Dimension = new List<string>();
            foreach (var dimension in dimensionJArray)
            {
                string dimensionString;
                
                if (dimension.Type == JTokenType.Object)
                {
                    // This assumes it is a NumericHistogram
                    dimensionString = $"from: {dimension["from"].Value<string>()}, to: {dimension["to"].Value<string>()}";
                }
                else
                {
                    var dimensionValue = (JValue)dimension;
                    //Temporary conversion back to string to avoid having multiple types for dimension, needs a rethink
                    if (dimensionValue.Type == JTokenType.Date)
                    {
                        dimensionString = ((DateTime) dimensionValue.Value).ToUniversalTime()
                            .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
                    }
                    else
                    {
                        dimensionString = dimensionValue.ToString();
                    }
                }

                aggregateResult.Dimension.Add(dimensionString);
            }
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

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => false;
    }
}
