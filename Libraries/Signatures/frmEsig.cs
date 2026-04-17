namespace LVS3
{
    public partial class frmESig : Form
    {
        private ESignature eSig;
        private ADGroupData signatureGroup = null;
        public bool SignatureAccepted;
        public bool ReasonRequired;
        public bool AuthenticationOnly;
        public string UserNotToConfirm;
        public string UseLastReason;
        public string SystemReasons = "";
        public string UserReasons = "";
        public bool DoNotAcceptLastUser;
        private bool canCancel = false;
        private IDataManager DataManager;

        public frmESig(ESignature esig, ADGroupData signaturegroup, string reasondescription, string title, bool reasonrequired, string errordescription, IDataManager dataManager)
        {
            canCancel = esig.CanCancel;
            InitializeComponent();
            DataManager = dataManager;
            cmdCancel.Enabled = canCancel;
            eSig = esig;
            signatureGroup = signaturegroup;
            lblSigMeaning.Text = reasondescription;
            SystemReasons = errordescription;
            lblReason.Text = errordescription;
            ReasonRequired = reasonrequired;
            this.Text = title;
            txtUsername.Focus();
            if (doEndReportControls(reasonrequired, title))
                txtUserReason.Focus();
            if (doTrainingControls(reasonrequired, title))
                txtUserReason.Focus();
        }

        private bool doEndReportControls(bool reasonrequired, string title)
        {
            bool retVal = false;
            if (lblInformation.InvokeRequired)
            {
                lblInformation.Invoke((MethodInvoker)delegate
                {
                    if (reasonrequired == false)
                        txtUserReason.Enabled = false;
                    if (title == "Inspection Complete")
                    {
                        ReasonRequired = true;
                        lblInformation.Text = "End Report Information:";
                        txtUserReason.Enabled = true;
                        retVal = true;
                    }
                });
            }
            else
            {
                if (reasonrequired == false)
                    txtUserReason.Enabled = false;
                if (title == "Inspection Complete")
                {
                    ReasonRequired = true;
                    lblInformation.Text = "End Report Information:";
                    txtUserReason.Enabled = true;
                    retVal = true;
                }
            }
            return retVal;
        }

        private bool doTrainingControls(bool reasonrequired, string title)
        {
            bool retVal = false;
            if (lblInformation.InvokeRequired)
            {
                lblInformation.Invoke((MethodInvoker)delegate
                {
                    if (reasonrequired == false)
                        txtUserReason.Enabled = false;
                    if (title == "Label Training")
                    {
                        ReasonRequired = true;
                        txtUserReason.Enabled = true;
                        retVal = true;
                    }
                });
            }
            else
            {
                if (reasonrequired == false)
                    txtUserReason.Enabled = false;
                if (title == "Label Training")
                {
                    ReasonRequired = true;
                    txtUserReason.Enabled = true;
                    retVal = true;
                }
            }
            return retVal;
        }

        private void doESig()
        {
            try
            {
#if BYPASS_SECURITY

                SignatureAccepted = true;
                eSig.SignatureAccepted = true;
                eSig.LastUserName = txtUsername.Text;
                eSig.UserReason = txtUserReason.Text.Trim();

#else
                eSig.SignatureAccepted = false;
                if (string.IsNullOrEmpty(txtUsername.Text.Trim()) || string.IsNullOrEmpty(txtPassword.Text.Trim()))
                {
                    MessageBox.Show("Both your user-name and password are required to e-sign", "E-Signature Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                if (ReasonRequired & txtUserReason.Text.Trim() == "")
                {
                    MessageBox.Show(string.Format("You must provide {0} details.", lblInformation.Text));
                    txtUserReason.Focus();
                    SignatureAccepted = false;
                    return;
                }
                else if (ReasonRequired)
                {
                    string strReasons = txtUserReason.Text.Trim();
                    eSig.LastReason = "";
                    eSig.LastReason = strReasons;
                    eSig.UserReason = strReasons;
                }


                if (!string.IsNullOrEmpty(UserNotToConfirm))
                {
                    if (txtUsername.Text.ToUpper() == UserNotToConfirm.ToUpper())
                    {
                        MessageBox.Show("A different user (with " + signatureGroup.ADGroupNameFriendly + " permissions) must sign.");
                        SignatureAccepted = false;
                        txtUsername.SelectAll();
                        txtUsername.Focus();
                        return;
                    }
                }

                if (VerifyUser(txtUsername.Text, txtPassword.Text, eSig.CurrentDomain))
                {
                    eSig.LastUserName = txtUsername.Text;
                    eSig.LastFullName = AD.GetUserInfo(txtUsername.Text, UserInfo.name);
                    if (memberOfGroup(eSig.LastUserName, signatureGroup.ADGroupLevel, eSig.CurrentDomain))
                    {
                        SignatureAccepted = true;
                        eSig.SignatureAccepted = true;
                        DataManager.SaveAction("Esign", this.Text, "", txtUsername.Text, "doESig()", "AUTHORIZED", eSig.LastReason);
                    }
                    else
                    {
                        MessageBox.Show("You are not authorized to e-sign", "Group Validation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        SignatureAccepted = false;
                        txtUsername.SelectAll();
                        txtUsername.Focus();
                        DataManager.SaveAction("Esign", this.Text, "", txtUsername.Text, "doESig()", "UNAUTHORIZED", "Authentication Failed");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Authentication failed for user {0}", txtUsername.Text), "User Validation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    SignatureAccepted = false;
                    txtUsername.SelectAll();
                    txtUsername.Focus();
                    DataManager.SaveAction("Esign", this.Text, "", txtUsername.Text, "doESig()", "UNAUTHORIZED", "Authentication Failed");
                    return;
                }
#endif
                this.Hide();
            }
            catch (Exception ex)
            {
                SignatureAccepted = false;
                MessageBox.Show("ESignature verification problem:" + ex.Message, "E-Sign Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DataManager.SaveAction("Esign", this.Text, "", txtUsername.Text, "doESig()", "UNAUTHORIZED", ex.Message);
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (SQLInjectionAttempt() == false)
                doESig();
        }

        private bool SQLInjectionAttempt()
        {
            bool retVal = false;
            if (txtPassword.Text.ToUpper().Contains("INSERT") || txtPassword.Text.ToUpper().Contains("UPDATE") || txtPassword.Text.ToUpper().Contains("DELETE") || txtPassword.Text.ToUpper().Contains("EXEC"))
                retVal = true;
            if (txtUsername.Text.ToUpper().Contains("INSERT") || txtUsername.Text.ToUpper().Contains("UPDATE") || txtUsername.Text.ToUpper().Contains("DELETE") || txtUsername.Text.ToUpper().Contains("EXEC"))
                retVal = true;
            if (txtUserReason.Text.ToUpper().Contains("INSERT") || txtUserReason.Text.ToUpper().Contains("UPDATE") || txtUserReason.Text.ToUpper().Contains("DELETE") || txtUserReason.Text.ToUpper().Contains("EXEC"))
                retVal = true;
            if (retVal == true)
                MessageBox.Show("One or more entries contain characters that could potentially corrupt the data in COSMOS", "Data Input", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return retVal;
        }

        protected override void OnShown(EventArgs e)
        {
            cmdOK.Enabled = true;
            if (txtUsername.Text.Trim() != "")
            {
                if (txtUserReason.Enabled)
                    txtUserReason.Focus();
                else
                    txtPassword.Focus();
            }
        }

        public bool VerifyUser(string Username, string Password, string Domain)
        {
            bool retVal = false;
            retVal = AD.AuthenticateUser(Domain, txtUsername.Text, txtPassword.Text);
            return retVal;
        }

        public string GetFullName(string username, string Domain)
        {
            string retVal = AD.GetUserInfo(username, UserInfo.name);
            return retVal;
        }

        public bool AllowAccess(string UserName, int Group, string Domain)
        {
            bool retVal = false;
            try
            {
                retVal = memberOfGroup(UserName, Group, Domain);
            }
            catch (Exception ex)
            {
                string err = "AllowAccess() err: " + ex.Message;
                MessageBox.Show(ex.Message, "AD");
            }
            return retVal;
        }

        private bool memberOfGroup(string Username, int Groups, string Domain)
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

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            if (ReasonRequired && canCancel == false)
            {
                MessageBox.Show("It is not possible to cancel because a reason is required.\n\nPlease click OK to submit an e-signiture");
                txtUserReason.Enabled = true;
                txtUserReason.Focus();
                return;
            }
            else
            {
                txtUserReason.Text = "";
                eSig.LastUserName = txtUsername.Text.Trim() != "" ? txtUsername.Text : Environment.UserName;
                eSig.LastReason = "";
                SignatureAccepted = false;
                this.Hide();
            }
        }

        private void frmEsig_Load(object sender, EventArgs e)
        {
            if (eSig.ReasonRequired == false)
                this.Width = 270;
        }

        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (txtPassword.Enabled)
                    {
                        txtPassword.Focus();
                        txtPassword.SelectAll();
                    }
                    e.Handled = true;
                }
            }
            catch { }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (txtUserReason.Enabled)
                        if (txtUserReason.Text.Trim() == "")
                        {
                            txtUserReason.Focus();
                            return;
                        }
                    if (cmdOK.Enabled)
                        cmdOK.PerformClick();
                }
            }
            catch { }
        }
    }
}
