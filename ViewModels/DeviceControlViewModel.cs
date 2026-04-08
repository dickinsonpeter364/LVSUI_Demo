using System.Windows.Input;
using LVS3;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WpfMvvmApp.Core;
using WpfMvvmApp.Core.Messages;
using WpfMvvmApp.Services;

namespace WpfMvvmApp.ViewModels
{
    public enum DeviceControlMode
    {
        Normal,
        PostLogin
    }

    public enum MotionDirection
    {
        None,
        Forward,
        Reverse
    }

    public class DeviceControlViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ImxClient _mxClient;
        private readonly IMessagingService _messaging;
        private readonly DeviceControlMode _mode;
        private readonly Action? _onContinue;

        private const int PLC_ID = 1;
        private const int PLC_ATTEMPTS = 3;
        private const int INITIAL_FWD_SPEED = 30;
        private const int INITIAL_RWD_SPEED = 5;
        private const int MIN_SPEED = 10;
        private const int MAX_SPEED = 154;

        public DeviceControlViewModel(INavigationService navigationService,
            DeviceControlMode mode = DeviceControlMode.Normal, Action? onContinue = null)
        {
            _navigationService = navigationService;
            _mxClient = App.Services.GetRequiredService<ImxClient>();
            _messaging = App.Services.GetRequiredService<IMessagingService>();
            _mode = mode;
            _onContinue = onContinue;

            _speed = MIN_SPEED;

            ForwardCommand = new RelayCommand(OnForward, _ => !IsRunning && !HasAlarm);
            ReverseCommand = new RelayCommand(OnReverse, _ => !IsRunning && !HasAlarm);
            StopCommand = new RelayCommand(OnStop, _ => IsRunning);
            ResetAlarmCommand = new RelayCommand(OnResetAlarm, _ => HasAlarm);
            GoBackCommand = new RelayCommand(_ =>
            {
                if (CanGoBackVisible && _navigationService.CanGoBack) _navigationService.GoBack();
            }, _ => !IsRunning);
            SetRewindSizeCommand = new RelayCommand(p => RewindReelSize = p?.ToString() ?? "Small");
            SetWindSizeCommand = new RelayCommand(p => WindReelSize = p?.ToString() ?? "Small");
            ContinueToInspectionCommand = new RelayCommand(_ => _onContinue?.Invoke(), _ => !IsRunning);

            _messaging.Register<AlarmMessage>(this, OnAlarmMessage);
        }

        // --- Properties ---

        public bool IsPostLoginMode => _mode == DeviceControlMode.PostLogin;
        public bool CanGoBackVisible => _mode == DeviceControlMode.Normal;

        private MotionDirection _activeDirection = MotionDirection.None;
        public MotionDirection ActiveDirection
        {
            get => _activeDirection;
            private set
            {
                if (SetProperty(ref _activeDirection, value))
                {
                    OnPropertyChanged(nameof(IsRunning));
                    OnPropertyChanged(nameof(IsForwardRunning));
                    OnPropertyChanged(nameof(IsReverseRunning));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsRunning => _activeDirection != MotionDirection.None;
        public bool IsForwardRunning => _activeDirection == MotionDirection.Forward;
        public bool IsReverseRunning => _activeDirection == MotionDirection.Reverse;

        private double _speed;
        public double Speed
        {
            get => _speed;
            set
            {
                if (SetProperty(ref _speed, Math.Clamp(value, MIN_SPEED, MAX_SPEED)))
                    OnSpeedChanged();
            }
        }

        private string _rewindReelSize = "Small";
        public string RewindReelSize
        {
            get => _rewindReelSize;
            set
            {
                if (SetProperty(ref _rewindReelSize, value))
                    WriteReelSizeRegisters();
            }
        }

        private string _windReelSize = "Small";
        public string WindReelSize
        {
            get => _windReelSize;
            set
            {
                if (SetProperty(ref _windReelSize, value))
                    WriteReelSizeRegisters();
            }
        }

        private string _statusText = "Ready";
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        private bool _hasAlarm;
        public bool HasAlarm
        {
            get => _hasAlarm;
            set
            {
                if (SetProperty(ref _hasAlarm, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        // --- Commands ---

        public ICommand ForwardCommand { get; }
        public ICommand ReverseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ResetAlarmCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand SetRewindSizeCommand { get; }
        public ICommand SetWindSizeCommand { get; }
        public ICommand ContinueToInspectionCommand { get; }

        // --- Actions ---

        private void OnForward(object? parameter)
        {
            _mxClient.InspectionLampOn();
            Speed = INITIAL_FWD_SPEED;
            _mxClient.WriteToRegister(PLC_ID, "FWD_Manual_Speed_SP", INITIAL_FWD_SPEED, PLC_ATTEMPTS);
            _mxClient.StartForward(PLC_ID);
            ActiveDirection = MotionDirection.Forward;
            StatusText = $"Winding forward at {Speed:F0}";
        }

        private void OnReverse(object? parameter)
        {
            _mxClient.InspectionLampOn();
            _mxClient.WriteToRegister(PLC_ID, "REV_Override", 1, PLC_ATTEMPTS);
            Speed = INITIAL_RWD_SPEED;
            _mxClient.WriteToRegister(PLC_ID, "RWD_Manual_Speed_SP", INITIAL_RWD_SPEED, PLC_ATTEMPTS);
            _mxClient.StartReverse(PLC_ID);
            ActiveDirection = MotionDirection.Reverse;
            StatusText = $"Rewinding at {Speed:F0}";
        }

        private void OnStop(object? parameter)
        {
            _mxClient.Stop(PLC_ID);
            _mxClient.InspectionLampOff();
            _mxClient.WriteToRegister(PLC_ID, "REV_Override", 0, PLC_ATTEMPTS);
            ActiveDirection = MotionDirection.None;
            StatusText = "Stopped";
        }

        private void OnResetAlarm(object? parameter)
        {
            _mxClient.ResetAlarm(PLC_ID);
            HasAlarm = false;
            StatusText = "Alarm reset";
        }

        private void OnSpeedChanged()
        {
            int speedVal = (int)_speed;
            if (_activeDirection == MotionDirection.Forward)
            {
                _mxClient.WriteToRegister(PLC_ID, "FWD_Manual_Speed_SP", speedVal, PLC_ATTEMPTS);
                StatusText = $"Winding forward at {speedVal}";
            }
            else if (_activeDirection == MotionDirection.Reverse)
            {
                _mxClient.WriteToRegister(PLC_ID, "RWD_Manual_Speed_SP", speedVal, PLC_ATTEMPTS);
                StatusText = $"Rewinding at {speedVal}";
            }
        }

        private void WriteReelSizeRegisters()
        {
            // Forward reel core size
            _mxClient.WriteToRegister(PLC_ID, "FWD_Small_Core_Selected", _windReelSize == "Small" ? 1 : 0, PLC_ATTEMPTS);
            _mxClient.WriteToRegister(PLC_ID, "FWD_Large_Core_Selected", _windReelSize == "Large" ? 1 : 0, PLC_ATTEMPTS);
            // Reverse reel core size
            _mxClient.WriteToRegister(PLC_ID, "RWD_Small_Core_Selected", _rewindReelSize == "Small" ? 1 : 0, PLC_ATTEMPTS);
            _mxClient.WriteToRegister(PLC_ID, "RWD_Large_Core_Selected", _rewindReelSize == "Large" ? 1 : 0, PLC_ATTEMPTS);
        }

        private void OnAlarmMessage(AlarmMessage msg)
        {
            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                if (msg.IsActive)
                {
                    HasAlarm = true;
                    ActiveDirection = MotionDirection.None;
                    StatusText = $"ALARM on channel {msg.Channel}";
                }
            });
        }
    }
}
