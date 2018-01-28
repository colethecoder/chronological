using Newtonsoft.Json.Linq;

namespace Chronological
{
    public class Order
    {
        private readonly string _order;
        private Order(string order)
        {
            _order = order;
        }

        public static Order Ascending()
        {
            return new Order("Asc");
        }

        public static Order Descending()
        {
            return new Order("Desc");
        }

        internal JProperty ToJProperty()
        {
            return new JProperty("order", _order);
        }
    }
}
