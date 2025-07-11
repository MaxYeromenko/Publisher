using MySql.Data.MySqlClient;
using System;
using System.CodeDom;
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
    /// Interaction logic for OrderBook.xaml
    /// </summary>
    public partial class OrderBook : Window
    {
        public OrderBook()
        {
            InitializeComponent();
        }

        private void AddBook(object sender, RoutedEventArgs e)
        {
            try
            {
                int bookNumber = int.Parse(BookNumber.Text);
                string bookName = BookTitlesList.Text;

                if (bookNumber > 0 && !string.IsNullOrEmpty(bookName))
                {
                    using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                    {
                        connection.Open();
                        string query = $"SELECT bookID, bookPrice, bookNumber FROM book WHERE bookName = '{bookName}';";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            int availableBooks = reader.GetInt32(2);
                            BookInfo orderedBook = OrderedBookList.orderedBooks.FirstOrDefault(b => b.BookName == bookName);

                            if (orderedBook != null)
                            {
                                if (availableBooks >= orderedBook.BookNumber + bookNumber)
                                {
                                    orderedBook.BookNumber += bookNumber;
                                    Close();
                                    return;
                                }
                                else if (availableBooks == orderedBook.BookNumber)
                                {
                                    Methods.ShowWarning($"Кількість книжок на складі перевищена. На цей момент немає доступних книжок.");
                                }
                                else
                                {
                                    Methods.ShowWarning($"Кількість книжок на складі перевищена. На цей момент {availableBooks - orderedBook.BookNumber} доступно.");
                                }
                            }
                            else if (availableBooks - bookNumber >= 0)
                            {
                                BookInfo book = new BookInfo
                                {
                                    BookName = bookName,
                                    BookNumber = bookNumber,
                                    BookID = reader.GetInt32(0),
                                    BookPrice = (float)Math.Round(reader.GetFloat(1), 2)
                                };
                                OrderedBookList.orderedBooks.Add(book);
                                Close();
                                return;
                            }
                            else
                            {
                                Methods.ShowWarning($"Кількість книжок на складі перевищена. На цей момент {availableBooks} доступно.");
                            }
                        }
                    }
                }
                else
                    throw new Exception();
            }
            catch
            {
                Methods.ShowError("Перевірте правильність заповнення полів.");
            }
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
    }
}
