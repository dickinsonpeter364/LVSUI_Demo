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
    public partial class frmLabelCount : Form
    {

        private int oldLabelCount = 0;
        public int NewLabelCount = 0;

        public frmLabelCount(int labelcount)
        {
            InitializeComponent();
            oldLabelCount = labelcount;
            NewLabelCount = labelcount;
            numLabelCount.Value = labelcount;
            numLabelCount.Minimum = labelcount;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            NewLabelCount = Convert.ToInt32(numLabelCount.Value);
            this.Hide();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            NewLabelCount = oldLabelCount;
            this.Hide();
        }
    }
}
