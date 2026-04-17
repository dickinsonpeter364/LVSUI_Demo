/*
 * using System;
 
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using static SALS.Delegates;

namespace SALS
{
    public class LabelPrinter 
    {
        public new bool DEVICE_ACKNOWLEDGE { get; set; } = false;
        public new bool DEVICE_LOADED { get; set; } = false;

        private int printerReadAttempts = 0;
        private int m_deviceTimeout = 0;
        private Timer tmr;
        //public // SysMessageDelegate SysMsgMethod;
        private string m_ip = "127.0.0.1";
        private string m_port = "";
        private string m_name = "";
        private string m_output_data = "";
        public new string IPAddress { get { return m_ip;   } set { initserialport(); m_ip = value;   } }
        public new string Port      { get { return m_port; } set { initserialport(); m_port = value; } }

        public string Name { get { return ""; } }


        public virtual void disconnect()
        {
        }

        public bool execute()
        {
            return true;
        }

        public void executeTest(string testData)
        {

        }

        public bool doOutput()
        {
            return true;
        }
        
        private void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }

        public string OutputData { get {  return m_output_data; } set  { m_output_data = value; } }

        /// <summary>
        /// Reloads the printers RS232 com port settings
        /// </summary>
        /// <remarks></remarks>
        public void ResetPort()
        {
            try
            {
                string s = "";
                //if (m_name == "Carton")
                //    s = utilityFunctions.LoadValue("PrinterCarton", s);
                //else
                //    s = DataManager.LoadValue("PrinterPen", s); 

                //var settings = s.Split("|");
                //m_comPort.PortName = settings[1];
                //m_comPort.BaudRate = Conversions.ToInteger(settings[2]);
                //m_comPort.Parity = UtilityFunctions.StringToParity(settings[3]);
                //m_comPort.DataBits = Convert.ToInt32(settings[4]);
                //m_comPort.StopBits = UtilityFunctions.StringToStopbit(settings[5]);
                //m_comPort.Handshake = UtilityFunctions.StringToHandshake(settings[6]);
            }
            catch (Exception ex)
            {
                //this. SysMsgMethod("Pen printer ResetPort error: " + ex.Message, false, CriticalLevels.Red);
            }
        }


        /// <summary>
        /// re-initializes comport. Required because comports cannot be serialized
        /// </summary>
        /// <remarks></remarks>
        private void initserialport()
        {
            try
            {
                if (m_comPort is null)
                {
                    m_comPort = new SerialPort();
                    //var appset = new ApplicationSettings();
                    //appset.LoadAppSettings();
                    string s = "";
                    //if (m_name == "Carton")
                    //{
                    //    s = appset.PrinterCarton;
                    //}
                    //else
                    //{
                    //    s = appset.PrinterPen;
                    //}

                    //var settings = s.Split("|");
                    //m_ip = settings[0];
                    //m_comPort.PortName = settings[1];
                    //m_comPort.BaudRate = Convert.ToInt32(settings[2]);
                    //m_comPort.Parity = SALS.UtilityFunctions.StringToParity(settings[3]);
                    //m_comPort.DataBits = Conversions.ToInteger(settings[4]);
                    //m_comPort.StopBits = GlobalFunctions.StringToStopbit(settings[5]);
                    //m_comPort.Handshake = GlobalFunctions.StringToHandshake(settings[6]);
                }
                // m_UseTCP = CInt(settings(7))

                else if (m_comPort.IsOpen)
                {
                    disconnect();
                }
            }
            catch (Exception ex)
            {
                //this. SysMsgMethod("Pen printer initserialport error: " + ex.Message, false, CriticalLevels.Red);
            }
        }

        private void checkPortassignment()
        {
            //try
            //{
            //    // key="PrinterPen" value="127.0.0.1|COM4|9600|None|8|1"
            //    string s = ""; // frmMain.ALPPSettings.PrinterPen ' ConfigurationManager.AppSettings("Printer" & m_name)
            //    if (m_name == "Pen")
            //    {
            //        s = frmMain.ALPPSettings.PrinterPen;
            //    }
            //    else if (m_name == "Carton")
            //    {
            //        s = frmMain.ALPPSettings.PrinterCarton;
            //    }

            //    var settings = s.Split("|");
            //    m_ip = settings[0];
            //    m_port = settings[1];
            //    m_baud = Conversions.ToInteger(settings[2]);
            //    m_parity = StringToParity(settings[3]);
            //    m_databits = Conversions.ToInteger(settings[4]);
            //    m_stopbits = StringToStopbit(settings[5]);
            //}
            //catch (Exception ex)
            //{
            //    //this. SysMsgMethod("Pen printer checkPortassignment error: " + ex.Message, false, CriticalLevels.Red);
            //}
        }

        private void tmrCallback()
        {
            try
            {
                if (DEVICE_LOADED == true)
                {
                    tmr.Change(Timeout.Infinite, Timeout.Infinite);
                    return;
                }

                printerReadAttempts += 1;
                if (printerReadAttempts > m_deviceTimeout)
                {
                    tmr.Change(Timeout.Infinite, Timeout.Infinite);
                    DEVICE_ACKNOWLEDGE = false;
                }
            }
            catch (Exception ex)
            {
                tmr.Change(Timeout.Infinite, Timeout.Infinite);
                //this. SysMsgMethod("Pen printer tmrCallback timer error: " + ex.Message, false, CriticalLevels.Red);
            }
        }

        private bool connect()
        {
            bool retVal = false;
            try
            {
                disconnect();
                if (m_comPort != null)
                {
                    initserialport();
                    m_comPort.Open();
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                //this. SysMsgMethod("Pen printer error opening port " + m_name + " on port " + m_port + ". " + ex.Message, false, CriticalLevels.Red);
            }

            return retVal;
        }
    }

}

*/
