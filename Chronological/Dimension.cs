using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Dimension
    {
        private readonly string _dimensionType;
        private readonly Property _property;
        private readonly Breaks _breaks;
        private readonly Limit _limit;

        private Dimension(string dimensionType, Property property, Breaks breaks)
        {
            _dimensionType = dimensionType;
            _property = property;
            _breaks = breaks;
        }

        private Dimension(string dimensionType, Property property, Limit limit)
        {
            _dimensionType = dimensionType;
            _property = property;
            _limit = limit;
        }

        public static Dimension DateHistogram(Property property, Breaks breaks)
        {
            return new Dimension("dateHistogram", property, breaks);
        }

        public static Dimension UniqueValues(Property property, Limit limit)
        {
            return new Dimension("uniqueValues", property, limit);
        }

        internal JProperty ToJProperty()
        {
            if (_dimensionType == "uniqueValues")
            {
                return new JProperty("dimension", new JObject(
                    new JProperty(_dimensionType, new JObject(
                        _property.ToInputJProperty(),
                        _limit.ToJProperty()
                    ))));
            }
            return new JProperty("dimension", new JObject(
                    new JProperty(_dimensionType, new JObject(
                        _property.ToInputJProperty(),
                        _breaks.ToJProperty()
                    ))));
        }
    }
}