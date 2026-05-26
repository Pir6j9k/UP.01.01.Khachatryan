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
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        private List<Book> books;

        public CatalogPage()
        {
            InitializeComponent();

            LoadGenres();

            LoadBooks(); 
            this.Loaded += (s, e) => LoadBooks();
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

            SortCB.SelectedIndex = 0;
        }
 
        private void LoadBooks()
        {
            books = Core.DB.Books.Where(x => !x.IsFrozen).ToList();

            string search = SearchTB.Text.ToLower();

            books = books.Where(x => x.Title.ToLower().Contains(search) || x.User.DisplayName.ToLower().Contains(search)).ToList();

            Genre selectedGenre = GenreCB.SelectedItem as Genre;

            if (selectedGenre != null && selectedGenre.GenreName != "Все жанры")
            {
                books = books.Where(x => x.Genres.Any(g => g.GenreID == selectedGenre.GenreID)).ToList();
            }

            switch (SortCB.SelectedIndex)
            {
                case 1:
                    books = books.OrderBy(x => x.Title).ToList();
                    break;

                case 2:
                    books = books.OrderByDescending(x => x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0).ToList();
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

        private void OpenBookBtn_Click(object sender, RoutedEventArgs e)
        {
            Book selectedBook = (sender as Button).Tag as Book;

            if (selectedBook == null)
                return;

            Core.MainFrame.Navigate(new BookCardPage(selectedBook));
        }

        private void AddToPlanBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show( "Необходимо авторизоваться");
                return;
            }

            if (Core.CheckFrozen()) return;

            Book selectedBook = (sender as Button).Tag as Book;

            if (selectedBook == null)
                return;

            bool exists = Core.DB.ReadingLists.Any(x => x.BookID == selectedBook.BookID && x.UserID == Core.CurrentUser.UserID);

            if (exists)
            {
                MessageBox.Show( "Книга уже есть в списках");
                return;
            }

            ReadingList readingList = new ReadingList()
            {
                UserID = Core.CurrentUser.UserID,
                BookID = selectedBook.BookID,
                Status = "В планах",
                AddedAt = DateTime.Now
            };

            Core.DB.ReadingLists.Add(readingList);

            Core.DB.SaveChanges();

            MessageBox.Show( "Книга добавлена");
        }
    }
}
