
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tlpImages = new System.Windows.Forms.TableLayoutPanel();
            this.hWinBackingCam = new HalconDotNet.HSmartWindowControl();
            this.lblCurrentLabelNo = new LVS3.RoundRightLabel();
            this.Label1 = new LVS3.RoundLeftLabel();
            this.roundLabel3 = new LVS3.RoundLeftLabel();
            this.hWinReject = new HalconDotNet.HSmartWindowControl();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblBackingCamDesc = new LVS3.RoundLeftTopLabel();
            this.lblRotate = new LVS3.RoundLeftBottomLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.tlpJobStatus = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new LVS3.RoundTopLabel();
            this.optReject = new System.Windows.Forms.RadioButton();
            this.optAccept = new System.Windows.Forms.RadioButton();
            this.clstReason = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpImages.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tlpJobStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.0178F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.26113F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.64688F));
            this.tableLayoutPanel1.Controls.Add(this.tlpImages, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60.62874F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1.197605F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.02395F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1348, 668);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tlpImages
            // 
            this.tlpImages.ColumnCount = 2;
            this.tlpImages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.59844F));
            this.tlpImages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 88.40156F));
            this.tlpImages.Controls.Add(this.hWinBackingCam, 1, 2);
            this.tlpImages.Controls.Add(this.lblCurrentLabelNo, 1, 0);
            this.tlpImages.Controls.Add(this.Label1, 0, 0);
            this.tlpImages.Controls.Add(this.roundLabel3, 0, 1);
            this.tlpImages.Controls.Add(this.hWinReject, 1, 1);
            this.tlpImages.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tlpImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpImages.Location = new System.Drawing.Point(163, 2);
            this.tlpImages.Margin = new System.Windows.Forms.Padding(1, 2, 1, 1);
            this.tlpImages.Name = "tlpImages";
            this.tlpImages.RowCount = 3;
            this.tlpImages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpImages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpImages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 137F));
            this.tlpImages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpImages.Size = new System.Drawing.Size(1026, 402);
            this.tlpImages.TabIndex = 320;
            // 
            // hWinBackingCam
            // 
            this.hWinBackingCam.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hWinBackingCam.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.hWinBackingCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWinBackingCam.HDoubleClickToFitContent = true;
            this.hWinBackingCam.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            this.hWinBackingCam.HImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWinBackingCam.HKeepAspectRatio = true;
            this.hWinBackingCam.HMoveContent = true;
            this.hWinBackingCam.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            this.hWinBackingCam.Location = new System.Drawing.Point(118, 267);
            this.hWinBackingCam.Margin = new System.Windows.Forms.Padding(0, 2, 4, 2);
            this.hWinBackingCam.Name = "hWinBackingCam";
            this.hWinBackingCam.Size = new System.Drawing.Size(904, 133);
            this.hWinBackingCam.TabIndex = 337;
            this.hWinBackingCam.WindowSize = new System.Drawing.Size(904, 133);
            // 
            // lblCurrentLabelNo
            // 
            this.lblCurrentLabelNo.AutoSize = true;
            this.lblCurrentLabelNo.BackColor = System.Drawing.Color.White;
            this.lblCurrentLabelNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentLabelNo.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentLabelNo.IsLink = false;
            this.lblCurrentLabelNo.Location = new System.Drawing.Point(118, 2);
            this.lblCurrentLabelNo.Margin = new System.Windows.Forms.Padding(0, 2, 4, 1);
            this.lblCurrentLabelNo.Name = "lblCurrentLabelNo";
            this.lblCurrentLabelNo.Size = new System.Drawing.Size(904, 22);
            this.lblCurrentLabelNo.TabIndex = 334;
            this.lblCurrentLabelNo.Text = "-";
            this.lblCurrentLabelNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.Color.Silver;
            this.Label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label1.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.Label1.IsLink = false;
            this.Label1.Location = new System.Drawing.Point(2, 2);
            this.Label1.Margin = new System.Windows.Forms.Padding(2, 2, 0, 1);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(116, 22);
            this.Label1.TabIndex = 326;
            this.Label1.Text = "Current Label:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundLabel3
            // 
            this.roundLabel3.BackColor = System.Drawing.Color.Silver;
            this.roundLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundLabel3.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundLabel3.IsLink = false;
            this.roundLabel3.Location = new System.Drawing.Point(2, 27);
            this.roundLabel3.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.roundLabel3.Name = "roundLabel3";
            this.roundLabel3.Size = new System.Drawing.Size(116, 236);
            this.roundLabel3.TabIndex = 323;
            this.roundLabel3.Text = "Label Under\r\nInvestigation";
            this.roundLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hWinReject
            // 
            this.hWinReject.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hWinReject.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.hWinReject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWinReject.HDoubleClickToFitContent = true;
            this.hWinReject.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            this.hWinReject.HImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWinReject.HKeepAspectRatio = true;
            this.hWinReject.HMoveContent = true;
            this.hWinReject.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            this.hWinReject.Location = new System.Drawing.Point(118, 27);
            this.hWinReject.Margin = new System.Windows.Forms.Padding(0, 2, 4, 2);
            this.hWinReject.Name = "hWinReject";
            this.hWinReject.Size = new System.Drawing.Size(904, 236);
            this.hWinReject.TabIndex = 325;
            this.hWinReject.WindowSize = new System.Drawing.Size(904, 236);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.lblBackingCamDesc, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblRotate, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 265);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 74.45255F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.54745F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(118, 137);
            this.tableLayoutPanel3.TabIndex = 338;
            // 
            // lblBackingCamDesc
            // 
            this.lblBackingCamDesc.AutoSize = true;
            this.lblBackingCamDesc.BackColor = System.Drawing.Color.Silver;
            this.lblBackingCamDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBackingCamDesc.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.lblBackingCamDesc.IsLink = false;
            this.lblBackingCamDesc.Location = new System.Drawing.Point(2, 2);
            this.lblBackingCamDesc.Margin = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.lblBackingCamDesc.Name = "lblBackingCamDesc";
            this.lblBackingCamDesc.Size = new System.Drawing.Size(116, 99);
            this.lblBackingCamDesc.TabIndex = 337;
            this.lblBackingCamDesc.Text = "Backing Cam\r\n\r\nClick For\r\nNew Image";
            this.lblBackingCamDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBackingCamDesc.Click += new System.EventHandler(this.lblBackingCamDesc_Click);
            // 
            // lblRotate
            // 
            this.lblRotate.AutoSize = true;
            this.lblRotate.BackColor = System.Drawing.Color.Silver;
            this.lblRotate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRotate.Image = ((System.Drawing.Image)(resources.GetObject("lblRotate.Image")));
            this.lblRotate.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRotate.IsLink = false;
            this.lblRotate.Location = new System.Drawing.Point(3, 101);
            this.lblRotate.Margin = new System.Windows.Forms.Padding(3, 0, 0, 2);
            this.lblRotate.Name = "lblRotate";
            this.lblRotate.Size = new System.Drawing.Size(115, 34);
            this.lblRotate.TabIndex = 338;
            this.lblRotate.Text = "Rotate";
            this.lblRotate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRotate.Click += new System.EventHandler(this.lblRotate_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 8;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.208843F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.82126F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 3.474043F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.94974F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.74974F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.74974F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.16671F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayoutPanel2.Controls.Add(this.txtReason, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.Label5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tlpJobStatus, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.clstReason, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 4, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtUsername, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtPassword, 5, 2);
            this.tableLayoutPanel2.Controls.Add(this.cmdOK, 5, 6);
            this.tableLayoutPanel2.Controls.Add(this.cmdCancel, 6, 6);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(162, 413);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 9;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.053498F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.04527F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.80701F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.21705F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.426357F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.302325F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.812731F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.672657F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1028, 255);
            this.tableLayoutPanel2.TabIndex = 321;
            // 
            // txtReason
            // 
            this.txtReason.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel2.SetColumnSpan(this.txtReason, 3);
            this.txtReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReason.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReason.Location = new System.Drawing.Point(60, 181);
            this.txtReason.Margin = new System.Windows.Forms.Padding(0);
            this.txtReason.Multiline = true;
            this.txtReason.Name = "txtReason";
            this.tableLayoutPanel2.SetRowSpan(this.txtReason, 3);
            this.txtReason.Size = new System.Drawing.Size(544, 62);
            this.txtReason.TabIndex = 99;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Silver;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Calibri", 12F);
            this.label6.Location = new System.Drawing.Point(0, 181);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.tableLayoutPanel2.SetRowSpan(this.label6, 3);
            this.label6.Size = new System.Drawing.Size(60, 62);
            this.label6.TabIndex = 98;
            this.label6.Text = "Other\r\nReason";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label5
            // 
            this.Label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Label5.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.SetColumnSpan(this.Label5, 8);
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Label5.ForeColor = System.Drawing.Color.Black;
            this.Label5.Location = new System.Drawing.Point(3, 0);
            this.Label5.Margin = new System.Windows.Forms.Padding(3, 0, 3, 2);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(1022, 20);
            this.Label5.TabIndex = 97;
            this.Label5.Text = "By providing your username and password you agree to the following statement:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tlpJobStatus
            // 
            this.tlpJobStatus.BackColor = System.Drawing.Color.Transparent;
            this.tlpJobStatus.ColumnCount = 2;
            this.tlpJobStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.99999F));
            this.tlpJobStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.00002F));
            this.tlpJobStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpJobStatus.Controls.Add(this.label2, 0, 0);
            this.tlpJobStatus.Controls.Add(this.optReject, 1, 1);
            this.tlpJobStatus.Controls.Add(this.optAccept, 0, 1);
            this.tlpJobStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpJobStatus.Location = new System.Drawing.Point(382, 22);
            this.tlpJobStatus.Margin = new System.Windows.Forms.Padding(0);
            this.tlpJobStatus.Name = "tlpJobStatus";
            this.tlpJobStatus.RowCount = 2;
            this.tableLayoutPanel2.SetRowSpan(this.tlpJobStatus, 2);
            this.tlpJobStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44.3038F));
            this.tlpJobStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55.6962F));
            this.tlpJobStatus.Size = new System.Drawing.Size(222, 102);
            this.tlpJobStatus.TabIndex = 90;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpJobStatus.SetColumnSpan(this.label2, 2);
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Calibri Light", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.IsLink = false;
            this.label2.Location = new System.Drawing.Point(2, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(218, 43);
            this.label2.TabIndex = 83;
            this.label2.Text = "Decision";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // optReject
            // 
            this.optReject.Appearance = System.Windows.Forms.Appearance.Button;
            this.optReject.AutoSize = true;
            this.optReject.BackColor = System.Drawing.Color.Gainsboro;
            this.optReject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optReject.FlatAppearance.CheckedBackColor = System.Drawing.Color.OrangeRed;
            this.optReject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optReject.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optReject.Location = new System.Drawing.Point(110, 47);
            this.optReject.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.optReject.Name = "optReject";
            this.optReject.Size = new System.Drawing.Size(112, 53);
            this.optReject.TabIndex = 1;
            this.optReject.Text = "REJECT";
            this.optReject.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optReject.UseVisualStyleBackColor = false;
            // 
            // optAccept
            // 
            this.optAccept.Appearance = System.Windows.Forms.Appearance.Button;
            this.optAccept.AutoSize = true;
            this.optAccept.BackColor = System.Drawing.Color.Gainsboro;
            this.optAccept.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optAccept.FlatAppearance.CheckedBackColor = System.Drawing.Color.LimeGreen;
            this.optAccept.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optAccept.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optAccept.Location = new System.Drawing.Point(2, 47);
            this.optAccept.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.optAccept.Name = "optAccept";
            this.optAccept.Size = new System.Drawing.Size(108, 53);
            this.optAccept.TabIndex = 0;
            this.optAccept.Text = "ACCEPT";
            this.optAccept.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optAccept.UseVisualStyleBackColor = false;
            // 
            // clstReason
            // 
            this.clstReason.CheckOnClick = true;
            this.tableLayoutPanel2.SetColumnSpan(this.clstReason, 2);
            this.clstReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clstReason.Font = new System.Drawing.Font("Calibri", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clstReason.FormattingEnabled = true;
            this.clstReason.Items.AddRange(new object[] {
            "Patchy Print",
            "Mark On Label",
            "Ribbon Wrinkle",
            "Barcode Scanned Manually",
            "Text Movement",
            "Other"});
            this.clstReason.Location = new System.Drawing.Point(0, 24);
            this.clstReason.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.clstReason.Name = "clstReason";
            this.tableLayoutPanel2.SetRowSpan(this.clstReason, 3);
            this.clstReason.Size = new System.Drawing.Size(349, 144);
            this.clstReason.TabIndex = 81;
            this.clstReason.ThreeDCheckBoxes = true;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Calibri", 12F);
            this.label3.Location = new System.Drawing.Point(604, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 50);
            this.label3.TabIndex = 91;
            this.label3.Text = "Username";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Calibri", 12F);
            this.label4.Location = new System.Drawing.Point(604, 84);
            this.label4.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 40);
            this.label4.TabIndex = 92;
            this.label4.Text = "Password";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtUsername
            // 
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel2.SetColumnSpan(this.txtUsername, 2);
            this.txtUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUsername.Font = new System.Drawing.Font("Calibri", 14F);
            this.txtUsername.Location = new System.Drawing.Point(727, 24);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(241, 30);
            this.txtUsername.TabIndex = 93;
            // 
            // txtPassword
            // 
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel2.SetColumnSpan(this.txtPassword, 2);
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPassword.Font = new System.Drawing.Font("Calibri", 14F);
            this.txtPassword.Location = new System.Drawing.Point(727, 80);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.ShortcutsEnabled = false;
            this.txtPassword.Size = new System.Drawing.Size(241, 30);
            this.txtPassword.TabIndex = 94;
            // 
            // cmdOK
            // 
            this.cmdOK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdOK.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.Location = new System.Drawing.Point(730, 206);
            this.cmdOK.Name = "cmdOK";
            this.tableLayoutPanel2.SetRowSpan(this.cmdOK, 2);
            this.cmdOK.Size = new System.Drawing.Size(117, 34);
            this.cmdOK.TabIndex = 95;
            this.cmdOK.Text = "OK";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdCancel.Font = new System.Drawing.Font("Calibri", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCancel.Location = new System.Drawing.Point(853, 206);
            this.cmdCancel.Name = "cmdCancel";
            this.tableLayoutPanel2.SetRowSpan(this.cmdCancel, 2);
            this.cmdCancel.Size = new System.Drawing.Size(112, 34);
            this.cmdCancel.TabIndex = 96;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // frmUnderInvestigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1348, 668);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmUnderInvestigation";
            this.Text = "Label Under Investigation";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tlpImages.ResumeLayout(false);
            this.tlpImages.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tlpJobStatus.ResumeLayout(false);
            this.tlpJobStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tlpImages;
        private LVS3.RoundRightLabel lblCurrentLabelNo;
        private LVS3.RoundLeftLabel Label1;
        private LVS3.RoundLeftLabel roundLabel3;
        private HalconDotNet.HSmartWindowControl hWinReject;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckedListBox clstReason;
        private System.Windows.Forms.TableLayoutPanel tlpJobStatus;
        public LVS3.RoundTopLabel label2;
        private System.Windows.Forms.RadioButton optReject;
        private System.Windows.Forms.RadioButton optAccept;
        internal System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.TextBox txtReason;
        internal System.Windows.Forms.Label label6;
        private HalconDotNet.HSmartWindowControl hWinBackingCam;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private RoundLeftTopLabel lblBackingCamDesc;
        private RoundLeftBottomLabel lblRotate;
    }
}