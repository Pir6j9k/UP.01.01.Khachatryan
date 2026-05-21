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
    /// Логика взаимодействия для ChangePasswordPage.xaml
    /// </summary>
    public partial class ChangePasswordPage : Page
    {
        private User currentUser;

        public ChangePasswordPage(User user)
        {
            InitializeComponent();

            currentUser = user;

            UserTB.Text = currentUser.DisplayName;
        }

        private void SaveBtn_Click(object sender,
            RoutedEventArgs e)
        {
            string password = PasswordPB.Password.Trim();

            string repeatPassword = RepeatPasswordPB.Password.Trim();

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show( "Введите пароль");
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show( "Пароль должен быть минимум 6 символов");
                return;
            }

            if (password != repeatPassword)
            {
                MessageBox.Show( "Пароли не совпадают");
                return;
            }

            currentUser.PasswordHash = password;

            Core.DB.SaveChanges();

            MessageBox.Show( "Пароль изменён");

            Core.MainFrame.GoBack();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.MainFrame.GoBack();
        }
    }
}
