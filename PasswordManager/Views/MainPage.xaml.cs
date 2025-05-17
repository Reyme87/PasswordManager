using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using PasswordManager.Views;

namespace PasswordManager
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void PasswordsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PasswordsPage());
        }

        private void CardsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CardsPage());
        }
    }
}
