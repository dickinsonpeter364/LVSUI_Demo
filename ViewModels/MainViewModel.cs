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
            App.IsInspecting = false;
            System.Windows.Application.Current.Shutdown();
        }

        private bool _isLogOffVisible = true;
        public bool IsLogOffVisible
        {
            get => _isLogOffVisible;
            set => SetProperty(ref _isLogOffVisible, value);
        }

        private bool _isInspecting;
        public bool IsInspecting
        {
            get => _isInspecting;
            set => SetProperty(ref _isInspecting, value);
        }
    }
}
