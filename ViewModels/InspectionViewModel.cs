using System.Collections.ObjectModel;
using System.Windows.Input;
using LVS3;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvmApp.Core;
using WpfMvvmApp.Core.Messages;
using WpfMvvmApp.Services;
using WpfMvvmApp.Views;
using static LVS3.Delegates;

namespace WpfMvvmApp.ViewModels
{
    public class InspectionViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IInspection _inspection;
        private readonly ImxClient _mxClient;
        private readonly IDataManager _dataManager;
        private readonly IMessagingService _messaging;

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

        private string _infoText = string.Empty;
        public string InfoText
        {
            get => _infoText;
            set => SetProperty(ref _infoText, value);
        }

        private string _timeText = string.Empty;
        public string TimeText
        {
            get => _timeText;
            set => SetProperty(ref _timeText, value);
        }

        private bool _isInspecting;
        public bool IsInspecting
        {
            get => _isInspecting;
            set => SetProperty(ref _isInspecting, value);
        }

        private bool _endInspectionEnabled;
        public bool EndInspectionEnabled
        {
            get => _endInspectionEnabled;
            set => SetProperty(ref _endInspectionEnabled, value);
        }

        public ObservableCollection<string> SystemMessages { get; } = new();

        private readonly int _lafCount;
        public int LafCount => _lafCount;

        public InspectionViewModel(INavigationService navigationService, int lafCount = 1)
        {
            _navigationService = navigationService;
            _lafCount = lafCount;

            // Resolve services from DI
            _inspection = App.Services.GetRequiredService<IInspection>();
            _mxClient = App.Services.GetRequiredService<ImxClient>();
            _dataManager = App.Services.GetRequiredService<IDataManager>();
            _messaging = App.Services.GetRequiredService<IMessagingService>();

            _alarmsText = "System initialized.\nReady for inspection...";

            // Subscribe to messages
            _messaging.Register<SystemMessage>(this, OnSystemMessage);
            _messaging.Register<AlarmMessage>(this, OnAlarmMessage);
            _messaging.Register<InspectionStatusMessage>(this, OnInspectionStatus);

            StartCommand = new RelayCommand(OnStart);
            StopCommand = new RelayCommand(OnStop);
            NavigateToDeviceControlCommand = new RelayCommand(o => _navigationService.Navigate(new DeviceControlView(new DeviceControlViewModel(navigationService))));
            NavigateToSettingsCommand = new RelayCommand(o => _navigationService.Navigate(new SettingsView(new SettingsViewModel(navigationService))));
            NavigateToLabelInvestigationCommand = new RelayCommand(o => _navigationService.Navigate(new LabelInvestigationView(new LabelInvestigationViewModel(navigationService))));

            GoBackCommand = new RelayCommand(o => {
                if (_navigationService.CanGoBack) _navigationService.GoBack();
            });

            ViewAuditTrailCommand = new RelayCommand(o => _navigationService.Navigate(new AuditView(new AuditViewModel(navigationService))));
            ResetAlarmsCommand = new RelayCommand(o =>
            {
                AlarmsText = string.Empty;
                _mxClient.ResetAlarm(1);
            });
        }

        private void OnStart(object? parameter)
        {
            // Navigate to QC Review before starting
            _navigationService.Navigate(new QcReviewView(new QcReviewViewModel(_navigationService, _lafCount, this)));
        }

        /// <summary>
        /// Called by QcReviewViewModel after QC review is accepted and authorised.
        /// Initializes the inspection engine with a UI-agnostic context.
        /// </summary>
        public void CompleteStartInspection()
        {
            IsInspecting = true;
            App.IsInspecting = true;

            // Build the InspectionContext with UI callbacks
            var ctx = new InspectionContext
            {
                ReelLpn = "", // Set from LPN entry
                MxClient = _mxClient,
                OnInfoTextChanged = text => InfoText = text,
                OnTimeTextChanged = text => TimeText = text,
                OnEndInspectionEnabled = enabled => EndInspectionEnabled = enabled,
                OnPassFailImageChanged = img => { /* TODO: bind to WPF Image control */ },
                OnImageDisplay = img => { /* TODO: display inspection image */ },
                OnImageClear = () => { /* TODO: clear display */ },
                OnSystemMessage = smea =>
                {
                    var msg = $"[{smea.TITLE}] {smea.MSG}";
                    System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                        SystemMessages.Add(msg));
                },
                OnAlarm = () => AlarmsText += "\nAlarm triggered.",
                OnError = err => AlarmsText += $"\nError: {err}"
            };

            _inspection.InitInspection(ctx);
            AlarmsText += "\nInspection Started.";
        }

        private void OnStop(object? parameter)
        {
            IsInspecting = false;
            App.IsInspecting = false;
            _mxClient.Stop(1);

            // Navigate to Authorise View (Cancel Inspection)
            _navigationService.Navigate(new AuthoriseView(
                new AuthoriseViewModel(_navigationService, () =>
                {
                    _navigationService.Navigate(new DeviceControlView(new DeviceControlViewModel(_navigationService, DeviceControlMode.PostLogin, () =>
                    {
                        _navigationService.Navigate(new LpnEntryView(new LpnEntryViewModel(_navigationService)));
                    })));
                })
            ));
        }

        private void OnSystemMessage(SystemMessage msg)
        {
            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                SystemMessages.Add($"[{msg.Severity}] {msg.Message}");
                if (msg.Severity == MessageSeverity.Error || msg.Severity == MessageSeverity.Critical)
                    AlarmsText += $"\n{msg.Message}";
            });
        }

        private void OnAlarmMessage(AlarmMessage msg)
        {
            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                AlarmsText += $"\nAlarm Ch{msg.Channel}: {(msg.IsActive ? "ACTIVE" : "cleared")}";
            });
        }

        private void OnInspectionStatus(InspectionStatusMessage msg)
        {
            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                IsInspecting = msg.IsProcessing;
                App.IsInspecting = msg.IsProcessing;
                if (msg.Result != null)
                    AlarmsText += $"\nLabel {msg.LabelIndex}: {msg.Result}";
            });
        }
    }
}
