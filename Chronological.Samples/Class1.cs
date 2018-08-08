using System;

namespace Chronological.Samples
{
    public static class EventQueries
    {
        public static string CreateSimpleEventQuery()
        {
            var testEnv = new Environment("foo", "bar");

            return testEnv.EventQuery<TestType1>(DateTime.UtcNow, DateTime.UtcNow, Limit.Take, 200)
                    .ToString();
        }

        public static string CreatePredicateEventQuery()
        {
            var testEnv = new Environment("foo", "bar");

            return testEnv.EventQuery<TestType1>(DateTime.UtcNow, DateTime.UtcNow, Limit.Take, 200)
                    .Where(x => x.DataType.Contains("foobar"))
                    .ToString();
        }
    }
}
