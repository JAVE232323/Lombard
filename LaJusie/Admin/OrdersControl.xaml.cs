using LaJusie.Model;
using LaJusie.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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

namespace LaJusie.Admin
{
    /// <summary>
    /// Логика взаимодействия для OrdersControl.xaml
    /// </summary>
    public partial class OrdersControl : UserControl
    {
        private readonly LaJusieEntities _db = new LaJusieEntities();

        public OrdersControl()
        {
            InitializeComponent();
            LoadStatuses();
            LoadOrders();
        }

        private void LoadStatuses()
        {
            StatusFilterComboBox.ItemsSource = _db.Status.ToList();
            StatusFilterComboBox.SelectedIndex = 0;
        }

        private void LoadOrders()
        {
            try
            {
                OrdersGrid.ItemsSource = _db.Order
                    .Include(o => o.Clients) 
                        .Include(o => o.Status)  
                        .OrderByDescending(o => o.Date)
                        .ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки залогов: {ex.Message}");
            }
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            DateFromPicker.SelectedDate = null;
            DateToPicker.SelectedDate = null;
            StatusFilterComboBox.SelectedIndex = 0;
            LoadOrders();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                LoadOrders();
                return;
            }

            var filtered = _db.Order.Include(o => o.Clients).Include(o => o.Status).Where(o => o.Address.ToLower().Contains(searchText) ||
                           o.Order_ID.ToString().Contains(searchText) ||
                           o.Clients.LastName.ToLower().Contains(searchText) ||
                           o.Clients.FirstName.ToLower().Contains(searchText))
                .OrderByDescending(o => o.Date).ToList();

            OrdersGrid.ItemsSource = filtered;
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditOrderWindow()
            {
                Owner = Window.GetWindow(this)
            };

            if (addWindow.ShowDialog() == true)
            {
                LoadOrders();
            }
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null) return;

            var selectedOrder = (Order)OrdersGrid.SelectedItem;
            var editWindow = new AddEditOrderWindow(selectedOrder.Order_ID)
            {
                Owner = Window.GetWindow(this)
            };

            if (editWindow.ShowDialog() == true)
            {
                LoadOrders();
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null) return;

            var selectedOrder = (Order)OrdersGrid.SelectedItem;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить залог №{selectedOrder.Order_ID}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Order.Remove(selectedOrder);
                    _db.SaveChanges();
                    LoadOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении залога: {ex.Message}");
                }
            }
        }

        private void RefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void PrintOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.DataContext is OrderDisplayItem orderItem))
            {
                MessageBox.Show("Не удалось получить данные залога");
                return;
            }

            try
            {
                // Создаем имя файла
                string fileName = $"Залог_{orderItem.Order.Order_ID}_{DateTime.Now:yyyyMMddHHmmss}.txt";

                // Получаем путь для сохранения
                string filePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    fileName);

                // Формируем содержимое файла
                string content = GenerateOrderContent(orderItem);

                // Записываем в файл
                File.WriteAllText(filePath, content, Encoding.UTF8);

                MessageBox.Show($"Файл сохранен: {filePath}", "Печать",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании файла: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateOrderContent(OrderDisplayItem order)
        {
            var sb = new StringBuilder();

            // Шапка документа
            sb.AppendLine("==================================================================");
            sb.AppendLine($"ЗАЛОГ № {order.Order.Order_ID}".PadLeft(50));
            sb.AppendLine("==================================================================");
            sb.AppendLine();

            // Данные клиента
            sb.AppendLine("КЛИЕНТ:");
            sb.AppendLine($"ФИО: {order.Client.LastName} {order.Client.FirstName} {order.Client.MiddleName}");
            sb.AppendLine($"Телефон: {order.Client.Phone}");
            sb.AppendLine();

            // Информация о залоге
            sb.AppendLine("ДЕТАЛИ ЗАЛОГА:");
            sb.AppendLine($"Дата: {order.Order.Date:dd.MM.yyyy}");
            sb.AppendLine($"Адрес: {order.Order.Address}");
            sb.AppendLine($"Размеры: {order.Order.Width} мм x {order.Order.Height} мм");
            sb.AppendLine($"Статус: {order.Status.Name}");
            sb.AppendLine();

            // Список электроники
            sb.AppendLine("СПИСОК ЭЛЕКТРОНИКИ:");
            sb.AppendLine("--------------------------------------------------");
            foreach (var item in order.Jalousies)
            {
                sb.AppendLine($"Тип: {item.Type.Name}");
                sb.AppendLine($"Материал: {item.Material.Name}");
                sb.AppendLine($"Цена за м²: {item.Price} руб");
                sb.AppendLine($"Площадь: {item.Area:0.00} м²");
                sb.AppendLine($"Стоимость: {item.TotalPrice:0.00} руб");
                sb.AppendLine("--------------------------------------------------");
            }

            // Итоговая стоимость
            sb.AppendLine();
            sb.AppendLine($"ИТОГО: {order.TotalOrderPrice:0.00} руб".PadLeft(60));
            sb.AppendLine();

            // Подпись
            sb.AppendLine("==================================================================");
            sb.AppendLine("Дата формирования: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            sb.AppendLine("==================================================================");

            return sb.ToString();
        }
    }
}
