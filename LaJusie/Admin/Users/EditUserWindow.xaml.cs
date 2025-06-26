using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LaJusie.Model;

namespace LaJusie.Admin
{
    /// <summary>
    /// Логика взаимодействия для EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {

        private readonly LombardEntities _db = new LombardEntities();
        private readonly Users _user;

        public EditUserWindow(int userId)
        {
            InitializeComponent();

            _user = _db.Users.FirstOrDefault(u => u.User_ID == userId);
            if (_user == null)
            {
                MessageBox.Show("Пользователь не найден!");
                Close();
                return;
            }

            // Заполняем поля данными
            LoginTextBox.Text = _user.Login;
            PasswordTextBox.Text = _user.Password;

            // Загружаем роли в ComboBox
            RoleComboBox.ItemsSource = _db.Role.ToList();
            RoleComboBox.SelectedValue = _user.Role_ID;
            RoleComboBox.SelectedValuePath = "Role_ID";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordTextBox.Text) ||
                RoleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            _user.Login = LoginTextBox.Text;
            _user.Password = PasswordTextBox.Text;
            _user.Role_ID = ((Role)RoleComboBox.SelectedItem).Role_ID;

            try
            {
                _db.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
