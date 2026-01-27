using System.Windows;
using System.Windows.Input;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;
using WpfMvvmApp.Views;

namespace WpfMvvmApp.ViewModels
{
    public class InspectionViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand NavigateToDeviceControlCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToLabelInvestigationCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ViewAuditTrailCommand { get; }
        public ICommand ResetAlarmsCommand { get; }

        private string _alarmsText = string.Empty;
        public string AlarmsText
        {
            get => _alarmsText;
            set => SetProperty(ref _alarmsText, value);
        }

        private bool _isInspecting;
        public bool IsInspecting
        {
            get => _isInspecting;
            set => SetProperty(ref _isInspecting, value);
        }

        public InspectionViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _alarmsText = "System initialized.\nReady for inspection...";

            StartCommand = new RelayCommand(OnStart);
            StopCommand = new RelayCommand(OnStop);
            NavigateToDeviceControlCommand = new RelayCommand(o => _navigationService.Navigate(new DeviceControlView(new DeviceControlViewModel(navigationService))));
            NavigateToSettingsCommand = new RelayCommand(o => _navigationService.Navigate(new SettingsView(new SettingsViewModel(navigationService))));
            NavigateToLabelInvestigationCommand = new RelayCommand(o => _navigationService.Navigate(new LabelInvestigationView(new LabelInvestigationViewModel(navigationService))));
            
            GoBackCommand = new RelayCommand(o => {
                if (_navigationService.CanGoBack) _navigationService.GoBack();
            });

            ViewAuditTrailCommand = new RelayCommand(o => _navigationService.Navigate(new AuditView(new AuditViewModel(navigationService))));
            ResetAlarmsCommand = new RelayCommand(o => AlarmsText = string.Empty);
        }

        private void OnStart(object? parameter)
        {
            IsInspecting = true;
            App.IsInspecting = true;
            AlarmsText += "\nInspection Started.";
        }

        private void OnStop(object? parameter)
        {
             IsInspecting = false;
             App.IsInspecting = false;
             // Navigate to Authorise View (Cancel Inspection)
             _navigationService.Navigate(new AuthoriseView(
                 new AuthoriseViewModel(_navigationService, () => 
                 {
                     // On successful authorisation, navigate to LpnEntryView
                     _navigationService.Navigate(new LpnEntryView(new LpnEntryViewModel(_navigationService)));
                 })
             ));
        }
    }
}
