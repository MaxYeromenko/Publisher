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
using static iTextSharp.text.pdf.AcroFields;

namespace Publisher
{
    /// <summary>
    /// Interaction logic for BooksGeneralList.xaml
    /// </summary>
    public partial class BooksGeneralList : Window
    {
        int currentRow = 0;
        public BooksGeneralList()
        {
            InitializeComponent();
        }
        private void ChangeNumberBookCopies(object sender, RoutedEventArgs e)
        {
            ChangeNumberOfBookCopies changeNumberOfBookCopies = new ChangeNumberOfBookCopies();
            changeNumberOfBookCopies.ShowDialog();
            UpdateBooksTable(null, null);
        }

        private void AddColumn(string text, int row, int column)
        {
            Border border = new Border
            {
                Height = 30,
                BorderThickness = new Thickness(0, 0, 1, 1),
                BorderBrush = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Top,
                Child = new TextBlock
                {
                    Text = text,
                    Padding = new Thickness(5, 0, 5, 0),
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);
            BooksTable.Children.Add(border);
        }

        private void UpdateBooksTable(object sender, RoutedEventArgs e)
        {
            currentRow = 0;
            BooksTable.Children.Clear();
            BooksTable.RowDefinitions.Clear();
            BooksTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) });
            AddColumn("Назва", currentRow, 0);
            AddColumn("Ціна", currentRow, 1);
            AddColumn("Жанр", currentRow, 2);
            AddColumn("ISBN", currentRow, 3);
            AddColumn("К-ть стр.", currentRow, 4);
            AddColumn("Формат", currentRow, 5);
            AddColumn("К-ть екз.", currentRow++, 6);
            //AddColumn("Замовлені", currentRow++, 7);

            StringBuilder queryWhere = new StringBuilder(" WHERE 1=2 ");

            if(AvailableBooks.IsChecked == true)
            {
                queryWhere.Append(" OR bookNumber > 0 ");
            }
            
            if(CompletelySoldOutBooks.IsChecked == true)
            {
                queryWhere.Append(" OR bookNumber = 0 ");
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT bookName, bookPrice, bookGenreName, ISBN, pagesNumber, " +
                        $"bookFormat, bookNumber FROM publisher.book JOIN bookgenre USING(bookGenreID) " +
                        $"{queryWhere};";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        BooksTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) });
                        AddColumn(reader.GetString(0), currentRow, 0);
                        AddColumn(reader.GetFloat(1).ToString(), currentRow, 1);
                        AddColumn(reader.GetString(2), currentRow, 2);
                        AddColumn(reader.GetString(3), currentRow, 3);
                        AddColumn(reader.GetInt32(4).ToString(), currentRow, 4);
                        AddColumn(reader.GetString(5), currentRow, 5);
                        AddColumn(reader.GetInt32(6).ToString(), currentRow++, 6);
                        //Border border = new Border
                        //{
                        //    Height = 30,
                        //    BorderThickness = new Thickness(0, 0, 1, 1),
                        //    BorderBrush = Brushes.Black,
                        //    VerticalAlignment = VerticalAlignment.Top,
                        //    Child = new CheckBox
                        //    {
                        //        HorizontalAlignment = HorizontalAlignment.Center,
                        //        VerticalAlignment = VerticalAlignment.Center
                        //    }
                        //};
                        //Grid.SetRow(border, currentRow++);
                        //Grid.SetColumn(border, 7);
                        //BooksTable.Children.Add(border);
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
