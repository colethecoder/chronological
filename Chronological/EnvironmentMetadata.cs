using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class EnvironmentMetadata
    {
        public IEnumerable<Property> Properties { get; }

        internal EnvironmentMetadata(JArray properties)
        {
            Properties = from property in properties
                         select new Property(false, property["name"].Value<string>(), new DataType(property["type"].Value<string>()));
        }
    }

}
