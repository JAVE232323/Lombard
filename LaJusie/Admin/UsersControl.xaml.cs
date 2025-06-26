using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using LaJusie.Pages;

namespace LaJusie.Admin
{
    /// <summary>
    /// Логика взаимодействия для UsersControl.xaml
    /// </summary>
    public partial class UsersControl : UserControl
    {
        LombardEntities db = new LombardEntities();

        public UsersControl()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            EmployeesGrid.ItemsSource = db.Users
                .OrderBy(x => x.User_ID)
                .Join(db.Role,
                user => user.Role_ID,
                role => role.Role_ID,
                (user, role) => new // Анонимный тип
                {
                    user.User_ID,
                    user.Login,
                    user.Password,
                    RoleName = role.Name // Добавляем название роли
                 })
                .ToList();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Dispatcher.InvokeAsync(() =>
                {
                    string searchText = SearchTextBox.Text.Trim().ToLower();

                    IQueryable<Users> query = db.Users.OrderBy(x => x.User_ID);

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query = query.Where(u => u.Login.ToLower().Contains(searchText));
                    }

                    EmployeesGrid.ItemsSource = query
                        .Join(db.Role,
                    user => user.Role_ID,
                    role => role.Role_ID,
                    (user, role) => new // Анонимный тип
                    {
                        user.User_ID,
                        user.Login,
                        user.Password,
                        RoleName = role.Name // Добавляем название роли
                    })
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

        private void EmployeesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem == null) return;

            dynamic selectedUser = EmployeesGrid.SelectedItem;
            int userId = selectedUser.User_ID;

            var editWindow = new EditUserWindow(userId)
            {
                Owner = Window.GetWindow(this)
            };

            if (editWindow.ShowDialog() == true)
            {
                LoadUsers(); // Обновляем список после редактирования
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem == null) return;

            dynamic selectedUser = EmployeesGrid.SelectedItem;
            int userId = selectedUser.User_ID;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {selectedUser.Login}?",
                                        "Подтверждение удаления",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var userToDelete = db.Users.FirstOrDefault(u => u.User_ID == userId);
                    if (userToDelete != null)
                    {
                        db.Users.Remove(userToDelete);
                        db.SaveChanges();
                        LoadUsers();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}");
                }
            }
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddUserWindow()
            {
                Owner = Window.GetWindow(this)
            };

            if (addWindow.ShowDialog() == true)
            {
                LoadUsers(); 
            }
        }

        private void RefreshEmployees_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }


    }
}
