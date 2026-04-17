using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LVS3
{
    partial class frmInspect
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
            try { base.Dispose(disposing); } catch { }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInspect));
            tlpLayout = new TableLayoutPanel();
            roundTopLabel4 = new RoundTopLabel();
            label2 = new Label();
            tlpControl = new TableLayoutPanel();
            roundTopLabel3 = new RoundTopLabel();
            tableLayoutPanel5 = new TableLayoutPanel();
            sbRWDRadius = new HScrollBar();
            lblRWDRadius = new Label();
            cmdEndInspection = new Button();
            cmdStart = new Button();
            rtlSL = new RoundTopLabel();
            tableLayoutPanel2 = new TableLayoutPanel();
            lblSL = new Label();
            sbSpeedLimit = new HScrollBar();
            flpVDEItem = new FlowLayoutPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            cmdRWDReelLarge = new Button();
            cmdFWDReelLarge = new Button();
            cmdFWDReelSmall = new Button();
            cmdRWDReelSmall = new Button();
            tableLayoutPanel4 = new TableLayoutPanel();
            roundTopLabel6 = new RoundTopLabel();
            roundTopLabel5 = new RoundTopLabel();
            tlpImages = new TableLayoutPanel();
            tlpHwinCurrent = new TableLayoutPanel();
            lblTime = new Label();
            lblInfoText = new Label();
            hWinCurrent = new HalconDotNet.HSmartWindowControl();
            pbPass = new PictureBox();
            lblCurrentImage = new RoundTopLabel();
            roundTopLabel2 = new Label();
            roundTopLabel1 = new Label();
            label4 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            tlpToolsTop = new TableLayoutPanel();
            label11 = new RoundLeftTopLabel();
            lstTools = new ListView();
            columnHeader1 = new ColumnHeader();
            tlpToolsBottom = new TableLayoutPanel();
            lblCount = new Label();
            lblCustomer = new Label();
            lblLIN = new Label();
            lblReelLPN = new Label();
            hWinLayout = new HalconDotNet.HSmartWindowControl();
            uscMD = new uscMessageDisplay();
            uscProgress = new UserControls.uscProgress();
            tlpLayout.SuspendLayout();
            tlpControl.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tlpImages.SuspendLayout();
            tlpHwinCurrent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbPass).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tlpToolsTop.SuspendLayout();
            tlpToolsBottom.SuspendLayout();
            SuspendLayout();
            // 
            // tlpLayout
            // 
            tlpLayout.BackColor = Color.Transparent;
            tlpLayout.ColumnCount = 4;
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 456F));
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 53.67742F));
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46.32258F));
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 309F));
            tlpLayout.Controls.Add(roundTopLabel4, 3, 1);
            tlpLayout.Controls.Add(label2, 2, 0);
            tlpLayout.Controls.Add(tlpControl, 3, 2);
            tlpLayout.Controls.Add(tlpImages, 1, 1);
            tlpLayout.Controls.Add(roundTopLabel2, 0, 0);
            tlpLayout.Controls.Add(roundTopLabel1, 3, 0);
            tlpLayout.Controls.Add(label4, 1, 0);
            tlpLayout.Controls.Add(tableLayoutPanel1, 0, 1);
            tlpLayout.Controls.Add(uscMD, 1, 3);
            tlpLayout.Controls.Add(uscProgress, 0, 4);
            tlpLayout.Dock = DockStyle.Fill;
            tlpLayout.Location = new Point(0, 0);
            tlpLayout.Margin = new Padding(3, 0, 0, 0);
            tlpLayout.Name = "tlpLayout";
            tlpLayout.RowCount = 5;
            tlpLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 0F));
            tlpLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 67.36527F));
            tlpLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 32.63473F));
            tlpLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 49F));
            tlpLayout.Size = new Size(1728, 1026);
            tlpLayout.TabIndex = 320;
            // 
            // roundTopLabel4
            // 
            roundTopLabel4.BackColor = Color.LightSlateGray;
            roundTopLabel4.Dock = DockStyle.Fill;
            roundTopLabel4.Font = new Font("Calibri", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            roundTopLabel4.IsLink = false;
            roundTopLabel4.Location = new Point(1439, 2);
            roundTopLabel4.Margin = new Padding(21, 2, 21, 0);
            roundTopLabel4.Name = "roundTopLabel4";
            roundTopLabel4.Size = new Size(268, 36);
            roundTopLabel4.TabIndex = 352;
            roundTopLabel4.Text = "Control";
            roundTopLabel4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.BackColor = Color.LightSlateGray;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Calibri", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(975, 3);
            label2.Margin = new Padding(3, 3, 3, 2);
            label2.Name = "label2";
            label2.Size = new Size(440, 1);
            label2.TabIndex = 349;
            label2.Text = "Configuration:";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tlpControl
            // 
            tlpControl.BackColor = Color.Transparent;
            tlpControl.ColumnCount = 1;
            tlpControl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpControl.Controls.Add(roundTopLabel3, 0, 7);
            tlpControl.Controls.Add(tableLayoutPanel5, 0, 8);
            tlpControl.Controls.Add(cmdEndInspection, 0, 2);
            tlpControl.Controls.Add(cmdStart, 0, 1);
            tlpControl.Controls.Add(rtlSL, 0, 3);
            tlpControl.Controls.Add(tableLayoutPanel2, 0, 4);
            tlpControl.Controls.Add(flpVDEItem, 0, 9);
            tlpControl.Controls.Add(tableLayoutPanel3, 0, 6);
            tlpControl.Controls.Add(tableLayoutPanel4, 0, 5);
            tlpControl.Dock = DockStyle.Fill;
            tlpControl.Location = new Point(1418, 38);
            tlpControl.Margin = new Padding(0);
            tlpControl.Name = "tlpControl";
            tlpControl.RowCount = 10;
            tlpLayout.SetRowSpan(tlpControl, 2);
            tlpControl.RowStyles.Add(new RowStyle(SizeType.Absolute, 9F));
            tlpControl.RowStyles.Add(new RowStyle(SizeType.Absolute, 172F));
            tlpControl.RowStyles.Add(new RowStyle(SizeType.Absolute, 172F));
            tlpControl.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
            tlpControl.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
            tlpControl.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
            tlpControl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            tlpControl.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
            tlpControl.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
            tlpControl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpControl.Size = new Size(310, 938);
            tlpControl.TabIndex = 343;
            // 
            // roundTopLabel3
            // 
            roundTopLabel3.AutoSize = true;
            roundTopLabel3.BackColor = Color.Blue;
            roundTopLabel3.Dock = DockStyle.Fill;
            roundTopLabel3.Font = new Font("Calibri", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            roundTopLabel3.ForeColor = SystemColors.ButtonHighlight;
            roundTopLabel3.IsLink = false;
            roundTopLabel3.Location = new Point(21, 518);
            roundTopLabel3.Margin = new Padding(21, 2, 21, 0);
            roundTopLabel3.Name = "roundTopLabel3";
            roundTopLabel3.Size = new Size(268, 35);
            roundTopLabel3.TabIndex = 347;
            roundTopLabel3.Text = "  RWD Reel Radius         mm";
            roundTopLabel3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 213F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Controls.Add(sbRWDRadius, 0, 0);
            tableLayoutPanel5.Controls.Add(lblRWDRadius, 1, 0);
            tableLayoutPanel5.Location = new Point(21, 555);
            tableLayoutPanel5.Margin = new Padding(21, 2, 21, 2);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Size = new Size(267, 33);
            tableLayoutPanel5.TabIndex = 346;
            // 
            // sbRWDRadius
            // 
            sbRWDRadius.Dock = DockStyle.Fill;
            sbRWDRadius.LargeChange = 5;
            sbRWDRadius.Location = new Point(0, 5);
            sbRWDRadius.Margin = new Padding(0, 5, 0, 0);
            sbRWDRadius.Maximum = 204;
            sbRWDRadius.MaximumSize = new Size(212, 26);
            sbRWDRadius.Minimum = 42;
            sbRWDRadius.MinimumSize = new Size(173, 26);
            sbRWDRadius.Name = "sbRWDRadius";
            sbRWDRadius.Size = new Size(212, 26);
            sbRWDRadius.TabIndex = 345;
            sbRWDRadius.Value = 42;
            sbRWDRadius.Scroll += sbRWDRadius_Scroll;
            // 
            // lblRWDRadius
            // 
            lblRWDRadius.BackColor = Color.LightGray;
            lblRWDRadius.BorderStyle = BorderStyle.FixedSingle;
            lblRWDRadius.Dock = DockStyle.Fill;
            lblRWDRadius.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRWDRadius.Location = new Point(216, 0);
            lblRWDRadius.Margin = new Padding(3, 0, 0, 0);
            lblRWDRadius.Name = "lblRWDRadius";
            lblRWDRadius.Size = new Size(51, 33);
            lblRWDRadius.TabIndex = 344;
            lblRWDRadius.Text = "42";
            lblRWDRadius.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmdEndInspection
            // 
            cmdEndInspection.BackColor = Color.Teal;
            cmdEndInspection.Dock = DockStyle.Fill;
            cmdEndInspection.Enabled = false;
            cmdEndInspection.FlatAppearance.BorderSize = 0;
            cmdEndInspection.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            cmdEndInspection.FlatStyle = FlatStyle.Flat;
            cmdEndInspection.Font = new Font("Calibri", 18F, FontStyle.Bold);
            cmdEndInspection.ImageAlign = ContentAlignment.TopRight;
            cmdEndInspection.Location = new Point(21, 183);
            cmdEndInspection.Margin = new Padding(21, 2, 21, 6);
            cmdEndInspection.Name = "cmdEndInspection";
            cmdEndInspection.Size = new Size(268, 164);
            cmdEndInspection.TabIndex = 338;
            cmdEndInspection.Text = "Cancel\r\nInspection";
            cmdEndInspection.UseVisualStyleBackColor = false;
            cmdEndInspection.Click += cmdEndLPN_Click;
            // 
            // cmdStart
            // 
            cmdStart.BackColor = Color.Teal;
            cmdStart.Dock = DockStyle.Fill;
            cmdStart.Enabled = false;
            cmdStart.FlatAppearance.BorderSize = 0;
            cmdStart.FlatAppearance.MouseOverBackColor = Color.FromArgb(212, 255, 255);
            cmdStart.FlatStyle = FlatStyle.Flat;
            cmdStart.Font = new Font("Calibri", 18F, FontStyle.Bold);
            cmdStart.Image = (Image)resources.GetObject("cmdStart.Image");
            cmdStart.ImageAlign = ContentAlignment.TopRight;
            cmdStart.Location = new Point(21, 11);
            cmdStart.Margin = new Padding(21, 2, 21, 2);
            cmdStart.Name = "cmdStart";
            cmdStart.Size = new Size(268, 168);
            cmdStart.TabIndex = 337;
            cmdStart.Text = "Start\r\nInspection";
            cmdStart.UseVisualStyleBackColor = false;
            cmdStart.Click += cmdStart_Click;
            // 
            // rtlSL
            // 
            rtlSL.AutoSize = true;
            rtlSL.BackColor = Color.Blue;
            rtlSL.Dock = DockStyle.Fill;
            rtlSL.Font = new Font("Calibri", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtlSL.ForeColor = SystemColors.ButtonHighlight;
            rtlSL.IsLink = false;
            rtlSL.Location = new Point(21, 355);
            rtlSL.Margin = new Padding(21, 2, 21, 0);
            rtlSL.Name = "rtlSL";
            rtlSL.Size = new Size(268, 35);
            rtlSL.TabIndex = 341;
            rtlSL.Text = "  Max Line Speed Limit         %";
            rtlSL.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 53F));
            tableLayoutPanel2.Controls.Add(lblSL, 1, 0);
            tableLayoutPanel2.Controls.Add(sbSpeedLimit, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(21, 395);
            tableLayoutPanel2.Margin = new Padding(21, 5, 21, 5);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(268, 27);
            tableLayoutPanel2.TabIndex = 343;
            // 
            // lblSL
            // 
            lblSL.BackColor = Color.LightGray;
            lblSL.BorderStyle = BorderStyle.FixedSingle;
            lblSL.Dock = DockStyle.Fill;
            lblSL.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSL.Location = new Point(218, 0);
            lblSL.Margin = new Padding(3, 0, 0, 0);
            lblSL.Name = "lblSL";
            lblSL.Size = new Size(50, 27);
            lblSL.TabIndex = 343;
            lblSL.Text = "100";
            lblSL.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // sbSpeedLimit
            // 
            sbSpeedLimit.Dock = DockStyle.Fill;
            sbSpeedLimit.LargeChange = 5;
            sbSpeedLimit.Location = new Point(0, 0);
            sbSpeedLimit.Maximum = 104;
            sbSpeedLimit.MaximumSize = new Size(212, 26);
            sbSpeedLimit.Minimum = 15;
            sbSpeedLimit.MinimumSize = new Size(212, 26);
            sbSpeedLimit.Name = "sbSpeedLimit";
            sbSpeedLimit.Size = new Size(212, 26);
            sbSpeedLimit.SmallChange = 5;
            sbSpeedLimit.TabIndex = 340;
            sbSpeedLimit.Value = 100;
            sbSpeedLimit.Scroll += sbSpeedLimit_Scroll;
            // 
            // flpVDEItem
            // 
            flpVDEItem.AutoScroll = true;
            flpVDEItem.Dock = DockStyle.Fill;
            flpVDEItem.FlowDirection = FlowDirection.TopDown;
            flpVDEItem.Location = new Point(21, 590);
            flpVDEItem.Margin = new Padding(21, 0, 21, 0);
            flpVDEItem.Name = "flpVDEItem";
            flpVDEItem.Size = new Size(268, 348);
            flpVDEItem.TabIndex = 339;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 5;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 13F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.Controls.Add(cmdRWDReelLarge, 4, 0);
            tableLayoutPanel3.Controls.Add(cmdFWDReelLarge, 1, 0);
            tableLayoutPanel3.Controls.Add(cmdFWDReelSmall, 0, 0);
            tableLayoutPanel3.Controls.Add(cmdRWDReelSmall, 3, 0);
            tableLayoutPanel3.Location = new Point(21, 466);
            tableLayoutPanel3.Margin = new Padding(21, 2, 21, 2);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(267, 48);
            tableLayoutPanel3.TabIndex = 344;
            // 
            // cmdRWDReelLarge
            // 
            cmdRWDReelLarge.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cmdRWDReelLarge.ForeColor = SystemColors.ControlText;
            cmdRWDReelLarge.Location = new Point(202, 0);
            cmdRWDReelLarge.Margin = new Padding(0);
            cmdRWDReelLarge.Name = "cmdRWDReelLarge";
            cmdRWDReelLarge.Size = new Size(65, 48);
            cmdRWDReelLarge.TabIndex = 3;
            cmdRWDReelLarge.Text = "L";
            cmdRWDReelLarge.UseVisualStyleBackColor = true;
            cmdRWDReelLarge.Click += cmdRWDReelLarge_Click;
            // 
            // cmdFWDReelLarge
            // 
            cmdFWDReelLarge.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cmdFWDReelLarge.ForeColor = SystemColors.ControlText;
            cmdFWDReelLarge.Location = new Point(63, 0);
            cmdFWDReelLarge.Margin = new Padding(0);
            cmdFWDReelLarge.Name = "cmdFWDReelLarge";
            cmdFWDReelLarge.Size = new Size(63, 48);
            cmdFWDReelLarge.TabIndex = 1;
            cmdFWDReelLarge.Text = "L";
            cmdFWDReelLarge.UseVisualStyleBackColor = true;
            cmdFWDReelLarge.Click += cmdFWDReelLarge_Click;
            // 
            // cmdFWDReelSmall
            // 
            cmdFWDReelSmall.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cmdFWDReelSmall.ForeColor = SystemColors.ControlText;
            cmdFWDReelSmall.Location = new Point(0, 0);
            cmdFWDReelSmall.Margin = new Padding(0);
            cmdFWDReelSmall.Name = "cmdFWDReelSmall";
            cmdFWDReelSmall.Size = new Size(63, 48);
            cmdFWDReelSmall.TabIndex = 0;
            cmdFWDReelSmall.Text = "S";
            cmdFWDReelSmall.UseVisualStyleBackColor = true;
            cmdFWDReelSmall.Click += cmdFWDReelSmall_Click;
            // 
            // cmdRWDReelSmall
            // 
            cmdRWDReelSmall.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cmdRWDReelSmall.ForeColor = SystemColors.ControlText;
            cmdRWDReelSmall.Location = new Point(139, 0);
            cmdRWDReelSmall.Margin = new Padding(0);
            cmdRWDReelSmall.Name = "cmdRWDReelSmall";
            cmdRWDReelSmall.Size = new Size(63, 48);
            cmdRWDReelSmall.TabIndex = 2;
            cmdRWDReelSmall.Text = "S";
            cmdRWDReelSmall.UseVisualStyleBackColor = true;
            cmdRWDReelSmall.Click += cmdRWDReelSmall_Click;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 3;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 13F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(roundTopLabel6, 2, 0);
            tableLayoutPanel4.Controls.Add(roundTopLabel5, 0, 0);
            tableLayoutPanel4.Location = new Point(21, 429);
            tableLayoutPanel4.Margin = new Padding(21, 2, 21, 2);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new Size(267, 33);
            tableLayoutPanel4.TabIndex = 348;
            // 
            // roundTopLabel6
            // 
            roundTopLabel6.AutoSize = true;
            roundTopLabel6.BackColor = Color.MidnightBlue;
            roundTopLabel6.Dock = DockStyle.Fill;
            roundTopLabel6.Font = new Font("Calibri", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            roundTopLabel6.ForeColor = SystemColors.ButtonHighlight;
            roundTopLabel6.IsLink = false;
            roundTopLabel6.Location = new Point(140, 2);
            roundTopLabel6.Margin = new Padding(0, 2, 0, 0);
            roundTopLabel6.Name = "roundTopLabel6";
            roundTopLabel6.Size = new Size(127, 31);
            roundTopLabel6.TabIndex = 343;
            roundTopLabel6.Text = "RWD Core";
            roundTopLabel6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // roundTopLabel5
            // 
            roundTopLabel5.AutoSize = true;
            roundTopLabel5.BackColor = Color.Blue;
            roundTopLabel5.Dock = DockStyle.Fill;
            roundTopLabel5.Font = new Font("Calibri", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            roundTopLabel5.ForeColor = SystemColors.ButtonHighlight;
            roundTopLabel5.IsLink = false;
            roundTopLabel5.Location = new Point(0, 2);
            roundTopLabel5.Margin = new Padding(0, 2, 0, 0);
            roundTopLabel5.Name = "roundTopLabel5";
            roundTopLabel5.Size = new Size(127, 31);
            roundTopLabel5.TabIndex = 342;
            roundTopLabel5.Text = "FWD Core";
            roundTopLabel5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tlpImages
            // 
            tlpImages.BackColor = Color.Transparent;
            tlpImages.ColumnCount = 1;
            tlpLayout.SetColumnSpan(tlpImages, 2);
            tlpImages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpImages.Controls.Add(tlpHwinCurrent, 0, 1);
            tlpImages.Controls.Add(lblCurrentImage, 0, 0);
            tlpImages.Dock = DockStyle.Fill;
            tlpImages.Location = new Point(456, 0);
            tlpImages.Margin = new Padding(0);
            tlpImages.Name = "tlpImages";
            tlpImages.RowCount = 2;
            tlpLayout.SetRowSpan(tlpImages, 2);
            tlpImages.RowStyles.Add(new RowStyle(SizeType.Absolute, 37F));
            tlpImages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpImages.Size = new Size(962, 670);
            tlpImages.TabIndex = 341;
            // 
            // tlpHwinCurrent
            // 
            tlpHwinCurrent.BackColor = Color.Transparent;
            tlpHwinCurrent.ColumnCount = 3;
            tlpHwinCurrent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpHwinCurrent.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 107F));
            tlpHwinCurrent.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64F));
            tlpHwinCurrent.Controls.Add(lblTime, 0, 1);
            tlpHwinCurrent.Controls.Add(lblInfoText, 0, 1);
            tlpHwinCurrent.Controls.Add(hWinCurrent, 0, 0);
            tlpHwinCurrent.Controls.Add(pbPass, 2, 1);
            tlpHwinCurrent.Dock = DockStyle.Fill;
            tlpHwinCurrent.Location = new Point(5, 37);
            tlpHwinCurrent.Margin = new Padding(5, 0, 5, 0);
            tlpHwinCurrent.Name = "tlpHwinCurrent";
            tlpHwinCurrent.RowCount = 2;
            tlpHwinCurrent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpHwinCurrent.RowStyles.Add(new RowStyle(SizeType.Absolute, 49F));
            tlpHwinCurrent.Size = new Size(952, 633);
            tlpHwinCurrent.TabIndex = 350;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.BackColor = Color.Black;
            lblTime.Dock = DockStyle.Fill;
            lblTime.Font = new Font("Calibri", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTime.ForeColor = Color.WhiteSmoke;
            lblTime.Location = new Point(781, 584);
            lblTime.Margin = new Padding(0, 0, 0, 3);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(107, 46);
            lblTime.TabIndex = 352;
            lblTime.Text = "...";
            lblTime.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblInfoText
            // 
            lblInfoText.AutoSize = true;
            lblInfoText.BackColor = Color.Black;
            lblInfoText.Dock = DockStyle.Fill;
            lblInfoText.Font = new Font("Calibri", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblInfoText.ForeColor = Color.White;
            lblInfoText.Location = new Point(0, 584);
            lblInfoText.Margin = new Padding(0, 0, 0, 3);
            lblInfoText.Name = "lblInfoText";
            lblInfoText.Size = new Size(781, 46);
            lblInfoText.TabIndex = 350;
            lblInfoText.Text = "...";
            lblInfoText.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // hWinCurrent
            // 
            hWinCurrent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            hWinCurrent.AutoValidate = AutoValidate.EnableAllowFocusChange;
            tlpHwinCurrent.SetColumnSpan(hWinCurrent, 3);
            hWinCurrent.Dock = DockStyle.Fill;
            hWinCurrent.HDoubleClickToFitContent = true;
            hWinCurrent.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            hWinCurrent.HImagePart = new Rectangle(0, 0, 640, 480);
            hWinCurrent.HKeepAspectRatio = true;
            hWinCurrent.HMoveContent = false;
            hWinCurrent.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.Off;
            hWinCurrent.Location = new Point(0, 0);
            hWinCurrent.Margin = new Padding(0);
            hWinCurrent.Name = "hWinCurrent";
            hWinCurrent.Size = new Size(952, 584);
            hWinCurrent.TabIndex = 347;
            hWinCurrent.WindowSize = new Size(952, 584);
            hWinCurrent.Load += hwinImage_Load;
            // 
            // pbPass
            // 
            pbPass.BackColor = Color.Black;
            pbPass.Dock = DockStyle.Fill;
            pbPass.Location = new Point(888, 584);
            pbPass.Margin = new Padding(0, 0, 0, 3);
            pbPass.Name = "pbPass";
            pbPass.Padding = new Padding(3);
            pbPass.Size = new Size(64, 46);
            pbPass.SizeMode = PictureBoxSizeMode.StretchImage;
            pbPass.TabIndex = 351;
            pbPass.TabStop = false;
            // 
            // lblCurrentImage
            // 
            lblCurrentImage.BackColor = Color.LightSlateGray;
            lblCurrentImage.Dock = DockStyle.Fill;
            lblCurrentImage.Font = new Font("Calibri", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrentImage.IsLink = false;
            lblCurrentImage.Location = new Point(5, 2);
            lblCurrentImage.Margin = new Padding(5, 2, 5, 0);
            lblCurrentImage.Name = "lblCurrentImage";
            lblCurrentImage.Size = new Size(952, 35);
            lblCurrentImage.TabIndex = 342;
            lblCurrentImage.Text = "Currrent Label Image";
            lblCurrentImage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // roundTopLabel2
            // 
            roundTopLabel2.BackColor = Color.LightSlateGray;
            roundTopLabel2.Dock = DockStyle.Fill;
            roundTopLabel2.Font = new Font("Calibri", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            roundTopLabel2.Location = new Point(3, 3);
            roundTopLabel2.Margin = new Padding(3, 3, 3, 2);
            roundTopLabel2.Name = "roundTopLabel2";
            roundTopLabel2.Size = new Size(450, 1);
            roundTopLabel2.TabIndex = 337;
            roundTopLabel2.Text = "Label Item:";
            roundTopLabel2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // roundTopLabel1
            // 
            roundTopLabel1.BackColor = Color.LightSlateGray;
            roundTopLabel1.Dock = DockStyle.Fill;
            roundTopLabel1.Font = new Font("Calibri", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            roundTopLabel1.Location = new Point(1421, 3);
            roundTopLabel1.Margin = new Padding(3, 3, 3, 2);
            roundTopLabel1.Name = "roundTopLabel1";
            roundTopLabel1.Size = new Size(304, 1);
            roundTopLabel1.TabIndex = 335;
            roundTopLabel1.Text = "L W O:";
            roundTopLabel1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.BackColor = Color.LightSlateGray;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Calibri", 13F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(459, 3);
            label4.Margin = new Padding(3, 3, 3, 2);
            label4.Name = "label4";
            label4.Size = new Size(510, 1);
            label4.TabIndex = 322;
            label4.Text = "Reel LPN:";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 456F));
            tableLayoutPanel1.Controls.Add(tlpToolsTop, 0, 0);
            tableLayoutPanel1.Controls.Add(lstTools, 0, 1);
            tableLayoutPanel1.Controls.Add(tlpToolsBottom, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tlpLayout.SetRowSpan(tableLayoutPanel1, 3);
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 200F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 643F));
            tableLayoutPanel1.Size = new Size(456, 976);
            tableLayoutPanel1.TabIndex = 344;
            // 
            // tlpToolsTop
            // 
            tlpToolsTop.ColumnCount = 1;
            tlpToolsTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpToolsTop.Controls.Add(label11, 0, 0);
            tlpToolsTop.Dock = DockStyle.Fill;
            tlpToolsTop.Location = new Point(0, 0);
            tlpToolsTop.Margin = new Padding(0);
            tlpToolsTop.Name = "tlpToolsTop";
            tlpToolsTop.RowCount = 1;
            tlpToolsTop.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpToolsTop.Size = new Size(456, 38);
            tlpToolsTop.TabIndex = 3;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.BackColor = Color.LightSlateGray;
            label11.Dock = DockStyle.Fill;
            label11.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.IsLink = false;
            label11.Location = new Point(0, 2);
            label11.Margin = new Padding(0, 2, 0, 0);
            label11.Name = "label11";
            label11.Size = new Size(456, 36);
            label11.TabIndex = 2;
            label11.Text = "Configuration  Data";
            label11.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lstTools
            // 
            lstTools.BackColor = Color.White;
            lstTools.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
            lstTools.Dock = DockStyle.Fill;
            lstTools.Font = new Font("Arial", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lstTools.GridLines = true;
            lstTools.Location = new Point(0, 38);
            lstTools.Margin = new Padding(0, 0, 1, 0);
            lstTools.Name = "lstTools";
            lstTools.Size = new Size(455, 295);
            lstTools.TabIndex = 357;
            lstTools.UseCompatibleStateImageBehavior = false;
            lstTools.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "";
            columnHeader1.Width = 350;
            // 
            // tlpToolsBottom
            // 
            tlpToolsBottom.ColumnCount = 1;
            tlpToolsBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpToolsBottom.Controls.Add(lblCount, 0, 5);
            tlpToolsBottom.Controls.Add(lblCustomer, 0, 2);
            tlpToolsBottom.Controls.Add(lblLIN, 0, 3);
            tlpToolsBottom.Controls.Add(lblReelLPN, 0, 2);
            tlpToolsBottom.Controls.Add(hWinLayout, 0, 1);
            tlpToolsBottom.Dock = DockStyle.Fill;
            tlpToolsBottom.Location = new Point(0, 333);
            tlpToolsBottom.Margin = new Padding(0);
            tlpToolsBottom.Name = "tlpToolsBottom";
            tlpToolsBottom.RowCount = 6;
            tlpToolsBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpToolsBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 437F));
            tlpToolsBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 77F));
            tlpToolsBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 43F));
            tlpToolsBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 43F));
            tlpToolsBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 43F));
            tlpToolsBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
            tlpToolsBottom.Size = new Size(456, 643);
            tlpToolsBottom.TabIndex = 2;
            // 
            // lblCount
            // 
            lblCount.BackColor = Color.White;
            lblCount.Dock = DockStyle.Fill;
            lblCount.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCount.Location = new Point(3, 600);
            lblCount.Margin = new Padding(3, 0, 1, 0);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(452, 43);
            lblCount.TabIndex = 359;
            lblCount.Text = "-";
            lblCount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblCustomer
            // 
            lblCustomer.BackColor = Color.White;
            lblCustomer.Dock = DockStyle.Fill;
            lblCustomer.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCustomer.Location = new Point(3, 514);
            lblCustomer.Margin = new Padding(3, 0, 1, 0);
            lblCustomer.Name = "lblCustomer";
            lblCustomer.Size = new Size(452, 43);
            lblCustomer.TabIndex = 358;
            lblCustomer.Text = "-";
            lblCustomer.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblLIN
            // 
            lblLIN.BackColor = Color.White;
            lblLIN.Dock = DockStyle.Fill;
            lblLIN.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLIN.Location = new Point(3, 557);
            lblLIN.Margin = new Padding(3, 0, 1, 0);
            lblLIN.Name = "lblLIN";
            lblLIN.Size = new Size(452, 43);
            lblLIN.TabIndex = 357;
            lblLIN.Text = "-";
            lblLIN.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblReelLPN
            // 
            lblReelLPN.BackColor = Color.White;
            lblReelLPN.Dock = DockStyle.Fill;
            lblReelLPN.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblReelLPN.Location = new Point(3, 437);
            lblReelLPN.Margin = new Padding(3, 0, 1, 0);
            lblReelLPN.Name = "lblReelLPN";
            lblReelLPN.Size = new Size(452, 77);
            lblReelLPN.TabIndex = 356;
            lblReelLPN.Text = "-";
            lblReelLPN.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // hWinLayout
            // 
            hWinLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            hWinLayout.AutoValidate = AutoValidate.EnableAllowFocusChange;
            hWinLayout.Dock = DockStyle.Fill;
            hWinLayout.HDoubleClickToFitContent = true;
            hWinLayout.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            hWinLayout.HImagePart = new Rectangle(0, 0, 640, 480);
            hWinLayout.HKeepAspectRatio = true;
            hWinLayout.HMoveContent = true;
            hWinLayout.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            hWinLayout.Location = new Point(3, 0);
            hWinLayout.Margin = new Padding(3, 0, 0, 0);
            hWinLayout.Name = "hWinLayout";
            hWinLayout.Size = new Size(453, 437);
            hWinLayout.TabIndex = 355;
            hWinLayout.WindowSize = new Size(453, 437);
            hWinLayout.Load += hwinImage_Load;
            hWinLayout.MouseEnter += hWinLayout_MouseEnter;
            hWinLayout.MouseLeave += hWinLayout_MouseLeave;
            // 
            // uscMD
            // 
            tlpLayout.SetColumnSpan(uscMD, 2);
            uscMD.Dock = DockStyle.Fill;
            uscMD.Location = new Point(456, 670);
            uscMD.Margin = new Padding(0);
            uscMD.Name = "uscMD";
            uscMD.Size = new Size(962, 306);
            uscMD.TabIndex = 350;
            // 
            // uscProgress
            // 
            tlpLayout.SetColumnSpan(uscProgress, 4);
            uscProgress.Dock = DockStyle.Fill;
            uscProgress.Location = new Point(5, 982);
            uscProgress.Margin = new Padding(5, 6, 5, 6);
            uscProgress.Name = "uscProgress";
            uscProgress.Size = new Size(1718, 38);
            uscProgress.TabIndex = 351;
            uscProgress.Visible = false;
            // 
            // frmInspect
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            ClientSize = new Size(1728, 1026);
            Controls.Add(tlpLayout);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            Margin = new Padding(4, 5, 4, 5);
            MinimizeBox = false;
            Name = "frmInspect";
            ShowInTaskbar = false;
            Text = "LVS3";
            WindowState = FormWindowState.Maximized;
            FormClosing += frmInspect_FormClosing;
            Load += frmInspect_Load;
            KeyDown += frmMain_KeyDown;
            tlpLayout.ResumeLayout(false);
            tlpControl.ResumeLayout(false);
            tlpControl.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tlpImages.ResumeLayout(false);
            tlpHwinCurrent.ResumeLayout(false);
            tlpHwinCurrent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbPass).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tlpToolsTop.ResumeLayout(false);
            tlpToolsTop.PerformLayout();
            tlpToolsBottom.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
        private TableLayoutPanel tlpLayout;
        private TableLayoutPanel tlpImages;
        public uscMessageDisplay uscMD;
        private UserControls.uscProgress uscProgress;
        private TableLayoutPanel tlpControl;
        public Button cmdEndInspection;
        public Button cmdStart;
        private RoundTopLabel lblCurrentImage;
        private Label label2;
        private Label roundTopLabel2;
        private Label roundTopLabel1;
        private Label label4;
        private TableLayoutPanel tableLayoutPanel1;
        private RoundTopLabel roundTopLabel4;
        private TableLayoutPanel tlpHwinCurrent;
        public Label lblTime;
        public Label lblInfoText;
        public HalconDotNet.HSmartWindowControl hWinCurrent;
        public PictureBox pbPass;
        private ListView lstTools;
        private TableLayoutPanel tlpToolsTop;
        private RoundLeftTopLabel label11;
        private TableLayoutPanel tlpToolsBottom;
        private Label lblCount;
        private Label lblCustomer;
        public Label lblLIN;
        public Label lblReelLPN;
        private HalconDotNet.HSmartWindowControl hWinLayout;
        public FlowLayoutPanel flpVDEItem;
        private ColumnHeader columnHeader1;
        private RoundTopLabel rtlSL;
        private TableLayoutPanel tableLayoutPanel2;
        private HScrollBar sbSpeedLimit;
        private Label lblSL;
        private TableLayoutPanel tableLayoutPanel3;
        private RoundTopLabel roundTopLabel3;
        private TableLayoutPanel tableLayoutPanel5;
        private RoundTopLabel roundTopLabel6;
        private RoundTopLabel roundTopLabel5;
        private HScrollBar sbRWDRadius;
        private Label lblRWDRadius;
        private Button cmdFWDReelSmall;
        private Button cmdRWDReelLarge;
        private Button cmdRWDReelSmall;
        private Button cmdFWDReelLarge;
        private TableLayoutPanel tableLayoutPanel4;
    }

    public static class InspectionStatus
    {
        public static Bitmap GetImage(bool pass)
        {
            // return pass.png from Resources if pass is true
            return pass ? InspectionManagement.Properties.Resources.pass : InspectionManagement.Properties.Resources.fail;
        }

    }
}