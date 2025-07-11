using MySql.Data.MySqlClient;
using System;
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
    /// Interaction logic for DatePick.xaml
    /// </summary>
    public partial class DatePick : Window
    {
        public DatePick()
        {
            InitializeComponent();
            if (BookAuthorPopularityList.IsSalesReport)
            {
                NumberStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void SelectElements(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime startDate = DateTime.Parse(StartDate.Text);
                DateTime endDate = DateTime.Parse(EndDate.Text);

                string numberOfElements = NumberOfElements.Text.Trim();
                if (startDate <= endDate && (string.IsNullOrEmpty(numberOfElements) || int.Parse(numberOfElements) > 0) &&
                    (DateTime.Now >= startDate && DateTime.Now >= endDate))
                {
                    using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                    {
                        connection.Open();
                        string sStartDate = DateTime.ParseExact(StartDate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        string sEndDate = DateTime.ParseExact(EndDate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        string query = "";
                        if (BookAuthorPopularityList.IsBook)
                        {
                            query = $"SELECT bookName, SUM(booksNumber), SUM(booksNumber * bookPrice) " +
                                    $"FROM customBooks JOIN book USING(bookID) JOIN custom c USING(customID) " +
                                    $"WHERE customDate >= '{sStartDate}' AND customDate <= '{sEndDate}' " +
                                    $"GROUP BY 1 ORDER BY 2 DESC";
                        }
                        else if (BookAuthorPopularityList.IsAuthor)
                        {
                            query = $"SELECT CONCAT(SUBSTRING_INDEX(fullName, ' ', 1), ' ', " +
                                $"LEFT(SUBSTRING_INDEX(fullName, ' ', -2), 1), '.', " +
                                $"LEFT(SUBSTRING_INDEX(fullName, ' ', -1), 1), '.'), " +
                                $"SUM(booksNumber), SUM(booksNumber * bookPrice) " +
                                $"FROM customBooks JOIN book USING(bookID) JOIN custom c USING(customID) " +
                                $"JOIN bookauthor USING(bookID) JOIN author USING(authorID) " +
                                $"WHERE customDate >= '{sStartDate}' AND customDate <= '{sEndDate}' " +
                                $"GROUP BY 1 ORDER BY 2 DESC";
                        }
                        else if (BookAuthorPopularityList.IsSalesReport)
                        {
                            query = $"SELECT SUM(booksNumber*bookPrice) FROM custom " +
                                $"JOIN custombooks USING(customID) JOIN book USING(bookID) " +
                                $"WHERE customDate >= '{sStartDate}' AND customDate <= '{sEndDate}';";
                            MySqlCommand commandLocal = new MySqlCommand(query, connection);
                            MySqlDataReader readerLocal = commandLocal.ExecuteReader();

                            if (readerLocal.Read())
                            {
                                if (!readerLocal.IsDBNull(0))
                                    Methods.ShowInformation($"Загальний дохід за цей період: {Math.Round(readerLocal.GetDouble(0), 2)} грн.");
                                else
                                    Methods.ShowInformation("За цей час продажі відсутні.");
                            }
                        }
                        if (!BookAuthorPopularityList.IsSalesReport)
                        {
                            if (!string.IsNullOrEmpty(numberOfElements))
                            {
                                query = string.Concat(query, $" LIMIT {numberOfElements}");
                            }

                            MySqlCommand command = new MySqlCommand(query, connection);
                            MySqlDataReader reader = command.ExecuteReader();
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    BookAuthorPopularity bookAuthorPopularity = new BookAuthorPopularity
                                    {
                                        Name = reader.GetString(0),
                                        TotalBooks = reader.GetInt32(1),
                                        TotalIncome = reader.GetFloat(2)
                                    };
                                    BookAuthorPopularityList.itemsList.Add(bookAuthorPopularity);
                                }
                            else Methods.ShowInformation("За цей час продажі відсутні.");
                        }
                    }
                    BookAuthorPopularityList.IsBook = false;
                    BookAuthorPopularityList.IsAuthor = false;
                    BookAuthorPopularityList.IsSalesReport = false;

                    Close();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                Methods.ShowError($"Перевірте правильність заповнення полів. Початкова дата має бути меншою за кінцеву та " +
                    $"обрані дати не мають бути більшими за поточну, а кількість більшою за нуль або порожньою щоб отримати повний результат.");
            }
        }
    }
}
