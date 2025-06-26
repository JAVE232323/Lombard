using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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

namespace LaJusie.Orders
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {

        LaJusieEntities db = new LaJusieEntities();
        
        public OrdersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var orders = db.Order
                    .AsNoTracking()
                    .OrderByDescending(o => o.Date)
                    .ToList();

                var result = new List<OrderDisplayItem>();

                foreach (var order in orders)
                {
                    var client = db.Clients
                        .AsNoTracking()
                        .FirstOrDefault(c => c.Client_ID == order.Client_ID);

                    var status = db.Status
                        .AsNoTracking()
                        .FirstOrDefault(s => s.Status_ID == order.Status_ID);

                    var listJalousie = db.ListJalousie
                        .AsNoTracking()
                        .Where(lj => lj.Order_ID == order.Order_ID)
                        .ToList();

                    var jalousies = new List<JalousieDisplayItem>();
                    decimal totalOrderPrice = 0; 

                    foreach (var lj in listJalousie)
                    {
                        var jalousie = db.Jalousies
                            .AsNoTracking()
                            .Include("Type")
                            .Include("Materials")
                            .FirstOrDefault(j => j.Jalousie_ID == lj.Jalousie_ID);

                        if (jalousie != null)
                        {
                            
                            decimal area = (order.Width / 100m) * (order.Height / 100m);

                           
                            decimal totalPrice = (jalousie.Price * area) + 2000m;
                            totalOrderPrice += totalPrice;

                            jalousies.Add(new JalousieDisplayItem
                            {
                                Jalousie = jalousie,
                                Type = jalousie.Type,
                                Material = jalousie.Materials,
                                Price = jalousie.Price, 
                                Area = area,            
                                TotalPrice = totalPrice 
                            });
                        }
                    }

                    result.Add(new OrderDisplayItem
                    {
                        Order = order,
                        Client = client,
                        Status = status,
                        Jalousies = jalousies,
                        TotalOrderPrice = totalOrderPrice 
                    });
                }

                itemsControlOrders.ItemsSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке залогов: {ex.Message}\n\n{ex.InnerException?.Message}");
            }
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            // Получаем данные заказа
            if (!(sender is Button button) || !(button.DataContext is OrderDisplayItem orderItem))
            {
                MessageBox.Show("Не удалось получить данные залога");
                return;
            }

            // Создаем и настраиваем UserControl
            var statusControl = new StatusSelection
            {
                // Запоминаем Order_ID для обновления
                Tag = orderItem.Order.Order_ID
            };

            // Добавляем UserControl в главный контейнер
            OrderGrid.Children.Add(statusControl);

            // Загружаем список статусов
            LoadStatuses(statusControl);

            // Подписываемся на событие выбора статуса
            statusControl.StatusSelected += (selectedStatus) =>
            {
                UpdateOrderStatus(orderItem, selectedStatus);
                OrderGrid.Children.Remove(statusControl);
            };
        }

        private void LoadStatuses(StatusSelection statusControl)
        {
            try
            {
                
                
                    statusControl.ComboBoxStatuses.ItemsSource = db.Status
                        .AsNoTracking()
                        .OrderBy(s => s.Status_ID)
                        .ToList();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статусов: {ex.Message}");
            }
        }

        private void UpdateOrderStatus(OrderDisplayItem orderItem, Status newStatus)
        {
            try
            {
                
                
                    var order = db.Order.Find(orderItem.Order.Order_ID);
                    if (order != null)
                    {
                        order.Status_ID = newStatus.Status_ID;
                        db.SaveChanges();

                        // Обновляем данные в интерфейсе
                        orderItem.Status = newStatus;
                        orderItem.Order.Status_ID = newStatus.Status_ID;

                        MessageBox.Show($"Статус изменен на: {newStatus.Name}");
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления статуса: {ex.Message}");
            }
        }

        public void Print_Click(object sender, RoutedEventArgs e)
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

            // Информация о заказе
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
