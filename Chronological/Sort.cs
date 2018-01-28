using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Sort
    {
        private readonly Property _property;
        private readonly Order _order;

        private Sort(Property property, Order order)
        {
            _property = property;
            _order = order;
        }

        public static Sort Ascending(Property property)
        {
            return new Sort(property, Order.Ascending());
        }

        public static Sort Descending(Property property)
        {
            return new Sort(property, Order.Descending());
        }

        internal JObject ToJObject()
        {
            return new JObject(_property.ToInputJProperty(), _order.ToJProperty());
        }
    }
}
