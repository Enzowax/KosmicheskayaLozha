// Partial-классы: ToString() для всех EF-сущностей, чтобы ComboBox показывал нормальные значения,
// а не "System.Data.Entity.DynamicProxies.ServiceTypes_74F19FD929..."

namespace KosmicheskayaLozha
{
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

    // Users.ToString() — показываем ФИО в ComboBox мастеров/клиентов
    // FullName уже есть в UsersExtensions.cs
    public partial class Users
    {
        public override string ToString() => FullName;
    }
}
