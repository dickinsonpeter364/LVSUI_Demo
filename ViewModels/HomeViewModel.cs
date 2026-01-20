using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;
using WpfMvvmApp.Views;

namespace WpfMvvmApp.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public HomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateToSettingsCommand = new RelayCommand(o => _navigationService.Navigate(new SettingsView(new SettingsViewModel(navigationService))));
        }

        public string WelcomeMessage => "Welcome to the Home Page!";

        public ICommand NavigateToSettingsCommand { get; }
    }
}
