using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WpfMvvmApp.Services;

/// <summary>
/// Singleton alarm state shared across all views. The alarms panel in
/// MainWindow binds to this service so alarms appear on every screen.
/// </summary>
public interface IAlarmService : INotifyPropertyChanged
{
    string AlarmsText { get; }
    void Append(string text);
    void Clear();
    bool Suppressed { get; set; }
}

public class AlarmService : IAlarmService
{
    private string _alarmsText = string.Empty;
    public string AlarmsText
    {
        get => _alarmsText;
        private set
        {
            if (_alarmsText == value) return;
            _alarmsText = value;
            OnPropertyChanged();
        }
    }

    public bool Suppressed { get; set; }

    public void Append(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        var next = string.IsNullOrEmpty(_alarmsText) ? text : $"{_alarmsText} | {text}";
        // Marshal to UI thread if needed (AlarmsText is bound in XAML)
        if (Application.Current?.Dispatcher != null && !Application.Current.Dispatcher.CheckAccess())
            Application.Current.Dispatcher.Invoke(() => AlarmsText = next);
        else
            AlarmsText = next;
    }

    public void Clear()
    {
        Suppressed = false;
        if (Application.Current?.Dispatcher != null && !Application.Current.Dispatcher.CheckAccess())
            Application.Current.Dispatcher.Invoke(() => AlarmsText = string.Empty);
        else
            AlarmsText = string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
