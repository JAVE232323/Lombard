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
    /// Логика взаимодействия для AddClient.xaml
    /// </summary>
    public partial class AddClient : UserControl
    {
        private LombardEntities db = new LombardEntities();
        private int User_ID;
        private int JalID;
        private Grid MainGrid;

        public AddClient(int user_ID, int JalId, Grid grid)
        {
            InitializeComponent();
            User_ID = user_ID;
            JalID = JalId;
            MainGrid = grid;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MakeOrder makeOrder = new MakeOrder(User_ID, JalID, MainGrid);
            Grid.SetRowSpan(makeOrder, 2);
            MainGrid.Children.Add(makeOrder);
            this.Visibility = Visibility.Collapsed;
        }


        private void SearchClient_Click(object sender, RoutedEventArgs e)
        {

            

            string lastName = txtLastName.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string middleName = txtMiddleName.Text.Trim();

            var query = db.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
                query = query.Where(c => c.FirstName.Contains(firstName));

            if (!string.IsNullOrEmpty(lastName))
                query = query.Where(c => c.LastName.Contains(lastName));            

            if (!string.IsNullOrEmpty(middleName))
                query = query.Where(c => c.MiddleName.Contains(middleName));

            ClientsResults.ItemsSource = query.ToList();
        }


        private void SelectClient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int clientId)
            {
                OtherInfoOrder otherInfoOrder = new OtherInfoOrder(User_ID, JalID, clientId, MainGrid);
                Grid.SetRowSpan(otherInfoOrder, 2);
                MainGrid.Children.Add(otherInfoOrder);
                this.Visibility = Visibility.Collapsed;
            }
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClient2 addClient = new AddClient2(User_ID, JalID, MainGrid);
            Grid.SetRowSpan(addClient, 2);
            MainGrid.Children.Add(addClient);
            this.Visibility = Visibility.Collapsed;
        }
    }
}
