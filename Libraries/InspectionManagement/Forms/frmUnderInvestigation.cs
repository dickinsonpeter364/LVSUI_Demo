using CONSTANTS;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using static LVS3.CameraManager;
using static LVS3.Enums;

namespace LVS3
{
    public partial class frmUnderInvestigation : Form
    {

        private int failImageCount = 0;
        private int failImagesInvestigated = 0;
        private HObject VariationRegion = null;
        private int failIndex = 0;
        public string LIN = "";
        public string FAIL_FOLDER = "";
        public bool AcceptedByOperator = false;        
        private FailRecord FP = null;
        private bool SAMPLE = false;

        public frmUnderInvestigation(string layoutpath, string lin, HObject variationregion)
        {
            try
            {
                InitializeComponent();
                LIN = lin;
            }
            catch { }
            if (variationregion != null)
                VariationRegion = variationregion;
        }


        protected override void OnShown(EventArgs e)
        {
            showBackingCamera();
            displayTypes();
            hWinQuery.SetFullImagePart();
        }

        private void showBackingCamera()
        {
            try
            {
                var frm = Application.OpenForms.Cast<Form>().Where(x => x.Name == "frmBackingCamera").FirstOrDefault();
                if (frm != null)
                {
                    try { ((frmBackingCamera)frm).Capture(false); } catch { }
                    try { frm.Close(); } catch { }
                    try { frm = null; } catch { }
                }
                frmBackingCamera frmBC = new frmBackingCamera();
                frmBC.FormBorderStyle = FormBorderStyle.None;
                frmBC.Dock = DockStyle.Fill;
                frmBC.TopLevel = false;
                frmBC.Show();
                tlpBC.Controls.Add(frmBC, 0, 0);
                ((frmBackingCamera)frmBC).Capture(true);
            }
            catch { }
        }

        public void AutoHandle()
        {
            INSPECTION.ReviewLUIModeIndex++;                     
            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    if (this.SAMPLE != true)
                    {
                        this.optAccept.Checked = true;
                        FP.ACCEPTED = true;
                        AcceptedByOperator = true;
                        string formattedIndex = INSPECTION.ReviewLUIModeIndex.ToString("D3");
                        FP.BackingNumber = formattedIndex;
                        FP.UserData = $"Auto Mode User Data Entry {formattedIndex}";
                        FP.ACCEPTED_BY_USER = true;
                        try { HOperatorSet.DetachBackgroundFromWindow(hWinQuery.HalconWindow); } catch { }
                        UtilityFunctions.ReleaseHalconMemoryVariables();
                    }                    
                    foreach (RegionFailData rfd in FP.RegionFailDataList)
                        if (rfd.Img != null) rfd.Img.Dispose();
                    saveResultData();
                    hideBackingCamera();
                    this.Hide();
                });
                
            }
            else
            {
                if (this.SAMPLE != true)
                {
                    this.optAccept.Checked = true;
                    FP.ACCEPTED = true;
                    AcceptedByOperator = true;
                    string formattedIndex = INSPECTION.ReviewLUIModeIndex.ToString("D3");
                    FP.BackingNumber = formattedIndex;
                    FP.UserData = $"Auto Mode User Data Entry {formattedIndex}";
                    FP.ACCEPTED_BY_USER = true;
                    try { HOperatorSet.DetachBackgroundFromWindow(hWinQuery.HalconWindow); } catch { }
                    UtilityFunctions.ReleaseHalconMemoryVariables();
                }
                foreach (RegionFailData rfd in FP.RegionFailDataList)
                    if (rfd.Img != null) rfd.Img.Dispose();
                saveResultData();
                hideBackingCamera();
                this.Hide();
            }
        }

        private void hideBackingCamera()
        {
            try
            {
                var frm = Application.OpenForms.Cast<Form>().Where(x => x.Name == "frmBackingCamera").FirstOrDefault();
                if (frm != null)
                {
                    ((frmBackingCamera)frm).Capture(false);
                    frm.TopLevel = false;
                    frm.TopMost = false;
                    try { tlpBC.Controls.Remove(frm); } catch { }
                    try { frm.Hide(); } catch { }
                    try { frm.Close(); } catch { }
                    try { frm = null; } catch { }
                }
            }
            catch { }
        }


        private void saveFiles()
        {
            if (FP.RegionFailDataList.Count > 0)
            {
                try
                {
                    int index = -1;
                    foreach (RegionFailData rfd in FP.RegionFailDataList)
                    {
                        if (rfd.Img != null)
                        {
                            if (rfd.Img.IsInitialized())
                            {
                                index += 1;
                                if (!FP.SAMPLE)
                                    rfd.Filename = Path.Combine(FAIL_FOLDER, FP.LabelIndex.ToString() + "_" + index.ToString() + Defaults.DumpFile);
                                else
                                    rfd.Filename = Path.Combine(FAIL_FOLDER, FP.LabelIndex.ToString() + "_" + "SAMPLE_" + "1".ToString() + Defaults.DumpFile);
                                HOperatorSet.WriteImage(rfd.Img, "jpeg", 255, rfd.Filename);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("saveFiles() err:" + ex.Message);
                }
            }
        }

        public void Init(FailRecord fp, string lin, string failfolder)
        {
            RegionFailData rfd = null;
            try
            {
                this.FP = fp;
                FAIL_FOLDER = failfolder;
                saveFiles();
                failImageCount = fp.RegionFailDataList.Count;
                failImagesInvestigated = 1;
                cmdCycle.Enabled = (fp.RegionFailDataList.Count > 1);
                this.LIN = lin;
                hWinQuery.HalconWindow.ClearWindow();
                hWinQuery.HalconWindow.SetColor("red");
                HOperatorSet.SetDraw(hWinQuery.HalconWindow, "margin");
                HOperatorSet.SetLineWidth(hWinQuery.HalconWindow, 6);
                HTuple countFails = 0;


                //cmdCycle.Invoke((MethodInvoker)delegate
                //{
                if (fp.SAMPLE == false)
                {
                    if (fp.RegionFailDataList.Count > 0)
                    {
                        rfd = fp.RegionFailDataList[0];
                        fp.DisplayedImageIndex = 0;
                        txtLUIReasons.Text = "";
                        if (rfd.Reasons.Count > 0)
                        {
                            foreach (string reason in rfd.Reasons)
                                txtLUIReasons.Text = txtLUIReasons.Text + reason + Environment.NewLine;
                        }
                        try
                        {
                            UtilityFunctions.PaintRedRectangle(ref fp, failfolder, ref VariationRegion);
                        }
                        catch { }
                        HOperatorSet.DetachBackgroundFromWindow(hWinQuery.HalconWindow);
                        try { HOperatorSet.AttachBackgroundToWindow(rfd.Img, hWinQuery.HalconWindow); } catch { }
                        hWinQuery.SetFullImagePart();
                    }
                }
                else
                {
                    SAMPLE = true;
                    clstReason.Items.Clear();
                    clstReason.Items.Add("SAMPLE LABEL");
                    clstReason.Focus();
                    txtLUIReasons.Text = "SAMPLE LABEL";
                    optAccept.Enabled = false;
                    optReject.Enabled = false;
                    cmdOK.Enabled = true;
                    fp.ACCEPTED_BY_USER = true;
                    rfd = fp.RegionFailDataList[0];
                    try { HOperatorSet.DetachBackgroundFromWindow(hWinQuery.HalconWindow); } catch { }
                    try { HOperatorSet.AttachBackgroundToWindow(rfd.Img, hWinQuery.HalconWindow); } catch { }
                    hWinQuery.SetFullImagePart();
                    if (rfd.Img.IsInitialized())
                    {
                        rfd.Filename = Path.Combine(FAIL_FOLDER, FP.LabelIndex.ToString() + "_" + "SAMPLE_" + "1".ToString() + Defaults.DumpFile);
                        HOperatorSet.WriteImage(rfd.Img, "jpeg", 255, rfd.Filename);
                    }
                    return;
                }

                int indexer = 0;
                ListViewItem item = new ListViewItem();

                string labelpos = fp.LabelIndex.ToString();
                item = new ListViewItem("Label Index:  " + labelpos);
                lstVDE.Items.Add(item);

                foreach (string placeholder in fp.PlaceHolders)
                {
                    item = new ListViewItem("-----");
                    lstVDE.Items.Add(item);
                    item = new ListViewItem(fp.PlaceHolders[indexer]);
                    lstVDE.Items.Add(item);
                    item = new ListViewItem(fp.Datas[indexer]);
                    lstVDE.Items.Add(item);
                    indexer++;
                }
                item = new ListViewItem("-----");
                lstVDE.Items.Add(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Init() err:" + ex.Message);
            }
            finally
            {
                hWinQuery.SetFullImagePart();
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (failImagesInvestigated < failImageCount)
                {
                    flashBackground(cmdCycle);
                    return;
                }
                if (resumeInspection())
                {
                    foreach (RegionFailData rfd in FP.RegionFailDataList)
                        if (rfd.Img != null) rfd.Img.Dispose();
                    
                    if (optMissing.Checked)
                    {
                        try { mxClient.WriteToRegister(1, "Missing_Label_Inhibit", 1, 3); } catch { }
                    }
                    saveResultData();
                    hideBackingCamera();
                    this.Hide();
                }
                else
                    flashBackground(optAccept, optReject, optMissing);
            }
            catch (Exception ex)
            {
                string err = "cmdOK_Click() err: (resume inspection) " + ex.Message;
                MessageBox.Show(err);
            }
        }

        private void flashBackground(Button btn)
        {
            btn.BackColor = Color.Teal;
            btn.Refresh();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < 300)
                ;
            sw.Reset();
            btn.BackColor = Color.LightGray;
            btn.Refresh();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < 400)
                ;
            sw.Reset();
            btn.BackColor = Color.Teal;
            btn.Refresh();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < 400)
                ;
            sw.Reset();
            btn.BackColor = Color.LightGray;
            btn.Refresh();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < 400)
                ;
            sw.Reset();
            btn.Enabled = true;
            btn.BackColor = Color.Teal;
            btn.Refresh();
        }

        private void flashBackground(RadioButton accept, RadioButton reject, RadioButton missing)
        {
            accept.BackColor = Color.Gray;
            reject.BackColor = Color.Gray;
            missing.BackColor = Color.Gray;
            accept.Refresh();
            reject.Refresh();
            missing.Refresh();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < 200)
                ;
            sw.Reset();
            accept.BackColor = Color.Teal;
            reject.BackColor = Color.Teal;
            missing.BackColor = Color.DodgerBlue;
            accept.Refresh();
            reject.Refresh();
            missing.Refresh();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < 200)
                ;
            sw.Reset();
            accept.BackColor = Color.Gray;
            reject.BackColor = Color.Gray;
            missing.BackColor = Color.Gray;
            accept.Refresh();
            reject.Refresh();
            missing.Refresh();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < 200)
                ;
            sw.Reset();
            accept.BackColor = Color.Teal;
            reject.BackColor = Color.Teal;
            missing.BackColor = Color.DodgerBlue;
            accept.Refresh();
            reject.Refresh();
            missing.Refresh();
        }

        public void displayTypes()
        {
            try
            {
                lbl1Bright.Text = DataManager.GetInspectLightAreas(Defaults.StationID, this.LIN) == true ? "Light Marks/Blemishes Inspection: ON" : "Light Marks/Blemishes Inspection: OFF";
                lbl1MinLabel.Text = "Label Debris Size: " + DataManager.GetDebrisSizeLabel(Defaults.StationID, this.LIN);
                lbl1MinVar.Text = "Op-Zone Debris Size: " + getDebrisSize();
                lbl1Type.Text = "Label Type: " + DataManager.GetLabelTypeAsString(Defaults.StationID, this.LIN);
                lbl1VDEContrast.Text = "VDE Contrast: " + DataManager.GetVDEContrastAsString(Defaults.StationID, this.LIN);

            }
            catch (Exception ex)
            {
                string err = "displayTypes() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        private string getDebrisSize()
        {
            string retVal = "SMALL";
            double debrisSizeDefault = DataManager.GetInnerRadiusDefaultSmall();
            double debrisSizeThisLabel = DataManager.GetInnerRadius(Defaults.StationID, this.LIN);
            if (debrisSizeDefault < debrisSizeThisLabel)
                retVal = "LARGE";
            return retVal;
        }

        private void saveResultData()
        {
            int accepted = optAccept.Checked == true ? 0 : 1;
            string checkedItems = getCheckedItems();
            FP.CheckedItems = checkedItems;

            string medItems = getMedItems();
            checkedItems = checkedItems.Replace('\n', ' ');
            checkedItems = checkedItems.Replace('\r', ' ');
            medItems = medItems.Replace('\n', ' ');
            medItems = medItems.Replace('\r', ' ');
            if (this.SAMPLE == false)
            {
                string strlabelPosition = txtBackingNumber.Text.Trim() != "" ? "Web #: " + txtBackingNumber.Text.Trim() : "" + "Index: " + FP.LabelIndex.ToString();
                DataManager.SaveAction1(txtLUIReasons.Text.Trim(), "Inspection", strlabelPosition == null ? "" : strlabelPosition, Defaults.UserLoggedIn, checkedItems, AcceptedByOperator == true ? "SYSTEM: QUERY / OPERATOR: ACCEPT" : "SYSTEM: QUERY / OPERATOR: REJECT", txtOpInformation.Text, "Reel: " + FP.REEL + " | " + medItems);
                DataManager.SaveResultData(FP.REEL, LIN, AcceptedByOperator == true ? "SYSTEM: QUERY / OPERATOR: ACCEPT" : "SYSTEM: QUERY / OPERATOR: REJECT", txtBackingNumber.Text.Trim(), checkedItems, txtOpInformation.Text, txtLUIReasons.Text.Trim(), medItems, FP.LabelIndex);
            }
            else
            {
                string strlabelPosition = "SAMPLE";
                checkedItems = "Sample Label";
                string datasfound = "";
                if (FP.Datas[0] != "")
                {
                    for (int x = 0; x < FP.Datas.Count; x++)
                        if (FP.Datas[x] != "")
                            datasfound = FP.Datas[x] + ", ";
                    if (datasfound.EndsWith(", "))
                        datasfound = datasfound.Substring(0, datasfound.Length - 2);
                }
                DataManager.SaveAction1(txtLUIReasons.Text.Trim(), "Inspection", strlabelPosition, Defaults.UserLoggedIn, "SAMPLE", AcceptedByOperator == true ? "SYSTEM: QUERY / OPERATOR: ACCEPT" : "SYSTEM: QUERY / OPERATOR: REJECT", txtOpInformation.Text, "Reel: " + FP.REEL + " | " + medItems);
            }
        }

        private string getMedItems()
        {
            string retVal = "";
            try
            {
                int numVDE = FP.PlaceHolders.Count;
                for (int x = 0; x < numVDE; x++)
                {
                    retVal = retVal + FP.PlaceHolders[x] + ": ";
                    retVal = retVal + FP.Datas[x];
                    retVal = retVal + Environment.NewLine;
                }
                retVal = retVal.Trim();
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "getMedItems() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        private string getCheckedItems()
        {
            string retVal = "";
            try
            {
                foreach (object itemChecked in clstReason.CheckedItems)
                {
                    if (itemChecked.ToString().ToUpper().Contains("SAMPLE") == false)
                        retVal = retVal + itemChecked.ToString() + "\n ";
                }
                retVal = retVal.Trim();
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "getCheckedItems() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }


        //public void displaySetupData()
        //{
        //    try
        //    {
        //        TrainingData td = null;
        //        td = DataManager.GetTrainingData(LIN);
        //        string tmp = td.ToString();
        //        txtSetupData.Text = tmp;
        //    }
        //    catch (Exception ex)
        //    {
        //        string err = "displaySetupData() err: " + ex.Message;
        //        MessageBox.Show(this, err);
        //    }
        //}

        private bool resumeInspection()
        {
            bool retVal = false;
            try
            {
                bool OtherSelected = false;
                string strSelectedItems = "";
                if (this.SAMPLE == true)
                {
                    retVal = true;
                    return retVal;
                }

                for (int i = 0; i <= (clstReason.Items.Count - 1); i++)
                {
                    if (clstReason.GetItemCheckState(i) == CheckState.Checked)
                    {
                        string item = (string)clstReason.Items[i];
                        strSelectedItems = strSelectedItems + item + "\n ";
                        if (item.ToUpper() == "OTHER")
                            OtherSelected = true;
                    }
                }

                if (txtOpInformation.Text.Trim() != "")
                    if (!strSelectedItems.Contains("Other"))
                        strSelectedItems = strSelectedItems + "Other";

                if (optAccept.Checked == false && optReject.Checked == false && optMissing.Checked == false)
                    return retVal;               

                if (strSelectedItems.Trim() == "")
                {                    
                    MessageBox.Show("Please select an option from the list", "Label Under Investigation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return retVal;
                }

                if (OtherSelected)
                {
                    if (txtOpInformation.Text.Trim() == "")
                    {
                        MessageBox.Show("You must enter the details if you select option 'Other'", "Label Under Investigation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        txtOpInformation.Focus();
                        return retVal;
                    }
                }

                bool canContinue = true;
                txtLUIReasons.Text = txtLUIReasons.Text.Trim();
                string strOPText = "";
                strOPText = txtOpInformation.Text.Trim();
                FP.ACCEPTED = optAccept.Checked; // == false ? "NO" : "YES";
                int val = -1;

                canContinue = mxClient.ReadRegister(1, "At_Investigation", ref val, 3);
                {                    
                    if (optAccept.Checked)
                    {
                        while (canContinue)
                        {
                            MessageBox.Show("label has been removed - please replace label to continue");
                            canContinue = mxClient.ReadRegister(1, "At_Investigation", ref val, 3);
                            retVal = false;
                            return retVal;
                        }
                    }
                    else if (optReject.Checked)
                    {
                        while (canContinue == false)
                        {
                            MessageBox.Show("please remove label to continue");
                            canContinue = mxClient.ReadRegister(1, "At_Investigation", ref val, 3);
                            retVal = false;
                            return retVal;
                        }
                    }
                    else if (optMissing.Checked)
                    {
                        while (canContinue)
                        {
                            MessageBox.Show("label has been removed - please replace label to continue");
                            canContinue = mxClient.ReadRegister(1, "At_Investigation", ref val, 3);
                            retVal = false;
                            return retVal;
                        }
                    }
                }
                if (txtBackingNumber.Text.Trim() == "")
                {
                    DialogResult dr = MessageBox.Show("No backing number. Do you want to enter a backing number?", "Label Under Investigation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        txtBackingNumber.Focus();
                        return retVal;
                    }
                }
                
                if (optAccept.Checked)
                    AcceptedByOperator = true;
                else
                    AcceptedByOperator = false;

                if (optMissing.Checked)
                {
                    INSPECTION.Labelcounts.CountMissing += 1;
                    INSPECTION.Labelcounts.CountQueried -= 1;
                    INSPECTION.Labelcounts.CountRejectedOp -= 1;
                    FP.MISSING = true;
                    FP.DATA_FOUND = false;
                    FP.DATA_INCOMPLETE = false;
                    FP.DUPLICATE = false;
                    FP.ACTIONED = true;
                }
                else
                {
                    FP.MISSING = false;
                }

                FP.BackingNumber = txtBackingNumber.Text.Trim();
                FP.UserData = strOPText;
                FP.ACCEPTED_BY_USER = optAccept.Checked;
                try { HOperatorSet.DetachBackgroundFromWindow(hWinQuery.HalconWindow); } catch { }
                UtilityFunctions.ReleaseHalconMemoryVariables();
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = true;
                //if (lblImg != null) lblImg.Dispose();
                string err = "resumeInspection() err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (VariationRegion != null)
                    VariationRegion.Dispose();
            }
            return retVal;
        }

        private void Options_Click(object sender, EventArgs e)
        {
            try
            {

                bool otherClicked = false;
                cmdOK.Enabled = false;
                cmdOK.Enabled = true;
                if (clstReason.SelectedItem != null)
                {
                    for (int i = 0; i <= (clstReason.Items.Count - 1); i++)
                    {
                        if (clstReason.GetItemChecked(clstReason.SelectedIndex) == false && clstReason.SelectedIndex == clstReason.Items.Count - 1)
                        {
                            otherClicked = true;
                            txtOpInformation.Select();
                            txtOpInformation.Focus();
                            break;
                        }
                    }
                    if (SAMPLE == false)
                    {
                        if (otherClicked == false)
                        {
                            txtBackingNumber.Select();
                            txtBackingNumber.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "Options_Click() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        private void hWin_Load(object sender, EventArgs e)
        {
            this.MouseWheel -= h_MouseWheel;
            this.MouseWheel += h_MouseWheel;
        }

        private void txtBackingNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == (char)Keys.Enter)
            //{
            //    cmdOK.Enabled = true;
            //    cmdOK.Focus();
            //}
        }
        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Tab)
            {
                txtBackingNumber.Enabled = true;
                txtBackingNumber.Focus();
            }
        }

        private void h_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - hWinQuery.Location.X, e.Y - hWinQuery.Location.Y, e.Delta);
                hWinQuery.HSmartWindowControl_MouseWheel(sender, newe);
            }
            catch { }
        }

        private void lblCancel_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(this, "Close application? - Inspection will have to restart\n\rYou should only use this option if LVS3 system is not responding!", "Exit Application", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Stop);
            if (dr == DialogResult.Yes)
            {
                lblCancel.Enabled = false;
                Application.Exit();
            }
        }

        HTuple greyVal = null;

        private void hWinQuery_HMouseMove(object sender, HMouseEventArgs e)
        {
            try
            {
                double x = e.X;
                double y = e.Y;
                try { HalconDotNet.HOperatorSet.GetGrayval(FP.RegionFailDataList[FP.DisplayedImageIndex].Img, e.Y, e.X, out greyVal); } catch { }
                if (greyVal != null)
                    try { uscPixelData1.pointData(Convert.ToInt32(e.X), Convert.ToInt32(e.Y), Convert.ToInt32(greyVal.D)); } catch { }
                else
                    try { uscPixelData1.pointData(Convert.ToInt32(e.X), Convert.ToInt32(e.Y), -1); } catch { }
            }
            catch { }
        }

        private void cmdCycle_Click(object sender, EventArgs e)
        {
            RegionFailData rfd = null;
            try
            {
                failImagesInvestigated += 1;
                hWinQuery.HalconWindow.ClearWindow();
                hWinQuery.HalconWindow.SetColor("red");
                HOperatorSet.SetDraw(hWinQuery.HalconWindow, "margin");
                HOperatorSet.SetLineWidth(hWinQuery.HalconWindow, 3);
                if (failIndex < FP.RegionFailDataList.Count - 1)
                    failIndex++;
                else
                    failIndex = 0;
                rfd = FP.RegionFailDataList[failIndex];
                FP.DisplayedImageIndex = failIndex;
                try { HOperatorSet.DetachBackgroundFromWindow(hWinQuery.HalconWindow); } catch { }
                try { HOperatorSet.AttachBackgroundToWindow(rfd.Img, hWinQuery.HalconWindow); } catch { }
                hWinQuery.SetFullImagePart();

                if (rfd.Reasons.Count > 0)
                {
                    txtLUIReasons.Text = "";
                    foreach (string reason in rfd.Reasons)
                        txtLUIReasons.Text = txtLUIReasons.Text + reason + Environment.NewLine;
                }
                uscPixelData1.SetToNull();
            }
            catch { }
        }

        private void frmUnderInvestigation_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                FP.ClearData();
            }
            catch { }
        }

        //private void btnMissing_Click(object sender, EventArgs e)
        //{
        //    if(btnMissing.BackColor == Color.DodgerBlue)
        //    {
        //        INSPECTION.MissingLabelTrigger = true;
        //        btnMissing.BackColor = Color.OrangeRed;
        //    }
        //    else
        //    {
        //        btnMissing.BackColor = Color.DodgerBlue;
        //        INSPECTION.MissingLabelTrigger = false;
        //    }
        //}
    }
}
