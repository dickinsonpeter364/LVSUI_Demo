using System.Management;
using System.Runtime.InteropServices;
using static LVS3.Enums;
using static LVS3.Delegates;

namespace LVS3
{


    public partial class uscScanner : UserControl, IStatusInformation
    {
        private string statusLevel = "";
        private StatusLevel SL = new StatusLevel((int)StatusLevels.INFORMATION, "");
        public static SystemMessageHandler SMH;
        private string m_componentName = "";
        //private PeripheralType pt;
        private DeviceConfig m_dS;
        public string ComponentName => m_componentName;
        public DeviceConfig DS => m_dS;
        public UserControl uscControl => this;
        private bool m_dummy = false;
        public bool DummyDevice { get => m_dummy;  }
        private USBLib USBLibrary;
        private List<USBLib.DeviceProperties> ListOfUSBDeviceProperties;
        private bool USBDeviceConnected;
        private uint m_Scanner_VID = 0X00;
        private uint m_Scanner_PID = 0X00;
        private uint m_Scanner_MI = 0X00;
        private IUtilityFunctions UtilityFunctions;

        public uscScanner(IUtilityFunctions utilityFunctions)
        {
            InitializeComponent();
            UtilityFunctions = utilityFunctions;
        }

        public void InitScanner(DeviceConfig ds)
        {
            m_dS = ds;
            this.lblComponentName.Text = ds.name;
            m_componentName = ds.name;

            USBLibrary = new USBLib();
            USBLibrary.USBDeviceAttached += new USBLib.USBDeviceEventHandler(USBPort_USBDeviceAttached);
            USBLibrary.USBDeviceRemoved += new USBLib.USBDeviceEventHandler(USBPort_USBDeviceRemoved);

            ListOfUSBDeviceProperties = new List<USBLib.DeviceProperties>();
            //DeviceConfig tmp = OracleDatabase.GetScannerDeviceData();
            m_Scanner_PID = uint.Parse(ds.product, System.Globalization.NumberStyles.AllowHexSpecifier);
            m_Scanner_VID = uint.Parse(ds.vendor, System.Globalization.NumberStyles.AllowHexSpecifier);
            InitializeDeviceTextBoxes(ds);
            USBLibrary.RegisterForDeviceChange(true, this.Handle);
            USBTryDeviceConnection();
            this.Enabled = true;
        }

        private void USBPort_USBDeviceAttached(object sender, USBLib.USBDeviceEventArgs e)
        {
            USBTryDeviceConnection();
        }

        private void InitializeDeviceTextBoxes(DeviceConfig tmp)
        {
            if (tmp.SUCCESS)
            {
                this.lblComponentName.Text = tmp.name;
                //this.lblPID.Text = m_Scanner_PID.ToString("X4") + "|" + m_Scanner_VID.ToString("X4");
                SL.Details = "...";
                SL.Status = (int)StatusLevels.INFORMATION;
                statusLevel = UtilityFunctions.DoStatusGUI(SL, lblComponentName, false);
                SystemMessageEventArgs smea = new SystemMessageEventArgs(SL.Details, "Scanner Initialization", (int)Enums.CriticalLevels.Black);
                SMH?.Invoke(smea);
            }
            else
            {
                SL.Details = "Initialization error\n" + tmp.name;
                SL.Status = (int)StatusLevels.WARNING;
                statusLevel = UtilityFunctions.DoStatusGUI(SL, lblComponentName, false);
                this.lblComponentName.Text = "Scanner not detected.";
                SystemMessageEventArgs smea = new SystemMessageEventArgs(SL.Details, "Scanner Initialization", (int)Enums.CriticalLevels.Red);
                SMH?.Invoke(smea);
            }
        }

        private void UpdateTextBoxes(bool arrived)
        {
            if (arrived)
            {
                SL.Details = "Scanner Connected";
                SL.Status = (int)StatusLevels.INFORMATION;
                statusLevel = UtilityFunctions.DoStatusGUI(SL, lblComponentName, false);
                SystemMessageEventArgs smea = new SystemMessageEventArgs(SL.Details, m_dS.name + "Status", (int)Enums.CriticalLevels.Black);
                SMH?.Invoke(smea);
            }
            else
            {
                SL.Details = "Scanner Disconnected";
                SL.Status = (int)StatusLevels.WARNING;
                statusLevel = UtilityFunctions.DoStatusGUI(SL, lblComponentName, false);
                SystemMessageEventArgs smea = new SystemMessageEventArgs(SL.Details, m_dS.name + "Status", (int)Enums.CriticalLevels.Amber);
                SMH?.Invoke(smea);
            }
        }

        public bool DeviceConnected()
        {
            bool retVal = true;
            try
            {
                USBTryDeviceConnection();
            }
            catch //(Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        private bool USBTryDeviceConnection()
        {
            try
            {
                Nullable<UInt32> MI = 0;
                MI = m_Scanner_MI;//uint.Parse(MITextBox.Text, System.Globalization.NumberStyles.AllowHexSpecifier);
                if (USBLib.GetUSBDevice(uint.Parse(m_Scanner_VID.ToString("X4"), System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse(m_Scanner_PID.ToString("X4"), System.Globalization.NumberStyles.AllowHexSpecifier), ref ListOfUSBDeviceProperties, false, MI))
                {
                    USBDeviceConnected = true;
                    UpdateTextBoxes(true);
                }
                else
                {
                    USBDeviceConnected = false;
                    UpdateTextBoxes(false);
                }
            }
            catch (Exception ex)
            {
                USBDeviceConnected = false;
                MessageBox.Show(ex.Message);
            }
            return USBDeviceConnected;
        }

        private void USBPort_USBDeviceRemoved(object sender, USBLib.USBDeviceEventArgs e)
        {
            USBDeviceConnected = false;
            if (!USBLib.GetUSBDevice(uint.Parse(m_Scanner_VID.ToString("X4"), System.Globalization.NumberStyles.AllowHexSpecifier), uint.Parse(m_Scanner_PID.ToString("X4"), System.Globalization.NumberStyles.AllowHexSpecifier), ref ListOfUSBDeviceProperties, false))
            {
                // Device is removed
                USBDeviceConnected = false;
                UpdateTextBoxes(false);
            }
            else
            {
                USBDeviceConnected = true;
                UpdateTextBoxes(true);
            }
        }

        protected override void WndProc(ref Message m) //IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr wParam = m.WParam;
            IntPtr lParam = m.LParam;
            bool handled = false;
            if (USBLibrary != null)
                USBLibrary.ProcessWindowsMessage(m.Msg, wParam, lParam, ref handled);
            base.WndProc(ref m);
        }

        private bool HoneywellScannerInstalled()
        {
            bool retVal = false;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPSignedDriver");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string s;
                    if (obj == null)
                        continue;
                    try
                    {
                        s = string.IsNullOrEmpty(obj.GetPropertyValue("DeviceName").ToString()) ? string.Empty : obj.GetPropertyValue("DeviceName").ToString();
                    }
                    catch { s = ""; }

                    if (s.Contains("Honeywell") && s.Contains("Barcode"))
                        retVal = true;
                }

            }
            catch (Exception ex)
            {
                string err = "HoneywellScannerInstalled() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, m_dS.name + "Status", (int)Enums.CriticalLevels.Red);
                SMH?.Invoke(smea);
            }
            return retVal;
        }

        private int nEventType = 0;

        public void ScannerWndProc(ref Message m)
        {
            int wparam = 0;
            if (m.WParam != null)
            {
                wparam = m.WParam.ToInt32();
            }

            if (m.Msg == Dbt.WM_DEVICECHANGE)
            {
                // Get the message event type
                if (nEventType == m.WParam.ToInt32())
                    return;
                else
                    nEventType = m.WParam.ToInt32();

                // Check for devices being connected or disconnected
                if (nEventType == Dbt.DBT_DEVICEARRIVAL ||
                    nEventType == Dbt.DBT_DEVICEREMOVECOMPLETE)
                {
                    Dbt.DEV_BROADCAST_HDR hdr = new Dbt.DEV_BROADCAST_HDR();
                    // Convert lparam to DEV_BROADCAST_HDR structure
                    Marshal.PtrToStructure(m.LParam, hdr);
                    if (hdr.dbch_devicetype == Dbt.DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        Dbt.DEV_BROADCAST_DEVICEINTERFACE_1 devIF = new Dbt.DEV_BROADCAST_DEVICEINTERFACE_1();
                        // Convert lparam to DEV_BROADCAST_DEVICEINTERFACE structure
                        Marshal.PtrToStructure(m.LParam, devIF);
                        // Get the device path from the broadcast message
                        string devicePath = new string(devIF.dbcc_name);
                        // Remove null-terminated data from the string
                        int pos = devicePath.IndexOf((char)0);
                        if (pos != -1)
                            devicePath = devicePath.Substring(0, pos);

                        // A HID device was connected or removed
                        if (nEventType == Dbt.DBT_DEVICEREMOVECOMPLETE)
                        {
                            USBDeviceConnected = false;
                            UpdateTextBoxes(false);
                        }
                        else if (nEventType == Dbt.DBT_DEVICEARRIVAL)
                        {
                            USBDeviceConnected = true;
                            UpdateTextBoxes(true);
                        }
                    }
                }
            }
        }

        public bool ComponentReady()
        {
            return USBDeviceConnected;
        }

        public void SetStatusInformation(StatusLevel state)
        {
            statusLevel = state.Details;
            UtilityFunctions.DoStatusGUI(state, lblComponentName, false);
            int level = (state.Status == 1) ? 2 : (state.Status == 1) ? 2 : 0;
            SystemMessageEventArgs smea = new SystemMessageEventArgs(state.Details, m_dS.name + " Status", level);
            SMH?.Invoke(smea);
        }
    }

    class Dbt
    {
        #region Dbt Class - Constants
        public const ushort WM_DEVICECHANGE = 0x0219;
        public const ushort DBT_DEVICEARRIVAL = 0x8000;
        public const ushort DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const ushort DBT_DEVTYP_DEVICEINTERFACE = 0x0005;
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x0000;
        #endregion

        #region Dbt Class - Device Change Structures

        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            public char dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class DEV_BROADCAST_DEVICEINTERFACE_1
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
            public char[] dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_HDR
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
        }

        #endregion

        #region DLL Imports

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

        [DllImport("user32.dll")]
        public static extern uint UnregisterDeviceNotification(IntPtr Handle);

        #endregion
    }

}
