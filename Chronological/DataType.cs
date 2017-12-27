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

        internal static DataType FromType<T>(T type)
        {
            switch (type)
            {
                case double d:
                    return Double;
                case string s:
                    return String;
                case DateTime dt:
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
