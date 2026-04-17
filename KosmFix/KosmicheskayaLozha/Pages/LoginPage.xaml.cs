using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage() => InitializeComponent();

        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.Navigate(new StartPage());

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login    = TxtLogin.Text.Trim();
            var password = TxtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            { ShowError("Введите логин и пароль."); return; }

            using (var db = new KosmicheskayaLozhaEntities())
            {
                var user = db.Users.Include("Roles")
                             .FirstOrDefault(u => u.Login == login
                                               && u.Password == password
                                               && !u.IsFrozen);
                if (user == null)
                { ShowError("Неверный логин / пароль или аккаунт заморожен."); return; }

                Session.Login(user);
            }

            if (Session.IsMaster)  MainWindow.Instance.Navigate(new MasterPage());
            else if (Session.IsManager) MainWindow.Instance.Navigate(new ManagerPage());
            else if (Session.IsAdmin)   MainWindow.Instance.Navigate(new AdminPage());
            else                        MainWindow.Instance.Navigate(new StartPage());
        }

        private void ShowError(string msg)
        {
            TxtError.Text       = msg;
            TxtError.Visibility = Visibility.Visible;
        }
    }
}
