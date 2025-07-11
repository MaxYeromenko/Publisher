using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Publisher
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        public AddEmployee()
        {
            InitializeComponent();
        }
        private void UpdateEmployee(object sender, EventArgs e)
        {
            EmployeePostList.Items.Clear();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT postID, post FROM employeepost WHERE reqAmount > 0;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeePostList.Items.Add($"{reader.GetInt32(0)} {reader.GetString(1)}");
                    }
                }
            }
            catch
            {
                Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
            }
        }
        private void ADDEmployee(object sender, RoutedEventArgs e)
        {
            string employeeFullName = EmployeeFullName.Text.Trim();
            string employeeSex = EmployeeSex.Text.Trim();
            string employeePhoneNumber = EmployeePhoneNumber.Text.Trim();
            string employeeEmail = EmployeeEmail.Text.Trim();
            string employeePassport = EmployeePassport.Text.Trim();
            string employeePostID = EmployeePostList.Text.Trim().Split()[0];
            string employeeBirthDate = EmployeeBirthDate.Text.Trim();
            string employeeFamilyStatus = EmployeeFamilyStatus.Text.Trim();
            string employeeChildrenNumber = EmployeeChildrenNumber.Text.Trim();
            string employeeExperience = EmployeeExperience.Text.Trim();
            string employeeOrdersCompleted = EmployeeOrdersCompleted.Text.Trim();

            if (Methods.AddEmployeeValidation(employeeFullName, employeeSex, employeePhoneNumber,
                employeePassport, employeePostID, employeeBirthDate, employeeFamilyStatus, employeeChildrenNumber,
                employeeExperience, employeeOrdersCompleted, employeeEmail))
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                    {
                        connection.Open();
                        string sEmployeeBirthDate = DateTime.ParseExact(employeeBirthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        string query =
                            $"INSERT INTO employee (fullName, sex, phoneNumber, email, passport, postID," +
                            $" birthDate, familyStatus, childrenNumber, experience, ordersCompleted) " +
                            $"VALUE('{employeeFullName}', '{employeeSex}', '{employeePhoneNumber}', '{employeeEmail}', " +
                            $"'{employeePassport}', {employeePostID}, '{sEmployeeBirthDate}', '{employeeFamilyStatus}', " +
                            $"{employeeChildrenNumber}, {employeeExperience}, {employeeOrdersCompleted}); ";
                        MySqlCommand queryCommand = new MySqlCommand(query, connection);
                        queryCommand.ExecuteNonQuery();

                        query = $"UPDATE employeepost SET reqAmount = reqAmount - 1 WHERE postID = {employeePostID};";
                        queryCommand = new MySqlCommand(query, connection);
                        queryCommand.ExecuteNonQuery();

                        query = "SELECT LAST_INSERT_ID()";
                        queryCommand = new MySqlCommand(query, connection);
                        object result = queryCommand.ExecuteScalar();
                        if (result != null)
                        {
                            string currentDate = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            query = $"INSERT INTO employeeInfo (employeeID, hireDate, fireDate) " +
                                    $"VALUES ({Convert.ToInt32(result)}, '{currentDate}', NULL);";
                            queryCommand = new MySqlCommand(query, connection);
                            queryCommand.ExecuteNonQuery();
                        }
                        Methods.ShowInformation("Працівника додано успішно!");
                    }
                    Close();
                }
                catch
                {
                    Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
                }
            }
        }
    }
}
