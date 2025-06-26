using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaJusie.Pages
{
    /// <summary>
    /// Логика взаимодействия для MakeOrder.xaml
    /// </summary>
    public partial class MakeOrder : UserControl
    {
        private int User_ID;
        private int JalID;
        private Grid MainGrid;
        

        public MakeOrder(int user_ID, int JalId, Grid mainGrid)
        {
            InitializeComponent();
            User_ID = user_ID;
            JalID = JalId;
            MainGrid = mainGrid;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // Только цифры
            e.Handled = regex.IsMatch(e.Text);

            if (e.Handled)
            {
                MessageBox.Show("Допустимы только цифры!", "Ошибка ввода",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            AddClient addClient = new AddClient(User_ID, JalID, MainGrid);
            Grid.SetRowSpan(addClient, 2);
            MainGrid.Children.Add(addClient);
            this.Visibility = Visibility.Collapsed;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
