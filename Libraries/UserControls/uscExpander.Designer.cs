namespace LVS3
{
    partial class uscExpander
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscExpander));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblMasking = new LVS3.RoundLeftLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel27 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.optMaskDarkC = new LVS3.RoundRadioButton();
            this.optMaskBrightC = new LVS3.RoundRadioButton();
            this.optMaskFullC = new LVS3.RoundRadioButton();
            this.optMaskDarkR = new LVS3.RoundRadioButton();
            this.optMaskBrightR = new LVS3.RoundRadioButton();
            this.optMaskFullR = new LVS3.RoundRadioButton();
            this.lblTT = new System.Windows.Forms.Label();
            this.pnlProductionData = new System.Windows.Forms.Panel();
            this.tlpProductionData = new System.Windows.Forms.TableLayoutPanel();
            this.cmdGoToProduction = new LVS3.RoundButton();
            this.label28 = new System.Windows.Forms.Label();
            this.lblPHData = new System.Windows.Forms.Label();
            this.lblMRData = new System.Windows.Forms.Label();
            this.lblHWidth = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.lblPWidth = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.lblProductName = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.lblReview = new LVS3.RoundLeftLabel();
            this.tlpMask = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel27.SuspendLayout();
            this.pnlProductionData.SuspendLayout();
            this.tlpProductionData.SuspendLayout();
            this.tlpMask.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "up_arrow.png");
            this.imageList1.Images.SetKeyName(1, "down_arrow.png");
            // 
            // lblMasking
            // 
            this.lblMasking.AutoSize = true;
            this.lblMasking.BackColor = System.Drawing.Color.LightGray;
            this.lblMasking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMasking.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMasking.Image = ((System.Drawing.Image)(resources.GetObject("lblMasking.Image")));
            this.lblMasking.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblMasking.IsLink = true;
            this.lblMasking.Location = new System.Drawing.Point(1, 1);
            this.lblMasking.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.lblMasking.Name = "lblMasking";
            this.lblMasking.Size = new System.Drawing.Size(233, 38);
            this.lblMasking.TabIndex = 8;
            this.lblMasking.Text = "Masking";
            this.lblMasking.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMasking.Click += new System.EventHandler(this.MaskButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightGray;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 308);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(234, 55);
            this.panel1.TabIndex = 2;
            // 
            // tableLayoutPanel27
            // 
            this.tableLayoutPanel27.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel27.ColumnCount = 2;
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel27.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel27.Controls.Add(this.label3, 1, 1);
            this.tableLayoutPanel27.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel27.Controls.Add(this.label20, 0, 0);
            this.tableLayoutPanel27.Controls.Add(this.optMaskDarkC, 1, 4);
            this.tableLayoutPanel27.Controls.Add(this.optMaskBrightC, 1, 3);
            this.tableLayoutPanel27.Controls.Add(this.optMaskFullC, 1, 2);
            this.tableLayoutPanel27.Controls.Add(this.optMaskDarkR, 0, 4);
            this.tableLayoutPanel27.Controls.Add(this.optMaskBrightR, 0, 3);
            this.tableLayoutPanel27.Controls.Add(this.optMaskFullR, 0, 2);
            this.tableLayoutPanel27.Controls.Add(this.lblTT, 0, 5);
            this.tableLayoutPanel27.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel27.Location = new System.Drawing.Point(3, 84);
            this.tableLayoutPanel27.Name = "tableLayoutPanel27";
            this.tableLayoutPanel27.RowCount = 7;
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.03965F));
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.7084F));
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.24063F));
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.24063F));
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.24063F));
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.53007F));
            this.tableLayoutPanel27.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel27.Size = new System.Drawing.Size(228, 1);
            this.tableLayoutPanel27.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(116, -1);
            this.label3.Margin = new System.Windows.Forms.Padding(2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 1);
            this.label3.TabIndex = 29;
            this.label3.Text = "Circular";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(2, -1);
            this.label2.Margin = new System.Windows.Forms.Padding(2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 1);
            this.label2.TabIndex = 28;
            this.label2.Text = "Rectangular";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel27.SetColumnSpan(this.label20, 2);
            this.label20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label20.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(0, 0);
            this.label20.Margin = new System.Windows.Forms.Padding(0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(228, 1);
            this.label20.TabIndex = 18;
            this.label20.Text = "Mask Types";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // optMaskDarkC
            // 
            this.optMaskDarkC.Appearance = System.Windows.Forms.Appearance.Button;
            this.optMaskDarkC.BackColor = System.Drawing.Color.Gray;
            this.optMaskDarkC.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.optMaskDarkC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optMaskDarkC.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.optMaskDarkC.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.optMaskDarkC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.optMaskDarkC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optMaskDarkC.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optMaskDarkC.Image = ((System.Drawing.Image)(resources.GetObject("optMaskDarkC.Image")));
            this.optMaskDarkC.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.optMaskDarkC.Location = new System.Drawing.Point(117, -12);
            this.optMaskDarkC.Name = "optMaskDarkC";
            this.optMaskDarkC.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.optMaskDarkC.Size = new System.Drawing.Size(108, 1);
            this.optMaskDarkC.TabIndex = 27;
            this.optMaskDarkC.TabStop = true;
            this.optMaskDarkC.Text = "Dark";
            this.optMaskDarkC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optMaskDarkC.UseVisualStyleBackColor = false;
            this.optMaskDarkC.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Masks_MouseDown);
            this.optMaskDarkC.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Mask_MouseMove);
            // 
            // optMaskBrightC
            // 
            this.optMaskBrightC.Appearance = System.Windows.Forms.Appearance.Button;
            this.optMaskBrightC.BackColor = System.Drawing.Color.Gray;
            this.optMaskBrightC.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.optMaskBrightC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optMaskBrightC.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.optMaskBrightC.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.optMaskBrightC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.optMaskBrightC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optMaskBrightC.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optMaskBrightC.Image = ((System.Drawing.Image)(resources.GetObject("optMaskBrightC.Image")));
            this.optMaskBrightC.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.optMaskBrightC.Location = new System.Drawing.Point(117, -8);
            this.optMaskBrightC.Name = "optMaskBrightC";
            this.optMaskBrightC.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.optMaskBrightC.Size = new System.Drawing.Size(108, 1);
            this.optMaskBrightC.TabIndex = 26;
            this.optMaskBrightC.TabStop = true;
            this.optMaskBrightC.Text = "Bright";
            this.optMaskBrightC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optMaskBrightC.UseVisualStyleBackColor = false;
            this.optMaskBrightC.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Masks_MouseDown);
            this.optMaskBrightC.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Mask_MouseMove);
            // 
            // optMaskFullC
            // 
            this.optMaskFullC.Appearance = System.Windows.Forms.Appearance.Button;
            this.optMaskFullC.BackColor = System.Drawing.Color.Gray;
            this.optMaskFullC.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.optMaskFullC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optMaskFullC.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.optMaskFullC.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.optMaskFullC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.optMaskFullC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optMaskFullC.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optMaskFullC.Image = ((System.Drawing.Image)(resources.GetObject("optMaskFullC.Image")));
            this.optMaskFullC.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.optMaskFullC.Location = new System.Drawing.Point(117, -4);
            this.optMaskFullC.Name = "optMaskFullC";
            this.optMaskFullC.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.optMaskFullC.Size = new System.Drawing.Size(108, 1);
            this.optMaskFullC.TabIndex = 25;
            this.optMaskFullC.TabStop = true;
            this.optMaskFullC.Text = "All";
            this.optMaskFullC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optMaskFullC.UseVisualStyleBackColor = false;
            this.optMaskFullC.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Masks_MouseDown);
            this.optMaskFullC.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Mask_MouseMove);
            // 
            // optMaskDarkR
            // 
            this.optMaskDarkR.Appearance = System.Windows.Forms.Appearance.Button;
            this.optMaskDarkR.BackColor = System.Drawing.Color.Gray;
            this.optMaskDarkR.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.optMaskDarkR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optMaskDarkR.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.optMaskDarkR.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.optMaskDarkR.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.optMaskDarkR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optMaskDarkR.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optMaskDarkR.Image = ((System.Drawing.Image)(resources.GetObject("optMaskDarkR.Image")));
            this.optMaskDarkR.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.optMaskDarkR.Location = new System.Drawing.Point(3, -12);
            this.optMaskDarkR.Name = "optMaskDarkR";
            this.optMaskDarkR.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.optMaskDarkR.Size = new System.Drawing.Size(108, 1);
            this.optMaskDarkR.TabIndex = 23;
            this.optMaskDarkR.TabStop = true;
            this.optMaskDarkR.Text = "Dark";
            this.optMaskDarkR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optMaskDarkR.UseVisualStyleBackColor = false;
            this.optMaskDarkR.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Masks_MouseDown);
            this.optMaskDarkR.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Mask_MouseMove);
            // 
            // optMaskBrightR
            // 
            this.optMaskBrightR.Appearance = System.Windows.Forms.Appearance.Button;
            this.optMaskBrightR.BackColor = System.Drawing.Color.Gray;
            this.optMaskBrightR.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.optMaskBrightR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optMaskBrightR.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.optMaskBrightR.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.optMaskBrightR.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.optMaskBrightR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optMaskBrightR.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optMaskBrightR.Image = ((System.Drawing.Image)(resources.GetObject("optMaskBrightR.Image")));
            this.optMaskBrightR.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.optMaskBrightR.Location = new System.Drawing.Point(3, -8);
            this.optMaskBrightR.Name = "optMaskBrightR";
            this.optMaskBrightR.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.optMaskBrightR.Size = new System.Drawing.Size(108, 1);
            this.optMaskBrightR.TabIndex = 22;
            this.optMaskBrightR.TabStop = true;
            this.optMaskBrightR.Text = "Bright";
            this.optMaskBrightR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optMaskBrightR.UseVisualStyleBackColor = false;
            this.optMaskBrightR.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Masks_MouseDown);
            this.optMaskBrightR.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Mask_MouseMove);
            // 
            // optMaskFullR
            // 
            this.optMaskFullR.Appearance = System.Windows.Forms.Appearance.Button;
            this.optMaskFullR.BackColor = System.Drawing.Color.Gray;
            this.optMaskFullR.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.optMaskFullR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optMaskFullR.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.optMaskFullR.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.optMaskFullR.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.optMaskFullR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optMaskFullR.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optMaskFullR.Image = ((System.Drawing.Image)(resources.GetObject("optMaskFullR.Image")));
            this.optMaskFullR.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.optMaskFullR.Location = new System.Drawing.Point(3, -4);
            this.optMaskFullR.Name = "optMaskFullR";
            this.optMaskFullR.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.optMaskFullR.Size = new System.Drawing.Size(108, 1);
            this.optMaskFullR.TabIndex = 21;
            this.optMaskFullR.TabStop = true;
            this.optMaskFullR.Text = "All";
            this.optMaskFullR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optMaskFullR.UseVisualStyleBackColor = false;
            this.optMaskFullR.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Masks_MouseDown);
            this.optMaskFullR.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Mask_MouseMove);
            // 
            // lblTT
            // 
            this.lblTT.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel27.SetColumnSpan(this.lblTT, 2);
            this.lblTT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTT.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTT.Location = new System.Drawing.Point(2, -17);
            this.lblTT.Margin = new System.Windows.Forms.Padding(2);
            this.lblTT.Name = "lblTT";
            this.tableLayoutPanel27.SetRowSpan(this.lblTT, 2);
            this.lblTT.Size = new System.Drawing.Size(224, 19);
            this.lblTT.TabIndex = 30;
            this.lblTT.Text = "...";
            this.lblTT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlProductionData
            // 
            this.pnlProductionData.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlProductionData.Controls.Add(this.tlpProductionData);
            this.pnlProductionData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProductionData.Location = new System.Drawing.Point(0, 363);
            this.pnlProductionData.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProductionData.Name = "pnlProductionData";
            this.pnlProductionData.Size = new System.Drawing.Size(234, 1);
            this.pnlProductionData.TabIndex = 27;
            // 
            // tlpProductionData
            // 
            this.tlpProductionData.BackColor = System.Drawing.Color.White;
            this.tlpProductionData.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpProductionData.ColumnCount = 2;
            this.tlpProductionData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.26455F));
            this.tlpProductionData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.73545F));
            this.tlpProductionData.Controls.Add(this.cmdGoToProduction, 0, 5);
            this.tlpProductionData.Controls.Add(this.label28, 0, 3);
            this.tlpProductionData.Controls.Add(this.lblPHData, 1, 3);
            this.tlpProductionData.Controls.Add(this.lblMRData, 1, 4);
            this.tlpProductionData.Controls.Add(this.lblHWidth, 1, 2);
            this.tlpProductionData.Controls.Add(this.label29, 0, 4);
            this.tlpProductionData.Controls.Add(this.label31, 0, 1);
            this.tlpProductionData.Controls.Add(this.lblPWidth, 1, 1);
            this.tlpProductionData.Controls.Add(this.label32, 0, 2);
            this.tlpProductionData.Controls.Add(this.lblProductName, 1, 0);
            this.tlpProductionData.Controls.Add(this.label33, 0, 0);
            this.tlpProductionData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProductionData.Location = new System.Drawing.Point(0, 0);
            this.tlpProductionData.Margin = new System.Windows.Forms.Padding(20, 0, 20, 10);
            this.tlpProductionData.Name = "tlpProductionData";
            this.tlpProductionData.RowCount = 6;
            this.tlpProductionData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.16783F));
            this.tlpProductionData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.97902F));
            this.tlpProductionData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.tlpProductionData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.97902F));
            this.tlpProductionData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.39161F));
            this.tlpProductionData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.48252F));
            this.tlpProductionData.Size = new System.Drawing.Size(234, 1);
            this.tlpProductionData.TabIndex = 21;
            // 
            // cmdGoToProduction
            // 
            this.cmdGoToProduction.BackColor = System.Drawing.Color.LimeGreen;
            this.tlpProductionData.SetColumnSpan(this.cmdGoToProduction, 2);
            this.cmdGoToProduction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdGoToProduction.Enabled = false;
            this.cmdGoToProduction.FlatAppearance.BorderColor = System.Drawing.Color.LimeGreen;
            this.cmdGoToProduction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdGoToProduction.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGoToProduction.Image = ((System.Drawing.Image)(resources.GetObject("cmdGoToProduction.Image")));
            this.cmdGoToProduction.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdGoToProduction.Location = new System.Drawing.Point(3, 4);
            this.cmdGoToProduction.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.cmdGoToProduction.Name = "cmdGoToProduction";
            this.cmdGoToProduction.Size = new System.Drawing.Size(228, 1);
            this.cmdGoToProduction.TabIndex = 75;
            this.cmdGoToProduction.Text = "Continue To Production";
            this.cmdGoToProduction.UseVisualStyleBackColor = false;
            this.cmdGoToProduction.Click += new System.EventHandler(this.cmdGoToProduction_Click);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.BackColor = System.Drawing.Color.Gainsboro;
            this.label28.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label28.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(1, 3);
            this.label28.Margin = new System.Windows.Forms.Padding(0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(116, 1);
            this.label28.TabIndex = 57;
            this.label28.Text = "Punch Hole Data";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPHData
            // 
            this.lblPHData.AutoSize = true;
            this.lblPHData.BackColor = System.Drawing.Color.White;
            this.lblPHData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPHData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPHData.Location = new System.Drawing.Point(120, 5);
            this.lblPHData.Margin = new System.Windows.Forms.Padding(2);
            this.lblPHData.Name = "lblPHData";
            this.lblPHData.Size = new System.Drawing.Size(111, 1);
            this.lblPHData.TabIndex = 56;
            this.lblPHData.Text = "-";
            this.lblPHData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMRData
            // 
            this.lblMRData.AutoSize = true;
            this.lblMRData.BackColor = System.Drawing.Color.White;
            this.lblMRData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMRData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMRData.Location = new System.Drawing.Point(120, 5);
            this.lblMRData.Margin = new System.Windows.Forms.Padding(2);
            this.lblMRData.Name = "lblMRData";
            this.lblMRData.Size = new System.Drawing.Size(111, 1);
            this.lblMRData.TabIndex = 53;
            this.lblMRData.Text = "-";
            this.lblMRData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHWidth
            // 
            this.lblHWidth.AutoSize = true;
            this.lblHWidth.BackColor = System.Drawing.Color.White;
            this.lblHWidth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHWidth.Location = new System.Drawing.Point(120, 4);
            this.lblHWidth.Margin = new System.Windows.Forms.Padding(2);
            this.lblHWidth.Name = "lblHWidth";
            this.lblHWidth.Size = new System.Drawing.Size(111, 1);
            this.lblHWidth.TabIndex = 51;
            this.lblHWidth.Text = "-";
            this.lblHWidth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.BackColor = System.Drawing.Color.Gainsboro;
            this.label29.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label29.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(1, 3);
            this.label29.Margin = new System.Windows.Forms.Padding(0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(116, 1);
            this.label29.TabIndex = 43;
            this.label29.Text = "Mask Region Data";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.BackColor = System.Drawing.Color.Gainsboro;
            this.label31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label31.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.label31.Location = new System.Drawing.Point(1, 2);
            this.label31.Margin = new System.Windows.Forms.Padding(0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(116, 1);
            this.label31.TabIndex = 41;
            this.label31.Text = "Web Width";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPWidth
            // 
            this.lblPWidth.AutoSize = true;
            this.lblPWidth.BackColor = System.Drawing.Color.White;
            this.lblPWidth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPWidth.Location = new System.Drawing.Point(120, 4);
            this.lblPWidth.Margin = new System.Windows.Forms.Padding(2);
            this.lblPWidth.Name = "lblPWidth";
            this.lblPWidth.Size = new System.Drawing.Size(111, 1);
            this.lblPWidth.TabIndex = 40;
            this.lblPWidth.Text = "-";
            this.lblPWidth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.BackColor = System.Drawing.Color.Gainsboro;
            this.label32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label32.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.label32.Location = new System.Drawing.Point(1, 2);
            this.label32.Margin = new System.Windows.Forms.Padding(0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(116, 1);
            this.label32.TabIndex = 36;
            this.label32.Text = "Insert/Header Width";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSize = true;
            this.lblProductName.BackColor = System.Drawing.Color.White;
            this.lblProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductName.Location = new System.Drawing.Point(120, 3);
            this.lblProductName.Margin = new System.Windows.Forms.Padding(2);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(111, 1);
            this.lblProductName.TabIndex = 24;
            this.lblProductName.Text = "-";
            this.lblProductName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.BackColor = System.Drawing.Color.Gainsboro;
            this.label33.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label33.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(1, 1);
            this.label33.Margin = new System.Windows.Forms.Padding(0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(116, 1);
            this.label33.TabIndex = 23;
            this.label33.Text = "Product";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblReview
            // 
            this.lblReview.AutoSize = true;
            this.lblReview.BackColor = System.Drawing.Color.LightGray;
            this.lblReview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReview.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReview.Image = ((System.Drawing.Image)(resources.GetObject("lblReview.Image")));
            this.lblReview.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblReview.IsLink = true;
            this.lblReview.Location = new System.Drawing.Point(1, 41);
            this.lblReview.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
            this.lblReview.Name = "lblReview";
            this.lblReview.Size = new System.Drawing.Size(233, 39);
            this.lblReview.TabIndex = 28;
            this.lblReview.Text = "Review";
            this.lblReview.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblReview.Click += new System.EventHandler(this.ReviewButton_Click);
            // 
            // tlpMask
            // 
            this.tlpMask.BackColor = System.Drawing.Color.Lavender;
            this.tlpMask.ColumnCount = 1;
            this.tlpMask.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.53796F));
            this.tlpMask.Controls.Add(this.lblReview, 0, 1);
            this.tlpMask.Controls.Add(this.pnlProductionData, 0, 4);
            this.tlpMask.Controls.Add(this.tableLayoutPanel27, 0, 2);
            this.tlpMask.Controls.Add(this.panel1, 0, 3);
            this.tlpMask.Controls.Add(this.lblMasking, 0, 0);
            this.tlpMask.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMask.Location = new System.Drawing.Point(0, 0);
            this.tlpMask.Margin = new System.Windows.Forms.Padding(1);
            this.tlpMask.Name = "tlpMask";
            this.tlpMask.RowCount = 5;
            this.tlpMask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpMask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tlpMask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.tlpMask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.tlpMask.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMask.Size = new System.Drawing.Size(234, 363);
            this.tlpMask.TabIndex = 0;
            // 
            // uscExpander
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMask);
            this.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.Name = "uscExpander";
            this.Size = new System.Drawing.Size(234, 363);
            this.tableLayoutPanel27.ResumeLayout(false);
            this.pnlProductionData.ResumeLayout(false);
            this.tlpProductionData.ResumeLayout(false);
            this.tlpProductionData.PerformLayout();
            this.tlpMask.ResumeLayout(false);
            this.tlpMask.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private LVS3.RoundLeftLabel lblMasking;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel27;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label20;
        private LVS3.RoundRadioButton optMaskDarkC;
        private LVS3.RoundRadioButton optMaskBrightC;
        private LVS3.RoundRadioButton optMaskFullC;
        private LVS3.RoundRadioButton optMaskDarkR;
        private LVS3.RoundRadioButton optMaskBrightR;
        private LVS3.RoundRadioButton optMaskFullR;
        private System.Windows.Forms.Label lblTT;
        private System.Windows.Forms.Panel pnlProductionData;
        private System.Windows.Forms.TableLayoutPanel tlpProductionData;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label lblPHData;
        private System.Windows.Forms.Label lblMRData;
        private System.Windows.Forms.Label lblHWidth;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label lblPWidth;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label label33;
        private LVS3.RoundLeftLabel lblReview;
        private System.Windows.Forms.TableLayoutPanel tlpMask;
        private LVS3.RoundButton cmdGoToProduction;
    }
}
