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
            var measure = new Measure<TestType1>(Property<double>.Create<TestType1>(x => x.Value), Measure.MaximumMeasureExpression);
            
            Assert.Equal(measure.MeasureType, Measure.MaximumMeasureExpression);
            Assert.True(JToken.DeepEquals(measure.Property.ToInputJProperty(), TestType1JProperties.Value));                       
        }
    }
}
