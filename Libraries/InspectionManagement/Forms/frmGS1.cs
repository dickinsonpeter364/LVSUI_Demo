using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LVS3
{
    public partial class frmGS1 : Form
    {
        public bool _INSPECT_DATA = false;
        public string _DATA = "";
        private string _vde = "";

        public frmGS1(string data, List<string> vdedatalist)
        {
            foreach (string sd in vdedatalist)
            {
                _vde = _vde + sd + Environment.NewLine;
            }

            InitializeComponent();
            txtData.Text = data;
            lblData.Text = data;
            txtData.SelectAll();
            txtData.Focus();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if ((!opt2.Checked) && (!opt3.Checked))
            {
                    MessageBox.Show("Please select an option:\n\n'Is Required' or \n'Is NOT Required'\n\n(to be present during label inspection)");
                    return;
                }
            if (opt3.Checked)
            {
                _INSPECT_DATA = false;
                _DATA = "";
            }
            else
            {
                if(txtData.Text.Trim()=="")
                {
                    MessageBox.Show("Please enter the text that must appear during inspection\n\nNote: It should be formatted and capitalized correctly, including spaces.\n\nOr, to use the text that was detected, click the refresh button. then press OK");
                    return;
                }
                else
                {
                    _DATA = txtData.Text.Trim();
                    _INSPECT_DATA = _DATA == "" ? false : true;
                }
            }

            this.Hide();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            _DATA = "";
            _INSPECT_DATA = false;
            this.Hide();
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            txtData.Text = lblData.Text;
        }

        private void cmdShowVDE_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, _vde, "Barcode VDE Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
