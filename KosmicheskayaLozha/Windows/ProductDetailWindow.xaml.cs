using System.Linq;
using System.Windows;

namespace KosmicheskayaLozha.Windows
{
    public partial class ProductDetailWindow : Window
    {
        private readonly Products _product;

        public ProductDetailWindow(Products product)
        {
            InitializeComponent();
            _product = product;
            Loaded += (s, e) => Fill();
        }

        private void Fill()
        {
            TxtName.Text = _product.Name;
            TxtType.Text = _product.ProductTypeName;
            TxtMfr.Text  = _product.ManufacturerName;
            TxtDesc.Text = string.IsNullOrEmpty(_product.Description) ? "Описание отсутствует." : _product.Description;
            TxtRating.Text = $"⭐ {_product.Rating:F1}";

            if (_product.Discount > 0)
            {
                DiscountBadge.Visibility = Visibility.Visible;
                TxtDiscount.Text         = $"-{_product.Discount}%";
                TxtOldPrice.Visibility   = Visibility.Visible;
                TxtOldPrice.Text         = $"{_product.Price:N2} ₽";
                TxtPrice.Text            = $"{_product.FinalPrice:N2} ₽";
            }
            else
            {
                TxtPrice.Text = $"{_product.Price:N2} ₽";
            }

            if (_product.IsFrozen)
            {
                TxtFrozen.Visibility = Visibility.Visible;
                BtnAdd.IsEnabled     = false;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.IsLoggedIn)
            {
                MessageBox.Show("Для добавления в корзину войдите в аккаунт.",
                    "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            using (var db = DbHelper.CreateContext())
            {
                var item = db.CartItems.FirstOrDefault(
                    c => c.UserId == Session.CurrentUser.Id && c.ProductId == _product.Id);
                if (item != null) item.Quantity++;
                else db.CartItems.Add(new CartItems
                    { UserId = Session.CurrentUser.Id, ProductId = _product.Id, Quantity = 1 });
                db.SaveChanges();
            }
            MessageBox.Show($"«{_product.Name}» добавлен в корзину!", "Корзина",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
}
