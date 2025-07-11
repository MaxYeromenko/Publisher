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
    /// Interaction logic for Catalog.xaml
    /// </summary>
    public partial class Catalog : Window
    {
        int currentPage = 0;
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
        public Catalog()
        {
            InitializeComponent();
        }
        private void GoHome(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            home.Show();
            Close();
        }
        private void ClearText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Text = string.Empty;
            textBox.Foreground = (Brush)new BrushConverter().ConvertFromString("#312e26");
        }

        private void BooksANDButtonsLoad(object sender, RoutedEventArgs e)
        {
            SearchControl(Search, null);
            if (SessionData.isEmployee)
                BTNAddBook.Visibility = Visibility.Visible;
            if (!SessionData.isEmployee)
                BTNMakeOrder.Visibility = Visibility.Visible;
        }

        private void PrintBookInfo(object sender, RoutedEventArgs e)
        {
            try
            {
                Button bookInfo = sender as Button;
                BookInformation bookInformation = new BookInformation(int.Parse(bookInfo.Name.Split('_')[1]));
                bookInformation.Show();
            }
            catch
            {
                Methods.ShowError("Виникла помилка. Спробуйте пізніше.");
            }
        }

        private void SearchControl(object sender, KeyEventArgs e)
        {
            CatalogBooks.Children.Clear();
            CatalogBooks.HorizontalAlignment = HorizontalAlignment.Stretch;
            CatalogBooks.VerticalAlignment = VerticalAlignment.Stretch;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT bookImage, bookName, bookGenreName, CONCAT(SUBSTRING_INDEX(fullName, ' ', 1), ' ', " +
                        $"LEFT(SUBSTRING_INDEX(fullName, ' ', -2), 1), '.', " +
                        $"LEFT(SUBSTRING_INDEX(fullName, ' ', -1), 1), '.') as fullName, bookPrice, bookID FROM book " +
                        $"JOIN bookgenre USING(bookGenreID) JOIN bookauthor USING(bookID) " +
                        $"JOIN author USING(authorID) WHERE {ReturnFilters()} bookName LIKE '%{Search.Text}%'" +
                        $"ORDER BY bookName ASC LIMIT {pageNumber}, 5;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    {
                        //< Border Margin = "10 0 10 10"
                        //    BorderBrush = "#49453a" BorderThickness = "1" Background = "#aaa086" >
                        //    < StackPanel Orientation = "Horizontal" Margin = "5" >
                        //        < Image Height = "50" Width = "35" Source = "https://content1.rozetka.com.ua/goods/images/big/47837891.jpg" />
                        //        < StackPanel Width = "320" Margin = "15 0" VerticalAlignment = "Center" >
                        //            < Button Style = "{StaticResource MainStyle}" Width = "NaN" Height = "NaN" >
                        //                < TextBlock Style = "{StaticResource BookTitle}" >
                        //                Гаррі Поттер і в`язень Азкабану
                        //                </ TextBlock >
                        //            </ Button >
                        //            < StackPanel Orientation = "Horizontal" HorizontalAlignment = "Center" >
                        //                < StackPanel Orientation = "Horizontal" >
                        //                    < TextBlock Style = "{StaticResource BookProperties}" Text = "Жанр:" />
                        //                    < TextBlock Style = "{StaticResource BookPropertiesData}" Text = "Наукова фантастика" />
                        //                </ StackPanel >
                        //                < StackPanel Orientation = "Horizontal" >
                        //                    < TextBlock Style = "{StaticResource BookProperties}" Text = "Ціна:" />
                        //                    < TextBlock Style = "{StaticResource BookPropertiesData}" Text = "1234.56" />
                        //                </ StackPanel >
                        //            </ StackPanel >
                        //        </ StackPanel >
                        //        < Button Background = "Transparent" BorderThickness = "0"
                        //            VerticalAlignment = "Center" Width = "30" Height = "30" >
                        //            < Image Source = "/sources/images/cart.png" />
                        //        </ Button >
                        //    </ StackPanel >
                        //</ Border>
                    }
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // Wrappers
                            var outerBorder = new Border
                            {
                                Margin = new Thickness(10, 0, 10, 10),
                                BorderBrush = new SolidColorBrush(Color.FromRgb(73, 69, 58)),
                                BorderThickness = new Thickness(1),
                                Background = new SolidColorBrush(Color.FromRgb(170, 160, 134))
                            };
                            var stackPanel = new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                Margin = new Thickness(5)
                            };
                            var innerStackPanel = new StackPanel
                            {
                                Width = 320,
                                Margin = new Thickness(15, 0, 15, 0),
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            var genreAuthorPanel = new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                HorizontalAlignment = HorizontalAlignment.Center
                            };
                            // Parts installation
                            string imageUrl = reader.GetString(0);
                            Uri uriResult;
                            bool result = Uri.TryCreate(imageUrl, UriKind.Absolute, out uriResult)
                                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                            Button button = new Button
                            {
                                Name = $"Book_{reader["bookID"]}",
                                Content = new TextBlock
                                {
                                    Text = reader.GetString(1),
                                    Style = (Style)FindResource("BookTitle")
                                },
                                Style = (Style)FindResource("MainStyle"),
                                Width = double.NaN,
                                Height = double.NaN
                            };
                            button.Click += PrintBookInfo;
                            innerStackPanel.Children.Add(button);
                            genreAuthorPanel.Children.Add(new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                Children =
                                    {
                                        new TextBlock
                                        {
                                            Text = "Жанр:",
                                            Style = (Style)FindResource("BookProperties")
                                        },
                                        new TextBlock
                                        {
                                            Text = reader.GetString(2),
                                            Style = (Style)FindResource("BookPropertiesData")
                                        }
                                    }
                            });
                            //genreAuthorPanel.Children.Add(new StackPanel
                            //{
                            //    Orientation = Orientation.Horizontal,
                            //    Children =
                            //    {
                            //        new TextBlock
                            //        {
                            //            Text = "Ціна:",
                            //            Style = (Style)FindResource("BookProperties")
                            //        },
                            //        new TextBlock
                            //        {
                            //            Text = reader.GetDouble(4).ToString(),
                            //            Style = (Style)FindResource("BookPropertiesData")
                            //        }
                            //    }
                            //});
                            genreAuthorPanel.Children.Add(new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                Children =
                                    {
                                        new TextBlock
                                        {
                                            Text = "Автор:",
                                            Style = (Style)FindResource("BookProperties")
                                        },
                                        new TextBlock
                                        {
                                            Text = reader.GetString(3),
                                            Style = (Style)FindResource("BookPropertiesData")
                                        }
                                    }
                            });
                            stackPanel.Children.Add(new Image
                            {
                                Height = 50,
                                Width = 40,
                                Source = result ? new BitmapImage(new Uri(imageUrl)) :
                                new BitmapImage(new Uri("/sources/images/empty.png", UriKind.Relative))
                            });
                            innerStackPanel.Children.Add(genreAuthorPanel);
                            stackPanel.Children.Add(innerStackPanel);
                            //stackPanel.Children.Add(new Button
                            //{
                            //    Background = Brushes.Transparent,
                            //    BorderThickness = new Thickness(0),
                            //    VerticalAlignment = VerticalAlignment.Center,
                            //    Width = 30,
                            //    Height = 30,
                            //    Content = new Image
                            //    {
                            //        Source = new BitmapImage(new Uri("/sources/images/cart.png", UriKind.Relative))
                            //    }
                            //});
                            outerBorder.Child = stackPanel;
                            CatalogBooks.Children.Add(outerBorder);
                        }
                    }
                    else
                    {
                        CatalogBooks.VerticalAlignment = VerticalAlignment.Center;
                        TextBlock placeholder = new TextBlock
                        {
                            Text = "Немає результатів за вашим запитом...",
                            Style = (Style)FindResource("Hint"),
                            HorizontalAlignment = HorizontalAlignment.Center
                        };
                        CatalogBooks.Children.Add(placeholder);
                        if (pageNumber > 0)
                        {
                            GoToPreviousPage(BTNPreviousPage, null);
                            Methods.ShowInformation("Це остання сторінка!");
                        }
                    }
                }
            }
            catch
            {
                Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
            }
        }
        private void GoToNextPage(object sender, RoutedEventArgs e)
        {
            pageNumber = 5;
            PageNumber.Text = (pageNumber / 5 + 1).ToString();
            SearchControl(Search, null);
        }
        private void GoToPreviousPage(object sender, RoutedEventArgs e)
        {
            pageNumber = -5;
            PageNumber.Text = (pageNumber / 5 + 1).ToString();
            SearchControl(Search, null);
        }
        private void ClearAllFilters(object sender, RoutedEventArgs e)
        {
            MinPrice.Text = string.Empty;
            MaxPrice.Text = string.Empty;

            AuthorName.Text = string.Empty;

            SoftFormat.IsChecked = false;
            HardFormat.IsChecked = false;
            ElectronicFormat.IsChecked = false;
            AudioFormat.IsChecked = false;

            Fiction.IsChecked = false;
            Scientific.IsChecked = false;
            Historical.IsChecked = false;
            Fantastic.IsChecked = false;
            ForChildren.IsChecked = false;
            SearchControl(Search, null);
        }
        private string ReturnFilters()
        {
            string minPrice = MinPrice.Text.Replace('.', ',');
            string maxPrice = MaxPrice.Text.Replace('.', ',');
            string filters = $"fullName LIKE '%{AuthorName.Text}%' AND ";
            if (!(string.IsNullOrEmpty(minPrice) && string.IsNullOrEmpty(maxPrice)) &&
                    Methods.ValidateDoubleField(minPrice, maxPrice) &&
                    double.Parse(minPrice) <= double.Parse(maxPrice))
            {
                filters += $"bookPrice >= {MinPrice.Text.Replace(',', '.')} " +
                        $"AND bookPrice <= {MaxPrice.Text.Replace(',', '.')} AND ";
            }

            if (SoftFormat.IsChecked == true) filters += $" bookFormat = 1 AND ";
            else if (HardFormat.IsChecked == true) filters += $" bookFormat = 2 AND ";
            else if (ElectronicFormat.IsChecked == true) filters += $" bookFormat = 3 AND ";
            else if (AudioFormat.IsChecked == true) filters += $" bookFormat = 4 AND ";

            if (Fiction.IsChecked == true) filters += $" bookGenreID = 1 AND ";
            else if (Scientific.IsChecked == true) filters += $" bookGenreID = 2 AND ";
            else if (Historical.IsChecked == true) filters += $" bookGenreID = 3 AND ";
            else if (Fantastic.IsChecked == true) filters += $" bookGenreID = 4 AND ";
            else if (ForChildren.IsChecked == true) filters += $" bookGenreID = 5 AND ";

            return filters;
        }
        private void ApplyAllFilters(object sender, RoutedEventArgs e)
        {
            SearchControl(Search, null);
        }
        private void OpenBookAdd(object sender, RoutedEventArgs e)
        {
            AddBook addBook = new AddBook();
            addBook.Show();
            Close();
        }

        private void OpenMakeOrder(object sender, RoutedEventArgs e)
        {
            MakeOrder makeOrder = new MakeOrder();
            makeOrder.Show();
            Close();
        }
    }
}
