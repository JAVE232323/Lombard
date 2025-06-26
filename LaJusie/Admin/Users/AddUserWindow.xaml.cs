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
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        private readonly LaJusieEntities _db = new LaJusieEntities();

        public AddUserWindow()
        {
            InitializeComponent();

            RoleComboBox.ItemsSource = _db.Role.ToList();
            RoleComboBox.SelectedValuePath = "Role_ID";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordTextBox.Text) ||
                RoleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            var newUser = new Users
            {
                Login = LoginTextBox.Text,
                Password = PasswordTextBox.Text,
                Role_ID = ((Role)RoleComboBox.SelectedItem).Role_ID
            };

            try
            {
                _db.Users.Add(newUser);
                _db.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления пользователя: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
