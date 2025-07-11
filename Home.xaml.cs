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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Publisher
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Text = string.Empty;
            textBox.Foreground = (Brush)new BrushConverter().ConvertFromString("#312e26");
        }

        private void SearchPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Foreground = (Brush)new BrushConverter().ConvertFromString("#7a7360");
                textBox.Text = "Пошук";
            }
        }

        private void OpenBurgerMenu(object sender, RoutedEventArgs e)
        {
            if (SearchHints.Visibility == Visibility.Visible)
                SearchHints.Visibility = Visibility.Collapsed;
            if (SessionData.UserId == "" || SessionData.UserId == null)
            {
                BTNLogIn.Visibility = Visibility.Visible;
                BTNProfSettings.Visibility = Visibility.Collapsed;
                BTNCatalog.IsEnabled = false;
            }
            else
            {
                BTNProfSettings.Visibility = Visibility.Visible;
                BTNLogIn.Visibility = Visibility.Collapsed;
                BTNCatalog.IsEnabled = true;
            }
            if (SessionData.isEmployee)
            {
                BTNEmployeeDepartment.Visibility = Visibility.Visible;
                BTNSalesDepartment.Visibility = Visibility.Visible;
            }
            else
            {
                BTNEmployeeDepartment.Visibility = Visibility.Collapsed;
                BTNSalesDepartment.Visibility = Visibility.Collapsed;
            }

            BurgerMenuBG.Visibility = Visibility.Visible;
            BurgerMenu.Visibility = Visibility.Visible;

            DoubleAnimation animationBG = new DoubleAnimation(0, .4, TimeSpan.FromSeconds(1.5));
            DoubleAnimation animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5));
            BurgerMenuBG.BeginAnimation(UIElement.OpacityProperty, animationBG);
            BurgerMenu.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private async void CloseBurgerMenu(object sender, RoutedEventArgs e)
        {

            DoubleAnimation animationBG = new DoubleAnimation(.4, 0, TimeSpan.FromSeconds(1.5));
            DoubleAnimation animation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1.5));
            BurgerMenuBG.BeginAnimation(UIElement.OpacityProperty, animationBG);
            BurgerMenu.BeginAnimation(UIElement.OpacityProperty, animation);
            await Task.Delay(2000);
            BurgerMenuBG.Visibility = Visibility.Collapsed;
            BurgerMenu.Visibility = Visibility.Collapsed;
        }

        private void OpenCatalog(object sender, RoutedEventArgs e)
        {
            Catalog catalog = new Catalog();
            catalog.Show();
            Close();
        }

        private void OpenLogIn(object sender, RoutedEventArgs e)
        {
            LogIn logIn = new LogIn();
            logIn.Show();
            Close();
        }

        private void ProfileSettings(object sender, RoutedEventArgs e)
        {
            UserProfileSettings userProfileSettings = new UserProfileSettings();
            userProfileSettings.Show();
            Close();
        }

        private void SalesDepartment(object sender, RoutedEventArgs e)
        {
            SalesDepartment salesDepartment = new SalesDepartment();
            salesDepartment.Show();
            Close();
        }

        private void LDSessionData(object sender, RoutedEventArgs e)
        {
            SessionData.LoadSessionData();
        }

        private void PrintBookInfo(object sender, RoutedEventArgs e)
        {
            Button bookInfo = sender as Button;
            BookInformation bookInformation = new BookInformation(int.Parse(bookInfo.Name.Split('_')[1]));
            bookInformation.Show();
        }

        private void SearchControl(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SearchHints.Visibility = Visibility.Collapsed;
                return;
            }
            SearchHints.Children.Clear();
            if (SearchHints.Visibility != Visibility.Visible)
                SearchHints.Visibility = Visibility.Visible;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.rootConnection))
                {
                    connection.Open();
                    string query = $"SELECT bookName, bookPrice, bookGenreName, pagesNumber, bookID " +
                        $"FROM book JOIN bookgenre USING(bookGenreID) " +
                        $"WHERE bookName LIKE '%{Search.Text}%' ORDER BY bookName ASC LIMIT 5;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        {//<StackPanel Background = "#c2b79a" Margin = "10 0 10 10" >
                         //    < Button Style = "{StaticResource MainStyle}" Width = "NaN" >
                         //        < TextBlock x: Name = "BookTitle" Text = "Якась дуже довга назва" Style = "{StaticResource BookTitle}" />
                         //    </ Button >
                         //    < Border BorderThickness = "1 0 1 1" BorderBrush = "#49453a" >
                         //        < StackPanel Orientation = "Horizontal" HorizontalAlignment = "Center" >
                         //            < TextBlock Style = "{StaticResource BookProperties}" Text = "Ціна:" />
                         //            < Border BorderThickness = "0 0 1 0" BorderBrush = "#49453a" >
                         //                < TextBlock x: Name = "BookPrice" Style = "{StaticResource BookPropertiesData}" Text = "123.5" />
                         //            </ Border >
                         //            < TextBlock Style = "{StaticResource BookProperties}" Text = "Жанр:" />
                         //            < Border BorderThickness = "0 0 1 0" BorderBrush = "#49453a" >
                         //                < TextBlock x: Name = "BookGenre" Style = "{StaticResource BookPropertiesData}"
                         //                           Text = "Наукова фантастика" />
                         //            </ Border >
                         //            < TextBlock Style = "{StaticResource BookProperties}" Text = "Cторінок:" />
                         //            < TextBlock x: Name = "BookPages" Style = "{StaticResource BookPropertiesData}" Text = "354" />
                         //        </ StackPanel >
                         //    </ Border >
                         //</ StackPanel >
                         // Wrappers
                        }
                        StackPanel panel = new StackPanel
                        {
                            Background = new SolidColorBrush(Color.FromRgb(194, 183, 154)),
                            Margin = new Thickness(10, 0, 10, 10)
                        };
                        Button button = new Button
                        {
                            Name = $"Book_{reader.GetInt32(4)}",
                            Style = (Style)FindResource("MainStyle"),
                            Width = double.NaN
                        };
                        TextBlock title = new TextBlock
                        {
                            Text = reader.GetString(0),
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
                        button.Click += PrintBookInfo;
                        panel.Children.Add(button);
                        innerPanel.Children.Add(new TextBlock
                        {
                            Style = (Style)FindResource("BookProperties"),
                            Text = "Ціна:"
                        });
                        innerPanel.Children.Add(new TextBlock
                        {
                            Style = (Style)FindResource("BookPropertiesData"),
                            Text = reader.GetDouble(1).ToString()
                        });
                        innerPanel.Children.Add(new TextBlock
                        {
                            Style = (Style)FindResource("BookProperties"),
                            Text = "Жанр:"
                        });
                        innerPanel.Children.Add(new TextBlock
                        {
                            Style = (Style)FindResource("BookPropertiesData"),
                            Text = reader.GetString(2)
                        });
                        innerPanel.Children.Add(new TextBlock
                        {
                            Style = (Style)FindResource("BookProperties"),
                            Text = "Cторінок:"
                        });
                        innerPanel.Children.Add(new TextBlock
                        {
                            Style = (Style)FindResource("BookPropertiesData"),
                            Text = reader.GetInt16(3).ToString()
                        });
                        border.Child = innerPanel;
                        panel.Children.Add(border);
                        SearchHints.Children.Add(panel);
                    }
                }
            }
            catch
            {
                Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
            }
        }

        private void EmployeeDepartment(object sender, RoutedEventArgs e)
        {
            EmployeesDepartment employeesDepartment = new EmployeesDepartment();
            employeesDepartment.Show();
            Close();
        }
    }
}
