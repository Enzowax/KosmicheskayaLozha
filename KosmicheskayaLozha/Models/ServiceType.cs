namespace KosmicheskayaLozha.Models
{
    public class ServiceType
    {
        public int     Id    { get; set; }
        public string  Name  { get; set; }
        public decimal Price { get; set; }

        public override string ToString() => Name;
    }

    public class Manufacturer
    {
        public int    Id   { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }

    public class ProductType
    {
        public int    Id   { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}
