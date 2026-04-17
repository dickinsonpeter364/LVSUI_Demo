
namespace LVS3
{
    partial class frmEditVariation
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblMessage = new LVS3.RoundTopLabel();
            this.hWinOCR = new HalconDotNet.HSmartWindowControl();
            this.tlpMask2 = new System.Windows.Forms.TableLayoutPanel();
            this.roundTopLabel2 = new LVS3.RoundTopLabel();
            this.tbDark = new System.Windows.Forms.TrackBar();
            this.lblDark = new LVS3.RoundBottomLabel();
            this.lblDark1 = new LVS3.RoundBottomLabel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.uscPixelData1 = new LVS3.uscPixelData();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpMask2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbDark)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1.3947F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 98.6053F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 194F));
            this.tableLayoutPanel1.Controls.Add(this.lblMessage, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.hWinOCR, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tlpMask2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3.157895F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 96.8421F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1115, 703);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.TabStop = true;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.BackColor = System.Drawing.Color.Black;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("Calibri", 12F);
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.IsLink = false;
            this.lblMessage.Location = new System.Drawing.Point(12, 22);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(908, 25);
            this.lblMessage.TabIndex = 119;
            this.lblMessage.Text = "...";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hWinOCR
            // 
            this.hWinOCR.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hWinOCR.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.hWinOCR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWinOCR.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.hWinOCR.ForeColor = System.Drawing.Color.LimeGreen;
            this.hWinOCR.HDoubleClickToFitContent = true;
            this.hWinOCR.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            this.hWinOCR.HImagePart = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.hWinOCR.HKeepAspectRatio = true;
            this.hWinOCR.HMoveContent = true;
            this.hWinOCR.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            this.hWinOCR.Location = new System.Drawing.Point(12, 47);
            this.hWinOCR.Margin = new System.Windows.Forms.Padding(0);
            this.hWinOCR.Name = "hWinOCR";
            this.hWinOCR.Size = new System.Drawing.Size(908, 614);
            this.hWinOCR.TabIndex = 106;
            this.hWinOCR.WindowSize = new System.Drawing.Size(908, 614);
            this.hWinOCR.HMouseMove += new HalconDotNet.HMouseEventHandler(this.hWinOCR_HMouseMove);
            this.hWinOCR.Load += new System.EventHandler(this.hwinImage_Load);
            this.hWinOCR.MouseEnter += new System.EventHandler(this.WinControls_MouseEnter);
            this.hWinOCR.MouseLeave += new System.EventHandler(this.WinControls_MouseLeave);
            // 
            // tlpMask2
            // 
            this.tlpMask2.BackColor = System.Drawing.Color.LightSlateGray;
            this.tlpMask2.ColumnCount = 2;
            this.tlpMask2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMask2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMask2.Controls.Add(this.roundTopLabel2, 0, 0);
            this.tlpMask2.Controls.Add(this.tbDark, 0, 2);
            this.tlpMask2.Controls.Add(this.lblDark, 0, 3);
            this.tlpMask2.Controls.Add(this.lblDark1, 0, 1);
            this.tlpMask2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMask2.Location = new System.Drawing.Point(922, 0);
            this.tlpMask2.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.tlpMask2.Name = "tlpMask2";
            this.tlpMask2.RowCount = 4;
            this.tableLayoutPanel1.SetRowSpan(this.tlpMask2, 4);
            this.tlpMask2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tlpMask2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tlpMask2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMask2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tlpMask2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMask2.Size = new System.Drawing.Size(193, 687);
            this.tlpMask2.TabIndex = 105;
            this.tlpMask2.MouseEnter += new System.EventHandler(this.WinControls_MouseEnter);
            this.tlpMask2.MouseLeave += new System.EventHandler(this.WinControls_MouseLeave);
            // 
            // roundTopLabel2
            // 
            this.roundTopLabel2.BackColor = System.Drawing.Color.LightGray;
            this.tlpMask2.SetColumnSpan(this.roundTopLabel2, 2);
            this.roundTopLabel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.roundTopLabel2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.roundTopLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundTopLabel2.IsLink = false;
            this.roundTopLabel2.Location = new System.Drawing.Point(3, 10);
            this.roundTopLabel2.Name = "roundTopLabel2";
            this.roundTopLabel2.Size = new System.Drawing.Size(187, 47);
            this.roundTopLabel2.TabIndex = 147;
            this.roundTopLabel2.Text = "OpZone Detection\r\nSettings";
            this.roundTopLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbDark
            // 
            this.tbDark.AutoSize = false;
            this.tbDark.BackColor = System.Drawing.Color.White;
            this.tlpMask2.SetColumnSpan(this.tbDark, 2);
            this.tbDark.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbDark.Enabled = false;
            this.tbDark.LargeChange = 1;
            this.tbDark.Location = new System.Drawing.Point(32, 98);
            this.tbDark.Margin = new System.Windows.Forms.Padding(32, 2, 32, 0);
            this.tbDark.Maximum = 80;
            this.tbDark.Minimum = 1;
            this.tbDark.Name = "tbDark";
            this.tbDark.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbDark.Size = new System.Drawing.Size(129, 545);
            this.tbDark.TabIndex = 119;
            this.tbDark.TickFrequency = 10;
            this.tbDark.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbDark.Value = 5;
            this.tbDark.Scroll += new System.EventHandler(this.tbDark_Scroll);
            this.tbDark.ValueChanged += new System.EventHandler(this.sliderValueChangeDark);
            this.tbDark.MouseEnter += new System.EventHandler(this.WinControls_MouseEnter);
            this.tbDark.MouseLeave += new System.EventHandler(this.WinControls_MouseLeave);
            // 
            // lblDark
            // 
            this.lblDark.AutoSize = true;
            this.lblDark.BackColor = System.Drawing.Color.White;
            this.lblDark.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tlpMask2.SetColumnSpan(this.lblDark, 2);
            this.lblDark.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDark.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDark.IsLink = false;
            this.lblDark.Location = new System.Drawing.Point(54, 643);
            this.lblDark.Margin = new System.Windows.Forms.Padding(54, 0, 54, 10);
            this.lblDark.Name = "lblDark";
            this.lblDark.Size = new System.Drawing.Size(85, 34);
            this.lblDark.TabIndex = 144;
            this.lblDark.Text = "0";
            this.lblDark.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDark.Click += new System.EventHandler(this.lblDark_Click);
            // 
            // lblDark1
            // 
            this.lblDark1.BackColor = System.Drawing.Color.LightGray;
            this.tlpMask2.SetColumnSpan(this.lblDark1, 2);
            this.lblDark1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDark1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblDark1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDark1.IsLink = false;
            this.lblDark1.Location = new System.Drawing.Point(3, 57);
            this.lblDark1.Name = "lblDark1";
            this.lblDark1.Size = new System.Drawing.Size(187, 39);
            this.lblDark1.TabIndex = 148;
            this.lblDark1.Text = "Threshold\r\nAdjustment";
            this.lblDark1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.uscPixelData1, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(12, 661);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(908, 26);
            this.tableLayoutPanel3.TabIndex = 117;
            // 
            // uscPixelData1
            // 
            this.uscPixelData1.BackColor = System.Drawing.Color.Black;
            this.uscPixelData1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uscPixelData1.Location = new System.Drawing.Point(334, 0);
            this.uscPixelData1.Margin = new System.Windows.Forms.Padding(0);
            this.uscPixelData1.Name = "uscPixelData1";
            this.uscPixelData1.Size = new System.Drawing.Size(240, 26);
            this.uscPixelData1.TabIndex = 117;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(908, 20);
            this.label1.TabIndex = 118;
            this.label1.Text = "Variation Model Settings";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmEditVariation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSlateGray;
            this.ClientSize = new System.Drawing.Size(1115, 703);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "frmEditVariation";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Variation Data Management";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tlpMask2.ResumeLayout(false);
            this.tlpMask2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbDark)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tlpMask2;
        private System.Windows.Forms.TrackBar tbDark;
        private HalconDotNet.HSmartWindowControl hWinOCR;
        private RoundTopLabel roundTopLabel2;
        private RoundBottomLabel lblDark1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private uscPixelData uscPixelData1;
        private System.Windows.Forms.Label label1;
        private RoundTopLabel lblMessage;
        private RoundBottomLabel lblDark;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}