using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Publisher
{
    public static class SessionData
    {
        public const string rootConnection = "Server=localhost;Database=publisher;Uid=root;Pwd=Gang-12345;";
        public const string filePath = "./session/sessionData.txt";
        public static string UserId { get; set; }
        public static string ConnectionString { get; set; }
        public static bool isEmployee { get; set; }
        public static void LoadSessionData()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    UserId = lines[0];
                    ConnectionString = lines[1];
                    isEmployee = bool.Parse(lines[2]);
                }
                else
                {
                    Methods.ShowError("Відсутній файл для зберігання сесії.");
                    using (var file = File.CreateText(filePath))
                    {
                        file.WriteLine(UserId ?? "");
                        file.WriteLine(ConnectionString ?? "");
                        file.WriteLine(isEmployee.ToString() ?? "");
                    }
                    Methods.ShowInformation("Файл для зберігання сесії був створений. Будь ласка, увійдіть до свого аккаунту.");
                }
            }
            catch
            {
                Methods.ShowWarning($"Виникла помилка під час спроби читання з файлу сесії. Увійдіть в акаунт.");
            }
        }
        public static void SaveSessionData()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.WriteAllLines(filePath, new string[] { UserId, ConnectionString, isEmployee.ToString() });
                }
                else
                {
                    Methods.ShowError("Відсутній файл для зберігання сесії.");
                    using (var file = File.CreateText(filePath))
                    {
                        file.WriteLine(UserId ?? "");
                        file.WriteLine(ConnectionString ?? "");
                        file.WriteLine(isEmployee.ToString() ?? "");
                    }
                    Methods.ShowInformation("Файл для зберігання сесії був створений. Дані збережено.");
                }
            }
            catch { }
        }
    }
}
