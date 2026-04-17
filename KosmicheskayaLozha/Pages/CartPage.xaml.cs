using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KosmicheskayaLozha.Windows;

namespace KosmicheskayaLozha.Pages
{
    public partial class CartPage : Page
    {
        private List<CartItems> _items;

        public CartPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadCart();
        }

        private void LoadCart()
        {
            using (var db = DbHelper.CreateContext())
            {
                _items = db.CartItems
                           .Include("Products")
                           .Where(c => c.UserId == Session.CurrentUser.Id)
                           .ToList();
            }

            WpCart.Children.Clear();

            if (_items.Count == 0)
            {
                WpCart.Children.Add(new TextBlock
                {
                    Text       = "Корзина пуста",
                    Foreground = (Brush)FindResource("SubtextBrush"),
                    FontSize   = 18, Margin = new Thickness(0, 30, 0, 0),
                });
                TxtTotal.Text = "0,00 ₽";
                return;
            }

            foreach (var item in _items)
                WpCart.Children.Add(CreateCard(item));

            TxtTotal.Text = $"{_items.Sum(i => i.Products.FinalPrice * i.Quantity):N2} ₽";
        }

        private Border CreateCard(CartItems item)
        {
            var p = item.Products;
            decimal unitPrice = p.FinalPrice;

            var border = new Border
            {
                Width = 220, Margin = new Thickness(0, 0, 12, 12),
                Background      = new SolidColorBrush(Color.FromRgb(0x2D, 0x00, 0x50)),
                BorderBrush     = new SolidColorBrush(Color.FromRgb(0x6A, 0x0D, 0xAD)),
                BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8),
                Padding         = new Thickness(12),
            };

            var sp = new StackPanel();

            sp.Children.Add(new TextBlock
            {
                Text = p.Name, TextWrapping = TextWrapping.Wrap,
                Foreground = (Brush)FindResource("TextBrush"),
                FontSize = 13, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 0, 0, 4),
            });
            sp.Children.Add(new TextBlock
            {
                Text = $"{unitPrice:N2} ₽ / шт.",
                Foreground = (Brush)FindResource("SubtextBrush"), FontSize = 11,
            });

            // Количество
            var qRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 8, 0, 8) };
            var btnM = new Button { Content = "−", Width = 30, Height = 30, Style = (Style)FindResource("BackButton"), FontSize = 16, Tag = item };
            var lblQ = new TextBlock
            {
                Text = item.Quantity.ToString(),
                Foreground = (Brush)FindResource("TextBrush"),
                FontSize = 15, FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 10, 0),
            };
            var btnP = new Button { Content = "+", Width = 30, Height = 30, Style = (Style)FindResource("PrimaryButton"), FontSize = 16, Tag = item };

            btnM.Click += (s, e) => ChangeQty(item, -1);
            btnP.Click += (s, e) => ChangeQty(item, +1);

            qRow.Children.Add(btnM);
            qRow.Children.Add(lblQ);
            qRow.Children.Add(btnP);
            sp.Children.Add(qRow);

            sp.Children.Add(new TextBlock
            {
                Text = $"Сумма: {unitPrice * item.Quantity:N2} ₽",
                Foreground = (Brush)FindResource("AccentBrush"),
                FontSize = 14, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 8),
            });

            var btnDel = new Button { Content = "🗑 Удалить", Style = (Style)FindResource("DangerButton"), FontSize = 11, Tag = item };
            btnDel.Click += (s, e) =>
            {
                using (var db = DbHelper.CreateContext())
                {
                    var c = db.CartItems.Find(item.Id);
                    if (c != null) { db.CartItems.Remove(c); db.SaveChanges(); }
                }
                LoadCart();
            };
            sp.Children.Add(btnDel);
            border.Child = sp;
            return border;
        }

        private void ChangeQty(CartItems item, int delta)
        {
            int newQty = item.Quantity + delta;
            if (newQty < 1)
            {
                var r = MessageBox.Show("Удалить товар из корзины?", "Удаление",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (r != MessageBoxResult.Yes) return;
                using (var db = DbHelper.CreateContext())
                {
                    var c = db.CartItems.Find(item.Id);
                    if (c != null) { db.CartItems.Remove(c); db.SaveChanges(); }
                }
                LoadCart();
                return;
            }
            using (var db = DbHelper.CreateContext())
            {
                var c = db.CartItems.Find(item.Id);
                if (c != null) { c.Quantity = newQty; db.SaveChanges(); }
            }
            LoadCart();
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_items == null || _items.Count == 0)
            { MessageBox.Show("Корзина пуста.", "Корзина", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            var w = new OrderWindow(_items) { Owner = Window.GetWindow(this) };
            if (w.ShowDialog() == true) LoadCart();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.Navigate(new ProductsPage());
    }
}
