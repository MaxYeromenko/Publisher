using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Publisher
{
    public static class Methods
    {
        public static bool Validation(string password, string confirmPassword, string fullName, string phoneNumber, string email,
            string country, string index, string city, string street, string houseNumber, bool? male, bool? female)
        {
            const string emailPattern = @"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@(([^<>()[\]\\.,;:\s@\""]+\.)+[^<>()[\]\\.,;:\s@\""]{2,})$";
            // Exception Dictionary
            Dictionary<Func<bool>, string> rules = new Dictionary<Func<bool>, string>
            {
                { () => string.IsNullOrEmpty(fullName), "ПІБ не може бути порожнім." },
                { () => fullName.Length < 3, "Довжина ПІБ менша за 3 символи." },
                { () => password.Length == 0, "Пароль не може бути порожнім." },
                { () => password.Length < 8, "Довжина паролю менша за 8 символів." },
                { () => password.Count(char.IsLetter) < 3, "Пароль має містити щонайменше 3 літери." },
                { () => password.Count(char.IsDigit) < 1, "Пароль має містити хоча б одну цифру." },
                { () => password.Contains(' '), "Пароль не має містити прогалин." },
                { () => password != confirmPassword, "Паролі не співпадають." },
                { () => string.IsNullOrEmpty(phoneNumber), "Номер телефону не може бути порожнім." },
                { () => phoneNumber[0] != '0', $"Номер телефону має починатися з 0ХХ." },
                { () => phoneNumber.Count(char.IsLetter) != 0, "Номер телефону не має містити літер." },
                { () => phoneNumber.Length != 10, "Номер телефону має містити 10 цифр." },
                { () => string.IsNullOrEmpty(email), "Електрона пошта не може бути порожньою." },
                { () => !Regex.IsMatch(email, emailPattern), "Електрона пошта повинна бути записана у форматі: example@ex.ex" },
                { () => male == false && female == false, "Введіть стать 'чоловік' або 'жінка'." },
                { () => country == "Країна", "Поле Країна не може бути порожнім." },
                { () => index == "Індекс", "Поле Індекс не може бути порожнім." },
                { () => index.Count(char.IsLetter) != 0, "Поле Індекс має містити лише цифри." },
                { () => index.Length != 5, "Поле Індекс має містити 5 цифр." },
                { () => city == "Місто", "Поле Місто не може бути порожнім." },
                { () => street == "Вулиця", "Поле Вулиця не може бути порожнім." },
                { () => houseNumber == "№ Буд.", "Поле Будинок не може бути порожнім." }
            };
            foreach (var rule in rules)
            {
                if (rule.Key.Invoke())
                {
                    MessageBox.Show(rule.Value);
                    return false;
                }
            }
            return true;
        }
        public static MessageBoxResult ShowConfirmation(string message)
        {
            return MessageBox.Show(message, "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
        public static MessageBoxResult ShowInformation(string message)
        {
            return MessageBox.Show(message, "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public static MessageBoxResult ShowWarning(string message)
        {
            return MessageBox.Show(message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public static MessageBoxResult ShowError(string message)
        {
            return MessageBox.Show(message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static bool AddUserValidation(string fullName, string sex, string address,
            string phoneNumber, string email)
        {
            string[] parts = { };
            try
            {
                parts = address.Split(new string[] { ", " }, StringSplitOptions.None);
                if (parts.Length != 5)
                    throw new Exception();
            }
            catch
            {
                ShowWarning("Заповніть поле адреса правильно! У форматі 'Країна, індекс, місто, вулиця, № будинку'.");
                return false;
            }
            const string emailPattern = @"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@(([^<>()[\]\\.,;:\s@\""]+\.)+[^<>()[\]\\.,;:\s@\""]{2,})$";
            // Exception Dictionary
            Dictionary<Func<bool>, string> rules = new Dictionary<Func<bool>, string>
            {
                { () => string.IsNullOrEmpty(fullName), "ПІБ не може бути порожнім." },
                { () => fullName.Length < 3, "Довжина ПІБ менша за 3 символи." },
                { () => fullName.Count(char.IsDigit) != 0, "ПІБ має містити лише літери." },
                { () => string.IsNullOrEmpty(phoneNumber), "Номер телефону не може бути порожнім." },
                { () => phoneNumber[0] != '0', $"Номер телефону має починатися з 0ХХ." },
                { () => phoneNumber.Count(char.IsLetter) != 0, "Номер телефону не має містити літер." },
                { () => phoneNumber.Length != 10, "Номер телефону має містити 10 цифр." },
                { () => string.IsNullOrEmpty(email), "Електрона пошта не може бути порожньою." },
                { () => !Regex.IsMatch(email, emailPattern), "Електрона пошта повинна бути записана у форматі: example@ex.ex" },
                { () => string.IsNullOrEmpty(sex), "Оберіть стать." },
                { () => string.IsNullOrEmpty(parts[0]), "Поле Країна не може бути порожнім." },
                { () => string.IsNullOrEmpty(parts[1]), "Поле Індекс не може бути порожнім." },
                { () => parts[1].Count(char.IsLetter) != 0, "Поле Індекс має містити лише цифри." },
                { () => parts[1].Length != 5, "Поле Індекс має містити 5 цифр." },
                { () => string.IsNullOrEmpty(parts[2]), "Поле Місто не може бути порожнім." },
                { () => string.IsNullOrEmpty(parts[3]), "Поле Вулиця не може бути порожнім." },
                { () => string.IsNullOrEmpty(parts[4]), "Поле Будинок не може бути порожнім." }
            };
            foreach (var rule in rules)
            {
                if (rule.Key.Invoke())
                {
                    ShowWarning(rule.Value);
                    return false;
                }
            }
            return true;
        }
        public static bool AddEmployeeValidation(string employeeFullName, string employeeSex, string employeePhoneNumber, string employeePassport, string employeePostID,
            string employeeFamilyStatus, string employeeChildrenNumber, string employeeExperience,
            string employeeOrdersCompleted, string employeeEmail = "example@ex.ex", string employeeBirthDate = "12.12.1999")
        {
            const string emailPattern = @"^(([^<>()[\]\\.,;:\s@\""]+(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@(([^<>()[\]\\.,;:\s@\""]+\.)+[^<>()[\]\\.,;:\s@\""]{2,})$";
            string regexLetters = @"^[A-Za-zА-Яа-я]{2}\d{6}$";
            string regexNumbers = @"^\d{9}$";
            // Exception Dictionary
            try
            {
                DateTime.Parse(employeeBirthDate);
            }
            catch { return false; }
            Dictionary<Func<bool>, string> rules = new Dictionary<Func<bool>, string>
            {
                { () => string.IsNullOrEmpty(employeeFullName), "ПІБ не може бути порожнім." },
                { () => employeeFullName.Length < 3, "Довжина ПІБ менша за 3 символи." },
                { () => employeeFullName.Count(char.IsDigit) != 0, "ПІБ має містити лише літери." },
                { () => string.IsNullOrEmpty(employeeSex), "Оберіть стать." },
                { () => string.IsNullOrEmpty(employeePhoneNumber), "Номер телефону не може бути порожнім." },
                { () => employeePhoneNumber[0] != '0', $"Номер телефону має починатися з 0ХХ." },
                { () => employeePhoneNumber.Count(char.IsLetter) != 0, "Номер телефону не має містити літер." },
                { () => employeePhoneNumber.Length != 10, "Номер телефону має містити 10 цифр." },
                { () => string.IsNullOrEmpty(employeeEmail), "Електрона пошта не може бути порожньою." },
                { () => !Regex.IsMatch(employeeEmail, emailPattern), "Електрона пошта повинна бути записана у форматі: example@ex.ex" },
                { () => string.IsNullOrEmpty(employeePassport), "Номер паспорту не може бути порожнім." },
                { () => !Regex.IsMatch(employeePassport, regexLetters) && !Regex.IsMatch(employeePassport, regexNumbers), "Перевірте правильність номеру паспорту." },
                { () => string.IsNullOrEmpty(employeePostID), "Оберіть посаду." },
                { () => string.IsNullOrEmpty(employeeBirthDate), "Поле Дата народження не може бути порожнім." },
                { () => (DateTime.Now - DateTime.Parse(employeeBirthDate)).Days / 365.25 < 18, "Працівником може стати людина старше 18 років." },
                { () => string.IsNullOrEmpty(employeeFamilyStatus), "Оберіть сімейний стан." },
                { () => string.IsNullOrEmpty(employeeChildrenNumber), "Поле Кількість дітей не має бути порожнім." },
                { () => int.Parse(employeeChildrenNumber) < 0, "Кількість дітей має бути більшою або дорівнювати 0." },
                { () => string.IsNullOrEmpty(employeeExperience), "Поле Досвід роботи не має бути порожнім." },
                { () => int.Parse(employeeExperience) < 0, "Досвід роботи має бути більшим або дорівнювати 0." },
                { () => string.IsNullOrEmpty(employeeOrdersCompleted), "Поле Завершені замовлення не має бути порожнім." },
                { () => int.Parse(employeeOrdersCompleted) < 0, "Завершені замовлення мають бути більшими або дорівнювати 0." },
            };
            foreach (var rule in rules)
            {
                if (rule.Key.Invoke())
                {
                    ShowWarning(rule.Value);
                    return false;
                }
            }
            return true;
        }
        public static bool AddBookValidation(string imageURL, string bookName, float bookPrice,
            int bookGenre, int bookAuthor, string bookISBN, int bookPagesNumber, int bookFormat,
            string bookDescription, int bookNumber)
        {
            // Exception Dictionary
            Dictionary<Func<bool>, string> rules = new Dictionary<Func<bool>, string>
            {
                { () => string.IsNullOrEmpty(imageURL), "Посилання на обкладинку не може бути порожнім." },
                { () => string.IsNullOrEmpty(bookName), "Назва книги не може бути порожньою." },
                { () => bookPrice <= 0, "Ціна має бути вищою за 0." },
                { () => bookISBN.Count(char.IsLetter) != 0 && bookISBN.Length != 13, "ISBN не має містити літер та менше ніж 13 цифр." },
                { () => bookPagesNumber <= 0, "К-ть сторінок має бути більшою за 0." },
                { () => string.IsNullOrEmpty(bookDescription), "Опис не може бути порожнім." },
                { () => bookNumber <= 0, "Кількість екземплярів має бути додатнім числом." },
            };
            foreach (var rule in rules)
            {
                if (rule.Key.Invoke())
                {
                    ShowWarning(rule.Value);
                    return false;
                }
            }
            return true;
        }
        public static bool AddCustomValidation(string phoneNumber, string address)
        {
            string[] parts = { };
            try
            {
                parts = address.Split(new string[] { ", " }, StringSplitOptions.None);
                if (parts.Length != 5)
                    throw new Exception();
            }
            catch
            {
                ShowWarning("Заповніть поле адреса правильно! У форматі 'Країна, індекс, місто, вулиця, № будинку'.");
                return false;
            }
            // Exception Dictionary
            Dictionary<Func<bool>, string> rules = new Dictionary<Func<bool>, string>
            {
                { () => string.IsNullOrEmpty(phoneNumber), "Номер телефону не може бути порожнім." },
                { () => phoneNumber[0] != '0', $"Номер телефону має починатися з 0ХХ." },
                { () => phoneNumber.Count(char.IsLetter) != 0, "Номер телефону не має містити літер." },
                { () => phoneNumber.Length != 10, "Номер телефону має містити 10 цифр." },
                { () => string.IsNullOrEmpty(parts[0]), "Поле Країна не може бути порожнім." },
                { () => string.IsNullOrEmpty(parts[1]), "Поле Індекс не може бути порожнім." },
                { () => parts[1].Count(char.IsLetter) != 0, "Поле Індекс має містити лише цифри." },
                { () => parts[1].Length != 5, "Поле Індекс має містити 5 цифр." },
                { () => string.IsNullOrEmpty(parts[2]), "Поле Місто не може бути порожнім." },
                { () => string.IsNullOrEmpty(parts[3]), "Поле Вулиця не може бути порожнім." },
                { () => string.IsNullOrEmpty(parts[4]), "Поле Будинок не може бути порожнім." }
            };
            foreach (var rule in rules)
            {
                if (rule.Key.Invoke())
                {
                    ShowWarning(rule.Value);
                    return false;
                }
            }
            return true;
        }

        public static bool ValidateIntField(params string[] values)
        {
            try
            {
                foreach (string value in values)
                {
                    if (!string.IsNullOrEmpty(value) && int.Parse(value) < 0) throw new Exception();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ValidateIntField(params int[] values)
        {
            try
            {
                foreach (int value in values)
                {
                    if (value < 0) throw new Exception();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ValidateDoubleField(params string[] values)
        {
            try
            {
                foreach (string value in values)
                {
                    if (!string.IsNullOrEmpty(value) && double.Parse(value) < 0) throw new Exception();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ValidateDoubleField(params double[] values)
        {
            try
            {
                foreach (double value in values)
                {
                    if (value < 0) throw new Exception();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ValidateDateField(params string[] values)
        {
            try
            {
                foreach (string value in values)
                {
                    DateTime.Parse(value);
                    if (string.IsNullOrEmpty(value)) throw new Exception();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ValidateDateField(params DateTime[] values)
        {
            try
            {
                foreach (DateTime value in values)
                {
                    if (value > DateTime.Now) throw new Exception();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
