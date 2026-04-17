using System.ComponentModel;
using static LVS3.Enums;
using static LVS3.Delegates;

namespace LVS3
{
    public partial class uscCamera : UserControl, IStatusInformation
    {
        private string statusLevel = "";
        private StatusLevel SL = new StatusLevel((int)StatusLevels.INFORMATION, "");
        public static SystemMessageHandler SMH;

        private DeviceConfig m_dS;
        public string ComponentName => m_dS.name;
        public DeviceConfig DS => m_dS;
        public UserControl uscControl => this;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DummyDevice { get; set; } = false;
        private IDataManager DataManager;
        private IUtilityFunctions UtilityFunctions;

        public uscCamera(DeviceConfig ds, IDataManager dataManager, IUtilityFunctions utilityFunctions)
        {
            InitializeComponent();
            m_dS = ds;
            DataManager = dataManager;
            UtilityFunctions = utilityFunctions;

            initCamera();
        }

        private void initCamera()
        {
            lblComponentName.Text = m_dS.name;
            string address = "IP / Port: " + m_dS.ip.ToString() + ": " + m_dS.port;
            SystemMessageEventArgs smea = new SystemMessageEventArgs(address, m_dS.name + " Status", (int)CriticalLevels.Black);
            SMH?.Invoke(smea);

            this.lblComponentName.Text = m_dS.name;
            SL.Details = address;
            SL.Status = (int)StatusLevels.INFORMATION;
            statusLevel = UtilityFunctions.DoStatusGUI(SL, lblComponentName, false);
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
