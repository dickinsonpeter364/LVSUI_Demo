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
            NavigateToAuthoriseCommand = new RelayCommand(o => _navigationService.Navigate(new AuthoriseView(new AuthoriseViewModel(navigationService))));
            NavigateToDeviceControlCommand = new RelayCommand(o => _navigationService.Navigate(new DeviceControlView(new DeviceControlViewModel(navigationService))));
            NavigateToInspectionCommand = new RelayCommand(o => _navigationService.Navigate(new LpnEntryView(new LpnEntryViewModel(navigationService))));
        }

        // Add a public parameterless constructor
        public HomeViewModel()
        {
            // Initialization code if needed
        }

        public string WelcomeMessage => "UI Demo for Label Verification System";

        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToAuthoriseCommand { get; }
        public ICommand NavigateToDeviceControlCommand { get; }
        public ICommand NavigateToInspectionCommand { get; }
    }
}
