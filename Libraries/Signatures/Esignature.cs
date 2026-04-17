
using CONSTANTS;
using static LVS3.Enums;

namespace LVS3
{
    public class ESignature
    {
        private ADGroupData signatureGroupLevel;
        private frmESig frmeSig = null;
        private ESigReason eSigReason;
        private string reasonDescription = "";
        private string userReason = "";
        private string errorDescription = "";
        private string m_LastReason;
        private string m_LastUserName;
        private string m_LastFullName;
        public bool SignatureAccepted;
        public bool ReasonRequired;
        public bool CanCancel = false;
        public bool AuthenticationOnly;
        public bool UserNotToConfirm;
        public bool UseLastReason;
        public bool DoNotAcceptLastUser;
        private string esigTitle = "";

        public ESigReason ESigReason { get { return eSigReason; } }
        public string LastReason { get { return m_LastReason; } set { m_LastReason = value; } }
        public string ReasonDescription { get { return reasonDescription; } }
        public string ErrorDescription { get { return errorDescription; } }
        public string UserReason { get { return userReason; } set { userReason = value; } }
        public string LastUserName { get { return m_LastUserName; } set { m_LastUserName = value; } }
        public string LastFullName { get { return m_LastFullName; } set { m_LastFullName = value; } }
        public string CurrentDomain { get { return SystemInformation.UserDomainName; } }
        private IDataManager DataManager;

        public ESignature(ESigReason reason, Roles userrole, string error, IDataManager dataManager)
        {
            DataManager = dataManager;
            eSigReason = reason;
            signatureGroupLevel = AD.GetUserGroupFromRole(userrole);
            reasonDescription = DataManager.GetMeaning(reason);
            errorDescription = error;
            esigTitle = "E-Signature Authorization";

            switch (reason)
            {

                case ESigReason.CancelLabelTraining:
                    esigTitle = "Label Training Cancelled by User";
                    break;
                case ESigReason.CancelTrainingError:
                    esigTitle = "Label Training Cancelled - Error";
                    break;
                case ESigReason.CancelTrainingAlarm:
                    esigTitle = "Label Training Cancelled - Alarm";
                    break;

                case ESigReason.EndReel:
                    esigTitle = "Inspection Complete";
                    break;
                case ESigReason.EndReelAlarm:
                    esigTitle = "Inspection Cancelled - Alarm";
                    break;
                case ESigReason.EndReelError:
                    esigTitle = "Inspection Cancelled - Error";
                    break;
                case ESigReason.EndReelUser:
                    esigTitle = "Inspection Cancelled by User";
                    break;
                case ESigReason.LoginUser:
                    esigTitle = "Log In";
                    break;
                case ESigReason.SaveTraining:
                    esigTitle = "Save Label Training";
                    break;
                default:
                    esigTitle = "Esignature Required";
                    break;
            }

            int temp = reasonDescription.IndexOf("\\n");
            if (temp > 0)
            {
                string first = reasonDescription.Substring(0, temp);
                reasonDescription = first + Environment.NewLine + reasonDescription.Substring(temp + 2);
            }
        }

        public bool CaptureSignature()
        {
            try
            {
                frmeSig = new frmESig(this, signatureGroupLevel, reasonDescription, esigTitle, ReasonRequired, errorDescription, DataManager);
                frmeSig.txtUsername.Text = Defaults.UserLoggedIn == "" ? Environment.UserName : Defaults.UserLoggedIn;
                frmeSig.TopLevel = true;
                frmeSig.TopMost = true;
                frmeSig.ShowDialog();
                SignatureAccepted = frmeSig.SignatureAccepted;
                Defaults.UserSigning = frmeSig.txtUsername.Text;
                frmeSig.Close();
                frmeSig = null;
                return this.SignatureAccepted;
            }
            catch
            {
                return false;
            }
        }
    }
}
