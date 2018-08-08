using BenchmarkDotNet.Attributes;

namespace Chronological.Benchmarks
{
    [MemoryDiagnoser]
    [RyuJitX64Job, LegacyJitX86Job]
    public class QueryCreation
    {
        [Benchmark]
        public void SimpleEventQueryToString()
        {
            var result = Samples.EventQueries.CreateSimpleEventQuery();
        }

        [Benchmark]
        public void PredicateWhereEventQueryToString()
        {
            var result = Samples.EventQueries.CreatePredicateEventQuery();
        }
    }
}
