using Xunit;

namespace Chronological.Tests
{
    public class PropertyExpressionTests
    {
        [Fact]
        public void Test1()
        {
            var property = Property<double>.Create<TestType1>(y => y.Value);

            Assert.Equal(property._dataType._dataType, DataType.Double._dataType);
            Assert.Equal(property._propertyName, "data.value");
            Assert.Equal(property._isBuiltIn, false);
        }
    }
}
