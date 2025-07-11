using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Publisher
{
    /// <summary>
    /// Interaction logic for SalesDepartment.xaml
    /// </summary>
    public partial class SalesDepartment : Window
    {
        int currentPage = 0;

        int currentRow { get; set; } = 0;

        StringBuilder queryHaving = new StringBuilder();
        StringBuilder queryWhere = new StringBuilder(" WHERE 1=1 ");

        int pageNumber
        {
            get => currentPage;
            set
            {
                if (currentPage + value >= 0)
                    currentPage += value;
                else
                    Methods.ShowInformation("Це перша сторінка!");
            }
        }
        bool dateToLowest = true;
        bool priceToLowest = true;
        string imgDateToLowest = "/sources/images/dateToLowest.png",
            imgDateToHighest = "/sources/images/dateToHighest.png";
        string imgPriceToLowest = "/sources/images/priceToLowest.png",
            imgPriceToHighest = "/sources/images/priceToHighest.png";
        string reqOrders = "";

        public SalesDepartment()
        {
            InitializeComponent();
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            home.Show();
            Close();
        }

        private void GoToNextPage(object sender, RoutedEventArgs e)
        {
            pageNumber = 5;
            PageNumber.Text = (pageNumber / 5 + 1).ToString();
            ApplyAllFilters(sender, null);
        }

        private void GoToPreviousPage(object sender, RoutedEventArgs e)
        {
            pageNumber = -5;
            PageNumber.Text = (pageNumber / 5 + 1).ToString();
            ApplyAllFilters(sender, null);
        }

        private void ResetSortByDate()
        {
            dateToLowest = true;
            ImgSortByDate.Source = new BitmapImage(new Uri(imgDateToLowest, UriKind.Relative));
        }

        private void ResetSortByPrice()
        {
            priceToLowest = true;
            ImgSortByPrice.Source = new BitmapImage(new Uri(imgPriceToLowest, UriKind.Relative));
        }

        private void PopularityListItemTemplate(string name, int bookNumber, float totalIncome)
        {
            PopularityListGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) });
            AddColumn(name, ++currentRow, 0);
            AddColumn(bookNumber.ToString(), currentRow, 1);
            AddColumn(Math.Round(totalIncome, 2).ToString(), currentRow, 2);
        }

        private void AddColumn(string text, int row, int column)
        {
            Border border = new Border
            {
                BorderThickness = new Thickness(0, 0, 1, 1),
                BorderBrush = Brushes.Black,
                Child = new TextBlock
                {
                    Text = text,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                }
            };
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);
            PopularityListGrid.Children.Add(border);
        }

        private void CloseInfo(object sender, RoutedEventArgs e)
        {
            PopularityListGridBorder.Visibility = Visibility.Collapsed;
            FiltersGrid.Visibility = Visibility.Collapsed;
            BTNCloseInfo.Visibility = Visibility.Collapsed;
            InfoTitle.Visibility = Visibility.Collapsed;
            BGOrderInfo.Visibility = Visibility.Collapsed;
            BTNPrintReceipt.Visibility = Visibility.Collapsed;
        }

        private void AuthorsBooksPopularity(object sender, RoutedEventArgs e)
        {
            currentRow = 0;
            BookAuthorPopularityList.itemsList.Clear();
            PopularityListGrid.Children.Clear();
            PopularityListGrid.RowDefinitions.Clear();
            Button button = (Button)sender;
            string buttonName = button.Name;
            BookAuthorPopularityList.IsBook = buttonName == "BTNBooksPopularity";
            BookAuthorPopularityList.IsAuthor = buttonName == "BTNAuthorsPopularity";
            DatePick datePick = new DatePick();
            datePick.ShowDialog();
            if (!BookAuthorPopularityList.IsBook && !BookAuthorPopularityList.IsAuthor
                && BookAuthorPopularityList.itemsList.Count > 0)
            {
                BTNCloseInfo.Visibility = Visibility.Visible;
                PopularityListGridBorder.Visibility = Visibility.Visible;
                PopularityListGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) });
                AddColumn("Назва / ПІБ", 0, 0);
                AddColumn("Продано книжок", 0, 1);
                AddColumn("Загальний дохід", 0, 2);
                foreach (var item in BookAuthorPopularityList.itemsList)
                {
                    PopularityListItemTemplate(item.Name, item.TotalBooks, item.TotalIncome);
                }
                BookAuthorPopularityList.IsBook = false;
                BookAuthorPopularityList.IsAuthor = false;
            }
        }

        private void SalesReportSelection(object sender, RoutedEventArgs e)
        {
            BookAuthorPopularityList.IsSalesReport = true;
            DatePick datePick = new DatePick();
            datePick.ShowDialog();
            if (!BookAuthorPopularityList.IsSalesReport)
            {
                BookAuthorPopularityList.IsSalesReport = false;
            }
        }

        private void OpenFilters(object sender, RoutedEventArgs e)
        {
            BTNCloseInfo.Visibility = Visibility.Visible;
            FiltersGrid.Visibility = Visibility.Visible;
        }

        private void ClearAllFilters(object sender, RoutedEventArgs e)
        {
            OrderList.Children.Clear();
            MinPrice.Text = string.Empty;
            MaxPrice.Text = string.Empty;
            MinNumber.Text = string.Empty;
            MaxNumber.Text = string.Empty;
            StartDate.Text = string.Empty;
            EndDate.Text = string.Empty;
            DeliveryType.Text = string.Empty;
            queryHaving.Clear();
            queryWhere.Clear();
            queryWhere.Append(" WHERE 1=1 ");
            ApplyAllFilters(null, null);
        }

        private void ApplyAllFilters(object sender, RoutedEventArgs e)
        {
            try
            {
                OrderList.Children.Clear();
                string minPrice = MinPrice.Text;
                string maxPrice = MaxPrice.Text;
                string startDate = StartDate.Text;
                string endDate = EndDate.Text;
                string minNumber = MinNumber.Text;
                string maxNumber = MaxNumber.Text;

                queryHaving = new StringBuilder();
                queryWhere = new StringBuilder(" WHERE 1=1 ");

                if (!(string.IsNullOrEmpty(minPrice) && string.IsNullOrEmpty(maxPrice)) &&
                    Methods.ValidateIntField(minPrice, maxPrice) &&
                    int.Parse(minPrice) <= int.Parse(maxPrice))
                {
                    queryHaving.Append($" HAVING (SUM(bookPrice * booksNumber) >= {minPrice} AND SUM(bookPrice * booksNumber) <= {maxPrice}) ");
                }

                if (!(string.IsNullOrEmpty(minNumber) && string.IsNullOrEmpty(maxNumber)) &&
                    Methods.ValidateIntField(minNumber, maxNumber) &&
                    int.Parse(minNumber) <= int.Parse(maxNumber))
                {
                    if (queryHaving.Length > 0)
                        queryHaving.Append($" AND SUM(booksNumber) >= {minNumber} AND SUM(booksNumber) <= {maxNumber} ");
                    else
                        queryHaving.Append($" HAVING SUM(booksNumber) >= {minNumber} AND SUM(booksNumber) <= {maxNumber} ");
                }

                if (Methods.ValidateDateField(startDate, endDate))
                {
                    if (DateTime.Parse(startDate) <= DateTime.Parse(endDate) && DateTime.Parse(startDate) <= DateTime.Now
                        && DateTime.Parse(endDate) <= DateTime.Now)
                    {
                        string sStartDate = DateTime.ParseExact(startDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        string sEndDate = DateTime.ParseExact(endDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        queryWhere.Append($" AND (customDate >= '{sStartDate}' AND customDate <= '{sEndDate}') ");
                    }
                    else throw new Exception();
                }

                if (!string.IsNullOrEmpty(DeliveryType.Text))
                {
                    queryWhere.Append($" AND deliveryType = '{DeliveryType.Text}' ");
                }

                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT customID, SUM(bookPrice * booksNumber) AS totalPrice, customDate " +
                        $"FROM custom JOIN custombooks USING(customID) JOIN book USING(bookID) " +
                        $" {queryWhere} GROUP BY 1 {queryHaving} {reqOrders} LIMIT {pageNumber}, 5";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            OrderTemplate(reader.GetInt16(0), reader.GetFloat(1), reader.GetDateTime(2).ToShortDateString());
                        }
                    }
                    else
                    {
                        TextBlock placeholder = new TextBlock
                        {
                            Text = "Немає результатів за вашим запитом...",
                            Style = (Style)FindResource("Hint"),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Padding = new Thickness(0, 130, 0, 0)
                        };
                        OrderList.Children.Add(placeholder);
                        if (pageNumber != 0)
                        {
                            GoToPreviousPage(BTNPreviousPage, null);
                            Methods.ShowInformation("Це остання сторінка!");
                        }
                    }
                }
            }
            catch
            {
                Methods.ShowError($"Присутні не до кінця заповненні поля.");
            }
        }

        private void ShowBooksList(object sender, RoutedEventArgs e)
        {
            BooksGeneralList booksGeneralList = new BooksGeneralList();
            booksGeneralList.Show();
        }

        private void SortOrders(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Name == "SortByDate")
            {
                ResetSortByPrice();
                if (dateToLowest)
                {
                    dateToLowest = false;
                    ImgSortByDate.Source = new BitmapImage(new Uri(imgDateToHighest, UriKind.Relative));
                    reqOrders = " ORDER BY customID DESC ";
                }
                else
                {
                    dateToLowest = true;
                    ImgSortByDate.Source = new BitmapImage(new Uri(imgDateToLowest, UriKind.Relative));
                    reqOrders = " ORDER BY customID ASC ";
                }
            }
            else if (button.Name == "SortByPrice")
            {
                ResetSortByDate();
                if (priceToLowest)
                {
                    priceToLowest = false;
                    ImgSortByPrice.Source = new BitmapImage(new Uri(imgPriceToHighest, UriKind.Relative));
                    reqOrders = " ORDER BY totalPrice DESC ";
                }
                else
                {
                    priceToLowest = true;
                    ImgSortByPrice.Source = new BitmapImage(new Uri(imgPriceToLowest, UriKind.Relative));
                    reqOrders = " ORDER BY totalPrice ASC ";
                }
            }
            ApplyAllFilters(sender, null);
        }

        private void PrintReceipt(object sender, RoutedEventArgs e)
        {
            OrderInfo.CreateReceipt();
        }

        private void GetOrderInfo(object sender, RoutedEventArgs e)
        {
            BTNCloseInfo.Visibility = Visibility.Visible;
            FiltersGrid.Visibility = Visibility.Collapsed;
            PopularityListGridBorder.Visibility = Visibility.Collapsed;
            InfoTitle.Visibility = Visibility.Visible;
            BGOrderInfo.Visibility = Visibility.Visible;
            BTNPrintReceipt.Visibility = Visibility.Visible;
            OrderInfo.BookList.Clear();
            BookList.Children.Clear();
            string[] words = { };
            Button BTNOrder = sender as Button;
            if (BTNOrder != null)
            {
                TextBlock textBlock = BTNOrder.Content as TextBlock;

                if (textBlock != null)
                {
                    words = textBlock.Text.Split('№');
                }
            }
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT customer.fullName, recipientPhoneNumber, deliveryType, deliveryAddress, " +
                        $"employee.fullName, customDate, SUM(bookPrice * booksNumber), custom.isRegular " +
                        $"FROM custom JOIN customer USING(customerID) JOIN employee USING(employeeID) " +
                        $"JOIN custombooks USING(customID) JOIN book USING(bookID) " +
                        $"WHERE customID = '{words[1]}' GROUP BY 1, 2, 3, 4";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        CustomerFullName.Text = reader.GetString(0);
                        RecipientPhoneNumber.Text = reader.GetString(1);
                        DeliveryTypeInfo.Text = reader.GetString(2);
                        DeliveryAddress.Text = reader.GetString(3);
                        EmployeeFullName.Text = reader.GetString(4);
                        RegistrationDate.Text = reader.GetDateTime(5).Date.ToShortDateString();
                        TotalPrice.Text = Math.Round(reader.GetDouble(6), 2).ToString();
                        OrderInfo.ReceiptNumber = words[1];
                        OrderInfo.CustomerFullName = CustomerFullName.Text;
                        OrderInfo.DeliveryAddress = DeliveryAddress.Text;
                        OrderInfo.EmployeeFullName = EmployeeFullName.Text;
                        OrderInfo.RegistrationDate = RegistrationDate.Text;
                        OrderInfo.TotalPrice = Math.Round(reader.GetDouble(6), 2);
                        OrderInfo.IsRegular = reader.GetBoolean(7);
                    }
                }
                //< Grid >
                //    < Grid.columndefinitions >
                //        < columndefinition />
                //        < columndefinition />
                //        < columndefinition />
                //    </ grid.columndefinitions >
                //    < textblock x: name = "bookname" horizontalalignment = "center"
                //               text = "назва" grid.column = "0" padding = "5 0" />
                //    < textblock x: name = "booknumber" horizontalalignment = "center"
                //               text = "10" grid.column = "1" />
                //    < textblock x: name = "bookprice" horizontalalignment = "center"
                //               text = "10000.10" grid.column = "2" />
                //</ Grid >
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT bookName, booksNumber, (bookPrice * booksNumber)" +
                        $"FROM custombooks JOIN book USING(bookID) " +
                        $"WHERE customID = {words[1]}";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Grid grid = new Grid();
                        ColumnDefinition col1 = new ColumnDefinition();
                        ColumnDefinition col2 = new ColumnDefinition();
                        ColumnDefinition col3 = new ColumnDefinition();

                        grid.ColumnDefinitions.Add(col1);
                        grid.ColumnDefinitions.Add(col2);
                        grid.ColumnDefinitions.Add(col3);

                        TextBlock textBlock1 = new TextBlock
                        {
                            Name = "BookName",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Margin = new Thickness(5, 0, 5, 0),
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            Text = reader.GetString(0)
                        };
                        Grid.SetColumn(textBlock1, 0);
                        grid.Children.Add(textBlock1);

                        TextBlock textBlock2 = new TextBlock
                        {
                            Name = "BookNumber",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Text = reader.GetInt32(1).ToString()
                        };
                        Grid.SetColumn(textBlock2, 1);
                        grid.Children.Add(textBlock2);

                        TextBlock textBlock3 = new TextBlock
                        {
                            Name = "BookPrice",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Text = Math.Round(reader.GetDecimal(2), 2).ToString()
                        };
                        Grid.SetColumn(textBlock3, 2);
                        grid.Children.Add(textBlock3);
                        BookList.Children.Add(grid);

                        OrderInfo.BookList.Add(new BookInfo
                        {
                            BookName = reader.GetString(0),
                            BookNumber = reader.GetInt32(1),
                            BookPrice = (float)Math.Round(reader.GetFloat(2), 2)
                        });
                    }
                }
            }
            catch
            {
                Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
            }
        }

        private void OrderTemplate(int orderID, float totalPrice, string CustomDate)
        {
            StackPanel panel = new StackPanel
            {
                Background = new SolidColorBrush(Color.FromRgb(194, 183, 154)),
                Margin = new Thickness(10, 0, 10, 8.55)
            };
            Button button = new Button
            {
                Style = (Style)FindResource("MainStyle"),
                Height = 35,
                Width = double.NaN
            };
            TextBlock title = new TextBlock
            {
                Text = $"Замовлення №{orderID}",
                Style = (Style)FindResource("BookTitle")
            };
            Border border = new Border
            {
                BorderThickness = new Thickness(1, 0, 1, 1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(73, 69, 58))
            };
            StackPanel innerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            // Parts installation
            button.Content = title;
            button.Click += GetOrderInfo;
            panel.Children.Add(button);
            innerPanel.Children.Add(new TextBlock
            {
                Style = (Style)FindResource("BookProperties"),
                Text = "Загальна вартість:"
            });
            innerPanel.Children.Add(new TextBlock
            {
                Style = (Style)FindResource("BookPropertiesData"),
                Text = Math.Round(totalPrice, 2).ToString()
            });
            innerPanel.Children.Add(new TextBlock
            {
                Style = (Style)FindResource("BookProperties"),
                Text = "Дата оформлення:"
            });
            innerPanel.Children.Add(new TextBlock
            {
                Style = (Style)FindResource("BookPropertiesData"),
                Text = CustomDate
            });
            border.Child = innerPanel;
            panel.Children.Add(border);
            OrderList.Children.Add(panel);
        }
    }
}
