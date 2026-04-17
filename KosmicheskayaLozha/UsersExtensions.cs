using System.Linq;

namespace KosmicheskayaLozha
{
    public partial class Users
    {
        public string FullName =>
            $"{LastName} {FirstName} {MiddleName}".Trim();

        public string ShortName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName)) return LastName;
                string mid = string.IsNullOrEmpty(MiddleName) ? "" : $" {MiddleName[0]}.";
                return $"{LastName} {FirstName[0]}.{mid}";
            }
        }

        public string RoleName => Roles?.Name ?? "";

        // Нужен для ComboBox без DisplayMemberPath
        public override string ToString() => FullName;
    }
}
