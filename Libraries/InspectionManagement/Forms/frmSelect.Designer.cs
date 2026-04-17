namespace LVS3
{
    partial class frmSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelect));
            this.tlpFrame = new System.Windows.Forms.TableLayoutPanel();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pbLabel = new System.Windows.Forms.PictureBox();
            this.cmdResetAlarm = new LVS3.RoundButton();
            this.lblStationID = new LVS3.RoundLabel();
            this.tlpHMI = new System.Windows.Forms.TableLayoutPanel();
            this.uscMD = new LVS3.uscMessageDisplay();
            this.roundLabel2 = new LVS3.RoundLabel();
            this.cmdCheckLPN = new LVS3.RoundButton();
            this.cmdOK = new LVS3.RoundButton();
            this.label2 = new System.Windows.Forms.Label();
            this.pbScanner = new System.Windows.Forms.PictureBox();
            this.cmdClear = new LVS3.RoundButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLIN = new System.Windows.Forms.Label();
            this.lblerrorReel = new System.Windows.Forms.Label();
            this.txtReel = new System.Windows.Forms.TextBox();
            this.cmdCancel = new LVS3.RoundButton();
            this.optTrain = new LVS3.RoundRadioButton();
            this.optInspect = new LVS3.RoundRadioButton();
            this.roundPanel1 = new LVS3.RoundPanel();
            this.tlpManual = new System.Windows.Forms.TableLayoutPanel();
            this.cmdReports = new LVS3.RoundButton();
            this.sbForward = new System.Windows.Forms.HScrollBar();
            this.cmdReverse = new LVS3.RoundButton();
            this.cmdForward = new LVS3.RoundButton();
            this.cmdStopManual = new LVS3.RoundButton();
            this.sbReverse = new System.Windows.Forms.HScrollBar();
            this.tlpDock = new System.Windows.Forms.TableLayoutPanel();
            this.lblBCBottom = new LVS3.RoundBottomLabel();
            this.lblBCTop = new LVS3.RoundTopLabel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tlpFrame.SuspendLayout();
            this.tlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbScanner)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.roundPanel1.SuspendLayout();
            this.tlpManual.SuspendLayout();
            this.tlpDock.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpFrame
            // 
            this.tlpFrame.BackColor = System.Drawing.Color.Gainsboro;
            this.tlpFrame.ColumnCount = 6;
            this.tlpFrame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpFrame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tlpFrame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 932F));
            this.tlpFrame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpFrame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.tlpFrame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpFrame.Controls.Add(this.tlpMain, 2, 1);
            this.tlpFrame.Controls.Add(this.roundPanel1, 4, 1);
            this.tlpFrame.Controls.Add(this.tlpDock, 1, 1);
            this.tlpFrame.Controls.Add(this.lblVersion, 4, 0);
            this.tlpFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFrame.Location = new System.Drawing.Point(0, 0);
            this.tlpFrame.Margin = new System.Windows.Forms.Padding(0);
            this.tlpFrame.Name = "tlpFrame";
            this.tlpFrame.RowCount = 3;
            this.tlpFrame.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpFrame.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 623F));
            this.tlpFrame.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpFrame.Size = new System.Drawing.Size(1306, 831);
            this.tlpFrame.TabIndex = 102;
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.LightSlateGray;
            this.tlpMain.ColumnCount = 5;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.7957F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.881721F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.79281F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.3103F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.11989F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Controls.Add(this.pbLabel, 2, 3);
            this.tlpMain.Controls.Add(this.cmdResetAlarm, 0, 6);
            this.tlpMain.Controls.Add(this.lblStationID, 0, 0);
            this.tlpMain.Controls.Add(this.tlpHMI, 0, 9);
            this.tlpMain.Controls.Add(this.uscMD, 0, 8);
            this.tlpMain.Controls.Add(this.roundLabel2, 2, 0);
            this.tlpMain.Controls.Add(this.cmdCheckLPN, 4, 2);
            this.tlpMain.Controls.Add(this.cmdOK, 4, 6);
            this.tlpMain.Controls.Add(this.label2, 1, 2);
            this.tlpMain.Controls.Add(this.pbScanner, 1, 3);
            this.tlpMain.Controls.Add(this.cmdClear, 4, 3);
            this.tlpMain.Controls.Add(this.tableLayoutPanel2, 2, 2);
            this.tlpMain.Controls.Add(this.cmdCancel, 4, 0);
            this.tlpMain.Controls.Add(this.optTrain, 2, 6);
            this.tlpMain.Controls.Add(this.optInspect, 3, 6);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(124, 104);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 10;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26.9962F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 21.673F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.70569F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 39.46488F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 173F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(930, 623);
            this.tlpMain.TabIndex = 102;
            // 
            // pbLabel
            // 
            this.pbLabel.BackColor = System.Drawing.Color.LightSlateGray;
            this.pbLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpMain.SetColumnSpan(this.pbLabel, 2);
            this.pbLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbLabel.Location = new System.Drawing.Point(191, 137);
            this.pbLabel.Margin = new System.Windows.Forms.Padding(8, 2, 8, 2);
            this.pbLabel.Name = "pbLabel";
            this.pbLabel.Padding = new System.Windows.Forms.Padding(2);
            this.tlpMain.SetRowSpan(this.pbLabel, 3);
            this.pbLabel.Size = new System.Drawing.Size(533, 166);
            this.pbLabel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLabel.TabIndex = 359;
            this.pbLabel.TabStop = false;
            this.pbLabel.Visible = false;
            // 
            // cmdResetAlarm
            // 
            this.cmdResetAlarm.BackColor = System.Drawing.Color.LightGray;
            this.tlpMain.SetColumnSpan(this.cmdResetAlarm, 2);
            this.cmdResetAlarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdResetAlarm.Enabled = false;
            this.cmdResetAlarm.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.cmdResetAlarm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdResetAlarm.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdResetAlarm.ForeColor = System.Drawing.Color.White;
            this.cmdResetAlarm.Image = ((System.Drawing.Image)(resources.GetObject("cmdResetAlarm.Image")));
            this.cmdResetAlarm.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.cmdResetAlarm.Location = new System.Drawing.Point(4, 313);
            this.cmdResetAlarm.Margin = new System.Windows.Forms.Padding(4, 8, 4, 4);
            this.cmdResetAlarm.Name = "cmdResetAlarm";
            this.tlpMain.SetRowSpan(this.cmdResetAlarm, 2);
            this.cmdResetAlarm.Size = new System.Drawing.Size(175, 94);
            this.cmdResetAlarm.TabIndex = 340;
            this.cmdResetAlarm.TabStop = false;
            this.cmdResetAlarm.Text = "Alarm Reset";
            this.cmdResetAlarm.UseVisualStyleBackColor = false;
            this.cmdResetAlarm.Click += new System.EventHandler(this.cmdResetAlarm_Click);
            // 
            // lblStationID
            // 
            this.lblStationID.BackColor = System.Drawing.Color.CornflowerBlue;
            this.lblStationID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStationID.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStationID.ForeColor = System.Drawing.Color.White;
            this.lblStationID.IsLink = false;
            this.lblStationID.Location = new System.Drawing.Point(4, 4);
            this.lblStationID.Margin = new System.Windows.Forms.Padding(4, 4, 2, 2);
            this.lblStationID.Name = "lblStationID";
            this.lblStationID.Size = new System.Drawing.Size(113, 40);
            this.lblStationID.TabIndex = 339;
            this.lblStationID.Text = "station";
            this.lblStationID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpHMI
            // 
            this.tlpHMI.ColumnCount = 4;
            this.tlpMain.SetColumnSpan(this.tlpHMI, 5);
            this.tlpHMI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHMI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHMI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHMI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpHMI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpHMI.Location = new System.Drawing.Point(2, 584);
            this.tlpHMI.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            this.tlpHMI.Name = "tlpHMI";
            this.tlpHMI.RowCount = 1;
            this.tlpHMI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHMI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tlpHMI.Size = new System.Drawing.Size(924, 39);
            this.tlpHMI.TabIndex = 338;
            // 
            // uscMD
            // 
            this.tlpMain.SetColumnSpan(this.uscMD, 5);
            this.uscMD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uscMD.Location = new System.Drawing.Point(0, 411);
            this.uscMD.Margin = new System.Windows.Forms.Padding(0);
            this.uscMD.Name = "uscMD";
            this.uscMD.Size = new System.Drawing.Size(930, 173);
            this.uscMD.TabIndex = 337;
            // 
            // roundLabel2
            // 
            this.roundLabel2.BackColor = System.Drawing.Color.CornflowerBlue;
            this.tlpMain.SetColumnSpan(this.roundLabel2, 2);
            this.roundLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundLabel2.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundLabel2.ForeColor = System.Drawing.Color.White;
            this.roundLabel2.IsLink = false;
            this.roundLabel2.Location = new System.Drawing.Point(185, 4);
            this.roundLabel2.Margin = new System.Windows.Forms.Padding(2, 4, 2, 2);
            this.roundLabel2.Name = "roundLabel2";
            this.roundLabel2.Size = new System.Drawing.Size(545, 40);
            this.roundLabel2.TabIndex = 125;
            this.roundLabel2.Text = "Label Verification System III";
            this.roundLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmdCheckLPN
            // 
            this.cmdCheckLPN.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.cmdCheckLPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdCheckLPN.Enabled = false;
            this.cmdCheckLPN.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.cmdCheckLPN.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdCheckLPN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCheckLPN.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCheckLPN.Location = new System.Drawing.Point(740, 70);
            this.cmdCheckLPN.Margin = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.cmdCheckLPN.Name = "cmdCheckLPN";
            this.cmdCheckLPN.Size = new System.Drawing.Size(182, 57);
            this.cmdCheckLPN.TabIndex = 120;
            this.cmdCheckLPN.TabStop = false;
            this.cmdCheckLPN.Text = "Check LPN";
            this.cmdCheckLPN.UseVisualStyleBackColor = false;
            this.cmdCheckLPN.Click += new System.EventHandler(this.cmdCheckLPN_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.cmdOK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdOK.Enabled = false;
            this.cmdOK.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.cmdOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdOK.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.Location = new System.Drawing.Point(740, 313);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(8);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(182, 79);
            this.cmdOK.TabIndex = 104;
            this.cmdOK.TabStop = false;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = false;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(119, 78);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 57);
            this.label2.TabIndex = 110;
            this.label2.Text = "Reel";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pbScanner
            // 
            this.pbScanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbScanner.Enabled = false;
            this.pbScanner.Image = ((System.Drawing.Image)(resources.GetObject("pbScanner.Image")));
            this.pbScanner.Location = new System.Drawing.Point(127, 143);
            this.pbScanner.Margin = new System.Windows.Forms.Padding(8);
            this.pbScanner.Name = "pbScanner";
            this.pbScanner.Size = new System.Drawing.Size(48, 36);
            this.pbScanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbScanner.TabIndex = 113;
            this.pbScanner.TabStop = false;
            // 
            // cmdClear
            // 
            this.cmdClear.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.cmdClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdClear.Enabled = false;
            this.cmdClear.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.cmdClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdClear.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdClear.Location = new System.Drawing.Point(740, 143);
            this.cmdClear.Margin = new System.Windows.Forms.Padding(8);
            this.cmdClear.Name = "cmdClear";
            this.tlpMain.SetRowSpan(this.cmdClear, 3);
            this.cmdClear.Size = new System.Drawing.Size(182, 154);
            this.cmdClear.TabIndex = 97;
            this.cmdClear.TabStop = false;
            this.cmdClear.Text = "Clear";
            this.cmdClear.UseVisualStyleBackColor = false;
            this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tlpMain.SetColumnSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.lblLIN, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblerrorReel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtReel, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(183, 70);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 43.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 56.66667F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(549, 65);
            this.tableLayoutPanel2.TabIndex = 124;
            // 
            // lblLIN
            // 
            this.lblLIN.BackColor = System.Drawing.Color.Transparent;
            this.lblLIN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLIN.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLIN.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblLIN.Location = new System.Drawing.Point(275, 29);
            this.lblLIN.Margin = new System.Windows.Forms.Padding(1);
            this.lblLIN.Name = "lblLIN";
            this.lblLIN.Size = new System.Drawing.Size(273, 35);
            this.lblLIN.TabIndex = 117;
            this.lblLIN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblerrorReel
            // 
            this.lblerrorReel.BackColor = System.Drawing.Color.Transparent;
            this.lblerrorReel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblerrorReel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblerrorReel.ForeColor = System.Drawing.Color.Maroon;
            this.lblerrorReel.Location = new System.Drawing.Point(1, 29);
            this.lblerrorReel.Margin = new System.Windows.Forms.Padding(1);
            this.lblerrorReel.Name = "lblerrorReel";
            this.lblerrorReel.Size = new System.Drawing.Size(272, 35);
            this.lblerrorReel.TabIndex = 116;
            this.lblerrorReel.Text = "  ";
            this.lblerrorReel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtReel
            // 
            this.txtReel.AcceptsTab = true;
            this.txtReel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel2.SetColumnSpan(this.txtReel, 2);
            this.txtReel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReel.Enabled = false;
            this.txtReel.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReel.Location = new System.Drawing.Point(1, 1);
            this.txtReel.Margin = new System.Windows.Forms.Padding(1, 1, 1, 2);
            this.txtReel.Name = "txtReel";
            this.txtReel.ReadOnly = true;
            this.txtReel.Size = new System.Drawing.Size(547, 30);
            this.txtReel.TabIndex = 0;
            this.txtReel.Tag = "";
            this.txtReel.Text = "System loading ...please wait";
            this.txtReel.WordWrap = false;
            this.txtReel.TextChanged += new System.EventHandler(this.txtReel_TextChanged);
            this.txtReel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtReel_KeyPress);
            // 
            // cmdCancel
            // 
            this.cmdCancel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.cmdCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdCancel.Enabled = false;
            this.cmdCancel.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Font = new System.Drawing.Font("Calibri", 14F);
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.cmdCancel.Location = new System.Drawing.Point(735, 3);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(192, 40);
            this.cmdCancel.TabIndex = 341;
            this.cmdCancel.Text = "Exit Application";
            this.cmdCancel.UseVisualStyleBackColor = false;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // optTrain
            // 
            this.optTrain.Appearance = System.Windows.Forms.Appearance.Button;
            this.optTrain.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.optTrain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optTrain.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.optTrain.FlatAppearance.BorderSize = 0;
            this.optTrain.FlatAppearance.CheckedBackColor = System.Drawing.Color.LimeGreen;
            this.optTrain.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.optTrain.FlatAppearance.MouseOverBackColor = System.Drawing.Color.PaleGreen;
            this.optTrain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optTrain.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optTrain.Location = new System.Drawing.Point(191, 313);
            this.optTrain.Margin = new System.Windows.Forms.Padding(8);
            this.optTrain.Name = "optTrain";
            this.optTrain.Size = new System.Drawing.Size(279, 79);
            this.optTrain.TabIndex = 356;
            this.optTrain.Text = "Train Label";
            this.optTrain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optTrain.UseVisualStyleBackColor = false;
            this.optTrain.Click += new System.EventHandler(this.optionSelected_Click);
            // 
            // optInspect
            // 
            this.optInspect.Appearance = System.Windows.Forms.Appearance.Button;
            this.optInspect.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.optInspect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optInspect.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.optInspect.FlatAppearance.BorderSize = 0;
            this.optInspect.FlatAppearance.CheckedBackColor = System.Drawing.Color.LimeGreen;
            this.optInspect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.optInspect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.PaleGreen;
            this.optInspect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optInspect.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optInspect.Location = new System.Drawing.Point(486, 313);
            this.optInspect.Margin = new System.Windows.Forms.Padding(8);
            this.optInspect.Name = "optInspect";
            this.optInspect.Size = new System.Drawing.Size(238, 79);
            this.optInspect.TabIndex = 357;
            this.optInspect.Text = "Inspect Label";
            this.optInspect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optInspect.UseVisualStyleBackColor = false;
            this.optInspect.Click += new System.EventHandler(this.optionSelected_Click);
            // 
            // roundPanel1
            // 
            this.roundPanel1.BackColor = System.Drawing.Color.LightSlateGray;
            this.roundPanel1.Controls.Add(this.tlpManual);
            this.roundPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundPanel1.Location = new System.Drawing.Point(1085, 104);
            this.roundPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel1.Name = "roundPanel1";
            this.roundPanel1.Size = new System.Drawing.Size(190, 623);
            this.roundPanel1.TabIndex = 103;
            // 
            // tlpManual
            // 
            this.tlpManual.BackColor = System.Drawing.Color.LightSlateGray;
            this.tlpManual.ColumnCount = 1;
            this.tlpManual.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpManual.Controls.Add(this.cmdReports, 0, 0);
            this.tlpManual.Controls.Add(this.sbForward, 0, 2);
            this.tlpManual.Controls.Add(this.cmdReverse, 0, 3);
            this.tlpManual.Controls.Add(this.cmdForward, 0, 1);
            this.tlpManual.Controls.Add(this.cmdStopManual, 0, 5);
            this.tlpManual.Controls.Add(this.sbReverse, 0, 4);
            this.tlpManual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpManual.Location = new System.Drawing.Point(0, 0);
            this.tlpManual.Margin = new System.Windows.Forms.Padding(8);
            this.tlpManual.Name = "tlpManual";
            this.tlpManual.Padding = new System.Windows.Forms.Padding(8);
            this.tlpManual.RowCount = 6;
            this.tlpManual.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tlpManual.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpManual.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpManual.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpManual.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpManual.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tlpManual.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpManual.Size = new System.Drawing.Size(190, 623);
            this.tlpManual.TabIndex = 332;
            // 
            // cmdReports
            // 
            this.cmdReports.BackColor = System.Drawing.Color.LightGray;
            this.cmdReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdReports.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.cmdReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdReports.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdReports.ForeColor = System.Drawing.Color.Black;
            this.cmdReports.Image = ((System.Drawing.Image)(resources.GetObject("cmdReports.Image")));
            this.cmdReports.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.cmdReports.Location = new System.Drawing.Point(8, 16);
            this.cmdReports.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this.cmdReports.Name = "cmdReports";
            this.cmdReports.Size = new System.Drawing.Size(174, 54);
            this.cmdReports.TabIndex = 338;
            this.cmdReports.Text = "Reports";
            this.cmdReports.UseVisualStyleBackColor = false;
            this.cmdReports.Click += new System.EventHandler(this.cmdReports_Click);
            // 
            // sbForward
            // 
            this.sbForward.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sbForward.LargeChange = 5;
            this.sbForward.Location = new System.Drawing.Point(8, 221);
            this.sbForward.Maximum = 154;
            this.sbForward.Minimum = 10;
            this.sbForward.Name = "sbForward";
            this.sbForward.Size = new System.Drawing.Size(174, 24);
            this.sbForward.SmallChange = 5;
            this.sbForward.TabIndex = 335;
            this.sbForward.Value = 10;
            this.sbForward.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScroll_Scroll);
            // 
            // cmdReverse
            // 
            this.cmdReverse.BackColor = System.Drawing.Color.LightGray;
            this.cmdReverse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdReverse.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.cmdReverse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdReverse.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdReverse.ForeColor = System.Drawing.Color.Black;
            this.cmdReverse.Image = ((System.Drawing.Image)(resources.GetObject("cmdReverse.Image")));
            this.cmdReverse.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdReverse.Location = new System.Drawing.Point(8, 261);
            this.cmdReverse.Margin = new System.Windows.Forms.Padding(0, 16, 0, 4);
            this.cmdReverse.Name = "cmdReverse";
            this.cmdReverse.Size = new System.Drawing.Size(174, 127);
            this.cmdReverse.TabIndex = 331;
            this.cmdReverse.Text = "Rewind";
            this.cmdReverse.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.cmdReverse.UseVisualStyleBackColor = false;
            this.cmdReverse.Click += new System.EventHandler(this.cmdReverse_Click);
            // 
            // cmdForward
            // 
            this.cmdForward.BackColor = System.Drawing.Color.LightGray;
            this.cmdForward.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdForward.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.cmdForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdForward.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdForward.ForeColor = System.Drawing.Color.Black;
            this.cmdForward.Image = ((System.Drawing.Image)(resources.GetObject("cmdForward.Image")));
            this.cmdForward.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdForward.Location = new System.Drawing.Point(8, 82);
            this.cmdForward.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this.cmdForward.Name = "cmdForward";
            this.cmdForward.Size = new System.Drawing.Size(174, 135);
            this.cmdForward.TabIndex = 330;
            this.cmdForward.Text = "Wind";
            this.cmdForward.UseVisualStyleBackColor = false;
            this.cmdForward.Click += new System.EventHandler(this.cmdForward_Click);
            // 
            // cmdStopManual
            // 
            this.cmdStopManual.BackColor = System.Drawing.Color.Red;
            this.cmdStopManual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdStopManual.Enabled = false;
            this.cmdStopManual.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.cmdStopManual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdStopManual.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdStopManual.ForeColor = System.Drawing.Color.Black;
            this.cmdStopManual.Image = ((System.Drawing.Image)(resources.GetObject("cmdStopManual.Image")));
            this.cmdStopManual.Location = new System.Drawing.Point(8, 432);
            this.cmdStopManual.Margin = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this.cmdStopManual.Name = "cmdStopManual";
            this.cmdStopManual.Size = new System.Drawing.Size(174, 183);
            this.cmdStopManual.TabIndex = 321;
            this.cmdStopManual.UseVisualStyleBackColor = false;
            this.cmdStopManual.Click += new System.EventHandler(this.cmdStopManual_Click);
            // 
            // sbReverse
            // 
            this.sbReverse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sbReverse.LargeChange = 5;
            this.sbReverse.Location = new System.Drawing.Point(8, 392);
            this.sbReverse.Maximum = 154;
            this.sbReverse.Minimum = 10;
            this.sbReverse.Name = "sbReverse";
            this.sbReverse.Size = new System.Drawing.Size(174, 24);
            this.sbReverse.SmallChange = 5;
            this.sbReverse.TabIndex = 334;
            this.sbReverse.Value = 10;
            this.sbReverse.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScroll_Scroll);
            // 
            // tlpDock
            // 
            this.tlpDock.BackColor = System.Drawing.Color.LightSlateGray;
            this.tlpDock.ColumnCount = 1;
            this.tlpDock.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDock.Controls.Add(this.lblBCBottom, 0, 2);
            this.tlpDock.Controls.Add(this.lblBCTop, 0, 0);
            this.tlpDock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDock.Location = new System.Drawing.Point(31, 104);
            this.tlpDock.Margin = new System.Windows.Forms.Padding(0);
            this.tlpDock.Name = "tlpDock";
            this.tlpDock.RowCount = 2;
            this.tlpDock.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tlpDock.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDock.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tlpDock.Size = new System.Drawing.Size(93, 623);
            this.tlpDock.TabIndex = 105;
            // 
            // lblBCBottom
            // 
            this.lblBCBottom.BackColor = System.Drawing.Color.LightSlateGray;
            this.lblBCBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBCBottom.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBCBottom.ForeColor = System.Drawing.Color.White;
            this.lblBCBottom.IsLink = false;
            this.lblBCBottom.Location = new System.Drawing.Point(4, 585);
            this.lblBCBottom.Margin = new System.Windows.Forms.Padding(4, 0, 2, 2);
            this.lblBCBottom.Name = "lblBCBottom";
            this.lblBCBottom.Size = new System.Drawing.Size(87, 36);
            this.lblBCBottom.TabIndex = 341;
            this.lblBCBottom.Text = "loading...";
            this.lblBCBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBCTop
            // 
            this.lblBCTop.BackColor = System.Drawing.Color.CornflowerBlue;
            this.lblBCTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBCTop.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBCTop.ForeColor = System.Drawing.Color.White;
            this.lblBCTop.IsLink = false;
            this.lblBCTop.Location = new System.Drawing.Point(4, 4);
            this.lblBCTop.Margin = new System.Windows.Forms.Padding(4, 4, 2, 0);
            this.lblBCTop.Name = "lblBCTop";
            this.lblBCTop.Size = new System.Drawing.Size(87, 40);
            this.lblBCTop.TabIndex = 340;
            this.lblBCTop.Text = "Backing Camera";
            this.lblBCTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVersion.Font = new System.Drawing.Font("Calibri", 11F);
            this.lblVersion.Location = new System.Drawing.Point(1085, 0);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lblVersion.Size = new System.Drawing.Size(190, 104);
            this.lblVersion.TabIndex = 106;
            this.lblVersion.Text = "** information **";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.ClientSize = new System.Drawing.Size(1306, 831);
            this.Controls.Add(this.tlpFrame);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimizeBox = false;
            this.Name = "frmSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "LVSIII";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.frmSelect_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSelect_FormClosing);
            this.Load += new System.EventHandler(this.frmSelect_Load);
            this.Click += new System.EventHandler(this.cmdOK_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSelect_KeyDown);
            this.tlpFrame.ResumeLayout(false);
            this.tlpFrame.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbScanner)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.roundPanel1.ResumeLayout(false);
            this.tlpManual.ResumeLayout(false);
            this.tlpDock.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpFrame;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private RoundButton cmdResetAlarm;
        private RoundLabel lblStationID;
        public System.Windows.Forms.TableLayoutPanel tlpHMI;
        public uscMessageDisplay uscMD;
        private RoundLabel roundLabel2;
        private RoundButton cmdCheckLPN;
        private RoundButton cmdOK;
        internal System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pbScanner;
        private RoundButton cmdClear;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        internal System.Windows.Forms.Label lblerrorReel;
        internal System.Windows.Forms.TextBox txtReel;
        private RoundButton cmdCancel;
        private RoundPanel roundPanel1;
        private System.Windows.Forms.TableLayoutPanel tlpManual;
        private System.Windows.Forms.HScrollBar sbForward;
        private RoundButton cmdReverse;
        private RoundButton cmdForward;
        private RoundButton cmdStopManual;
        private System.Windows.Forms.HScrollBar sbReverse;
        private System.Windows.Forms.TableLayoutPanel tlpDock;
        private RoundButton cmdReports;
        private System.Windows.Forms.Label lblVersion;
        private RoundRadioButton optTrain;
        private RoundRadioButton optInspect;
        internal System.Windows.Forms.Label lblLIN;
        private System.Windows.Forms.PictureBox pbLabel;
        private RoundTopLabel lblBCTop;
        private RoundBottomLabel lblBCBottom;
    }
}