using MySql.Data.MySqlClient;
using System;
using System.Collections;
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
    /// Interaction logic for ChangeNumberOfBookCopies.xaml
    /// </summary>
    public partial class ChangeNumberOfBookCopies : Window
    {
        public ChangeNumberOfBookCopies()
        {
            InitializeComponent();
        }
        private void UpdateBookTitles(object sender, EventArgs e)
        {
            BookTitlesList.Items.Clear();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT bookName FROM book ORDER BY bookName ASC;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        BookTitlesList.Items.Add(reader.GetString(0));
                    }
                }
            }
            catch
            {
                Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
            }
        }
        private void AddBook(object sender, RoutedEventArgs e)
        {
            try
            {
                int bookNumber = int.Parse(BookNumber.Text);
                string bookName = BookTitlesList.Text;

                if (!string.IsNullOrEmpty(bookName))
                {
                    using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                    {
                        connection.Open();
                        string query = $"SELECT bookNumber FROM book WHERE bookName = '{bookName}';";
                        MySqlCommand queryUserCommand = new MySqlCommand(query, connection);
                        int? result = (int)queryUserCommand.ExecuteScalar();

                        if (result != null)
                        {
                            if (result >= -bookNumber)
                            {
                                query = $"UPDATE book SET bookNumber = bookNumber + {bookNumber} " +
                                $"WHERE bookName = '{bookName}';";
                                queryUserCommand = new MySqlCommand(query, connection);
                                queryUserCommand.ExecuteNonQuery();
                                string shortBookName = bookName.Length > 15 ? bookName.Substring(0, 15) : bookName;
                                Methods.ShowInformation($"Кількість екземплярів \"{shortBookName}... \" " +
                                    $"становить {result + bookNumber}.");
                            }
                            else
                            {
                                Methods.ShowWarning($"Кількість книжок не може бути нижчою за нуль.");
                            }
                        }
                    }
                }
                else
                    throw new Exception();
            }
            catch
            {
                Methods.ShowError($"Перевірте правильність заповнення полів.");
            }
        }
    }
}
