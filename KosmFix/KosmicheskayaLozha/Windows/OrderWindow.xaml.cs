using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KosmicheskayaLozha.Windows
{
    public partial class OrderWindow : Window
    {
        private readonly List<CartItems> _items;

        public OrderWindow(List<CartItems> items)
        {
            InitializeComponent();
            _items = items;
            Loaded += (s, e) =>
            {
                decimal total = _items.Sum(i => i.Products.FinalPrice * i.Quantity);
                TxtTotal.Text = $"{total:N2} ₽";

                DpDelivery.DisplayDateStart = DateTime.Today;
                DpDelivery.DisplayDateEnd   = DateTime.Today.AddDays(7);
                DpDelivery.SelectedDate     = DateTime.Today.AddDays(1);
            };
        }

        private void DpDelivery_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!DpDelivery.SelectedDate.HasValue) return;
            bool valid = DpDelivery.SelectedDate.Value >= DateTime.Today
                      && DpDelivery.SelectedDate.Value <= DateTime.Today.AddDays(7);
            TxtDateError.Visibility = valid ? Visibility.Collapsed : Visibility.Visible;
            TxtDateError.Text       = "Выберите дату в пределах 7 дней от сегодня.";
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!DpDelivery.SelectedDate.HasValue)
            { MessageBox.Show("Выберите дату доставки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            var date = DpDelivery.SelectedDate.Value;
            if (date < DateTime.Today || date > DateTime.Today.AddDays(7))
            { MessageBox.Show("Выберите дату в пределах 7 дней.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            var payment = (CmbPayment.SelectedItem as ComboBoxItem)?.Content?.ToString();

            using (var db = new KosmicheskayaLozhaEntities())
            {
                var order = new Orders
                {
                    ClientId      = Session.CurrentUser.Id,
                    OrderDate     = DateTime.Now,
                    DeliveryDate  = date,
                    PaymentMethod = payment,
                    IsClosed      = false,
                };
                db.Orders.Add(order);
                db.SaveChanges();

                foreach (var item in _items)
                {
                    db.OrderItems.Add(new OrderItems
                    {
                        OrderId   = order.Id,
                        ProductId = item.ProductId,
                        Quantity  = item.Quantity,
                        Price     = item.Products.FinalPrice,
                    });
                }

                // Очистить корзину
                var cartItems = db.CartItems.Where(c => c.UserId == Session.CurrentUser.Id).ToList();
                db.CartItems.RemoveRange(cartItems);
                db.SaveChanges();
            }

            MessageBox.Show("✅ Заказ успешно оформлен!", "Заказ создан",
                MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        { DialogResult = false; Close(); }
    }
}
