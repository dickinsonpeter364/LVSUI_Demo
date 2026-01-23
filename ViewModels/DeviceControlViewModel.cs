using System.Windows;
using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;

namespace WpfMvvmApp.ViewModels
{
    public class DeviceControlViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private double _speed;

        public double Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value);
        }

        public ICommand RewindCommand { get; }
        public ICommand WindCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand GoBackCommand { get; }

        public DeviceControlViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            // Default speed
            Speed = 50;

            RewindCommand = new RelayCommand(OnRewind);
            WindCommand = new RelayCommand(OnWind);
            StopCommand = new RelayCommand(OnStop);
            GoBackCommand = new RelayCommand(o => {
                if (_navigationService.CanGoBack) _navigationService.GoBack();
            });
        }

        private void OnRewind(object parameter)
        {
            MessageBox.Show($"Rewinding at speed {Speed:F0}%", "Device Control", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnWind(object parameter)
        {
             MessageBox.Show($"Winding at speed {Speed:F0}%", "Device Control", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnStop(object parameter)
        {
             MessageBox.Show("Stopping Device", "Device Control", MessageBoxButton.OK, MessageBoxImage.Exclamation);
             Speed = 0;
        }
    }
}
