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
    /// Логика взаимодействия для AppealPage.xaml
    /// </summary>
    public partial class AppealPage : Page
    {
        public AppealPage()
        {
            InitializeComponent();

            CheckUser();
        }

        private void CheckUser()
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Необходимо авторизоваться");
                Core.MainFrame.Navigate(new Pages.Auth.AuthPage());
            }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            string reason = ReasonTB.Text.Trim();

            if (string.IsNullOrWhiteSpace(reason))
            {
                MessageBox.Show("Введите причину обращения");
                return;
            }

            if (!Core.CurrentUser.IsFrozen)
            {
                MessageBox.Show("Ваш аккаунт не заморожен");
                return;
            }

            bool exists = Core.DB.UnfreezeRequests.Any(x => x.UserID == Core.CurrentUser.UserID && x.Status == "На рассмотрении");

            if (exists)
            {
                MessageBox.Show("У вас уже есть активная заявка");
                return;
            }

            UnfreezeRequest request = new UnfreezeRequest()
            {
                UserID = Core.CurrentUser.UserID,
                Reason = reason,
                Status = "На рассмотрении",
                CreatedAt = DateTime.Now
            };

            Core.DB.UnfreezeRequests.Add(request);

            Core.DB.SaveChanges();

            MessageBox.Show("Заявка отправлена");

            Core.MainFrame.Navigate(new ProfilePage());
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.MainFrame.Navigate(new ProfilePage());
        }
    }
}
