using CONSTANTS;
using System;
using System.Windows.Forms;

namespace LVS3
{
    public partial class frmLabelType : Form
    {
        //public LabelType label_Type = LabelType.PANEL;
        public LabelType label_Type = LabelType.FLATPANEL;

        public frmLabelType()
        {
            InitializeComponent();
        }


        private void optClicked_Click(object sender, EventArgs e)
        {
            optLabelTypeSelected(sender, e);
            cmdOK.Enabled = true;
        }


        private void optLabelTypeSelected(object sender, EventArgs e)
        {
            RadioButton rb = null;
            if (sender is RadioButton)
            {
                rb = (RadioButton)sender;
                if (rb.Name == "optType0")
                {
                    label_Type = LabelType.FLATPANEL;
                    CameraManager.SetGain(1);
                }
                else if (rb.Name == "optType1")
                {
                    label_Type = LabelType.DARK;
                    CameraManager.SetGain(9);
                }
                else if (rb.Name == "optType2")
                {
                    label_Type = LabelType.BOOKLET;
                    CameraManager.SetGain(1);
                }
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
