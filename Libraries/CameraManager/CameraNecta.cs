using System;
using AlkUSB3;
using Serilog;
using static LVS3.Delegates;
using CONSTANTS;
using static LVS3.Enums;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;


namespace LVS3
{
    public class CameraManager
    {
        public static object? BackingCamControl = null;
        public static SystemMessageHandler SMH;
        public static IUtilityFunctions UtilityFunctions;
        #region public static data
        public static bool CamerasLoaded = false;
        public static int CameraCount = 0;

        public static CameraNecta[] NectaCameras;
        public static CameraAria[] AriaCameras;
        #endregion



        public static int GetGainFromCameras()
        {
            int retVal = 0;
            try
            {
                if (NectaCameras != null)
                {
                    foreach (CameraNecta cam in NectaCameras)
                    {
                        if (cam != null)
                        {
                            try { retVal = (int)cam.nectaCam.Gain; } catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "GetGainFromCameras() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Setup Inspection Camera", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
            }
            return retVal;
        }

        public static void SetGain(int gain)
        {
            try
            {

                if (NectaCameras != null)
                {
                    foreach (CameraNecta cam in NectaCameras)
                    {
                        if (cam != null)
                        {
                            try { cam.nectaCam.Gain = (uint)gain; } catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "SetGain() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Setup Inspection Camera", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
            }
        }

        public static bool AdjustGain(Bitmap? img)
        {
            bool retVal = true;
            try
            {
                if (img == null) return retVal;

                // Count pixels at max brightness (255) to detect overexposure
                int overexposedCount = 0;
                var bmpData = img.LockBits(new Rectangle(200, 200, Math.Max(1, img.Width - 300), Math.Max(1, img.Height - 300)),
                    ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
                unsafe
                {
                    byte* ptr = (byte*)bmpData.Scan0;
                    for (int y = 0; y < bmpData.Height; y++)
                    {
                        byte* row = ptr + y * bmpData.Stride;
                        for (int x = 0; x < bmpData.Width; x++)
                            if (row[x] == 255) overexposedCount++;
                    }
                }
                img.UnlockBits(bmpData);

                if (overexposedCount > 100000)
                {
                    if (NectaCameras != null)
                        foreach (CameraNecta cam in NectaCameras)
                            if (cam != null)
                                try { cam.nectaCam.Gain = (uint)Defaults.CameraGain; } catch { }
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "AdjustGain() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static void CloseGrabbers()
        {
            if (NectaCameras != null)
            {
                foreach (CameraNecta cam in NectaCameras)
                {
                    if (cam != null)
                    {
                        try { cam.CloseGrabber(); } catch { }
                        cam.GrabberOpen = false;
                    }
                }
            }
            if (AriaCameras != null)
            {
                foreach (CameraAria cam in AriaCameras)
                {
                    if (cam != null)
                    {
                        try { cam.CloseGrabber(); } catch { }
                        cam.GrabberOpen = false;
                    }
                }
            }
        }

        public static bool CamerasReady
        {
            get
            {
                if (NectaCameras == null || NectaCameras.Length == 0)
                    return false;
                foreach (CameraNecta cam in NectaCameras)
                {
                    if (cam == null)
                        return false;
                    if (cam.GrabberOpen == false)
                        return false;
                }
                return true;
            }
        }

        public static bool LoadCameras()
        {
            bool retVal = false;
            try
            {
                CamerasLoaded = false;
                CameraCount = 1;
                retVal = CreateCameras(CameraCount);

                if (retVal == true)
                {
                    SystemMessageEventArgs smea = new SystemMessageEventArgs("LoadCameras(): Camera Loaded - OK!", "Load Camera", (int)CriticalLevels.Black);
                    SMH?.Invoke(smea);
                }
                else
                {
                    string err = string.Format("LoadCameras()\n\nLoading Camera Problem:\nNo USB Camera could be initialized!\nPlease check USB connections");
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Initialize Inspection Camera", (int)CriticalLevels.Red);
                    SMH?.Invoke(smea);
                    UtilityFunctions.DoApplicationShutdown();
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "LoadCameras() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Initialize Inspection Camera", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                UtilityFunctions.DoApplicationShutdown();
            }
            finally
            {
                CamerasLoaded = retVal;
            }
            return retVal;
        }

        public static bool CreateCameras(int numCameras)
        {
            bool retVal = false;
            CloseGrabbers();
            retVal = OpenGrabbersNecta(numCameras);
            if (retVal == true)
            {
                return OpenGrabbersAria(numCameras);
            }
            return retVal;
        }

        private static bool OpenGrabbersNecta(int numcameras)
        {
            bool retVal = false;
            CameraNecta cam;
            NectaCameras = new CameraNecta[numcameras];
            NectaCamera NCList = new NectaCamera();
            string[] camList = NCList.GetCameraList();
            NCList.Close();
            if (camList.Length != numcameras)
            {
                string err = string.Format("\n{0}\n\nThe list of attached Inspection cameras does not match the number expected!\n\nCamera count expected: {1}, camera count listed: {2}.", string.Join("\n", camList), numcameras, camList.Length);
                Log.Logger.Error(err);
                UtilityFunctions.DoApplicationShutdown();
            }
            else
            {
                for (int x = 0; x < CameraCount; x++)
                {
                    cam = new CameraNecta(x);
                    NectaCameras[x] = cam;
                    if (cam.OpenGrabber(x) == false)
                    {
                        string err = string.Format("Could Not Load Inspection Camera {0}!", camList[x]);
                        Log.Logger.Error(err);
                        UtilityFunctions.DoApplicationShutdown();
                    }
                    else
                    {
                        cam.nectaCam.Init();
                        cam.nectaCam.Load(0);
                    }
                }
                retVal = true;
            }
            return retVal;
        }

        public static StatusLevel GetCameraStatus(CameraType camtype)
        {
            var sl = new StatusLevel();
            try
            {
                if (camtype == CameraType.NECTACAM)
                {
                    if (NectaCameras == null || NectaCameras.Length == 0 ||
                        NectaCameras[0] == null || NectaCameras[0].CameraConnected() == false)
                    {
                        sl.Details = "Inspection Camera is not present";
                        sl.Status = (int)Enums.StatusLevels.ERROR;
                    }
                    else
                    {
                        sl.Details = "Inspection Camera is present";
                        sl.Status = (int)Enums.StatusLevels.INFORMATION;
                    }
                }
                else
                {
                    if (AriaCameras == null || AriaCameras.Length == 0 ||
                        AriaCameras[0] == null || AriaCameras[0].CameraConnected() == false)
                    {
                        sl.Details = "Backing Camera is not present";
                        sl.Status = (int)Enums.StatusLevels.WARNING;
                    }
                    else
                    {
                        sl.Details = "Backing Camera is present";
                        sl.Status = (int)Enums.StatusLevels.INFORMATION;
                    }
                }
            }
            catch { }
            return sl;
        }

        private static bool OpenGrabbersAria(int numcameras)
        {
            bool retVal = false;
            CameraAria cam = null;
            AriaCameras = new CameraAria[numcameras];
            AriaCamera NCList = new AriaCamera();
            string[] camList = NCList.GetCameraList();
            NCList.Close();
            if (camList.Length != numcameras)
            {
                string err = string.Format("\n{0}\n\nThe list of attached backing cameras does not match the number expected!\n\nCamera count expected: {1}, camera count listed: {2}.", string.Join("\n", camList), numcameras, camList.Length);
                Log.Logger.Error(err);
                UtilityFunctions.DoApplicationShutdown();
            }
            else
            {
                for (int x = 0; x < CameraCount; x++)
                {
                    cam = new CameraAria(x);
                    AriaCameras[x] = cam;
                    if (cam.OpenGrabber(x) == false)
                    {
                        string err = string.Format("Could Not Load Backing Camera {0}!", camList[x]);
                        Log.Logger.Error(err);
                        UtilityFunctions.DoApplicationShutdown();
                    }
                    if (cam.AriaCam.Acquire == true)
                        cam.AriaCam.Acquire = false;
                    cam.AriaCam.Init();
                    cam.AriaCam.Load(0);

                }
                retVal = true;
            }
            return retVal;
        }

        #region Camera Classes

        public class CameraAria : IDisposable
        {

            public int DeviceID { get => m_ID; set => m_ID = value; }
            protected internal int m_ID = -1;
            internal protected string CAMERA_ERROR_MESSAGE { get; set; }

            private int m_ImageHeight { get; set; }
            private int m_ImageRight { get; set; }
            private string m_deviceName { get; set; }
            private string m_aliasName { get; set; }
            private string m_FailDescription { get; set; }
            private bool m_grabberOpen { get; set; }
            public string DeviceName { get => m_deviceName; set => m_deviceName = value; }
            public string AliasName { get => m_aliasName; set => m_aliasName = value; }
            public bool GrabberOpen { get => m_grabberOpen; set => m_grabberOpen = value; }
            public string FailDescription { get => m_FailDescription; set => m_FailDescription = value; }
            public AriaCamera AriaCam { get => m_ariaCam; set => m_ariaCam = value; }

            private int bufferIndex = 0;
            public RawDataResult dataResult = RawDataResult.IDLE;
            private AriaCamera m_ariaCam = null;
            private Bitmap? m_CameraImage = null;
            public Bitmap? CameraImage { get => m_CameraImage; set => m_CameraImage = value; }
            private uint imgX = 0;
            private uint imgY = 0;
            public uint ImgX { get => imgX; }
            public uint ImgY { get => imgY; set => imgY = value; }

            

            public CameraAria(int id)
            {
                DeviceID = id;
                m_grabberOpen = false;
                CAMERA_ERROR_MESSAGE = "";
            }

            public bool CameraConnected()
            {
                bool retVal = true;
                if (this.m_ariaCam.Camera == -1)
                {
                    string err = this.AliasName + " is not connected.Please exit vision system and check camera data connections.";
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Camera Connection", (int)CriticalLevels.Red);
                    CameraManager.SMH(smea);
                    Log.Logger.Error(err);
                    return false;
                }
                return retVal;
            }

            public void cam_Aria_RawFrameAcquired(object sender, EventArgs e)
            {
                BufferPtr ptr = null;
                try
                {
                    ptr = m_ariaCam.GetImagePtr(false);
                    if (ptr != null)
                    {
                        if (m_CameraImage != null)
                            m_CameraImage.Dispose();
                        m_CameraImage = extractBitmap(ptr);
                        dataResult = RawDataResult.COMPLETE;
                        channelMethodCaller?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "[AriaCam] cam_Aria_RawFrameAcquired err: {Message}", ex.Message);
                }
                finally
                {
                    if (ptr != null)
                        ptr.Dispose();
                }
            }


            public void GrabCameraImage(Action channelcaller, bool requiresoftwaretrigger)
            {
                string err = "";
                try
                {
                    bufferIndex = 0;
                    channelMethodCaller = channelcaller;
                    dataResult = RawDataResult.WAITING;
                    if (m_CameraImage != null)
                        m_CameraImage.Dispose();
                    m_CameraImage = null;
                    if (m_ariaCam.Acquire == false)
                        m_ariaCam.Acquire = true;
                    if (requiresoftwaretrigger == true)
                        m_ariaCam.SoftwareTrigger();
                }
                catch (Exception ex)
                {
                    err = "GrabCameraImage() err:\n" + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Grab Image", (int)CriticalLevels.Red);
                    CameraManager.SMH?.Invoke(smea);
                }
            }


            public void FlushInterrupt()
            {
                try
                {
                    m_ariaCam.FrameCombinerFlush();
                }
                catch (Exception ex)
                {
                    string err = "FlushInterrupt() err:\n" + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Flush Images", (int)CriticalLevels.Red);
                    CameraManager.SMH(smea);
                }
            }

            private Bitmap? extractBitmap(BufferPtr ptr)
            {
                Bitmap? retVal = null;
                try
                {
                    if (ptr == null)
                        throw new Exception(string.Format("Null image / data ptr is null.\nImage data buffer[{0}] null pointer", bufferIndex));
                    int width = (int)m_ariaCam.ImageSizeX;
                    int height = ptr.PartialCounter > 0 ? (int)ptr.PartialCounter : (int)m_ariaCam.ImageSizeY;

                    retVal = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                    ColorPalette palette = retVal.Palette;
                    for (int i = 0; i < 256; i++) palette.Entries[i] = Color.FromArgb(i, i, i);
                    retVal.Palette = palette;

                    // First pass: read entire image and find min/max pixel values
                    // so we can auto-stretch contrast to 0-255, matching Halcon's
                    // HSmartWindowControl display behaviour.
                    byte[] raw = new byte[width * height];
                    for (int y = 0; y < height; y++)
                        Marshal.Copy(ptr.Body() + y * width, raw, y * width, width);

                    byte min = 255, max = 0;
                    for (int i = 0; i < raw.Length; i++)
                    {
                        if (raw[i] < min) min = raw[i];
                        if (raw[i] > max) max = raw[i];
                    }

                    // Second pass: stretch min..max -> 0..255 (unless flat image)
                    if (max > min)
                    {
                        int range = max - min;
                        for (int i = 0; i < raw.Length; i++)
                            raw[i] = (byte)(((raw[i] - min) * 255) / range);
                    }

                    BitmapData bmpData = retVal.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                    for (int y = 0; y < height; y++)
                        Marshal.Copy(raw, y * width, bmpData.Scan0 + y * bmpData.Stride, width);
                    retVal.UnlockBits(bmpData);
                }
                catch (Exception ex)
                {
                    retVal?.Dispose();
                    retVal = null;
                    string err = "extractBitmap() err:\n" + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Extract Images", (int)CriticalLevels.Red);
                    CameraManager.SMH(smea);
                }
                finally
                {
                    ptr?.Dispose();
                }
                return retVal;
            }

            private Action channelMethodCaller = null;

            #region old_methods
            public bool CloseGrabber()
            {
                bool retVal = false;
                try
                {
                    if (m_ariaCam.Acquire == true)
                        m_ariaCam.Acquire = false;
                    m_ariaCam.Close();
                    m_grabberOpen = false;
                    retVal = true;
                }
                catch (Exception ex)
                {
                    CAMERA_ERROR_MESSAGE = "CloseGrabber(): " + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(CAMERA_ERROR_MESSAGE, "Close Camera", (int)CriticalLevels.Red);
                    CameraManager.SMH(smea);
                }
                m_grabberOpen = false;
                return retVal;
            }

            public void cam_CameraUnplugged(object sender, EventArgs e)
            {
                SYSTEM_IO.PROCESSING = false;
            }

            public void cam_CameraListChanged(object sender, EventArgs e)
            {
                SYSTEM_IO.PROCESSING = false;
            }


            public bool OpenGrabber(int x)
            {
                bool retVal = false;
                try
                {
                    if(m_ariaCam!=null)
                        try { m_ariaCam.Close(); } catch { }
                    m_ariaCam = new AriaCamera();
                    m_ariaCam.Camera = x;
                    m_ariaCam.Init();
                    m_ariaCam.Load(0);
                    m_aliasName = "Backing Camera";
                    m_deviceName = m_ariaCam.Name;
                    m_ariaCam.RawFrameAcquired -= new EventHandler(cam_Aria_RawFrameAcquired);
                    m_ariaCam.RawFrameAcquired += new EventHandler(cam_Aria_RawFrameAcquired);
                    // LiveControl for live preview is not used in WPF;
                    // frames are captured via RawFrameAcquired event instead
                    m_ariaCam.Acquire = true;
                    m_grabberOpen = true;
                    retVal = true;
                }
                catch (Exception ex)
                {
                    retVal = false;
                    Log.Logger.Error("OpenGrabber() err: {Error} Could not connect to or configure backing camera: {Serial}", ex.Message, m_ariaCam.SerialNumber);
                }
                return retVal;
            }

            public void Dispose()
            {
                try { m_ariaCam.Close(); } catch { }
            }

            #endregion
        }

        public class CameraNecta : IDisposable
        {

            public int DeviceID { get => m_ID; set => m_ID = value; }
            protected internal int m_ID = -1;
            internal protected string CAMERA_ERROR_MESSAGE { get; set; }

            private string m_deviceName { get; set; }
            private string m_aliasName { get; set; }
            private string m_FailDescription { get; set; }
            private bool m_grabberOpen { get; set; }
            public string DeviceName { get => m_deviceName; set => m_deviceName = value; }
            public string AliasName { get => m_aliasName; set => m_aliasName = value; }
            public bool GrabberOpen { get => m_grabberOpen; set => m_grabberOpen = value; }

            public NectaCamera nectaCam { get => m_nectaCam; set => m_nectaCam = value; }

            private int bufferIndex = 0;
            //public RawDataResult dataResult = RawDataResult.IDLE;
            private uint NUM_OF_FRAMES = 1;
            private BufferPtr[] buffers;
            private NectaCamera m_nectaCam = null;
            private Bitmap? m_CameraImage = null;
            public Bitmap? CameraImage { get => m_CameraImage; set => m_CameraImage = value; }            
            public static int imageIndexToSave = 0;

            public CameraNecta(int id)
            {
                DeviceID = id;
                m_grabberOpen = false;
                CAMERA_ERROR_MESSAGE = "";
            }

            public bool CameraConnected()
            {
                bool retVal = true;
                if (this.m_nectaCam.Camera == -1)
                {
                    string err = this.AliasName + " is not connected.Please exit vision system and check camera data connections.";
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Camera Connection", (int)CriticalLevels.Red);
                    CameraManager.SMH(smea);
                    Log.Logger.Error(err);
                    return false;
                }
                return retVal;
            }

            public void cam_RawFrameAcquired(object sender, EventArgs e)
            {
                Log.Logger.Information("[NectaCam] cam_RawFrameAcquired fired. channelMethodCaller={Null}",
                    channelMethodCaller == null ? "null" : "set");
                string err = "";

                BufferPtr ptr = null;
                try
                {
                    if (channelMethodCaller == null)
                    {
                        Log.Logger.Warning("[NectaCam] cam_RawFrameAcquired dropped: no callback registered");
                        return;
                    }
                    ptr = m_nectaCam.GetRawDataPtr(false);
                    if (ptr != null)
                    {
                        if (m_CameraImage != null)
                            m_CameraImage.Dispose();
                        m_CameraImage = extractBitmap(ptr);

                        Log.Logger.Information("[NectaCam] Frame captured, invoking callback");
                        channelMethodCaller?.Invoke();
                    }
                    else
                    {
                        err = string.Format("{0} : Null/no image returned in buffer", DeviceName);
                        Log.Logger.Warning("[NectaCam] {Err}", err);
                        SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Image Acquire", (int)CriticalLevels.Red);
                        CameraManager.SMH(smea);
                    }
                }
                catch (Exception ex)
                {
                    int id = m_nectaCam.Camera;
                    err = "cam_RawFrameAcquired() err:\n" + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Image Buffering", (int)CriticalLevels.Red);
                    CameraManager.SMH?.Invoke(smea);
                }
                finally
                {
                    if (ptr != null)
                        ptr.Dispose();
                    ptr = null;
                }
            }           

            public void FlushInterrupt()
            {
                try
                {
                    m_nectaCam.FrameCombinerFlush();
                }
                catch (Exception ex)
                {
                    string err = "FlushInterrupt() err:\n" + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Flush Images", (int)CriticalLevels.Red);
                    CameraManager.SMH(smea);
                }
            }

            private Bitmap? extractBitmap(BufferPtr ptr)
            {
                Bitmap? retVal = null;
                try
                {
                    if (ptr == null)
                        throw new Exception(string.Format("Null image / data ptr is null.\nImage data buffer[{0}] null pointer", bufferIndex));
                    int width = (int)m_nectaCam.ImageSizeX;
                    int height = ptr.PartialCounter > 0 ? (int)ptr.PartialCounter : (int)m_nectaCam.ImageSizeY;

                    retVal = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                    ColorPalette palette = retVal.Palette;
                    for (int i = 0; i < 256; i++) palette.Entries[i] = Color.FromArgb(i, i, i);
                    retVal.Palette = palette;

                    BitmapData bmpData = retVal.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                    byte[] rawRow = new byte[width];
                    for (int y = 0; y < height; y++)
                    {
                        Marshal.Copy(ptr.Body() + y * width, rawRow, 0, width);
                        Marshal.Copy(rawRow, 0, bmpData.Scan0 + y * bmpData.Stride, width);
                    }
                    retVal.UnlockBits(bmpData);
                }
                catch (Exception ex)
                {
                    retVal?.Dispose();
                    retVal = null;
                    string err = "extractBitmap() err:\n" + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Extract Images", (int)CriticalLevels.Red);
                    CameraManager.SMH(smea);
                }
                finally
                {
                    if (ptr != null)
                        ptr.Dispose();
                }
                return retVal;
            }

            private Action channelMethodCaller = null;

            /// <summary>
            /// Ensures the camera is actively acquiring. Matches old
            /// GetImageInspection() behaviour which set Acquire=true before
            /// registering the frame callback.
            /// </summary>
            public void EnsureAcquiring()
            {
                try
                {
                    if (m_nectaCam != null && m_nectaCam.Acquire == false)
                    {
                        m_nectaCam.Acquire = true;
                        Log.Logger.Information("[NectaCam] EnsureAcquiring: Acquire set to true");
                    }
                    else
                    {
                        Log.Logger.Information("[NectaCam] EnsureAcquiring: already acquiring ({Acq})",
                            m_nectaCam?.Acquire);
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "[NectaCam] EnsureAcquiring failed: {Message}", ex.Message);
                }
            }

            public void GrabCameraImage(Action channelcaller)
            {
                string err = "";
                try
                {
                    bufferIndex = 0;
                    channelMethodCaller = channelcaller;
                    if (m_CameraImage != null)
                        m_CameraImage.Dispose();
                    m_CameraImage = null;
                    Log.Logger.Information("[NectaCam] GrabCameraImage: callback registered. Acquire={Acq}",
                        m_nectaCam?.Acquire);
                }
                catch (Exception ex)
                {
                    err = "GrabCameraImage() err:\n" + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Grab Image", (int)CriticalLevels.Red);
                    CameraManager.SMH?.Invoke(smea);
                }
            }

            public void SetChannelMethodCallerFalse()
            {
                string err = "";
                try
                {
                    channelMethodCaller = null;
                    if (m_CameraImage != null)
                        m_CameraImage.Dispose();
                    m_CameraImage = null;
                }
                catch (Exception ex)
                {
                    err = "SetChannelMethodCallerFalse() err:\n" + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "GrabChunk Image", (int)CriticalLevels.Red);
                    CameraManager.SMH?.Invoke(smea);
                }
            }

            private void allocateFramesForGrab(int numimages)
            {
                // Allocate some buffers so that, when we call GetRawDataPtr(), the dll frame buffer queue is
                // ready to pass acquired buffers without the need to allocating new ones.
                // This is not mandatory, however it can significantly increase performances.
                m_nectaCam.AllocRawFrames(NUM_OF_FRAMES);
                try
                {
                    NUM_OF_FRAMES = (uint)numimages;
                    m_nectaCam.AcquisitionBurstLength = NUM_OF_FRAMES;
                    buffers = new BufferPtr[NUM_OF_FRAMES];
                    m_nectaCam.AllocRawFrames(NUM_OF_FRAMES);
                    bufferIndex = 0;
                }
                catch (Exception ex)
                {
                    CAMERA_ERROR_MESSAGE = "allocateFramesForGrab(): " + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(CAMERA_ERROR_MESSAGE, "Extract Images", (int)CriticalLevels.Red);
                    CameraManager.SMH(smea);
                }
            }

            #region old_methods
            public bool CloseGrabber()
            {
                bool retVal = false;
                try
                {
                    if (m_nectaCam.Acquire == true)
                        m_nectaCam.Acquire = false;
                    m_nectaCam.Close();
                    m_grabberOpen = false;
                    retVal = true;
                }
                catch (Exception ex)
                {
                    CAMERA_ERROR_MESSAGE = "CloseGrabber(): " + ex.Message;
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(CAMERA_ERROR_MESSAGE, "Close Camera", (int)CriticalLevels.Red);
                    CameraManager.SMH(smea);
                }
                m_grabberOpen = false;
                return retVal;
            }

            public void cam_CameraUnplugged(object sender, EventArgs e)
            {
                SYSTEM_IO.PROCESSING = false;
            }

            public void cam_CameraListChanged(object sender, EventArgs e)
            {
                SYSTEM_IO.PROCESSING = false;
            }

            public bool OpenGrabber(int x)
            {
                bool retVal = false;
                try
                {
                    m_nectaCam = new NectaCamera();
                    m_nectaCam.Camera = x;
                    m_aliasName = "Inspection Camera";
                    m_deviceName = m_nectaCam.Name;
                    retVal = OpenChunkMode();
                }
                catch (Exception ex)
                {
                    retVal = false;
                    Log.Logger.Error("OpenGrabber() err: {Error} Could not connect to or configure {Serial} USB3 camera.", ex.Message, m_nectaCam.SerialNumber);
                }
                return retVal;
            }

            private bool OpenChunkMode()
            {
                bool retVal = false;
                try
                {
                    if (nectaCam.Acquire == true)
                        nectaCam.Acquire = false;
                    nectaCam.Init();
                    nectaCam.Load(0);

                    m_nectaCam.PreserveRates = false;
                    m_nectaCam.RawFrameAcquired -= new EventHandler(cam_RawFrameAcquired);
                    m_nectaCam.RawFrameAcquired += new EventHandler(cam_RawFrameAcquired);
                    m_nectaCam.EnableImageThread = false;
                    //imgX = m_nectaCam.ImageSizeX;
                    //imgY = m_nectaCam.ImageSizeY;
                    allocateFramesForGrab(1);
                    m_nectaCam.Acquire = true;
                    m_grabberOpen = true;
                    retVal = true;
                }
                catch (Exception ex)
                {
                    retVal = false;
                    string err = "openForChunkMode() err:\n" + ex.Message;
                    Log.Logger.Error(err + " Could not open {Serial} USB3 camera for ChunkData access.", m_nectaCam.SerialNumber);
                }
                return retVal;
            }

            public void Dispose()
            {
                try { nectaCam.Close(); } catch { }
            }
            #endregion
        }
        #endregion
    }
}



