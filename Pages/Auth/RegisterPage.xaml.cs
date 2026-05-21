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

namespace УП._01._01.Khachatryan.Pages.Auth
{
    /// <summary>
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string name = NameTB.Text.Trim();
                string login = LoginTB.Text.Trim();
                string email = EmailTB.Text.Trim();
                string password = PasswordPB.Password.Trim();


                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(login) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Заполните все поля","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }


                bool loginExists = Core.DB.Users.Any(x => x.Login == login);

                if (loginExists)
                {
                    MessageBox.Show(
                        "Логин уже занят");

                    return;
                }

                // Проверка email

                bool emailExists =
                    Core.DB.Users.Any(x =>
                        x.Email == email);

                if (emailExists)
                {
                    MessageBox.Show(
                        "Email уже используется");

                    return;
                }

                // Роль пользователя

                Role userRole = Core.DB.Roles.FirstOrDefault(x => x.RoleName == "Читатель");

                if (userRole == null)
                {
                    MessageBox.Show(
                        "Роль 'Пользователь' не найдена");

                    return;
                }

                // Создание пользователя

                User newUser = new User()
                {
                    DisplayName = name,
                    Login = login,
                    Email = email,
                    PasswordHash = password,
                    RoleID = userRole.RoleID,
                    IsFrozen = false,
                    CreatedAt = DateTime.Now
                };

                Core.DB.Users.Add(newUser);

                Core.DB.SaveChanges();

                MessageBox.Show( "Регистрация успешна");

                Core.MainFrame.Navigate( new AuthPage());
                
                Core.CurrentUser = newUser;

                while (Core.MainFrame.CanGoBack)
                {
                    Core.MainFrame.RemoveBackEntry();
                }

            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

       

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.MainFrame.GoBack();
        }
    }
}
