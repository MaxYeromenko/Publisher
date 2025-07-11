using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Mysqlx.Datatypes;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for EmployeesDepartment.xaml
    /// </summary>
    public partial class EmployeesDepartment : Window
    {
        int currentPage = 0;

        int currentRow = 0;

        string currentEmployee = "";

        StringBuilder queryWhere = new StringBuilder(" WHERE 1=1 ");

        string birthDate, hireDate, fireDate;

        bool sexToLowest = true;

        bool postToLowest = true;

        string reqEmployees = "";

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

        ComboBox vacantPostName = new ComboBox
        {
            Margin = new Thickness(10, 0, 10, 0),
        };

        TextBox vacantPostRequiredAmount = new TextBox
        {
            Height = 30,
            BorderThickness = new Thickness(),
            Margin = new Thickness(10, 0, 10, 0),
            Text = "1",
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };

        public EmployeesDepartment()
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

        private void ResetSortBySex()
        {
            sexToLowest = true;
        }

        private void ResetSortByPost()
        {
            postToLowest = true;
        }

        private void SortEmployees(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Name == "SortBySex")
            {
                ResetSortByPost();
                if (sexToLowest)
                {
                    sexToLowest = false;
                    reqEmployees = " ORDER BY sex DESC ";
                }
                else
                {
                    sexToLowest = true;
                    reqEmployees = " ORDER BY sex ASC ";
                }
            }
            else if (button.Name == "SortByPost")
            {
                ResetSortBySex();
                if (postToLowest)
                {
                    postToLowest = false;
                    reqEmployees = " ORDER BY post DESC ";
                }
                else
                {
                    postToLowest = true;
                    reqEmployees = " ORDER BY post ASC ";
                }
            }
            ApplyAllFilters(sender, null);
        }

        private void CallGeneralBG()
        {
            GeneralBackground.Visibility = Visibility.Visible;
            BTNCloseInfo.Visibility = Visibility.Visible;
        }

        private void OpenFilters(object sender, RoutedEventArgs e)
        {
            CallGeneralBG();
            FiltersGrid.Visibility = Visibility.Visible;
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
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);
            VacantPostsResult.Children.Add(border);
        }

        private void CheckVacantPosts(object sender, RoutedEventArgs e)
        {
            CallGeneralBG();
            currentRow = 0;
            VacantPostsResult.Visibility = Visibility.Visible;
            BTNSavePostList.Visibility = Visibility.Visible;
            BTNEditPostList.Visibility = Visibility.Visible;
            VacantPostsResult.Children.Clear();
            VacantPostsResult.RowDefinitions.Clear();
            VacantPostsResult.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) });
            AddColumn("Назва вакансії", currentRow, 0);
            AddColumn("Потрібна кількість", currentRow++, 1);
            using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
            {
                connection.Open();
                string query = $"SELECT post, reqAmount FROM employeepost WHERE reqAmount;";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    VacantPostsResult.RowDefinitions.Add(new RowDefinition
                    {
                        Height = new GridLength(30, GridUnitType.Pixel)
                    });
                    AddColumn(reader.GetString(0), currentRow, 0);
                    AddColumn(reader.GetInt32(1).ToString(), currentRow++, 1);
                }
            }
        }

        private void UpdateEmployeePostList(object sender, EventArgs e)
        {
            EmployeePostList.Items.Clear();
            EditEmployeePostList.Items.Clear();
            ComboBox comboBox = (ComboBox)sender;
            using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
            {
                connection.Open();
                string query = $"SELECT postID, post FROM employeepost WHERE postID != 6;";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (comboBox.Name == "EmployeePostList")
                        EmployeePostList.Items.Add($"{reader.GetInt32(0)} {reader.GetString(1)}");
                    else if (comboBox.Name == "EditEmployeePostList")
                        EditEmployeePostList.Items.Add($"{reader.GetInt32(0)} {reader.GetString(1)}");
                }
            }
        }

        private void EmployeeTemplate(int employeeID, string fullName, string sex, string post)
        {
            StackPanel panel = new StackPanel
            {
                Background = new SolidColorBrush(Color.FromRgb(194, 183, 154)),
                Margin = new Thickness(10, 0, 10, 8.55)
            };
            Button button = new Button
            {
                Style = (Style)FindResource("MainStyle"),
                Width = double.NaN,
                Height = 35,
                Name = $"E{employeeID}"
            };
            TextBlock title = new TextBlock
            {
                Text = fullName,
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

            button.Content = title;
            button.Click += GetEmployeeInfo;
            panel.Children.Add(button);
            innerPanel.Children.Add(new TextBlock
            {
                Style = (Style)FindResource("BookProperties"),
                Text = "Стать:"
            });
            innerPanel.Children.Add(new TextBlock
            {
                Style = (Style)FindResource("BookPropertiesData"),
                Text = sex
            });
            innerPanel.Children.Add(new TextBlock
            {
                Style = (Style)FindResource("BookProperties"),
                Text = "Посада:"
            });
            innerPanel.Children.Add(new TextBlock
            {
                Style = (Style)FindResource("BookPropertiesData"),
                Text = post
            });
            border.Child = innerPanel;
            panel.Children.Add(border);
            EmployeeList.Children.Add(panel);
        }

        private void ClearAllFilters(object sender, RoutedEventArgs e)
        {
            queryWhere.Clear();
            queryWhere.Append(" WHERE 1=1 ");
            MinExperience.Text = string.Empty;
            MaxExperience.Text = string.Empty;
            StartDate.Text = string.Empty;
            EndDate.Text = string.Empty;
            EmploymentStatus.SelectedItem = null;
            EmployeePostList.SelectedItem = null;
            FamilyStatusList.SelectedItem = null;
            ChildrenNumberFilter.Text = string.Empty;
            MinCompletedNumber.Text = string.Empty;
            MaxCompletedNumber.Text = string.Empty;
            ApplyAllFilters(null, null);
        }

        private void ApplyAllFilters(object sender, RoutedEventArgs e)
        {
            try
            {
                EmployeeList.Children.Clear();
                string minExperience = MinExperience.Text;
                string maxExperience = MaxExperience.Text;
                string minCompletedNumber = MinCompletedNumber.Text;
                string maxCompletedNumber = MaxCompletedNumber.Text;
                string startDate = StartDate.Text;
                string endDate = EndDate.Text;
                string postList = EmployeePostList.Text;
                string statusList = FamilyStatusList.Text;
                string childrenNumber = ChildrenNumberFilter.Text;

                queryWhere.Clear();
                queryWhere.Append(" WHERE 1=1 ");

                if (!(string.IsNullOrEmpty(minExperience) && string.IsNullOrEmpty(maxExperience)) &&
                    Methods.ValidateIntField(minExperience, maxExperience) &&
                    int.Parse(minExperience) <= int.Parse(maxExperience))
                {
                    queryWhere.Append($" AND experience >= {minExperience} AND experience <= {maxExperience} ");
                }

                if (Methods.ValidateDateField(startDate, endDate))
                {
                    if (DateTime.Parse(startDate) <= DateTime.Parse(endDate) && DateTime.Parse(startDate) <= DateTime.Now
                        && DateTime.Parse(endDate) <= DateTime.Now)
                    {
                        string sStartDate = DateTime.ParseExact(startDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        string sEndDate = DateTime.ParseExact(endDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        queryWhere.Append($" AND hireDate >= '{sStartDate}' AND hireDate <= '{sEndDate}' ");
                    }
                    else throw new Exception();
                }

                if (!string.IsNullOrEmpty(postList))
                {
                    queryWhere.Append($" AND postID = '{postList.Split()[0]}' ");
                }

                if (!string.IsNullOrEmpty(EmploymentStatus.Text))
                {
                    if (EmploymentStatus.SelectedIndex == 0)
                        queryWhere.Append($" AND postID != 6 ");
                    else if (EmploymentStatus.SelectedIndex == 1)
                        queryWhere.Append($" AND postID = 6 ");
                }

                if (!string.IsNullOrEmpty(statusList))
                {
                    queryWhere.Append($" AND familyStatus = '{statusList}' ");
                }

                if (!string.IsNullOrEmpty(childrenNumber) && Methods.ValidateIntField(childrenNumber))
                {
                    queryWhere.Append($" AND childrenNumber = {childrenNumber} ");
                }

                if (!(string.IsNullOrEmpty(minCompletedNumber) && string.IsNullOrEmpty(maxCompletedNumber)) &&
                    Methods.ValidateIntField(minCompletedNumber, maxCompletedNumber) &&
                    int.Parse(minCompletedNumber) <= int.Parse(maxCompletedNumber))
                {
                    queryWhere.Append($" AND ordersCompleted >= {minCompletedNumber} AND ordersCompleted <= {maxCompletedNumber} ");
                }

                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT employeeID, CONCAT(SUBSTRING_INDEX(fullName, ' ', 1), ' ',  " +
                        $"LEFT(SUBSTRING_INDEX(fullName, ' ', -2), 1), '.', " +
                        $"LEFT(SUBSTRING_INDEX(fullName, ' ', -1), 1), '.') as fullName, sex, post " +
                        $"FROM employee JOIN employeepost USING(PostID) " +
                        $"JOIN employeeinfo USING(employeeID) " +
                        $" {queryWhere} GROUP BY employeeID {reqEmployees} LIMIT {pageNumber}, 5;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EmployeeTemplate(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
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
                        EmployeeList.Children.Add(placeholder);
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
                Methods.ShowError($"Присутні неправильно або не до кінця заповненні поля.");
            }
            UpdateResultsCount();
        }

        private void UpdateResultsCount()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT COUNT(*) FROM employee JOIN employeepost USING(PostID) " +
                        $"JOIN employeeinfo USING(employeeID) {queryWhere};";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    int result = Convert.ToInt32(command.ExecuteScalar());
                    ResultsNumber.Text = $"Результатів: {result}";
                }
            }
            catch
            {
                Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
            }
        }

        private void UpdateFamilyStatusList(object sender, EventArgs e)
        {
            FamilyStatusList.SelectedItem = null;
        }

        private void ADDEmployee(object sender, RoutedEventArgs e)
        {
            AddEmployee addEmployee = new AddEmployee();
            addEmployee.ShowDialog();
            ApplyAllFilters(null, null);
        }

        private void REMOVEEmployee(object sender, RoutedEventArgs e)
        {
            RemoveEmployee removeEmployee = new RemoveEmployee();
            removeEmployee.ShowDialog();
            ApplyAllFilters(null, null);
        }

        private void ShowEmployeesList(object sender, RoutedEventArgs e)
        {
            EmployeesGeneralList employeesGeneralList = new EmployeesGeneralList();
            employeesGeneralList.Show();
        }

        private void UpdateEmploymentStatus(object sender, EventArgs e)
        {
            EmploymentStatus.SelectedItem = null;
        }

        private void EditEmployeeInfo(object sender, RoutedEventArgs e)
        {
            BTNEditEmployeeInfo.Visibility = Visibility.Collapsed;

            EmployeeFullName.IsReadOnly = false;

            EmployeeSex.Visibility = Visibility.Collapsed;
            EditEmployeeSex.Visibility = Visibility.Visible;
            EditEmployeeSex.Text = EmployeeSex.Text;

            BirthDate.IsReadOnly = false;

            FamilyStatus.Visibility = Visibility.Collapsed;
            EditEmployeeFamilyStatus.Visibility = Visibility.Visible;
            EditEmployeeFamilyStatus.Text = FamilyStatus.Text;

            ChildrenNumber.IsReadOnly = false;
            EmployeePhoneNumber.IsReadOnly = false;
            EmployeePassport.IsReadOnly = false;
            Experience.IsReadOnly = false;

            EmployeePost.Visibility = Visibility.Collapsed;
            EditEmployeePostList.Visibility = Visibility.Visible;

            HireDate.IsReadOnly = false;
            FireDate.IsReadOnly = false;
            OrdersCompleted.IsReadOnly = false;

            hireDate = HireDate.Text;
            fireDate = FireDate.Text;
        }

        private void EditPostList(object sender, RoutedEventArgs e)
        {
            BTNEditPostList.Visibility = Visibility.Collapsed;
            AddVacantPost();
        }

        private void SavePostList(object sender, RoutedEventArgs e)
        {
            try
            {
                if (vacantPostName.SelectedItem != null && !string.IsNullOrEmpty(vacantPostRequiredAmount.Text))
                {
                    using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                    {
                        connection.Open();

                        string query = $"SELECT reqAmount FROM employeepost WHERE post = '{vacantPostName.Text}';";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        int result = (int)command.ExecuteScalar();
                        if (result + int.Parse(vacantPostRequiredAmount.Text) >= 0)
                        {
                            query = $"UPDATE employeepost SET reqAmount = reqAmount + {vacantPostRequiredAmount.Text} " +
                                $"WHERE post = '{vacantPostName.Text}';";

                            command = new MySqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            CheckVacantPosts(null, null);
                        }
                        else Methods.ShowWarning("Кількість необхідних працівників має бути більшою або дорівнювати 0.");

                    }
                }
                else CheckVacantPosts(null, null);
            }
            catch
            {
                Methods.ShowError($"Перевірте заповнені поля або спробуйте пізніше.");
            }
        }

        private void AddVacantPost()
        {
            vacantPostName.Items.Clear();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT post FROM employeepost WHERE postID != 6;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read()) vacantPostName.Items.Add(reader.GetString(0));
                    VacantPostsResult.RowDefinitions.Add(new RowDefinition
                    {
                        Height = new GridLength(30, GridUnitType.Pixel),
                    });

                    Grid.SetRow(vacantPostName, currentRow);
                    Grid.SetColumn(vacantPostName, 0);
                    VacantPostsResult.Children.Add(vacantPostName);

                    Grid.SetRow(vacantPostRequiredAmount, currentRow++);
                    Grid.SetColumn(vacantPostRequiredAmount, 1);
                    VacantPostsResult.Children.Add(vacantPostRequiredAmount);
                }
            }
            catch
            {
                Methods.ShowError("Перевірте заповнені поля або спробуйте пізніше.");
            }
        }

        private void ExitEditing()
        {
            EmployeeFullName.IsReadOnly = true;
            EmployeeSex.Visibility = Visibility.Visible;
            EditEmployeeSex.Visibility = Visibility.Collapsed;
            BirthDate.IsReadOnly = true;
            FamilyStatus.Visibility = Visibility.Visible;
            EditEmployeeFamilyStatus.Visibility = Visibility.Collapsed;
            ChildrenNumber.IsReadOnly = true;
            EmployeePhoneNumber.IsReadOnly = true;
            EmployeePassport.IsReadOnly = true;
            Experience.IsReadOnly = true;
            EmployeePost.Visibility = Visibility.Visible;
            EditEmployeePostList.Visibility = Visibility.Collapsed;
            OrdersCompleted.IsReadOnly = true;

            HireDate.IsReadOnly = true;
            FireDate.IsReadOnly = true;
        }

        private void SaveEmployeeInfo(object sender, RoutedEventArgs e)
        {
            string employeeFullName = EmployeeFullName.Text.Trim();
            string employeeSex = EditEmployeeSex.Text.Trim();
            string employeeBirthDate = BirthDate.Text.Trim();
            string employeeFamilyStatus = EditEmployeeFamilyStatus.Text.Trim();
            string employeeChildrenNumber = ChildrenNumber.Text.Trim();
            string employeePhoneNumber = EmployeePhoneNumber.Text.Trim();
            string employeePassport = EmployeePassport.Text.Trim();
            string employeeExperience = Experience.Text.Trim();
            string employeePostID = EditEmployeePostList.Text.Trim().Split()[0];
            string employeeOrdersCompleted = OrdersCompleted.Text.Trim();

            if (Methods.AddEmployeeValidation(employeeFullName, employeeSex, employeePhoneNumber,
                employeePassport, employeePostID, employeeFamilyStatus, employeeChildrenNumber,
                employeeExperience, employeeOrdersCompleted))
            {
                if ((FireDate.Text == "-" && Methods.ValidateDateField(HireDate.Text)) ||
                    (Methods.ValidateDateField(HireDate.Text, FireDate.Text) &&
                    DateTime.Parse(HireDate.Text) <= DateTime.Parse(FireDate.Text)))
                {
                    hireDate = HireDate.Text;
                    fireDate = FireDate.Text;
                }

                if (Methods.ValidateDateField(BirthDate.Text))
                {
                    birthDate = BirthDate.Text;
                }

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                    {
                        connection.Open();

                        string sEmployeeHireDate = DateTime.ParseExact(hireDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        string sEmployeeFireDate = fireDate == "-" ? null : DateTime.ParseExact(fireDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        string sEmployeeBirthDate = DateTime.ParseExact(birthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                        string query = $"UPDATE employee SET fullName = '{employeeFullName}', " +
                            $"sex = '{employeeSex}', phoneNumber = '{employeePhoneNumber}', " +
                            $"passport = '{employeePassport}', postID = {employeePostID}, " +
                            $"birthDate = '{sEmployeeBirthDate}', familyStatus = '{employeeFamilyStatus}', " +
                            $"childrenNumber = {employeeChildrenNumber}, experience = {employeeExperience}, " +
                            $"ordersCompleted = {employeeOrdersCompleted} " +
                            $"WHERE employeeID = {currentEmployee.Substring(1)};";

                        MySqlCommand queryCommand = new MySqlCommand(query, connection);
                        queryCommand.ExecuteNonQuery();
                        if (string.IsNullOrEmpty(sEmployeeFireDate))
                            query = $"UPDATE employeeinfo SET hireDate = '{sEmployeeHireDate}', fireDate = NULL " +
                                $"WHERE employeeID = {currentEmployee.Substring(1)};";
                        else
                            query = $"UPDATE employeeinfo SET hireDate = '{sEmployeeHireDate}', fireDate = '{sEmployeeFireDate}' " +
                            $"WHERE employeeID = {currentEmployee.Substring(1)};";

                        queryCommand = new MySqlCommand(query, connection);
                        queryCommand.ExecuteNonQuery();
                    }
                    Methods.ShowInformation("Дані успішно збережено!");
                    BTNEditEmployeeInfo.Visibility = Visibility.Visible;
                    ApplyAllFilters(null, null);
                    CloseInfo(null, null);
                }
                catch
                {
                    Methods.ShowError($"Перевірте правильність заповнення полів!");
                }
            }
            else Methods.ShowError($"Перевірте правильність заповнення полів!");
        }

        private void CloseInfo(object sender, RoutedEventArgs e)
        {
            BTNCloseInfo.Visibility = Visibility.Collapsed;
            TitleEmployeeInfo.Visibility = Visibility.Collapsed;
            BGEmployeeInfo.Visibility = Visibility.Collapsed;
            GeneralBackground.Visibility = Visibility.Collapsed;
            VacantPostsResult.Visibility = Visibility.Collapsed;
            FiltersGrid.Visibility = Visibility.Collapsed;
            BTNSavePostList.Visibility = Visibility.Collapsed;
            BTNEditPostList.Visibility = Visibility.Collapsed;
        }

        private void GetEmployeeInfo(object sender, RoutedEventArgs e)
        {
            ExitEditing();
            CloseInfo(null, null);
            CallGeneralBG();
            BGEmployeeInfo.Visibility = Visibility.Visible;
            TitleEmployeeInfo.Visibility = Visibility.Visible;
            BTNEditEmployeeInfo.Visibility = Visibility.Visible;
            Button BTNEmployee = sender as Button;
            currentEmployee = BTNEmployee.Name;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(SessionData.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT fullName, sex, phoneNumber, email, passport, birthDate, familyStatus, " +
                        $"childrenNumber, experience, ordersCompleted, post, hireDate, fireDate, AVG(salary) as averageSalary " +
                        $"FROM employee LEFT JOIN employeeinfo USING(employeeID) LEFT JOIN employeepost USING(postID) " +
                        $"LEFT JOIN employeesalary USING(employeeInfoID) " +
                        $"WHERE employeeID = {BTNEmployee.Name.Substring(1)} GROUP BY 12, 13;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        EmployeeFullName.Text = reader["fullname"].ToString();
                        EmployeeSex.Text = reader["sex"].ToString();
                        BirthDate.Text = ((DateTime)reader["birthDate"]).ToShortDateString();
                        FamilyStatus.Text = reader["familyStatus"].ToString();
                        ChildrenNumber.Text = reader["childrenNumber"].ToString();
                        EmployeePhoneNumber.Text = reader["phoneNumber"].ToString();
                        EmployeeEmail.Text = reader["email"].ToString();
                        EmployeePassport.Text = reader["passport"].ToString();
                        Experience.Text = reader["experience"].ToString();
                        EmployeePost.Text = reader["post"].ToString();
                        HireDate.Text = ((DateTime)reader["hireDate"]).ToShortDateString();
                        FireDate.Text = reader.IsDBNull(reader.GetOrdinal("fireDate")) ?
                            "-" : ((DateTime)reader["fireDate"]).ToShortDateString();
                        OrdersCompleted.Text = reader["ordersCompleted"].ToString();
                        AverageEmployeeSalary.Text = reader.IsDBNull(reader.GetOrdinal("averageSalary")) ?
                            "-" : Math.Round((double)reader["averageSalary"], 2).ToString();
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
