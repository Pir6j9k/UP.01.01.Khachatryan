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
    /// Логика взаимодействия для BookCardPage.xaml
    /// </summary>
    public partial class BookCardPage : Page
    {
        private Book currentBook;

        private int selectedRating = 10;

        public Visibility IsAdminVisibility => Core.CurrentUser?.Role?.RoleName == "Администратор" ? Visibility.Visible : Visibility.Collapsed;

        public BookCardPage(Book selectedBook)
        {
            InitializeComponent();

            DataContext = this;

            currentBook = selectedBook;

            LoadBook();

            LoadReviews();

            CheckAdmin();

            InitStars();
        }

        private void InitStars()
        {
            foreach (Button star in StarsPanel.Children)
            {
                int number = Convert.ToInt32(star.Tag);

                star.Content = number <= selectedRating ? "★" : "☆";
            }
        }
        private void Star_Click(object sender, RoutedEventArgs e)
        {
            Button clickedStar = sender as Button;

            if (clickedStar == null)
                return;

            selectedRating = Convert.ToInt32( clickedStar.Tag);

            int current = 1;

            foreach (Button star in StarsPanel.Children)
            {
                if (current <= selectedRating)
                {
                    star.Content = "★";
                }
                else
                {
                    star.Content = "☆";
                }

                current++;
            }
        }

        private void LoadBook()
        {
            TitleTB.Text = currentBook.Title;

            DescriptionTB.Text = currentBook.Description;

            AuthorTB.Text = $"Автор: {currentBook.User?.DisplayName}";

            try
            {
                if (!string.IsNullOrWhiteSpace(currentBook.CoverPath))
                {
                    CoverImage.Source = new BitmapImage( new Uri( currentBook.CoverPath, UriKind.RelativeOrAbsolute));
                }
            }
            catch { }


            GenresWP.Children.Clear();

            foreach (var genre in currentBook.Genres)
            {
                Border border = new Border()
                {
                    BorderBrush = (SolidColorBrush) new BrushConverter() .ConvertFromString("#fff000"),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(5),
                    Padding = new Thickness(5)
                };

                border.Child = new TextBlock()
                {
                    Text = genre.GenreName,
                    FontSize = 18
                };

                GenresWP.Children.Add(border);
            }
        }

        private void LoadReviews()
        {
            ReviewsIC.ItemsSource = Core.DB.Reviews.Where(x => x.BookID == currentBook.BookID).OrderByDescending(x => x.CreatedAt).ToList();
        }

        private void CheckAdmin()
        {
            if (Core.CurrentUser?.Role?.RoleName == "Администратор")
            {
                AdminPanel.Visibility = Visibility.Visible;
            }
        }

        private void ReadBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.MainFrame.Navigate(new ReadBookPage(currentBook));
        }

        private void SendReviewBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Core.CurrentUser == null)
                {
                    MessageBox.Show("Необходимо авторизоваться");
                    return;
                }

                Review review = new Review()
                {
                    BookID = currentBook.BookID,
                    UserID = Core.CurrentUser.UserID,
                    Rating = selectedRating,
                    ReviewText = ReviewTB.Text.Trim(),
                    CreatedAt = DateTime.Now
                };

                Core.DB.Reviews.Add(review);

                Core.DB.SaveChanges();

                ReviewTB.Clear();

                selectedRating = 10;

                InitStars();

                LoadReviews();

                MessageBox.Show("Отзыв успешно добавлен");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReportBookBtn_Click( object sender, RoutedEventArgs e)
        {
            try
            {
                if (Core.CurrentUser == null)
                {
                    MessageBox.Show( "Необходимо авторизоваться");
                    return;
                }

                Complaint complaint = new Complaint()
                {
                    UserID = Core.CurrentUser.UserID,

                    BookID = currentBook.BookID,

                    Reason = "Жалоба на книгу",

                    CreatedAt = DateTime.Now
                };

                Core.DB.Complaints.Add( complaint);

                Core.DB.SaveChanges();

                MessageBox.Show( "Жалоба отправлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message);
            }
        }

        private void ReportAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Core.CurrentUser == null)
                {
                    MessageBox.Show("Необходимо авторизоваться");
                    return;
                }

                Complaint complaint = new Complaint()
                {
                    UserID = Core.CurrentUser.UserID,

                    BookID = currentBook.BookID,

                    Reason = "Жалоба на автора",

                    CreatedAt = DateTime.Now
                };

                Core.DB.Complaints.Add(complaint);

                Core.DB.SaveChanges();

                MessageBox.Show("Жалоба отправлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReportReviewBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Core.CurrentUser == null)
                {
                    MessageBox.Show("Необходимо авторизоваться");
                    return;
                }

                Review review = (sender as Button)?.Tag as Review;

                if (review == null)
                    return;

                Complaint complaint = new Complaint()
                {
                    UserID = Core.CurrentUser.UserID,

                    ReviewID = review.ReviewID,

                    Reason =  "Жалоба на отзыв",

                    CreatedAt = DateTime.Now
                };

                Core.DB.Complaints.Add(complaint);

                Core.DB.SaveChanges();

                MessageBox.Show( "Жалоба отправлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FreezeBookBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentBook.IsFrozen = true;

                Core.DB.SaveChanges();

                MessageBox.Show("Книга заморожена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FreezeReviewBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Review review = (sender as Button)?.Tag as Review;

                if (review == null)
                    return;

                Core.DB.Reviews.Remove(review);

                Core.DB.SaveChanges();

                LoadReviews();

                MessageBox.Show( "Отзыв удалён");
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message);
            }
        }
    }
}
