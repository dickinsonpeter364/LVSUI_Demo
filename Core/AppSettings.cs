namespace WpfMvvmApp.Core;

public class AppSettings
{
    public string DatabaseType { get; set; } = "dummy";
    public string MxClient { get; set; } = "dummy";
    public bool SaveImages { get; set; }
    public string SavedImagesPath { get; set; } = "";
    public int StationID { get; set; }
    public string IPAddressPLC { get; set; } = "";
    public bool DebugMode { get; set; }
    public bool BypassSecurity { get; set; }
    public bool CaptureOnly { get; set; }
}

public class DatabaseSettings
{
    public string SqlitePath { get; set; } = "";
}
