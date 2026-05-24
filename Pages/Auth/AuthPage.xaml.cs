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
using УП._01._01.Khachatryan.Pages.Admin;
using УП._01._01.Khachatryan.Pages.Books;

namespace УП._01._01.Khachatryan.Pages.Auth
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginTB.Text.Trim();
                string password = PasswordPB.Password.Trim();

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Заполните все поля");
                    return;
                }

                User user = Core.DB.Users.FirstOrDefault(x => x.Login == login && x.PasswordHash == password);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль");
                    return;
                }

                Core.CurrentUser = user;

                MainWindow mainWindow = new MainWindow();

                mainWindow.Show();

                Window.GetWindow(this).Close();

                MessageBox.Show($"Добро пожаловать, {user.DisplayName}");

                if (user.Role.RoleName == "Администратор")
                {
                    Core.MainFrame.Navigate(new AdminPage());
                }
                else
                {
                    Core.MainFrame.Navigate(new CatalogPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RegisterBtn_Click(Object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}
