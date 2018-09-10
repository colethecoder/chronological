using System;
using BenchmarkDotNet.Running;

namespace Chronological.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<QueryCreation>();
            //var summary = BenchmarkRunner.Run<ResultParsing>();
            var summary = BenchmarkRunner.Run<AggregateParsing>();
            Console.ReadKey();
        }
    }
}
