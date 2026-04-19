using System.Drawing;
using LVS3;
using static LVS3.Enums;

namespace WpfMvvmApp.Services;

/// <summary>
/// Wraps the static CameraManager class as an injectable service.
/// </summary>
public class CameraService : ICameraService
{
    public bool LoadCameras() => CameraManager.LoadCameras();
    public bool CamerasReady => CameraManager.CamerasReady;
    public bool CamerasLoaded => CameraManager.CamerasLoaded;
    public void CloseGrabbers() => CameraManager.CloseGrabbers();
    public int GetGain() => CameraManager.GetGainFromCameras();
    public void SetGain(int gain) => CameraManager.SetGain(gain);

    public StatusLevel GetCameraStatus(CameraType cameraType) =>
        CameraManager.GetCameraStatus(cameraType);

    public void StartCapture(int cameraIndex, Action onFrameAcquired)
    {
        if (CameraManager.NectaCameras != null &&
            cameraIndex < CameraManager.NectaCameras.Length &&
            CameraManager.NectaCameras[cameraIndex] != null)
        {
            var cam = CameraManager.NectaCameras[cameraIndex];
            // Ensure the camera is acquiring (matches old GetImageInspection)
            cam.EnsureAcquiring();
            cam.GrabCameraImage(onFrameAcquired);
        }
    }

    public void StopCapture(int cameraIndex)
    {
        if (CameraManager.NectaCameras != null &&
            cameraIndex < CameraManager.NectaCameras.Length &&
            CameraManager.NectaCameras[cameraIndex] != null)
        {
            CameraManager.NectaCameras[cameraIndex].SetChannelMethodCallerFalse();
        }
    }

    public Bitmap? GetLastCameraImage(int cameraIndex)
    {
        if (CameraManager.NectaCameras != null &&
            cameraIndex < CameraManager.NectaCameras.Length &&
            CameraManager.NectaCameras[cameraIndex] != null)
        {
            return CameraManager.NectaCameras[cameraIndex].CameraImage;
        }
        return null;
    }
}
