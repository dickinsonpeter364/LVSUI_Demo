using System.Windows;
using WpfMvvmApp.Services;
using WpfMvvmApp.ViewModels;

namespace WpfMvvmApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Composition Root (Simplified)
            INavigationService navigationService = new NavigationService(MainFrame);
            MainViewModel mainViewModel = new MainViewModel(navigationService);
            DataContext = mainViewModel;

            // Navigate to Login/Authorise
            navigationService.Navigate(new AuthoriseView(new AuthoriseViewModel(navigationService)));
        }
    }
}