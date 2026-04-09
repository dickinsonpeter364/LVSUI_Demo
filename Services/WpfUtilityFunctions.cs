using System.Diagnostics;
using System.Drawing;
using System.Windows;
using LVS3;
using Serilog;

namespace WpfMvvmApp.Services;

/// <summary>
/// WPF implementation of IUtilityFunctions.
/// Provides the methods needed by IOClasses, CameraManager, and other libraries.
/// HALCON methods are no-ops since HALCON has been removed.
/// </summary>
public class WpfUtilityFunctions : IUtilityFunctions
{
    public void WriteLog(string msg, EventLogEntryType iconType, string heading)
    {
        switch (iconType)
        {
            case EventLogEntryType.Error:
                Log.Logger.Error("[{Heading}] {Message}", heading, msg);
                break;
            case EventLogEntryType.Warning:
                Log.Logger.Warning("[{Heading}] {Message}", heading, msg);
                break;
            default:
                Log.Logger.Information("[{Heading}] {Message}", heading, msg);
                break;
        }

        // Also write to Windows EventLog via Messaging if initialised
        Messaging.WriteLog(heading, msg, iconType, iconType == EventLogEntryType.Error ? 1001 : 3001);
    }

    public void DoApplicationShutdown()
    {
        Log.Logger.Warning("DoApplicationShutdown called — shutting down application");
        Application.Current?.Dispatcher?.Invoke(() =>
        {
            Application.Current?.Shutdown();
        });
    }

    public void DoApplicationShutdown(IDevice component, string msg)
    {
        Log.Logger.Error("DoApplicationShutdown called for {Component}: {Message}",
            component.ComponentName, msg);
        DoApplicationShutdown();
    }

    public void MessagingInit()
    {
        Messaging.Init();
    }

    public void DeleteEventLog()
    {
        try
        {
            if (Messaging.AppEventLog != null && EventLog.SourceExists(Messaging.AppEventLog.Source))
                EventLog.Delete(Messaging.AppEventLog.Log);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "DeleteEventLog error");
        }
    }

    public bool PingComponent(IDevice component)
    {
        // Basic TCP ping for network devices
        try
        {
            if (component.DS.ip != null)
            {
                using var ping = new System.Net.NetworkInformation.Ping();
                var reply = ping.Send(component.DS.ip, 2000);
                return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
        }
        catch { }
        return false;
    }

    public string OpenFile()
    {
        // Use WPF OpenFileDialog instead of WinForms
        var dialog = new Microsoft.Win32.OpenFileDialog();
        return dialog.ShowDialog() == true ? dialog.FileName : "";
    }

    // --- HALCON methods: no-ops since HALCON has been removed ---

    public void SetHalconMemoryVariables() { }
    public void ReleaseHalconMemoryVariables() { }
    public void HalconSystemVariables() { }
    public object GetTextReader() => new TReader();

    public bool MSERCheckVDE(ref Bitmap img, int vdelength, double msercontrast,
        ref bool countmatcherror, ref string errordescription, ref bool erroroccured,
        ref Bitmap connectedregions)
    {
        // MSER text detection was HALCON-only — now handled by OpenCVComMatcherLib
        return false;
    }

    public void PaintRedRectangle(ref FailRecord fp, string failfolder, ref Bitmap variationregion)
    {
        // Was HALCON region painting — to be reimplemented with System.Drawing if needed
    }

    public void RemoveBackground(ref Bitmap img)
    {
        // Was HALCON background removal — to be reimplemented if needed
    }

    public bool IsDarkTextOnLightBackground(Bitmap imgROI)
    {
        // Simple histogram check: if average pixel > 128, background is light
        // This replaces the HALCON GrayHisto implementation
        try
        {
            if (imgROI == null) return true;
            long total = 0;
            int count = 0;
            for (int y = 0; y < imgROI.Height; y += 4) // sample every 4th pixel for speed
            {
                for (int x = 0; x < imgROI.Width; x += 4)
                {
                    total += imgROI.GetPixel(x, y).R;
                    count++;
                }
            }
            return count > 0 && (total / count) > 128;
        }
        catch { return true; }
    }

    public IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();

    public bool SetSystemParams() => true;

    public bool AlignsWith(double a, double b, int within) => Math.Abs(a - b) <= within;
    public bool AlignsWith(List<string> a, List<string> b) => a.SequenceEqual(b);
    public bool AlignsWith(int a, int b, int within) => Math.Abs(a - b) <= within;
    public bool AlignsWith(int a, int b, double percent)
    {
        if (b == 0) return a == 0;
        return Math.Abs(a - b) <= Math.Abs(b * percent / 100.0);
    }

    public void CloseRunningPLCServerInstances()
    {
        try
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("mxServer");
            foreach (var p in processes)
            {
                try { p.Kill(); } catch { }
            }
        }
        catch { }
    }
}
