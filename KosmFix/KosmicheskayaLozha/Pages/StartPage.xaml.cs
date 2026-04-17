using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KosmicheskayaLozha.Pages
{
    public partial class StartPage : Page
    {
        private List<ServiceTypes>  _services;
        private List<Users>         _masters;
        private ServiceTypes        _selectedService;
        private Users               _selectedMaster;

        public StartPage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                UpdateAuthButtons();
                LoadFilters();
                LoadServices();
            };
        }

        private void UpdateAuthButtons()
        {
            bool logged = Session.IsLoggedIn;
            BtnLogin.Visibility   = logged ? Visibility.Collapsed : Visibility.Visible;
            BtnLogout.Visibility  = logged ? Visibility.Visible   : Visibility.Collapsed;
            BtnAccount.Visibility = logged && Session.IsClient ? Visibility.Visible : Visibility.Collapsed;

            if (!logged) return;
            if (Session.IsMaster)  { MainWindow.Instance.Navigate(new MasterPage());  return; }
            if (Session.IsManager) { MainWindow.Instance.Navigate(new ManagerPage()); return; }
            if (Session.IsAdmin)   { MainWindow.Instance.Navigate(new AdminPage());   return; }
        }

        private void LoadFilters()
        {
            using (var db = new KosmicheskayaLozhaEntities())
            {
                var services = db.ServiceTypes.OrderBy(s => s.Name).ToList();
                var masters  = db.Users.Where(u => u.RoleId == 2 && !u.IsFrozen)
                                       .OrderBy(u => u.LastName).ToList();

                CmbServiceFilter.Items.Clear();
                CmbServiceFilter.Items.Add(new ServiceTypes { Id = 0, Name = "— Все услуги —", Price = 0 });
                services.ForEach(s => CmbServiceFilter.Items.Add(s));
                CmbServiceFilter.SelectedIndex = 0;

                CmbMasterFilter.Items.Clear();
                CmbMasterFilter.Items.Add(new Users { Id = 0, LastName = "— Все мастера —", FirstName = "", Password = "", Login = "" });
                masters.ForEach(m => CmbMasterFilter.Items.Add(m));
                CmbMasterFilter.SelectedIndex = 0;
            }
        }

        private void LoadServices()
        {
            int serviceId = (CmbServiceFilter.SelectedItem as ServiceTypes)?.Id ?? 0;
            int masterId  = (CmbMasterFilter.SelectedItem  as Users)?.Id        ?? 0;

            using (var db = new KosmicheskayaLozhaEntities())
            {
                var q = db.ServiceTypes.AsQueryable();
                if (serviceId > 0) q = q.Where(s => s.Id == serviceId);
                if (masterId  > 0) q = q.Where(s => s.Users.Any(u => u.Id == masterId));
                _services = q.OrderBy(s => s.Name).ToList();
            }

            LstServices.ItemsSource = _services;
            LstMasters.ItemsSource  = null;
            WpSlots.Children.Clear();
            TxtSlotsTitle.Text    = "";
            TxtNoSlots.Visibility = Visibility.Collapsed;
        }

        private void LstServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedService = LstServices.SelectedItem as ServiceTypes;
            if (_selectedService == null) return;

            TxtMastersTitle.Text = $"Мастера по услуге «{_selectedService.Name}»";
            int masterId = (CmbMasterFilter.SelectedItem as Users)?.Id ?? 0;

            using (var db = new KosmicheskayaLozhaEntities())
            {
                var st = db.ServiceTypes.Include("Users").FirstOrDefault(s => s.Id == _selectedService.Id);
                _masters = st?.Users.Where(u => !u.IsFrozen && (masterId == 0 || u.Id == masterId))
                                     .OrderBy(u => u.LastName).ToList() ?? new List<Users>();
            }

            LstMasters.ItemsSource = _masters;
            WpSlots.Children.Clear();
            TxtSlotsTitle.Text    = "";
            TxtNoSlots.Visibility = Visibility.Collapsed;
        }

        private void LstMasters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMaster = LstMasters.SelectedItem as Users;
            if (_selectedMaster != null && _selectedService != null)
                LoadSlots();
        }

        private void LoadSlots()
        {
            WpSlots.Children.Clear();
            var now = DateTime.Now;

            List<Appointments> slots;
            using (var db = new KosmicheskayaLozhaEntities())
            {
                slots = db.Appointments
                          .Include("Users")
                          .Include("Users1")
                          .Include("ServiceTypes")
                          .Where(a => a.MasterId == _selectedMaster.Id
                                   && a.ServiceTypeId == _selectedService.Id
                                   && a.ClientId == null
                                   && !a.IsCancelled && !a.IsCompleted
                                   && a.AppointmentDateTime > now)
                          .OrderBy(a => a.AppointmentDateTime)
                          .ToList();
            }

            TxtSlotsTitle.Text    = $"Доступные записи к {_selectedMaster.ShortName}";
            TxtNoSlots.Visibility = slots.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            foreach (var slot in slots)
            {
                var btn = new Button
                {
                    Content  = slot.AppointmentDateTime.ToString("dd MMM\nHH:mm"),
                    Width    = 90, Height = 60,
                    Margin   = new Thickness(0, 0, 8, 8),
                    FontSize = 12,
                    Style    = (Style)FindResource("PrimaryButton"),
                    Tag      = slot,
                };
                btn.Click += SlotBtn_Click;
                WpSlots.Children.Add(btn);
            }
        }

        private void SlotBtn_Click(object sender, RoutedEventArgs e)
        {
            var slot = ((Button)sender).Tag as Appointments;
            if (slot == null) return;
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show("Для записи войдите в аккаунт.", "Требуется авторизация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.Instance.Navigate(new LoginPage());
                return;
            }
            MainWindow.Instance.Navigate(new AppointmentDetailPage(slot.Id));
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e) => LoadServices();
        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CmbServiceFilter.SelectedIndex = 0;
            CmbMasterFilter.SelectedIndex  = 0;
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.Navigate(new LoginPage());
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        { Session.Logout(); MainWindow.Instance.Navigate(new StartPage()); }
        private void BtnProducts_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.Navigate(new ProductsPage());
        private void BtnAccount_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.Navigate(new AccountPage());
    }
}
