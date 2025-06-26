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
using System.Windows.Threading;
using LaJusie.Model;

namespace LaJusie.Admin
{
    /// <summary>
    /// Логика взаимодействия для ClientsControl.xaml
    /// </summary>
    public partial class ClientsControl : UserControl
    {
        LaJusieEntities _db = new LaJusieEntities();

        public ClientsControl()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                ClientsGrid.ItemsSource = _db.Clients
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Dispatcher.InvokeAsync(() =>
                {
                    string searchText = SearchTextBox.Text.Trim().ToLower();

                    var query = _db.Clients.AsQueryable();

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query = query.Where(c =>
                            c.LastName.ToLower().Contains(searchText) ||
                            c.FirstName.ToLower().Contains(searchText) ||
                            c.MiddleName.ToLower().Contains(searchText) ||
                            c.Phone.ToLower().Contains(searchText));
                    }

                    ClientsGrid.ItemsSource = query
                        .OrderBy(c => c.LastName)
                        .ThenBy(c => c.FirstName)
                        .ToList();

                }, DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}");
            }
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem == null) return;

            var selectedClient = (LaJusie.Model.Clients)ClientsGrid.SelectedItem;
            var editWindow = new AddEditClientWindow(selectedClient.Client_ID)
            {
                Owner = Window.GetWindow(this)
            };

            if (editWindow.ShowDialog() == true)
            {
                LoadClients();
            }
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem == null) return;

            var selectedClient = (LaJusie.Model.Clients)ClientsGrid.SelectedItem;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить клиента {selectedClient.LastName} {selectedClient.FirstName}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Проверяем, есть ли заказы у клиента
                    bool hasOrders = _db.Order.Any(o => o.Client_ID == selectedClient.Client_ID);

                    if (hasOrders)
                    {
                        MessageBox.Show("Нельзя удалить клиента, у которого есть заказы!");
                        return;
                    }

                    _db.Clients.Remove(selectedClient);
                    _db.SaveChanges();
                    LoadClients();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}");
                }
            }
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditClientWindow()
            {
                Owner = Window.GetWindow(this)
            };

            if (addWindow.ShowDialog() == true)
            {
                LoadClients();
            }
        }

        private void RefreshClients_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void ShowClientOrders_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem == null) return;

            var selectedClient = (LaJusie.Model.Clients)ClientsGrid.SelectedItem;
            var ordersWindow = new ClientOrdersWindow(selectedClient.Client_ID)
            {
                Owner = Window.GetWindow(this)
            };
            ordersWindow.ShowDialog();
        }
    }
}
