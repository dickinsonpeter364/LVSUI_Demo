using System.Drawing;
using static LVS3.Enums;

namespace WpfMvvmApp.Services;

/// <summary>
/// Abstraction over the camera hardware.
/// </summary>
public interface ICameraService
{
    bool LoadCameras();
    bool CamerasReady { get; }
    bool CamerasLoaded { get; }
    void CloseGrabbers();
    int GetGain();
    void SetGain(int gain);

    LVS3.StatusLevel GetCameraStatus(CameraType cameraType);

    /// <summary>
    /// Starts capturing frames. Callback invoked each time a frame is acquired.
    /// </summary>
    void StartCapture(int cameraIndex, Action onFrameAcquired);

    void StopCapture(int cameraIndex);

    /// <summary>
    /// Gets the last captured image as a standard Bitmap.
    /// </summary>
    Bitmap? GetLastCameraImage(int cameraIndex);
}
