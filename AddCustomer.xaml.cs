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
    /// Interaction logic for AddCustomer.xaml
    /// </summary>
    public partial class AddCustomer : Window
    {
        public AddCustomer()
        {
            InitializeComponent();
        }
        private void ADDCustomer(object sender, RoutedEventArgs e)
        {
            string customerFullName = CustomerFullName.Text.Trim();
            string customerSex = CustomerSex.Text.Trim();
            string customerAddress = CustomerAdress.Text.Trim();
            string customerPhoneNumber = CustomerPhoneNumber.Text.Trim();
            string customerEmail = CustomerEmail.Text.Trim();

            if (Methods.AddUserValidation(customerFullName, customerSex, customerAddress, customerPhoneNumber, customerEmail))
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                    {
                        connection.Open();
                        string query =
                            $"INSERT INTO customer (fullName, sex, address, phoneNumber, email, isRegular) " +
                            $"VALUE('{customerFullName}', '{customerSex}', '{customerAddress}', " +
                            $"'{customerPhoneNumber}', '{customerEmail}', FALSE); ";
                        MySqlCommand queryUserCommand = new MySqlCommand(query, connection);
                        queryUserCommand.ExecuteNonQuery();
                        Methods.ShowInformation("Покупця додано успішно!");
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
