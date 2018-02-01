using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public interface ILimit
    {

    }

    public interface ISortableLimit
    {
        string SortName { get; }
    }

    public interface INonSortableLimit
    {
        string SortName { get; }
    }

    public class Top : ISortableLimit
    {
        public string SortName { get; } = "top";        
    }

    public class Take : INonSortableLimit
    {
        public string SortName { get; } = "take";
    }

    public class Sample : INonSortableLimit
    {
        public string SortName { get; } = "sample";
    }

    public class Limit
    {
        private readonly string _limitType;
        private readonly int _count;
        private readonly List<Sort> _sorts;

        public static Top Top
        {
            get => new Top();
        }

        public static Take Take
        {
            get => new Take();
        }

        public static Sample Sample
        {
            get => new Sample();
        }

        internal Limit(string limitType, int count, List<Sort> sorts = null)
        {
            _limitType = limitType;
            _count = count;
            _sorts = sorts;
        }

        internal static Limit CreateLimit(INonSortableLimit limit, int count)
        {
            return new Limit(limit.SortName, count);
        }

        internal static Limit CreateLimit(ISortableLimit limit, int count, Sort sort)
        {
            return new Limit(limit.SortName, count, new List<Sort> { sort });
        }

        private JArray SortsToJArray()
        {
            var sortsJArray = new JArray();
            foreach (var sort in _sorts)
            {
                sortsJArray.Add(sort.ToJObject());
            }
            return sortsJArray;
        }

        internal JProperty ToJProperty()
        {
            if (_limitType == "top")
            {
                return new JProperty("top", new JObject(
                    new JProperty("sort", SortsToJArray()),
                    new JProperty("count", _count)));
            }

            return new JProperty(_limitType,_count);
        }
    }
}
