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
using УП._01._01.Khachatryan.Pages.Auth;
using УП._01._01.Khachatryan.Pages.Author;
using УП._01._01.Khachatryan.Pages.Books;
using УП._01._01.Khachatryan.Pages.Profile;

namespace УП._01._01.Khachatryan
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Core.MainFrame = MainFrame;

            CheckRoles();

            MainFrame.Navigate(new CatalogPage());
        }

        private void Navigating(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string command)
            {
                switch (command)
                {
                    case "Catalog":
                        MainFrame.Navigate(new CatalogPage());
                        break;
                    case "BooksList":
                        MainFrame.Navigate(new BookListsPage());
                        break;
                    case "Admin":
                        MainFrame.Navigate(new AdminPage());
                        break;
                    case "Author":
                        MainFrame.Navigate(new AuthorPage());
                        break;                    
                    case "Frozen":
                        MainFrame.Navigate(new AppealPage());
                        break;
                }
            }
        }

        private void CheckRoles()
        {
            BtnAdmin.Visibility = Visibility.Collapsed;

            BtnAuthor.Visibility = Visibility.Collapsed;

            BtnFrozen.Visibility = Visibility.Collapsed;

            if (Core.CurrentUser == null)
                return;

            if (Core.CurrentUser.Role.RoleName == "Автор")
            {
                BtnAuthor.Visibility = Visibility.Visible;
            }

            if (Core.CurrentUser.Role.RoleName == "Администратор")
            {
                BtnAdmin.Visibility = Visibility.Visible;
            }

            if (Core.CurrentUser.IsFrozen)
            {
                BtnFrozen.Visibility = Visibility.Visible;
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            BtnBack.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Hidden;

            if (MainFrame.Content is Page page)
            {
                TxtPageTitle.Text = page.Title;
            }
            CheckRoles();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                Core.MainFrame.Navigate(
                    new Pages.Auth.AuthPage());

                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите выйти из аккаунта?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            Core.CurrentUser = null;

            while (MainFrame.CanGoBack)
            {
                MainFrame.RemoveBackEntry();
            }

            CheckRoles();

            MainFrame.Navigate(new Pages.Books.CatalogPage());

            MessageBox.Show("Вы вышли из аккаунта");
            Application.Current.Shutdown();
        }
        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MainFrame.Navigate(new AuthPage());
                return;
            }

            MainFrame.Navigate(new ProfilePage());
        }
    }
}
