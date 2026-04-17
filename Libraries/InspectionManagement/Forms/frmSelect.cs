using CONSTANTS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
//using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LVS3.Enums;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace LVS3
{
    public partial class frmSelect : Form
    {
        private frmBackingCamera frmBCam = null;
        public static event Delegates.SystemMessageHandler SM;
        public LabelItemAndVersion rd = null;
        public static Delegates.SystemMessageHandler SMH;

        public string REELNumber { get => txtReel.Text.Trim().ToUpper(); }

        ///// this to disable the control-box Close icon //////////////
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        ///////////////////////////////////////////////////////////////



        public frmSelect()
        {
            showSpinner(true);
            //showSpinner();
            InitializeComponent();
            loadEvents();
            this.Cursor = Cursors.WaitCursor;
        }

        private void showSpinner(bool showSpinning)
        {
            try
            {
                Invoke((MethodInvoker)delegate
                {
                    if (!showSpinning)
                    {
                        try { this.Cursor = Cursors.Default; } catch { }
                        try { Cursor.Current = Cursors.Default; } catch { }
                    }
                    else
                    {
                        try { this.Cursor = Cursors.AppStarting; } catch { }
                        try { Cursor.Current = Cursors.AppStarting; } catch { }

                    }
                    this.Refresh();
                    this.Invalidate();
                });
            }
            catch
            {
            }
        }

        private void frmSelect_Load(object sender, EventArgs e)
        {
            SystemMessageEventArgs smea = new SystemMessageEventArgs("IO device loaded ok: " + Defaults.IODeviceXML, "Startup", (int)CriticalLevels.Black);
            uscMD.SystemMessage(smea);
            showSpinner(true);
            showVersion();
        }

        private void showVersion()
        {
            try
            {
                lblVersion.Text = "";
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                Defaults.ProductVersion = "version: " + version.Major.ToString().ToString() + "." + version.Minor.ToString() + "." + version.Revision.ToString();
                lblVersion.Text = Defaults.ProductVersion;
            }
            catch { }
        }

        protected override void OnShown(EventArgs e)
        {
            try
            {
                showSpinner(true);
                uscMD.ResizeCols(tlpHMI.Width - 10);
                Thread t = new Thread(new ThreadStart(loadDevices));
                t.IsBackground = true;
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                cmdCancel.Enabled = true;
                cmdReverse.Enabled = true;
                cmdForward.Enabled = true;
                txtReel.Focus();
                base.OnShown(e);
                mxClient.InspectionLampOn();
            }
            catch { }
            finally
            {
                CameraManager.LoadCameras();
                showBackingCamera();
                showSpinner(false);
                txtReel.Select();
                txtReel.Focus();
                txtReel.Text = txtReel.Tag.ToString();
            }
        }

        #region Select and Verify LPN
        private void VerifyLIN()
        {
            try
            {
                cmdOK.Enabled = false;
                {
                    cmdOK.Enabled = true;
                    cmdOK.Focus();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("VerifyMedNos() err: " + ex.Message);
                string err = "VerifyLIN() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Configuration", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
            }
        }

        private void txtReel_TextChanged(object sender, EventArgs e)
        {
            cmdCheckLPN.Enabled = txtReel.Text.Length > 0 ? true : false;
        }

        private bool checkLPN()
        {
            bool retVal = false;
            try
            {
                cmdClear.Enabled = txtReel.Text == "" ? false : true;
                lblLIN.Text = "";
                cmdOK.Enabled = false;
                lblerrorReel.Text = "";
                cmdCheckLPN.Enabled = false;
                optInspect.Enabled = false;
                optTrain.Enabled = false;
                string reeltmp = txtReel.Text.Trim();
                reeltmp = reeltmp.Replace('\n', ' ');
                reeltmp = reeltmp.Replace('\t', ' ');
                reeltmp = reeltmp.Replace('\r', ' ');
                reeltmp = reeltmp.Replace("\n", " ");
                reeltmp = reeltmp.Replace("\t", " ");
                reeltmp = reeltmp.Replace("\r", " ");
                lblerrorReel.Text = "checking Reel Number...";
                lblerrorReel.Refresh();
                if (rd != null)
                    rd = null;
                //identify this Reel exists in vde first
                rd = DataManager.GoodREEL_LWO_LIN_VERSION_Data(reeltmp.Trim().ToUpper());
                if (rd == null)
                {
                    lblerrorReel.Text = "waiting for valid Reel Number...";
                    if (DataManager.ErrorDesription != "")
                    {
                        MessageBox.Show(this, DataManager.ErrorDesription + Environment.NewLine + Environment.NewLine + "Please check connection to COSMOS");
                        retVal = false;
                        return retVal;
                    }
                }
                if (!rd.DataPresent)
                {
                    lblerrorReel.Text = "Incomplete or wrong Reel number";
                    string err = string.Format("Incomplete vde data found for this label under Reel Number: {0}\n\nREEL: {1}\nLWO: {2}\nLabel Item: {3}", txtReel.Text, rd.REEL == "" ? "Missing" : rd.REEL, rd.LWO == "" ? "Missing" : rd.LWO, rd.LIN == "" ? "Missing" : rd.LIN);
                    MessageBox.Show(err);
                    //txtReel.Text = "";
                    txtReel.Enabled = true;
                    pbScanner.Enabled = txtReel.Enabled;
                    txtReel.Focus();
                    txtReel.SelectAll();
                    retVal = false;
                }
                else  
                {
                    lblLIN.Text = rd.LIN;
                    lblerrorReel.Refresh();
                    lblerrorReel.Text = "Reel OK";
                    txtReel.Enabled = false;
                    pbScanner.Enabled = txtReel.Enabled;
                    cmdCheckLPN.Enabled = false;
                    optTrain.Enabled = true;
                    optInspect.Enabled = false;
                    if (DataManager.LabelIsTrained(Defaults.StationID, rd.LIN))
                        optInspect.Enabled = true;
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                string err = "checkLPN() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "New Label", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
            }
            return retVal;
        }

        private void setOptions()
        {
            if (checkLPN())
            {
                if (!DataManager.LabelIsTrained(Defaults.StationID, rd.LIN))
                {
                    optInspect.Enabled = false;
                    optTrain.Enabled = true;
                    optTrain.Select();
                    optTrain.Checked = true;
                    cmdOK.Select();
                }
                else
                {
                    optInspect.Enabled = true;
                    optInspect.Select();
                    optInspect.Checked = true;
                    cmdOK.Select();
                }
            }
        }

        private void cmdCheckLPN_Click(object sender, EventArgs e)
        {
            if (SQLInjectionAttempt() == false)
            {
                setOptions();
                showLabelLayout();
            }
        }

        private bool SQLInjectionAttempt()
        {
            bool retVal = false;
            if (txtReel.Text.ToUpper().Contains("INSERT") || txtReel.Text.ToUpper().Contains("UPDATE") || txtReel.Text.ToUpper().Contains("DELETE") || txtReel.Text.ToUpper().Contains("EXEC") || txtReel.Text.ToUpper().Contains("SELECT"))
                retVal = true;
            if (retVal)
                MessageBox.Show("The text in the REEL input box contains some characters that could potentially be used to corrupt data in COSMOS schema", "Data Input", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return retVal;
        }

        private void showLabelLayout()
        {
            try
            {
                pbLabel.Visible = false;
                string imgPath = ImageData.CreateImageFilepath();
                string layoutFilename = Path.Combine(imgPath, rd.LIN + "_layout.png");
                if (layoutFilename != "")
                    if (File.Exists(layoutFilename))
                    {

                        Image img1 = Image.FromFile(layoutFilename);
                        Bitmap bmp = new Bitmap(img1);
                        pbLabel.Image = bmp;
                        img1 = null;
                        pbLabel.Visible = true;
                    }
            }
            catch { }
        }
        #endregion

        #region Form Controls

        private void frmSelect_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F4 && e.Alt)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
            catch { }
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            pbLabel.Visible = false;
            txtReel.Text = "";
            lblLIN.Text = "";
            lblerrorReel.Text = "";
            txtReel.Enabled = true;
            pbScanner.Enabled = txtReel.Enabled;
            txtReel.Focus();
            optInspect.Enabled = false;
            optTrain.Enabled = false;
            cmdOK.Enabled = false;
            cmdCheckLPN.Enabled = false;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (cmdResetAlarm.Enabled)
            {
                cmdOK.Enabled = false;
                return;
            }

            if(rd==null)
            {
                MessageBox.Show("No data for reel" + REELNumber);
                return;
            }

            cmdOK.Enabled = false; 
            cmdReverse.Enabled = false;
            cmdForward.Enabled = false;
            bool trainedAlready = false;
            if (optTrain.Checked)
            {
                if (DataManager.LabelIsTrained(Defaults.StationID, rd.LIN))
                {
                    DialogResult dr = MessageBox.Show("Label " + rd.LIN + " has already been trained." + Environment.NewLine + "Do you want to re-train it now?" + Environment.NewLine + Environment.NewLine + "(E-Signature is required)", "Train Label", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (dr != DialogResult.Yes)
                    {
                        cmdOK.Enabled = true;
                        cmdReverse.Enabled = true;
                        cmdForward.Enabled = true;
                        return;
                    }                    
                    trainedAlready = true;
                    string trainedLabel = "Previously Trained Label";
                    ESignature es = new ESignature(ESigReason.StartLabelTraining, Roles.LVSIII_USer, trainedLabel);
                    es.UseLastReason = false;
                    es.DoNotAcceptLastUser = false;
                    es.AuthenticationOnly = true;
                    if (trainedAlready)
                        es.ReasonRequired = true;
                    else
                        es.ReasonRequired = false;
                    es.CanCancel = true;
                    es.CaptureSignature();
                    if (es.SignatureAccepted)
                    {
                        DataManager.SaveAction("Re-Train Label", "Label Training", REELNumber, es.LastUserName, "doOK()", es.ErrorDescription + ": " + lblLIN.Text, es.UserReason);
                    }
                    else
                    {
                        cmdOK.Enabled = true;
                        cmdReverse.Enabled = true;
                        cmdForward.Enabled = true;
                        return;
                    }
                }
                hideBackingCamera();
                optTrain.Checked = false;
                unloadEvents();
                closeAllTrainingForms();
                trainLabel();
                loadEvents();
                showLabelLayout();
                showSpinner(false);
                showBackingCamera();

            }
            else if (optInspect.Checked)
            {
                if (DataManager.GetCompleteReport(rd.REEL, rd.LIN))
                {
                    //DialogResult dr = MessageBox.Show("Reel " + rd.REEL + ", label " + rd.LIN + " has already been verified, and a report has been generated.\n\nAre you sure you want to re-verify this reel?","Label Inspection",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);
                    //if (dr != DialogResult.Yes)
                    //{
                    //    cmdOK.Enabled = true;
                    //    cmdReverse.Enabled = true;
                    //    cmdForward.Enabled = true;
                    //    return;
                    //}
                    //else
                    //{
                        DataManager.SaveAction("Inspecting a previously verified reel", "Inspection", rd.REEL, Defaults.UserName, "InspectLabel()", "Reel " + rd.REEL + ", label " + rd.LIN, "This reel had been previously verified");
                    //}
                }
                closeAllOpenForms();
                optInspect.Checked = false;
                hideBackingCamera();
                unloadEvents();
                inspectLabel();
                try { this.Visible = true; } catch { }
                loadEvents();
            }
            txtReel.ReadOnly = false;
            txtReel.SelectAll();
            txtReel.Focus();
            cmdReverse.Enabled = true;
            cmdForward.Enabled = true;
            cmdCancel.Enabled = true;
            cmdReports.Enabled = true;
            showBackingCamera();
            setOptions();
        }

        private void inspectLabel()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                frmInspect frmI = new frmInspect(this.REELNumber, this.lblLIN.Text, rd.LWO);


                if (frmI.LoadInspectionData(this.rd, this.uscMD))
                {
                    try { UtilityFunctions.ReleaseHalconMemoryVariables(); } catch { }
                    try { UtilityFunctions.SetHalconMemoryVariables(); } catch { }

                    this.Cursor = Cursors.Default;
                    frmI.ShowDialog();
                    if (frmI != null)
                        try { frmI = null; } catch { }
                    closeAllOpenForms();
                    loadEvents();
                    this.Visible = true;
                    this.Enabled = true;
                    this.Cursor = Cursors.Default;
                }
            }
            catch { }
            finally
            {
                try { UtilityFunctions.ReleaseHalconMemoryVariables(); } catch { }
            }
        }

        private void closeAllTrainingForms()
        {
            try
            {
                int openCount = Application.OpenForms.OfType<frmLabelConfiguration>().Count();
                if (openCount > 0)
                {
                    List<frmLabelConfiguration> openList = Application.OpenForms.OfType<frmLabelConfiguration>().ToList();
                    for (int x = openCount - 1; x >= 0; x--)
                    {
                        frmLabelConfiguration f = openList[x];

                        if (f.InvokeRequired)
                        {
                            f.Invoke((MethodInvoker)delegate
                            {
                                //try { f.Hide(); } catch { }
                                try { f.Close(); } catch { }
                            });
                        }
                        else
                        {
                            //try { f.Hide(); } catch { }
                            try { f.Close(); } catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "closeAllTrainingForms() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        private void closeAllOpenForms()
        {
            try
            {
                int openCount = Application.OpenForms.OfType<frmInspect>().Count();
                if (openCount > 0)
                {
                    List<frmInspect> openList = Application.OpenForms.OfType<frmInspect>().ToList();
                    for (int x = openCount - 1; x >= 0; x--)
                    {
                        frmInspect f = openList[x];

                        if (f.InvokeRequired)
                        {
                            f.Invoke((MethodInvoker)delegate
                            {
                                //try { f.Hide(); } catch { }
                                try { f.Close(); } catch { }
                            });
                        }
                        else
                        {
                            //try { f.Hide(); } catch { }
                            try { f.Close(); } catch { }
                        }
                    }
                }

                openCount = Application.OpenForms.OfType<frmLabelConfiguration>().Count();
                if (openCount > 0)
                {
                    List<frmLabelConfiguration> openList = Application.OpenForms.OfType<frmLabelConfiguration>().ToList();
                    for (int x = openCount - 1; x >= 0; x--)
                    {
                        frmLabelConfiguration f = openList[x];

                        if (f.InvokeRequired)
                        {
                            f.Invoke((MethodInvoker)delegate
                            {
                                try { f.Close(); } catch { }
                            });
                        }
                        else
                        {
                            try { f.Close(); } catch { }
                        }
                    }
                }

                openCount = Application.OpenForms.OfType<frmUnderInvestigation>().Count();
                if (openCount > 0)
                {
                    List<frmUnderInvestigation> openList = Application.OpenForms.OfType<frmUnderInvestigation>().ToList();
                    for (int x = openCount - 1; x >= 0; x--)
                    {
                        frmUnderInvestigation f = openList[x];

                        if (f.InvokeRequired)
                        {
                            f.Invoke((MethodInvoker)delegate
                            {
                                try { f.Close(); } catch { }
                            });
                        }
                        else
                        {
                            try { f.Close(); } catch { }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string err = "closeAllOpenForms() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            if (cmdCancel.Enabled)
            {
                lblLIN.Text = "";
                this.Close();
            }
        }
        #endregion

        #region Init HMI

        private void loadMXServerApp()
        {
            try
            {
                uscPLC plc = null;
                IDevice idev = null;
                foreach (Control c in tlpHMI.Controls)
                    if (c is uscPLC)
                    {
                        plc = (uscPLC)c;
                        break;
                    }
                foreach (IDevice id in Peripherals.Devices)
                    if (id.uscControl == plc)
                    {
                        idev = id;
                        break;
                    }
                if (plc != null && idev != null)
                {
                    plc.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        bool res = false;
                        List<string> errors = new List<string>();
                        if (mxClient.Open(1))
                        {
                            alarm_reset();

                            int errorspresent = 0;

                            mxClient.ReadRegister(1, "Errors_Present", ref errorspresent, 3);                  
                            if (errorspresent > 0)
                                errors = mxClient.ErrorRegisters(1);
                            if (errors.Count > 0)
                                MessageBox.Show("PLC Errors: " + errors);
                            else
                            {
                                mxClient.WriteToRegister(1, "Mode_Inspect", 0, 3);
                                res = mxClient.WriteToRegister(1, "Mode_Teach", 0, 3);
                            }
                        }
                        else
                        {
                            MessageBox.Show("A connection to PLC (machine) cannot be established!\nPlease check all ethernet connections and power to the PLC", "Startup Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                UtilityFunctions.DoApplicationShutdown();
                            return;
                        }
                        StatusLevel sl = new StatusLevel();
                        if (res == true)
                        {
                            sl.Details = "device open";
                            sl.Status = (int)Enums.StatusLevels.INFORMATION;
                            mxClient.ResetAlarm(1);
                        }
                        else
                        {
                            sl.Details = "Open PLC Failed";
                            sl.Status = (int)Enums.StatusLevels.ERROR;
                        }
                        ((IStatusInformation)plc).SetStatusInformation(sl);
                    });
                }
                else
                    throw new Exception("PLC device could not be instantiated!");

                txtReel.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    this.Activate();
                    this.Select();
                    txtReel.Enabled = true;
                    pbScanner.Enabled = txtReel.Enabled;
                    txtReel.ReadOnly = false;
                    txtReel.Text = txtReel.Tag.ToString();
                    txtReel.CharacterCasing = CharacterCasing.Upper;
                    txtReel.SelectAll();
                    txtReel.Select();
                    txtReel.Focus();

                });
            }
            catch (Exception ex)
            {
                string shutdownNotice = ". Application cannot continue";
                SystemMessageEventArgs smea = new SystemMessageEventArgs("STARTUP ERROR: " + ex.Message + shutdownNotice, "Start Application", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                cmdCheckLPN.Enabled = false;
                cmdClear.Enabled = false;
                cmdCancel.Enabled = true;
                //UtilityFunctions.DoApplicationShutdown();
            }
            finally
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    try { Cursor.Current = Cursors.AppStarting; } catch { }
                });
                try { Cursor.Current = Cursors.AppStarting; } catch { }
            }
        }

        private void loadDevices()
        {
            try
            {
                showSpinner(true);
                if (Peripherals.LoadDevices())
                {
                    tlpHMI.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        lblStationID.Text = Defaults.StationID.ToString();

                        foreach (IDevice device in Peripherals.Devices)
                        {
                            if (device.DS.pt == Enums.PeripheralType.Scanner)
                            {
                                uscScanner usc = (uscScanner)device.uscControl;
                                uscScanner.SMH -= SM;
                                uscScanner.SMH += SM;
                                tlpHMI.Controls.Add(usc, 1, 0);
                                usc.Dock = DockStyle.Fill;
                                usc.InitScanner(device.DS);
                            }
                            if (device.DS.pt == Enums.PeripheralType.PLC)
                            {
                                uscPLC usc = (uscPLC)device.uscControl;
                                uscPLC.SMH -= SM;
                                uscPLC.SMH += SM;
                                tlpHMI.Controls.Add(usc, 0, 0);
                                usc.Dock = DockStyle.Fill;
                                usc.LoadRegisters(device.DS.group);
                                usc.LoadPLCFailCodes();

                            }
                            if (device.DS.pt == Enums.PeripheralType.Camera)
                            {
                                if (device.DS.group == 1 || device.DS.dummy == 0)
                                {
                                    uscNectaCam usc = (uscNectaCam)device.uscControl;
                                    uscNectaCam.SMH -= SM;
                                    uscNectaCam.SMH += SM;
                                    tlpHMI.Controls.Add(usc, 2, 0);
                                    usc.Dock = DockStyle.Fill;
                                    CameraManager.IsCameraConnected(CameraType.NECTACAM, usc);
                                }
                                else
                                {
                                    uscAriaCam usc = (uscAriaCam)device.uscControl;
                                    uscAriaCam.SMH -= SM;
                                    uscAriaCam.SMH += SM;
                                    tlpHMI.Controls.Add(usc, 3, 0);
                                    usc.Dock = DockStyle.Fill;
                                    if (CameraManager.IsCameraConnected(CameraType.ARIACAM, usc))
                                    {
                                        CameraManager.AriaCameras[0].AriaCam.Acquire = false;
                                        CameraManager.AriaCameras[0].AriaCam.Rotate = AlkUSB3.RotateMode.R90;
                                        CameraManager.AriaCameras[0].AriaCam.Acquire = true;
                                    }
                                }
                            }
                        }
                        Task t = new Task(loadMXServerApp);
                        t.Start();
                    });
                }
            }
            catch (Exception ex)
            {
                SystemMessageEventArgs smea = new SystemMessageEventArgs("loadDevices() err " + ex.Message, "Load Devices", (int)Enums.CriticalLevels.Red);
                SM?.Invoke(smea);
                //SetMenuOptions(MenuOptions.ExitOnly);
            }
        }

        private void loadEvents()
        {
            uscMD.setEvents();
            SM -= uscMessageDisplay.SM;
            SM += uscMessageDisplay.SM;
            DataManager.DataManagerSMHL -= SM;
            DataManager.DataManagerSMHL += SM;
            DataManager.DataManagerSMHO -= SM;
            DataManager.DataManagerSMHO += SM;
            Peripherals.SMH -= SM;
            Peripherals.SMH += SM;
            SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
            SYSTEM_IO.IO_CHANGE_Handler += IO_COS_Handler;
        }

        private void unloadEvents()
        {
            try
            {
                uscMD.UnsetEvents();
                SM -= uscMessageDisplay.SM;
                DataManager.DataManagerSMHL -= SM;
                DataManager.DataManagerSMHO -= SM;
                Peripherals.SMH -= SM;
                SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
            }
            catch { }
        }
        #endregion

        #region Alarms

        private bool alarm_reset()
        {
            return mxClient.ResetAlarm(1);
        }

        private void alarm_display_reset()
        {
            string msg = "Alarm Reset - OK";
            string title = "Alarm Reset";
            SystemMessageEventArgs smea = new SystemMessageEventArgs(msg, title, (int)CriticalLevels.Black);
            uscMD.SystemMessage(smea);
        }

        private void IO_COS_Handler(int channel, bool high)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 10)
                ;
            sw.Stop();
            if (channel == 8)
            {
                //MXSERVER.Stop();
                int errorspresent = 0;
                List<string> errors = new List<string>();

                mxClient.ReadRegister(1, "Errors_Present", ref errorspresent, 3);
                if (errorspresent > 0)
                    errors = mxClient.ErrorRegisters(1);
                if (errors.Count > 0)
                {
                    string title = "Alarm Status";
                    foreach (string err in errors)
                    {
                        if (err != "")
                        {
                            string alarmFriendlyName ="Active Alarm: " + err + " : " + PLCFailCodes.GetDescription(err); 
                            SystemMessageEventArgs smea = new SystemMessageEventArgs(alarmFriendlyName, title, (int)CriticalLevels.Amber);
                            uscMD.SystemMessage(smea);
                        }
                    }
                }

                if (cmdResetAlarm.InvokeRequired)
                {
                    cmdResetAlarm.Invoke((MethodInvoker)delegate
                    {
                        cmdCancel.Enabled = true;
                        cmdCheckLPN.Enabled = false;
                        cmdStopManual.Enabled = false;
                        cmdResetAlarm.Enabled = true;
                        cmdResetAlarm.BackColor = Color.Red;
                        cmdForward.Enabled = false;
                        cmdReverse.Enabled = false;
                        optInspect.Enabled = false;
                        optTrain.Enabled = false;
                        foreach (Control c in tlpHMI.Controls)
                        {
                            if (c is uscPLC)
                            {
                                uscPLC usc = (uscPLC)c;
                                StatusLevel SL = new StatusLevel();
                                SL.Details = "PLC fault";
                                SL.Status = (int)StatusLevels.ERROR;
                                usc.SetStatusInformation(SL);
                                SystemMessageEventArgs smea = new SystemMessageEventArgs(SL.Details, "PLC Status", (int)Enums.CriticalLevels.Red);
                                SMH?.Invoke(smea);
                                break;
                            }
                        }
                    });
                }
                else
                {
                    cmdCheckLPN.Enabled = false;
                    cmdCancel.Enabled = true;
                    cmdStopManual.Enabled = false;
                    cmdResetAlarm.Enabled = true;
                    cmdResetAlarm.BackColor = Color.Red;
                    cmdForward.Enabled = false;
                    cmdReverse.Enabled = false;
                    optInspect.Enabled = false;
                    optTrain.Enabled = false;
                    foreach (Control c in tlpHMI.Controls)
                    {
                        if (c is uscPLC)
                        {
                            uscPLC usc = (uscPLC)c;
                            StatusLevel SL = new StatusLevel();
                            SL.Details = "PLC fault";
                            SL.Status = (int)StatusLevels.ERROR;
                            usc.SetStatusInformation(SL);
                            SystemMessageEventArgs smea = new SystemMessageEventArgs(SL.Details, "PLC Status", (int)Enums.CriticalLevels.Red);
                            SMH?.Invoke(smea);
                            break;
                        }
                    }
                }
            }
        }

        private void cmdResetAlarm_Click(object sender, EventArgs e)
        {
            SystemMessageEventArgs smea = new SystemMessageEventArgs("Alarm Reset", "System Status", (int)CriticalLevels.Black);
            uscMD.SystemMessage(smea);
            alarm_reset();
            alarm_display_reset();

            cmdResetAlarm.Enabled = false;
            foreach (Control c in tlpHMI.Controls)
            {
                if (c is uscPLC)
                {
                    uscPLC usc = (uscPLC)c;
                    StatusLevel SL = new StatusLevel();
                    SL.Details = "PLC ready";
                    SL.Status = (int)StatusLevels.INFORMATION;
                    usc.SetStatusInformation(SL);
                    smea = new SystemMessageEventArgs(SL.Details, "PLC Status", (int)Enums.CriticalLevels.Black);
                    SMH?.Invoke(smea);
                    break;
                }
            }
            if (txtReel.Text.Length > 0)
            {
                cmdClear.Enabled = true;
                if (REELNumber != "")
                {
                    cmdCheckLPN.Enabled = true;
                    txtReel.Enabled = true;
                    pbScanner.Enabled = txtReel.Enabled;
                    txtReel.Select();
                    txtReel.Focus();
                    optInspect.Enabled = true;
                    optTrain.Enabled = true;
                    cmdOK.Enabled = false;
                    cmdReverse.Enabled = true;
                    cmdForward.Enabled = true;
                }
            }
            else
            {
                cmdCheckLPN.Enabled = true;
                cmdClear.Enabled = false;
                txtReel.Enabled = true;
                pbScanner.Enabled = txtReel.Enabled;
                txtReel.Select();
                txtReel.Focus();
                optInspect.Enabled = false;
                optTrain.Enabled = false;
                cmdOK.Enabled = false;
                cmdReverse.Enabled = true;
                cmdForward.Enabled = true;
                return;
            }
        }
        #endregion

        private void trainLabel()
        {
            frmLabelConfiguration frmLC = null;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                this.Cursor = Cursors.WaitCursor;

                mxClient.WriteToRegister(1, "REV_Override", 0, 3);
                string lin = rd.LIN;
                string reel = rd.REEL;
                LabelItemAndVersion lvi = rd;
                frmLC = new frmLabelConfiguration(lvi, this.uscMD);
                Cursor.Current = Cursors.WaitCursor;
                this.Cursor = Cursors.WaitCursor;
                frmLC.ShowDialog();
                if (frmLC != null)
                    frmLC = null;
                closeAllOpenForms();
            }
            catch (Exception ex)
            {
                SystemMessageEventArgs smea = new SystemMessageEventArgs("trainLabel() err: " + ex.Message, "Label Training", (int)Enums.CriticalLevels.Red);
                SM?.Invoke(smea);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                this.Cursor = Cursors.Default;
            }
        }

        private void frmSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            UtilityFunctions.CloseRunningPLCServerInstances();
            unloadEvents();
        }

        private void txtReel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                cmdCheckLPN.Enabled = true;
                cmdCheckLPN.PerformClick();
            }
        }

        private void optionSelected_Click(object sender, EventArgs e)
        {
            cmdOK.Enabled = true;
            cmdOK.Focus();
        }

        private void cmdForward_Click(object sender, EventArgs e)
        {
            try
            {
                mxClient.InspectionLampOn();
                cmdCancel.Enabled = false;
                optInspect.Enabled = false;
                optTrain.Enabled = false;
                cmdOK.Enabled = false;
                sbForward.Value = 10;

                mxClient.WriteToRegister(1, "FWD_Manual_Speed_SP", 30, 3);
                mxClient.WriteToRegister(1, "Manual_FWD", 1, 3);
                cmdForward.Enabled = false;
                cmdReverse.Enabled = false;
                cmdStopManual.Enabled = true;
                cmdStopManual.BackColor = Color.Red;
            }
            catch (Exception ex)
            {
                cmdCancel.Enabled = true;
                string err = "cmdForward_Click() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Manual Wind", (int)CriticalLevels.Amber);
                SMH?.Invoke(smea);
            }
        }

        private void cmdReverse_Click(object sender, EventArgs e)
        {
            try
            {                
                mxClient.InspectionLampOn();
                cmdCancel.Enabled = false;
                cmdOK.Enabled = false;
                optInspect.Enabled = false;
                optTrain.Enabled = false;
                if (sbReverse.Value > 10)
                    sbReverse.Value = 10;
                mxClient.WriteToRegister(1, "REV_Override", 1, 3);
                mxClient.WriteToRegister(1, "RWD_Manual_Speed_SP", 5, 3);
                mxClient.WriteToRegister(1, "Manual_RWD", 1, 3);

                changeReverse(10);

                cmdForward.Enabled = false;
                cmdReverse.Enabled = false;
                cmdStopManual.Enabled = true;
                cmdStopManual.BackColor = Color.Red;
            }
            catch (Exception ex)
            {
                cmdCancel.Enabled = true;
                string err = "cmdReverse_Click() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Manual Wind", (int)CriticalLevels.Amber);
                SMH?.Invoke(smea);
            }
        }

        private void cmdStopManual_Click(object sender, EventArgs e)
        {
            try
            {
                mxClient.Stop(1);
                mxClient.InspectionLampOff();
                mxClient.WriteToRegister(1, "REV_Override", 0, 3);
                optInspect.Enabled = false;
                optTrain.Enabled = false;
                cmdForward.Enabled = true;
                cmdReverse.Enabled = true;
                cmdStopManual.Enabled = false;
                cmdCancel.Enabled = true;
                if (checkLPN())
                {
                    optInspect.Enabled = true;
                    optTrain.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                cmdCancel.Enabled = true;
                cmdForward.Enabled = false;
                cmdReverse.Enabled = false;
                cmdStopManual.Enabled = true; 
                string err = "cmdStopManual_Click() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Manual Control", (int)CriticalLevels.Amber);
                SMH?.Invoke(smea);
            }
        }

        private void scrollbar_ValueChanged(object sender, EventArgs e)
        {
            //if (sender is HScrollBar)
            //{
            //    HScrollBar hsb = (HScrollBar)sender;
            //    switch (hsb.Name)
            //    {
            //        case "sbForward":
            //            sbForward.Enabled = false;
            //            Thread t = new Thread(() => changeForward(sbForward.Value));
            //            t.Start();
            //            break;
            //        case "sbReverse":
            //            sbReverse.Enabled = false;
            //            t = new Thread(() => changeReverse(sbReverse.Value));
            //            t.Start();
            //            break;
            //    }
            //}
        }


        private void hScroll_Scroll(object sender, ScrollEventArgs e)
        {
            if (sender is HScrollBar)
            {
                if (e.Type == ScrollEventType.EndScroll)
                {
                    HScrollBar hsb = (HScrollBar)sender;
                    switch (hsb.Name)
                    {
                        case "sbForward":
                            sbForward.Enabled = false;
                            Thread t = new Thread(() => changeForward(sbForward.Value));
                            t.Start();
                            break;
                        case "sbReverse":
                            sbReverse.Enabled = false;
                            t = new Thread(() => changeReverse(sbReverse.Value));
                            t.Start();
                            break;
                    }
                }
            }
        }

        private void changeReverse(int newspeed)
        {
            mxClient.WriteToRegister(1, "RWD_Manual_Speed_SP", newspeed, 3);
            sbReverse.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                sbReverse.Enabled = true;
            });
        }

        private void changeForward(int newspeed)
        {
            mxClient.WriteToRegister(1, "FWD_Manual_Speed_SP", newspeed, 3);
            sbForward.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                sbForward.Enabled = true;
            });
        }
        private void showBackingCamera()
        {
            try
            {
                //var frm = Application.OpenForms.Cast<Form>().Where(x => x.Name == "frmBackingCamera").FirstOrDefault();
                if (frmBCam != null)
                {
                    tlpDock.Controls.Remove(frmBCam);
                    try { frmBCam.Close(); } catch { }
                    try { frmBCam = null; } catch { }
                }
                frmBCam = new frmBackingCamera();
                frmBCam.FormBorderStyle = FormBorderStyle.None;
                frmBCam.Dock = DockStyle.Fill;
                frmBCam.TopLevel = false;
                frmBCam.Visible = true;
                frmBCam.Show();
                tlpDock.Controls.Add(frmBCam, 0, 1);
                frmBCam.Capture(true);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            finally
            {
                try
                {
                    lblBCBottom.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        lblBCBottom.Text = "";
                    });
                }
                catch { }
            }
        }

        private void hideBackingCamera()
        {
            try
            {
                //var frm = Application.OpenForms.Cast<Form>().Where(x => x.Name == "frmBackingCamera").FirstOrDefault();
                if (frmBCam != null)
                {
                    frmBCam.Capture(false);
                    frmBCam.TopLevel = false;
                    //frm.TopMost = false;
                    frmBCam.Hide();
                    frmBCam.Close();
                }
            }
            catch { }
        }

        private void cmdReports_Click(object sender, EventArgs e)
        {
            frmResults frm = new frmResults();
            frm.ShowDialog();
        }

        //private void pbLabel_DoubleClick(object sender, EventArgs e)
        //{
        //    if(pbLabel.Visible)
        //    {
        //        if (pbLabel.SizeMode == PictureBoxSizeMode.Normal)
        //            pbLabel.SizeMode = PictureBoxSizeMode.StretchImage;
        //        else  if (pbLabel.SizeMode == PictureBoxSizeMode.StretchImage)
        //            pbLabel.SizeMode = PictureBoxSizeMode.CenterImage;
        //        else if (pbLabel.SizeMode == PictureBoxSizeMode.CenterImage)
        //            pbLabel.SizeMode = PictureBoxSizeMode.Zoom;
        //        else if (pbLabel.SizeMode == PictureBoxSizeMode.Zoom)
        //            pbLabel.SizeMode = PictureBoxSizeMode.AutoSize;
        //        else if (pbLabel.SizeMode == PictureBoxSizeMode.AutoSize)
        //            pbLabel.SizeMode = PictureBoxSizeMode.Normal;
        //    }
        //}

        private void frmSelect_Activated(object sender, EventArgs e)
        {
            //return;
            loadEvents();
            //if(CameraManager.AriaCameras!=null)
            //    showBackingCamera();
        }
    }
}
