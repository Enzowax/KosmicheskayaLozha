using System.Windows;
using KosmicheskayaLozha.Pages;

namespace KosmicheskayaLozha
{
    public partial class MainWindow : Window
    {
        private static MainWindow _instance;
        public static MainWindow Instance => _instance;

        public MainWindow()
        {
            InitializeComponent();
            _instance = this;
            Navigate(new StartPage());
        }

        public void Navigate(System.Windows.Controls.Page page)
        {
            MainFrame.Navigate(page);
        }

        public void GoBack()
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }
    }
}
