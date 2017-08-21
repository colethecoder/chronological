using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Dimension
    {
        private readonly string _dimensionType;
        private readonly Property _property;
        private readonly Breaks _breaks;

        private Dimension(string dimensionType, Property property, Breaks breaks)
        {
            _dimensionType = dimensionType;
            _property = property;
            _breaks = breaks;
        }

        public static Dimension DateHistogram(Property property, Breaks breaks)
        {
            return new Dimension("dateHistogram", property, breaks);
        }

        internal JProperty ToJProperty()
        {
            return new JProperty("dimension", new JObject(
                new JProperty(_dimensionType, new JObject(
                    _property.ToInputJProperty(),
                    _breaks.ToJProperty()
                ))));
        }
    }
}