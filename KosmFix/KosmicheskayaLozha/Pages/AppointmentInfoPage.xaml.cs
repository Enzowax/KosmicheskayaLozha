using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Pages
{
    public partial class AppointmentInfoPage : Page
    {
        private readonly int _id;

        public AppointmentInfoPage(int appointmentId)
        {
            InitializeComponent();
            _id = appointmentId;
            Loaded += (s, e) => Fill();
        }

        private void Fill()
        {
            Appointments apt;
            using (var db = new KosmicheskayaLozhaEntities())
            {
                apt = db.Appointments
                        .Include("Users")
                        .Include("Users1")
                        .Include("ServiceTypes")
                        .FirstOrDefault(a => a.Id == _id);
            }
            if (apt == null) return;

            TxtDateTime.Text = apt.AppointmentDateTime.ToString("dd.MM.yyyy HH:mm");
            TxtService.Text  = apt.ServiceTypeName;
            TxtPrice.Text    = $"{apt.ServiceTypePrice:N2} ₽";
            TxtClient.Text   = apt.ClientId.HasValue ? apt.ClientFullName : "Свободна";
            TxtPhone.Text    = apt.ClientId.HasValue ? apt.ClientPhone : "—";
            TxtComment.Text  = string.IsNullOrEmpty(apt.Comment) ? "—" : apt.Comment;
            TxtStatus.Text   = apt.StatusText;

            BtnComplete.IsEnabled = apt.ClientId.HasValue && !apt.IsCompleted && !apt.IsCancelled;
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Отметить запись как выполненную?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            using (var db = new KosmicheskayaLozhaEntities())
            {
                var apt = db.Appointments.Find(_id);
                apt.IsCompleted = true;
                db.SaveChanges();
            }
            MessageBox.Show("Запись отмечена как выполненная.", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Fill();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.Navigate(new MasterPage());
    }
}
