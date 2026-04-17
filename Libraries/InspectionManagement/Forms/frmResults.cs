using System;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using HalconDotNet;
using System.IO;
using LVS3;
using static LVS3.Enums;
using CONSTANTS;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace LVS3
{
    public partial class frmResults : Form
    {
        private string reportFilePath = "";
        private string reportFullPDFPath = "";

        public frmResults()
        {
            InitializeComponent();
            if (cboReportPDF.Items.Count > 0)
                cboReportPDF.SelectedIndex = 0;
        }
        protected override void OnShown(EventArgs e)
        {
            cmdClose.Enabled = true;
            uscMD.ResizeCols(this.Width - 10);
            base.OnShown(e);
            createReportColumns();
            loadPDFNames();
            loadUserNames();
        }

        private void createReportColumns()
        {
            DataGridViewColumn col;
            DataGridViewCell dvc = new DataGridViewTextBoxCell();
            dgvGrid.Rows.Clear();
            dgvGrid.Columns.Clear();
            col = new DataGridViewColumn();
            dvc = new DataGridViewTextBoxCell();
            col.CellTemplate = dvc;
            col.HeaderText = "User";
            col.Width = 100;
            dgvGrid.Columns.Add(col);
            col = new DataGridViewColumn();
            dvc = new DataGridViewTextBoxCell();
            col.CellTemplate = dvc;
            col.HeaderText = "Date/Time";
            col.Width = 150;
            dgvGrid.Columns.Add(col);
            col = new DataGridViewColumn();
            dvc = new DataGridViewTextBoxCell();
            col.CellTemplate = dvc;
            col.HeaderText = "DTZ";
            col.Width = 40;
            dgvGrid.Columns.Add(col);
            col = new DataGridViewColumn();
            dvc = new DataGridViewTextBoxCell();
            col.CellTemplate = dvc;
            col.HeaderText = "Heading";
            col.Width = 140;
            dgvGrid.Columns.Add(col);

            col = new DataGridViewColumn();
            dvc = new DataGridViewTextBoxCell();
            col.CellTemplate = dvc;
            col.HeaderText = "Action";
            col.Width = 220;
            dgvGrid.Columns.Add(col);
            col = new DataGridViewColumn();
            dvc = new DataGridViewTextBoxCell();
            col.CellTemplate = dvc;
            col.HeaderText = "Details";
            col.Width = 220;
            dgvGrid.Columns.Add(col);
            col = new DataGridViewColumn();
            dvc = new DataGridViewTextBoxCell();
            col.CellTemplate = dvc;
            col.HeaderText = "User Information";
            col.Width = 220;
            dgvGrid.Columns.Add(col); col = new DataGridViewColumn();
            dvc = new DataGridViewTextBoxCell();
            col.CellTemplate = dvc;
            col.HeaderText = "Machine";
            col.Width = 120;
            dgvGrid.Columns.Add(col);
        }

        private void loadPDFNames()
        {
            try
            {
                cboReportPDF.Items.Clear();
                reportFilePath = Defaults.ReportPath;
                //if (reportFilePath.EndsWith("/") || reportFilePath.EndsWith(@"\"))
                //reportFilePath = reportFilePath.Substring(0, reportFilePath.Length - 2);
                reportFilePath = "\\\\" + reportFilePath;
                reportFilePath = reportFilePath.Replace('/', '\\');

                DirectoryInfo reportDirectory = new DirectoryInfo(reportFilePath);
                FileInfo[] pdfs = reportDirectory.GetFiles("*.pdf");
                if (pdfs.Length > 0)
                {
                    foreach (FileInfo file in pdfs)
                        cboReportPDF.Items.Add(file.Name);
                    cboReportPDF.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                string err = "loadPDFNames() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Load Data", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
        }

        private void loadAuditData()
        {
            DataTable dt;
            DataSet ds;
            try
            {
                dgvGrid.Rows.Clear();
                cmdCSV.Enabled = false;
                string userFilter = (cboUser.SelectedIndex <= 0) ? " LIKE '%' OR (ACTION_USERNAME IS NULL)) " : " = '" + cboUser.SelectedItem.ToString() + "') ";
                ds = DataManager.AuditLog(userFilter, dtmFrom.Value, dtmTo.Value);
                if (ds == null)
                {
                    MessageBox.Show("selection returned no data" + Environment.NewLine + Environment.NewLine + "Check date ranges?");
                    return;
                }
                dt = ds.Tables[0];
                if (dt == null)
                {
                    MessageBox.Show("selection returned no data" + Environment.NewLine + Environment.NewLine + "Check date ranges?");
                    return;
                }
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("selection returned no data" + Environment.NewLine + Environment.NewLine + "Check date ranges?");
                    return;
                }
                cmdCSV.Enabled = true;

                foreach (DataRow dr in dt.Rows)
                {
                    DataGridViewRow dgr = new DataGridViewRow();
                    int indexer = dgvGrid.Rows.Add(dgr);
                    string value = dr.ItemArray[0].ToString().TrimEnd();
                    dgvGrid.Rows[indexer].Cells[0].Value = removeLineFeedsAndNulls(value, " "); 
                    value = dr.ItemArray[1].ToString();
                    dgvGrid.Rows[indexer].Cells[1].Value = removeLineFeedsAndNulls(value, " "); //date
                    value = dr.ItemArray[2].ToString();
                    dgvGrid.Rows[indexer].Cells[2].Value = removeLineFeedsAndNulls(value, " "); //time zone
                    value = dr.ItemArray[3].ToString();
                    dgvGrid.Rows[indexer].Cells[3].Value = removeLineFeedsAndNulls(value, " "); //title
                    value = dr.ItemArray[4].ToString();
                    dgvGrid.Rows[indexer].Cells[4].Value = removeLineFeedsAndNulls(value, " "); // message
                    value = dr.ItemArray[5].ToString();
                    dgvGrid.Rows[indexer].Cells[5].Value = removeLineFeedsAndNulls(value, " "); //details
                    value = dr.ItemArray[6].ToString();
                    dgvGrid.Rows[indexer].Cells[6].Value = removeLineFeedsAndNulls(value, " "); //user info
                    value = dr.ItemArray[7].ToString();
                    dgvGrid.Rows[indexer].Cells[7].Value = removeLineFeedsAndNulls(value, " "); //user info
                }
            }
            catch (Exception ex)
            {
                string err = "loadAuditData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Load Audit Data", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
        }

        private string removeLineFeedsAndNulls(string value, string replacewith)
        {
            if (String.IsNullOrEmpty(value))
                return replacewith;
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();
            return value.Replace("\r\n", replacewith)
                        .Replace("\n", replacewith)
                        .Replace("\r", replacewith)
                        .Replace(lineSeparator, replacewith)
                        .Replace(paragraphSeparator, replacewith);
        }

        private void loadUserNames()
        {
            List<string> usernames = new List<string>();
            try
            {
                cboUser.Items.Clear();
                usernames = DataManager.UserNames();
                if (usernames.Count == 0)
                {
                    MessageBox.Show("selection returned no data" + Environment.NewLine + Environment.NewLine + "Check date ranges?");
                    return;
                }
                cboUser.Items.Add("<any>");
                foreach (string s in usernames)
                    cboUser.Items.Add(s);
            }
            catch (Exception ex)
            {
                string err = "loadUserNames() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Load User Data", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
        }


        private void frmResults_ResizeEnd(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn col in dgvGrid.Columns)
                col.Width = (Width / 12) - 5;
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdInspection_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", reportFullPDFPath);
            }
            catch (Exception ex)
            {
                string err = "cmdInspection_Click() err: " + ex.Message;
                if (ex.Message.Contains("another proc"))
                    err = err + Environment.NewLine + Environment.NewLine + "Please close any open PDF report documents, then try to re-opene this report";
                SystemMessageEventArgs SMEA = new SystemMessageEventArgs(err, "Open Inspection Report", (int)Enums.CriticalLevels.Black);
                uscMD.SystemMessage(SMEA);
            }
            finally
            {
                cmdInspection.Enabled = false;
            }
        }

        private void cboReportPDF_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (cboReportPDF.SelectedIndex >= 0)
                {
                    cmdInspection.Enabled = true;
                    reportFullPDFPath = Path.Combine(reportFilePath, cboReportPDF.SelectedItem.ToString());
                }
                else
                    cmdInspection.Enabled = false;
            }
            catch (Exception ex)
            {
                string err = "cboReportPDF_DropDownClosed() err: " + ex.Message;
                //if (ex.Message.Contains("another proc"))
                //    err = err + Environment.NewLine + Environment.NewLine + "Please close any open PDF report documents, then try to re-create this report";
                SystemMessageEventArgs SMEA = new SystemMessageEventArgs(err, "Open Inspection Report", (int)Enums.CriticalLevels.Black);
                uscMD.SystemMessage(SMEA);
            }
        }

        private void cboUser_DropDownClosed(object sender, EventArgs e)
        {
            cmdAudit.Enabled = true;
            cmdCSV.Enabled = false;
        }

        private void cmdAudit_Click(object sender, EventArgs e)
        {
            loadAuditData();
        }

        private void outputCSVData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("User,").Append("Date/Time,").Append("Heading,").Append("Action,").Append("Details,").Append("User Information,").Append("Machine,").Append(Environment.NewLine);
            DataGridViewRowCollection dgvCollection = dgvGrid.Rows;
            foreach (DataGridViewRow dr in dgvCollection)
            {
                string value = "";
                value = dr.Cells[0].Value.ToString();
                value = value.Replace(",", " ");
                value = value.TrimEnd();
                sb.Append(value).Append(",");
                value = dr.Cells[1].Value.ToString();
                value = value.Replace(",", " ");
                value = value.TrimEnd();
                value = value + dr.Cells[2].Value.ToString();
                sb.Append(value).Append(",");
                value = dr.Cells[3].Value.ToString();
                value = value.Replace(",", " ");
                value = value.TrimEnd();
                sb.Append(value).Append(",");
                value = dr.Cells[4].Value.ToString();
                value = value.Replace(",", " ");
                value = value.TrimEnd();
                sb.Append(value).Append(",");
                value = dr.Cells[5].Value.ToString();
                value = value.Replace(",", " ");
                value = value.TrimEnd();
                sb.Append(value).Append(",");
                value = dr.Cells[6].Value.ToString();
                value = value.Replace(",", " ");
                value = value.TrimEnd();
                sb.Append(value).Append(",");
                value = dr.Cells[7].Value.ToString();
                value = value.Replace(",", " ");
                value = value.TrimEnd();
                sb.Append(value).Append(",").Append(Environment.NewLine);
            }
            string backupFile = Path.Combine(Application.StartupPath, "audit.csv");
            CreateCSVFile(sb.ToString(), backupFile, true);
        }

        private bool CreateCSVFile(string data, string fPath, bool bOpen)
        {
            bool retVal = true;
            if (System.IO.File.Exists(fPath))
            {
                try
                {
                    using (Stream stream = new FileStream(fPath, FileMode.Open))
                        stream.Close();
                }
                catch (IOException ioe)
                {
                    MessageBox.Show("Cannot create the file audit.csv because it is currently open\nWindows info:\n" + ioe.Message, "Create CSV File");
                    return true;
                }
            }
            try
            {
                if (File.Exists(fPath))
                    File.Delete(fPath);
                using (StreamWriter writer = new StreamWriter(fPath))
                    writer.Write(data);
                if (bOpen == true)
                {
                    Process.Start(fPath);
                    //System.Diagnostics.Process.Start("notepad", fPath);
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "CreateCSVFile() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Create CSV File", (int)CriticalLevels.Black);
                uscMD.SystemMessage(smea);
            }
            return retVal;
        }

        private void cmdCSV_Click(object sender, EventArgs e)
        {
            outputCSVData();
        }
    }
}
