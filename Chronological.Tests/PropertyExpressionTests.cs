using Xunit;

namespace Chronological.Tests
{
    public class PropertyExpressionTests
    {
        [Fact]
        public void Test1()
        {
            var property = Property<double>.Create<TestType1>(y => y.Value);

            Assert.Equal(property.DataType.TimeSeriesInsightsType, DataType.Double.TimeSeriesInsightsType);
            Assert.Equal(property.Name, "data.value");
            Assert.Equal(property._isBuiltIn, false);
        }
    }
}
