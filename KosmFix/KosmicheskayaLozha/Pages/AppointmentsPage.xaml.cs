using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Pages
{
    public partial class AppointmentsPage : Page
    {
        private readonly int _masterId;
        private readonly int _serviceTypeId;

        public AppointmentsPage(int masterId, int serviceTypeId, string title)
        {
            InitializeComponent();
            _masterId      = masterId;
            _serviceTypeId = serviceTypeId;
            TxtTitle.Text  = title;
            Loaded += (s, e) => LoadSlots();
        }

        private void LoadSlots()
        {
            WpSlots.Children.Clear();
            var now  = DateTime.Now;
            DateTime? date = DpDate.SelectedDate;

            List<Appointments> slots;
            using (var db = new KosmicheskayaLozhaEntities())
            {
                var q = db.Appointments
                          .Include("Users1")
                          .Include("ServiceTypes")
                          .Where(a => a.MasterId == _masterId
                                   && a.ServiceTypeId == _serviceTypeId
                                   && a.ClientId == null
                                   && !a.IsCancelled && !a.IsCompleted
                                   && a.AppointmentDateTime > now);

                if (date.HasValue)
                    q = q.Where(a => System.Data.Entity.DbFunctions.TruncateTime(a.AppointmentDateTime)
                                     == date.Value.Date);

                slots = q.OrderBy(a => a.AppointmentDateTime).ToList();
            }

            if (slots.Count == 0)
            {
                WpSlots.Children.Add(new TextBlock
                {
                    Text       = "Нет доступных записей",
                    Foreground = (System.Windows.Media.Brush)FindResource("SubtextBrush"),
                    FontSize   = 16, Margin = new Thickness(0, 20, 0, 0),
                });
                return;
            }

            foreach (var slot in slots)
            {
                var card = new Border
                {
                    Style  = (Style)FindResource("Card"),
                    Width  = 160, Margin = new Thickness(0, 0, 12, 12),
                    Cursor = System.Windows.Input.Cursors.Hand,
                };
                var sp = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
                sp.Children.Add(new TextBlock
                {
                    Text          = slot.AppointmentDateTime.ToString("dd MMMM"),
                    Foreground    = (System.Windows.Media.Brush)FindResource("AccentBrush"),
                    FontSize      = 14, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center,
                });
                sp.Children.Add(new TextBlock
                {
                    Text          = slot.AppointmentDateTime.ToString("HH:mm"),
                    Foreground    = (System.Windows.Media.Brush)FindResource("TextBrush"),
                    FontSize      = 28, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center,
                });
                var btnBook = new Button
                {
                    Content = "Записаться",
                    Style   = (Style)FindResource("PrimaryButton"),
                    Margin  = new Thickness(0, 8, 0, 0),
                    Tag     = slot.Id,
                };
                btnBook.Click += BtnBook_Click;
                sp.Children.Add(btnBook);
                card.Child = sp;
                WpSlots.Children.Add(card);
            }
        }

        private void BtnBook_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show("Войдите в аккаунт, чтобы записаться.", "Требуется авторизация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.Instance.Navigate(new LoginPage());
                return;
            }
            MainWindow.Instance.Navigate(new AppointmentDetailPage(id));
        }

        private void DpDate_Changed(object sender, SelectionChangedEventArgs e) => LoadSlots();
        private void BtnResetDate_Click(object sender, RoutedEventArgs e)
        { DpDate.SelectedDate = null; LoadSlots(); }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.GoBack();
    }
}
