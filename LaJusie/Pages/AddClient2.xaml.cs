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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaJusie.Model;

namespace LaJusie.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddClient2.xaml
    /// </summary>
    public partial class AddClient2 : UserControl
    {
        LombardEntities db = new LombardEntities();
        private int User_ID;
        private int JalID;

        public AddClient2(int user_ID, int jalID)
        {
            InitializeComponent();
            User_ID = user_ID;
            JalID = jalID;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AddClient addClient = new AddClient(User_ID, JalID);
            this.Visibility = Visibility.Collapsed;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Введите фамилию!");
                txtLastName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите имя!");
                txtFirstName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Введите телефон!");
                txtPhone.Focus();
                return;
            }

            // Создание нового клиента
            var newClient = new Clients
            {
                LastName = txtLastName.Text.Trim(),
                FirstName = txtFirstName.Text.Trim(),
                MiddleName = txtMiddleName.Text.Trim(),
                Phone = txtPhone.Text.Trim()
            };
            try
            {               
               
                db.Clients.Add(newClient);
                db.SaveChanges();              

                MessageBox.Show("Клиент успешно добавлен!");

                AddClient addClient = new AddClient(User_ID, JalID);
                this.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }
    }
}
