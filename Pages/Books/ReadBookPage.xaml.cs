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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace УП._01._01.Khachatryan.Pages.Books
{
    /// <summary>
    /// Логика взаимодействия для ReadBookPage.xaml
    /// </summary>
    public partial class ReadBookPage : Page
    {
        private Book currentBook;

        public ReadBookPage(Book selectedBook)
        {
            InitializeComponent();

            currentBook = selectedBook;

            LoadBook();
        }

        // =========================
        // ЗАГРУЗКА КНИГИ
        // =========================

        private void LoadBook()
        {
            BookTitleTB.Text = currentBook.Title;

            BookContentTB.Text = currentBook.Content;

            if (currentBook.User != null)
            {
                AuthorTB.Text =
                    $"Автор: {currentBook.User.DisplayName}";
            }
        }

        // =========================
        // СТАТУС: ПРОЧИТАНО
        // =========================

        private void MarkAsReadBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Прочитано");
        }

        // =========================
        // СТАТУС: ЧИТАЮ
        // =========================

        private void MarkAsReadingBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Читаю");
        }

        // =========================
        // ОБНОВЛЕНИЕ СТАТУСА
        // =========================

        private void UpdateStatus(string status)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Необходимо авторизоваться");
                return;
            }

            ReadingList readingBook =
                Core.DB.ReadingLists.FirstOrDefault(x =>
                    x.UserID == Core.CurrentUser.UserID &&
                    x.BookID == currentBook.BookID);

            if (readingBook == null)
            {
                readingBook = new ReadingList()
                {
                    UserID = Core.CurrentUser.UserID,
                    BookID = currentBook.BookID,
                    Status = status,
                    AddedAt = DateTime.Now
                };

                Core.DB.ReadingLists.Add(readingBook);
            }
            else
            {
                readingBook.Status = status;
            }

            Core.DB.SaveChanges();

            MessageBox.Show($"Статус изменён: {status}");
        }

        // =========================
        // НАЗАД
        // =========================

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.MainFrame.GoBack();
        }
    }
}
