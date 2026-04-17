
namespace LVS3
{
    partial class frmUnderInvestigation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUnderInvestigation));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tlpInspectDecision = new System.Windows.Forms.TableLayoutPanel();
            this.cmdCycle = new LVS3.RoundButton();
            this.txtLUIReasons = new System.Windows.Forms.TextBox();
            this.lblLUITop = new System.Windows.Forms.Label();
            this.tlpUserOpinion = new System.Windows.Forms.TableLayoutPanel();
            this.txtBackingNumber = new System.Windows.Forms.TextBox();
            this.clstReason = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOpInformation = new System.Windows.Forms.TextBox();
            this.lblR2 = new System.Windows.Forms.Label();
            this.hWinQuery = new HalconDotNet.HSmartWindowControl();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblCancel = new LVS3.RoundNoneLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.uscPixelData1 = new LVS3.uscPixelData();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tlpDecision = new System.Windows.Forms.TableLayoutPanel();
            this.optMissing = new System.Windows.Forms.RadioButton();
            this.optReject = new System.Windows.Forms.RadioButton();
            this.optAccept = new System.Windows.Forms.RadioButton();
            this.cmdOK = new LVS3.RoundButton();
            this.lbl1VDEContrast = new System.Windows.Forms.Label();
            this.tlpBC = new System.Windows.Forms.TableLayoutPanel();
            this.lbl1Bright = new System.Windows.Forms.Label();
            this.lbl1MinLabel = new System.Windows.Forms.Label();
            this.lbl1MinVar = new System.Windows.Forms.Label();
            this.lbl1Type = new System.Windows.Forms.Label();
            this.lstVDE = new System.Windows.Forms.ListView();
            this.Details = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel2.SuspendLayout();
            this.tlpInspectDecision.SuspendLayout();
            this.tlpUserOpinion.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tlpDecision.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tlpMain.SetColumnSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.13113F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 39.91805F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.95083F));
            this.tableLayoutPanel2.Controls.Add(this.tlpInspectDecision, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tlpUserOpinion, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 287);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1475, 207);
            this.tableLayoutPanel2.TabIndex = 321;
            // 
            // tlpInspectDecision
            // 
            this.tlpInspectDecision.BackColor = System.Drawing.Color.Transparent;
            this.tlpInspectDecision.ColumnCount = 1;
            this.tlpInspectDecision.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInspectDecision.Controls.Add(this.cmdCycle, 0, 1);
            this.tlpInspectDecision.Controls.Add(this.txtLUIReasons, 0, 2);
            this.tlpInspectDecision.Controls.Add(this.lblLUITop, 0, 0);
            this.tlpInspectDecision.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInspectDecision.Location = new System.Drawing.Point(0, 0);
            this.tlpInspectDecision.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.tlpInspectDecision.Name = "tlpInspectDecision";
            this.tlpInspectDecision.RowCount = 3;
            this.tlpInspectDecision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpInspectDecision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tlpInspectDecision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInspectDecision.Size = new System.Drawing.Size(531, 207);
            this.tlpInspectDecision.TabIndex = 101;
            // 
            // cmdCycle
            // 
            this.cmdCycle.BackColor = System.Drawing.Color.Teal;
            this.cmdCycle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdCycle.Enabled = false;
            this.cmdCycle.FlatAppearance.BorderSize = 0;
            this.cmdCycle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdCycle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCycle.Font = new System.Drawing.Font("Calibri Light", 11F);
            this.cmdCycle.Location = new System.Drawing.Point(2, 28);
            this.cmdCycle.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            this.cmdCycle.Name = "cmdCycle";
            this.cmdCycle.Size = new System.Drawing.Size(525, 32);
            this.cmdCycle.TabIndex = 18;
            this.cmdCycle.Text = "Multiple Reasons";
            this.cmdCycle.UseVisualStyleBackColor = false;
            this.cmdCycle.Click += new System.EventHandler(this.cmdCycle_Click);
            // 
            // txtLUIReasons
            // 
            this.txtLUIReasons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLUIReasons.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLUIReasons.Location = new System.Drawing.Point(0, 60);
            this.txtLUIReasons.Margin = new System.Windows.Forms.Padding(0);
            this.txtLUIReasons.Multiline = true;
            this.txtLUIReasons.Name = "txtLUIReasons";
            this.txtLUIReasons.ReadOnly = true;
            this.txtLUIReasons.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLUIReasons.Size = new System.Drawing.Size(531, 147);
            this.txtLUIReasons.TabIndex = 17;
            // 
            // lblLUITop
            // 
            this.lblLUITop.AutoSize = true;
            this.lblLUITop.BackColor = System.Drawing.Color.LightGray;
            this.lblLUITop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLUITop.Font = new System.Drawing.Font("Calibri Light", 11F);
            this.lblLUITop.Location = new System.Drawing.Point(0, 1);
            this.lblLUITop.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblLUITop.Name = "lblLUITop";
            this.lblLUITop.Size = new System.Drawing.Size(531, 27);
            this.lblLUITop.TabIndex = 3;
            this.lblLUITop.Text = "Reasons For Investigation";
            this.lblLUITop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpUserOpinion
            // 
            this.tlpUserOpinion.ColumnCount = 2;
            this.tableLayoutPanel2.SetColumnSpan(this.tlpUserOpinion, 2);
            this.tlpUserOpinion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.10735F));
            this.tlpUserOpinion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.89265F));
            this.tlpUserOpinion.Controls.Add(this.txtBackingNumber, 1, 3);
            this.tlpUserOpinion.Controls.Add(this.clstReason, 0, 1);
            this.tlpUserOpinion.Controls.Add(this.label1, 1, 0);
            this.tlpUserOpinion.Controls.Add(this.label2, 0, 0);
            this.tlpUserOpinion.Controls.Add(this.txtOpInformation, 1, 1);
            this.tlpUserOpinion.Controls.Add(this.lblR2, 1, 2);
            this.tlpUserOpinion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpUserOpinion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tlpUserOpinion.Location = new System.Drawing.Point(532, 0);
            this.tlpUserOpinion.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.tlpUserOpinion.Name = "tlpUserOpinion";
            this.tlpUserOpinion.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.tlpUserOpinion.RowCount = 4;
            this.tlpUserOpinion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpUserOpinion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpUserOpinion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpUserOpinion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpUserOpinion.Size = new System.Drawing.Size(935, 207);
            this.tlpUserOpinion.TabIndex = 102;
            // 
            // txtBackingNumber
            // 
            this.txtBackingNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBackingNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBackingNumber.Font = new System.Drawing.Font("Calibri", 19F);
            this.txtBackingNumber.Location = new System.Drawing.Point(497, 167);
            this.txtBackingNumber.Margin = new System.Windows.Forms.Padding(1, 2, 1, 0);
            this.txtBackingNumber.Name = "txtBackingNumber";
            this.txtBackingNumber.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBackingNumber.Size = new System.Drawing.Size(437, 38);
            this.txtBackingNumber.TabIndex = 350;
            this.txtBackingNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // clstReason
            // 
            this.clstReason.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clstReason.CheckOnClick = true;
            this.clstReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clstReason.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
            this.clstReason.FormattingEnabled = true;
            this.clstReason.Items.AddRange(new object[] {
            "Patchy Print",
            "Mark On Label",
            "Ribbon Wrinkle",
            "Barcode Scanned Manually",
            "Text Movement",
            "Other"});
            this.clstReason.Location = new System.Drawing.Point(1, 29);
            this.clstReason.Margin = new System.Windows.Forms.Padding(1, 1, 1, 0);
            this.clstReason.Name = "clstReason";
            this.tlpUserOpinion.SetRowSpan(this.clstReason, 3);
            this.clstReason.Size = new System.Drawing.Size(494, 176);
            this.clstReason.TabIndex = 107;
            this.clstReason.Click += new System.EventHandler(this.Options_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.LightGray;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Calibri Light", 11F);
            this.label1.Location = new System.Drawing.Point(496, 1);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(439, 27);
            this.label1.TabIndex = 349;
            this.label1.Text = "Operator Information";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.LightGray;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Calibri Light", 11F);
            this.label2.Location = new System.Drawing.Point(0, 1);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(496, 27);
            this.label2.TabIndex = 110;
            this.label2.Text = "Investigation Category";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtOpInformation
            // 
            this.txtOpInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOpInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOpInformation.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOpInformation.Location = new System.Drawing.Point(497, 29);
            this.txtOpInformation.Margin = new System.Windows.Forms.Padding(1);
            this.txtOpInformation.Multiline = true;
            this.txtOpInformation.Name = "txtOpInformation";
            this.txtOpInformation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOpInformation.Size = new System.Drawing.Size(437, 114);
            this.txtOpInformation.TabIndex = 352;
            // 
            // lblR2
            // 
            this.lblR2.BackColor = System.Drawing.Color.LightGray;
            this.lblR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblR2.Font = new System.Drawing.Font("Calibri Light", 11F);
            this.lblR2.Location = new System.Drawing.Point(498, 144);
            this.lblR2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblR2.Name = "lblR2";
            this.lblR2.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblR2.Size = new System.Drawing.Size(435, 21);
            this.lblR2.TabIndex = 353;
            this.lblR2.Text = "Backing Number:";
            this.lblR2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hWinQuery
            // 
            this.hWinQuery.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hWinQuery.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.hWinQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWinQuery.HDoubleClickToFitContent = true;
            this.hWinQuery.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            this.hWinQuery.HImagePart = new System.Drawing.Rectangle(0, 0, -1, -1);
            this.hWinQuery.HKeepAspectRatio = true;
            this.hWinQuery.HMoveContent = true;
            this.hWinQuery.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            this.hWinQuery.Location = new System.Drawing.Point(200, 29);
            this.hWinQuery.Margin = new System.Windows.Forms.Padding(0, 8, 8, 0);
            this.hWinQuery.Name = "hWinQuery";
            this.hWinQuery.Size = new System.Drawing.Size(1267, 233);
            this.hWinQuery.TabIndex = 348;
            this.hWinQuery.WindowSize = new System.Drawing.Size(1267, 233);
            this.hWinQuery.HMouseMove += new HalconDotNet.HMouseEventHandler(this.hWinQuery_HMouseMove);
            this.hWinQuery.Load += new System.EventHandler(this.hWin_Load);
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.SystemColors.Control;
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.hWinQuery, 1, 1);
            this.tlpMain.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this.tlpMain.Controls.Add(this.lblCancel, 1, 0);
            this.tlpMain.Controls.Add(this.tableLayoutPanel1, 1, 2);
            this.tlpMain.Controls.Add(this.tableLayoutPanel3, 0, 4);
            this.tlpMain.Controls.Add(this.lstVDE, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 5;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 207F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32F));
            this.tlpMain.Size = new System.Drawing.Size(1475, 608);
            this.tlpMain.TabIndex = 0;
            // 
            // lblCancel
            // 
            this.lblCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblCancel.Image = ((System.Drawing.Image)(resources.GetObject("lblCancel.Image")));
            this.lblCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCancel.IsLink = false;
            this.lblCancel.Location = new System.Drawing.Point(1448, 1);
            this.lblCancel.Margin = new System.Windows.Forms.Padding(1);
            this.lblCancel.Name = "lblCancel";
            this.lblCancel.Size = new System.Drawing.Size(26, 19);
            this.lblCancel.TabIndex = 349;
            this.lblCancel.Click += new System.EventHandler(this.lblCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.uscPixelData1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(200, 262);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1267, 25);
            this.tableLayoutPanel1.TabIndex = 350;
            // 
            // uscPixelData1
            // 
            this.uscPixelData1.BackColor = System.Drawing.Color.Black;
            this.uscPixelData1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uscPixelData1.Location = new System.Drawing.Point(513, 0);
            this.uscPixelData1.Margin = new System.Windows.Forms.Padding(0);
            this.uscPixelData1.Name = "uscPixelData1";
            this.uscPixelData1.Size = new System.Drawing.Size(240, 25);
            this.uscPixelData1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tlpMain.SetColumnSpan(this.tableLayoutPanel3, 2);
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 259F));
            this.tableLayoutPanel3.Controls.Add(this.tlpDecision, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.lbl1VDEContrast, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.tlpBC, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.lbl1Bright, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lbl1MinLabel, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lbl1MinVar, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.lbl1Type, 0, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 497);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1469, 108);
            this.tableLayoutPanel3.TabIndex = 371;
            // 
            // tlpDecision
            // 
            this.tlpDecision.ColumnCount = 1;
            this.tlpDecision.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDecision.Controls.Add(this.optMissing, 0, 2);
            this.tlpDecision.Controls.Add(this.optReject, 0, 1);
            this.tlpDecision.Controls.Add(this.optAccept, 0, 0);
            this.tlpDecision.Controls.Add(this.cmdOK, 0, 3);
            this.tlpDecision.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDecision.Location = new System.Drawing.Point(1209, 0);
            this.tlpDecision.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.tlpDecision.Name = "tlpDecision";
            this.tlpDecision.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.tlpDecision.RowCount = 4;
            this.tableLayoutPanel3.SetRowSpan(this.tlpDecision, 5);
            this.tlpDecision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDecision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDecision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDecision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDecision.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpDecision.Size = new System.Drawing.Size(254, 108);
            this.tlpDecision.TabIndex = 371;
            // 
            // optMissing
            // 
            this.optMissing.Appearance = System.Windows.Forms.Appearance.Button;
            this.optMissing.AutoSize = true;
            this.optMissing.BackColor = System.Drawing.Color.DodgerBlue;
            this.optMissing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optMissing.FlatAppearance.BorderSize = 0;
            this.optMissing.FlatAppearance.CheckedBackColor = System.Drawing.Color.OrangeRed;
            this.optMissing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optMissing.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.optMissing.Location = new System.Drawing.Point(1, 53);
            this.optMissing.Margin = new System.Windows.Forms.Padding(1);
            this.optMissing.Name = "optMissing";
            this.optMissing.Size = new System.Drawing.Size(252, 24);
            this.optMissing.TabIndex = 90;
            this.optMissing.Text = "MISSING";
            this.optMissing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optMissing.UseVisualStyleBackColor = false;
            this.optMissing.Click += new System.EventHandler(this.Options_Click);
            // 
            // optReject
            // 
            this.optReject.Appearance = System.Windows.Forms.Appearance.Button;
            this.optReject.AutoSize = true;
            this.optReject.BackColor = System.Drawing.Color.Teal;
            this.optReject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optReject.FlatAppearance.BorderSize = 0;
            this.optReject.FlatAppearance.CheckedBackColor = System.Drawing.Color.OrangeRed;
            this.optReject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optReject.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.optReject.Location = new System.Drawing.Point(1, 27);
            this.optReject.Margin = new System.Windows.Forms.Padding(1);
            this.optReject.Name = "optReject";
            this.optReject.Size = new System.Drawing.Size(252, 24);
            this.optReject.TabIndex = 86;
            this.optReject.Text = "REJECT";
            this.optReject.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optReject.UseVisualStyleBackColor = false;
            this.optReject.Click += new System.EventHandler(this.Options_Click);
            // 
            // optAccept
            // 
            this.optAccept.Appearance = System.Windows.Forms.Appearance.Button;
            this.optAccept.AutoSize = true;
            this.optAccept.BackColor = System.Drawing.Color.Teal;
            this.optAccept.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optAccept.FlatAppearance.BorderSize = 0;
            this.optAccept.FlatAppearance.CheckedBackColor = System.Drawing.Color.LimeGreen;
            this.optAccept.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optAccept.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.optAccept.Location = new System.Drawing.Point(1, 1);
            this.optAccept.Margin = new System.Windows.Forms.Padding(1);
            this.optAccept.Name = "optAccept";
            this.optAccept.Size = new System.Drawing.Size(252, 24);
            this.optAccept.TabIndex = 85;
            this.optAccept.Text = "ACCEPT";
            this.optAccept.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optAccept.UseVisualStyleBackColor = false;
            this.optAccept.Click += new System.EventHandler(this.Options_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.BackColor = System.Drawing.Color.Red;
            this.cmdOK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdOK.FlatAppearance.BorderSize = 0;
            this.cmdOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdOK.Font = new System.Drawing.Font("Calibri", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.Location = new System.Drawing.Point(2, 80);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(250, 26);
            this.cmdOK.TabIndex = 87;
            this.cmdOK.Text = "Continue";
            this.cmdOK.UseVisualStyleBackColor = false;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // lbl1VDEContrast
            // 
            this.lbl1VDEContrast.BackColor = System.Drawing.Color.White;
            this.lbl1VDEContrast.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl1VDEContrast.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl1VDEContrast.Location = new System.Drawing.Point(2, 84);
            this.lbl1VDEContrast.Margin = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.lbl1VDEContrast.Name = "lbl1VDEContrast";
            this.lbl1VDEContrast.Size = new System.Drawing.Size(287, 24);
            this.lbl1VDEContrast.TabIndex = 369;
            this.lbl1VDEContrast.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpBC
            // 
            this.tlpBC.ColumnCount = 1;
            this.tlpBC.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpBC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBC.Location = new System.Drawing.Point(290, 0);
            this.tlpBC.Margin = new System.Windows.Forms.Padding(0);
            this.tlpBC.Name = "tlpBC";
            this.tlpBC.RowCount = 1;
            this.tableLayoutPanel3.SetRowSpan(this.tlpBC, 5);
            this.tlpBC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpBC.Size = new System.Drawing.Size(919, 108);
            this.tlpBC.TabIndex = 353;
            // 
            // lbl1Bright
            // 
            this.lbl1Bright.BackColor = System.Drawing.Color.White;
            this.lbl1Bright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl1Bright.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl1Bright.Location = new System.Drawing.Point(2, 0);
            this.lbl1Bright.Margin = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.lbl1Bright.Name = "lbl1Bright";
            this.lbl1Bright.Size = new System.Drawing.Size(287, 21);
            this.lbl1Bright.TabIndex = 363;
            this.lbl1Bright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl1MinLabel
            // 
            this.lbl1MinLabel.BackColor = System.Drawing.Color.White;
            this.lbl1MinLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl1MinLabel.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl1MinLabel.Location = new System.Drawing.Point(2, 21);
            this.lbl1MinLabel.Margin = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.lbl1MinLabel.Name = "lbl1MinLabel";
            this.lbl1MinLabel.Size = new System.Drawing.Size(287, 21);
            this.lbl1MinLabel.TabIndex = 366;
            this.lbl1MinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl1MinVar
            // 
            this.lbl1MinVar.BackColor = System.Drawing.Color.White;
            this.lbl1MinVar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl1MinVar.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl1MinVar.Location = new System.Drawing.Point(2, 42);
            this.lbl1MinVar.Margin = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.lbl1MinVar.Name = "lbl1MinVar";
            this.lbl1MinVar.Size = new System.Drawing.Size(287, 21);
            this.lbl1MinVar.TabIndex = 367;
            this.lbl1MinVar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl1Type
            // 
            this.lbl1Type.BackColor = System.Drawing.Color.White;
            this.lbl1Type.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl1Type.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl1Type.Location = new System.Drawing.Point(2, 63);
            this.lbl1Type.Margin = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.lbl1Type.Name = "lbl1Type";
            this.lbl1Type.Size = new System.Drawing.Size(287, 21);
            this.lbl1Type.TabIndex = 368;
            this.lbl1Type.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lstVDE
            // 
            this.lstVDE.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Details});
            this.lstVDE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstVDE.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstVDE.GridLines = true;
            this.lstVDE.HideSelection = false;
            this.lstVDE.Location = new System.Drawing.Point(3, 24);
            this.lstVDE.Name = "lstVDE";
            this.lstVDE.Size = new System.Drawing.Size(194, 235);
            this.lstVDE.TabIndex = 372;
            this.lstVDE.UseCompatibleStateImageBehavior = false;
            this.lstVDE.View = System.Windows.Forms.View.Details;
            // 
            // Details
            // 
            this.Details.Text = "Details";
            this.Details.Width = 190;
            // 
            // frmUnderInvestigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1475, 608);
            this.ControlBox = false;
            this.Controls.Add(this.tlpMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmUnderInvestigation";
            this.Text = "Label Under Investigation";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmUnderInvestigation_FormClosing);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tlpInspectDecision.ResumeLayout(false);
            this.tlpInspectDecision.PerformLayout();
            this.tlpUserOpinion.ResumeLayout(false);
            this.tlpUserOpinion.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tlpDecision.ResumeLayout(false);
            this.tlpDecision.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private HalconDotNet.HSmartWindowControl hWinQuery;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tlpInspectDecision;
        private System.Windows.Forms.Label lblLUITop;
        private System.Windows.Forms.TableLayoutPanel tlpUserOpinion;
        private RoundNoneLabel lblCancel;
        private System.Windows.Forms.TextBox txtLUIReasons;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private uscPixelData uscPixelData1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOpInformation;
        private System.Windows.Forms.TextBox txtBackingNumber;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label lblR2;
        private RoundButton cmdCycle;
        public System.Windows.Forms.CheckedListBox clstReason;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lbl1VDEContrast;
        private System.Windows.Forms.TableLayoutPanel tlpBC;
        private System.Windows.Forms.Label lbl1Bright;
        private System.Windows.Forms.Label lbl1MinLabel;
        private System.Windows.Forms.Label lbl1MinVar;
        private System.Windows.Forms.Label lbl1Type;
        private System.Windows.Forms.TableLayoutPanel tlpDecision;
        private System.Windows.Forms.RadioButton optReject;
        private System.Windows.Forms.RadioButton optAccept;
        private RoundButton cmdOK;
        private System.Windows.Forms.ListView lstVDE;
        private System.Windows.Forms.ColumnHeader Details;
        private System.Windows.Forms.RadioButton optMissing;
    }
}