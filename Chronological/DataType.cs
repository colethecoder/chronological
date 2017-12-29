using System;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class DataType
    {
        internal readonly string _dataType;
        internal DataType(string dataType)
        {
            _dataType = dataType;
        }

        internal JProperty ToJProperty()
        {
            return new JProperty("type", _dataType);
        }

        internal static DataType FromType(Type type)
        {
            switch (type)
            {
                case Type doubleType when doubleType == typeof(double):
                    return Double;
                case Type stringType when stringType == typeof(string):
                    return String;
                case Type dateTimeType when dateTimeType == typeof(DateTime):
                    return DateTime;
                default:
                    //Todo: Better exceptions
                    throw new Exception("Unexpected Type");
            }
        }

        public static DataType Double => new DataType("Double");
        public static DataType String => new DataType("String");
        public static DataType DateTime => new DataType("DateTime");
    }
}
