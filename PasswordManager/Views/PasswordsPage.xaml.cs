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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordManager.Views
{
    /// <summary>
    /// Логика взаимодействия для PasswordsPage.xaml
    /// </summary>
    public partial class PasswordsPage : Page
    {
        private readonly PasswordsViewModel _vm = PasswordsViewModel.Instance;
        private bool isClicked = false;
        public PasswordsPage()
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
            NavigationService.Navigate(new AddAccountPage());
            _vm.UsernameField = _vm.PasswordField = _vm.UrlField = null;
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddAccountPage());
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation();
            if (!isClicked)
            {
                anim.From = 35;
                anim.To = 224;
                anim.Completed += (s, e) => ResetButton.Visibility = Visibility.Visible;
                isClicked = true;
            }
            else
            {
                anim.From = 224;
                anim.To = 35;
                ResetButton.Visibility = Visibility.Hidden;
                isClicked = false;
            }
            anim.Duration = TimeSpan.FromSeconds(0.5);
            anim.EasingFunction = new QuadraticEase();
            SearchBox.BeginAnimation(WidthProperty, anim);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 3;
            anim.AutoReverse = true;
            anim.Duration = TimeSpan.FromSeconds(1.5);
            anim.EasingFunction = new QuadraticEase();
            NotificationBorder.BeginAnimation(OpacityProperty, anim);
        }
    }
}
