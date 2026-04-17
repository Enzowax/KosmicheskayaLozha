namespace KosmicheskayaLozha.Models
{
    public class User
    {
        public int    Id         { get; set; }
        public string Login      { get; set; }
        public string Password   { get; set; }
        public string FirstName  { get; set; }
        public string LastName   { get; set; }
        public string MiddleName { get; set; }
        public string Phone      { get; set; }
        public int    RoleId     { get; set; }
        public string RoleName   { get; set; }
        public bool   IsFrozen   { get; set; }

        public string FullName =>
            $"{LastName} {FirstName} {MiddleName}".Trim();

        public string ShortName =>
            string.IsNullOrEmpty(FirstName) ? LastName :
            $"{LastName} {FirstName[0]}.{(string.IsNullOrEmpty(MiddleName) ? "" : $" {MiddleName[0]}.")}";
    }
}
