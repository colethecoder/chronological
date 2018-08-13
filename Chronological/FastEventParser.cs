using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using FastMember;

namespace Chronological
{
    public static class FastEventParser
    {
        public static WebSocketResult<T> ParseEvents<T>(StreamReader sr)
        {            
            var jr = new JsonTextReader(sr);
            var percent = 0.0;
            IEnumerable<T> content = null;

            while (jr.Read())
            {
                switch(jr.TokenType)
                {
                    case JsonToken.PropertyName:
                        switch ((string)jr.Value)
                        {
                            case ("headers"):
                                HandleHeaders(jr);
                                break;
                            case ("content"):
                                content = HandleContent<T>(jr);
                                break;
                            case ("warnings"):
                                HandleWarnings(jr);
                                break;
                            case ("percentageCompleted"):
                                percent = jr.ReadAsDouble().Value;
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            return new WebSocketResult<T>.WebSocketSuccess(
                results: content,
                cont: (percent - 100d) < 0.01 
                );
        }

        private static void HandleHeaders(JsonTextReader jr)
        {
            while (jr.Read())
            {
                if (jr.TokenType == JsonToken.EndObject)
                {
                    return;
                }
            }
        }

        private static void HandleWarnings(JsonTextReader jr)
        {
            while (jr.Read())
            {
                if (jr.TokenType == JsonToken.EndArray)
                {
                    return;
                }
            }
        }

        private static IEnumerable<T> HandleContent<T>(JsonTextReader jr)
        {
            IEnumerable<T> content = new T[0];

            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case JsonToken.PropertyName:
                        switch((string)jr.Value)
                        {
                            case ("events"):
                                content = HandleEvents<T>(jr);
                                break;
                        }
                        break;
                    case JsonToken.EndObject:
                        return content;
                    default:
                        break;
                }
            }

            return content;
        }

        private static IEnumerable<T> HandleEvents<T>(JsonTextReader jr)
        {
            var schemas = new Dictionary<int, TsiSchema>();
            var content = new List<T>();

            var accessor = TypeAccessor.Create(typeof(T));



            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case (JsonToken.StartArray):
                        // swallow the start of array
                        break;
                    case (JsonToken.StartObject):
                        content.Add(HandleEvent<T>(jr, schemas));
                        break;
                    case (JsonToken.EndArray):
                        return content;
                }
            }

            return content;
        }

        private static T HandleEvent<T>(JsonTextReader jr, Dictionary<int, TsiSchema> schemas)
        {
            KeyValuePair<int, TsiSchema> schema;
            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case (JsonToken.PropertyName):
                        switch ((string)jr.Value)
                        {
                            case "schema":
                                schema = HandleSchema(jr);
                                schemas.Add(schema.Key, schema.Value);
                                break;
                            case "schemaRid":
                                var rid = jr.ReadAsInt32().Value;
                                schema = new KeyValuePair<int, TsiSchema>(rid, schemas[rid]);
                                break;
                            
                            default:
                                break;
                        }
                        break;
                    case (JsonToken.EndObject):
                        return default(T);
                        
                }
            }
            return default(T);
        }

        private static KeyValuePair<int, TsiSchema> HandleSchema(JsonTextReader jr)
        {
            int? rid = null;
            TsiSchema schema = new TsiSchema();

            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case (JsonToken.StartObject):
                        //swallow start of object
                        break;
                    case (JsonToken.PropertyName):
                        switch ((string)jr.Value)
                        {
                            case ("rid"):
                                rid = jr.ReadAsInt32().Value;
                                break;
                            case ("$esn"):
                                schema.Esn = jr.ReadAsString();
                                break;
                            case ("properties"):
                                schema.Properties = HandleSchemaProperties(jr);
                                break;
                            default:
                                throw new Exception("Should never get here!");
                        }
                        break;
                    case (JsonToken.EndObject):
                        return new KeyValuePair<int, TsiSchema>(rid.Value, schema);
                    default:
                        throw new Exception("Should never get here");
                }
            }
            // Should never get here
            return new KeyValuePair<int, TsiSchema>(rid.Value, schema);
        }

        private static List<TsiEventProperty> HandleSchemaProperties(JsonTextReader jr)
        {
            var properties = new List<TsiEventProperty>();            
            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case (JsonToken.StartArray):
                        //swallow start of array
                        break;
                    case (JsonToken.StartObject):
                        properties.Add(HandleSchemaProperty(jr));
                        break;
                    case (JsonToken.EndArray):
                        return properties;
                }
            }
            //should never get here
            return properties;
        }

        private static TsiEventProperty HandleSchemaProperty(JsonTextReader jr)
        {
            var prop = new TsiEventProperty();            
            jr.Read(); // Name Property
            prop.Name = jr.ReadAsString();
            jr.Read(); // Datatype Property
            prop.DataType = jr.ReadAsString();
            jr.Read(); // read end object
            return prop;
        }

        public class TsiSchema
        {
            public string Esn { get; set; }
            public List<TsiEventProperty> Properties { get; set; }
        }

        public class TsiEventProperty
        {
            public string DataType { get; set; }
            public string Name { get; set; }
        }
    }
}
