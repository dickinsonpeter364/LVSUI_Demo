using CONSTANTS;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Markup;
using static LVS3.CameraManager;
using static LVS3.Delegates;
using static LVS3.Enums;

namespace LVS3
{
    public partial class frmInspect : Form
    {
        private INSPECTION inspection = null;
        private int hWinHasFocus = 1;
        public static ProgressHandler PH;
        public static AlarmMethodHandler AMD;
        public static ErrorMethodHandler EMD;
        //public static ShowBackingCameraHandler BC;
        public static frmUnderInvestigation frmUI = null;
        private bool START_COMMITED = false;
        public static int labelCount = 0;
        public bool InspectionCompleted = false;
        private bool PAUSED = false;
        internal string LABEL_ITEM = "";
        internal string LABEL_WORK_ORDER = "";
        public string REEL = "";
        ///// this to disable the control-box Close icon //////////////
        private bool FWDSmallCoreSelected = false;
        private bool FWDLargeCoreSelected = false;
        private bool RWDSmallCoreSelected = false;
        private bool RWDLargeCoreSelected = false;
        private bool RWDSartRadiusSet = false;
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

        public frmInspect(string lpn, string lin, string lwo)
        {
            InitializeComponent();

            int cameraGain = DataManager.GetCameraGain(Defaults.StationID, lin);
            CameraManager.SetGain(cameraGain);

            lblReelLPN.Text = lpn;
            LABEL_ITEM = lin;
            LABEL_WORK_ORDER = lwo;
            REEL = lpn;
            uscMD.setEvents();
            SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
            SYSTEM_IO.IO_CHANGE_Handler += IO_COS_Handler;
            inspection = new INSPECTION();


        }

        private void frmInspect_Load(object sender, EventArgs e)
        {
            SYSTEM_IO.IODeviceRefresh();
            UtilityFunctions.ReleaseHalconMemoryVariables();
            UtilityFunctions.SetHalconMemoryVariables();
            loadDelegates();
            loadCounters();

            mxClient.WriteToRegister(1, "Capture_Image", 0, 3);
            mxClient.WriteToRegister(1, "Mode_Teach", 0, 3);
            mxClient.WriteToRegister(1, "Mode_Manual", 1, 3);
            mxClient.WriteToRegister(1, "REV_Override", 0, 3);
            uscMD.ResizeCols((this.Width - tlpControl.Width) + 24);
            displayLegend();
            displaySetupData();
            //displayTypes();
        }

        private void loadDelegates()
        {
            EMD -= new ErrorMethodHandler(do_error_end);
            EMD += new ErrorMethodHandler(do_error_end);
            AMD -= new AlarmMethodHandler(do_alarm);
            AMD += new AlarmMethodHandler(do_alarm);
            PH -= new ProgressHandler(ProgressValue);
            PH += new ProgressHandler(ProgressValue);
        }

        private void unloadDelegates()
        {
            EMD -= new ErrorMethodHandler(do_error_end);
            AMD -= new AlarmMethodHandler(do_alarm);
            PH -= new ProgressHandler(ProgressValue);
        }

        protected override void OnShown(EventArgs e)
        {
            cmdStart.Enabled = false;
            cmdEndInspection.Enabled = true;
            int c = DataManager.GetCountMeds(REEL, Defaults.StationID);
            labelCount = c;
            lblCount.Text = "Labels: " + labelCount.ToString();
            hWinLayout.SetFullImagePart();
            base.OnShown(e);
        }

        private void unloadEvents()
        {
            SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
            uscMD.UnsetEvents();
        }

        public void IO_COS_Handler(int channel, bool high)
        {
            if (SYSTEM_IO.PROCESSING == false)
                return;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 10)
                ;
            sw.Stop();

            if (channel == SYSTEM_IO.ALARM)
                do_alarm();
        }

        public void do_alarm()
        {
            try
            {
                SYSTEM_IO.PROCESSING = false;
                int errorspresent = 0;
                List<string> errors = new List<string>();

                mxClient.ReadRegister(1, "Errors_Present", ref errorspresent, 3);
                if (errorspresent > 0)
                    errors = mxClient.ErrorRegisters(1);
                if (errors.Count > 0)
                {
                    string title = "Alarm Status";
                    string alarmFriendlyName = "";
                    foreach (string err in errors)
                    {
                        if (err != "")
                        {
                            alarmFriendlyName = "Active Alarm: " + err + " : " + PLCFailCodes.GetDescription(err);
                            SystemMessageEventArgs smea = new SystemMessageEventArgs(err, title, (int)CriticalLevels.Red);
                            uscMD.SystemMessage(smea);
                        }
                    }

                    ESignature es = null;
                    es = new ESignature(Enums.ESigReason.EndReelAlarm, Enums.Roles.LVSIII_USer, alarmFriendlyName);
                    es.AuthenticationOnly = true;
                    es.ReasonRequired = true;
                    es.UseLastReason = false;
                    es.DoNotAcceptLastUser = false;
                    es.CaptureSignature();
                    DataManager.SaveAction("Inspection cancelled due to an alarm", "Inspection Alarm", REEL, es.LastUserName, "do_alarm()", es.ReasonDescription, es.UserReason);
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            unloadDelegates();
                            unloadEvents();
                            mxClient.ResetAlarm(1); // clear the faults
                            Hide();
                            //this.Close();
                        });
                    }
                    else
                    {
                        unloadDelegates();
                        unloadEvents();
                        mxClient.ResetAlarm(1); // clear the faults
                        Hide();
                        //this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "doAlarm() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Alarm", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                do_error_end(ex.Message);
            }
        }

        private void loadCounters()
        {
            try
            {
                uscPLC ctrl = null;
                IDevice plcd = null;
                var values = UtilityFunctions.GetValues<CounterTypes>();
                int[] vals = new int[values.Count()];
                int x = 0;
                foreach (int ct in values)
                {
                    vals[x] = (int)ct;
                    x++;
                }
                foreach (IDevice plc in Peripherals.Devices)
                    if (plc.uscControl is uscPLC)
                    {
                        plcd = plc;
                        ctrl = (uscPLC)plc.uscControl;
                        break;
                    }
                if (ctrl != null)
                {
                    for (x = 0; x < vals.Length; x++)
                    {
                        foreach (PLCRegister plcr in ctrl.PLCRegisters)
                            if (Convert.ToInt32(plcr.Register) == (int)vals[x])
                            {
                                CountDisplay cd = new CountDisplay(plcr.Description, 0, plcd.DS.group, x);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "loadCounters() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Initialize Counters", (int)Enums.CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
        }

        public void SetMenuOptions(string menuoption)
        {
            try
            {
                cmdEndInspection.Enabled = false;
                cmdStart.Enabled = false;
                switch (menuoption)
                {
                    case MenuOptions.ExitOnly:
                        {
                            UtilityFunctions.CloseRunningPLCServerInstances();
                            try { Hide(); } catch { }
                            break;
                        }
                    case MenuOptions.IDLE:
                        {
                            break;
                        }


                    case MenuOptions.EndLPN:
                        {
                            //cmdReverse.Enabled = true;
                            //cmdForward.Enabled = true;
                            //tlpManual.Enabled = true;
                            break;
                        }

                    case MenuOptions.Start:
                        {
                            PAUSED = false;
                            cmdStart.Enabled = true;
                            cmdEndInspection.Enabled = true;
                            break;
                        }

                    case MenuOptions.RUNNING:
                        {
                            PAUSED = false;
                            cmdStart.Enabled = false;
                            cmdEndInspection.Enabled = true;
                            break;
                        }


                    case MenuOptions.ResumeInspection:
                        {
                            PAUSED = true;
                            cmdStart.Enabled = true;
                            cmdEndInspection.Enabled = true;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                SystemMessageEventArgs smea = new SystemMessageEventArgs("SetMenuOptions() err: " + ex.Message, "Menu", (int)Enums.CriticalLevels.Amber);
                uscMD.SystemMessage(smea);
            }
        }

        public bool LoadInspectionData(LabelItemAndVersion rd, uscMessageDisplay uscmd)
        {
            bool retVal = false;
            SystemMessageEventArgs smea = null;
            try
            {
                //this.uscMD = uscmd;
                mxClient.WriteToRegister(1, "REV_Override", 0, 3);

                smea = new SystemMessageEventArgs("loading VDE inspection tools...", "Inspection Setup", (int)Enums.CriticalLevels.Black);
                uscmd.SystemMessage(smea);
                if (inspection.loadVDEToolsAndData(rd.REEL, rd.LIN, rd.LWO, ref flpVDEItem) == false)
                {
                    smea = new SystemMessageEventArgs("Could not load VDE data required for inspection", "Load Inspection", (int)Enums.CriticalLevels.Amber);
                    uscmd.SystemMessage(smea);
                    REEL = "";
                    lblReelLPN.Text = "";
                    lblLIN.Text = "";
                    lblCustomer.Text = "";
                    lblCount.Text = "";
                    return retVal;
                }
                smea = new SystemMessageEventArgs("loading Opzone and masking data...", "Inspection Setup", (int)Enums.CriticalLevels.Black);
                uscmd.SystemMessage(smea);
                if (inspection.LoadInspectionVDEItemParams(rd.LIN) == false)
                {
                    smea = new SystemMessageEventArgs("Could not load parameters required from the training data", "Load Inspection", (int)Enums.CriticalLevels.Amber);
                    uscmd.SystemMessage(smea);
                    REEL = "";
                    lblReelLPN.Text = "";
                    lblLIN.Text = "";
                    lblCustomer.Text = "";
                    lblCount.Text = "";
                    return retVal;
                }

                smea = new SystemMessageEventArgs("loading VDE data lists...", "Inspection Setup", (int)Enums.CriticalLevels.Black);
                uscmd.SystemMessage(smea);
                if (inspection.LoadMedDataList(rd.LIN, rd.REEL, rd.LWO, flpVDEItem) == false)
                {
                    smea = new SystemMessageEventArgs("Could not load parameters required from the training data", "Load Inspection", (int)Enums.CriticalLevels.Amber);
                    uscmd.SystemMessage(smea);
                    REEL = "";
                    lblReelLPN.Text = "";
                    lblLIN.Text = "";
                    lblCustomer.Text = "";
                    lblCount.Text = "";
                    return retVal;
                }

                smea = new SystemMessageEventArgs("configuring inspection data...", "Inspection Setup", (int)Enums.CriticalLevels.Black);
                uscmd.SystemMessage(smea);
                if (inspection.LoadInspectionDataFromDB(rd.REEL, rd.LIN) == false)
                {
                    smea = new SystemMessageEventArgs("Could not load parameters required from the training data", "Load Inspection", (int)Enums.CriticalLevels.Amber);
                    uscmd.SystemMessage(smea);
                    REEL = "";
                    lblReelLPN.Text = "";
                    lblLIN.Text = "";
                    lblCustomer.Text = "";
                    lblCount.Text = "";
                    return retVal;
                }

                smea = new SystemMessageEventArgs("Loading complete", "Inspection Setup", (int)Enums.CriticalLevels.Black);
                uscmd.SystemMessage(smea);

                SetMenuOptions(MenuOptions.Start);
                lblReelLPN.Text = REEL;
                lblLIN.Text = rd.LIN + "   Issue No: " + DataManager.GetLabelIssue(rd.LIN, LABEL_WORK_ORDER);
                lblCustomer.Text = DataManager.GetCustomer(rd.LIN);
                lblCount.Text = "Labels: " + labelCount.ToString();
                inspection.SetPauseCaller(do_pause);
                inspection.SetAlarmCaller(do_alarm);
                inspection.MoveNextCaller(do_move_next);
                //inspection.EndInspectionCaller(do_end_reel);
                inspection.ClearFails();
                inspection.ClearResultData(rd.REEL, rd.LIN);
                smea = new SystemMessageEventArgs("Inspection parameters loaded: Click Start to continue", "Inspection Start", (int)Enums.CriticalLevels.Black);
                uscMD.SystemMessage(smea);
                DataManager.SaveAction("Inspection parameters loaded: " + lblCustomer.Text, "Inspection Start", rd.REEL, Defaults.UserName, "LoadInspectionData()", "", "");
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                smea = new SystemMessageEventArgs("LoadInspectionData() err: " + ex.Message, "Inspection Start", (int)Enums.CriticalLevels.Red);
                uscmd.SystemMessage(smea);
                do_error_end(ex.Message);
            }
            return retVal;
        }

        private void do_move_next()
        {
            try
            {
                foreach (Control c in flpVDEItem.Controls)
                    if (c is uscVDEItem)
                    {
                        uscVDEItem vdi = (uscVDEItem)c;
                        vdi.MoveToNextVDE();
                    }
            }
            catch (Exception ex)
            {
                string err = "do_move_next() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)Enums.CriticalLevels.Amber);
                uscMD.SystemMessage(smea);
            }
        }

        private void frmInspect_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!e.Cancel)
            {
                SYSTEM_IO.PROCESSING = false;

                unloadEvents();
                unloadDelegates();
                //hideBackingCamera();
                FailRecord.ClearAll(inspection.FAIL_FOLDER);
            }
        }

        private void cmdEndLPN_Click(System.Object sender, System.EventArgs e)
        {
            if (START_COMMITED == false)
            {
                SYSTEM_IO.PROCESSING = false;
                Hide();
                return;
            }
            DialogResult dr = MessageBox.Show(this, "Cancel This Inspection" + Environment.NewLine + Environment.NewLine + "A reason will be required if you cancel the inspection", "Inspection - Cancel", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                do_pause();
                do_user_end();
            }
        }

        internal void startProgress(int failcount)
        {
            if (uscProgress.InvokeRequired)
            {
                uscProgress.Invoke((MethodInvoker)delegate
                {
                    uscProgress.Visible = true;
                    uscProgress.Val(0);
                    uscProgress.Max(failcount);
                    uscProgress.Refresh();
                });
            }
            else
            {
                uscProgress.Visible = true;
                uscProgress.Val(0);
                uscProgress.Max(failcount);
                uscProgress.Refresh();
            }
        }

        private void ProgressValue(int progress)
        {
            if (uscProgress.InvokeRequired)
            {
                uscProgress.Invoke((MethodInvoker)delegate
                {
                    uscProgress.Val(progress);
                });
            }
            else
                uscProgress.Val(progress);
        }

        public void do_user_end()
        {
            try
            {
                ESignature es = null;
                es = new ESignature(Enums.ESigReason.EndReelUser, Enums.Roles.LVSIII_USer, "Inspection Cancelled By User");
                es.AuthenticationOnly = true;
                es.ReasonRequired = true;
                es.UseLastReason = false;
                es.DoNotAcceptLastUser = false;
                es.CanCancel = false;
                es.CaptureSignature();
                if (es.SignatureAccepted)
                {
                    EndInspectionUser();
                    Hide();
                }
                else
                {
                    es.LastReason = "";
                    do_start();
                }
            }
            catch (Exception ex)
            {
                SystemMessageEventArgs smea = new SystemMessageEventArgs("do_user_end() err: " + ex.Message, "End Inspection", (int)Enums.CriticalLevels.Amber);
                uscMD.SystemMessage(smea);
            }
        }

        public void do_error_end(string err)
        {
            ESignature es = null;
            try
            {
                mxClient.Stop(1);
                es = new ESignature(Enums.ESigReason.EndReelError, Enums.Roles.LVSIII_USer, err);
                es.AuthenticationOnly = true;
                es.ReasonRequired = true;
                es.CanCancel = false;
                es.UseLastReason = false;
                es.DoNotAcceptLastUser = false;
                es.CaptureSignature();
                DataManager.SaveAction("Inspection cancelled due to error", "Inspection Error", REEL, es.LastUserName, "do_error_end()", err, "");
                EndInspectionError(err);
            }
            catch (Exception ex)
            {
                SystemMessageEventArgs smea = new SystemMessageEventArgs("do_error_end() err: " + ex.Message, "End Inspection", (int)Enums.CriticalLevels.Amber);
                uscMD.SystemMessage(smea);
                DataManager.SaveAction("Inspection cancelled due to error", "Inspection Error", REEL, es.LastUserName, "do_error_end()", "originating error: " + err, ex.Message);
            }
        }

        private void removeVDEDataControls()
        {
            Control c = null;
            for (int x = 0; x < flpVDEItem.Controls.Count; x++)
            {
                c = flpVDEItem.Controls[x];
                if (c is uscVDEItem)
                {
                    flpVDEItem.Controls.RemoveAt(x);
                    x--;
                }
            }
        }

        private void EndInspectionUser()
        {
            try
            {
                SYSTEM_IO.PROCESSING = false;

                DataManager.SummaryDataInspectionStart(REEL, lblLIN.Text, Defaults.CurrentUser, Defaults.StationID, true);

                if (lblReelLPN.InvokeRequired)
                {
                    lblReelLPN.Invoke((MethodInvoker)delegate
                    {
                        inspection.ENABLE_CAPTURE = false;
                        CameraManager.NectaCameras[0].nectaCam.Acquire = false;
                        removeVDEDataControls();
                        mxClient.WriteToRegister(1, "Mode_Teach", 0, 3);
                        mxClient.WriteToRegister(1, "Mode_Inspect", 0, 3);
                        mxClient.WriteToRegister(1, "Inspection_Cancelled", 1, 3);
                        SystemMessageEventArgs smea = new SystemMessageEventArgs("Inspection Cancelled by User", "Inspection", (int)Enums.CriticalLevels.Amber);
                        uscMD.SystemMessage(smea);
                    });
                }
                else
                {
                    inspection.ENABLE_CAPTURE = false;
                    CameraManager.NectaCameras[0].nectaCam.Acquire = false;
                    removeVDEDataControls();

                    mxClient.WriteToRegister(1, "Mode_Teach", 0, 3);
                    mxClient.WriteToRegister(1, "Mode_Inspect", 0, 3);
                    mxClient.WriteToRegister(1, "Inspection_Cancelled", 1, 3);
                    SystemMessageEventArgs smea = new SystemMessageEventArgs("Inspection Cancelled by User", "Inspection", (int)Enums.CriticalLevels.Amber);
                    uscMD.SystemMessage(smea);
                }
                mxClient.WriteToRegister(1, "Mode_Inspect", 0, 3);
            }
            catch (Exception ex)
            {
                SystemMessageEventArgs smea = new SystemMessageEventArgs("EndInspectionUser() err: " + ex.Message, "Inspection", (int)Enums.CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
        }

        internal void EndInspectionError(string err)
        {
            try
            {
                mxClient.Stop(1);
                SYSTEM_IO.PROCESSING = false;
                if (lblReelLPN.InvokeRequired)
                {
                    lblReelLPN.Invoke((MethodInvoker)delegate
                    {
                        inspection.ENABLE_CAPTURE = false;
                        CameraManager.NectaCameras[0].nectaCam.Acquire = false;
                        try { HOperatorSet.DetachBackgroundFromWindow(hWinCurrent.HalconWindow); } catch { }

                        mxClient.WriteToRegister(1, "Mode_Teach", 0, 3);
                        mxClient.WriteToRegister(1, "Mode_Inspect", 0, 3);
                        Hide();
                    });
                }
                else
                {
                    inspection.ENABLE_CAPTURE = false;
                    CameraManager.NectaCameras[0].nectaCam.Acquire = false;
                    try { HOperatorSet.DetachBackgroundFromWindow(hWinCurrent.HalconWindow); } catch { }
                    CameraManager.NectaCameras[0].nectaCam.Acquire = false;

                    mxClient.WriteToRegister(1, "Mode_Teach", 0, 3);
                    mxClient.WriteToRegister(1, "Mode_Inspect", 0, 3);
                    Hide();
                }
            }
            catch (Exception ex)
            {
                SystemMessageEventArgs smea = new SystemMessageEventArgs("EndInspectionError() err: " + ex.Message, "Inspection", (int)Enums.CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            DataManager.SummaryDataInspectionStart(REEL, lblLIN.Text, Defaults.CurrentUser, Defaults.StationID, false);
            DataManager.SaveAction1("Inspection Start: " + REEL, "Inspection", REEL, Defaults.UserLoggedIn, "cmdStart_Click()", "Start Inspection", "Start Inspection", "START");
            SetMenuOptions(MenuOptions.RUNNING);

            if (INSPECTION.SampleIncluded == true)
            {
                mxClient.WriteToRegister(1, "Doc_Sample_Included", 1, 3);
                mxClient.WriteToRegister(1, "Doc_Sample_Not_Included", 0, 3);
            }
            else
            {
                mxClient.WriteToRegister(1, "Doc_Sample_Included", 0, 3);
                mxClient.WriteToRegister(1, "Doc_Sample_Not_Included", 1, 3);
            }
            mxClient.WriteToRegister(1, "Speed_Top_Limit", sbSpeedLimit.Value, 3);
            mxClient.WriteToRegister(1, "Label_Count", labelCount, 3);
            sbSpeedLimit.Enabled = false;

            if (FWDSmallCoreSelected)
            {
                mxClient.WriteToRegister(1, "FWD_Large_Core_Selected", 0, 3);
                mxClient.WriteToRegister(1, "FWD_Small_Core_Selected", 1, 3);
            }
            if (FWDLargeCoreSelected)
            {
                mxClient.WriteToRegister(1, "FWD_Small_Core_Selected", 0, 3);
                mxClient.WriteToRegister(1, "FWD_Large_Core_Selected", 1, 3);
            }
            if (RWDSmallCoreSelected)
            {
                mxClient.WriteToRegister(1, "RWD_Large_Core_Selected", 0, 3);
                mxClient.WriteToRegister(1, "RWD_Small_Core_Selected", 1, 3);
            }
            if (RWDLargeCoreSelected)
            {
                mxClient.WriteToRegister(1, "RWD_Small_Core_Selected", 0, 3);
                mxClient.WriteToRegister(1, "RWD_Large_Core_Selected", 1, 3);
            }

            mxClient.WriteToRegister(1, "RWD_Radius", sbRWDRadius.Value, 3);

            // Disable user controls
            sbRWDRadius.Enabled = false;
            cmdFWDReelSmall.Enabled = false;
            cmdFWDReelLarge.Enabled = false;
            cmdRWDReelSmall.Enabled = false;
            cmdRWDReelLarge.Enabled = false;

            do_start();
        }

        private bool doResetCount()
        {
            bool retVal = true;
            try
            {
                bool fixedData = true;
                foreach (Control vdecontrol in flpVDEItem.Controls)
                {
                    if (vdecontrol is uscVDEItem)
                    {
                        uscVDEItem ctrl = (uscVDEItem)vdecontrol;
                        if (ctrl.FIXED == false)
                        {
                            fixedData = false;
                            break;
                        }
                    }
                }
                if (fixedData == true)
                {
                    int originalCount = labelCount;
                    frmLabelCount frmLC = new frmLabelCount(labelCount);
                    frmLC.ShowDialog();
                    if (frmLC.NewLabelCount == originalCount)
                    {
                        labelCount = originalCount;
                        frmLC.Close();
                        frmLC = null;
                        return true;
                    }
                    labelCount = frmLC.NewLabelCount;
                    frmLC.Close();
                    frmLC = null;
                    foreach (Control vdecontrol in flpVDEItem.Controls)
                        if (vdecontrol is uscVDEItem)
                        {
                            uscVDEItem ctrl = (uscVDEItem)vdecontrol;
                            ctrl.ResetCount(labelCount);
                        }
                    inspection.LABEL_COUNT = labelCount;
                    inspection.FIXED_DATA = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "doResetCount() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Amber);
                uscMD.SystemMessage(smea);
            }
            return retVal;
        }

        private bool do_start()
        {
            try
            {
                if (!PAUSED)
                {
                    if (doResetCount())
                    {
                        lblReelLPN.Text = REEL; //  "";
                        uscMD.Clear(true);
                        hWinCurrent.SetFullImagePart();
                        hWinLayout.SetFullImagePart();
                        lblInfoText.Text = "...";
                        if (!inspection.InitInspection(this)) // lblReelLPN.Text, hWinCurrent, hWinLayout, this.uscMD, this.cmdStart, this.cmdEndInspection, this.lblInfoText, this.lblTime, this.pbPass))
                            return false;
                        unloadEvents();
                        SystemMessageEventArgs smea = new SystemMessageEventArgs("Inspection Starting...", "Inspection Mode", (int)CriticalLevels.Black);
                        uscMD.SystemMessage(smea);

                        mxClient.WriteToRegister(1, "Mode_Inspect", 1, 3);
                        PAUSED = false;
                        SetMenuOptions(MenuOptions.RUNNING);
                        Thread t1 = new Thread(do_grab_inspection_image);
                        t1.IsBackground = true;
                        t1.SetApartmentState(ApartmentState.STA);
                        t1.Start();
                    }
                    else
                        return false;
                }
                else
                {
                    SetMenuOptions(MenuOptions.RUNNING);

                    mxClient.WriteToRegister(1, "Capture_Image", 1, 3);
                }
                inspection.ENABLE_CAPTURE = true;
                SYSTEM_IO.PROCESSING = true;
                START_COMMITED = true;
            }
            catch (Exception ex)
            {
                string err = "do_start() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Amber);
                uscMD.SystemMessage(smea);
                return false;
            }
            return true;
        }

        public void displaySetupData()
        {
            try
            {
                List<string> dataList = DataManager.GetTrainingData(LABEL_ITEM, TrainingDataType.OPZONE);
                ListViewItem item = new ListViewItem();
                if (dataList != null)
                    if (dataList.Count > 0)
                    {
                        item = new ListViewItem("OpZone Data:");
                        lstTools.Items.Add(item);

                        foreach (string newItem in dataList)
                        {
                            item = new ListViewItem(newItem);
                            lstTools.Items.Add(item);
                        }
                        item = new ListViewItem("");
                        lstTools.Items.Add(item);
                    }

                dataList = DataManager.GetTrainingData(LABEL_ITEM, TrainingDataType.BARCODE);
                if (dataList != null)
                    if (dataList.Count > 0)
                    {
                        item = new ListViewItem("Barcode Data:");
                        lstTools.Items.Add(item);

                        foreach (string newItem in dataList)
                        {
                            item = new ListViewItem(newItem);
                            lstTools.Items.Add(item);
                        }
                        item = new ListViewItem("");
                        lstTools.Items.Add(item);
                    }

                dataList = DataManager.GetTrainingData(LABEL_ITEM, TrainingDataType.MASK);
                if (dataList != null)
                    if (dataList.Count > 0)
                    {
                        item = new ListViewItem("Mask Data:");
                        lstTools.Items.Add(item);

                        foreach (string newItem in dataList)
                        {
                            item = new ListViewItem(newItem);
                            lstTools.Items.Add(item);
                        }
                        item = new ListViewItem("");
                        lstTools.Items.Add(item);
                    }

                if (DataManager.GetInspectLightAreas(Defaults.StationID, this.LABEL_ITEM) == true)
                {
                    item = new ListViewItem("Light Marks/Blemishes Inspection: ON");
                    lstTools.Items.Add(item);
                }
                else
                {
                    item = new ListViewItem("Light Marks/Blemishes Inspection: OFF");
                    lstTools.Items.Add(item);
                }
                item = new ListViewItem("\nLabel Min Debris Size: " + DataManager.GetDebrisSizeLabel(Defaults.StationID, this.LABEL_ITEM).ToUpper());
                lstTools.Items.Add(item);
                item = new ListViewItem("\nLabel Min Debris Size: " + DataManager.GetDebrisSizeLabel(Defaults.StationID, this.LABEL_ITEM).ToUpper());
                lstTools.Items.Add(item);
                item = new ListViewItem("\nLabel Type: " + DataManager.GetLabelTypeAsString(Defaults.StationID, this.LABEL_ITEM).ToUpper());
                lstTools.Items.Add(item);
            }
            catch (Exception ex)
            {
                string err = "displaySetupData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
        }

        private string getDebrisSize()
        {
            string retVal = "SMALL";
            double debrisSizeDefault = DataManager.GetInnerRadiusDefaultSmall();
            double debrisSizeThisLabel = DataManager.GetInnerRadius(Defaults.StationID, this.LABEL_ITEM);
            if (debrisSizeDefault < debrisSizeThisLabel)
                retVal = "LARGE";
            return retVal;
        }

        public void displayLegend()
        {
            HObject tmpImg = null;
            try
            {
                try { HOperatorSet.DetachBackgroundFromWindow(hWinLayout.HalconWindow); } catch { }
                string IMAGE_FOLDER = ImageData.CreateImageFilepath();
                //string legendFile = Path.Combine(IMAGE_FOLDER, LABEL_ITEM + "_legendkey.png");
                string legendFile = Path.Combine(IMAGE_FOLDER, LABEL_ITEM + "_layout.png");
                HOperatorSet.ReadImage(out tmpImg, legendFile);
                try { HOperatorSet.AttachBackgroundToWindow(tmpImg, hWinLayout.HalconWindow); } catch { }
                hWinLayout.SetFullImagePart();
            }
            catch (Exception ex)
            {
                string err = "displayLegend() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
            finally
            {
                if (tmpImg != null)
                    tmpImg.Dispose();
            }
        }

        private void do_grab_inspection_image()
        {
            if (NectaCameras[0].CameraConnected() == false)
            {
                MessageBox.Show(this, "Camera " + NectaCameras[0].AliasName + " is not responding.\n\nPlease check all camera connections.", "Acquire Image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            PAUSED = false;
            cmdStart.Enabled = false;
            inspection.GetImageInspection();
        }

        private void h_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (hWinHasFocus == 1)
                {
                    MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - hWinLayout.Location.X, e.Y - hWinLayout.Location.Y, e.Delta);
                    hWinLayout.HSmartWindowControl_MouseWheel(sender, newe);
                }
            }
            catch { }
        }

        private void hwinImage_Load(object sender, EventArgs e)
        {
            hWinHasFocus = 0;
            this.MouseWheel -= h_MouseWheel;
            this.MouseWheel += h_MouseWheel;
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
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

        private void do_pause()
        {
            PAUSED = true;
            mxClient.Stop(1);
            //Maurice123
        }

        private void hWinLayout_MouseEnter(object sender, EventArgs e)
        {
            hWinHasFocus = 1;
        }

        private void hWinLayout_MouseLeave(object sender, EventArgs e)
        {
            hWinHasFocus = 0;
        }

        public void ShowBackingCamera()
        {
            return;
        }

        private void hideBackingCamera()
        {
            return;
        }

        private void sbSpeedLimit_Scroll(object sender, ScrollEventArgs e)
        {
            lblSL.Text = sbSpeedLimit.Value.ToString();
        }

        private void sbRWDRadius_Scroll(object sender, ScrollEventArgs e)
        {
            if (RWDSmallCoreSelected && sbRWDRadius.Value > 42)
            {
                RWDSartRadiusSet = true;
                if ((FWDSmallCoreSelected || FWDLargeCoreSelected) && (RWDSmallCoreSelected || RWDLargeCoreSelected))
                {
                    cmdStart.Enabled = true;
                }
            }
            else
            {
                if (RWDLargeCoreSelected && sbRWDRadius.Value > 75)
                {
                    RWDSartRadiusSet = true;
                    if ((FWDSmallCoreSelected || FWDLargeCoreSelected) && (RWDSmallCoreSelected || RWDLargeCoreSelected))
                    {
                        cmdStart.Enabled = true;
                    }
                }
                else
                {
                    RWDSartRadiusSet = false;
                    cmdStart.Enabled = false;
                }
            }
            lblRWDRadius.Text = sbRWDRadius.Value.ToString();
        }

        private void cmdFWDReelSmall_Click(object sender, EventArgs e)
        {
            cmdFWDReelSmall.BackColor = Color.Green;
            cmdFWDReelLarge.BackColor = Color.Transparent;

            FWDLargeCoreSelected = false;
            FWDSmallCoreSelected = true;
            if ((RWDSmallCoreSelected && sbRWDRadius.Value > 42) || (RWDLargeCoreSelected && sbRWDRadius.Value > 75))
            {
                cmdStart.Enabled = true;
            }
            else
            {
                cmdStart.Enabled = false;
            }
        }

        private void cmdFWDReelLarge_Click(object sender, EventArgs e)
        {
            cmdFWDReelLarge.BackColor = Color.Green;
            cmdFWDReelSmall.BackColor = Color.Transparent;

            FWDSmallCoreSelected = false;
            FWDLargeCoreSelected = true;
            if ((RWDSmallCoreSelected && sbRWDRadius.Value > 42) || (RWDLargeCoreSelected && sbRWDRadius.Value > 75))
            {
                cmdStart.Enabled = true;
            }
            else
            {
                cmdStart.Enabled = false;
            }
        }

        private void cmdRWDReelSmall_Click(object sender, EventArgs e)
        {
            cmdRWDReelSmall.BackColor = Color.Green;
            cmdRWDReelLarge.BackColor = Color.Transparent;

            RWDLargeCoreSelected = false;
            RWDSmallCoreSelected = true;
            if ((FWDSmallCoreSelected || FWDLargeCoreSelected) && sbRWDRadius.Value > 42)
            {
                cmdStart.Enabled = true;
            }
            else
            {
                cmdStart.Enabled = false;
            }
        }

        private void cmdRWDReelLarge_Click(object sender, EventArgs e)
        {
            cmdRWDReelLarge.BackColor = Color.Green;
            cmdRWDReelSmall.BackColor = Color.Transparent;

            RWDSmallCoreSelected = false;
            RWDLargeCoreSelected = true;
            if ((FWDSmallCoreSelected || FWDLargeCoreSelected) && sbRWDRadius.Value > 75)
            {
                cmdStart.Enabled = true;
            }
            else
            {
                cmdStart.Enabled = false;
            }
        }
    }

    public interface ITrainingData
    {
        List<string> TrainingData(TrainingDataType tdt);
    }

    public class TrainingData : ITrainingData
    {
        private string LABEL_ITEM = "";
        public TrainingData(string lin)
        {
            this.LABEL_ITEM = lin;
        }

        List<string> ITrainingData.TrainingData(TrainingDataType tdt)
        {
            return DataManager.GetTrainingData(LABEL_ITEM, tdt);
        }
    }
}

