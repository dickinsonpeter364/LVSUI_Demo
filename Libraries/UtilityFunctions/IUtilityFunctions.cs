using System.Diagnostics;
using System.Drawing;

namespace LVS3;

/// <summary>
/// Utility functions interface. Methods that reference WinForms types (Label, etc.)
/// have been replaced with UI-agnostic signatures.
/// </summary>
public interface IUtilityFunctions
{
    void SetHalconMemoryVariables();
    void ReleaseHalconMemoryVariables();
    void HalconSystemVariables();
    object GetTextReader();
    bool MSERCheckVDE(ref Bitmap img, int vdelength, double msercontrast, ref bool countmatcherror, ref string errordescription, ref bool erroroccured, ref Bitmap connectedregions);
    void PaintRedRectangle(ref FailRecord fp, string failfolder, ref Bitmap variationregion);
    void RemoveBackground(ref Bitmap img);
    IEnumerable<T> GetValues<T>();
    bool SetSystemParams();
    bool AlignsWith(double a, double b, int within);
    bool AlignsWith(List<string> a, List<string> b);
    bool AlignsWith(int a, int b, int within);
    bool AlignsWith(int a, int b, double percent);
    void CloseRunningPLCServerInstances();
    bool IsDarkTextOnLightBackground(Bitmap imgROI);
    string OpenFile();
    void DoApplicationShutdown();
    void DoApplicationShutdown(IDevice component, string msg);
    bool PingComponent(IDevice component);
    void MessagingInit();
    void DeleteEventLog();
    void WriteLog(string msg, EventLogEntryType iconType, string heading);
}
