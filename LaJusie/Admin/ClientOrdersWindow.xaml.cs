using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для ClientOrdersWindow.xaml
    /// </summary>
    public partial class ClientOrdersWindow : Window
    {
        private readonly LombardEntities _db = new LombardEntities();
        private readonly int _clientId;

        public ClientOrdersWindow(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            LoadClientData();
            LoadClientOrders();
        }

        private void LoadClientData()
        {
            var client = _db.Clients.Find(_clientId);
            if (client != null)
            {
                ClientInfoText.Text = $"Залоги клиента: {client.LastName} {client.FirstName} {client.MiddleName} | Телефон: {client.Phone}";
            }
        }

        private void LoadClientOrders()
        {
            OrdersGrid.ItemsSource = _db.Orders
                .Where(o => o.ClientId == _clientId)
                .Include(o => o.Items)
                .OrderByDescending(o => o.Date)
                .ToList();
        }


    }
}

