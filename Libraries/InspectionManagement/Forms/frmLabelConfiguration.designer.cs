
namespace LVS3
{
    partial class frmLabelConfiguration
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLabelConfiguration));
            this.ttTip = new System.Windows.Forms.ToolTip(this.components);
            this.cmdForward = new LVS3.RoundRightButton();
            this.cmdBack = new LVS3.RoundLeftButton();
            this.label12 = new LVS3.RoundRightTopLabel();
            this.tlpBorder = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnufile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDarkLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLightMarks = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cboVariationDebrisSize = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.cboDebrisSize = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.cboLabelType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRunTestRT = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuVDE = new System.Windows.Forms.ToolStripMenuItem();
            this.manualDefToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteInOpZoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.fplvdeitems = new System.Windows.Forms.FlowLayoutPanel();
            this.tlpAreaTools = new System.Windows.Forms.TableLayoutPanel();
            this.tlpZones = new System.Windows.Forms.TableLayoutPanel();
            this.cmdFullScreen = new System.Windows.Forms.Button();
            this.cmdGenerate = new System.Windows.Forms.Button();
            this.cmdAddMask = new System.Windows.Forms.Button();
            this.cmdAddZone = new System.Windows.Forms.Button();
            this.lblOP1 = new LVS3.RoundTopLabel();
            this.lblImageCount = new System.Windows.Forms.Label();
            this.tlpCoords = new System.Windows.Forms.TableLayoutPanel();
            this.rbl2 = new System.Windows.Forms.Label();
            this.hWinOCR = new HalconDotNet.HSmartWindowControl();
            this.lblMessage = new LVS3.RoundTopLabel();
            this.rbl1 = new System.Windows.Forms.Label();
            this.uscPixelData1 = new LVS3.uscPixelData();
            this.tlpLabelInfo = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblReel = new System.Windows.Forms.Label();
            this.lblLWO = new System.Windows.Forms.Label();
            this.lblLIN = new System.Windows.Forms.Label();
            this.tlpSummary = new System.Windows.Forms.TableLayoutPanel();
            this.lblOP = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblMasking = new System.Windows.Forms.TextBox();
            this.lblBarcode = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblVDE = new System.Windows.Forms.TextBox();
            this.tlpBorder.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.tlpAreaTools.SuspendLayout();
            this.tlpZones.SuspendLayout();
            this.tlpCoords.SuspendLayout();
            this.tlpLabelInfo.SuspendLayout();
            this.tlpSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // ttTip
            // 
            this.ttTip.AutoPopDelay = 5000;
            this.ttTip.InitialDelay = 50;
            this.ttTip.ReshowDelay = 50;
            this.ttTip.StripAmpersands = true;
            this.ttTip.ToolTipTitle = "Settings";
            // 
            // cmdForward
            // 
            this.cmdForward.BackColor = System.Drawing.Color.LightGray;
            this.cmdForward.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdForward.Enabled = false;
            this.cmdForward.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.cmdForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdForward.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdForward.Location = new System.Drawing.Point(100, 228);
            this.cmdForward.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.cmdForward.Name = "cmdForward";
            this.cmdForward.Size = new System.Drawing.Size(100, 41);
            this.cmdForward.TabIndex = 16;
            this.cmdForward.Text = ">>";
            this.ttTip.SetToolTip(this.cmdForward, "Move to next label");
            this.cmdForward.UseVisualStyleBackColor = false;
            this.cmdForward.EnabledChanged += new System.EventHandler(this.mask_buttons_EnabledChanged);
            this.cmdForward.Click += new System.EventHandler(this.scroll_images_clicked);
            // 
            // cmdBack
            // 
            this.cmdBack.BackColor = System.Drawing.Color.LightGray;
            this.cmdBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdBack.Enabled = false;
            this.cmdBack.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.cmdBack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdBack.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdBack.Location = new System.Drawing.Point(0, 228);
            this.cmdBack.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.cmdBack.Name = "cmdBack";
            this.cmdBack.Size = new System.Drawing.Size(100, 41);
            this.cmdBack.TabIndex = 15;
            this.cmdBack.Text = "<<";
            this.ttTip.SetToolTip(this.cmdBack, "Move to previous label");
            this.cmdBack.UseVisualStyleBackColor = false;
            this.cmdBack.EnabledChanged += new System.EventHandler(this.mask_buttons_EnabledChanged);
            this.cmdBack.Click += new System.EventHandler(this.scroll_images_clicked);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.LightSlateGray;
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Image = ((System.Drawing.Image)(resources.GetObject("label12.Image")));
            this.label12.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label12.IsLink = true;
            this.label12.Location = new System.Drawing.Point(0, 0);
            this.label12.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(202, 36);
            this.label12.TabIndex = 10;
            this.label12.Text = "Training Data";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ttTip.SetToolTip(this.label12, "Click to copy all traning data");
            this.label12.Click += new System.EventHandler(this.roundTopLabel2_Click);
            // 
            // tlpBorder
            // 
            this.tlpBorder.BackColor = System.Drawing.Color.LightSlateGray;
            this.tlpBorder.ColumnCount = 3;
            this.tlpBorder.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tlpBorder.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBorder.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tlpBorder.Controls.Add(this.menuStrip1, 0, 0);
            this.tlpBorder.Controls.Add(this.tlpMain, 1, 1);
            this.tlpBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBorder.Location = new System.Drawing.Point(0, 0);
            this.tlpBorder.Margin = new System.Windows.Forms.Padding(0);
            this.tlpBorder.Name = "tlpBorder";
            this.tlpBorder.RowCount = 3;
            this.tlpBorder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tlpBorder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBorder.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tlpBorder.Size = new System.Drawing.Size(1157, 588);
            this.tlpBorder.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.tlpBorder.SetColumnSpan(this.menuStrip1, 3);
            this.menuStrip1.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnufile,
            this.mnuOptions,
            this.mnuVDE});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.menuStrip1.Size = new System.Drawing.Size(1157, 31);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnufile
            // 
            this.mnufile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRestart,
            this.toolStripSeparator3,
            this.mnuCancel,
            this.toolStripSeparator1,
            this.mnuClose});
            this.mnufile.Name = "mnufile";
            this.mnufile.Size = new System.Drawing.Size(54, 31);
            this.mnufile.Text = "File";
            // 
            // mnuRestart
            // 
            this.mnuRestart.Enabled = false;
            this.mnuRestart.Name = "mnuRestart";
            this.mnuRestart.Size = new System.Drawing.Size(252, 30);
            this.mnuRestart.Text = "Restart Setup";
            this.mnuRestart.Click += new System.EventHandler(this.mnuRestart_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(249, 6);
            // 
            // mnuCancel
            // 
            this.mnuCancel.Name = "mnuCancel";
            this.mnuCancel.Size = new System.Drawing.Size(252, 30);
            this.mnuCancel.Text = "Cancel Setup";
            this.mnuCancel.Click += new System.EventHandler(this.mnuCancel_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(249, 6);
            // 
            // mnuClose
            // 
            this.mnuClose.Enabled = false;
            this.mnuClose.Name = "mnuClose";
            this.mnuClose.Size = new System.Drawing.Size(252, 30);
            this.mnuClose.Text = "Save And Exit Setup";
            this.mnuClose.Click += new System.EventHandler(this.mnuClose_Click);
            // 
            // mnuOptions
            // 
            this.mnuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDarkLabel,
            this.mnuLightMarks,
            this.toolStripMenuItem2,
            this.cboVariationDebrisSize,
            this.toolStripMenuItem5,
            this.cboDebrisSize,
            this.toolStripSeparator5,
            this.toolStripMenuItem3,
            this.cboLabelType,
            this.toolStripSeparator4,
            this.mnuRunTestRT});
            this.mnuOptions.Font = new System.Drawing.Font("Calibri", 16F);
            this.mnuOptions.Name = "mnuOptions";
            this.mnuOptions.ShowShortcutKeys = false;
            this.mnuOptions.Size = new System.Drawing.Size(96, 31);
            this.mnuOptions.Text = "Options";
            this.mnuOptions.Click += new System.EventHandler(this.mnuFile_Click);
            // 
            // mnuDarkLabel
            // 
            this.mnuDarkLabel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.mnuDarkLabel.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mnuDarkLabel.Name = "mnuDarkLabel";
            this.mnuDarkLabel.Size = new System.Drawing.Size(363, 32);
            this.mnuDarkLabel.Text = "Dark Label Background: Off";
            this.mnuDarkLabel.ToolTipText = "Labels that have a darker background can have light areas piercing through. Selec" +
    "t Noisy Background: On to help filter these areas";
            this.mnuDarkLabel.Visible = false;
            this.mnuDarkLabel.Click += new System.EventHandler(this.mnuDarkLabel_Click);
            // 
            // mnuLightMarks
            // 
            this.mnuLightMarks.BackColor = System.Drawing.Color.WhiteSmoke;
            this.mnuLightMarks.Font = new System.Drawing.Font("Arial", 13.25F);
            this.mnuLightMarks.Name = "mnuLightMarks";
            this.mnuLightMarks.Size = new System.Drawing.Size(363, 32);
            this.mnuLightMarks.Text = "Light Marks/Blemish Inspection: Off";
            this.mnuLightMarks.ToolTipText = resources.GetString("mnuLightMarks.ToolTipText");
            this.mnuLightMarks.Click += new System.EventHandler(this.mnuLightMarks_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Enabled = false;
            this.toolStripMenuItem2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(363, 32);
            this.toolStripMenuItem2.Text = "Op-Zone Debris Size";
            // 
            // cboVariationDebrisSize
            // 
            this.cboVariationDebrisSize.AutoSize = false;
            this.cboVariationDebrisSize.DropDownHeight = 120;
            this.cboVariationDebrisSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVariationDebrisSize.DropDownWidth = 300;
            this.cboVariationDebrisSize.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cboVariationDebrisSize.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cboVariationDebrisSize.IntegralHeight = false;
            this.cboVariationDebrisSize.Items.AddRange(new object[] {
            "Small",
            "Large"});
            this.cboVariationDebrisSize.Margin = new System.Windows.Forms.Padding(8, 2, 2, 8);
            this.cboVariationDebrisSize.Name = "cboVariationDebrisSize";
            this.cboVariationDebrisSize.Size = new System.Drawing.Size(140, 33);
            this.cboVariationDebrisSize.DropDownClosed += new System.EventHandler(this.cboVariationDebrisSize_DropDownClosed);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Enabled = false;
            this.toolStripMenuItem5.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(363, 32);
            this.toolStripMenuItem5.Text = "Label Debris Size";
            this.toolStripMenuItem5.ToolTipText = "Optimizes the segmentation of characters from the background - Lowest to highest " +
    "";
            // 
            // cboDebrisSize
            // 
            this.cboDebrisSize.AutoSize = false;
            this.cboDebrisSize.DropDownHeight = 120;
            this.cboDebrisSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDebrisSize.DropDownWidth = 300;
            this.cboDebrisSize.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cboDebrisSize.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cboDebrisSize.IntegralHeight = false;
            this.cboDebrisSize.Items.AddRange(new object[] {
            "Smallest (default)",
            "Medium",
            "Large"});
            this.cboDebrisSize.Margin = new System.Windows.Forms.Padding(8, 2, 2, 8);
            this.cboDebrisSize.Name = "cboDebrisSize";
            this.cboDebrisSize.Size = new System.Drawing.Size(180, 33);
            this.cboDebrisSize.DropDownClosed += new System.EventHandler(this.cboDebrisSize_DropDownClosed);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(360, 6);
            this.toolStripSeparator5.Visible = false;
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Enabled = false;
            this.toolStripMenuItem3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(363, 32);
            this.toolStripMenuItem3.Text = "Label Type:";
            this.toolStripMenuItem3.ToolTipText = "Optimizes the segmentation of characters from the background - Lowest to highest " +
    "";
            // 
            // cboLabelType
            // 
            this.cboLabelType.AutoSize = false;
            this.cboLabelType.DropDownHeight = 120;
            this.cboLabelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLabelType.DropDownWidth = 400;
            this.cboLabelType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cboLabelType.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.cboLabelType.IntegralHeight = false;
            this.cboLabelType.Items.AddRange(new object[] {
            "Flat Panel",
            "Dark Label",
            "Booklet"});
            this.cboLabelType.Margin = new System.Windows.Forms.Padding(8, 2, 2, 8);
            this.cboLabelType.Name = "cboLabelType";
            this.cboLabelType.Size = new System.Drawing.Size(180, 33);
            this.cboLabelType.DropDownClosed += new System.EventHandler(this.cboLabelType_DropDownClosed);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(360, 6);
            // 
            // mnuRunTestRT
            // 
            this.mnuRunTestRT.Enabled = false;
            this.mnuRunTestRT.Name = "mnuRunTestRT";
            this.mnuRunTestRT.Size = new System.Drawing.Size(363, 32);
            this.mnuRunTestRT.Text = "Run Setup Test";
            this.mnuRunTestRT.Click += new System.EventHandler(this.mnuRunTestRT_Click);
            // 
            // mnuVDE
            // 
            this.mnuVDE.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualDefToolStripMenuItem,
            this.deleteAllToolStripMenuItem,
            this.deleteInOpZoneToolStripMenuItem});
            this.mnuVDE.Enabled = false;
            this.mnuVDE.Name = "mnuVDE";
            this.mnuVDE.Size = new System.Drawing.Size(59, 31);
            this.mnuVDE.Text = "VDE";
            // 
            // manualDefToolStripMenuItem
            // 
            this.manualDefToolStripMenuItem.Name = "manualDefToolStripMenuItem";
            this.manualDefToolStripMenuItem.Size = new System.Drawing.Size(253, 30);
            this.manualDefToolStripMenuItem.Text = "Manual Definition";
            this.manualDefToolStripMenuItem.ToolTipText = "Draw rectangle region surrounding VDE item to be included for inspection";
            this.manualDefToolStripMenuItem.Click += new System.EventHandler(this.manualDefToolStripMenuItem_Click);
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(253, 30);
            this.deleteAllToolStripMenuItem.Text = "Delete All";
            this.deleteAllToolStripMenuItem.Click += new System.EventHandler(this.deleteAllToolStripMenuItem_Click);
            // 
            // deleteInOpZoneToolStripMenuItem
            // 
            this.deleteInOpZoneToolStripMenuItem.Name = "deleteInOpZoneToolStripMenuItem";
            this.deleteInOpZoneToolStripMenuItem.Size = new System.Drawing.Size(253, 30);
            this.deleteInOpZoneToolStripMenuItem.Text = "Delete In Op-Zone...";
            this.deleteInOpZoneToolStripMenuItem.Click += new System.EventHandler(this.deleteInOpZoneToolStripMenuItem_Click);
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.LightSlateGray;
            this.tlpMain.ColumnCount = 3;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 208F));
            this.tlpMain.Controls.Add(this.fplvdeitems, 0, 2);
            this.tlpMain.Controls.Add(this.tlpAreaTools, 0, 1);
            this.tlpMain.Controls.Add(this.lblImageCount, 2, 0);
            this.tlpMain.Controls.Add(this.tlpCoords, 1, 1);
            this.tlpMain.Controls.Add(this.tlpLabelInfo, 1, 0);
            this.tlpMain.Controls.Add(this.tlpSummary, 2, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(2, 31);
            this.tlpMain.Margin = new System.Windows.Forms.Padding(0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 272F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(1153, 549);
            this.tlpMain.TabIndex = 2;
            // 
            // fplvdeitems
            // 
            this.fplvdeitems.AutoScroll = true;
            this.fplvdeitems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fplvdeitems.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.fplvdeitems.Location = new System.Drawing.Point(0, 307);
            this.fplvdeitems.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.fplvdeitems.Name = "fplvdeitems";
            this.fplvdeitems.Size = new System.Drawing.Size(200, 242);
            this.fplvdeitems.TabIndex = 340;
            // 
            // tlpAreaTools
            // 
            this.tlpAreaTools.ColumnCount = 1;
            this.tlpAreaTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAreaTools.Controls.Add(this.tlpZones, 0, 0);
            this.tlpAreaTools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAreaTools.Location = new System.Drawing.Point(0, 33);
            this.tlpAreaTools.Margin = new System.Windows.Forms.Padding(0);
            this.tlpAreaTools.Name = "tlpAreaTools";
            this.tlpAreaTools.RowCount = 1;
            this.tlpAreaTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAreaTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 272F));
            this.tlpAreaTools.Size = new System.Drawing.Size(200, 272);
            this.tlpAreaTools.TabIndex = 110;
            // 
            // tlpZones
            // 
            this.tlpZones.ColumnCount = 2;
            this.tlpZones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpZones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpZones.Controls.Add(this.cmdForward, 1, 5);
            this.tlpZones.Controls.Add(this.cmdBack, 0, 5);
            this.tlpZones.Controls.Add(this.cmdFullScreen, 0, 1);
            this.tlpZones.Controls.Add(this.cmdGenerate, 0, 4);
            this.tlpZones.Controls.Add(this.cmdAddMask, 0, 3);
            this.tlpZones.Controls.Add(this.cmdAddZone, 0, 2);
            this.tlpZones.Controls.Add(this.lblOP1, 0, 0);
            this.tlpZones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpZones.Location = new System.Drawing.Point(0, 0);
            this.tlpZones.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.tlpZones.Name = "tlpZones";
            this.tlpZones.RowCount = 6;
            this.tlpZones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tlpZones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpZones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpZones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpZones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpZones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpZones.Size = new System.Drawing.Size(200, 270);
            this.tlpZones.TabIndex = 11;
            // 
            // cmdFullScreen
            // 
            this.cmdFullScreen.AutoSize = true;
            this.cmdFullScreen.BackColor = System.Drawing.Color.LightGray;
            this.tlpZones.SetColumnSpan(this.cmdFullScreen, 2);
            this.cmdFullScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdFullScreen.Enabled = false;
            this.cmdFullScreen.FlatAppearance.BorderSize = 0;
            this.cmdFullScreen.FlatAppearance.CheckedBackColor = System.Drawing.Color.Cyan;
            this.cmdFullScreen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.cmdFullScreen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.cmdFullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdFullScreen.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold);
            this.cmdFullScreen.Location = new System.Drawing.Point(0, 31);
            this.cmdFullScreen.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.cmdFullScreen.Name = "cmdFullScreen";
            this.cmdFullScreen.Size = new System.Drawing.Size(200, 48);
            this.cmdFullScreen.TabIndex = 14;
            this.cmdFullScreen.TabStop = false;
            this.cmdFullScreen.Text = "Zoom To Fit";
            this.cmdFullScreen.UseVisualStyleBackColor = false;
            this.cmdFullScreen.EnabledChanged += new System.EventHandler(this.mask_buttons_EnabledChanged);
            this.cmdFullScreen.Click += new System.EventHandler(this.cmdZoomToFit_Click);
            // 
            // cmdGenerate
            // 
            this.cmdGenerate.AutoSize = true;
            this.cmdGenerate.BackColor = System.Drawing.Color.LightGray;
            this.tlpZones.SetColumnSpan(this.cmdGenerate, 2);
            this.cmdGenerate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdGenerate.Enabled = false;
            this.cmdGenerate.FlatAppearance.BorderSize = 0;
            this.cmdGenerate.FlatAppearance.CheckedBackColor = System.Drawing.Color.Cyan;
            this.cmdGenerate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.cmdGenerate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.cmdGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdGenerate.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold);
            this.cmdGenerate.Location = new System.Drawing.Point(0, 178);
            this.cmdGenerate.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.cmdGenerate.Name = "cmdGenerate";
            this.cmdGenerate.Size = new System.Drawing.Size(200, 48);
            this.cmdGenerate.TabIndex = 11;
            this.cmdGenerate.TabStop = false;
            this.cmdGenerate.Text = "Generate Models";
            this.cmdGenerate.UseVisualStyleBackColor = false;
            this.cmdGenerate.EnabledChanged += new System.EventHandler(this.mask_buttons_EnabledChanged);
            this.cmdGenerate.Click += new System.EventHandler(this.cmdGenerate_Click);
            // 
            // cmdAddMask
            // 
            this.cmdAddMask.AutoSize = true;
            this.cmdAddMask.BackColor = System.Drawing.Color.LightGray;
            this.tlpZones.SetColumnSpan(this.cmdAddMask, 2);
            this.cmdAddMask.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdAddMask.Enabled = false;
            this.cmdAddMask.FlatAppearance.BorderSize = 0;
            this.cmdAddMask.FlatAppearance.CheckedBackColor = System.Drawing.Color.Cyan;
            this.cmdAddMask.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.cmdAddMask.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.cmdAddMask.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdAddMask.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold);
            this.cmdAddMask.Location = new System.Drawing.Point(0, 129);
            this.cmdAddMask.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.cmdAddMask.Name = "cmdAddMask";
            this.cmdAddMask.Size = new System.Drawing.Size(200, 48);
            this.cmdAddMask.TabIndex = 10;
            this.cmdAddMask.TabStop = false;
            this.cmdAddMask.Text = "Add Mask";
            this.cmdAddMask.UseVisualStyleBackColor = false;
            this.cmdAddMask.EnabledChanged += new System.EventHandler(this.mask_buttons_EnabledChanged);
            this.cmdAddMask.Click += new System.EventHandler(this.cmdAddMask_Click);
            // 
            // cmdAddZone
            // 
            this.cmdAddZone.AutoSize = true;
            this.cmdAddZone.BackColor = System.Drawing.Color.LightGray;
            this.tlpZones.SetColumnSpan(this.cmdAddZone, 2);
            this.cmdAddZone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdAddZone.Enabled = false;
            this.cmdAddZone.FlatAppearance.BorderSize = 0;
            this.cmdAddZone.FlatAppearance.CheckedBackColor = System.Drawing.Color.Cyan;
            this.cmdAddZone.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.cmdAddZone.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.cmdAddZone.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdAddZone.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold);
            this.cmdAddZone.Location = new System.Drawing.Point(0, 80);
            this.cmdAddZone.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.cmdAddZone.Name = "cmdAddZone";
            this.cmdAddZone.Size = new System.Drawing.Size(200, 48);
            this.cmdAddZone.TabIndex = 0;
            this.cmdAddZone.TabStop = false;
            this.cmdAddZone.Text = "Add Zone";
            this.cmdAddZone.UseVisualStyleBackColor = false;
            this.cmdAddZone.EnabledChanged += new System.EventHandler(this.mask_buttons_EnabledChanged);
            this.cmdAddZone.Click += new System.EventHandler(this.cmdAddZone_Click);
            // 
            // lblOP1
            // 
            this.lblOP1.AutoSize = true;
            this.lblOP1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tlpZones.SetColumnSpan(this.lblOP1, 2);
            this.lblOP1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOP1.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOP1.IsLink = false;
            this.lblOP1.Location = new System.Drawing.Point(0, 0);
            this.lblOP1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.lblOP1.Name = "lblOP1";
            this.lblOP1.Size = new System.Drawing.Size(200, 30);
            this.lblOP1.TabIndex = 6;
            this.lblOP1.Text = "Setup Tools";
            this.lblOP1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblImageCount
            // 
            this.lblImageCount.BackColor = System.Drawing.Color.Transparent;
            this.lblImageCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblImageCount.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImageCount.Location = new System.Drawing.Point(945, 0);
            this.lblImageCount.Margin = new System.Windows.Forms.Padding(0);
            this.lblImageCount.Name = "lblImageCount";
            this.lblImageCount.Size = new System.Drawing.Size(208, 33);
            this.lblImageCount.TabIndex = 109;
            this.lblImageCount.Text = "image count";
            this.lblImageCount.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // tlpCoords
            // 
            this.tlpCoords.BackColor = System.Drawing.Color.LightSlateGray;
            this.tlpCoords.ColumnCount = 3;
            this.tlpCoords.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCoords.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 206F));
            this.tlpCoords.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCoords.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCoords.Controls.Add(this.rbl2, 2, 2);
            this.tlpCoords.Controls.Add(this.hWinOCR, 0, 1);
            this.tlpCoords.Controls.Add(this.lblMessage, 0, 0);
            this.tlpCoords.Controls.Add(this.rbl1, 0, 2);
            this.tlpCoords.Controls.Add(this.uscPixelData1, 1, 2);
            this.tlpCoords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCoords.Location = new System.Drawing.Point(200, 33);
            this.tlpCoords.Margin = new System.Windows.Forms.Padding(0);
            this.tlpCoords.Name = "tlpCoords";
            this.tlpCoords.RowCount = 4;
            this.tlpMain.SetRowSpan(this.tlpCoords, 2);
            this.tlpCoords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpCoords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCoords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tlpCoords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCoords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCoords.Size = new System.Drawing.Size(745, 516);
            this.tlpCoords.TabIndex = 104;
            // 
            // rbl2
            // 
            this.rbl2.AutoSize = true;
            this.rbl2.BackColor = System.Drawing.Color.Black;
            this.rbl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbl2.Location = new System.Drawing.Point(475, 483);
            this.rbl2.Margin = new System.Windows.Forms.Padding(0);
            this.rbl2.Name = "rbl2";
            this.tlpCoords.SetRowSpan(this.rbl2, 2);
            this.rbl2.Size = new System.Drawing.Size(270, 33);
            this.rbl2.TabIndex = 107;
            // 
            // hWinOCR
            // 
            this.hWinOCR.AllowDrop = true;
            this.hWinOCR.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hWinOCR.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.tlpCoords.SetColumnSpan(this.hWinOCR, 3);
            this.hWinOCR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWinOCR.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.hWinOCR.ForeColor = System.Drawing.Color.LimeGreen;
            this.hWinOCR.HDoubleClickToFitContent = false;
            this.hWinOCR.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            this.hWinOCR.HImagePart = new System.Drawing.Rectangle(0, 0, -1, -1);
            this.hWinOCR.HKeepAspectRatio = true;
            this.hWinOCR.HMoveContent = true;
            this.hWinOCR.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            this.hWinOCR.Location = new System.Drawing.Point(0, 36);
            this.hWinOCR.Margin = new System.Windows.Forms.Padding(0);
            this.hWinOCR.Name = "hWinOCR";
            this.hWinOCR.Size = new System.Drawing.Size(745, 447);
            this.hWinOCR.TabIndex = 102;
            this.hWinOCR.WindowSize = new System.Drawing.Size(745, 447);
            this.hWinOCR.HMouseMove += new HalconDotNet.HMouseEventHandler(this.hWinOCR_HMouseMove);
            this.hWinOCR.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWinOCR_HMouseDown);
            this.hWinOCR.HMouseUp += new HalconDotNet.HMouseEventHandler(this.hWinOCR_HMouseUp);
            this.hWinOCR.HMouseDoubleClick += new HalconDotNet.HMouseEventHandler(this.hWinOCR_HMouseDoubleClick);
            this.hWinOCR.Load += new System.EventHandler(this.hwinImage_Load);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.BackColor = System.Drawing.Color.Black;
            this.tlpCoords.SetColumnSpan(this.lblMessage, 3);
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("Calibri", 12F);
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.IsLink = false;
            this.lblMessage.Location = new System.Drawing.Point(0, 2);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(745, 34);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "...";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // rbl1
            // 
            this.rbl1.AutoSize = true;
            this.rbl1.BackColor = System.Drawing.Color.Black;
            this.rbl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbl1.Location = new System.Drawing.Point(0, 483);
            this.rbl1.Margin = new System.Windows.Forms.Padding(0);
            this.rbl1.Name = "rbl1";
            this.tlpCoords.SetRowSpan(this.rbl1, 2);
            this.rbl1.Size = new System.Drawing.Size(269, 33);
            this.rbl1.TabIndex = 106;
            // 
            // uscPixelData1
            // 
            this.uscPixelData1.BackColor = System.Drawing.Color.Black;
            this.uscPixelData1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uscPixelData1.Location = new System.Drawing.Point(269, 483);
            this.uscPixelData1.Margin = new System.Windows.Forms.Padding(0);
            this.uscPixelData1.Name = "uscPixelData1";
            this.tlpCoords.SetRowSpan(this.uscPixelData1, 2);
            this.uscPixelData1.Size = new System.Drawing.Size(206, 33);
            this.uscPixelData1.TabIndex = 108;
            // 
            // tlpLabelInfo
            // 
            this.tlpLabelInfo.BackColor = System.Drawing.Color.LightSlateGray;
            this.tlpLabelInfo.ColumnCount = 6;
            this.tlpLabelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.011137F));
            this.tlpLabelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.93571F));
            this.tlpLabelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.971917F));
            this.tlpLabelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.1F));
            this.tlpLabelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.4F));
            this.tlpLabelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.49619F));
            this.tlpLabelInfo.Controls.Add(this.label3, 2, 0);
            this.tlpLabelInfo.Controls.Add(this.label1, 4, 0);
            this.tlpLabelInfo.Controls.Add(this.label2, 0, 0);
            this.tlpLabelInfo.Controls.Add(this.lblReel, 3, 0);
            this.tlpLabelInfo.Controls.Add(this.lblLWO, 1, 0);
            this.tlpLabelInfo.Controls.Add(this.lblLIN, 5, 0);
            this.tlpLabelInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpLabelInfo.Location = new System.Drawing.Point(200, 0);
            this.tlpLabelInfo.Margin = new System.Windows.Forms.Padding(0);
            this.tlpLabelInfo.Name = "tlpLabelInfo";
            this.tlpLabelInfo.RowCount = 1;
            this.tlpLabelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLabelInfo.Size = new System.Drawing.Size(745, 33);
            this.tlpLabelInfo.TabIndex = 93;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(200, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 33);
            this.label3.TabIndex = 99;
            this.label3.Text = "Reel LPN:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(498, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 33);
            this.label1.TabIndex = 98;
            this.label1.Text = "Label Item:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 33);
            this.label2.TabIndex = 97;
            this.label2.Text = "LWO:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblReel
            // 
            this.lblReel.BackColor = System.Drawing.Color.Transparent;
            this.lblReel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReel.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReel.Location = new System.Drawing.Point(274, 0);
            this.lblReel.Margin = new System.Windows.Forms.Padding(0);
            this.lblReel.Name = "lblReel";
            this.lblReel.Size = new System.Drawing.Size(224, 33);
            this.lblReel.TabIndex = 95;
            this.lblReel.Text = "Reel LPN: R1234-5678-901112";
            this.lblReel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLWO
            // 
            this.lblLWO.BackColor = System.Drawing.Color.Transparent;
            this.lblLWO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLWO.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLWO.Location = new System.Drawing.Point(59, 0);
            this.lblLWO.Margin = new System.Windows.Forms.Padding(0);
            this.lblLWO.Name = "lblLWO";
            this.lblLWO.Size = new System.Drawing.Size(141, 33);
            this.lblLWO.TabIndex = 94;
            this.lblLWO.Text = "A1234-5678-901112";
            this.lblLWO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLIN
            // 
            this.lblLIN.BackColor = System.Drawing.Color.Transparent;
            this.lblLIN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLIN.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLIN.Location = new System.Drawing.Point(575, 0);
            this.lblLIN.Margin = new System.Windows.Forms.Padding(0);
            this.lblLIN.Name = "lblLIN";
            this.lblLIN.Size = new System.Drawing.Size(170, 33);
            this.lblLIN.TabIndex = 91;
            this.lblLIN.Text = "Label Item: L12345678";
            this.lblLIN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpSummary
            // 
            this.tlpSummary.BackColor = System.Drawing.Color.LightSlateGray;
            this.tlpSummary.ColumnCount = 1;
            this.tlpSummary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSummary.Controls.Add(this.lblOP, 0, 4);
            this.tlpSummary.Controls.Add(this.label4, 0, 3);
            this.tlpSummary.Controls.Add(this.lblMasking, 0, 8);
            this.tlpSummary.Controls.Add(this.lblBarcode, 0, 6);
            this.tlpSummary.Controls.Add(this.label9, 0, 7);
            this.tlpSummary.Controls.Add(this.label8, 0, 5);
            this.tlpSummary.Controls.Add(this.label7, 0, 1);
            this.tlpSummary.Controls.Add(this.label12, 0, 0);
            this.tlpSummary.Controls.Add(this.lblVDE, 0, 2);
            this.tlpSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSummary.Location = new System.Drawing.Point(949, 33);
            this.tlpSummary.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.tlpSummary.Name = "tlpSummary";
            this.tlpSummary.RowCount = 9;
            this.tlpMain.SetRowSpan(this.tlpSummary, 2);
            this.tlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.74512F));
            this.tlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.3961F));
            this.tlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28531F));
            this.tlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpSummary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28531F));
            this.tlpSummary.Size = new System.Drawing.Size(204, 516);
            this.tlpSummary.TabIndex = 96;
            // 
            // lblOP
            // 
            this.lblOP.BackColor = System.Drawing.Color.White;
            this.lblOP.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblOP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOP.Location = new System.Drawing.Point(4, 224);
            this.lblOP.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblOP.Multiline = true;
            this.lblOP.Name = "lblOP";
            this.lblOP.ReadOnly = true;
            this.lblOP.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lblOP.Size = new System.Drawing.Size(200, 118);
            this.lblOP.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Gainsboro;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(4, 204);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(200, 20);
            this.label4.TabIndex = 18;
            this.label4.Text = "Op-Zone Data:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMasking
            // 
            this.lblMasking.BackColor = System.Drawing.Color.White;
            this.lblMasking.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblMasking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMasking.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMasking.Location = new System.Drawing.Point(4, 448);
            this.lblMasking.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblMasking.Multiline = true;
            this.lblMasking.Name = "lblMasking";
            this.lblMasking.ReadOnly = true;
            this.lblMasking.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lblMasking.Size = new System.Drawing.Size(200, 68);
            this.lblMasking.TabIndex = 17;
            // 
            // lblBarcode
            // 
            this.lblBarcode.BackColor = System.Drawing.Color.White;
            this.lblBarcode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBarcode.Location = new System.Drawing.Point(4, 362);
            this.lblBarcode.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblBarcode.Multiline = true;
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.ReadOnly = true;
            this.lblBarcode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lblBarcode.Size = new System.Drawing.Size(200, 66);
            this.lblBarcode.TabIndex = 16;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Gainsboro;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(4, 428);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(200, 20);
            this.label9.TabIndex = 9;
            this.label9.Text = "Mask Data:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Gainsboro;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(4, 342);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(200, 20);
            this.label8.TabIndex = 8;
            this.label8.Text = "Barcode Data:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Gainsboro;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(4, 36);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(200, 20);
            this.label7.TabIndex = 7;
            this.label7.Text = "VDE Data:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVDE
            // 
            this.lblVDE.BackColor = System.Drawing.Color.White;
            this.lblVDE.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblVDE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVDE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVDE.Location = new System.Drawing.Point(4, 56);
            this.lblVDE.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblVDE.Multiline = true;
            this.lblVDE.Name = "lblVDE";
            this.lblVDE.ReadOnly = true;
            this.lblVDE.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lblVDE.Size = new System.Drawing.Size(200, 148);
            this.lblVDE.TabIndex = 14;
            // 
            // frmLabelConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1157, 588);
            this.Controls.Add(this.tlpBorder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "frmLabelConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Label Training";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLabelConfiguration_FormClosing);
            this.Load += new System.EventHandler(this.frmLabelConfiguration_Load);
            this.tlpBorder.ResumeLayout(false);
            this.tlpBorder.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tlpAreaTools.ResumeLayout(false);
            this.tlpZones.ResumeLayout(false);
            this.tlpZones.PerformLayout();
            this.tlpCoords.ResumeLayout(false);
            this.tlpCoords.PerformLayout();
            this.tlpLabelInfo.ResumeLayout(false);
            this.tlpSummary.ResumeLayout(false);
            this.tlpSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip ttTip;
        private System.Windows.Forms.TableLayoutPanel tlpBorder;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpCoords;
        private HalconDotNet.HSmartWindowControl hWinOCR;
        private RoundTopLabel lblMessage;
        private System.Windows.Forms.Label rbl1;
        private System.Windows.Forms.TableLayoutPanel tlpLabelInfo;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label lblReel;
        public System.Windows.Forms.Label lblLWO;
        public System.Windows.Forms.Label lblLIN;
        private System.Windows.Forms.TableLayoutPanel tlpSummary;
        private System.Windows.Forms.TextBox lblOP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox lblMasking;
        private System.Windows.Forms.TextBox lblBarcode;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private RoundRightTopLabel label12;
        private System.Windows.Forms.TextBox lblVDE;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuOptions;
        private System.Windows.Forms.ToolStripMenuItem mnufile;
        private System.Windows.Forms.ToolStripMenuItem mnuCancel;
        private System.Windows.Forms.ToolStripMenuItem mnuClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuRestart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mnuRunTestRT;
        private System.Windows.Forms.ToolStripMenuItem mnuDarkLabel;
        private System.Windows.Forms.ToolStripMenuItem mnuLightMarks;
        public System.Windows.Forms.Label lblImageCount;
        private System.Windows.Forms.Label rbl2;
        private uscPixelData uscPixelData1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        public System.Windows.Forms.ToolStripComboBox cboLabelType;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripComboBox cboDebrisSize;
        private System.Windows.Forms.ToolStripComboBox cboVariationDebrisSize;
        private System.Windows.Forms.ToolStripMenuItem mnuVDE;
        private System.Windows.Forms.ToolStripMenuItem manualDefToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteInOpZoneToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tlpAreaTools;
        private System.Windows.Forms.TableLayoutPanel tlpZones;
        private System.Windows.Forms.Button cmdFullScreen;
        private System.Windows.Forms.Button cmdGenerate;
        private System.Windows.Forms.Button cmdAddMask;
        private System.Windows.Forms.Button cmdAddZone;
        private RoundTopLabel lblOP1;
        public System.Windows.Forms.FlowLayoutPanel fplvdeitems;
        private RoundRightButton cmdForward;
        private RoundLeftButton cmdBack;
    }
}

