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
    /// Interaction logic for MakeOrder.xaml
    /// </summary>
    public partial class MakeOrder : Window
    {
        static int rowNumber { get; set; }
        public MakeOrder()
        {
            InitializeComponent();
            DateTime dateTime = DateTime.Now;
            RegistrationDate.Text = dateTime.ToShortDateString();
            OrderedBookList.orderedBooks.Clear();
        }
        private void GoHome(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            home.Show();
            Close();
        }
        private void ADDOrder(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    if (Methods.AddCustomValidation(CustomerPhoneNumber.Text, DeliveryAddress.Text))
                    {
                        if (OrderedBookList.orderedBooks.Count == 0)
                        {
                            Methods.ShowError("Список книжок не має бути порожнім.");
                            return;
                        }
                        string query = $"SELECT employeeID FROM employee WHERE email = '{SessionData.UserId}'";
                        MySqlCommand queryUserCommand = new MySqlCommand(query, connection);
                        var result = queryUserCommand.ExecuteScalar();
                        if (result != null)
                        {
                            string sRegistrationDate = DateTime.ParseExact(RegistrationDate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            query =
                            $"INSERT INTO custom (customerID, recipientPhoneNumber, deliveryType, " +
                            $"deliveryAddress, employeeID, customDate, isRegular) " +
                            $"VALUE({CustomerFullName.Text.Split()[0]}, '{CustomerPhoneNumber.Text}', '{DeliveryType.Text}', " +
                            $"'{DeliveryAddress.Text}', {result}, '{sRegistrationDate}', {IsRegular.IsChecked});";
                            queryUserCommand = new MySqlCommand(query, connection);
                            queryUserCommand.ExecuteNonQuery();
                            query = $"UPDATE employee SET ordersCompleted = ordersCompleted + 1 WHERE employeeID = {result};";
                            queryUserCommand = new MySqlCommand(query, connection);
                            queryUserCommand.ExecuteNonQuery();
                        }
                        query = "SELECT LAST_INSERT_ID()";
                        queryUserCommand = new MySqlCommand(query, connection);
                        result = queryUserCommand.ExecuteScalar();
                        if (result != null)
                        {
                            foreach (var book in OrderedBookList.orderedBooks)
                            {
                                query =
                                $"INSERT INTO customBooks (bookID, customID, booksNumber) " +
                                $"VALUES('{book.BookID}', '{result}', '{book.BookNumber}'); ";
                                queryUserCommand = new MySqlCommand(query, connection);
                                queryUserCommand.ExecuteNonQuery();
                            }
                        }
                        foreach (var book in OrderedBookList.orderedBooks)
                        {
                            query = $"UPDATE book SET bookNumber = bookNumber - {book.BookNumber} " +
                                $"WHERE bookID = {book.BookID};";
                            queryUserCommand = new MySqlCommand(query, connection);
                            queryUserCommand.ExecuteNonQuery();
                        }
                        Methods.ShowInformation("Замовлення додано успішно!");
                        GoHome(null, null);
                        Close();
                    }
                }
            }
            catch
            {
                Methods.ShowError($"Перевірте правильність заповнення полів.");
            }
        }
        private void ADDCustomer(object sender, RoutedEventArgs e)
        {
            AddCustomer addCustomer = new AddCustomer();
            addCustomer.Show();
        }

        private void BookInfoTemplate(string bookName, int bookNumber, float bookPrice)
        {
            Border border = new Border
            {
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#49453a"))
            };

            Grid grid = new Grid
            {
                Margin = new Thickness(0, 10, 0, 10)
            };

            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.3, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.6, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.4, GridUnitType.Star) });

            TextBlock textBlock1 = new TextBlock
            {
                Text = $"{++rowNumber}",
                TextTrimming = TextTrimming.CharacterEllipsis,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            TextBlock textBlock2 = new TextBlock
            {
                Text = bookName,
                TextTrimming = TextTrimming.CharacterEllipsis,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                MaxHeight = 50,
            };

            TextBlock textBlock3 = new TextBlock
            {
                Text = bookNumber.ToString(),
                Padding = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock textBlock4 = new TextBlock
            {
                Text = bookPrice.ToString(),
                TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock textBlock5 = new TextBlock
            {
                Text = (bookPrice * bookNumber).ToString(),
                TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            //Button button1 = new Button
            //{
            //    Background = Brushes.Transparent,
            //    BorderThickness = new Thickness(0),
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Name = $"Edit_{rowNumber}",
            //    Content = new Image
            //    {
            //        Source = new BitmapImage(new Uri("/sources/images/editBookIcon.png", UriKind.Relative)),
            //        Width = 30
            //    }
            //};
            //button1.IsEnabled = false;
            //button1.SetValue(Grid.ColumnProperty, 2);
            //button1.SetValue(Grid.RowProperty, 1);
            //button1.Click += EditBookInList;

            Button button2 = new Button
            {
                Margin = new Thickness(10, 0, 10, 0),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Name = $"Delete_{rowNumber}",
                Content =
                new Image
                {
                    Source = new BitmapImage(new Uri("/sources/images/deleteBookIcon.png", UriKind.Relative)),
                    Width = 30
                }
            };
            button2.SetValue(Grid.ColumnProperty, 5);
            button2.Click += DeleteBookFromList;

            //grid.Children.Add(button1);
            grid.Children.Add(button2);

            grid.Children.Add(textBlock1);
            Grid.SetColumn(textBlock1, 0);

            grid.Children.Add(textBlock2);
            Grid.SetColumn(textBlock2, 1);

            grid.Children.Add(textBlock3);
            Grid.SetColumn(textBlock3, 2);

            grid.Children.Add(textBlock4);
            Grid.SetColumn(textBlock4, 3);

            grid.Children.Add(textBlock5);
            Grid.SetColumn(textBlock5, 4);

            border.Child = grid;

            BookList.Children.Add(border);
        }

        private void AddBookToList(object sender, RoutedEventArgs e)
        {
            OrderBook OrderBook = new OrderBook();
            OrderBook.ShowDialog();
            {
                //< Border BorderThickness = "1 0 1 1" BorderBrush = "#49453a" >
                //    < Grid Margin = "0 10" >
                //        < Grid.RowDefinitions >
                //            < RowDefinition />
                //            < RowDefinition />
                //        </ Grid.RowDefinitions >
                //        < Grid.ColumnDefinitions >
                //            < ColumnDefinition Width = ".3*" />
                //            < ColumnDefinition />
                //            < ColumnDefinition />
                //            < ColumnDefinition />
                //            < ColumnDefinition />
                //        </ Grid.ColumnDefinitions >
                //        < TextBlock Grid.Column = "0" Text = "1" TextTrimming = "CharacterEllipsis"
                //            VerticalAlignment = "Center" HorizontalAlignment = "Center" />
                //        < TextBlock Grid.Column = "1" Text = "Дуже довга назва книги" TextTrimming = "WordEllipsis"
                //            VerticalAlignment = "Center" />
                //        < TextBlock Grid.Column = "2" Text = "0" Padding = "0"
                //                    HorizontalAlignment = "Center" VerticalAlignment = "Center" />
                //        < TextBlock Grid.Column = "3" Text = "10000000.99" TextTrimming = "CharacterEllipsis"
                //            HorizontalAlignment = "Center" VerticalAlignment = "Center" />
                //        < TextBlock Grid.Column = "4" Text = "10000000.99" TextTrimming = "CharacterEllipsis"
                //            HorizontalAlignment = "Center" VerticalAlignment = "Center" />
                //        < Button Grid.Column = "2" Grid.Row = "1" Background = "Transparent" BorderThickness = "0"
                //                HorizontalAlignment = "Center" VerticalAlignment = "Center" >
                //            < Image Source = "/sources/images/editBookIcon.png" Width = "30" />
                //        </ Button >
                //        < Button Grid.Column = "3" Grid.Row = "1" Background = "Transparent" BorderThickness = "0"
                //                HorizontalAlignment = "Center" VerticalAlignment = "Center" >
                //            < Image Source = "/sources/images/deleteBookIcon.png" Width = "30" />
                //        </ Button >
                //    </ Grid >
                //</ Border >
            }
            UpdateBookList();
        }

        private void EditBookInList(object sender, RoutedEventArgs e)
        {
            UpdateBookList();
        }

        private void DeleteBookFromList(object sender, RoutedEventArgs e)
        {
            Button rowIndex = sender as Button;
            OrderedBookList.orderedBooks.RemoveAt(int.Parse(rowIndex.Name.Split('_')[1]) - 1);
            UpdateBookList();
        }

        private void UpdateBookList()
        {
            BookList.Children.Clear();
            rowNumber = 0;
            int totalBooksNumber = 0;
            float totalBooksCost = 0;

            foreach (var book in OrderedBookList.orderedBooks)
            {
                BookInfoTemplate(book.BookName, book.BookNumber, book.BookPrice);
                totalBooksNumber += book.BookNumber;
                totalBooksCost += book.BookNumber * book.BookPrice;
            }
            AllBookNumber.Text = totalBooksNumber.ToString();
            AllBookPrice.Text = totalBooksCost.ToString();
            DiscountUse(totalBooksCost);
        }

        private void DiscountUse(float value)
        {
            try
            {
                if (value > 0 && (bool)IsRegular.IsChecked)
                {
                    AllBookPrice.TextDecorations.Add(TextDecorations.Strikethrough);
                    DiscountImage.Visibility = Visibility.Visible;
                    AllBookPriceWithDiscount.Text = Math.Round(value - value * 0.03, 2).ToString();
                    AllBookPriceWithDiscount.Visibility = Visibility.Visible;
                }
                else
                {
                    AllBookPrice.TextDecorations.Clear();
                    DiscountImage.Visibility = Visibility.Collapsed;
                    AllBookPriceWithDiscount.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
        }

        private void IsRegularChecked(object sender, RoutedEventArgs e)
        {
            DiscountUse(float.Parse(AllBookPrice.Text));
        }

        private void UpdateCustomer(object sender, EventArgs e)
        {
            CustomerFullName.Items.Clear();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT customerID, CONCAT(SUBSTRING_INDEX(fullName, ' ', 1), ' ', " +
                        $"LEFT(SUBSTRING_INDEX(fullName, ' ', -2), 1), '.', " +
                        $"LEFT(SUBSTRING_INDEX(fullName, ' ', -1), 1), '.') FROM customer " +
                        $"ORDER BY fullName ASC;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        CustomerFullName.Items.Add($"{reader.GetInt32(0)} {reader.GetString(1)}");
                    }
                }
            }
            catch
            {
                Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
            }
        }

        private void UpdateCustomerInfo(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = (string)((ComboBox)sender).SelectedItem;
            if (selectedItem != null)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                    {
                        connection.Open();
                        string query = $"UPDATE customer JOIN (SELECT customerID, COUNT(*) as cnt " +
                            $"FROM custom WHERE customerID = {selectedItem.Split()[0]} GROUP BY customerID) " +
                            $"AS customCount ON customer.customerID = customCount.customerID " +
                            $"SET customer.isRegular = 1 WHERE customCount.cnt > 2;";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        query = $"SELECT phoneNumber, address, isRegular " +
                           $"FROM customer WHERE customerID = {selectedItem.Split()[0]};";
                        command = new MySqlCommand(query, connection);
                        MySqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            CustomerPhoneNumber.Text = reader.GetString(0);
                            DeliveryAddress.Text = reader.GetString(1);
                            IsRegular.IsChecked = reader.GetInt32(2) == 1;
                        }
                    }
                }
                catch
                {
                    Methods.ShowError($"Не вдалося автоматично підставити інформацію про клієнта. Введіть, будь ласка, власноруч.");
                }
            }
        }
    }
}
