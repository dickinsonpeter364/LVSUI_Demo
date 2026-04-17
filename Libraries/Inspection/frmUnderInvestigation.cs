using CONSTANTS;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static LVS3.Enums;

namespace LVS3
{
    public partial class frmUnderInvestigation : Form
    {

        private HObject tmpBackingImg = null;
        private ESignature eSig;
        private ADGroupData signatureGroupLevel;
        public bool SignatureAccepted;
        public bool ReasonRequired;
        public bool AuthenticationOnly;
        public string UserNotToConfirm;
        public string UseLastReason;
        public bool DoNotAcceptLastUser;
        public string UserSigningNow = "";
        private InvestigationData iData = null;

        public frmUnderInvestigation(InvestigationData id)
        {
            eSig = new ESignature(Enums.ESigReason.Authoriser, Roles.LVSIII_OperatorLvl);
            InitializeComponent();
            hWinReject.HalconWindow.ClearWindow();
            if(id.imgReject!=null)
                id.imgReject.DispObj(hWinReject.HalconWindow);
            hWinReject.SetFullImagePart();
            iData = id;
            //pictureBox1.Image = iData.imgBackingCam;
        }

        private void cmdCancel_Click(object sender, System.EventArgs e)
        {
            SignatureAccepted = false;
            this.Hide();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            try
            {
#if BYPASS_SECURITY

                SignatureAccepted = true;
                eSig.SignatureAccepted = true;
#else

                if (ReasonRequired && clstReason.SelectedItem is null && txtReason.Text == "")
                {
                    MessageBox.Show("You must provide or enter a reason", "Label Under Investigation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    txtReason.Focus();
                    SignatureAccepted = false;
                    return;
                }
                else if (ReasonRequired && clstReason.SelectedItems.Count > 0)
                {
                    bool OtherSelected = false;
                    string strReasons = "";
                    eSig.LastReason = "";
                    foreach (object itemChecked in clstReason.CheckedItems)
                    {
                        if (itemChecked.ToString().ToUpper() == "OTHER")
                            OtherSelected = true;
                        else
                            strReasons = strReasons + itemChecked.ToString() + ".\n ";
                    }
                    if (OtherSelected && txtReason.Text.Trim() == "")
                    {
                        MessageBox.Show("You must enter a reason a reason if you select option 'Other'", "Label Under Investigation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        txtReason.Focus();
                        return;
                    }
                    else
                        strReasons = strReasons + txtReason.Text;
                    eSig.LastReason = strReasons;
                }


                if (optAccept.Checked == false && optReject.Checked == false)
                {
                    MessageBox.Show("Please select Accept or Reject", "Label Under Investigation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int val = -1;
                int canContinue = mxClient.ReadRegister(Defaults.StationID, "M7", ref val);

                if(optAccept.Checked)
                {
                    while (canContinue == 0)
                    {
                        MessageBox.Show("label has been removed - please replace label to continue");
                        canContinue = mxClient.ReadRegister(Defaults.StationID, "M7", ref val);
                        return;
                    }
                }
                else if (optReject.Checked)
                {
                    while (canContinue == 1)
                    {
                        MessageBox.Show("please remove label to continue");
                        canContinue = mxClient.ReadRegister(Defaults.StationID, "M7", ref val);
                        return;
                    }
                }

                eSig.SignatureAccepted = false;
                if (string.IsNullOrEmpty(txtUsername.Text.Trim()) || string.IsNullOrEmpty(txtPassword.Text.Trim()))
                {
                    MessageBox.Show("Both name and password are required to e-sign", "E-Signature Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (string.IsNullOrEmpty(txtUsername.Text.Trim()))
                    {
                        txtUsername.Text = "";
                        txtUsername.Focus();
                    }
                    else
                    {
                        txtPassword.Text = "";
                        txtPassword.Focus();
                    }
                    return;
                }

                if (verifyUser(txtUsername.Text, txtPassword.Text, eSig.CurrentDomain))
                {
                    eSig.LastUserName = txtUsername.Text;
                    eSig.LastFullName = AD.GetUserInfo(txtUsername.Text, UserInfo.name);

                    if (memberOfGroup(eSig.LastUserName, eSig.CurrentDomain))
                    {
                        SignatureAccepted = true;
                        eSig.SignatureAccepted = true;
                    }
                    else
                    {
                        MessageBox.Show("You are not authorized to e-sign", "Group Validation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        SignatureAccepted = false;
                        txtUsername.SelectAll();
                        txtUsername.Focus();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Authentication failed for user {0}", txtUsername.Text), "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    SignatureAccepted = false;
                    txtUsername.SelectAll();
                    txtUsername.Focus();
                    return;
                }
                SignatureAccepted = true;
                eSig.SignatureAccepted = true;

#endif
                this.Hide();
            }
            catch (Exception ex)
            {
                SignatureAccepted = false;
                eSig.SignatureAccepted = false;
                MessageBox.Show("ESignature verification problem:" + ex.Message, "E-Sign Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private bool verifyUser(string Username, string Password, string Domain)
        {
            bool retVal;
            retVal = AD.AuthenticateUser(Domain, txtUsername.Text, txtPassword.Text);
            return retVal;
        }

        private bool memberOfGroup(string Username, string Domain)
        {
            bool retVal = false;
            try
            {
                List<string> ug = AD.ADUserGroups(Username, Domain);
                if (ug.Count == 0)
                    return retVal;
                if (AD.ADGroups.Count == 0)
                    return retVal;

                foreach (ADGroupData adgd in AD.ADGroups)
                    foreach (string gp in ug)
                        if (gp.Trim().ToLower() == adgd.ADGroupName.Trim().ToLower())
                        {
                            retVal = true;
                            return retVal;
                        }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "MemberOfGroup() err: " + ex.Message;
                MessageBox.Show(ex.Message, "AD");
            }
            return retVal;
        }

        private void lblBackingCamDesc_Click(object sender, EventArgs e)
        {
           CameraManager.AriaCameras[0].GrabCameraImage(this, showAriaCamImage, true);
        }

        private void showAriaCamImage()
        {
            if (CameraManager.AriaCameras[0].CameraImage != null)
                if (CameraManager.AriaCameras[0].CameraImage.CountObj() > 0)
                {
                    hWinBackingCam.HalconWindow.ClearWindow();
                    if (tmpBackingImg != null)
                        tmpBackingImg.Dispose();
                    HOperatorSet.CopyObj(CameraManager.AriaCameras[0].CameraImage, out tmpBackingImg, 1, 1);
                    HOperatorSet.RotateImage(tmpBackingImg, out tmpBackingImg, rotateAngleBackingImg * -90, "constant");
                    tmpBackingImg.DispObj(hWinBackingCam.HalconWindow);
                    hWinBackingCam.SetFullImagePart();
                }
        }

        private int rotateAngleBackingImg = 0;

        private void lblRotate_Click(object sender, EventArgs e)
        {
            rotateAngleBackingImg += 1;
            if (rotateAngleBackingImg > 3)
                rotateAngleBackingImg = 0;
            if (tmpBackingImg != null)
                if (tmpBackingImg.CountObj() > 0)
                {
                    hWinBackingCam.HalconWindow.ClearWindow();
                    HOperatorSet.RotateImage(tmpBackingImg, out tmpBackingImg, -90, "constant");
                    tmpBackingImg.DispObj(hWinBackingCam.HalconWindow);
                    hWinBackingCam.SetFullImagePart();
                }
        }


    }
}
