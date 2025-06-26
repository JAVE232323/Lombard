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

namespace LaJusie.Orders
{
    /// <summary>
    /// Логика взаимодействия для StatusSelection.xaml
    /// </summary>
    public partial class StatusSelection : UserControl
    {
        public event Action<Status> StatusSelected;
        public StatusSelection()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (Parent is Panel parent)
            {
                parent.Children.Remove(this);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxStatuses.SelectedItem is Status selectedStatus)
            {
                StatusSelected?.Invoke(selectedStatus);
            }
            else
            {
                MessageBox.Show("Выберите статус!");
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Parent is Panel parent)
            {
                parent.Children.Remove(this);
            }
        }
    }
}
