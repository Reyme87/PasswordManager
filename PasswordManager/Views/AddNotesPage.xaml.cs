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
    /// Логика взаимодействия для AddNotesPage.xaml
    /// </summary>
    public partial class AddNotesPage : Page
    {
        private readonly NotesViewModel _vm = NotesViewModel.Instance;
        public AddNotesPage()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NotesPage());
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
