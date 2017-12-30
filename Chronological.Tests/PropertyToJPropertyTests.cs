using Newtonsoft.Json.Linq;
using Xunit;

namespace Chronological.Tests
{
    public class PropertyToJPropertyTests
    {
        [Fact]
        public void CustomPropertyTest()
        {
            var testProperty = new Property(false, "data.value", DataType.Double);
            var result = testProperty.ToInputJProperty();

            var expected = TestType1JProperties.Value;

            Assert.True(JToken.DeepEquals(result, expected));
            
        }

        [Fact]
        public void BuiltInPropertyTest()
        {
            var testProperty = Property.EventTimeStamp;
            var result = testProperty.ToInputJProperty();

            var expected = TestType1JProperties.Date;

            Assert.True(JToken.DeepEquals(result, expected));

        }
    }
}
