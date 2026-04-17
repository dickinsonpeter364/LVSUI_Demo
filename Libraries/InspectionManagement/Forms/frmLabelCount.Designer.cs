
namespace LVS3
{
    partial class frmLabelCount
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
            this.numLabelCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdCancel = new LVS3.RoundButton();
            this.cmdOK = new LVS3.RoundButton();
            ((System.ComponentModel.ISupportInitialize)(this.numLabelCount)).BeginInit();
            this.SuspendLayout();
            // 
            // numLabelCount
            // 
            this.numLabelCount.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numLabelCount.Font = new System.Drawing.Font("Calibri", 20F);
            this.numLabelCount.Location = new System.Drawing.Point(125, 80);
            this.numLabelCount.Margin = new System.Windows.Forms.Padding(0);
            this.numLabelCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numLabelCount.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numLabelCount.Name = "numLabelCount";
            this.numLabelCount.Size = new System.Drawing.Size(144, 36);
            this.numLabelCount.TabIndex = 1;
            this.numLabelCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numLabelCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LightGray;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 50);
            this.label1.TabIndex = 2;
            this.label1.Text = "Fixed VDE: \r\nPlease enter total number of labels to be inspected.";
            // 
            // cmdCancel
            // 
            this.cmdCancel.BackColor = System.Drawing.Color.Teal;
            this.cmdCancel.FlatAppearance.BorderSize = 0;
            this.cmdCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.cmdCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCancel.Location = new System.Drawing.Point(231, 155);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(112, 51);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = false;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.BackColor = System.Drawing.Color.Teal;
            this.cmdOK.FlatAppearance.BorderSize = 0;
            this.cmdOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.cmdOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdOK.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.Location = new System.Drawing.Point(87, 155);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(112, 51);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = false;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // frmLabelCount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(382, 226);
            this.ControlBox = false;
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numLabelCount);
            this.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.Name = "frmLabelCount";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Label Inspection Count - Fixed VDE ";
            ((System.ComponentModel.ISupportInitialize)(this.numLabelCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numLabelCount;
        private System.Windows.Forms.Label label1;
        private RoundButton cmdOK;
        private RoundButton cmdCancel;
    }
}