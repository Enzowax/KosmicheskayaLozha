namespace KosmicheskayaLozha
{
    // Эти ToString() нужны чтобы ComboBox/ListBox корректно
    // отображал EF-сущности без DisplayMemberPath
    // (Dynamic Proxy не умеет сам выводить Name)

    public partial class ServiceTypes
    {
        public override string ToString() => Name ?? "";
    }

    public partial class Manufacturers
    {
        public override string ToString() => Name ?? "";
    }

    public partial class ProductTypes
    {
        public override string ToString() => Name ?? "";
    }

    public partial class Roles
    {
        public override string ToString() => Name ?? "";
    }
}
