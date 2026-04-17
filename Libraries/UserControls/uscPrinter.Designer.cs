/* using MVA_CORE;

namespace LVS3
{
    partial class uscPrinter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscPrinter));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.roundLeftLabel1 = new System.Windows.Forms.Label();
            this.roundLeftLabel3 = new MVA_CORE.RoundLeftBottomLabel();
            this.lblIP = new System.Windows.Forms.Label();
            this.lblStatus = new MVA_CORE.RoundRightBottomLabel();
            this.tlpInkPots = new System.Windows.Forms.TableLayoutPanel();
            this.lblComponentName = new MVA_CORE.RoundTopLabel();
            this.lblPadding = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.22034F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblComponentName, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44.92188F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(258, 169);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.51485F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.95411F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.53104F));
            this.tableLayoutPanel2.Controls.Add(this.roundLeftLabel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.roundLeftLabel3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblIP, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblStatus, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.tlpInkPots, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblPadding, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 47);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(258, 122);
            this.tableLayoutPanel2.TabIndex = 308;
            // 
            // roundLeftLabel1
            // 
            this.roundLeftLabel1.AutoSize = true;
            this.roundLeftLabel1.BackColor = System.Drawing.SystemColors.Control;
            this.roundLeftLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundLeftLabel1.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundLeftLabel1.Location = new System.Drawing.Point(2, 0);
            this.roundLeftLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 0, 1);
            this.roundLeftLabel1.Name = "roundLeftLabel1";
            this.roundLeftLabel1.Size = new System.Drawing.Size(61, 23);
            this.roundLeftLabel1.TabIndex = 0;
            this.roundLeftLabel1.Text = "Comms";
            this.roundLeftLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // roundLeftLabel3
            // 
            this.roundLeftLabel3.AutoSize = true;
            this.roundLeftLabel3.BackColor = System.Drawing.SystemColors.Control;
            this.roundLeftLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundLeftLabel3.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundLeftLabel3.IsLink = false;
            this.roundLeftLabel3.Location = new System.Drawing.Point(2, 24);
            this.roundLeftLabel3.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.roundLeftLabel3.Name = "roundLeftLabel3";
            this.roundLeftLabel3.Size = new System.Drawing.Size(61, 94);
            this.roundLeftLabel3.TabIndex = 2;
            this.roundLeftLabel3.Text = "Status";
            this.roundLeftLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.BackColor = System.Drawing.Color.White;
            this.lblIP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblIP.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIP.Location = new System.Drawing.Point(104, 0);
            this.lblIP.Margin = new System.Windows.Forms.Padding(0, 0, 2, 1);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(152, 23);
            this.lblIP.TabIndex = 3;
            this.lblIP.Text = "177.168.8.417";
            this.lblIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.Color.White;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.IsLink = false;
            this.lblStatus.Location = new System.Drawing.Point(104, 24);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(152, 94);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpInkPots
            // 
            this.tlpInkPots.BackColor = System.Drawing.Color.White;
            this.tlpInkPots.ColumnCount = 1;
            this.tlpInkPots.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpInkPots.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInkPots.Location = new System.Drawing.Point(63, 24);
            this.tlpInkPots.Margin = new System.Windows.Forms.Padding(0);
            this.tlpInkPots.Name = "tlpInkPots";
            this.tlpInkPots.RowCount = 1;
            this.tlpInkPots.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpInkPots.Size = new System.Drawing.Size(41, 94);
            this.tlpInkPots.TabIndex = 6;
            // 
            // lblComponentName
            // 
            this.lblComponentName.AutoSize = true;
            this.lblComponentName.BackColor = System.Drawing.Color.Gainsboro;
            this.lblComponentName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblComponentName.Enabled = false;
            this.lblComponentName.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComponentName.Image = ((System.Drawing.Image)(resources.GetObject("lblComponentName.Image")));
            this.lblComponentName.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblComponentName.IsLink = false;
            this.lblComponentName.Location = new System.Drawing.Point(2, 1);
            this.lblComponentName.Margin = new System.Windows.Forms.Padding(2, 1, 2, 0);
            this.lblComponentName.Name = "lblComponentName";
            this.lblComponentName.Size = new System.Drawing.Size(254, 46);
            this.lblComponentName.TabIndex = 305;
            this.lblComponentName.Text = "G-Series.Printer 1";
            this.lblComponentName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPadding
            // 
            this.lblPadding.AutoSize = true;
            this.lblPadding.BackColor = System.Drawing.Color.White;
            this.lblPadding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPadding.Location = new System.Drawing.Point(63, 0);
            this.lblPadding.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.lblPadding.Name = "lblPadding";
            this.lblPadding.Size = new System.Drawing.Size(41, 23);
            this.lblPadding.TabIndex = 7;
            // 
            // uscPrinter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSlateGray;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "uscPrinter";
            this.Size = new System.Drawing.Size(258, 169);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private RoundTopLabel lblComponentName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label roundLeftLabel1;
        private RoundLeftBottomLabel roundLeftLabel3;
        public System.Windows.Forms.Label lblIP;
        public RoundRightBottomLabel lblStatus;
        private System.Windows.Forms.TableLayoutPanel tlpInkPots;
        private System.Windows.Forms.Label lblPadding;
    }
}
*/