namespace LVS3
{
    partial class frmResults
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.dgvGrid = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.roundLabel2 = new LVS3.RoundLabel();
            this.cmdCSV = new LVS3.RoundButton();
            this.cmdClose = new LVS3.RoundButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboUser = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtmFrom = new System.Windows.Forms.DateTimePicker();
            this.dtmTo = new System.Windows.Forms.DateTimePicker();
            this.cboReportPDF = new System.Windows.Forms.ComboBox();
            this.cmdInspection = new LVS3.RoundButton();
            this.cmdAudit = new LVS3.RoundButton();
            this.roundLabel1 = new LVS3.RoundLabel();
            this.uscMD = new LVS3.uscMessageDisplay();
            this.panel1.SuspendLayout();
            this.tlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrid)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tlpMain);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1239, 646);
            this.panel1.TabIndex = 36;
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.dgvGrid, 0, 1);
            this.tlpMain.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tlpMain.Controls.Add(this.uscMD, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 111F));
            this.tlpMain.Size = new System.Drawing.Size(1239, 646);
            this.tlpMain.TabIndex = 36;
            // 
            // dgvGrid
            // 
            this.dgvGrid.AllowUserToAddRows = false;
            this.dgvGrid.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGrid.Location = new System.Drawing.Point(0, 140);
            this.dgvGrid.Margin = new System.Windows.Forms.Padding(0);
            this.dgvGrid.Name = "dgvGrid";
            this.dgvGrid.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvGrid.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvGrid.Size = new System.Drawing.Size(1239, 395);
            this.dgvGrid.TabIndex = 38;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 9;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 233F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel2.Controls.Add(this.roundLabel2, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.cmdCSV, 7, 2);
            this.tableLayoutPanel2.Controls.Add(this.cmdClose, 9, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label5, 5, 2);
            this.tableLayoutPanel2.Controls.Add(this.label4, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.cboUser, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.dtmFrom, 6, 1);
            this.tableLayoutPanel2.Controls.Add(this.dtmTo, 6, 2);
            this.tableLayoutPanel2.Controls.Add(this.cboReportPDF, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.cmdInspection, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.cmdAudit, 7, 1);
            this.tableLayoutPanel2.Controls.Add(this.roundLabel1, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1239, 140);
            this.tableLayoutPanel2.TabIndex = 39;
            // 
            // roundLabel2
            // 
            this.roundLabel2.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.roundLabel2, 3);
            this.roundLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundLabel2.Font = new System.Drawing.Font("Calibri", 9.25F, System.Drawing.FontStyle.Italic);
            this.roundLabel2.IsLink = false;
            this.roundLabel2.Location = new System.Drawing.Point(319, 86);
            this.roundLabel2.Margin = new System.Windows.Forms.Padding(0);
            this.roundLabel2.Name = "roundLabel2";
            this.roundLabel2.Size = new System.Drawing.Size(343, 54);
            this.roundLabel2.TabIndex = 52;
            this.roundLabel2.Text = "The Audit Activity Log is displayed \r\non this page";
            this.roundLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmdCSV
            // 
            this.cmdCSV.BackColor = System.Drawing.Color.Teal;
            this.cmdCSV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdCSV.Enabled = false;
            this.cmdCSV.FlatAppearance.BorderSize = 0;
            this.cmdCSV.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCSV.Font = new System.Drawing.Font("Calibri", 12F);
            this.cmdCSV.ForeColor = System.Drawing.Color.White;
            this.cmdCSV.Location = new System.Drawing.Point(946, 87);
            this.cmdCSV.Margin = new System.Windows.Forms.Padding(8, 1, 8, 4);
            this.cmdCSV.Name = "cmdCSV";
            this.cmdCSV.Size = new System.Drawing.Size(145, 49);
            this.cmdCSV.TabIndex = 50;
            this.cmdCSV.Text = "Save As \r\ncsv file...";
            this.cmdCSV.UseVisualStyleBackColor = false;
            this.cmdCSV.Click += new System.EventHandler(this.cmdCSV_Click);
            // 
            // cmdClose
            // 
            this.cmdClose.BackColor = System.Drawing.Color.Teal;
            this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdClose.FlatAppearance.BorderSize = 0;
            this.cmdClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdClose.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.cmdClose.ForeColor = System.Drawing.Color.White;
            this.cmdClose.Location = new System.Drawing.Point(1107, 34);
            this.cmdClose.Margin = new System.Windows.Forms.Padding(8, 1, 8, 4);
            this.cmdClose.Name = "cmdClose";
            this.tableLayoutPanel2.SetRowSpan(this.cmdClose, 2);
            this.cmdClose.Size = new System.Drawing.Size(124, 102);
            this.cmdClose.TabIndex = 48;
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = false;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.label3, 5);
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(319, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.label3.Size = new System.Drawing.Size(619, 33);
            this.label3.TabIndex = 44;
            this.label3.Text = "Audit Activity Log";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Calibri", 10.25F);
            this.label5.Location = new System.Drawing.Point(666, 86);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label5.Size = new System.Drawing.Size(90, 54);
            this.label5.TabIndex = 43;
            this.label5.Text = "Date To:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Calibri", 10.25F);
            this.label4.Location = new System.Drawing.Point(666, 33);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label4.Size = new System.Drawing.Size(90, 53);
            this.label4.TabIndex = 42;
            this.label4.Text = "Date From:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboUser
            // 
            this.cboUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUser.Font = new System.Drawing.Font("Calibri", 12F);
            this.cboUser.FormattingEnabled = true;
            this.cboUser.Location = new System.Drawing.Point(427, 43);
            this.cboUser.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.cboUser.Name = "cboUser";
            this.cboUser.Size = new System.Drawing.Size(180, 27);
            this.cboUser.Sorted = true;
            this.cboUser.TabIndex = 3;
            this.cboUser.DropDownClosed += new System.EventHandler(this.cboUser_DropDownClosed);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Calibri", 10.25F);
            this.label2.Location = new System.Drawing.Point(323, 37);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 4, 2, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 8, 2, 0);
            this.label2.Size = new System.Drawing.Size(102, 49);
            this.label2.TabIndex = 2;
            this.label2.Text = "User";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(319, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "Inspection Reports";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtmFrom
            // 
            this.dtmFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtmFrom.Font = new System.Drawing.Font("Calibri", 12F);
            this.dtmFrom.Location = new System.Drawing.Point(759, 45);
            this.dtmFrom.Margin = new System.Windows.Forms.Padding(1, 12, 1, 1);
            this.dtmFrom.MinDate = new System.DateTime(2022, 1, 1, 0, 0, 0, 0);
            this.dtmFrom.Name = "dtmFrom";
            this.dtmFrom.Size = new System.Drawing.Size(178, 27);
            this.dtmFrom.TabIndex = 39;
            // 
            // dtmTo
            // 
            this.dtmTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtmTo.Font = new System.Drawing.Font("Calibri", 12F);
            this.dtmTo.Location = new System.Drawing.Point(759, 98);
            this.dtmTo.Margin = new System.Windows.Forms.Padding(1, 12, 1, 1);
            this.dtmTo.MinDate = new System.DateTime(2022, 1, 1, 0, 0, 0, 0);
            this.dtmTo.Name = "dtmTo";
            this.dtmTo.Size = new System.Drawing.Size(178, 27);
            this.dtmTo.TabIndex = 40;
            // 
            // cboReportPDF
            // 
            this.cboReportPDF.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboReportPDF.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboReportPDF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboReportPDF.DropDownWidth = 280;
            this.cboReportPDF.Font = new System.Drawing.Font("Calibri", 12F);
            this.cboReportPDF.FormattingEnabled = true;
            this.cboReportPDF.ItemHeight = 19;
            this.cboReportPDF.Location = new System.Drawing.Point(1, 41);
            this.cboReportPDF.Margin = new System.Windows.Forms.Padding(1, 8, 1, 1);
            this.cboReportPDF.Name = "cboReportPDF";
            this.cboReportPDF.Size = new System.Drawing.Size(231, 27);
            this.cboReportPDF.Sorted = true;
            this.cboReportPDF.TabIndex = 45;
            this.cboReportPDF.DropDownClosed += new System.EventHandler(this.cboReportPDF_DropDownClosed);
            // 
            // cmdInspection
            // 
            this.cmdInspection.BackColor = System.Drawing.Color.Teal;
            this.cmdInspection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdInspection.Enabled = false;
            this.cmdInspection.FlatAppearance.BorderSize = 0;
            this.cmdInspection.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdInspection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdInspection.Font = new System.Drawing.Font("Calibri", 12F);
            this.cmdInspection.ForeColor = System.Drawing.Color.White;
            this.cmdInspection.Location = new System.Drawing.Point(241, 34);
            this.cmdInspection.Margin = new System.Windows.Forms.Padding(8, 1, 8, 1);
            this.cmdInspection.Name = "cmdInspection";
            this.cmdInspection.Size = new System.Drawing.Size(70, 51);
            this.cmdInspection.TabIndex = 46;
            this.cmdInspection.Text = "Open Report";
            this.cmdInspection.UseVisualStyleBackColor = false;
            this.cmdInspection.Click += new System.EventHandler(this.cmdInspection_Click);
            // 
            // cmdAudit
            // 
            this.cmdAudit.BackColor = System.Drawing.Color.Teal;
            this.cmdAudit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdAudit.FlatAppearance.BorderSize = 0;
            this.cmdAudit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdAudit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdAudit.Font = new System.Drawing.Font("Calibri", 12F);
            this.cmdAudit.ForeColor = System.Drawing.Color.White;
            this.cmdAudit.Location = new System.Drawing.Point(946, 34);
            this.cmdAudit.Margin = new System.Windows.Forms.Padding(8, 1, 8, 4);
            this.cmdAudit.Name = "cmdAudit";
            this.cmdAudit.Size = new System.Drawing.Size(145, 48);
            this.cmdAudit.TabIndex = 47;
            this.cmdAudit.Text = "View Data";
            this.cmdAudit.UseVisualStyleBackColor = false;
            this.cmdAudit.Click += new System.EventHandler(this.cmdAudit_Click);
            // 
            // roundLabel1
            // 
            this.roundLabel1.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.roundLabel1, 2);
            this.roundLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundLabel1.Font = new System.Drawing.Font("Calibri", 9.25F, System.Drawing.FontStyle.Italic);
            this.roundLabel1.IsLink = false;
            this.roundLabel1.Location = new System.Drawing.Point(0, 86);
            this.roundLabel1.Margin = new System.Windows.Forms.Padding(0);
            this.roundLabel1.Name = "roundLabel1";
            this.roundLabel1.Size = new System.Drawing.Size(319, 54);
            this.roundLabel1.TabIndex = 49;
            this.roundLabel1.Text = "The Inspection Report will open in the default \r\nPDF document reader";
            this.roundLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uscMD
            // 
            this.uscMD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uscMD.Location = new System.Drawing.Point(1, 536);
            this.uscMD.Margin = new System.Windows.Forms.Padding(1);
            this.uscMD.Name = "uscMD";
            this.uscMD.Size = new System.Drawing.Size(1237, 109);
            this.uscMD.TabIndex = 40;
            // 
            // frmResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1239, 646);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "frmResults";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporting";
            this.ResizeEnd += new System.EventHandler(this.frmResults_ResizeEnd);
            this.panel1.ResumeLayout(false);
            this.tlpMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrid)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.DataGridView dgvGrid;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtmFrom;
        private System.Windows.Forms.DateTimePicker dtmTo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboReportPDF;
        private LVS3.RoundButton cmdInspection;
        private LVS3.RoundButton cmdAudit;
        private LVS3.RoundButton cmdClose;
        private LVS3.RoundLabel roundLabel1;
        private LVS3.RoundButton cmdCSV;
        private LVS3.uscMessageDisplay uscMD;
        private RoundLabel roundLabel2;
    }
}