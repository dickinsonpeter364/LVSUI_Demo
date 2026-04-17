
namespace LVS3
{
    partial class frmESig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmESig));
            this.lblInformation = new System.Windows.Forms.Label();
            this.txtUserReason = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.lblSelectedMed = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblReason = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblSigMeaning = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.cmdOK = new LVS3.RoundButton();
            this.cmdCancel = new LVS3.RoundButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInformation
            // 
            this.lblInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInformation.BackColor = System.Drawing.Color.Transparent;
            this.lblInformation.Font = new System.Drawing.Font("Calibri", 11F);
            this.lblInformation.Location = new System.Drawing.Point(22, 238);
            this.lblInformation.Name = "lblInformation";
            this.lblInformation.Size = new System.Drawing.Size(225, 22);
            this.lblInformation.TabIndex = 78;
            this.lblInformation.Text = "User Information:";
            // 
            // txtUserReason
            // 
            this.txtUserReason.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserReason.BackColor = System.Drawing.Color.White;
            this.txtUserReason.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserReason.Font = new System.Drawing.Font("Calibri", 9F);
            this.txtUserReason.Location = new System.Drawing.Point(25, 260);
            this.txtUserReason.Margin = new System.Windows.Forms.Padding(0);
            this.txtUserReason.MaxLength = 1000;
            this.txtUserReason.Multiline = true;
            this.txtUserReason.Name = "txtUserReason";
            this.txtUserReason.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUserReason.Size = new System.Drawing.Size(296, 105);
            this.txtUserReason.TabIndex = 1;
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Font = new System.Drawing.Font("Calibri", 12F);
            this.txtPassword.Location = new System.Drawing.Point(359, 223);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.ShortcutsEnabled = false;
            this.txtPassword.Size = new System.Drawing.Size(202, 27);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.Font = new System.Drawing.Font("Calibri", 12F);
            this.txtUsername.Location = new System.Drawing.Point(359, 167);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(202, 27);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtUsername_KeyPress);
            // 
            // Label2
            // 
            this.Label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label2.BackColor = System.Drawing.Color.Transparent;
            this.Label2.Font = new System.Drawing.Font("Calibri", 11F);
            this.Label2.Location = new System.Drawing.Point(359, 202);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(152, 19);
            this.Label2.TabIndex = 69;
            this.Label2.Text = "Password";
            // 
            // Label1
            // 
            this.Label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label1.BackColor = System.Drawing.Color.Transparent;
            this.Label1.Font = new System.Drawing.Font("Calibri", 11F);
            this.Label1.Location = new System.Drawing.Point(359, 147);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(152, 19);
            this.Label1.TabIndex = 68;
            this.Label1.Text = "Username";
            // 
            // lblSelectedMed
            // 
            this.lblSelectedMed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelectedMed.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectedMed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblSelectedMed.ForeColor = System.Drawing.Color.DarkRed;
            this.lblSelectedMed.Location = new System.Drawing.Point(60, 26);
            this.lblSelectedMed.Name = "lblSelectedMed";
            this.lblSelectedMed.Size = new System.Drawing.Size(518, 20);
            this.lblSelectedMed.TabIndex = 81;
            this.lblSelectedMed.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblSelectedMed.Visible = false;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 11F);
            this.label7.Location = new System.Drawing.Point(23, 147);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 18);
            this.label7.TabIndex = 82;
            this.label7.Text = "Errors /  Reasons:";
            // 
            // lblReason
            // 
            this.lblReason.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReason.BackColor = System.Drawing.Color.Gainsboro;
            this.lblReason.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReason.Font = new System.Drawing.Font("Calibri", 9F);
            this.lblReason.Location = new System.Drawing.Point(25, 167);
            this.lblReason.Name = "lblReason";
            this.lblReason.Size = new System.Drawing.Size(296, 60);
            this.lblReason.TabIndex = 0;
            this.lblReason.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblSigMeaning, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.Label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Label3, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(589, 144);
            this.tableLayoutPanel1.TabIndex = 85;
            // 
            // lblSigMeaning
            // 
            this.lblSigMeaning.BackColor = System.Drawing.Color.Transparent;
            this.lblSigMeaning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSigMeaning.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSigMeaning.ForeColor = System.Drawing.Color.Black;
            this.lblSigMeaning.Location = new System.Drawing.Point(3, 96);
            this.lblSigMeaning.Name = "lblSigMeaning";
            this.lblSigMeaning.Size = new System.Drawing.Size(583, 48);
            this.lblSigMeaning.TabIndex = 81;
            this.lblSigMeaning.Text = "<<meaning>>";
            this.lblSigMeaning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label5
            // 
            this.Label5.BackColor = System.Drawing.Color.Transparent;
            this.Label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label5.Font = new System.Drawing.Font("Calibri", 10F);
            this.Label5.ForeColor = System.Drawing.Color.Black;
            this.Label5.Location = new System.Drawing.Point(0, 48);
            this.Label5.Margin = new System.Windows.Forms.Padding(0);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(589, 48);
            this.Label5.TabIndex = 80;
            this.Label5.Text = "By providing your username and password you agree to the following statement:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.Color.Transparent;
            this.Label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label3.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.Label3.Location = new System.Drawing.Point(0, 0);
            this.Label3.Margin = new System.Windows.Forms.Padding(0);
            this.Label3.Name = "Label3";
            this.Label3.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.Label3.Size = new System.Drawing.Size(589, 48);
            this.Label3.TabIndex = 76;
            this.Label3.Text = "Signature Required";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.BackColor = System.Drawing.Color.Teal;
            this.cmdOK.FlatAppearance.BorderSize = 0;
            this.cmdOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdOK.Font = new System.Drawing.Font("Calibri", 13F);
            this.cmdOK.Location = new System.Drawing.Point(359, 309);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(94, 55);
            this.cmdOK.TabIndex = 84;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = false;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.BackColor = System.Drawing.Color.Teal;
            this.cmdCancel.FlatAppearance.BorderSize = 0;
            this.cmdCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Font = new System.Drawing.Font("Calibri", 13F);
            this.cmdCancel.Location = new System.Drawing.Point(467, 309);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(94, 55);
            this.cmdCancel.TabIndex = 83;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = false;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // frmESig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(589, 386);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.txtUserReason);
            this.Controls.Add(this.lblReason);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.lblSelectedMed);
            this.Controls.Add(this.lblInformation);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmESig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "E-Signature";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmEsig_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtUserReason;
        private System.Windows.Forms.TextBox txtPassword;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label1;
        public System.Windows.Forms.Label lblSelectedMed;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblReason;
        public System.Windows.Forms.Label lblInformation;
        protected internal System.Windows.Forms.TextBox txtUsername;
        private RoundButton cmdCancel;
        private RoundButton cmdOK;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblSigMeaning;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.Label Label3;
    }
}