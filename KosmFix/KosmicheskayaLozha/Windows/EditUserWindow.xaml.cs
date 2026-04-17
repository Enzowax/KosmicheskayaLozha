using System.Linq;
using System.Windows;

namespace KosmicheskayaLozha.Windows
{
    public partial class EditUserWindow : Window
    {
        private readonly Users _user;
        private readonly bool  _isNew;

        public EditUserWindow(Users user)
        {
            InitializeComponent();
            _isNew = user == null;
            _user  = user ?? new Users();
            Loaded += EditUserWindow_Loaded;
        }

        private void EditUserWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtTitle.Text = _isNew ? "➕ Новый пользователь" : "✏ Редактирование пользователя";

            using (var db = new KosmicheskayaLozhaEntities())
            {
                var roles = db.Roles.OrderBy(r => r.Id).ToList();
                CmbRole.ItemsSource = roles;

                if (!_isNew)
                {
                    TxtLogin.Text      = _user.Login;
                    TxtLastName.Text   = _user.LastName;
                    TxtFirstName.Text  = _user.FirstName;
                    TxtMiddleName.Text = _user.MiddleName;
                    TxtPhone.Text      = _user.Phone;
                    CmbRole.SelectedItem = roles.FirstOrDefault(r => r.Id == _user.RoleId);
                }
                else
                {
                    CmbRole.SelectedIndex = 0;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLogin.Text))
            { ShowError("Введите логин."); return; }
            if (_isNew && string.IsNullOrEmpty(TxtPassword.Password))
            { ShowError("Введите пароль."); return; }
            if (string.IsNullOrWhiteSpace(TxtLastName.Text))
            { ShowError("Введите фамилию."); return; }
            if (string.IsNullOrWhiteSpace(TxtFirstName.Text))
            { ShowError("Введите имя."); return; }
            if (CmbRole.SelectedItem == null)
            { ShowError("Выберите роль."); return; }

            var role = (Roles)CmbRole.SelectedItem;

            using (var db = new KosmicheskayaLozhaEntities())
            {
                Users user;
                if (_isNew)
                {
                    user = new Users();
                    db.Users.Add(user);
                }
                else
                {
                    user = db.Users.Find(_user.Id);
                }

                user.Login      = TxtLogin.Text.Trim();
                user.LastName   = TxtLastName.Text.Trim();
                user.FirstName  = TxtFirstName.Text.Trim();
                user.MiddleName = TxtMiddleName.Text.Trim();
                user.Phone      = TxtPhone.Text.Trim();
                user.RoleId     = role.Id;

                if (!string.IsNullOrEmpty(TxtPassword.Password))
                    user.Password = TxtPassword.Password;
                else if (_isNew)
                    user.Password = "password";

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
