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
    /// Interaction logic for EmployeesGeneralList.xaml
    /// </summary>
    public partial class EmployeesGeneralList : Window
    {
        public EmployeesGeneralList()
        {
            InitializeComponent();
        }
        int currentRow = 0;

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
            EmployeesTable.Children.Add(border);
        }

        private void UpdateEmployeesTable(object sender, RoutedEventArgs e)
        {
            currentRow = 0;
            EmployeesTable.Children.Clear();
            EmployeesTable.RowDefinitions.Clear();
            EmployeesTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) });
            AddColumn("ПІБ", currentRow, 0);
            AddColumn("Стать", currentRow, 1);
            AddColumn("Телефон", currentRow, 2);
            AddColumn("Пошта", currentRow, 3);
            AddColumn("Посада", currentRow, 4);
            AddColumn("Досвід", currentRow, 5);
            AddColumn("ЗЗ", currentRow, 6);
            AddColumn("Найнято", currentRow, 7);
            AddColumn("Звільнено", currentRow, 8);
            AddColumn("Ставка", currentRow++, 9);

            StringBuilder queryWhere = new StringBuilder(" WHERE 1 = 2 ");

            if (WorkingEmployees.IsChecked == true) queryWhere.Append(" OR fireDate IS NULL ");

            if (DismissedEmployees.IsChecked == true) queryWhere.Append(" OR fireDate IS NOT NULL ");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT fullName, sex, phoneNumber, email, post, " +
                        $"experience, ordersCompleted, hireDate, fireDate, rate " +
                        $"FROM publisher.employee LEFT JOIN employeeinfo USING(employeeID) " +
                        $"LEFT JOIN employeepost USING(postID) " +
                        $"{queryWhere}";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        EmployeesTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) });
                        AddColumn(reader.GetString(0), currentRow, 0);
                        AddColumn(reader.GetString(1), currentRow, 1);
                        AddColumn(reader.GetString(2), currentRow, 2);
                        AddColumn(reader.GetString(3), currentRow, 3);
                        AddColumn(reader.GetString(4), currentRow, 4);
                        AddColumn(reader.GetInt32(5).ToString(), currentRow, 5);
                        AddColumn(reader.GetInt32(6).ToString(), currentRow, 6);
                        AddColumn(reader.GetDateTime(7).ToShortDateString(), currentRow, 7);
                        AddColumn(reader.IsDBNull(8) ? null : reader.GetDateTime(8).ToShortDateString(), currentRow, 8);
                        AddColumn(reader.GetFloat(9).ToString(), currentRow++, 9);
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
