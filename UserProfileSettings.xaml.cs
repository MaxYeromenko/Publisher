using MySql.Data.MySqlClient;
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

namespace Publisher
{
    /// <summary>
    /// Interaction logic for UserProfileSettings.xaml
    /// </summary>
    public partial class UserProfileSettings : Window
    {
        public UserProfileSettings()
        {
            InitializeComponent();
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            home.Show();
            Close();
        }

        public void LogOut()
        {
            SessionData.UserId = null;
            SessionData.ConnectionString = null;
            SessionData.isEmployee = false;
            SessionData.SaveSessionData();
            Methods.ShowInformation("Ви вийшли з акаунту.");
            GoHome(null, null);
        }

        private void PasteInfo(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    if (SessionData.isEmployee)
                    {
                        string query = $"SELECT fullName, sex, phoneNumber, email, passport, post " +
                            $"FROM employee JOIN employeepost USING(postID) " +
                            $"WHERE email = '{SessionData.UserId}';";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            EmployeeFullName.Text = reader["fullName"].ToString();
                            EmployeePhoneNumber.Text = reader["phoneNumber"].ToString();
                            EmployeeEmail.Text = reader["email"].ToString();
                            EmployeeSex.Text = reader["sex"].ToString();
                            EmployeePassport.Text = reader["passport"].ToString();
                            EmployeePost.Text = reader["post"].ToString();
                        }
                    }
                }
            }
            catch
            {
                Methods.ShowError($"Виникла помилка під час входу в акаунт. Увійдіть ще раз, будь ласка.");
                LogOut();
            }
        }

        private void LogOut(object sender, RoutedEventArgs e)
        {
            if (Methods.ShowConfirmation("Вийти з акаунту?") == MessageBoxResult.Yes)
            {
                LogOut();
            }
        }
    }
}
