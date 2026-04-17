using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace KosmicheskayaLozha.Windows
{
    public partial class EditProductWindow : Window
    {
        private readonly Products _product;
        private readonly bool     _isNew;

        public EditProductWindow(Products product)
        {
            InitializeComponent();
            _isNew   = product == null;
            _product = product ?? new Products();
            Loaded  += EditProductWindow_Loaded;
        }

        private void EditProductWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtTitle.Text = _isNew ? "➕ Новый товар" : "✏ Редактирование товара";

            using (var db = new KosmicheskayaLozhaEntities())
            {
                var mfrs  = db.Manufacturers.OrderBy(m => m.Name).ToList();
                var types = db.ProductTypes.OrderBy(t => t.Name).ToList();

                CmbManufacturer.ItemsSource = mfrs;
                CmbType.ItemsSource         = types;

                if (!_isNew)
                {
                    TxtName.Text     = _product.Name;
                    TxtPrice.Text    = _product.Price.ToString("F2");
                    TxtDiscount.Text = _product.Discount.ToString();
                    TxtRating.Text   = _product.Rating.ToString("F1");
                    TxtDesc.Text     = _product.Description;

                    CmbManufacturer.SelectedItem = mfrs.FirstOrDefault(m => m.Id == _product.ManufacturerId);
                    CmbType.SelectedItem         = types.FirstOrDefault(t => t.Id == _product.ProductTypeId);
                }
                else
                {
                    TxtDiscount.Text = "0";
                    TxtRating.Text   = "0";
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            { ShowError("Введите название товара."); return; }

            if (!decimal.TryParse(TxtPrice.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            { ShowError("Введите корректную цену."); return; }

            if (!int.TryParse(TxtDiscount.Text, out int disc) || disc < 0 || disc > 99)
            { ShowError("Скидка — от 0 до 99%."); return; }

            if (!decimal.TryParse(TxtRating.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rating) || rating < 0 || rating > 5)
            { ShowError("Рейтинг — от 0.0 до 5.0."); return; }

            if (CmbManufacturer.SelectedItem == null)
            { ShowError("Выберите производителя."); return; }

            if (CmbType.SelectedItem == null)
            { ShowError("Выберите тип товара."); return; }

            var mfr  = (Manufacturers)CmbManufacturer.SelectedItem;
            var type = (ProductTypes)CmbType.SelectedItem;

            using (var db = new KosmicheskayaLozhaEntities())
            {
                Products prod;
                if (_isNew)
                {
                    prod = new Products();
                    db.Products.Add(prod);
                }
                else
                {
                    prod = db.Products.Find(_product.Id);
                }

                prod.Name             = TxtName.Text.Trim();
                prod.Price            = price;
                prod.Discount         = disc;
                prod.Rating           = rating;
                prod.Description      = TxtDesc.Text.Trim();
                prod.ManufacturerId   = mfr.Id;
                prod.ProductTypeId    = type.Id;
                prod.IsFrozen         = _isNew ? false : prod.IsFrozen;

                db.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void ShowError(string msg)
        {
            TxtError.Text       = msg;
            TxtError.Visibility = Visibility.Visible;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        { DialogResult = false; Close(); }
    }
}
