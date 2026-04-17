
/*using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using static LVS3.Enums;
using System.Drawing;
using static LVS3.Delegates;

namespace LVS3
{
    public partial class uscPrinter : UserControl, IPeripheral, ISendMedDataToPrinter
    {
        public static SystemMessageHandler SMH;
        public LabelItemInfo LabelData;
        public StatusLevel SL = new StatusLevel((int)StatusLevels.INFORMATION, "");
        private List<uscInkLevel> InkLevels = new List<uscInkLevel>();

        public string MedData { get => MedData; set => MedData = value; }
        private bool m_dummy = false;
        public bool DummyDevice { get => m_dummy; }

        private string m_componentName = "";
        private PeripheralType m_pt = PeripheralType.NOT_ASSIGNED;
        private DeviceConfig m_dS;
        public PeripheralType Type => m_pt;
        public string ComponentName => m_componentName;
        public DeviceConfig DS => m_dS;
        public UserControl uscControl => this;
        public List<string> COLUMNS = new List<string>();
        public List<string> PLACEHOLDERS = new List<string>();

        public uscPrinter()
        {
            InitializeComponent();
        }

        public void InitPrinter(DeviceConfig ds)
        {
            m_dS = ds;
            m_pt = ds.pt;
            this.lblComponentName.Text = ds.name;
            m_componentName = ds.name;
            this.lblIP.Text = ds.ip.ToString() + ":" + ds.port;
            if (!ComponentReady())
            {
                SL.Details = "Device NOT Ready";
                SL.Status = (int)StatusLevels.WARNING;
                UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, false);
            }
            else
            {
                SL.Details = "Device Ready";
                SL.Status = (int)StatusLevels.INFORMATION;
                UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, false);
            }
        }


        private void ResetStatus()
        {
            SL.RESET();
            UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, true);
        }

        private void diagStatus()
        {
            TcpClient DynamarkClient = null;
            NetworkStream nwStream = null;
            try
            {
                if(!UtilityFunctions.PingComponent(this))
                {
                    string err = string.Format("Cannot connect to {0}. Printer Group # {1}",this.ComponentName, DS.group);
                    SL.Details = err;
                    SL.Status = (int)StatusLevels.ERROR;
                    UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, true);
                    //UtilityFunctions.DoApplicationShutdown();
                    return;
                }
                try { DynamarkClient = new TcpClient(DS.ip.ToString(), Convert.ToInt32(DS.port));  } 
                catch (Exception ex) 
                {
                    SL.Details = "diagStatus() err:\n" + ex.Message;
                    SL.Status = (int)StatusLevels.WARNING;
                    UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, true);
                    return; 
                }
                string textToSend = "GETSTATUS \r\n";
                nwStream = DynamarkClient.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                byte[] bytesToRead = new byte[DynamarkClient.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, DynamarkClient.ReceiveBufferSize);
                string status = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                if (status.ToUpper().Contains("OK"))
                    lblStatus.Text = "OK";
                else if (status.ToUpper().Contains("ERROR"))
                {
                    SL.Details = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                    SL.Status = (int)StatusLevels.ERROR;
                    UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, true);
                }
                else
                {
                    SL.Details = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                    SL.Status = (int)StatusLevels.WARNING;
                    UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, true);
                }
                DynamarkClient.Close();
            }
            catch (Exception ex)
            {
                string err = string.Format("diagStatus() err: {0}\n",ex.Message);
                if (SMH != null)
                {
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Device Status", CriticalLevels.Red);
                    SMH(smea);
                }
            }
            finally
            {
                if (DynamarkClient != null)
                    DynamarkClient = null;
            }
        }

        private void MARKPrinter(bool startstop)
        {
            TcpClient DynamarkClient = null;
            try
            {
                string printerid = this.lblIP.Text; // "192.168.1.254"; // this.lblIP.Text;
                string setMark = "MARK " + (startstop == true ? "START" : "STOP");
                setMark += "\r\n";
                DynamarkClient = new TcpClient(DS.ip.ToString(), Convert.ToInt32(DS.port));
                NetworkStream nwStream = DynamarkClient.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(setMark);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                byte[] bytesToRead = new byte[DynamarkClient.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, DynamarkClient.ReceiveBufferSize);
                string status = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

                if (status.ToUpper().Contains("OK"))
                {
                    SL.Details = setMark + " - OK";
                    SL.Status = (int)StatusLevels.INFORMATION;
                    UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, false);
                }
                else if (status.ToUpper().Contains("ERROR"))
                {
                    SL.Details = setMark + " - ERROR";
                    SL.Details = SL.Details + '\n' + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                    SL.Status = (int)StatusLevels.ERROR;
                    UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, false);
                }
                DynamarkClient.Close();
            }
            catch (Exception ex)
            {
                string err = string.Format("MARKPrinter() err: {0}\n", ex.Message);
                if (SMH != null)
                {
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Printer Command", CriticalLevels.Red);
                    SMH(smea);
                }
            }
            finally
            {
                if (DynamarkClient != null)
                    DynamarkClient = null;
            }
        }

        public bool ComponentReady()
        {
            try
            {
                return UtilityFunctions.PingComponent(this);
            }
            catch (Exception ex)
            {
                string err = string.Format("ComponentReady() err: {0}\n", ex.Message);
                if (SMH != null)
                {
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Device Ready", CriticalLevels.Red);
                    SMH(smea);
                }
            }
            return false;
        }
    }
}
*/