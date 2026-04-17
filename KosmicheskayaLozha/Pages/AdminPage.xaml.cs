using System.Linq;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using KosmicheskayaLozha.Windows;

namespace KosmicheskayaLozha.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadUsers();
        }

        private void LoadUsers()
        {
            TxtTitle.Text = $"🛡 Панель администратора — {Session.CurrentUser.FullName}";
            using (var db = DbHelper.CreateContext())
            {
                DgUsers.ItemsSource =
                    db.Users.Include("Roles")
                      .OrderBy(u => u.LastName)
                      .ToList();
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var w = new EditUserWindow(null) { Owner = Window.GetWindow(this) };
            if (w.ShowDialog() == true) LoadUsers();
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            var u = DgUsers.SelectedItem as Users;
            if (u == null) { Warn("Выберите пользователя."); return; }
            var w = new EditUserWindow(u) { Owner = Window.GetWindow(this) };
            if (w.ShowDialog() == true) LoadUsers();
        }

        private void BtnFreezeUser_Click(object sender, RoutedEventArgs e)
        {
            var u = DgUsers.SelectedItem as Users;
            if (u == null) { Warn("Выберите пользователя."); return; }
            if (u.Id == Session.CurrentUser.Id) { Warn("Нельзя заморозить самого себя."); return; }

            string action = u.IsFrozen ? "разморозить" : "заморозить";
            if (MessageBox.Show($"Вы хотите {action} пользователя {u.FullName}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            using (var db = DbHelper.CreateContext())
            {
                var user = db.Users.Find(u.Id);
                user.IsFrozen = !user.IsFrozen;
                db.SaveChanges();
            }
            LoadUsers();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        { Session.Logout(); MainWindow.Instance.Navigate(new StartPage()); }

        private void Warn(string msg) =>
            MessageBox.Show(msg, "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
