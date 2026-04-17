using System.ComponentModel;
using static LVS3.Enums;
using static LVS3.Delegates;

namespace LVS3
{
    public partial class uscAriaCam : UserControl,
        IStatusInformation
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
        private IDataManager DataManager;
        private IUtilityFunctions UtilityFunctions;

        private bool m_dummy = false;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DummyDevice { get => m_dummy; set => m_dummy = value; }

        public uscAriaCam(DeviceConfig ds, IDataManager dataManager, IUtilityFunctions utilityFunctions)
        {
            InitializeComponent();
            DataManager = dataManager;
            UtilityFunctions = utilityFunctions;
            m_dS = ds;
            initCamera();
            m_alkeriaName = m_dS.alkerianame;
        }

        private void initCamera()
        {
            this.lblComponentName.Text = m_dS.name;
            m_componentName = m_dS.name;
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
