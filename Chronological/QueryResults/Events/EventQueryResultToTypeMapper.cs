using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chronological.QueryResults.Events
{
    internal class EventQueryResultToTypeMapper
    {
        internal IEnumerable<T> Map<T>(EventQueryResult eventQueryResult)
        {
            return Map<T>(eventQueryResult.Content.Events);
        }

        internal IEnumerable<T> Map<T>(IEnumerable<EventResult> eventResults)
        {
            var schemaDictionary = GetSchemaDictionary(eventResults);

            var results = new List<T>();

            var returnType = typeof(T);
            var typeProperties = returnType.GetRuntimeProperties();

            foreach (var eventResult in eventResults)
            {
                var instance = (T) Activator.CreateInstance<T>();

                var schema = eventResult.Schema ?? schemaDictionary[eventResult.SchemaRid.Value];

                for (var i = 0; i < schema.Properties.Count; i++)
                {
                    var name = schema.Properties.ElementAt(i).Name;
                    var propertyType = schema.Properties.ElementAt(i).Type;
                    var value = eventResult.Values.ElementAt(i);

                    foreach (var typeProperty in typeProperties)
                    {
                        var attributes = (ChronologicalEventFieldAttribute[])typeProperty.GetCustomAttributes(typeof(ChronologicalEventFieldAttribute), false);
                        if (typeProperty.CanWrite && (typeProperty.Name == name || attributes.Any(x => x.EventFieldName == name)))
                        {
                            if (propertyType.ToLower() == "datetime" && typeProperty.PropertyType == typeof(DateTime))
                            {
                                typeProperty.SetValue(instance, DateTime.Parse(value));
                            }
                            if (propertyType.ToLower() == "string" && typeProperty.PropertyType == typeof(string))
                            {
                                typeProperty.SetValue(instance, value);
                            }
                            if (propertyType.ToLower() == "double" && typeProperty.PropertyType == typeof(double))
                            {
                                typeProperty.SetValue(instance,double.Parse(value));
                            }
                            if (propertyType.ToLower() == "string" && typeProperty.PropertyType == typeof(double))
                            {
                                if (double.TryParse(value, out double x))
                                {
                                    typeProperty.SetValue(instance, x);
                                }
                            }
                        }
                    }

                }

                results.Add(instance);
                

            }

            return results;
        }

        internal Dictionary<int, Schema> GetSchemaDictionary(IEnumerable<EventResult> eventQueryResults)
        {
            var result = new Dictionary<int, Schema>();

            foreach (var eventResult in eventQueryResults)
            {
                if (eventResult.Schema != null)
                {
                    result.Add(eventResult.Schema.Rid, eventResult.Schema);
                }
            }

            return result;
        }
    }
}
