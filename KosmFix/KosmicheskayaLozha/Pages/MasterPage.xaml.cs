using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Pages
{
    public class ServiceCheckItem
    {
        public ServiceTypes ServiceType { get; set; }
        public bool         IsChecked   { get; set; }
    }

    public partial class MasterPage : Page
    {
        private List<ServiceCheckItem> _serviceItems;

        public MasterPage()
        {
            InitializeComponent();
            Loaded += (s, e) => Load();
        }

        private void Load()
        {
            var u = Session.CurrentUser;
            TxtTitle.Text = $"👩‍🎨 Кабинет мастера — {u.FullName}";

            using (var db = new KosmicheskayaLozhaEntities())
            {
                // Все услуги + отмечаем те, что уже у мастера
                var masterServiceIds = db.Users
                    .Include("ServiceTypes")
                    .FirstOrDefault(x => x.Id == u.Id)
                    ?.ServiceTypes.Select(s => s.Id).ToHashSet()
                    ?? new System.Collections.Generic.HashSet<int>();

                _serviceItems = db.ServiceTypes.OrderBy(s => s.Name).ToList()
                    .Select(s => new ServiceCheckItem
                    {
                        ServiceType = s,
                        IsChecked   = masterServiceIds.Contains(s.Id),
                    }).ToList();

                LstAllServices.ItemsSource = _serviceItems;

                // Записи к мастеру
                DgAppointments.ItemsSource =
                    db.Appointments
                      .Include("Users")
                      .Include("ServiceTypes")
                      .Where(a => a.MasterId == u.Id && !a.IsCancelled)
                      .OrderBy(a => a.AppointmentDateTime)
                      .ToList();
            }
        }

        private void BtnSaveServices_Click(object sender, RoutedEventArgs e)
        {
            var selected = _serviceItems.Where(si => si.IsChecked)
                                        .Select(si => si.ServiceType.Id)
                                        .ToList();
            using (var db = new KosmicheskayaLozhaEntities())
            {
                var master = db.Users.Include("ServiceTypes").First(x => x.Id == Session.CurrentUser.Id);
                master.ServiceTypes.Clear();
                foreach (var id in selected)
                {
                    var st = db.ServiceTypes.Find(id);
                    if (st != null) master.ServiceTypes.Add(st);
                }
                db.SaveChanges();
            }
            MessageBox.Show("Список услуг обновлён!", "Сохранено",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Load();
        }

        private void DgAppointments_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var apt = DgAppointments.SelectedItem as Appointments;
            if (apt != null)
                MainWindow.Instance.Navigate(new AppointmentInfoPage(apt.Id));
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Session.Logout();
            MainWindow.Instance.Navigate(new StartPage());
        }
    }
}
