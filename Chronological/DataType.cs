using System;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class DataType
    {
        public string TimeSeriesInsightsType { get; }

        internal DataType(string dataType)
        {
            TimeSeriesInsightsType = dataType;
        }

        internal JProperty ToJProperty()
        {
            return new JProperty("type", TimeSeriesInsightsType);
        }

        internal static DataType FromType(Type type)
        {
            switch (type)
            {
                case Type doubleType when doubleType == typeof(double) || doubleType == typeof(double?):
                    return Double;
                case Type stringType when stringType == typeof(string):
                    return String;
                case Type dateTimeType when dateTimeType == typeof(DateTime) || dateTimeType == typeof(DateTime?):
                    return DateTime;
                case Type boolType when boolType == typeof(bool) || boolType == typeof(bool?):
                    return Boolean;
                default:
                    //Todo: Better exceptions
                    throw new Exception("Unexpected Type");
            }
        }

        public static DataType Double => new DataType("Double");
        public static DataType String => new DataType("String");
        public static DataType DateTime => new DataType("DateTime");
        public static DataType Boolean => new DataType("Boolean");
    }
}
