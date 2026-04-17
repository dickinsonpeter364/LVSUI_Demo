using System.ComponentModel;
using static LVS3.Delegates;
using static LVS3.Enums;

namespace LVS3
{
    public partial class uscPLC : UserControl, IStatusInformation
    {
        private string statusLevel = "";
        private StatusLevel SL = new StatusLevel((int)StatusLevels.INFORMATION, "");
        public static SystemMessageHandler SMH;
        public List<PLCRegister> PLCRegisters = new List<PLCRegister>();

        private bool m_dummy = false;
        public bool DummyDevice { get => m_dummy; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PLC_ID { get => m_plcID; set => m_plcID = value; }
        private DeviceConfig m_dS;
        private int m_plcID = -1;
        private IDataManager DataManager;
        private IUtilityFunctions UtilityFunctions;
        public uscPLC(DeviceConfig ds, IDataManager dataManager, IUtilityFunctions utilityFunctions)
        {
            InitializeComponent();
            DataManager = dataManager;
            UtilityFunctions = utilityFunctions;
            m_dS = ds;
            initPLC(ds);
        }

        private void initPLC(DeviceConfig ds)
        {
            this.m_plcID = ds.group;
            lblComponentName.Text = ds.name;
            string ipport = "IP / Port Address: " + ds.ip.ToString() + ": " + ds.port;
            SystemMessageEventArgs smea = new SystemMessageEventArgs(ipport, ds.name + " Status", (int)Enums.CriticalLevels.Black);
            SMH?.Invoke(smea);
        }

        public bool LoadRegisters(int plc_id)
        {
            bool retVal = true;
            PLCRegisters = new List<PLCRegister>();
            try
            {
                PLCRegisters = DataManager.LoadPLCRegisters(plc_id);
            }
            catch (Exception ex)
            {
                retVal = false;
                SL.Details = string.Format("PLC #{0}:\nloadPLCRegisters() err: " + ex.Message, 1);
                SL.Status = (int)StatusLevels.WARNING;
                statusLevel = UtilityFunctions.DoStatusGUI(SL, lblComponentName, false);
                SystemMessageEventArgs smea = new SystemMessageEventArgs(SL.Details, m_dS.name + " Status", (int)Enums.CriticalLevels.Red);
                SMH?.Invoke(smea);
            }
            return retVal;
        }

        public bool LoadPLCFailCodes()
        {
            bool retVal = true;
            PLCFailCodes.FailCodes = new List<PLCFailCode>();
            try
            {
                PLCFailCodes.FailCodes = DataManager.LoadFailCodes();
            }
            catch (Exception ex)
            {
                retVal = false;
                SL.Details = "LoadPLCFailCodes() err: " + ex.Message;
                SL.Status = (int)StatusLevels.WARNING;
                statusLevel = UtilityFunctions.DoStatusGUI(SL, lblComponentName, false);
                SystemMessageEventArgs smea = new SystemMessageEventArgs(SL.Details, m_dS.name + " Status", (int)Enums.CriticalLevels.Red);
                SMH?.Invoke(smea);
            }
            return retVal;
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
}

