using System.Linq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Chronological.Tests
{
    public class MeasureExpressionTests
    {
        // Broken test to fiddle with build
        [Fact]
        public void Test1()
        {            
            var aggregateBuilder = new AggregateBuilder<TestType1>();
            var measure = aggregateBuilder.Maximum(x => x.Value);
            var measureProperty = measure.ToJProperty();
            Assert.Equal(measureProperty.Name, "max");
            Assert.Equal(measureProperty.Children().Count(), 1);
            
            Assert.Equal(measureProperty, new JProperty("Test"));
        }
    }
}
