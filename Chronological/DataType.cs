using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class DataType
    {
        private readonly string _dataType;
        public DataType(string dataType)
        {
            _dataType = dataType;
        }

        internal JProperty ToJProperty()
        {
            return new JProperty("type", _dataType);
        }

        public static DataType Double => new DataType("Double");
        public static DataType String => new DataType("String");
    }
}