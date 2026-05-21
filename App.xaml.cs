using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using УП._01._01.Khachatryan.Pages.Auth;

namespace УП._01._01.Khachatryan
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(
            StartupEventArgs e)
        {
            base.OnStartup(e);


            Window authWindow = new Window()
            {
                Title = "Читай, Пиши и не спеши",
                Width = 400,
                Height = 600,
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#db7093"),
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };


            Frame frame = new Frame()
            {
                NavigationUIVisibility = NavigationUIVisibility.Hidden
            };


            Core.MainFrame = frame;


            frame.Navigate(new AuthPage());


            authWindow.Content = frame;


            authWindow.Show();
        }
    }
}
