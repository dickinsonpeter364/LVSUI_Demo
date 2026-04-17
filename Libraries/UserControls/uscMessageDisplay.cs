using System.Text;


namespace LVS3
{

    public partial class uscMessageDisplay : UserControl
    {
        private contextMenuCopyClear cms = null;
        public static Delegates.SystemMessageHandler SM;

        public delegate void DCHandler(int failpipeindex);
        public event DCHandler DoubleClickMessageList;
        
        public uscMessageDisplay()
        {
            InitializeComponent();
        }

        public void setEvents()
        {
            SM -= SystemMessage;
            SM += SystemMessage;
        }

        public void UnsetEvents()
        {
            SM -= SystemMessage;
        }

        public void ResizeCols(int width)
        {
            lvMsg.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                int newsize = width;
                double sizer1 = (width * ((double)0.60)) - 8;
                double sizer2 = (width * ((double)0.20)) + 8;
                double sizer3 = (width * ((double)0.20));
                lvMsg.Columns[0].Width = Convert.ToInt32(sizer1);
                lvMsg.Columns[1].Width = Convert.ToInt32(sizer2);
                lvMsg.Columns[2].Width = Convert.ToInt32(sizer3);
            });
        }

        public void SystemMessage(SystemMessageEventArgs smea)
        {
            try
            {
                string remove_cr_lf = smea.MSG.Replace("\n", " ");
                remove_cr_lf = remove_cr_lf.Replace("\r", " ");
                Font font = new Font("Verdana", 9, FontStyle.Regular);
                Font fontTitle = new Font("Verdana", 10, FontStyle.Regular);
                ListViewItem lvi = new ListViewItem();
                ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem(lvi, smea.TITLE, Color.Black, Color.White, font);
                ListViewItem.ListViewSubItem lvDT = new ListViewItem.ListViewSubItem(lvi, String.Format("{0:G}", DateTime.Now), Color.Black, Color.White, fontTitle);
                if (lvMsg.InvokeRequired)
                {
                    lvMsg.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        lvi.Text = remove_cr_lf;
                        if (smea.FailID >= 0)
                        {
                            lvi.Tag = smea.FailID.ToString();
                        }

                        lvi.Font = font;
                        lvi.SubItems.Add(lvsi);
                        lvi.SubItems.Add(lvDT);
                        if (smea.CL == (int)Enums.CriticalLevels.Black)
                            lvi.ForeColor = Color.Black;
                        else if (smea.CL == (int)Enums.CriticalLevels.Red)
                            lvi.ForeColor = Color.Red;
                        else if (smea.CL == (int)Enums.CriticalLevels.Amber)
                            lvi.ForeColor = Color.OrangeRed;
                        lvMsg.Items.Insert(0, lvi);
                        lvMsg.Invalidate();
                        lvMsg.Refresh();
                        this.Refresh();
                    });
                }
                else
                {
                    lvi.Text = remove_cr_lf;
                    lvi.Font = font;
                    lvi.SubItems.Add(lvsi);
                    lvi.SubItems.Add(lvDT);
                    if (smea.CL == (int)Enums.CriticalLevels.Black)
                        lvi.ForeColor = Color.Black;
                    else if (smea.CL == (int)Enums.CriticalLevels.Red)
                        lvi.ForeColor = Color.Red;
                    else if (smea.CL == (int)Enums.CriticalLevels.Amber)
                        lvi.ForeColor = Color.OrangeRed;
                    lvMsg.Items.Insert(0, lvi);
                    lvMsg.Invalidate();
                    lvMsg.Refresh();
                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SystemMessage() err: " + ex.Message);
            }
        }


        public void Clear()
        {
            if (lvMsg.Items.Count > 0)
            {
                DialogResult dr = MessageBox.Show("Clear displayed messages?", "System Messages", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                    lvMsg.Items.Clear();
            }
        }
        public void Clear(bool silent)
        {
            if (silent)
            {
                if (lvMsg.Items.Count > 0)
                    lvMsg.Items.Clear();
            }
        }

        private void lvSysMessages_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (lvMsg.SelectedItems.Count == 1)
                {
                    if (cms == null)
                        cms = new contextMenuCopyClear(CopySelected, ClearSelected, ClearAll, CopyAll);
                    cms.ShowMenu(lvMsg);
                }
            }
        }


        private void CopySelected()
        {
            ListViewItem lvi = lvMsg.SelectedItems[0];
            Clipboard.SetText(lvi.SubItems[0].Text);
        }

        private void CopyAll()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ListViewItem lv in lvMsg.Items)
            {
                sb.Append(lv.SubItems[0].Text).Append('\t').Append(lv.SubItems[1].Text).Append('\t').Append(lv.SubItems[2].Text).Append(Environment.NewLine);
            }
            Clipboard.SetText(sb.ToString());
        }


        private void ClearSelected()
        {
            ListViewItem lvi = lvMsg.SelectedItems[0];
            lvi.Remove();
        }

        private void ClearAll()
        {
            Clear();
        }

        private void lvMsg_DoubleClick(object sender, EventArgs e)
        {
            if (lvMsg.SelectedItems.Count == 1)
            {
                var firstSelectedItem = lvMsg.SelectedItems[0];
                if (firstSelectedItem is ListViewItem)
                {
                    ListViewItem lvi = (ListViewItem)firstSelectedItem;

                    if (lvi.Tag != null)
                    {
                        var idx = lvi.Tag.ToString();
                        int indexer = -1;
                        int.TryParse(idx.ToString(), out indexer);
                        if (indexer > -1)
                            if (this.DoubleClickMessageList != null)
                                DoubleClickMessageList(indexer);
                    }
                }
            }
        }

    }


    public class contextMenuCopyClear : ContextMenuStrip
    {
        public int x = 0;
        public int y = 0;
        private Action copySelected;
        private Action clearSelected;
        private Action clearAll;
        private Action copyAllSelected;

        public contextMenuCopyClear(Action copyselected, Action clearselected, Action clearall, Action copyall)
        {
            this.copySelected = copyselected;
            this.clearSelected = clearselected;
            this.clearAll = clearall;
            this.copyAllSelected = copyall;
            buildMenu();
            setMenuEnabledStatus();
        }

        public void setXY(double x, double y)
        {
            this.x = Convert.ToInt32(x);
            this.y = Convert.ToInt32(y);
        }

        private void setMenuEnabledStatus()
        {
            this.Items["CopyMessage"].Enabled = true;
            this.Items["clearSelected"].Enabled = true;
            this.Items["clearAll"].Enabled = true;
            this.Items["CopyAllSelected"].Enabled = true;
        }

        private void buildMenu()
        {
            ToolStripMenuItem mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.Items.Add(mnuCopy);
            mnuCopy.Name = "CopyMessage";
            mnuCopy.Size = new System.Drawing.Size(157, 40);
            mnuCopy.Text = "Copy";
            mnuCopy.Click += new System.EventHandler(OnCopy_Click);
            this.Name = "contextMenu1";
            this.Size = new System.Drawing.Size(158, 114);


            ToolStripMenuItem mnuCopyAllSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.Items.Add(mnuCopyAllSelected);
            mnuCopyAllSelected.Name = "CopyAllSelected";
            mnuCopyAllSelected.Size = new System.Drawing.Size(157, 40);
            mnuCopyAllSelected.Text = "Copy All";
            mnuCopyAllSelected.Click += new System.EventHandler(OnCopyAllSelected_Click);


            ToolStripMenuItem mnuClearSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.Items.Add(mnuClearSelected);
            mnuClearSelected.Name = "clearSelected";
            mnuClearSelected.Size = new System.Drawing.Size(157, 40);
            mnuClearSelected.Text = "Clear Selected Message";
            mnuClearSelected.Click += new System.EventHandler(OnClearSelected_Click);

            ToolStripMenuItem mnuClearAll = new System.Windows.Forms.ToolStripMenuItem();
            this.Items.Add(mnuClearAll);
            mnuClearAll.Name = "clearAll";
            mnuClearAll.Size = new System.Drawing.Size(157, 40);
            mnuClearAll.Text = "Clear All Messages";
            mnuClearAll.Click += new System.EventHandler(OnClearAll_Click);
        }

        public void ShowMenu(Control sender)
        {
            Point p = new Point(this.x, this.y);
            p = new Point(MousePosition.X, MousePosition.Y);
            this.Show(p);
            setMenuEnabledStatus();
        }

        private void OnCopy_Click(object sender, EventArgs e)
        {
            this.Hide();
            copySelected();
        }

        private void OnCopyAllSelected_Click(object sender, EventArgs e)
        {
            this.Hide();
            copyAllSelected();
        }

        private void OnClearSelected_Click(object sender, EventArgs e)
        {
            this.Hide();
            clearSelected();
        }

        private void OnClearAll_Click(object sender, EventArgs e)
        {
            this.Hide();
            clearAll();
        }
    }
}
