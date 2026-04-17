using System.Net;
using static LVS3.Enums;
using System.Data;
using static LVS3.Delegates;

namespace LVS3
{

    public class Peripherals : IPeripherals
    {

        public SystemMessageHandler SMH { get; set; }
        public DeviceConfigMessageHandler DCMH { get; set; }

        public List<IDevice> Devices { get; } = new List<IDevice>();

        public IPAddress LocalIP { get { return m_local_ip; } }
        public string LocalPort { get { return m_local_port; } }
        private static IPAddress m_local_ip = null;
        private static string m_local_port = null;
        private static IDataManager DataManager;
        private static IUtilityFunctions UtilityFunctions;
        public Peripherals(IDataManager dataManager, IUtilityFunctions utilityFunctions )
        {
                DataManager = dataManager;
                UtilityFunctions = utilityFunctions;
        }

        public class Device : IDevice
        {
            UserControl m_uscControl = null;
            private string m_componentName = "";
            private DeviceConfig m_dS;
            public PeripheralType Type { get => m_type; }
            string IDevice.ComponentName => m_componentName;
            DeviceConfig IDevice.DS => m_dS;
            UserControl IDevice.uscControl { get => m_uscControl; set => m_uscControl = value; }

            private PeripheralType m_type = PeripheralType.NOT_ASSIGNED;
            private IUtilityFunctions UtilityFuinctions;
            public Device(DeviceConfig ds, IUtilityFunctions utilityFunctions)
            {
                m_dS = ds;
                UtilityFuinctions = utilityFunctions;
                m_componentName = ds.name;
                m_type = ds.pt;
                switch (ds.pt)
                {
                    case PeripheralType.Scanner:
                        m_uscControl = new uscScanner(UtilityFunctions);
                        break;
                    case PeripheralType.Camera:
                        switch (m_dS.group)
                        {
                            case 1:
                                m_uscControl = new uscNectaCam(m_dS, DataManager, UtilityFuinctions);
                                break;
                            case 2:
                                m_uscControl = new uscAriaCam(m_dS, DataManager, UtilityFuinctions);
                                break;
                        }

                        break;
                }
            }
        }

        public class PLCDevice : IDevice
        {
            private string m_componentName = "";
            private DeviceConfig m_dS;
            public PeripheralType Type { get => m_type; }
            string ComponentName => m_componentName;
            DeviceConfig DS => m_dS;

            string IDevice.ComponentName => m_componentName;

            DeviceConfig IDevice.DS => m_dS;

            private UserControl m_uscControl = null;
            public UserControl uscControl { get => m_uscControl; set => m_uscControl = value; }

            private PeripheralType m_type = PeripheralType.NOT_ASSIGNED;

            public PLCDevice(DeviceConfig ds)
            {
                m_dS = ds;
                m_componentName = ds.name;
                m_type = ds.pt;
                m_uscControl = new uscPLC(m_dS, DataManager, UtilityFunctions);
            }
        }
        private  bool createDeviceScanner(ref DeviceConfig ds, DataRow dr)
        {
            bool retVal = true;
            try
            {
                ds.name = dr["DEVICENAME"].ToString();
                ds.vendor = dr["VENDOR_ID"].ToString();
                ds.product = dr["PRODUCT_ID"].ToString();

                string config = "Configuring Scanner...";
                DCMH?.Invoke(config, ds.name);

                Device device = new Device(ds, UtilityFunctions);
                //((uscScanner)device.uscControl).InitScanner(ds);
                Devices.Add(device);
            }
            catch (Exception ex)
            {
                string err = "createDeviceScanner(DeviceSettings, DataRow) err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Create Device Component", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                retVal = false;
            }
            return retVal;
        }

        private  bool createDevicePLC(ref DeviceConfig ds, DataRow dr)
        {
            bool retVal = true;
            try
            {
                ds.name = dr["DEVICENAME"].ToString();
                string tmp = dr["IP_ADDRESS"].ToString();
                if (tmp != "")
                {
                    IPAddress ip = null;
                    if (IPAddress.TryParse(tmp.Trim(), out ip))
                        ds.ip = ip;
                    ds.port = dr["PORT_NUMBER"].ToString();
                }

                tmp = dr["GROUP_ID"].ToString();
                if (tmp != "")
                {
                    int id = -1;
                    if (Int32.TryParse(tmp.Trim(), out id))
                        ds.group = id;
                }
                string config = string.Format("configuring {0} PLC #{1}...", ds.name, ds.group);
                DCMH?.Invoke(config, ds.name);

                IDevice device = new PLCDevice(ds);
                Devices.Add((IDevice)device);

            }
            catch (Exception ex)
            {
                string err = "createDevicePLC(DeviceSettings, DataRow) err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Create Device Component", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                retVal = false;
            }
            return retVal;
        }


        private  bool createDeviceCameraNecta(ref DeviceConfig ds, DataRow dr)
        {//
            bool retVal = true;
            try
            {
                ds.name = dr["DEVICENAME"].ToString();
                string tmp = dr["IP_ADDRESS"].ToString();
                if (tmp != "")
                {
                    IPAddress ip = null;
                    if (IPAddress.TryParse(tmp.Trim(), out ip))
                        ds.ip = ip;
                    ds.port = dr["PORT_NUMBER"].ToString();
                }

                tmp = dr["DUMMY_DEVICE"].ToString();
                if (tmp != "")
                {
                    int dummy = 0;
                    if (int.TryParse(tmp.Trim(), out dummy))
                        ds.dummy = dummy;
                }
                tmp = dr["GROUP_ID"].ToString();
                if (tmp != "")
                {
                    int group = 0;
                    if (int.TryParse(tmp.Trim(), out group))
                        ds.group = group;
                }

                ds.alkerianame = dr["ALKERIA_NAME"].ToString();

                Device device = new Device(ds, UtilityFunctions);
                Devices.Add(device);
            }
            catch (Exception ex)
            {
                string err = "createDeviceCameraNecta() err: " + ex.Message;
                //UtilityFunctions.Notify(err, System.Diagnostics.EventLogEntryType.Error, "Create Device Component", true, null);
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Create Device Component", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                retVal = false;
            }
            return retVal;
        }

        private  bool createDeviceCameraAria(ref DeviceConfig ds, DataRow dr)
        {//
            bool retVal = true;
            try
            {
                ds.name = dr["DEVICENAME"].ToString();
                string tmp = dr["IP_ADDRESS"].ToString();
                if (tmp != "")
                {
                    IPAddress ip = null;
                    if (IPAddress.TryParse(tmp.Trim(), out ip))
                        ds.ip = ip;
                    ds.port = dr["PORT_NUMBER"].ToString();
                }

                tmp = dr["DUMMY_DEVICE"].ToString();
                if (tmp != "")
                {
                    int dummy = 0;
                    if (int.TryParse(tmp.Trim(), out dummy))
                        ds.dummy = dummy;
                }
                tmp = dr["GROUP_ID"].ToString();
                if (tmp != "")
                {
                    int group = 0;
                    if (int.TryParse(tmp.Trim(), out group))
                        ds.group = group;
                }

                ds.alkerianame = dr["ALKERIA_NAME"].ToString();

                Device device = new Device(ds, UtilityFunctions);
                Devices.Add(device);
            }
            catch (Exception ex)
            {
                string err = "createDeviceCameraAria() err: " + ex.Message;
                //UtilityFunctions.Notify(err, System.Diagnostics.EventLogEntryType.Error, "Create Device Component", true, null);
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Create Device Component", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                retVal = false;
            }
            return retVal;
        }


        public void LoadLocalSettings()
        {
            try
            {
                string tmpIP = DataManager.ApplicationSettingGet("MVAPC_IPADDRESS");
                if (!IPAddress.TryParse(tmpIP, out m_local_ip))
                    throw new Exception("System PC - IP Adress setting is missing!\n\n" + DataManager.ErrorDesription);
                else
                {
                    tmpIP = (string)DataManager.ApplicationSettingGet("MVAPC_PORT");
                    if (tmpIP != "")
                        m_local_port = tmpIP;
                    else
                        throw new Exception("System PC - Port Number setting is missing!\n\n" + DataManager.ErrorDesription);
                }
            }
            catch (Exception ex)
            {
                string err = "LoadLocalSettings() err: " + ex.Message;
                //UtilityFunctions.Notify(err, System.Diagnostics.EventLogEntryType.Error, "Create Device Component", true, null);
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Create Device Component", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
            }
        }

        public bool LoadDevices()
        {
            bool retVal = false;
            DataSet ds = null;
            try
            {
                try { Cursor.Current = Cursors.AppStarting; } catch { }
                try { ds = DataManager.GetSystemDevices(); } catch { }
                if (ds != null)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        DeviceConfig devset = new DeviceConfig(true);
                        string tmpType = dr["TYPE"].ToString();
                        devset.pt = (PeripheralType)(Convert.ToInt32(tmpType));
                        int tmpCamType = Convert.ToInt32(dr["GROUP_ID"].ToString());

                        PeripheralType pt = devset.pt;
                        switch (pt)
                        {
                            case PeripheralType.PLC:
                                createDevicePLC(ref devset, dr);
                                break;
                            case PeripheralType.Scanner:
                                createDeviceScanner(ref devset, dr);
                                break;
                            case PeripheralType.Camera:
                                if (tmpCamType == 1)
                                    createDeviceCameraNecta(ref devset, dr);
                                else
                                    createDeviceCameraAria(ref devset, dr);
                                break;
                        }
                    }
                    retVal = true;
                }
                else
                {
                    throw new Exception("No  device data received from GetSystemDevices()");
                }
            }
            catch (Exception ex)
            {
                string err = "LoadDevices() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Create Device Component", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
            }
            return retVal;
        }
    }
}


