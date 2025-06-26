using LaJusie.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Shapes;

namespace LaJusie.Admin
{
    /// <summary>
    /// Логика взаимодействия для EditJalousieWindow.xaml
    /// </summary>
    public partial class EditJalousieWindow : Window
    {
        private readonly Jalousies _jalousie;
        private readonly bool _isEditMode;

        public string WindowTitle => _isEditMode ?
            $"Редактирование жалюзи №{_jalousie.Jalousie_ID}" :
            "Новая жалюзи";

        public EditJalousieWindow()
        {
            InitializeComponent();
            _jalousie = new Jalousies();
            _isEditMode = false;
            DataContext = this;
            LoadComboBoxData();
        }

        public EditJalousieWindow(int jalousieId) : this()
        {
            using (var db = new LaJusieEntities())
            {
                _jalousie = db.Jalousies
                    .Include("Type")
                    .Include("Materials")
                    .FirstOrDefault(j => j.Jalousie_ID == jalousieId);

                if (_jalousie == null)
                {
                    MessageBox.Show("Комбинация не найдена!");
                    Close();
                    return;
                }

                _isEditMode = true;
                LoadExistingData();
            }
        }

        private void LoadComboBoxData()
        {
            using (var db = new LaJusieEntities())
            {
                try
                {
                    // Загружаем типы жалюзи
                    TypeComboBox.ItemsSource = db.Type.ToList();
                    TypeComboBox.SelectedValuePath = "Type_ID";
                    TypeComboBox.DisplayMemberPath = "Name";

                    // Загружаем материалы
                    MaterialComboBox.ItemsSource = db.Materials.ToList();
                    MaterialComboBox.SelectedValuePath = "Material_ID";
                    MaterialComboBox.DisplayMemberPath = "Name";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                    Close();
                }
            }
        }

        private void LoadExistingData()
        {
            TypeComboBox.SelectedValue = _jalousie.Type_ID;
            MaterialComboBox.SelectedValue = _jalousie.Material_ID;
            PriceTextBox.Text = _jalousie.Price.ToString("0.00");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (TypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип жалюзи!");
                return;
            }

            if (MaterialComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите материал!");
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Укажите корректную цену (положительное число)!");
                return;
            }

            // Обновляем данные объекта
            _jalousie.Type_ID = (int)TypeComboBox.SelectedValue;
            _jalousie.Material_ID = (int)MaterialComboBox.SelectedValue;
            _jalousie.Price = Convert.ToInt32(price);

            // Сохраняем в БД
            using (var db = new LaJusieEntities())
            {
                try
                {
                    if (_isEditMode)
                    {
                        db.Entry(_jalousie).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        db.Jalousies.Add(_jalousie);
                    }

                    db.SaveChanges();
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Валидация ввода цены (только числа)
        private void PriceTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }


    }


}

