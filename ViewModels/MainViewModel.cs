using System.ComponentModel;
using System.Windows.Input;
using LVS3;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvmApp.Core;
using WpfMvvmApp.Services;

namespace WpfMvvmApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IAlarmService _alarmService;
        private readonly ImxClient? _mxClient;

        public ICommand LogOffCommand { get; }
        public ICommand CancelAlarmsCommand { get; }

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _alarmService = App.Services.GetRequiredService<IAlarmService>();
            try { _mxClient = App.Services.GetService<ImxClient>(); } catch { _mxClient = null; }

            LogOffCommand = new RelayCommand(OnLogOff);
            CancelAlarmsCommand = new RelayCommand(OnCancelAlarms);

            // Forward alarm service changes to our own PropertyChanged so the view rebinds
            _alarmService.PropertyChanged += OnAlarmServiceChanged;
        }

        public string AlarmsText => _alarmService.AlarmsText;

        private void OnAlarmServiceChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IAlarmService.AlarmsText))
                OnPropertyChanged(nameof(AlarmsText));
        }

        private void OnCancelAlarms(object? parameter)
        {
            _alarmService.Clear();
            try { _mxClient?.ResetAlarm(1); } catch { }
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
