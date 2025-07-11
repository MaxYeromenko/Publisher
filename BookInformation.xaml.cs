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
    /// Interaction logic for BookInformation.xaml
    /// </summary>
    public partial class BookInformation : Window
    {
        public BookInformation(int bookID)
        {
            InitializeComponent();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.rootConnection))
                {
                    connection.Open();
                    string query = $"SELECT bookImage, bookName, bookPrice, bookGenreName, ISBN, pagesNumber, " +
                        $"bookFormat, bookDescription, bookNumber, fullName FROM book JOIN bookgenre USING(bookGenreID) " +
                        $"JOIN bookauthor USING(bookID) JOIN author USING(authorID) WHERE bookID = {bookID};";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            string imagePath = reader.GetString(0);
                            BookImage.Source = new BitmapImage(new Uri(imagePath));
                        }
                        catch
                        {
                            BookImage.Source = new BitmapImage(new Uri("/sources/images/empty.png", UriKind.Relative));
                        }

                        BookTitle.Text = reader.GetString(1);
                        BookPrice.Text = reader.GetFloat(2).ToString();
                        BookGenre.Text = reader.GetString(3);
                        BookISBN.Text = reader.GetString(4);
                        BookPagesNumber.Text = reader.GetInt32(5).ToString();
                        BookFormat.Text = reader.GetString(6);
                        BookNumber.Text = reader.GetInt32(8).ToString();
                        BookDescription.Text = reader.GetString(7);
                        BookAuthor.Text = reader.GetString(9);
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
