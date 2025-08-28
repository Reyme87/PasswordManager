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
    /// Логика взаимодействия для AddAccountPage.xaml
    /// </summary>
    public partial class AddAccountPage : Page
    {
        private readonly PasswordsViewModel _vm = PasswordsViewModel.Instance;
        public AddAccountPage()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PasswordsPage());
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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
