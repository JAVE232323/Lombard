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
using System.Windows.Shapes;
using LaJusie.Model;

namespace LaJusie.Auth
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {

        LombardEntities db = new LombardEntities();
        public Authorization()
        {
            InitializeComponent();
        }

        private void auth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (logintxb.Text != "" && Pass.Password != "")
                {
                    var user = db.Users.FirstOrDefault(p => p.Login == logintxb.Text && p.Password == Pass.Password);
                    if (user != null)
                    {
                        int id = user.User_ID;

                        MainWindow mainWindow = new MainWindow(id);
                        Close();
                        mainWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль", "Ошибка");
                    }
                }
                else
                {
                    MessageBox.Show("Укажите логин или пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error");
            }
        }

        private void reg_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            Close();
            registration.Show();
        }
    }
}
