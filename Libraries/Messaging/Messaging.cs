using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using CONSTANTS;
using Serilog;
using static LVS3.Delegates;

namespace LVS3;

/// <summary>
/// Static messaging hub. Replaces the legacy WinForms-coupled Messaging class.
/// Routes notifications via the MSGHandler event and writes to Windows EventLog.
/// No WinForms dependencies (ToolStripLabel, Form, Image, MessageBox removed).
/// </summary>
public static class Messaging
{
    public static event MSGHandler? MH;
    public static EventLog? AppEventLog = null;
    public static List<ArchivedNotification> NotificationHistoryList = new();

    public static void Init()
    {
        try
        {
            AppEventLog = new EventLog();
            AppEventLog.Source = "MVA";
            AppEventLog.Log = Defaults.AppTitle;

            if (!EventLog.SourceExists("MVA"))
            {
                var escd = new EventSourceCreationData("MVA", Defaults.AppTitle);
                EventLog.CreateEventSource(escd);
            }

            NotificationHistoryList = new List<ArchivedNotification>();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Messaging.Init() error: {Message}", ex.Message);
        }
    }

    public static void Notify(string msg, EventLogEntryType iconType, string title, bool showDialog = false)
    {
        try
        {
            int appEventId = MapEventId(iconType);
            var severity = MapSeverity(iconType);
            string timestamp = DateTime.UtcNow.ToString("HH:mm:ss");

            MH?.Invoke(new MSGEventArgs(msg, iconType, showDialog));

            if (AppEventLog != null && !string.IsNullOrEmpty(msg) && iconType != EventLogEntryType.Information)
            {
                try
                {
                    if (EventLog.SourceExists(AppEventLog.Source))
                    {
                        string user = (Defaults.CurrentUser ?? "").PadRight(30);
                        string paddedTitle = (title ?? "").PadRight(50);
                        AppEventLog.WriteEntry(user + paddedTitle + msg, iconType, appEventId);
                    }
                }
                catch { }
            }

            AddHistoryItem(msg, iconType, title);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Messaging.Notify() error: {Message}", ex.Message);
        }
    }

    public static void ActivityLog(string msg, EventLogEntryType iconType, string title)
    {
        try
        {
            int appEventId = MapEventId(iconType);

            MH?.Invoke(new MSGEventArgs(msg, iconType, false));

            if (AppEventLog != null)
            {
                try
                {
                    if (EventLog.SourceExists(AppEventLog.Source))
                    {
                        string user = (Defaults.CurrentUser ?? "").PadRight(30);
                        string paddedTitle = (title ?? "").PadRight(50);
                        AppEventLog.WriteEntry(user + paddedTitle + msg, iconType, appEventId);
                    }
                }
                catch { }
            }

            AddHistoryItem(msg, iconType, title);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Messaging.ActivityLog() error: {Message}", ex.Message);
        }
    }

    public static void AddHistoryItem(string msg, EventLogEntryType iconType, string title)
    {
        try
        {
            var severity = MapSeverity(iconType);
            var notification = new ArchivedNotification(
                title: title,
                user: Defaults.CurrentUser ?? "",
                body: msg,
                severity: severity,
                time: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            NotificationHistoryList.Add(notification);
        }
        catch { }
    }

    public static void WriteLog(string title, string msg, EventLogEntryType iconType, int myApplicationEventId)
    {
        try
        {
            if (Defaults.WriteToLog == false)
                return;
            if (AppEventLog != null && EventLog.SourceExists(AppEventLog.Source))
            {
                string user = (Defaults.CurrentUser ?? "").PadRight(30);
                string paddedTitle = (title ?? "").PadRight(50);
                AppEventLog.WriteEntry(user + paddedTitle + msg, iconType, myApplicationEventId);
            }
        }
        catch { }
    }

    public static List<ArchivedNotification> SelectEventsByDate(DateTime start)
    {
        var results = new List<ArchivedNotification>();
        try
        {
            var endTime = start.AddMinutes(1439);
            string query = string.Format(
                "*[System[TimeCreated[@SystemTime >= '{0}' and @SystemTime <= '{1}']]]",
                start.ToUniversalTime().ToString("o"),
                endTime.ToUniversalTime().ToString("o"));

            var eventQuery = new EventLogQuery(Defaults.AppTitle, PathType.LogName, query);
            using var reader = new EventLogReader(eventQuery);

            EventRecord? record;
            while ((record = reader.ReadEvent()) != null)
            {
                using (record)
                {
                    string desc = record.FormatDescription() ?? "";
                    string user = desc.Length >= 28 ? desc[..28].Trim() : "";
                    string eventTitle = desc.Length >= 79 ? desc[29..79].Trim() : "";
                    string eventMsg = desc.Length > 80 ? desc[80..].Trim() : "";

                    var severity = record.Id switch
                    {
                        1001 => NotificationSeverity.Error,
                        2001 or 4001 => NotificationSeverity.Warning,
                        3001 => NotificationSeverity.Information,
                        _ => NotificationSeverity.Error
                    };

                    results.Add(new ArchivedNotification(
                        eventTitle, user, eventMsg, severity,
                        record.TimeCreated?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""));
                }
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "SelectEventsByDate() error: {Message}", ex.Message);
        }
        return results;
    }

    private static int MapEventId(EventLogEntryType iconType) => iconType switch
    {
        EventLogEntryType.Error => 1001,
        EventLogEntryType.Warning => 2001,
        EventLogEntryType.Information => 3001,
        _ => 4001
    };

    private static NotificationSeverity MapSeverity(EventLogEntryType iconType) => iconType switch
    {
        EventLogEntryType.Error => NotificationSeverity.Error,
        EventLogEntryType.Warning => NotificationSeverity.Warning,
        EventLogEntryType.Information => NotificationSeverity.Information,
        _ => NotificationSeverity.Warning
    };
}

public enum NotificationSeverity
{
    Information,
    Warning,
    Error
}

/// <summary>
/// Archived notification record. Replaces legacy ArchivedNotification
/// that used System.Drawing.Image for the icon.
/// </summary>
public class ArchivedNotification
{
    public string Title { get; }
    public string User { get; }
    public string Body { get; }
    public NotificationSeverity Severity { get; }
    public string TimeMessage { get; }

    public ArchivedNotification(string title, string user, string body,
        NotificationSeverity severity, string time)
    {
        Title = title?.Trim() ?? "";
        User = user?.Trim() ?? "";
        Body = body?.Trim() ?? "";
        Severity = severity;
        TimeMessage = time;
    }
}
