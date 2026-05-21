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

namespace УП._01._01.Khachatryan.Pages.Profile
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();

            LoadProfile();

            LoadReviews();

            CheckFreeze();
        }

        // =========================
        // ПРОФИЛЬ
        // =========================

        private void LoadProfile()
        {
            User user = Core.CurrentUser;

            NameTB.Text =
                $"Имя: {user.DisplayName}";

            LoginTB.Text =
                $"Логин: {user.Login}";

            EmailTB.Text =
                $"Email: {user.Email}";

            RoleTB.Text =
                $"Роль: {user.Role?.RoleName}";
        }

        // =========================
        // ОТЗЫВЫ
        // =========================

        private void LoadReviews()
        {
            ReviewsIC.ItemsSource =
                Core.DB.Reviews
                .Where(x =>
                    x.UserID ==
                    Core.CurrentUser.UserID)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        // =========================
        // ПРОВЕРКА ЗАМОРОЗКИ
        // =========================

        private void CheckFreeze()
        {
            if (!Core.CurrentUser.IsFrozen)
                return;

            FrozenBorder.Visibility =
                Visibility.Visible;

            var complaint =
                Core.DB.Complaints
                .Where(x =>
                    x.UserID ==
                    Core.CurrentUser.UserID)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (complaint != null)
            {
                FreezeReasonTB.Text =
                    $"Причина: {complaint.Reason}";
            }
            else
            {
                FreezeReasonTB.Text =
                    "Причина не указана";
            }
        }

        // =========================
        // ЗАЯВКА НА АВТОРА
        // =========================

        private void RequestAuthorBtn_Click(object sender,
    RoutedEventArgs e)
        {
            // Проверка авторизации

            if (Core.CurrentUser == null)
            {
                MessageBox.Show(
                    "Необходимо авторизоваться");

                return;
            }

            // Если уже автор

            if (Core.CurrentUser.Role?.RoleName == "Автор")
            {
                MessageBox.Show(
                    "Вы уже являетесь автором");

                return;
            }


            if (Core.CurrentUser.Role?.RoleName == "Администратор")
            {
                MessageBox.Show(
                    "Администратор уже имеет права автора");

                return;
            }


            bool exists =
                Core.DB.RoleRequests.Any(x =>
                    x.UserID ==
                    Core.CurrentUser.UserID &&
                    x.Status == "На рассмотрении");

            if (exists)
            {
                MessageBox.Show(
                    "У вас уже есть активная заявка");

                return;
            }

            RoleRequest request =
                new RoleRequest()
                {
                    UserID = Core.CurrentUser.UserID,
                    RequestedRole = "Автор",
                    Status = "На рассмотрении",
                    CreatedAt = DateTime.Now
                };

            Core.DB.RoleRequests.Add(request);

            Core.DB.SaveChanges();

            MessageBox.Show(
                "Заявка отправлена");
        }

        // =========================
        // ОСПОРИТЬ ЗАМОРОЗКУ
        // =========================

        private void AppealBtn_Click(object sender,  RoutedEventArgs e)
        {
            NavigationService.Navigate( new AppealPage());
        }
    }
}
