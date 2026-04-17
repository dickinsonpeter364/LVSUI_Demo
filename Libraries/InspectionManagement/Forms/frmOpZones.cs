using CONSTANTS;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LVS3
{
    public partial class frmOpZones : Form
    {
        public string OpZoneName = "";

        public frmOpZones(List<VDEItem> opzones)
        {
            InitializeComponent();
            loadOpZoneNames(opzones);
        }


        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Escape)
                {
                    this.OpZoneName = "";
                    this.Hide();
                    e.Handled = true;
                    return;
                }
            }
            catch { }
        }


        private void loadOpZoneNames(List<VDEItem> opzones)
        {
            try
            {
                lstZones.Items.Clear();
                foreach (VDEItem opzone in opzones)
                {
                    if (opzone != null)
                    {
                        if (opzone.IsOPZone)
                        {
                            if (lstZones.Items.Count == 0)
                            {
                                lstZones.Items.Add(opzone.OpZoneName);
                                continue;
                            }
                            else
                            {
                                foreach (string s in lstZones.Items)
                                    if (s.ToLower() == opzone.OpZoneName.ToLower())
                                        continue;
                                lstZones.Items.Add(opzone.OpZoneName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "loadOpZoneNames() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpZoneName = "";
            if (lstZones.SelectedIndex >= 0)
            {
                string confirm = string.Format("Remove VDE from Opzone {0}?", lstZones.SelectedItem.ToString());
                DialogResult dr = MessageBox.Show(confirm, "Remove VDE Items", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    OpZoneName = lstZones.SelectedItem.ToString();
                    this.Hide();
                }
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.OpZoneName = "";
            this.Hide();
        }
    }
}
