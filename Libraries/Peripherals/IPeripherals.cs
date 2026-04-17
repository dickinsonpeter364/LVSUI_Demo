using System.Net;

namespace LVS3;

public interface IPeripherals
{
    Delegates.SystemMessageHandler SMH { get; set; }
    Delegates.DeviceConfigMessageHandler DCMH { get; set; }
    List<IDevice> Devices { get; }
    IPAddress LocalIP { get; }
    string LocalPort { get; }
    void LoadLocalSettings();
    bool LoadDevices();
}