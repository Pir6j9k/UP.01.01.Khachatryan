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
    /// Логика взаимодействия для BookListsPage.xaml
    /// </summary>
    public partial class BookListsPage : Page
    {
        private string currentStatus = "В планах";

        private List<ReadingList> books;

        public BookListsPage()
        {
            InitializeComponent();

            LoadGenres();

            SortCB.SelectedIndex = 0;

            LoadBooks();
        }

        private void LoadGenres()
        {
            List<Genre> genres = Core.DB.Genres.ToList();

            genres.Insert(0, new Genre()
            {
                GenreName = "Все жанры"
            });

            GenreCB.ItemsSource = genres;
            GenreCB.SelectedIndex = 0;
        }

        private void LoadBooks()
        {
            books = Core.DB.ReadingLists .Where(x => x.UserID == Core.CurrentUser.UserID && x.Status == currentStatus) .ToList();

            string search = SearchTB.Text.ToLower();

            books = books.Where(x => x.Book.Title.ToLower().Contains(search) || x.Book.User.DisplayName.ToLower().Contains(search)).ToList();

            Genre selectedGenre = GenreCB.SelectedItem as Genre;

            if (selectedGenre != null && selectedGenre.GenreName != "Все жанры")
            {
                books = books.Where(x => x.Book.Genres.Any(g => g.GenreID == selectedGenre.GenreID)).ToList();
            }

            switch (SortCB.SelectedIndex)
            {
                case 1:
                    books = books.OrderBy(x => x.Book.Title).ToList();
                    break;
                case 2:
                    books = books.OrderByDescending(x => x.Book.Reviews.Any() ? x.Book.Reviews.Average(r => r.Rating) : 0).ToList();
                    break;
            }

            BooksLV.ItemsSource = books;
        }

        private void FiltersChanged(object sender, RoutedEventArgs e)
        {
            if (BooksLV == null)
                return;

            LoadBooks();
        }

        private void PlansBtn_Click(object sender, RoutedEventArgs e)
        {
            currentStatus = "В планах";
            LoadBooks();
        }

        private void ReadingBtn_Click(object sender, RoutedEventArgs e)
        {
            currentStatus = "Читаю";
            LoadBooks();
        }

        private void FinishedBtn_Click(object sender, RoutedEventArgs e)
        {
            currentStatus = "Прочитано";
            LoadBooks();
        }

        private void DroppedBtn_Click(object sender, RoutedEventArgs e)
        {
            currentStatus = "Заброшено";
            LoadBooks();
        }

        private void OpenBookBtn_Click(object sender, RoutedEventArgs e)
        {
            Book selectedBook = (sender as Button).Tag as Book;
            if (selectedBook == null)
                return;

            Core.MainFrame.Navigate(new BookCardPage(selectedBook));
        }

        private void StatusCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb == null)
                return;

            ReadingList reading = cb.Tag as ReadingList;
            if (reading == null)
                return;

            ComboBoxItem item = cb.SelectedItem as ComboBoxItem;
            if (item == null)
                return;

            reading.Status = item.Content.ToString();

            Core.DB.SaveChanges();
            LoadBooks();
        }
    }
}
