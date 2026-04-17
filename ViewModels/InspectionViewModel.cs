using System.Collections.ObjectModel;
using System.Windows.Input;
using CONSTANTS;
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
        public ICommand SimulateAlarmCommand { get; }

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

        private bool _alarmsSuppressed;
        private int _simulateAlarmLevel;

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

            // Subscribe to IO alarm events globally so alarms are caught
            // even when no inspection is running
            SYSTEM_IO.IO_CHANGE_Handler += OnIOChangeOfState;

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
            SimulateAlarmCommand = new RelayCommand(OnSimulateAlarm);
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
            _alarmsSuppressed = false;
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
                OnAlarm = () => HandleAlarm(),
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

        private void OnIOChangeOfState(int channel, bool high)
        {
            if (channel != SYSTEM_IO.ALARM)
                return;

            if (_alarmsSuppressed)
                return;

            HandleAlarm();
        }

        private void HandleAlarm()
        {
            try
            {
                // Stop processing
                IsInspecting = false;
                App.IsInspecting = false;

                // Read error registers from PLC
                var errorsPresent = 0;
                _mxClient.ReadRegister(1, "Errors_Present", ref errorsPresent, 3);

                var errors = new List<string>();
                if (errorsPresent > 0)
                    errors = _mxClient.ErrorRegisters(1);

                if (errors.Count > 0)
                {
                    var alarmDescription = "";
                    foreach (var err in errors)
                    {
                        if (!string.IsNullOrEmpty(err))
                        {
                            alarmDescription = $"Active Alarm: {err} : {PLCFailCodes.GetDescription(err)}";

                            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                            {
                                AlarmsText += $"\n{alarmDescription}";
                                SystemMessages.Add($"[Alarm] {alarmDescription}");
                            });
                        }
                    }

                    // Capture e-signature on UI thread
                    var lastAlarmDesc = alarmDescription;
                    System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        var reasonDesc = _dataManager.GetMeaning(Enums.ESigReason.EndReelAlarm);
                        var esigWindow = new ESignatureWindow(
                            Enums.ESigReason.EndReelAlarm,
                            reasonDesc,
                            lastAlarmDesc,
                            reasonRequired: true,
                            canCancel: false);
                        esigWindow.ShowDialog();

                        // Audit trail with e-signature details
                        _dataManager.SaveAction(
                            "Inspection cancelled due to an alarm",
                            "Inspection Alarm",
                            "",
                            esigWindow.LastUserName,
                            "HandleAlarm()",
                            lastAlarmDesc,
                            esigWindow.UserReason);
                    });

                    // Reset PLC alarms
                    _mxClient.ResetAlarm(1);
                    _mxClient.Stop(1);

                    // Suppress further alarms until next inspection starts
                    _alarmsSuppressed = true;
                }
                else
                {
                    System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        AlarmsText += "\nAlarm triggered (no error details available).";
                    });

                    _alarmsSuppressed = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                {
                    AlarmsText += $"\nAlarm handler error: {ex.Message}";
                    SystemMessages.Add($"[Error] HandleAlarm: {ex.Message}");
                });
            }
        }

        private void OnSimulateAlarm(object? parameter)
        {
            _simulateAlarmLevel++;
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");

            if (_simulateAlarmLevel == 1)
            {
                // LEVEL 1: Direct property set from UI thread.
                // If this text does NOT appear, the binding/DataContext is broken.
                AlarmsText += $"\n[{timestamp}] DEBUG L1: Direct AlarmsText set from UI thread.";
            }
            else
            {
                // LEVEL 2: Fire the full handler chain.
                _alarmsSuppressed = false;
                AlarmsText += $"\n[{timestamp}] DEBUG L2: Invoking OnIOChangeOfState(ALARM, true)...";
                OnIOChangeOfState(SYSTEM_IO.ALARM, true);
                _simulateAlarmLevel = 0;
            }
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
