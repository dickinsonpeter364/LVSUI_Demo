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
    public string IODeviceName { get; set; } = "PCM-27D24DI,BID#0";
}

public class DatabaseSettings
{
    public string SqlitePath { get; set; } = "";
}
