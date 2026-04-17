using System;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Windows
{
    public class RescheduleDateDialog : Window
    {
        public DateTime? SelectedDateTime { get; private set; }
        private DatePicker _dp;
        private ComboBox   _cmbHour;
        private ComboBox   _cmbMinute;

        public RescheduleDateDialog()
        {
            Title  = "Перенос записи"; Width = 360; Height = 240;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode            = ResizeMode.NoResize;
            Background            = (System.Windows.Media.Brush)Application.Current.Resources["BackgroundBrush"];

            var sp = new StackPanel { Margin = new Thickness(20) };
            sp.Children.Add(new TextBlock
            {
                Text       = "✏ Новая дата и время",
                Foreground = (System.Windows.Media.Brush)Application.Current.Resources["AccentBrush"],
                FontSize   = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 12),
            });

            sp.Children.Add(Lbl("Дата:"));
            _dp = new DatePicker
            {
                Foreground       = (System.Windows.Media.Brush)Application.Current.Resources["TextBrush"],
                DisplayDateStart = DateTime.Today,
                SelectedDate     = DateTime.Today.AddDays(1),
                Margin           = new Thickness(0, 0, 0, 10),
            };
            sp.Children.Add(_dp);

            sp.Children.Add(Lbl("Время:"));
            var timeRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 16) };
            _cmbHour = new ComboBox { Style = (Style)Application.Current.Resources["ModernComboBox"], Width = 70, Margin = new Thickness(0, 0, 8, 0) };
            for (int h = 8; h <= 20; h++) _cmbHour.Items.Add(h.ToString("D2"));
            _cmbHour.SelectedIndex = 1;
            _cmbMinute = new ComboBox { Style = (Style)Application.Current.Resources["ModernComboBox"], Width = 70 };
            _cmbMinute.Items.Add("00"); _cmbMinute.Items.Add("30"); _cmbMinute.SelectedIndex = 0;

            timeRow.Children.Add(SubLbl("ч: ")); timeRow.Children.Add(_cmbHour);
            timeRow.Children.Add(SubLbl("  мин: ")); timeRow.Children.Add(_cmbMinute);
            sp.Children.Add(timeRow);

            var row = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var btnOk = new Button { Content = "Сохранить", Style = (Style)Application.Current.Resources["SuccessButton"], Margin = new Thickness(0, 0, 8, 0) };
            btnOk.Click += (s, e) =>
            {
                if (!_dp.SelectedDate.HasValue) return;
                int h = int.Parse(_cmbHour.SelectedItem.ToString());
                int m = int.Parse(_cmbMinute.SelectedItem.ToString());
                SelectedDateTime = _dp.SelectedDate.Value.Date.AddHours(h).AddMinutes(m);
                DialogResult = true; Close();
            };
            var btnCancel = new Button { Content = "Отмена", Style = (Style)Application.Current.Resources["BackButton"] };
            btnCancel.Click += (s, e) => { DialogResult = false; Close(); };
            row.Children.Add(btnOk); row.Children.Add(btnCancel);
            sp.Children.Add(row);
            Content = sp;
        }

        private TextBlock Lbl(string t) => new TextBlock
        {
            Text = t, Foreground = (System.Windows.Media.Brush)Application.Current.Resources["SubtextBrush"],
            Margin = new Thickness(0, 0, 0, 4),
        };
        private TextBlock SubLbl(string t) => new TextBlock
        {
            Text = t, Foreground = (System.Windows.Media.Brush)Application.Current.Resources["SubtextBrush"],
            VerticalAlignment = VerticalAlignment.Center,
        };
    }
}
