using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Chronological.Samples;

namespace Chronological.Benchmarks
{
    [MemoryDiagnoser]
    public class AggregateParsing
    {
        [Benchmark]
        public WebSocketResult<Aggregate<TestType1, string,
                                        Aggregate<TestType1, string,
                                            Aggregate<TestType1, DateTime, AggregateResultType1>>>> ParseJsonAggregates()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Chronological.Benchmarks.Data.aggregates.json";

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
                return result;
            }

        }
    }
}
