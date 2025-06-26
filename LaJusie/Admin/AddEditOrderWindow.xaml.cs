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
                .Select(c => new { c.ClientId, FullName = $"{c.LastName} {c.FirstName} {c.MiddleName}" });
            ItemComboBox.ItemsSource = _db.Items.ToList();
        }

        private void LoadOrderData()
        {
            ClientComboBox.SelectedValue = _order.ClientId;
            ItemComboBox.SelectedValue = _order.ItemId;
            OrderDatePicker.SelectedDate = _order.Date;
            PriceTextBox.Text = _order.Price?.ToString() ?? "";
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.SelectedItem == null || ItemComboBox.SelectedItem == null || OrderDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return;
            }
            dynamic selectedClient = ClientComboBox.SelectedItem;
            dynamic selectedItem = ItemComboBox.SelectedItem;
            _order.ClientId = selectedClient.ClientId;
            _order.ItemId = selectedItem.ItemId;
            _order.Date = OrderDatePicker.SelectedDate.Value;
            if (int.TryParse(PriceTextBox.Text, out int price))
                _order.Price = price;
            else
                _order.Price = null;
            _order.UserId = GetCurrentUserId();
            try
            {
                if (!_isEditMode)
                {
                    _db.Orders.Add(_order);
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
