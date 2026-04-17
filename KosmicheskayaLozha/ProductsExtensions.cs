using System;

namespace KosmicheskayaLozha
{
    public partial class Products
    {
        public string ManufacturerName => Manufacturers?.Name ?? "—";
        public string ProductTypeName  => ProductTypes?.Name  ?? "—";

        public decimal FinalPrice =>
            Discount > 0 ? Math.Round(Price * (1 - Discount / 100m), 2) : Price;

        public bool HasBigDiscount => Discount > 15;

        public override string ToString() => Name;
    }
}
