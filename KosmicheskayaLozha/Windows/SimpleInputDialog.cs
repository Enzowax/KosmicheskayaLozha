using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Windows
{
    public class SimpleInputDialog : Window
    {
        public string InputText { get; private set; }
        private readonly TextBox _box;

        public SimpleInputDialog(string prompt, string defaultValue = "")
        {
            Title  = "Ввод"; Width = 380; Height = 180;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode            = ResizeMode.NoResize;
            Background            = (System.Windows.Media.Brush)Application.Current.Resources["BackgroundBrush"];

            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var lbl = new TextBlock
            {
                Text       = prompt,
                Foreground = (System.Windows.Media.Brush)Application.Current.Resources["SubtextBrush"],
                FontSize   = 13, Margin = new Thickness(0, 0, 0, 6),
            };
            Grid.SetRow(lbl, 0);

            _box = new TextBox
            {
                Text   = defaultValue,
                Style  = (Style)Application.Current.Resources["ModernTextBox"],
                Margin = new Thickness(0, 0, 0, 12),
            };
            Grid.SetRow(_box, 1);

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var btnOk = new Button { Content = "OK", Style = (Style)Application.Current.Resources["PrimaryButton"], Width = 80, Margin = new Thickness(0, 0, 8, 0) };
            btnOk.Click += (s, e) => { InputText = _box.Text.Trim(); DialogResult = true; Close(); };
            var btnCancel = new Button { Content = "Отмена", Style = (Style)Application.Current.Resources["BackButton"], Width = 80 };
            btnCancel.Click += (s, e) => { DialogResult = false; Close(); };
            btnPanel.Children.Add(btnOk); btnPanel.Children.Add(btnCancel);
            Grid.SetRow(btnPanel, 2);

            grid.Children.Add(lbl); grid.Children.Add(_box); grid.Children.Add(btnPanel);
            Content = grid;
            Loaded += (s, e) => { _box.Focus(); _box.SelectAll(); };
        }
    }
}
