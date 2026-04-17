using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using KosmicheskayaLozha.Windows;

namespace KosmicheskayaLozha.Pages
{
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            Loaded += (s, e) => PageLoaded();
        }

        private void PageLoaded()
        {
            using (var db = DbHelper.CreateContext())
            {
                var types = db.ProductTypes.OrderBy(t => t.Name).ToList();
                CmbType.Items.Clear();
                CmbType.Items.Add(new ProductTypes { Id = 0, Name = "— Все типы —" });
                types.ForEach(t => CmbType.Items.Add(t));
                CmbType.SelectedIndex = 0;

                var mfrs = db.Manufacturers.OrderBy(m => m.Name).ToList();
                CmbManufacturer.Items.Clear();
                CmbManufacturer.Items.Add(new Manufacturers { Id = 0, Name = "— Все производители —" });
                mfrs.ForEach(m => CmbManufacturer.Items.Add(m));
                CmbManufacturer.SelectedIndex = 0;
            }
            LoadProducts();
        }

        private void LoadProducts()
        {
            var search = TxtSearch.Text.Trim();
            int typeId = (CmbType.SelectedItem         as ProductTypes)?.Id ?? 0;
            int mfrId  = (CmbManufacturer.SelectedItem as Manufacturers)?.Id ?? 0;

            System.Collections.Generic.List<Products> products;
            using (var db = DbHelper.CreateContext())
            {
                var q = db.Products.Include("Manufacturers").Include("ProductTypes")
                          .Where(p => !p.IsFrozen);
                if (!string.IsNullOrEmpty(search))
                    q = q.Where(p => p.Name.Contains(search));
                if (typeId > 0) q = q.Where(p => p.ProductTypeId == typeId);
                if (mfrId  > 0) q = q.Where(p => p.ManufacturerId == mfrId);
                products = q.OrderByDescending(p => p.Rating).ThenBy(p => p.Name).ToList();
            }

            WpProducts.Children.Clear();
            foreach (var p in products)
                WpProducts.Children.Add(CreateCard(p));
        }

        private Border CreateCard(Products p)
        {
            bool big = p.HasBigDiscount;
            var border = new Border
            {
                Width           = 200, Margin = new Thickness(0, 0, 12, 12),
                Background      = new SolidColorBrush(big ? Color.FromRgb(0x4A,0x1A,0x00) : Color.FromRgb(0x2D,0x00,0x50)),
                BorderBrush     = new SolidColorBrush(big ? Color.FromRgb(0xFF,0x6B,0x35) : Color.FromRgb(0x6A,0x0D,0xAD)),
                BorderThickness = new Thickness(big ? 2 : 1),
                CornerRadius    = new CornerRadius(8),
                Padding         = new Thickness(12),
                Cursor          = Cursors.Hand,
                Tag             = p,
            };
            border.MouseLeftButtonUp += (s, e) => OpenDetail(p);

            var sp = new StackPanel();

            if (p.Discount > 0)
                sp.Children.Add(new Border
                {
                    Background  = new SolidColorBrush(Color.FromRgb(0xFF,0x6B,0x35)),
                    CornerRadius = new CornerRadius(4), Padding = new Thickness(6,2,6,2),
                    HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0,0,0,6),
                    Child = new TextBlock { Text = $"-{p.Discount}%", Foreground = Brushes.White, FontWeight = FontWeights.Bold, FontSize = 11 },
                });

            sp.Children.Add(new TextBlock
            {
                Text = p.Name, TextWrapping = TextWrapping.Wrap,
                Foreground = (Brush)FindResource("TextBrush"),
                FontSize = 13, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0,0,0,4),
            });
            sp.Children.Add(new TextBlock
            {
                Text = p.ManufacturerName,
                Foreground = (Brush)FindResource("SubtextBrush"), FontSize = 11, Margin = new Thickness(0,0,0,6),
            });
            sp.Children.Add(new TextBlock
            {
                Text = $"⭐ {p.Rating:F1}",
                Foreground = (Brush)FindResource("AccentBrush"), FontSize = 12, Margin = new Thickness(0,0,0,6),
            });

            if (p.Discount > 0)
                sp.Children.Add(new TextBlock
                {
                    Text = $"{p.Price:N2} ₽", TextDecorations = TextDecorations.Strikethrough,
                    Foreground = (Brush)FindResource("SubtextBrush"), FontSize = 11,
                });

            sp.Children.Add(new TextBlock
            {
                Text = $"{p.FinalPrice:N2} ₽",
                Foreground = (Brush)FindResource("AccentBrush"),
                FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0,0,0,8),
            });

            var btn = new Button { Content = "🛒 В корзину", Style = (Style)FindResource("PrimaryButton"), FontSize = 11, Tag = p };
            btn.Click += BtnAddToCart_Click;
            sp.Children.Add(btn);

            border.Child = sp;
            return border;
        }

        private void OpenDetail(Products p)
        {
            new ProductDetailWindow(p) { Owner = Window.GetWindow(this) }.ShowDialog();
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            var p = (Products)((Button)sender).Tag;
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show("Для добавления в корзину войдите в аккаунт.",
                    "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            using (var db = DbHelper.CreateContext())
            {
                var item = db.CartItems.FirstOrDefault(c => c.UserId == Session.CurrentUser.Id && c.ProductId == p.Id);
                if (item != null) item.Quantity++;
                else db.CartItems.Add(new CartItems { UserId = Session.CurrentUser.Id, ProductId = p.Id, Quantity = 1 });
                db.SaveChanges();
            }
            MessageBox.Show($"«{p.Name}» добавлен в корзину!", "Корзина", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Filter_Changed(object sender, RoutedEventArgs e) => LoadProducts();
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Text = "";
            CmbType.SelectedIndex = 0;
            CmbManufacturer.SelectedIndex = 0;
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.Navigate(new StartPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.IsLoggedIn)
            { MessageBox.Show("Войдите в аккаунт.", "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            MainWindow.Instance.Navigate(new CartPage());
        }
    }
}
