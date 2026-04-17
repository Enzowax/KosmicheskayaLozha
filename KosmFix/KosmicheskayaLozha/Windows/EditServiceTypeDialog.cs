using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Windows
{
    public class EditServiceTypeDialog : Window
    {
        public string  ServiceName  { get; private set; }
        public decimal ServicePrice { get; private set; }

        private TextBox   _txtName;
        private TextBox   _txtPrice;
        private TextBlock _txtError;

        public EditServiceTypeDialog(ServiceTypes existing = null)
        {
            Title  = existing == null ? "Новый тип услуги" : "Редактировать услугу";
            Width  = 380; Height = 240;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode            = ResizeMode.NoResize;
            Background            = (System.Windows.Media.Brush)Application.Current.Resources["BackgroundBrush"];

            var sp = new StackPanel { Margin = new Thickness(20) };

            sp.Children.Add(Lbl("Название услуги:"));
            _txtName = new TextBox { Style = (Style)Application.Current.Resources["ModernTextBox"], Text = existing?.Name ?? "", Margin = new Thickness(0, 0, 0, 10) };
            sp.Children.Add(_txtName);

            sp.Children.Add(Lbl("Цена (₽):"));
            _txtPrice = new TextBox { Style = (Style)Application.Current.Resources["ModernTextBox"], Text = existing?.Price.ToString("F2") ?? "0", Margin = new Thickness(0, 0, 0, 6) };
            sp.Children.Add(_txtPrice);

            _txtError = new TextBlock { Foreground = (System.Windows.Media.Brush)Application.Current.Resources["DangerBrush"], FontSize = 11, Visibility = Visibility.Collapsed, Margin = new Thickness(0, 0, 0, 6) };
            sp.Children.Add(_txtError);

            var row = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 8, 0, 0) };
            var btnOk = new Button { Content = "💾 Сохранить", Style = (Style)Application.Current.Resources["SuccessButton"], Margin = new Thickness(0, 0, 8, 0) };
            btnOk.Click += BtnOk_Click;
            var btnCancel = new Button { Content = "Отмена", Style = (Style)Application.Current.Resources["BackButton"] };
            btnCancel.Click += (s, e) => { DialogResult = false; Close(); };
            row.Children.Add(btnOk); row.Children.Add(btnCancel);
            sp.Children.Add(row);
            Content = sp;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text)) { ShowError("Введите название."); return; }
            if (!decimal.TryParse(_txtPrice.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            { ShowError("Введите корректную цену."); return; }
            ServiceName = _txtName.Text.Trim(); ServicePrice = price;
            DialogResult = true; Close();
        }

        private void ShowError(string msg) { _txtError.Text = msg; _txtError.Visibility = Visibility.Visible; }
        private TextBlock Lbl(string t) => new TextBlock { Text = t, Foreground = (System.Windows.Media.Brush)Application.Current.Resources["SubtextBrush"], Margin = new Thickness(0, 0, 0, 4) };
    }
}
