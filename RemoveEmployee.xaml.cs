using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for RemoveEmployee.xaml
    /// </summary>
    public partial class RemoveEmployee : Window
    {
        public RemoveEmployee()
        {
            InitializeComponent();
        }

        private void REMOVEEmployee(object sender, RoutedEventArgs e)
        {
            if (EmployeeList.SelectedValue != null)
            {
                if (Methods.ShowConfirmation("Ви впевнені, що хочете вилучити працівника.") == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                        {
                            connection.Open();
                            int employeeID = int.Parse(EmployeeList.SelectedValue.ToString().Split()[0]);
                            string currentDate = DateTime.Now.ToShortDateString();
                            string sFireDate = DateTime.ParseExact(currentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            string query = $"UPDATE employeeinfo SET fireDate = '{sFireDate}' " +
                                $" WHERE employeeID = {employeeID} ";
                            MySqlCommand queryUserCommand = new MySqlCommand(query, connection);
                            queryUserCommand.ExecuteNonQuery();
                            query = $"UPDATE employee SET postID = 6 WHERE employeeID = {employeeID};";
                            queryUserCommand = new MySqlCommand(query, connection);
                            queryUserCommand.ExecuteNonQuery();
                            Methods.ShowInformation("Операцію успішно виконано!");
                            Close();
                        }
                    }
                    catch
                    {
                        Methods.ShowError("Виникла помилка під час спроби вилучити працівника. Спробуйте ще раз.");
                    }
                }
            }
            else Methods.ShowWarning("Оберіть працівника.");
        }

        private void UpdateEmployeeList(object sender, EventArgs e)
        {
            EmployeeList.Items.Clear();
            using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
            {
                connection.Open();
                string query = $"SELECT employeeID, fullName FROM employee " +
                    $"WHERE postID != 6 ORDER BY employeeID ASC;";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read()) EmployeeList.Items.Add($"{reader.GetInt32(0)} {reader.GetString(1)}");
            }
        }
    }
}
