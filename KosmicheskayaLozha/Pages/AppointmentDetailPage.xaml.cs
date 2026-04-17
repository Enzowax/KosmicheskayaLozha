using System.Linq;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Pages
{
    public partial class AppointmentDetailPage : Page
    {
        private readonly int _appointmentId;
        private Appointments _apt;

        public AppointmentDetailPage(int appointmentId)
        {
            InitializeComponent();
            _appointmentId = appointmentId;
            Loaded += (s, e) => Fill();
        }

        private void Fill()
        {
            using (var db = DbHelper.CreateContext())
            {
                _apt = db.Appointments
                         .Include("Users1")
                         .Include("ServiceTypes")
                         .FirstOrDefault(a => a.Id == _appointmentId);
            }
            if (_apt == null) return;

            TxtService.Text = _apt.ServiceTypeName;
            TxtMaster.Text  = _apt.MasterFullName;
            TxtDate.Text    = _apt.AppointmentDateTime.ToString("dd MMMM yyyy, HH:mm");
            TxtPrice.Text   = $"{_apt.ServiceTypePrice:N2} ₽";
        }

        private void BtnBook_Click(object sender, RoutedEventArgs e)
        {
            var payment = (CmbPayment.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var comment = TxtComment.Text.Trim();

            var result = MessageBox.Show(
                $"Подтвердите запись:\n\n" +
                $"Услуга: {_apt.ServiceTypeName}\n" +
                $"Мастер: {_apt.MasterFullName}\n" +
                $"Дата: {_apt.AppointmentDateTime:dd.MM.yyyy HH:mm}\n" +
                $"Оплата: {payment}\n" +
                $"Стоимость: {_apt.ServiceTypePrice:N2} ₽",
                "Подтверждение записи",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            using (var db = DbHelper.CreateContext())
            {
                var apt = db.Appointments.Find(_appointmentId);
                apt.ClientId      = Session.CurrentUser.Id;
                apt.PaymentMethod = payment;
                apt.Comment       = string.IsNullOrEmpty(comment) ? null : comment;
                db.SaveChanges();
            }

            MessageBox.Show("✅ Вы успешно записаны!", "Запись создана",
                MessageBoxButton.OK, MessageBoxImage.Information);
            MainWindow.Instance.Navigate(new AccountPage());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.GoBack();
    }
}
