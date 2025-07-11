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
    /// Interaction logic for AddAuthor.xaml
    /// </summary>
    public partial class AddAuthor : Window
    {
        public AddAuthor()
        {
            InitializeComponent();
        }
        private void ADDAuthor(object sender, RoutedEventArgs e)
        {
            string authorFullName = AuthorFullName.Text.Trim();
            string authorSex = AuthorSex.Text.Trim();
            string authorAddress = AuthorAdress.Text.Trim();
            string authorPhoneNumber = AuthorPhoneNumber.Text.Trim();
            string authorEmail = AuthorEmail.Text.Trim();

            if (Methods.AddUserValidation(authorFullName, authorSex, authorAddress, authorPhoneNumber, authorEmail))
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                    {
                        connection.Open();
                        string query =
                            $"INSERT INTO author (fullName, sex, address, phoneNumber, email) " +
                            $"VALUE('{authorFullName}', '{authorSex}', '{authorAddress}', " +
                            $"'{authorPhoneNumber}', '{authorEmail}'); ";
                        MySqlCommand queryUserCommand = new MySqlCommand(query, connection);
                        queryUserCommand.ExecuteNonQuery();
                        Methods.ShowInformation("Автора додано успішно!");
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
