using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly ICameraService _cameraService;
        private readonly IAlarmService _alarmService;
        private CancellationTokenSource? _captureCts;

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand NavigateToDeviceControlCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToLabelInvestigationCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ViewAuditTrailCommand { get; }
        public ICommand ResetAlarmsCommand { get; }
        public ICommand SimulateAlarmCommand { get; }

        public string AlarmsText => _alarmService.AlarmsText;

        private void AppendAlarm(string text) => _alarmService.Append(text);

        private System.Windows.Media.ImageSource? _latestImage;
        public System.Windows.Media.ImageSource? LatestImage
        {
            get => _latestImage;
            set => SetProperty(ref _latestImage, value);
        }

        private static System.Windows.Media.ImageSource BitmapToImageSource(System.Drawing.Bitmap bmp)
        {
            using var ms = new System.IO.MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;
            var bi = new System.Windows.Media.Imaging.BitmapImage();
            bi.BeginInit();
            bi.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            bi.StreamSource = ms;
            bi.EndInit();
            bi.Freeze();
            return bi;
        }

        private void AppendInfo(string text)
        {
            if (string.IsNullOrEmpty(InfoText))
                InfoText = text;
            else
                InfoText += $"\n{text}";
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
            _cameraService = App.Services.GetRequiredService<ICameraService>();
            _alarmService = App.Services.GetRequiredService<IAlarmService>();

            _infoText = "System initialized. Ready for inspection...";

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
                _alarmService.Clear();
                _alarmsSuppressed = false;
                try { _mxClient.ResetAlarm(1); } catch { }
            });
            SimulateAlarmCommand = new RelayCommand(OnSimulateAlarm);
        }

        private void OnStart(object? parameter)
        {
            var popup = new PreInspectionWindow();
            if (popup.ShowDialog() == true && popup.Accepted)
            {
                if (!App.IsDummyMode)
                {
                    _mxClient.WriteToRegister(1, "Speed_Control", (int)popup.Speed, 3);
                    // TODO: write reel size registers to PLC
                }

                if (App.CaptureOnly)
                {
                    StartCaptureOnly();
                }
                else
                {
                    CompleteStartInspection();
                }
            }
        }

        private void StartCaptureOnly()
        {
            IsInspecting = true;
            App.IsInspecting = true;
            AppendInfo("Capture-only inspection started. Reels rolling.");

            try
            {
                // Start the reels forward
                _mxClient.StartForward(1);
            }
            catch (Exception ex)
            {
                AppendInfo($"StartForward failed: {ex.Message}");
            }

            // Initialise the dummy inspection so it knows the ReelLpn for saving
            var ctx = new InspectionContext
            {
                ReelLpn = DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                MxClient = _mxClient,
                OnInfoTextChanged = text => System.Windows.Application.Current?.Dispatcher?.Invoke(() => InfoText = text)
            };
            _inspection.InitInspection(ctx);

            _captureCts = new CancellationTokenSource();
            var token = _captureCts.Token;

            Task.Run(async () =>
            {
                int captured = 0;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        // Trigger capture on the PLC
                        _mxClient.WriteToRegister(1, "Capture_Image", 1, 3);

                        // Give hardware a moment to capture, then pull the frame
                        await Task.Delay(200, token);

                        var bmp = _cameraService.GetLastCameraImage(0);
                        if (bmp != null)
                        {
                            var fp = new FailRecord("capture", captured);
                            _inspection.InspectLabel(bmp, ref fp);

                            // Show the captured image in the UI
                            var src = BitmapToImageSource(bmp);
                            System.Windows.Application.Current?.Dispatcher?.Invoke(() => LatestImage = src);

                            captured++;

                            if (captured % 10 == 0)
                            {
                                int c = captured;
                                System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                                    AppendInfo($"Captured {c} images"));
                            }
                        }
                    }
                    catch (OperationCanceledException) { break; }
                    catch (Exception ex)
                    {
                        System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                            AppendInfo($"Capture error: {ex.Message}"));
                    }
                }
            }, token);
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
                OnError = err => AppendInfo($"Error: {err}")
            };

            _inspection.InitInspection(ctx);
            AppendInfo("Inspection Started.");
        }

        private void OnStop(object? parameter)
        {
            IsInspecting = false;
            App.IsInspecting = false;

            // Stop the capture loop if running
            _captureCts?.Cancel();
            _captureCts = null;

            _mxClient.Stop(1);

            if (App.CaptureOnly || App.IsDummyMode)
            {
                AppendInfo("Inspection stopped.");
                return;
            }

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
                            var friendly = LVS3.mxClient.GetAlarmDescription(err);
                            alarmDescription = $"Active Alarm: {friendly}";

                            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                            {
                                if (err.StartsWith("DB8.DBX22."))
                                    AppendAlarm(friendly);
                                else
                                    AppendInfo(alarmDescription);
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
                        AppendAlarm("Alarm triggered (no error details available).");
                    });

                    _alarmsSuppressed = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                {
                    AppendAlarm($"Alarm handler error: {ex.Message}");
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
                AppendAlarm($"[{timestamp}] DEBUG L1: Direct AlarmsText set from UI thread.");
            }
            else
            {
                // LEVEL 2: Fire the full handler chain.
                _alarmsSuppressed = false;
                AppendAlarm($"[{timestamp}] DEBUG L2: Invoking OnIOChangeOfState(ALARM, true)...");
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
                    AppendInfo(msg.Message);
            });
        }

        private void OnAlarmMessage(AlarmMessage msg)
        {
            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                AppendAlarm($"Alarm Ch{msg.Channel}: {(msg.IsActive ? "ACTIVE" : "cleared")}");
            });
        }

        private void OnInspectionStatus(InspectionStatusMessage msg)
        {
            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                IsInspecting = msg.IsProcessing;
                App.IsInspecting = msg.IsProcessing;
                if (msg.Result != null)
                    AppendInfo($"Label {msg.LabelIndex}: {msg.Result}");
            });
        }
    }
}
