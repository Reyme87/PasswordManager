using PasswordManager.ViewModels;
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

namespace PasswordManager.Views
{
    /// <summary>
    /// Логика взаимодействия для CardsPage.xaml
    /// </summary>
    public partial class CardsPage : Page
    {
        private readonly CardsViewModel _vm = CardsViewModel.Instance;
        public CardsPage()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCardPage());
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCardPage());
        }
    }
}
