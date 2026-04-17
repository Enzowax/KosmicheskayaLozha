using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Pages
{
    public partial class AccountPage : Page
    {
        public AccountPage()
        {
            InitializeComponent();
            Loaded += (s, e) => Load();
        }

        private void Load()
        {
            if (!Session.IsLoggedIn) { MainWindow.Instance.Navigate(new LoginPage()); return; }
            var u = Session.CurrentUser;

            TxtName.Text  = u.FullName;
            TxtPhone.Text = u.Phone ?? "Телефон не указан";
            TxtRole.Text  = u.RoleName;

            using (var db = new KosmicheskayaLozhaEntities())
            {
                DgAppointments.ItemsSource =
                    db.Appointments
                      .Include("Users1")
                      .Include("ServiceTypes")
                      .Where(a => a.ClientId == u.Id)
                      .OrderByDescending(a => a.AppointmentDateTime)
                      .ToList();

                DgOrders.ItemsSource =
                    db.Orders
                      .Include("OrderItems")
                      .Where(o => o.ClientId == u.Id)
                      .OrderByDescending(o => o.OrderDate)
                      .ToList();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.Navigate(new StartPage());
    }
}
