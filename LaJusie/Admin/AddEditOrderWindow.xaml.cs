using LaJusie.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LaJusie.Admin
{
    /// <summary>
    /// Логика взаимодействия для AddEditOrderWindow.xaml
    /// </summary>
    public partial class AddEditOrderWindow : Window
    {
        private readonly LombardEntities _db = new LombardEntities();
        private readonly Order _order;
        private readonly bool _isEditMode;

        public string WindowTitle => _isEditMode ? $"Редактирование залога №{_order.Order_ID}" : "Новый залог";

        public AddEditOrderWindow()
        {
            InitializeComponent();
            _order = new Order { Date = DateTime.Now };
            _isEditMode = false;
            DataContext = this;
            LoadComboBoxData();
        }

        public AddEditOrderWindow(int orderId) : this()
        {
            _order = _db.Order
                .Include(o => o.Clients)
                .Include(o => o.Status)
                .FirstOrDefault(o => o.Order_ID == orderId);

            if (_order == null)
            {
                MessageBox.Show("Залог не найден!");
                Close();
                return;
            }

            _isEditMode = true;
            LoadOrderData();
        }

        private void LoadComboBoxData()
        {
            ClientComboBox.ItemsSource = _db.Clients
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToList()
                .Select(c => new { c.Client_ID, FullName = $"{c.LastName} {c.FirstName} {c.MiddleName}" });

            StatusComboBox.ItemsSource = _db.Status.ToList();

            if (!_isEditMode)
            {
                StatusComboBox.SelectedIndex = 0; // Первый статус по умолчанию (например, "Новый")
                OrderDatePicker.SelectedDate = DateTime.Now;
            }
        }

        private void LoadOrderData()
        {
            ClientComboBox.SelectedValue = _order.Client_ID;
            AddressTextBox.Text = _order.Address;
            OrderDatePicker.SelectedDate = _order.Date;
            WidthTextBox.Text = _order.Width.ToString();
            HeightTextBox.Text = _order.Height.ToString();
            StatusComboBox.SelectedValue = _order.Status_ID;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(AddressTextBox.Text) ||
                OrderDatePicker.SelectedDate == null ||
                StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return;
            }

            if (!int.TryParse(WidthTextBox.Text, out int width) ||
                !int.TryParse(HeightTextBox.Text, out int height) ||
                width <= 0 || height <= 0)
            {
                MessageBox.Show("Укажите корректные размеры (целые числа больше 0)!");
                return;
            }

            dynamic selectedClient = ClientComboBox.SelectedItem;
            _order.Client_ID = selectedClient.Client_ID;
            _order.Address = AddressTextBox.Text.Trim();
            _order.Date = OrderDatePicker.SelectedDate.Value;
            _order.Width = width;
            _order.Height = height;
            _order.Status_ID = ((Status)StatusComboBox.SelectedItem).Status_ID;
            _order.User_ID = GetCurrentUserId(); // Метод для получения ID текущего пользователя

            try
            {
                if (!_isEditMode)
                {
                    _db.Order.Add(_order);
                }

                _db.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения залога: {ex.Message}");
            }
        }

        private int GetCurrentUserId()
        {
            // Здесь должна быть логика получения ID текущего пользователя
            // Например, из настроек приложения или системы аутентификации
            return 1; // Временная заглушка
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
