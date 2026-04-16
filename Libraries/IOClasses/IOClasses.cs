using Automation.BDaq;
using CONSTANTS;
using System.Diagnostics;
using static LVS3.Delegates;
using Serilog;
using ILogger = Serilog.ILogger;


namespace LVS3
{

    public static class SYSTEM_IO
    {
        private static ILogger _logger = Log.ForContext(typeof(SYSTEM_IO));
        public static event IO_INTERRUPT_Handler IO_INTERRUPT_Handler;
        public static event IO_CHANGE_Handler IO_CHANGE_Handler;

        private static int FAIL = 0;
        private static int END_OF_REEL = 1;

        private static int m_ALARM = 8;
        private static int m_INPUT_FROM_PLC = 0;
        private static int m_STOP_INSPECTION = 0;
        private static int m_END_OF_INSPECTION = 4;
        private static int m_OUTPUT_TO_CAMERA = 3;
        //private static int m_WebLamp = 4;
        //private static int m_Stop = 1;
        //
        private static string IODeviceName = "";
        public static string FailDescription = "";
        private static InstantDiCtrl InCtrl = new InstantDiCtrl();
        private static InstantDoCtrl OutCtrl = new InstantDoCtrl();
        //public static int ReadyChannel { get { return m_Ready; } }
        public static int ALARM { get { return m_ALARM; } }
        public static int PULSE_INPUT { get { return m_INPUT_FROM_PLC; } }
        public static int STOP_INSPECTION { get { return m_STOP_INSPECTION; } }
        public static int END_OF_INSPECTION { get { return m_END_OF_INSPECTION; } }
        public static int OUTPUT_TO_CAMERA { get { return m_OUTPUT_TO_CAMERA; } }
        //public static int WebLamp { get { return m_WebLamp; } }
        public static bool PROCESSING = false;
        public static int PARTIAL = 0;
        public static bool CAPTURING = false;
        //public static int STOP { get { return m_Stop; } }
        private static IDataManager DataManager;
        private static IUtilityFunctions UtilityFunctions;

        public static bool Init( IDataManager dataManager, IUtilityFunctions utilityFunctions)
        {
            _logger.Information("Init called with IDataManager and IUtilityFunctions");
            bool retVal = false;
            DataManager = dataManager;
            UtilityFunctions = utilityFunctions;
            try
            {
                if (InCtrl != null)
                    InCtrl.Cleanup();
                InCtrl = null;
                InCtrl = new InstantDiCtrl();
                if (OutCtrl != null)
                    OutCtrl.Cleanup();
                OutCtrl = null;
                OutCtrl = new InstantDoCtrl();
                FailDescription = "";
                IODeviceName = Convert.ToString(Defaults.IODeviceXML);
                if (initInput() && initOutput())
                {
                    InCtrl.SnapStart();
                    retVal = true;
                }
                else
                {
                    string ProcessNav = getProcessesLikeNavigator();
                    string looksLikeNavigatorIsRunning = "";
                    if (ProcessNav != "")
                        looksLikeNavigatorIsRunning = string.Format("\n\nIt appears that {0} is running - please close it and try running the Vision System again.", ProcessNav);
                    string err = string.Format("Failed to initialize the IO card:\n\n{0}\n\nPlease check card is not in use by another application{1}", IODeviceName, looksLikeNavigatorIsRunning);
                    _logger.Error(err);
                    UtilityFunctions.WriteLog(err, EventLogEntryType.Error, "Read IO Status");
                    UtilityFunctions.WriteLog(err + "\nvision application exited, automatic stop, because IO functions are not available or could not be initialized.", EventLogEntryType.Warning, "Camera Initialization");
                    UtilityFunctions.DoApplicationShutdown();
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                FailDescription = ex.Message;
                _logger.Error(ex, "Exception in Init: {Message}", ex.Message);
                UtilityFunctions.WriteLog("init IO: " + FailDescription, System.Diagnostics.EventLogEntryType.Error, "IO Initialize");
            }
            _logger.Information("Init returning {RetVal}", retVal);
            return retVal;
        }

        public static string getProcessesLikeNavigator()
        {
            _logger.Information("getProcessesLikeNavigator called");
            string retVal = "";
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    if (process.MainWindowTitle.IndexOf("e", StringComparison.InvariantCulture) > -1)
                    {
                        retVal = process.MainWindowTitle;
                        if (retVal.Contains("Navigator Framework") || retVal.Contains("Navigator Explorer"))
                        {
                            _logger.Information("Navigator process found: {Process}", retVal);
                            return "Navigator Explorer";
                        }
                        else
                            retVal = "";
                    }
                    else
                        retVal = "";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in getProcessesLikeNavigator: {Message}", ex.Message);
                retVal = "";
            }
            _logger.Information("getProcessesLikeNavigator returning {RetVal}", retVal);
            return retVal;
        }

        public static int GetInputStatePortAndChannel(int port, int channel)
        {
            _logger.Information("GetInputStatePortAndChannel called with port={Port}, channel={Channel}", port, channel);
            int retVal = -1;
            byte portData = 0;
            ErrorCode err = ErrorCode.ErrorUndefined;
            try
            {
                err = InCtrl.ReadBit(port, channel, out portData);
                if (err == ErrorCode.Success)
                    retVal = Convert.ToInt16(portData.ToString("X2"));
                else
                    retVal = -1;
            }
            catch (Exception ex)
            {
                PROCESSING = false;
                retVal = -1;
                _logger.Error(ex, "Exception in GetInputStatePortAndChannel: {Message}", ex.Message);
                UtilityFunctions.WriteLog("GetIOChannelStatePort0: " + ex.Message, System.Diagnostics.EventLogEntryType.Error, "IO Read Error");
            }
            _logger.Information("GetInputStatePortAndChannel returning {RetVal}", retVal);
            return retVal;
        }

        static bool IOControlFailed(ErrorCode err)
        {
            _logger.Information("IOControlFailed called with ErrorCode={ErrorCode}", err);
            return err < ErrorCode.Success && err >= ErrorCode.ErrorHandleNotValid;
        }

        public static bool IODeviceRefresh()
        {
            _logger.Information("IODeviceRefresh called");
            bool retVal = true;
            try
            {
                string profilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceIO.xml");
                ErrorCode errorCode = InCtrl.LoadProfile(profilePath);
                if (IOControlFailed(errorCode))
                {
                    _logger.Error("Refresh IO card failed with error code {ErrorCode}", errorCode);
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                FailDescription = FailDescription + "IODeviceRefresh() err: " + ex.Message + ". ";
                _logger.Error(ex, "Exception in IODeviceRefresh: {Message}", ex.Message);
            }
            _logger.Information("IODeviceRefresh returning {RetVal}", retVal);
            return retVal;
        }

        private static bool initInput()
        {
            _logger.Information("initInput called");
            bool retVal = true;
            try
            {
                string profilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceIO.xml");
                InCtrl.SelectedDevice = new DeviceInformation(IODeviceName);
                DeviceInformation devinf = InCtrl.SelectedDevice;
                devinf.DeviceMode = AccessMode.ModeWriteShared;
                InCtrl.ChangeOfState -= new EventHandler<DiSnapEventArgs>(InCtrl_ChangeOfState);
                InCtrl.ChangeOfState += new EventHandler<DiSnapEventArgs>(InCtrl_ChangeOfState);
                InCtrl.Interrupt -= new EventHandler<DiSnapEventArgs>(InCtrl_Interrupt);
                InCtrl.Interrupt += new EventHandler<DiSnapEventArgs>(InCtrl_Interrupt);
                InCtrl.SelectedDevice = devinf;
                ErrorCode errorCode = InCtrl.LoadProfile(profilePath);
                if (IOControlFailed(errorCode))
                {
                    _logger.Error("IO card profile path error");
                    throw new Exception("IO card profile path?");
                }
                DiintChannel[] diintChannels = InCtrl.DiintChannels;
                if (diintChannels != null)
                {
                    if (diintChannels.Length >= 2)
                    {
                        diintChannels[0].Enabled = true;
                        diintChannels[1].Enabled = true;
                    }
                }
                else
                {
                    retVal = false;
                    FailDescription = "No Input channels were detected!\nIO cannot function without IO.\nPlease check IO card is not in use.";
                    _logger.Error(FailDescription);
                    throw new Exception(FailDescription);
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                FailDescription = FailDescription + "initInput() err: " + ex.Message + ". ";
                _logger.Error(ex, "Exception in initInput: {Message}", ex.Message);
            }
            _logger.Information("initInput returning {RetVal}", retVal);
            return retVal;
        }

        private static void handleError(ErrorCode err)
        {
            _logger.Information("handleError called with ErrorCode={ErrorCode}", err);
            if ((err >= ErrorCode.ErrorHandleNotValid) && (err != ErrorCode.Success))
            {
                FailDescription = FailDescription + "IO Error code: " + err.ToString() + ". ";
                _logger.Error("IO Error code: {ErrorCode}", err);
            }
        }

        private static void InCtrl_ChangeOfState(object sender, DiSnapEventArgs e)
        {
            _logger.Information("InCtrl_ChangeOfState called, SrcNum={Port}", e.SrcNum);
            try
            {
                int port = e.SrcNum;
                byte portData = e.PortData[port];
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (sw.ElapsedMilliseconds < 50)
                        ;

                    // ALARM is channel 8 = port 1, bit 0
                    if (port == 1 && (portData & 0x01) != 0)
                    {
                        bool channelHigh = (GetInputStatePortAndChannel(1, 0)) == 1;
                        _logger.Information("ALARM change-of-state detected, channelHigh={High}", channelHigh);
                        IO_CHANGE_Handler?.Invoke(ALARM, channelHigh);
                    }
                    // END_OF_INSPECTION is channel 4 = port 0, bit 4
                    else if (port == 0 && (portData & (1 << m_END_OF_INSPECTION)) != 0)
                    {
                        IO_CHANGE_Handler?.Invoke(m_END_OF_INSPECTION, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Exception in InCtrl_ChangeOfState inner: {Message}", ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in InCtrl_ChangeOfState: {Message}", ex.Message);
            }
        }

        private static void InCtrl_Interrupt(object sender, DiSnapEventArgs e)
        {
            _logger.Information("InCtrl_Interrupt called, SrcNum={Channel}", e.SrcNum);
            bool IsChannelSet = false;
            int channel = e.SrcNum;
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < 100)
                    ;
                if (channel == PULSE_INPUT)
                {
                    IsChannelSet = (e.PortData[0] != 0);
                    if (IsChannelSet == false)
                        return;
                    if (IO_INTERRUPT_Handler != null)
                    {
                        IOEventArgs ioe = new IOEventArgs(PULSE_INPUT, IsChannelSet);
                        IO_INTERRUPT_Handler(PULSE_INPUT, ioe);
                    }
                }
                else if (channel == ALARM)
                {
                    bool channelHigh = (GetInputStatePortAndChannel(1, 0)) == 1;
                    _logger.Information("ALARM interrupt detected, channelHigh={High}", channelHigh);
                    IO_CHANGE_Handler?.Invoke(ALARM, channelHigh);
                }
            }
            catch (Exception ex)
            {
                string err = "\nInCtrl_Interrupt() err:" + ex.Message;
                FailDescription = FailDescription + err.ToString() + ". ";
                _logger.Error(ex, "Exception in InCtrl_Interrupt: {Message}", ex.Message);
            }
        }

        private static bool initOutput()
        {
            _logger.Information("initOutput called");
            bool retVal = true;
            try
            {
                OutCtrl.SelectedDevice = new DeviceInformation(IODeviceName);
                DeviceInformation devinf = OutCtrl.SelectedDevice;
                OutCtrl.SelectedDevice = devinf;
            }
            catch (Exception ex)
            {
                retVal = false;
                FailDescription = FailDescription + "initOutput() err: " + ex.Message + ". ";
                _logger.Error(ex, "Exception in initOutput: {Message}", ex.Message);
            }
            _logger.Information("initOutput returning {RetVal}", retVal);
            return retVal;
        }

        public static void SetChannelState(int channel, bool high)
        {
            _logger.Information("SetChannelState called with channel={Channel}, high={High}", channel, high);
            try
            {
                byte b = Convert.ToByte(0xff);
                if (high == false)
                    b = Convert.ToByte(0x00);
                OutCtrl.WriteBit(0, channel, b);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in SetChannelState: {Message}", ex.Message);
                UtilityFunctions.WriteLog("Port[0] channel[" + channel.ToString() + "]: " + ex.Message, System.Diagnostics.EventLogEntryType.Warning, "SetChannelState");
            }
        }

        public static bool GetChannelState(int channel)
        {
            _logger.Information("GetChannelState called with channel={Channel}", channel);
            byte b = 0;
            try
            {
                b = Convert.ToByte(0x00);
                ErrorCode ec = InCtrl.ReadBit(0, channel, out b);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in GetChannelState: {Message}", ex.Message);
                UtilityFunctions.WriteLog("GetChannelState(), Port[0], channel[" + channel.ToString() + "]\n" + ex.Message, System.Diagnostics.EventLogEntryType.Warning, "GetChannelState");
            }
            bool result = b == Convert.ToByte(0xff);
            _logger.Information("GetChannelState returning {Result}", result);
            return result;
        }

        public static void REEL_END()
        {
            _logger.Information("REEL_END called");
            try
            {
                Stopwatch sw = new Stopwatch();
                SYSTEM_IO.SetChannelState(END_OF_REEL, true);
                sw.Start();
                while (sw.ElapsedMilliseconds < 50)
                    ;
                SYSTEM_IO.SetChannelState(END_OF_REEL, false);
                sw.Stop();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in REEL_END: {Message}", ex.Message);
                UtilityFunctions.WriteLog("REEL_END() err: " + ex.Message, System.Diagnostics.EventLogEntryType.Warning, "GetChannelState");
            }
        }

        public static void FAIL_OCCURED()
        {
            _logger.Information("FAIL_OCCURED called");
            try
            {
                Stopwatch sw = new Stopwatch();
                if (sw.IsRunning)
                    sw.Reset();
                sw.Start();
                SYSTEM_IO.SetChannelState(FAIL, true);
                while (sw.ElapsedMilliseconds < 50)
                    ;
                SYSTEM_IO.SetChannelState(FAIL, false);
                sw.Stop();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in FAIL_OCCURED: {Message}", ex.Message);
                UtilityFunctions.WriteLog("FAIL_OCCURED(), err: " + ex.Message, System.Diagnostics.EventLogEntryType.Warning, "GetChannelState");
            }
        }

        public static void PASS_OCCURED()
        {
            _logger.Information("PASS_OCCURED called");
            try
            {
                Stopwatch sw = new Stopwatch();
                if (sw.IsRunning)
                    sw.Reset();
                sw.Start();
                SYSTEM_IO.SetChannelState(FAIL, true);
                while (sw.ElapsedMilliseconds < 50)
                    ;
                SYSTEM_IO.SetChannelState(FAIL, false);
                sw.Stop();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in PASS_OCCURED: {Message}", ex.Message);
                UtilityFunctions.WriteLog("PASS_OCCURED(), err: " + ex.Message, System.Diagnostics.EventLogEntryType.Warning, "GetChannelState");
            }
        }
    }

/*
    public static class HeartBeat
    {
        private static InstantDoCtrl HeartbeatOut = new InstantDoCtrl();
        private static int m_heartbeatchannel = 0;
        private static int m_duration = 1000;
        private static System.Threading.Timer HeartbeatTimer;
        private static bool m_beating = false;
        private static byte outValue = 0x00;
        private static int m_port = 0;
        public static string FailDescription = "";
        public static bool Beating
        {
            get { return m_beating; }
            set { m_beating = value; }
        }

        public static bool init(int Port, int IOChannel, int Duration)
        {
            bool retVal = true;
            try
            {
                if (HeartbeatTimer != null)
                {
                    HeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    HeartbeatTimer = null;
                }
                HeartbeatTimer = new System.Threading.Timer(new TimerCallback(Heartbeat_tick));
                HeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                m_heartbeatchannel = IOChannel;
                m_duration = Duration;
                m_port = Port;
                retVal = initOutput();
            }
            catch (Exception ex)
            {
                retVal = false;
                FailDescription = "init() err: " + ex.Message;
            }
            finally
            {
                m_beating = retVal;
            }
            return retVal;
        }

        private static void Heartbeat_tick(object state)
        {
            try
            {
                if (outValue == 0xFF)
                    outValue = 0x00;
                else
                    outValue = 0xFF;
                ErrorCode ec = HeartbeatOut.WriteBit(m_port, m_heartbeatchannel, outValue);
                if (ec != ErrorCode.Success)
                {
                    try { Stop(); } catch { }
                    m_beating = false;
                    if (Start() == false)
                    {
                        FailDescription = "Heartbeat has stopped with error: " + ec.ToString();
                        UtilityFunctions.WriteLog(FailDescription, System.Diagnostics.EventLogEntryType.Error, "No Heartbeat");
                        //FormManager.Instance().SetMenuState("mnuStartHeartbeat", true); ;
                    }

                }
            }
            catch (Exception ex)
            {
                m_beating = false;
                FailDescription = "Heartbeat has stopped: " + ex.Message;
                UtilityFunctions.WriteLog(FailDescription, System.Diagnostics.EventLogEntryType.Error, "No Heartbeat");
                try
                {
                    Stop();
                }
                catch { }
            }
        }

        public static bool Stop()
        {
            bool retVal = true;
            byte b = 0x00;
            try
            {
                FailDescription = "";
                if (HeartbeatOut != null)
                    HeartbeatOut.WriteBit(m_port, m_heartbeatchannel, b);
                m_beating = false;
            }
            catch (Exception ex)
            {
                retVal = false;
                FailDescription = ex.Message;
            }
            finally
            {
                if (HeartbeatTimer != null)
                    HeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            return retVal;
        }

        public static bool Start()
        {
            bool retVal = true;
            try
            {
                m_beating = false;
                FailDescription = "";
                m_port = 0; // Settings.application.Instance().hbPort;
                //m_heartbeatchannel = Settings.application.Instance().hbChannel;
                m_heartbeatchannel = 0;
                m_duration = 1000;

                ErrorCode ec = HeartbeatOut.WriteBit(m_port, m_heartbeatchannel, outValue);
                if (ec != ErrorCode.Success)
                {
                    m_beating = false;
                    Stop();
                    FailDescription = "Heartbeat Start() err: " + ec.ToString();
                    retVal = false;
                }
                else
                {
                    m_beating = true;
                    HeartbeatTimer.Change(m_duration, m_duration);
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                FailDescription = ex.Message;
                System.Windows.Forms.MessageBox.Show("Heartbeat error: " + ex.Message);
            }
            finally
            {
                m_beating = retVal;
            }
            return retVal;
        }

        private static bool initOutput()
        {
            bool retVal = true;
            try
            {
                if (HeartbeatOut != null)
                    HeartbeatOut.Cleanup();
                HeartbeatOut.SelectedDevice = new DeviceInformation(Defaults.IODeviceXML);
                DeviceInformation devinf = HeartbeatOut.SelectedDevice;
                //devinf.Description = "PCIE-1730,BID#0";
                devinf.Description = Defaults.IODeviceXML;
                devinf.DeviceMode = AccessMode.ModeWriteShared;
                devinf.DeviceNumber = 0;
                devinf.ModuleIndex = 0;
                HeartbeatOut.SelectedDevice = devinf;
                m_beating = false;
            }
            catch (Exception ex)
            {
                retVal = false;
                FailDescription = "Heartbeat: initOutput() error: " + ex.Message;
                System.Windows.Forms.MessageBox.Show("Error initializing heartbeat: " + FailDescription);
                UtilityFunctions.WriteLog(FailDescription, EventLogEntryType.Error, "No HeartBeat From VS");
            }
            finally
            {
                m_beating = retVal;
            }
            return retVal;
        }
    }
*/
}




