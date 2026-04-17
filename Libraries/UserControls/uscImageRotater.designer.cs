namespace LVS3
{
    partial class uscImageRotater
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uscImageRotater));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cmdRotate = new LVS3.RoundButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.cmdRotate, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(114, 76);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cmdRotate
            // 
            this.cmdRotate.BackColor = System.Drawing.Color.LightGray;
            this.cmdRotate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdRotate.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.cmdRotate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdRotate.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdRotate.Image = ((System.Drawing.Image)(resources.GetObject("cmdRotate.Image")));
            this.cmdRotate.Location = new System.Drawing.Point(1, 1);
            this.cmdRotate.Margin = new System.Windows.Forms.Padding(1);
            this.cmdRotate.Name = "cmdRotate";
            this.cmdRotate.Size = new System.Drawing.Size(112, 74);
            this.cmdRotate.TabIndex = 1;
            this.cmdRotate.Text = "Rotate\r\n\r\n\r\nLabel";
            this.toolTip1.SetToolTip(this.cmdRotate, "Click to rotate image and auto-detect VDE data");
            this.cmdRotate.UseVisualStyleBackColor = false;
            this.cmdRotate.Click += new System.EventHandler(this.button_value_changed);
            // 
            // uscImageRotater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "uscImageRotater";
            this.Size = new System.Drawing.Size(114, 76);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private RoundButton cmdRotate;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
