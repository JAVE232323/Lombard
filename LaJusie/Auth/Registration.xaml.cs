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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        LombardEntities db = new LombardEntities();
        public Registration()
        {
            InitializeComponent();
        }

        private void registr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = db.Users.FirstOrDefault(p => p.Login == logintxb.Text);

                if (user != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует");
                } else
                {
                    if (Pass1.Password == Pass2.Password)
                    {
                        Users newUser = new Users
                        {
                            Login = logintxb.Text,
                            Password = Convert.ToString(Pass2.Password),
                            Role_ID = 2
                        };

                        db.Users.Add(newUser);
                        db.SaveChanges();

                        Authorization authorization = new Authorization();
                        Close();
                        authorization.Show();
                    }
                    else
                    {
                        MessageBox.Show("пароль не совпадает");
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERRoR");
            }
        }

        private void auth_Click(object sender, RoutedEventArgs e)
        {
            Authorization auth = new Authorization();
            Close();
            auth.Show();
        }
    }
}
