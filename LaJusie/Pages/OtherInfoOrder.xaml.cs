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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaJusie.Model;

namespace LaJusie.Pages
{
    /// <summary>
    /// Логика взаимодействия для OtherInfoOrder.xaml
    /// </summary>
    public partial class OtherInfoOrder : UserControl
    {
        private LaJusieEntities db = new LaJusieEntities();
        private int User_ID;
        private int JalID;
        private int Width1;
        private int Height1;
        private int Client_ID;
        private Grid MainGrid;

        public OtherInfoOrder(int user_ID, int jalID, int width, int height, int clientID, Grid grid)
        {
            InitializeComponent();
            User_ID = user_ID;
            JalID = jalID;
            Width1 = width;
            Height1 = height;
            Client_ID = clientID;
            MainGrid = grid;
        }

        private void MakeOrder_Click(object sender, RoutedEventArgs e)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Создаем основной заказ
                    var newOrder = new Model.Order
                    {
                        User_ID = User_ID,
                        Client_ID = Client_ID,
                        Address = AddressRtxb.Text,
                        Status_ID = 1, // Статус "Новый"
                        Width = Width1,
                        Height = Height1,
                        Date = DateTime.Today
                    };

                    db.Order.Add(newOrder);
                    db.SaveChanges(); // Сохраняем, чтобы получить Order_ID

                    // 2. Создаем связь с жалюзи
                    var listJalousie = new ListJalousie
                    {
                        Order_ID = newOrder.Order_ID,
                        Jalousie_ID = JalID
                    };

                    db.ListJalousie.Add(listJalousie);
                    db.SaveChanges();

                    // Фиксируем транзакцию
                    transaction.Commit();

                    MessageBox.Show($"Заказ #{newOrder.Order_ID} успешно создан!\n" +
                                  $"Дата: {newOrder.Date:yyyy-MM-dd}\n" +
                                  $"Адрес: {newOrder.Address}",
                                  "Успех",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);

                    this.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    // Откатываем транзакцию при ошибке
                    transaction.Rollback();
                    HandleOrderException(ex);
                }
            }
        }

        private void HandleOrderException(Exception ex)
        {
            string errorMessage = "Ошибка при создании заказа: " + ex.Message;

            // Извлекаем внутренние исключения
            Exception inner = ex.InnerException;
            while (inner != null)
            {
                errorMessage += $"\n\nВнутренняя ошибка: {inner.Message}";
                inner = inner.InnerException;
            }

            MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
            
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AddClient makeOrder = new AddClient(User_ID, JalID, Width1, Height1, MainGrid);
            Grid.SetRowSpan(makeOrder, 2);
            MainGrid.Children.Add(makeOrder);
            this.Visibility = Visibility.Collapsed;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
