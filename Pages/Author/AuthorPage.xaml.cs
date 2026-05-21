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
using УП._01._01.Khachatryan.Pages.Books;

namespace УП._01._01.Khachatryan.Pages.Author
{
    /// <summary>
    /// Логика взаимодействия для AuthorPage.xaml
    /// </summary>
    public partial class AuthorPage : Page
    {
        public AuthorPage()
        {
            InitializeComponent();

            LoadBooks();
        }

        private void LoadBooks()
        {
            BooksGrid.ItemsSource = Core.DB.Books
                .Where(x => x.AuthorID == Core.CurrentUser.UserID)
                .ToList();
        }

        private void AddBookBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.MainFrame.Navigate(new AddEditBookPage());
        }



        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Book selectedBook =
                (sender as Button).DataContext as Book;

            if (selectedBook == null)
                return;

            Core.MainFrame.Navigate(
                new AddEditBookPage(selectedBook));
        }


        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Book selectedBook = (sender as Button).DataContext as Book;
            if (selectedBook == null)
                return;

            MessageBoxResult result = MessageBox.Show(
                "Удалить книгу вместе со всеми отзывами и списками чтения?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var reviews = Core.DB.Reviews
                    .Where(r => r.BookID == selectedBook.BookID)
                    .ToList();

                foreach (var review in reviews)
                {
                    var complaints = Core.DB.Complaints
                        .Where(c => c.ReviewID == review.ReviewID)
                        .ToList();
                    Core.DB.Complaints.RemoveRange(complaints);
                }

                Core.DB.Reviews.RemoveRange(reviews);

                var readingLists = Core.DB.ReadingLists
                    .Where(rl => rl.BookID == selectedBook.BookID)
                    .ToList();
                Core.DB.ReadingLists.RemoveRange(readingLists);

                var bookToDelete = Core.DB.Books
                    .Include("Genres")
                    .FirstOrDefault(b => b.BookID == selectedBook.BookID);
                bookToDelete.Genres.Clear();

                Core.DB.Books.Remove(bookToDelete);
                Core.DB.SaveChanges();

                MessageBox.Show("Книга удалена.");
                LoadBooks();
            }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;

                MessageBox.Show("Ошибка при удалении:\n" + inner.Message);
            }
        }


        private void BooksGrid_MouseDoubleClick(object sender,
            MouseButtonEventArgs e)
        {
            Book selectedBook =
                BooksGrid.SelectedItem as Book;

            if (selectedBook == null)
                return;

            Core.MainFrame.Navigate(
                new BookCardPage(selectedBook));
        }
    }
}
