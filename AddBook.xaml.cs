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
    /// Interaction logic for AddBook.xaml
    /// </summary>
    public partial class AddBook : Window
    {
        public AddBook()
        {
            InitializeComponent();
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            home.Show();
            Close();
        }

        private void ADDBook(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    if (Methods.AddBookValidation(BookImage.Text, BookName.Text, float.Parse(BookPrice.Text),
                        int.Parse(GenreList.Text.Split(' ')[0]), int.Parse(AuthorList.Text.Split(' ')[0]), BookISBN.Text,
                        int.Parse(BookPagesNumber.Text), int.Parse(BookFormatList.Text.Split(' ')[0]), BookDescription.Text,
                        int.Parse(BookNumber.Text)))
                    {
                        string query = $"INSERT INTO book (bookImage, bookName, bookPrice, " +
                            $"bookGenreID, ISBN, pagesNumber, bookFormat, bookDescription, bookNumber) " +
                            $"VALUES ('{BookImage.Text}', '{BookName.Text}', {BookPrice.Text.Replace(',', '.')}, " +
                            $"{GenreList.Text.Split(' ')[0]}, '{BookISBN.Text}', {BookPagesNumber.Text}, " +
                            $"{BookFormatList.Text.Split(' ')[0]}, '{BookDescription.Text}', {BookNumber.Text})";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        query = "SELECT LAST_INSERT_ID()";
                        command = new MySqlCommand(query, connection);
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            query = $"INSERT INTO bookAuthor (authorID, bookID) VALUES " +
                                $"({AuthorList.Text.Split(' ')[0]}, {Convert.ToInt32(result)})";
                            command = new MySqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }
                        Methods.ShowInformation("Книгу додано успішно!");
                        GoHome(null, null);
                    }
                }
            }
            catch
            {
                Methods.ShowError("Перевірте введені дані.");
            }
        }

        private void ADDAuthor(object sender, RoutedEventArgs e)
        {
            AddAuthor addAuthor = new AddAuthor();
            addAuthor.Show();
        }

        private void LoadGenresAuthors(object sender, RoutedEventArgs e)
        {
            try
            {
                BookFormatList.Items.Add("1 обкладинка м'яка");
                BookFormatList.Items.Add("2 обкладинка тверда");
                BookFormatList.Items.Add("3 електронний");
                BookFormatList.Items.Add("4 аудіо");
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT bookGenreID, bookGenreName FROM bookgenre;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        GenreList.Items.Add($"{reader.GetInt32(0)} {reader.GetString(1).Split(' ')[0]}");
                    }
                }
                UpdateAuthorList(sender, null);
            }
            catch
            {
                Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
            }
        }

        private void UpdateAuthorList(object sender, EventArgs e)
        {
            AuthorList.Items.Clear();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT authorID, CONCAT(SUBSTRING_INDEX(fullName, ' ', 1), ' ', " +
                        $"LEFT(SUBSTRING_INDEX(fullName, ' ', -2), 1), '.', " +
                        $"LEFT(SUBSTRING_INDEX(fullName, ' ', -1), 1), '.') FROM author;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        AuthorList.Items.Add($"{reader.GetInt32(0)} {reader.GetString(1)}");
                    }
                }
            }
            catch
            {
                Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
            }
        }

        private void ChangeNumberBookCopies(object sender, RoutedEventArgs e)
        {
            ChangeNumberOfBookCopies changeNumberOfBookCopies = new ChangeNumberOfBookCopies();
            changeNumberOfBookCopies.Show();
        }
    }
}
