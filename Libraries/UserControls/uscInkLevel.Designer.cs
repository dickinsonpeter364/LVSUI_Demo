namespace LVS3
{
    partial class uscInkLevel
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new RoundBottomLabel();
            this.lblHead = new RoundTopLabel();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblHead, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(51, 116);
            this.tableLayoutPanel2.TabIndex = 309;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.IsLink = false;
            this.label3.Location = new System.Drawing.Point(2, 23);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 8);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.label3.Size = new System.Drawing.Size(47, 85);
            this.label3.TabIndex = 7;
            this.label3.MouseHover += new System.EventHandler(this.label3_MouseHover);
            // 
            // lblHead
            // 
            this.lblHead.AutoSize = true;
            this.lblHead.BackColor = System.Drawing.Color.White;
            this.lblHead.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHead.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHead.IsLink = false;
            this.lblHead.Location = new System.Drawing.Point(2, 2);
            this.lblHead.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.lblHead.Name = "lblHead";
            this.lblHead.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblHead.Size = new System.Drawing.Size(47, 21);
            this.lblHead.TabIndex = 6;
            this.lblHead.Text = "###";
            this.lblHead.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uscInkLevel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "uscInkLevel";
            this.Size = new System.Drawing.Size(51, 116);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private RoundBottomLabel label3;
        private RoundTopLabel lblHead;
    }
}
