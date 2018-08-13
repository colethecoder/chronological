using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Chronological.Samples;
using System.IO;
using System.Reflection;

namespace Chronological.Benchmarks
{
    [MemoryDiagnoser]
    public class ResultParsing
    {
        [Benchmark]
        public WebSocketResult<TestType1> ParseJsonEvents()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Chronological.Benchmarks.Data.events.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var result = new WebSocketReader<TestType1>(
                                new EventWebSocketRepository(null)
                                    .ParseEvents<TestType1>)
                                        .Read(reader);
                return result;
            }
            
        }

        [Benchmark]
        public WebSocketResult<TestType1> ParseJsonEvents_New()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Chronological.Benchmarks.Data.events.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var result = FastEventParser.ParseEvents<TestType1>(reader);
                return result;
            }
        }
    }
}
