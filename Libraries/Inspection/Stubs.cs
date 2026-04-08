namespace LVS3;

/// <summary>
/// Stub interfaces and classes to allow Inspection.cs to compile
/// while the deep WinForms coupling is being incrementally removed.
/// These will be replaced with proper abstractions as the migration progresses.
/// </summary>

// Stub for IPeripherals - the full version is in C:\REWINDER_ANISO\Peripherals
public interface IPeripherals
{
    Delegates.SystemMessageHandler SMH { get; set; }
    Delegates.DeviceConfigMessageHandler DCMH { get; set; }
    List<IDevice> Devices { get; }
    System.Net.IPAddress LocalIP { get; }
    string LocalPort { get; }
    void LoadLocalSettings();
    bool LoadDevices();
}

// IImageMatcher is now provided by Interop.OpenCVComMatcherLib.dll

// Stub for ISysConfig
public interface ISysConfig
{
    bool LoadAppSettings();
}

// Stub for ImageData (static helper)
public static class ImageData
{
    public static string CreateFailFilepath(string reelLpn)
    {
        var folder = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "FailImages", reelLpn);
        System.IO.Directory.CreateDirectory(folder);
        return folder;
    }
}
