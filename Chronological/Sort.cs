using Newtonsoft.Json.Linq;

namespace Chronological
{
    public interface ISortOrder
    {
        string OrderName { get; }
    }

    public class Ascending : ISortOrder
    {
        public string OrderName { get; } = "Asc";
    }

    public class Descending : ISortOrder
    {
        public string OrderName { get; } = "Desc";
    }

    public class Sort
    {
        private readonly Property _property;
        private readonly ISortOrder _sortOrder;

        public static ISortOrder Ascending
        {
            get => new Ascending();
        }

        public static ISortOrder Descending
        {
            get => new Descending();
        }

        private Sort(ISortOrder sortOrder, Property property)
        {
            _property = property;
            _sortOrder = sortOrder;
        }

        internal static Sort Create(ISortOrder sortOrder, Property property)
        {
            return new Sort(sortOrder, property);
        }

        internal JObject ToJObject()
        {
            return new JObject(_property.ToInputJProperty(), new JProperty("order", _sortOrder.OrderName));
        }
    }
}
