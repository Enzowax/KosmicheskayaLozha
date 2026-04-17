using System.Linq;

namespace KosmicheskayaLozha
{
    public partial class Orders
    {
        public string ClientName  => Users?.FullName ?? "—";
        public string StatusText  => IsClosed ? "Выдан" : "В обработке";
        public decimal Total      => OrderItems.Sum(i => i.Price * i.Quantity);
    }

    public partial class OrderItems
    {
        public string  ProductName => Products?.Name ?? "—";
        public decimal TotalPrice  => Price * Quantity;
    }
}
