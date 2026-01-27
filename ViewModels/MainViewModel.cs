using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;
using WpfMvvmApp.Views;

namespace WpfMvvmApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ICommand LogOffCommand { get; }

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            LogOffCommand = new RelayCommand(OnLogOff);
        }

        private void OnLogOff(object? parameter)
        {
            // Reset state
            App.IsInspecting = false;
            
            // Navigate back to startup screen (AuthoriseView with "Log On" title)
            _navigationService.Navigate(new AuthoriseView(new AuthoriseViewModel(_navigationService, title: "Log On")));
        }

        private bool _isInspecting;
        public bool IsInspecting
        {
            get => _isInspecting;
            set => SetProperty(ref _isInspecting, value);
        }
    }
}
