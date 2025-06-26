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
using LaJusie.Admin;
using LaJusie.Model;
using LaJusie.Orders;
using LaJusie.Pages;

namespace LaJusie
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LombardEntities db = new LombardEntities();
        private readonly int _userId;
        private readonly int _roleId;

        public MainWindow(int userId, int roleId)
        {
            InitializeComponent();            
        }

        private void LoadClients()
        {
            var clients = new List<Client>();

            using (var connection = DataAccessLayer.GetConnection())
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Clients", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clients.Add(new Client
                        {
                            ClientId = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            MiddleName = reader.GetString(3),
                            Phone = reader.GetString(4)
                        });
                    }
                }
            }

            dgClients.ItemsSource = clients;
        }

        private void LoadItems()
        {
            var items = new List<Item>();

            using (var connection = DataAccessLayer.GetConnection())
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT i.*, c.FirstName, c.LastName, c.MiddleName, cat.Category " +
                    "FROM Items i " +
                    "JOIN Clients c ON i.ClientId = c.ClientId " +
                    "JOIN Category cat ON i.CategoryId = cat.CategoryId", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Item
                        {
                            ItemId = reader.GetInt32(0),
                            ClientId = reader.GetInt32(1),
                            Name = reader.GetString(2),
                            CategoryId = reader.GetInt32(3),
                            Description = reader.GetString(4),
                            MarketPrice = reader.GetInt32(5),
                            ClientPrice = reader.GetInt32(6),
                            Client = new Client
                            {
                                FirstName = reader.GetString(7),
                                LastName = reader.GetString(8),
                                MiddleName = reader.GetString(9)
                            },
                            Category = new Category
                            {
                                CategoryName = reader.GetString(10)
                            }
                        });
                    }
                }
            }

            dgItems.ItemsSource = items;
        }

        private void LoadOrders()
        {
            var orders = new List<Order>();

            using (var connection = DataAccessLayer.GetConnection())
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT o.*, u.login, c.FirstName, c.LastName, c.MiddleName, i.Name " +
                    "FROM Orders o " +
                    "JOIN Users u ON o.UserId = u.UserId " +
                    "JOIN Clients c ON o.ClientId = c.ClientId " +
                    "JOIN Items i ON o.ItemId = i.ItemId", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            OrderId = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            ClientId = reader.GetInt32(2),
                            ItemId = reader.GetInt32(3),
                            Date = reader.GetDateTime(4),
                            Price = reader.GetInt32(5),
                            User = new User { Login = reader.GetString(6) },
                            Client = new Client
                            {
                                FirstName = reader.GetString(7),
                                LastName = reader.GetString(8),
                                MiddleName = reader.GetString(9)
                            },
                            Item = new Item { Name = reader.GetString(10) }
                        });
                    }
                }
            }

            dgOrders.ItemsSource = orders;
        }
    }
}
