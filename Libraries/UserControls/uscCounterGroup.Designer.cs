
namespace LVS3
{
    partial class uscCounterGroup
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
            this.tlpCounters = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tlpCounters
            // 
            this.tlpCounters.BackColor = System.Drawing.Color.Transparent;
            this.tlpCounters.ColumnCount = 1;
            this.tlpCounters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCounters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCounters.Location = new System.Drawing.Point(0, 0);
            this.tlpCounters.Margin = new System.Windows.Forms.Padding(0);
            this.tlpCounters.Name = "tlpCounters";
            this.tlpCounters.Padding = new System.Windows.Forms.Padding(1);
            this.tlpCounters.RowCount = 1;
            this.tlpCounters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCounters.Size = new System.Drawing.Size(150, 43);
            this.tlpCounters.TabIndex = 0;
            // 
            // uscCounterGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tlpCounters);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "uscCounterGroup";
            this.Size = new System.Drawing.Size(150, 43);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpCounters;
    }
}
