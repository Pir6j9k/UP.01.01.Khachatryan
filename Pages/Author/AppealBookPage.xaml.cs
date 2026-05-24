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
    /// Логика взаимодействия для AppealBookPage.xaml
    /// </summary>
    public partial class AppealBookPage : Page
    {
        private Book currentBook;

        public AppealBookPage(Book selectedBook)
        {
            InitializeComponent();

            currentBook = selectedBook;

            LoadBook();
        }

        private void LoadBook()
        {
            BookTitleTB.Text = $"Книга: {currentBook.Title}";
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            string reason = ReasonTB.Text.Trim();

            if (string.IsNullOrWhiteSpace(reason))
            {
                MessageBox.Show( "Введите причину обращения");
                return;
            }

            bool exists = Core.DB.UnfreezeRequests.Any(x => x.BookID == currentBook.BookID && x.Status == "На рассмотрении");

            if (exists)
            {
                MessageBox.Show( "Заявка для этой книги уже отправлена");
                return;
            }

            UnfreezeRequest request = new UnfreezeRequest()
            {
                UserID = Core.CurrentUser.UserID,
                BookID = currentBook.BookID,
                Reason = reason,
                Status = "На рассмотрении",
                CreatedAt = DateTime.Now
            };

            Core.DB.UnfreezeRequests.Add(request);

            Core.DB.SaveChanges();

            MessageBox.Show("Заявка успешно отправлена");

            Core.MainFrame.GoBack();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.MainFrame.GoBack();
        }
    }
}
