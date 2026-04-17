namespace KosmicheskayaLozha
{
    public static class Session
    {
        public static Users CurrentUser { get; private set; }

        public static bool IsLoggedIn => CurrentUser != null;
        public static bool IsClient   => CurrentUser?.RoleId == 1;
        public static bool IsMaster   => CurrentUser?.RoleId == 2;
        public static bool IsManager  => CurrentUser?.RoleId == 3;
        public static bool IsAdmin    => CurrentUser?.RoleId == 4;

        public static void Login(Users user) => CurrentUser = user;
        public static void Logout()          => CurrentUser = null;
    }
}
