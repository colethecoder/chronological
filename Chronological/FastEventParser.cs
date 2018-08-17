using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using FastMember;
using System.Linq;

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
                cont: !((percent - 100d) < 0.01 )
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

            var type = typeof(T);
            var accessor = TypeAccessor.Create(type);

            var props = (from property in type.GetProperties()
                        from attribute in property.CustomAttributes
                        where attribute.AttributeType == typeof(ChronologicalEventFieldAttribute)
                        select new { Key = (string)attribute.ConstructorArguments[0].Value, Property = property }).ToDictionary(x => x.Key, x => x.Property);



            //TODO: create dict of Chronological field names to T propname

            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case (JsonToken.StartArray):
                        // swallow the start of array
                        break;
                    case (JsonToken.StartObject):
                        content.Add(HandleEvent<T>(jr, schemas, accessor, props));
                        break;
                    case (JsonToken.EndArray):
                        return content;
                }
            }

            return content;
        }

        private static T HandleEvent<T>(JsonTextReader jr, Dictionary<int, TsiSchema> schemas, TypeAccessor accessor, Dictionary<string, System.Reflection.PropertyInfo> mapping)
        {
            KeyValuePair<int, TsiSchema> schema = default;

            var ev = accessor.CreateNew();

            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case (JsonToken.PropertyName):
                        switch ((string)jr.Value)
                        {
                            case "schema":
                                schema = HandleSchema(jr, mapping);
                                schemas.Add(schema.Key, schema.Value);
                                break;
                            case "schemaRid":
                                var rid = jr.ReadAsInt32().Value;
                                schema = new KeyValuePair<int, TsiSchema>(rid, schemas[rid]);
                                break;
                            case "$ts":
                                // handle timestamp
                                break;
                            case "values":
                                // handle values array
                                HandleEventValues<T>(jr, ev, schema.Value, accessor);
                                break;
                            default:
                                break;
                        }
                        break;
                    case (JsonToken.EndObject):
                        return (T)ev;
                        
                }
            }
            return (T)ev;
        }

        private static T HandleEventValues<T>(JsonTextReader jr, object ev, TsiSchema schema, TypeAccessor accessor)
        {
            //var ev = accessor.CreateNew();
            //accessor.GetMembers();
            var i = 0;
            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case (JsonToken.StartArray):
                        break;
                    case (JsonToken.EndArray):
                        return (T)ev;
                    case (JsonToken.Date):
                    case (JsonToken.Float):
                    case (JsonToken.String):
                        var schemaItem = schema.Properties[i];
                        if (!schemaItem.Ignore)
                        {
                            switch (schemaItem.JsonDataType)
                            {
                                case ("DateTime"):
                                    accessor[ev, schemaItem.ChronologicalPropertyName] = jr.ReadAsDateTime();
                                    break;
                                case ("Double"):
                                    accessor[ev, schemaItem.ChronologicalPropertyName] = jr.ReadAsDouble();
                                    break;
                                case ("String"):
                                    if (schemaItem.ChronologicalType == typeof(double))
                                    {
                                        accessor[ev, schemaItem.ChronologicalPropertyName] = double.Parse(jr.ReadAsString());
                                    }
                                    else
                                    {
                                        accessor[ev, schemaItem.ChronologicalPropertyName] = jr.ReadAsString();
                                    }
                                    break;
                            }
                        }
                        i++;
                        break;
                }
            }
            return (T)ev;
        }

        private static KeyValuePair<int, TsiSchema> HandleSchema(JsonTextReader jr, Dictionary<string, System.Reflection.PropertyInfo> mapping)
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
                                schema.Properties = HandleSchemaProperties(jr, mapping);
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

        private static List<TsiEventProperty> HandleSchemaProperties(JsonTextReader jr, Dictionary<string, System.Reflection.PropertyInfo> mapping)
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
                        properties.Add(HandleSchemaProperty(jr, mapping));
                        break;
                    case (JsonToken.EndArray):
                        return properties;
                }
            }
            //should never get here
            return properties;
        }

        private static TsiEventProperty HandleSchemaProperty(JsonTextReader jr, Dictionary<string, System.Reflection.PropertyInfo> mapping)
        {
            var prop = new TsiEventProperty();            
            jr.Read(); // Name Property
            prop.Name = jr.ReadAsString();
            jr.Read(); // Datatype Property
            prop.JsonDataType = jr.ReadAsString();
            if (mapping.TryGetValue(prop.Name, out var value))
            {
                prop.Ignore = false;
                prop.ChronologicalPropertyName = value.Name;
                prop.ChronologicalType = value.PropertyType;
            }
            else
            {
                prop.Ignore = true;
            }
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
            public string JsonDataType { get; set; }
            public string Name { get; set; }
            public bool Ignore { get; set; }
            public string ChronologicalPropertyName { get; set; }
            public Type ChronologicalType { get; set; }
        }

        public class ChronologicalSchema
        {
            public string ChronologicalTimeStampField { get; set; }
            public ChronologicalProperty[] Fields { get; set; } 
        }

        public class ChronologicalProperty
        {
            public string TsiFieldName { get; set; }
            public string ChronologicalFieldName { get; set; }
        }
    }
}
