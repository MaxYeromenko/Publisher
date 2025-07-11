using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Publisher
{
    public static class OrderInfo
    {
        public static string ReceiptNumber { get; set; }

        public static string CustomerFullName { get; set; }

        public static string DeliveryAddress { get; set; }

        public static string EmployeeFullName { get; set; }

        public static string RegistrationDate { get; set; }

        public static double TotalPrice { get; set; }

        public static bool IsRegular { get; set; }

        public static List<BookInfo> BookList { get; set; } = new List<BookInfo>();

        static string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";

        static iTextSharp.text.Paragraph CreateParagraph(string text)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font = new Font(baseFont, 12, Font.NORMAL);
            return new iTextSharp.text.Paragraph(text, font);
        }

        public static PdfPCell CreateCell(string text)
        {
            BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font = new Font(baseFont, 12, Font.NORMAL);
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            return cell;
        }

        public static void CreateReceipt()
        {
            try
            {
                Document document = new Document();
                PdfWriter.GetInstance(document, new FileStream(
                    Path.Combine(downloadsPath, $"Receipt{ReceiptNumber}.pdf"),
                    FileMode.Create));

                document.Open();

                PdfPTable table = new PdfPTable(3);
                table.AddCell(CreateCell("Назва книги"));
                table.AddCell(CreateCell("Кількість екземплярів"));
                table.AddCell(CreateCell("Ціна"));

                foreach (var book in BookList)
                {
                    table.AddCell(CreateCell(book.BookName));
                    table.AddCell(CreateCell(book.BookNumber.ToString()));
                    table.AddCell(CreateCell($"{book.BookPrice} грн"));
                }

                BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                Font PublisherNameFont = new Font(baseFont, 20, Font.NORMAL);
                Font Thanks = new Font(baseFont, 12, Font.ITALIC);

                var PublisherName = new iTextSharp.text.Paragraph("Книжкова вежа", PublisherNameFont);
                PublisherName.Alignment = Element.ALIGN_CENTER;

                document.Add(PublisherName);
                document.Add(new Chunk("\n"));
                document.Add(CreateParagraph($"Видаткова накладна №{ReceiptNumber}"));
                document.Add(CreateParagraph($"Дата оформлення: {RegistrationDate}"));
                document.Add(CreateParagraph($"ПІБ працівника, хто оформив: {EmployeeFullName}"));
                document.Add(CreateParagraph($"ПІБ клієнта: {CustomerFullName}"));
                document.Add(CreateParagraph($"Адреса доставки: {DeliveryAddress}"));
                document.Add(new Chunk("\n"));
                document.Add(table);
                document.Add(new Chunk("\n"));
                var TotalPriceParagraph = CreateParagraph($"Загальна вартість: {TotalPrice} грн.");
                TotalPriceParagraph.Alignment = Element.ALIGN_RIGHT;
                document.Add(TotalPriceParagraph);
                if (IsRegular)
                {
                    var TotalPriceParagraphDiscount = 
                        CreateParagraph($"Вартість з урахуванням знижки: {TotalPrice - TotalPrice * 0.03} грн.");
                    TotalPriceParagraphDiscount.Alignment = Element.ALIGN_RIGHT;
                    document.Add(TotalPriceParagraphDiscount);
                }
                document.Add(new iTextSharp.text.Paragraph("\nДякуємо за покупку в нашому видавництві! " +
                    "Ми цінуємо ваш вибір та сподіваємося побачити вас знову. Бажаємо вам гарного дня!", Thanks));
                document.Close();
                Methods.ShowInformation("Чек успішно створено!");
            }
            catch
            {
                Methods.ShowError($"Виникла помилка під час створення чека.");
            }
        }
    }
    public class BookInfo
    {
        public int BookID { get; set; }
        public string BookName { get; set; }
        public int BookNumber { get; set; }
        public float BookPrice { get; set; }
    }
    public static class OrderedBookList
    {
        public static List<BookInfo> orderedBooks = new List<BookInfo>();
    }
    public class BookAuthorPopularity
    {
        public string Name { get; set; }
        public int TotalBooks { get; set; }
        public float TotalIncome { get; set; }

    }
    public static class BookAuthorPopularityList
    {
        public static bool IsBook { get; set; } = false;
        public static bool IsAuthor { get; set; } = false;
        public static bool IsSalesReport { get; set; } = false;
        public static List<BookAuthorPopularity> itemsList = new List<BookAuthorPopularity>();
    }
}
