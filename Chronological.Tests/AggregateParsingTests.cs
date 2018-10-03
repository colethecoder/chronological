using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace Chronological.Tests
{
    public class AggregateParsingTests
    {
        [Fact]
        public void Test1()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Chronological.Tests.Data.aggregates.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var aggregates = new List<Aggregate<TestType1, string,
                                        Aggregate<TestType1, string,
                                            Aggregate<TestType1, DateTime, AggregateResultType1>>>> {
                                    new AggregateBuilder<TestType1>().UniqueValues(x => x.DataType, 10,
                                       new AggregateBuilder<TestType1>().UniqueValues(x => x.Id, 10,
                                         new AggregateBuilder<TestType1>().DateHistogram(x => x.Date, Breaks.InDays(1),
                                           new AggregateResultType1
                                           (
                                               count: new AggregateBuilder<TestType1>().Count(),
                                               max: new AggregateBuilder<TestType1>().Maximum(x => x.Value),
                                               first: new AggregateBuilder<TestType1>().First(x => x.Value),
                                               last: new AggregateBuilder<TestType1>().Last(x => x.Value)
                                           )))) };

                var result = new WebSocketReader<
                                    Aggregate<TestType1, string,
                                        Aggregate<TestType1, string,
                                            Aggregate<TestType1, DateTime, AggregateResultType1>>>>
                                            (new AggregateWebSocketRepository(null)
                                                .ParseAggregates(aggregates))
                                        .Read(reader);

                Assert.IsType<WebSocketResult<
                    Aggregate<TestType1, string,
                        Aggregate<TestType1, string,
                            Aggregate<TestType1, DateTime, 
                                AggregateResultType1>>>>
                                    .WebSocketSuccess>(result);
            }
        }
    }
    public class AggregateResultType1
    {
        public Measure<int> Count { get; set; }
        public Measure<double?> Max { get; set; }
        public Measure<double?> First { get; set; }
        public Measure<double?> Last { get; set; }

        public AggregateResultType1() { }

        public AggregateResultType1(Measure<int> count, Measure<double?> max, Measure<double?> first, Measure<double?> last)
        {
            Count = count;
            Max = max;
            First = first;
            Last = last;
        }
    }
}
