using LaJusie.Model;
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

namespace LaJusie.Admin
{
    /// <summary>
    /// Логика взаимодействия для JalousiesControl.xaml
    /// </summary>
    public partial class JalousiesControl : UserControl
    {

        private readonly LaJusieEntities db = new LaJusieEntities();

        public JalousiesControl()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Загрузка фильтров
                TypeFilterCombo.ItemsSource = db.Type.ToList();
                MaterialFilterCombo.ItemsSource = db.Materials.ToList();

                // Загрузка данных
                RefreshJalousies();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
        private void RefreshJalousies()
        {

            var query = db.Jalousies
                .Include(j => j.Materials)
                .Include(j => j.Type)
                .AsQueryable();

        // Применяем фильтры
        if (TypeFilterCombo.SelectedItem is LaJusie.Model.Type selectedType)
        {
            query = query.Where(j => j.Type_ID == selectedType.Type_ID);
        }

        if (MaterialFilterCombo.SelectedItem is Materials selectedMaterial)
        {
            query = query.Where(j => j.Material_ID == selectedMaterial.Material_ID);
        }

        JalousiesGrid.ItemsSource = query.ToList();
    }

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshJalousies();
    }

    private void AddJalousie_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new EditJalousieWindow()
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true)
        {
            RefreshJalousies();
        }
    }

    private void EditJalousie_Click(object sender, RoutedEventArgs e)
    {
        if (JalousiesGrid.SelectedItem == null) return;

        var selected = (Jalousies)JalousiesGrid.SelectedItem;
        var dialog = new EditJalousieWindow(selected.Jalousie_ID)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true)
        {
            RefreshJalousies();
        }
    }

    private void DeleteJalousie_Click(object sender, RoutedEventArgs e)
    {
        if (JalousiesGrid.SelectedItem == null) return;

        var selected = (Jalousies)JalousiesGrid.SelectedItem;
        var result = MessageBox.Show(
            $"Удалить комбинацию: {selected.Type.Name} + {selected.Materials.Name}?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                db.Jalousies.Remove(selected);
                db.SaveChanges();
                RefreshJalousies();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshJalousies();
    }
    }
}
