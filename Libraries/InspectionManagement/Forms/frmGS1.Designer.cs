
namespace LVS3
{
    partial class frmGS1
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblData = new LVS3.RoundRightLabel();
            this.roundTopLabel3 = new LVS3.RoundLeftLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.roundLeftLabel1 = new LVS3.RoundLeftLabel();
            this.opt3 = new System.Windows.Forms.RadioButton();
            this.opt2 = new System.Windows.Forms.RadioButton();
            this.roundRightPanel1 = new LVS3.RoundRightPanel();
            this.txtData = new System.Windows.Forms.TextBox();
            this.circularLabel1 = new LVS3.CircularLabel();
            this.cmdRefresh = new LVS3.RoundButton();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.cmdOK = new LVS3.RoundButton();
            this.cmdCancel = new LVS3.RoundButton();
            this.cmdShowVDE = new LVS3.RoundButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.roundRightPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.40257F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.59743F));
            this.tableLayoutPanel1.Controls.Add(this.lblData, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.roundTopLabel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.806452F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94.19355F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(467, 279);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblData
            // 
            this.lblData.AutoSize = true;
            this.lblData.BackColor = System.Drawing.Color.White;
            this.lblData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblData.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData.IsLink = false;
            this.lblData.Location = new System.Drawing.Point(170, 4);
            this.lblData.Margin = new System.Windows.Forms.Padding(0, 4, 4, 1);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(293, 45);
            this.lblData.TabIndex = 342;
            this.lblData.Text = "ABC123";
            this.lblData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundTopLabel3
            // 
            this.roundTopLabel3.BackColor = System.Drawing.Color.Silver;
            this.roundTopLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundTopLabel3.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundTopLabel3.IsLink = false;
            this.roundTopLabel3.Location = new System.Drawing.Point(4, 4);
            this.roundTopLabel3.Margin = new System.Windows.Forms.Padding(4, 4, 0, 1);
            this.roundTopLabel3.Name = "roundTopLabel3";
            this.roundTopLabel3.Size = new System.Drawing.Size(166, 45);
            this.roundTopLabel3.TabIndex = 341;
            this.roundTopLabel3.Text = "Additional Data Identified:\r\n(GS1 Barcode)";
            this.roundTopLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 11F);
            this.groupBox1.Location = new System.Drawing.Point(3, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(461, 172);
            this.groupBox1.TabIndex = 343;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Validation Options:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.12312F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.87688F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel2.Controls.Add(this.roundLeftLabel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.opt3, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.opt2, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.roundRightPanel1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.circularLabel1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.cmdRefresh, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 21);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.59799F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 29.14573F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.15578F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(455, 148);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // roundLeftLabel1
            // 
            this.roundLeftLabel1.BackColor = System.Drawing.Color.Silver;
            this.roundLeftLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundLeftLabel1.Font = new System.Drawing.Font("Calibri", 12F);
            this.roundLeftLabel1.IsLink = false;
            this.roundLeftLabel1.Location = new System.Drawing.Point(24, 4);
            this.roundLeftLabel1.Margin = new System.Windows.Forms.Padding(24, 4, 0, 4);
            this.roundLeftLabel1.Name = "roundLeftLabel1";
            this.roundLeftLabel1.Size = new System.Drawing.Size(63, 28);
            this.roundLeftLabel1.TabIndex = 342;
            this.roundLeftLabel1.Text = "Edit";
            this.roundLeftLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // opt3
            // 
            this.opt3.AutoSize = true;
            this.opt3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opt3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opt3.Location = new System.Drawing.Point(103, 92);
            this.opt3.Margin = new System.Windows.Forms.Padding(16, 3, 3, 3);
            this.opt3.Name = "opt3";
            this.opt3.Size = new System.Drawing.Size(273, 53);
            this.opt3.TabIndex = 2;
            this.opt3.TabStop = true;
            this.opt3.Text = "Is NOT Required";
            this.opt3.UseVisualStyleBackColor = true;
            // 
            // opt2
            // 
            this.opt2.AutoSize = true;
            this.opt2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.opt2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opt2.Location = new System.Drawing.Point(103, 39);
            this.opt2.Margin = new System.Windows.Forms.Padding(16, 3, 3, 3);
            this.opt2.Name = "opt2";
            this.opt2.Size = new System.Drawing.Size(273, 47);
            this.opt2.TabIndex = 1;
            this.opt2.TabStop = true;
            this.opt2.Text = "Is Required";
            this.opt2.UseVisualStyleBackColor = true;
            // 
            // roundRightPanel1
            // 
            this.roundRightPanel1.BackColor = System.Drawing.Color.White;
            this.roundRightPanel1.Controls.Add(this.txtData);
            this.roundRightPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundRightPanel1.Location = new System.Drawing.Point(87, 4);
            this.roundRightPanel1.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.roundRightPanel1.Name = "roundRightPanel1";
            this.roundRightPanel1.Size = new System.Drawing.Size(292, 28);
            this.roundRightPanel1.TabIndex = 344;
            // 
            // txtData
            // 
            this.txtData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtData.Font = new System.Drawing.Font("Calibri", 12F);
            this.txtData.Location = new System.Drawing.Point(9, 6);
            this.txtData.Margin = new System.Windows.Forms.Padding(0, 12, 12, 12);
            this.txtData.MaxLength = 40;
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(267, 20);
            this.txtData.TabIndex = 4;
            this.txtData.Text = "ABC";
            // 
            // circularLabel1
            // 
            this.circularLabel1.BackColor = System.Drawing.Color.DodgerBlue;
            this.circularLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.circularLabel1.Font = new System.Drawing.Font("Calibri", 10F);
            this.circularLabel1.ForeColor = System.Drawing.Color.White;
            this.circularLabel1.IsLink = false;
            this.circularLabel1.Location = new System.Drawing.Point(1, 52);
            this.circularLabel1.Margin = new System.Windows.Forms.Padding(1, 16, 1, 16);
            this.circularLabel1.Name = "circularLabel1";
            this.tableLayoutPanel2.SetRowSpan(this.circularLabel1, 2);
            this.circularLabel1.Size = new System.Drawing.Size(85, 80);
            this.circularLabel1.TabIndex = 345;
            this.circularLabel1.Text = "Inspection\r\nOptions";
            this.circularLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmdRefresh
            // 
            this.cmdRefresh.BackColor = System.Drawing.Color.LightGray;
            this.cmdRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdRefresh.FlatAppearance.BorderSize = 0;
            this.cmdRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.cmdRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdRefresh.Font = new System.Drawing.Font("Calibri", 9F);
            this.cmdRefresh.Location = new System.Drawing.Point(380, 1);
            this.cmdRefresh.Margin = new System.Windows.Forms.Padding(1);
            this.cmdRefresh.Name = "cmdRefresh";
            this.cmdRefresh.Size = new System.Drawing.Size(74, 34);
            this.cmdRefresh.TabIndex = 346;
            this.cmdRefresh.Text = "Refresh";
            this.cmdRefresh.UseVisualStyleBackColor = false;
            this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel3, 2);
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.50479F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.94888F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.cmdShowVDE, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cmdOK, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.cmdCancel, 2, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 242);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(461, 34);
            this.tableLayoutPanel3.TabIndex = 344;
            // 
            // cmdOK
            // 
            this.cmdOK.BackColor = System.Drawing.Color.LightGray;
            this.cmdOK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdOK.FlatAppearance.BorderSize = 0;
            this.cmdOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdOK.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.cmdOK.Location = new System.Drawing.Point(167, 2);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(8, 2, 2, 2);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(137, 30);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = false;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.BackColor = System.Drawing.Color.LightGray;
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdCancel.FlatAppearance.BorderSize = 0;
            this.cmdCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.cmdCancel.Location = new System.Drawing.Point(314, 2);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(8, 2, 2, 2);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(145, 30);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = false;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdShowVDE
            // 
            this.cmdShowVDE.BackColor = System.Drawing.Color.LightGray;
            this.cmdShowVDE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdShowVDE.FlatAppearance.BorderSize = 0;
            this.cmdShowVDE.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdShowVDE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdShowVDE.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.cmdShowVDE.Location = new System.Drawing.Point(2, 2);
            this.cmdShowVDE.Margin = new System.Windows.Forms.Padding(2);
            this.cmdShowVDE.Name = "cmdShowVDE";
            this.cmdShowVDE.Size = new System.Drawing.Size(155, 30);
            this.cmdShowVDE.TabIndex = 2;
            this.cmdShowVDE.Text = "Barcode VDE...";
            this.cmdShowVDE.UseVisualStyleBackColor = false;
            this.cmdShowVDE.Click += new System.EventHandler(this.cmdShowVDE_Click);
            // 
            // frmGS1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSlateGray;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(467, 279);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGS1";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Barcode  - Non VDE Data";
            this.TopMost = true;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.roundRightPanel1.ResumeLayout(false);
            this.roundRightPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public RoundRightLabel lblData;
        public RoundLeftLabel roundTopLabel3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RadioButton opt3;
        private System.Windows.Forms.RadioButton opt2;
        public RoundLeftLabel roundLeftLabel1;
        private RoundRightPanel roundRightPanel1;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private RoundButton cmdOK;
        private RoundButton cmdCancel;
        private CircularLabel circularLabel1;
        private RoundButton cmdRefresh;
        private RoundButton cmdShowVDE;
    }
}