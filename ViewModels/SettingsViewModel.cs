using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;

namespace WpfMvvmApp.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public SettingsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GoBackCommand = new RelayCommand(o => _navigationService.GoBack());
        }

        public string SettingsMessage => "This is the Settings Page.";

        public ICommand GoBackCommand { get; }
    }
}
