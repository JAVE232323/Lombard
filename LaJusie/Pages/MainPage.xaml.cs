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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaJusie.Model;

namespace LaJusie.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {

        LombardEntities db = new LombardEntities();
        private int User_ID;
        public MainPage(int user_ID)
        {
            InitializeComponent();
            User_ID = user_ID;
            LoadData();
            TypeComboBox();            
        }

        private void LoadData()
        {
            itemsControlJalousies.ItemsSource = db.Jalousies.ToList();
        }

        private void TypeComboBox()
        {
            Typecmb.ItemsSource = db.Type.ToList();
            Materialcmb.ItemsSource = db.Materials.ToList();
        }

        private void findbtn_Click(object sender, RoutedEventArgs e)
        {
            if (Typecmb.SelectedItem is LaJusie.Model.Type selectedType &&
                Materialcmb.SelectedItem is Materials selectedMaterial)
            {
                var filtredJalousies = db.Jalousies
                    .Where(j => j.Type_ID == selectedType.Type_ID &&
                                j.Material_ID == selectedMaterial.Material_ID)
                    .Include(j => j.Materials)
                    .Include(j => j.Type)
                    .ToList();

                itemsControlJalousies.ItemsSource = filtredJalousies;
            }
            else
            {
                MessageBox.Show("Выберите тип и материал!");
            }
        }

        private void MakeOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int jalId)
            {
                MakeOrder makeOrder = new MakeOrder(User_ID, jalId, MainGrid);
                Grid.SetRowSpan(makeOrder, 2);
                MainGrid.Children.Add(makeOrder);
            }

                
        }
    }
}
