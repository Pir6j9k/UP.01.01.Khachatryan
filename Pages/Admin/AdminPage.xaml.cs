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

namespace УП._01._01.Khachatryan.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            ComplaintsDG.ItemsSource = Core.DB.Complaints.ToList();

            UnfreezeDG.ItemsSource = Core.DB.UnfreezeRequests.Include("User1").Where(x => x.Status == "На рассмотрении").ToList();

            RoleRequestsDG.ItemsSource = Core.DB.RoleRequests.Include("User1").Where(x => x.Status == "На рассмотрении").ToList();

            FrozenBooksDG.ItemsSource = Core.DB.Books.Include("User").Where(x => x.IsFrozen).ToList();

            FrozenUsersDG.ItemsSource = Core.DB.Users.Where(x => x.IsFrozen).ToList();

            UsersDG.ItemsSource = Core.DB.Users.Include("Role").ToList();
        }

        private void AcceptComplaintBtn_Click(object sender, RoutedEventArgs e)
        {
            Complaint complaint = (sender as Button).Tag as Complaint;

            if (complaint == null)
                return;

            if (complaint.BookID != null)
            {
                Book book = Core.DB.Books.FirstOrDefault( x => x.BookID == complaint.BookID);

                if (book != null)
                    book.IsFrozen = true;
            }

            Core.DB.Complaints.Remove(complaint);

            Core.DB.SaveChanges();

            LoadData();
        }

        private void RejectComplaintBtn_Click(object sender, RoutedEventArgs e)
        {
            Complaint complaint = (sender as Button).Tag as Complaint;

            if (complaint == null)
                return;

            Core.DB.Complaints.Remove(complaint);

            Core.DB.SaveChanges();

            LoadData();
        }

        private void AcceptUnfreezeBtn_Click(object sender, RoutedEventArgs e)
        {
            UnfreezeRequest request = (sender as Button).Tag as UnfreezeRequest;

            if (request == null)
                return;

            request.Status = "Одобрено";

            User user = Core.DB.Users.FirstOrDefault( x => x.UserID == request.UserID);

            if (user != null)
                user.IsFrozen = false;

            Core.DB.SaveChanges();

            LoadData();
        }

        private void RejectUnfreezeBtn_Click(object sender, RoutedEventArgs e)
        {
            UnfreezeRequest request = (sender as Button).Tag as UnfreezeRequest;

            if (request == null)
                return;

            request.Status = "Отклонено";

            Core.DB.SaveChanges();

            LoadData();
        }

        private void AcceptRoleBtn_Click(object sender, RoutedEventArgs e)
        {
            RoleRequest request = (sender as Button).Tag as RoleRequest;

            if (request == null)
                return;

            Role authorRole = Core.DB.Roles.FirstOrDefault( x => x.RoleName == "Автор");

            User user = Core.DB.Users.FirstOrDefault( x => x.UserID == request.UserID);

            if (authorRole != null && user != null)
            {
                user.RoleID = authorRole.RoleID;
            }

            request.Status = "Одобрено";

            Core.DB.SaveChanges();

            LoadData();
        }

        private void RejectRoleBtn_Click(object sender, RoutedEventArgs e)
        {
            RoleRequest request = (sender as Button).Tag as RoleRequest;

            if (request == null)
                return;

            request.Status = "Отклонено";

            Core.DB.SaveChanges();

            LoadData();
        }

        private void DeleteUserBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = (sender as Button).Tag as User;

            if (user == null)
                return;

            if (user.UserID == Core.CurrentUser.UserID)
            {
                MessageBox.Show("Нельзя удалить самого себя");
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Удалить пользователя {user.DisplayName}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var books = Core.DB.Books.Where(x => x.AuthorID == user.UserID).ToList();

                foreach (var book in books)
                {
                    book.Genres.Clear();

                    var bookReviews = Core.DB.Reviews.Where(x => x.BookID == book.BookID).ToList();
                    Core.DB.Reviews.RemoveRange(bookReviews);

                    var bookComplaints = Core.DB.Complaints.Where(x => x.BookID == book.BookID).ToList();
                    Core.DB.Complaints.RemoveRange(bookComplaints);

                    var readingLists = Core.DB.ReadingLists.Where(x => x.BookID == book.BookID).ToList();
                    Core.DB.ReadingLists.RemoveRange(readingLists);

                    var unfreezeRequests = Core.DB.UnfreezeRequests.Where(x => x.BookID == book.BookID).ToList();
                    Core.DB.UnfreezeRequests.RemoveRange(unfreezeRequests);

                    Core.DB.Books.Remove(book);
                }

                var reviews = Core.DB.Reviews.Where(x => x.UserID == user.UserID).ToList();
                Core.DB.Reviews.RemoveRange(reviews);

                var complaints = Core.DB.Complaints.Where(x => x.UserID == user.UserID).ToList();
                Core.DB.Complaints.RemoveRange(complaints);

                var lists = Core.DB.ReadingLists.Where(x => x.UserID == user.UserID).ToList();
                Core.DB.ReadingLists.RemoveRange(lists);

                var roleRequests = Core.DB.RoleRequests.Where(x => x.UserID == user.UserID).ToList();
                Core.DB.RoleRequests.RemoveRange(roleRequests);

                var userUnfreezeRequests =Core.DB.UnfreezeRequests.Where(x => x.UserID == user.UserID).ToList();
                Core.DB.UnfreezeRequests.RemoveRange(userUnfreezeRequests);

                Core.DB.Users.Remove(user);

                Core.DB.SaveChanges();

                LoadData();

                MessageBox.Show("Пользователь удалён");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private void FreezeUserBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = (sender as Button).Tag as User;

            if (user == null)
                return;

            user.IsFrozen = true;

            Core.DB.SaveChanges();

            LoadData();

            MessageBox.Show("Пользователь заморожен");
        }

        private void SetAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = (sender as Button).Tag as User;

            if (user == null)
                return;

            Role authorRole = Core.DB.Roles.FirstOrDefault( x => x.RoleName == "Автор");

            if (authorRole == null)
                return;

            user.RoleID = authorRole.RoleID;

            Core.DB.SaveChanges();

            LoadData();

            MessageBox.Show("Пользователь стал автором");
        }

        private void ChangePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = (sender as Button).Tag as User;

            if (user == null)
                return;

            Core.MainFrame.Navigate( new ChangePasswordPage(user));
        }
    }
}
