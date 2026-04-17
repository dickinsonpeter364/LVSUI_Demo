using System.ComponentModel;
using static LVS3.Delegates;
using static LVS3.Enums;

namespace LVS3
{
    public partial class uscNectaCam : UserControl, IStatusInformation
    {
        private string statusLevel = "";
        private StatusLevel SL = new StatusLevel((int)StatusLevels.INFORMATION, "");
        public static SystemMessageHandler SMH;

        private string m_componentName = "";
        private string m_alkeriaName = "";
        public string AlkeriaName => m_alkeriaName;
        private DeviceConfig m_dS;
        public string ComponentName => m_componentName;
        public DeviceConfig DS => m_dS;
        public UserControl uscControl => this;

        private bool m_dummy = false;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DummyDevice { get => m_dummy; set => m_dummy = value; }
        private IDataManager DataManager;
        private IUtilityFunctions UtilityFunctions;


        public uscNectaCam(DeviceConfig ds, IDataManager dataManager, IUtilityFunctions utilityFunctions)
        {
            InitializeComponent();
            DataManager = dataManager;
            UtilityFunctions = utilityFunctions;
            m_dS = ds;
            m_alkeriaName = m_dS.alkerianame;
            initCamera();
        }

        private void initCamera()
        {
            lblComponentName.Text = m_dS.name;
            string address = "Type: " + m_dS.name;
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
