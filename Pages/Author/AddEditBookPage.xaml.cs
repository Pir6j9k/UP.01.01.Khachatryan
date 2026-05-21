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

namespace УП._01._01.Khachatryan.Pages.Author
{
    /// <summary>
    /// Логика взаимодействия для AddEditBookPage.xaml
    /// </summary>
    public partial class AddEditBookPage : Page
    {
        private Book currentBook;

        // Добавление
        public AddEditBookPage()
        {
            InitializeComponent();
            currentBook = new Book();
            LoadGenres();
        }

        // Редактирование
        public AddEditBookPage(Book selectedBook)
        {
            InitializeComponent();
            currentBook = Core.DB.Books
                .Include("Genres")
                .FirstOrDefault(b => b.BookID == selectedBook.BookID) ?? selectedBook;

            TitleTB.Text = "Редактирование книги";
            BookNameTB.Text = currentBook.Title;
            DescriptionTB.Text = currentBook.Description;
            CoverTB.Text = currentBook.CoverPath;
            ContentTB.Text = currentBook.Content;

            LoadGenres();
            SelectCurrentGenres();
        }

        private void LoadGenres()
        {
            GenresLB.ItemsSource = Core.DB.Genres.OrderBy(g => g.GenreName).ToList();
        }

        private void SelectCurrentGenres()
        {
            var bookGenreIds = currentBook.Genres.Select(g => g.GenreID).ToHashSet();
            foreach (var item in GenresLB.Items)
            {
                if (item is Genre genre && bookGenreIds.Contains(genre.GenreID))
                    GenresLB.SelectedItems.Add(item);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BookNameTB.Text) ||
                string.IsNullOrWhiteSpace(ContentTB.Text))
            {
                MessageBox.Show("Заполните обязательные поля: Название и Текст книги");
                return;
            }

            currentBook.Title = BookNameTB.Text.Trim();
            currentBook.Description = DescriptionTB.Text.Trim();
            currentBook.CoverPath = CoverTB.Text.Trim();
            currentBook.Content = ContentTB.Text.Trim();

            currentBook.Genres.Clear();
            foreach (var item in GenresLB.SelectedItems)
            {
                if (item is Genre selectedGenre)
                {
                    var genre = Core.DB.Genres.Find(selectedGenre.GenreID);
                    if (genre != null)
                        currentBook.Genres.Add(genre);
                }
            }

            if (currentBook.BookID == 0)
            {
                currentBook.AuthorID = Core.CurrentUser.UserID;
                currentBook.CreatedAt = DateTime.Now;
                currentBook.IsFrozen = false;
                Core.DB.Books.Add(currentBook);
            }

            try
            {
                Core.DB.SaveChanges();
                MessageBox.Show("Книга сохранена");
                Core.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.MainFrame.GoBack();
        }
    }
}
