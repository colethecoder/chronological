using System;
using System.Collections.Generic;
using System.Text;

namespace Chronological
{
    public class EnvironmentMetadata
    {
        public List<EnvironmentMetadataProperty> Properties { get; set; }
    }

    public class EnvironmentMetadataProperty
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }
}
