using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KosmicheskayaLozha.Windows;

namespace KosmicheskayaLozha.Pages
{
    public partial class ManagerPage : Page
    {
        public ManagerPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadAll();
        }

        private void LoadAll()
        {
            TxtTitle.Text = $"🗂 Кабинет менеджера — {Session.CurrentUser.FullName}";
            using (var db = new KosmicheskayaLozhaEntities())
            {
                DgAppointments.ItemsSource =
                    db.Appointments.Include("Users").Include("Users1").Include("ServiceTypes")
                      .OrderBy(a => a.AppointmentDateTime).ToList();

                DgOrders.ItemsSource =
                    db.Orders.Include("Users").Include("OrderItems")
                      .OrderByDescending(o => o.OrderDate).ToList();

                DgProducts.ItemsSource =
                    db.Products.Include("Manufacturers").Include("ProductTypes")
                      .OrderBy(p => p.Name).ToList();

                DgManufacturers.ItemsSource =
                    db.Manufacturers.OrderBy(m => m.Name).ToList();

                DgProductTypes.ItemsSource =
                    db.ProductTypes.OrderBy(t => t.Name).ToList();

                DgServiceTypes.ItemsSource =
                    db.ServiceTypes.OrderBy(s => s.Name).ToList();
            }
        }

        // ── ЗАПИСИ ──────────────────────────────────────────────

        private void BtnCreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CreateAppointmentDialog { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true) LoadAll();
        }

        private void BtnReschedule_Click(object sender, RoutedEventArgs e)
        {
            var apt = DgAppointments.SelectedItem as Appointments;
            if (apt == null) { Warn("Выберите запись."); return; }
            if (apt.IsCompleted || apt.IsCancelled) { Warn("Нельзя перенести завершённую или отменённую запись."); return; }

            var dlg = new RescheduleDateDialog { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true && dlg.SelectedDateTime.HasValue)
            {
                using (var db = new KosmicheskayaLozhaEntities())
                {
                    var a = db.Appointments.Find(apt.Id);
                    a.AppointmentDateTime = dlg.SelectedDateTime.Value;
                    db.SaveChanges();
                }
                LoadAll();
                MessageBox.Show("Запись перенесена.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCancelAppointment_Click(object sender, RoutedEventArgs e)
        {
            var apt = DgAppointments.SelectedItem as Appointments;
            if (apt == null) { Warn("Выберите запись."); return; }
            if (apt.IsCompleted || apt.IsCancelled) { Warn("Уже завершена или отменена."); return; }
            if (!Confirm($"Отменить запись от {apt.DateTimeStr}?")) return;

            using (var db = new KosmicheskayaLozhaEntities())
            {
                var a = db.Appointments.Find(apt.Id);
                a.IsCancelled = true;
                a.ClientId    = null;
                db.SaveChanges();
            }
            LoadAll();
        }

        // ── ЗАКАЗЫ ──────────────────────────────────────────────

        private void BtnCloseOrder_Click(object sender, RoutedEventArgs e)
        {
            var order = DgOrders.SelectedItem as Orders;
            if (order == null) { Warn("Выберите заказ."); return; }
            if (order.IsClosed) { Warn("Заказ уже закрыт."); return; }
            if (!Confirm($"Закрыть заказ №{order.Id}?")) return;

            using (var db = new KosmicheskayaLozhaEntities())
            {
                var o = db.Orders.Find(order.Id);
                o.IsClosed = true;
                db.SaveChanges();
            }
            LoadAll();
        }

        // ── ТОВАРЫ ──────────────────────────────────────────────

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var w = new EditProductWindow(null) { Owner = Window.GetWindow(this) };
            if (w.ShowDialog() == true) LoadAll();
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var p = DgProducts.SelectedItem as Products;
            if (p == null) { Warn("Выберите товар."); return; }
            var w = new EditProductWindow(p) { Owner = Window.GetWindow(this) };
            if (w.ShowDialog() == true) LoadAll();
        }

        private void BtnFreezeProduct_Click(object sender, RoutedEventArgs e)
        {
            var p = DgProducts.SelectedItem as Products;
            if (p == null) { Warn("Выберите товар."); return; }
            string action = p.IsFrozen ? "разморозить" : "заморозить";
            if (!Confirm($"Вы хотите {action} товар «{p.Name}»?")) return;

            using (var db = new KosmicheskayaLozhaEntities())
            {
                var prod = db.Products.Find(p.Id);
                prod.IsFrozen = !prod.IsFrozen;
                db.SaveChanges();
            }
            LoadAll();
        }

        // ── ПРОИЗВОДИТЕЛИ ────────────────────────────────────────

        private void BtnAddManufacturer_Click(object sender, RoutedEventArgs e)
        {
            var name = InputDialog("Название нового производителя:");
            if (name == null) return;
            using (var db = new KosmicheskayaLozhaEntities())
            { db.Manufacturers.Add(new Manufacturers { Name = name }); db.SaveChanges(); }
            LoadAll();
        }

        private void BtnEditManufacturer_Click(object sender, RoutedEventArgs e)
        {
            var m = DgManufacturers.SelectedItem as Manufacturers;
            if (m == null) { Warn("Выберите производителя."); return; }
            var name = InputDialog("Новое название:", m.Name);
            if (name == null) return;
            using (var db = new KosmicheskayaLozhaEntities())
            { var x = db.Manufacturers.Find(m.Id); x.Name = name; db.SaveChanges(); }
            LoadAll();
        }

        // ── ТИПЫ ТОВАРОВ ────────────────────────────────────────

        private void BtnAddProductType_Click(object sender, RoutedEventArgs e)
        {
            var name = InputDialog("Название нового типа товара:");
            if (name == null) return;
            using (var db = new KosmicheskayaLozhaEntities())
            { db.ProductTypes.Add(new ProductTypes { Name = name }); db.SaveChanges(); }
            LoadAll();
        }

        private void BtnEditProductType_Click(object sender, RoutedEventArgs e)
        {
            var pt = DgProductTypes.SelectedItem as ProductTypes;
            if (pt == null) { Warn("Выберите тип товара."); return; }
            var name = InputDialog("Новое название:", pt.Name);
            if (name == null) return;
            using (var db = new KosmicheskayaLozhaEntities())
            { var x = db.ProductTypes.Find(pt.Id); x.Name = name; db.SaveChanges(); }
            LoadAll();
        }

        // ── ТИПЫ УСЛУГ ──────────────────────────────────────────

        private void BtnAddServiceType_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new EditServiceTypeDialog { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true)
            {
                using (var db = new KosmicheskayaLozhaEntities())
                { db.ServiceTypes.Add(new ServiceTypes { Name = dlg.ServiceName, Price = dlg.ServicePrice }); db.SaveChanges(); }
                LoadAll();
            }
        }

        private void BtnEditServiceType_Click(object sender, RoutedEventArgs e)
        {
            var st = DgServiceTypes.SelectedItem as ServiceTypes;
            if (st == null) { Warn("Выберите тип услуги."); return; }
            var dlg = new EditServiceTypeDialog(st) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true)
            {
                using (var db = new KosmicheskayaLozhaEntities())
                {
                    var x = db.ServiceTypes.Find(st.Id);
                    x.Name  = dlg.ServiceName;
                    x.Price = dlg.ServicePrice;
                    db.SaveChanges();
                }
                LoadAll();
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        { Session.Logout(); MainWindow.Instance.Navigate(new StartPage()); }

        // ── HELPERS ─────────────────────────────────────────────

        private void Warn(string msg) =>
            MessageBox.Show(msg, "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);

        private bool Confirm(string msg) =>
            MessageBox.Show(msg, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

        private string InputDialog(string prompt, string def = "")
        {
            var d = new SimpleInputDialog(prompt, def) { Owner = Window.GetWindow(this) };
            return d.ShowDialog() == true ? d.InputText : null;
        }
    }
}
