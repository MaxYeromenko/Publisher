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
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace Publisher
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void ShowPassword(object sender, MouseEventArgs e)
        {
            LogInTextBoxPassword.Text = LogInPasswordBox.Password;
            LogInPasswordBox.Visibility = Visibility.Collapsed;
            LogInTextBoxPassword.Visibility = Visibility.Visible;
        }
        private void HidePassword(object sender, MouseEventArgs e)
        {
            LogInPasswordBox.Visibility = Visibility.Visible;
            LogInTextBoxPassword.Visibility = Visibility.Collapsed;
        }

        private void BTNLogIn(object sender, RoutedEventArgs e)
        {
            string myUsername = Email.Text.Trim();
            string myPassword = LogInPasswordBox.Password.Trim();

            string connectionString = $"Server=localhost;Database=publisher;Uid={myUsername};Pwd={myPassword};";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    SessionData.UserId = myUsername;
                    SessionData.ConnectionString = connectionString;
                    string query = $"SELECT * FROM employee WHERE email = '{SessionData.UserId}'";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows) SessionData.isEmployee = true;
                    else throw new Exception();
                    Methods.ShowInformation($"Ви успішно ввійшли до акаунта!");
                    SessionData.SaveSessionData();
                    GoHome(null, null);
                }
            }
            catch
            {
                Methods.ShowError($"Неправильний логін або пароль.");
            }
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            home.Show();
            Close();
        }
    }
}
