namespace Chronological.Samples
{
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
