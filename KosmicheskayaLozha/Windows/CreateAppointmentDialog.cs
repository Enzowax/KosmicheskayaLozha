using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Windows
{
    public class CreateAppointmentDialog : Window
    {
        private ComboBox   _cmbMaster;
        private ComboBox   _cmbService;
        private ComboBox   _cmbClient;
        private DatePicker _dpDate;
        private ComboBox   _cmbHour;
        private ComboBox   _cmbMinute;
        private TextBlock  _txtError;
        private TextBox    _txtSearch;

        public CreateAppointmentDialog()
        {
            Title  = "Создать запись";
            Width  = 440;
            Height = 520;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode            = ResizeMode.NoResize;
            Background            = (System.Windows.Media.Brush)Application.Current.Resources["BackgroundBrush"];

            var sv = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            var sp = new StackPanel { Margin = new Thickness(20) };

            sp.Children.Add(new TextBlock
            {
                Text       = "➕ Новая запись",
                Foreground = (System.Windows.Media.Brush)Application.Current.Resources["AccentBrush"],
                FontSize   = 18, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 12),
            });

            sp.Children.Add(Lbl("Мастер:"));
            _cmbMaster = Cmb("FullName");
            sp.Children.Add(_cmbMaster);

            sp.Children.Add(Lbl("Тип услуги:"));
            _cmbService = Cmb("Name");
            sp.Children.Add(_cmbService);

            sp.Children.Add(Lbl("Поиск клиента (ФИО / телефон):"));
            var searchRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
            _txtSearch = new TextBox { Style = (Style)Application.Current.Resources["ModernTextBox"], Width = 250, Margin = new Thickness(0, 0, 8, 0) };
            var btnSearch = new Button { Content = "🔍", Style = (Style)Application.Current.Resources["PrimaryButton"], Width = 36 };
            btnSearch.Click += (s, e) => SearchClients();
            searchRow.Children.Add(_txtSearch);
            searchRow.Children.Add(btnSearch);
            sp.Children.Add(searchRow);

            _cmbClient = Cmb("FullName");
            sp.Children.Add(_cmbClient);

            sp.Children.Add(Lbl("Дата:"));
            _dpDate = new DatePicker
            {
                Foreground       = (System.Windows.Media.Brush)Application.Current.Resources["TextBrush"],
                Margin           = new Thickness(0, 0, 0, 10),
                DisplayDateStart = DateTime.Today,
                SelectedDate     = DateTime.Today.AddDays(1),
            };
            sp.Children.Add(_dpDate);

            sp.Children.Add(Lbl("Время:"));
            var timeRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
            _cmbHour = new ComboBox { Style = (Style)Application.Current.Resources["ModernComboBox"], Width = 70, Margin = new Thickness(0, 0, 8, 0) };
            for (int h = 8; h <= 20; h++) _cmbHour.Items.Add(h.ToString("D2"));
            _cmbHour.SelectedIndex = 1;
            _cmbMinute = new ComboBox { Style = (Style)Application.Current.Resources["ModernComboBox"], Width = 70 };
            _cmbMinute.Items.Add("00"); _cmbMinute.Items.Add("30"); _cmbMinute.SelectedIndex = 0;
            timeRow.Children.Add(SubLbl("ч:")); timeRow.Children.Add(_cmbHour);
            timeRow.Children.Add(SubLbl("  мин:")); timeRow.Children.Add(_cmbMinute);
            sp.Children.Add(timeRow);

            _txtError = new TextBlock
            {
                Foreground = (System.Windows.Media.Brush)Application.Current.Resources["DangerBrush"],
                FontSize   = 11, Visibility = Visibility.Collapsed, Margin = new Thickness(0, 0, 0, 6),
            };
            sp.Children.Add(_txtError);

            var btnRow = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var btnOk = new Button { Content = "💾 Создать", Style = (Style)Application.Current.Resources["SuccessButton"], Margin = new Thickness(0, 0, 8, 0) };
            btnOk.Click += BtnOk_Click;
            var btnCancel = new Button { Content = "Отмена", Style = (Style)Application.Current.Resources["BackButton"] };
            btnCancel.Click += (s, e) => { DialogResult = false; Close(); };
            btnRow.Children.Add(btnOk); btnRow.Children.Add(btnCancel);
            sp.Children.Add(btnRow);

            sv.Content = sp;
            Content    = sv;
            Loaded    += (s, e) => LoadCombos();
        }

        private void LoadCombos()
        {
            using (var db = DbHelper.CreateContext())
            {
                _cmbMaster.ItemsSource  = db.Users.Where(u => u.RoleId == 2 && !u.IsFrozen).OrderBy(u => u.LastName).ToList();
                _cmbService.ItemsSource = db.ServiceTypes.OrderBy(s => s.Name).ToList();
            }
        }

        private void SearchClients()
        {
            var q = _txtSearch.Text.Trim();
            using (var db = DbHelper.CreateContext())
            {
                _cmbClient.ItemsSource = db.Users
                    .Where(u => u.RoleId == 1 &&
                               (u.LastName.Contains(q) || u.FirstName.Contains(q) || u.Phone.Contains(q)))
                    .OrderBy(u => u.LastName).ToList();
            }
            _cmbClient.SelectedIndex = _cmbClient.Items.Count > 0 ? 0 : -1;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            var master  = _cmbMaster.SelectedItem  as Users;
            var service = _cmbService.SelectedItem as ServiceTypes;
            if (master  == null) { ShowError("Выберите мастера.");    return; }
            if (service == null) { ShowError("Выберите тип услуги."); return; }
            if (!_dpDate.SelectedDate.HasValue) { ShowError("Выберите дату."); return; }

            int h = int.Parse(_cmbHour.SelectedItem.ToString());
            int m = int.Parse(_cmbMinute.SelectedItem.ToString());
            var dt = _dpDate.SelectedDate.Value.Date.AddHours(h).AddMinutes(m);

            using (var db = DbHelper.CreateContext())
            {
                var apt = new Appointments
                {
                    MasterId            = master.Id,
                    ServiceTypeId       = service.Id,
                    AppointmentDateTime = dt,
                    IsCompleted         = false,
                    IsCancelled         = false,
                };

                var client = _cmbClient.SelectedItem as Users;
                if (client != null)
                {
                    apt.ClientId      = client.Id;
                    apt.PaymentMethod = "Наличные";
                }

                db.Appointments.Add(apt);
                db.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void ShowError(string msg) { _txtError.Text = msg; _txtError.Visibility = Visibility.Visible; }

        private ComboBox Cmb(string displayPath) => new ComboBox
        {
            Style             = (Style)Application.Current.Resources["ModernComboBox"],
            DisplayMemberPath = displayPath,
            Margin            = new Thickness(0, 0, 0, 10),
        };
        private TextBlock Lbl(string t) => new TextBlock
        {
            Text       = t,
            Foreground = (System.Windows.Media.Brush)Application.Current.Resources["SubtextBrush"],
            FontSize   = 11, Margin = new Thickness(0, 0, 0, 3),
        };
        private TextBlock SubLbl(string t) => new TextBlock
        {
            Text              = t,
            Foreground        = (System.Windows.Media.Brush)Application.Current.Resources["SubtextBrush"],
            VerticalAlignment = VerticalAlignment.Center,
            Margin            = new Thickness(0, 0, 4, 0),
        };
    }
}
