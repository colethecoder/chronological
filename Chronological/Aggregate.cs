using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Aggregate
    {
        private Dimension _dimension;
        private List<Measure> _measures;

        private Aggregate()
        {
        }

        private Aggregate WithDimension(Dimension dimension)
        {
            _dimension = dimension;
            return this;
        }

        public static Aggregate DateHistogram(Property property, Breaks breaks)
        {
            return new Aggregate().WithDimension(Dimension.DateHistogram(property, breaks));
        }

        public Aggregate WithMeasure(Measure measure)
        {
            if (_measures == null)
            {
                _measures = new List<Measure>();
            }
            _measures.Add(measure);
            return this;
        }

        private JArray GetMeasuresJArray()
        {
            var array = new JArray();
            foreach (var measure in _measures)
            {
                array.Add(new JObject(measure.ToJProperty()));
            }
            return array;
        }

        internal JObject ToJObject()
        {
            return new JObject(
                new JProperty("measures", GetMeasuresJArray()),
                _dimension.ToJProperty());
        }
    }
}
