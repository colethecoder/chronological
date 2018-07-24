using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Chronological.Tests
{
    public class MeasureExpressionTests
    {
        [Fact]
        public void Test1()
        {
            var measure = new Measure<TestType1>(Property<double>.Create<TestType1>(x => x.Value), Measure.MaximumMeasureExpression);

            Assert.Equal(measure.MeasureType, Measure.MaximumMeasureExpression);
            Assert.True(JToken.DeepEquals(measure.Property.ToInputJProperty(), TestType1JProperties.Value));
        }

        [Fact]
        public void WhenUsingFirstInMeasureWithoutOrderByShouldReturnCorrectJson()
        {
            var measure = new Measure<TestType1>(Property<double>.Create<TestType1>(x => x.Value),
                Measure.FirstMeasureExpression);

            Assert.Equal(measure.MeasureType, Measure.FirstMeasureExpression);
            Assert.True(JToken.DeepEquals(measure.ToJProperty(), TestType1JProperties.FirstMeasureWithoutOrderBy));
        }

        [Fact]
        public void WhenUsingLastInMeasureWithoutOrderByShouldReturnCorrectJson()
        {
            var measure = new Measure<TestType1>(Property<double>.Create<TestType1>(x => x.Value),
                Measure.LastMeasureExpression);

            Assert.Equal(measure.MeasureType, Measure.LastMeasureExpression);
            Assert.True(JToken.DeepEquals(measure.ToJProperty(), TestType1JProperties.LastMeasureWithoutOrderBy));
        }

        [Fact]
        public void WhenUsingFirstInMeasureWithOrderByShouldReturnCorrectJson()
        {
            var measure = new Measure<TestType1>(Property<double>.Create<TestType1>(x => x.Value),
                Measure.FirstMeasureExpression, Property<DateTime>.Create<TestType1>(x => x.Date));

            Assert.Equal(measure.MeasureType, Measure.FirstMeasureExpression);
            Assert.True(JToken.DeepEquals(measure.ToJProperty(), TestType1JProperties.FirstMeasureWithOrderBy));
        }

        [Fact]
        public void WhenUsingLastInMeasureWithOrderByShouldReturnCorrectJson()
        {
            var measure = new Measure<TestType1>(Property<double>.Create<TestType1>(x => x.Value),
                Measure.LastMeasureExpression, Property<DateTime>.Create<TestType1>(x => x.Date));

            Assert.Equal(measure.MeasureType, Measure.LastMeasureExpression);
            Assert.True(JToken.DeepEquals(measure.ToJProperty(), TestType1JProperties.LastMeasureWithOrderBy));
        }

        [Fact]
        public void CountMeasureShouldHaveEmptyBody()
        {
            var measure = new Measure<int>(null, Measure.CountMeasureExpression);

            var expected = new JProperty("count", new JObject());

            Assert.True(JToken.DeepEquals(measure.ToJProperty(), expected));
        }
    }
}
