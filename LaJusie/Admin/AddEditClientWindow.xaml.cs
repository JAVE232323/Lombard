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
    /// Логика взаимодействия для AddEditClientWindow.xaml
    /// </summary>
    public partial class AddEditClientWindow : Window
    {
        private readonly LombardEntities _db = new LombardEntities();
        private readonly LaJusie.Model.Clients _client;
        private readonly bool _isEditMode;

        public string WindowTitle => _isEditMode ? "Редактирование клиента" : "Добавление клиента";

        public AddEditClientWindow()
        {
            InitializeComponent();
            _client = new Clients();
            _isEditMode = false;
            DataContext = this;
        }

        public AddEditClientWindow(int clientId) : this()
        {
            _client = _db.Clients.Find(clientId);
            if (_client == null)
            {
                MessageBox.Show("Клиент не найден!");
                Close();
                return;
            }

            _isEditMode = true;
            LoadClientData();
        }

        private void LoadClientData()
        {
            LastNameTextBox.Text = _client.LastName;
            FirstNameTextBox.Text = _client.FirstName;
            MiddleNameTextBox.Text = _client.MiddleName;
            PhoneTextBox.Text = _client.Phone;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля (Фамилия, Имя, Телефон)!");
                return;
            }

            _client.LastName = LastNameTextBox.Text.Trim();
            _client.FirstName = FirstNameTextBox.Text.Trim();
            _client.MiddleName = MiddleNameTextBox.Text?.Trim();
            _client.Phone = PhoneTextBox.Text.Trim();

            try
            {
                if (!_isEditMode)
                {
                    _db.Clients.Add(_client);
                }

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
