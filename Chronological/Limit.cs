using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Limit
    {
        private readonly string _limitType;
        private readonly int _count;
        private readonly List<Sort> _sorts;

        private Limit(string limitType, int count, List<Sort> sorts = null)
        {
            _limitType = limitType;
            _count = count;
            _sorts = sorts;
        }

        public static Limit Top(int count, Sort sort, params Sort[] additionalSorts)
        {
            var sorts = new List<Sort> {sort};
            sorts.AddRange(additionalSorts);
            return new Limit("top", count, sorts);
        }

        public static Limit Take(int count)
        {
            return new Limit("take", count);
        }

        public static Limit Sample(int count)
        {
            return new Limit("sample", count);
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
