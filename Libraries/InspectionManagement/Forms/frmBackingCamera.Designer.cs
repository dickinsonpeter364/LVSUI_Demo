
namespace LVS3
{
    partial class frmBackingCamera
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
            this.tlpBackingCam = new System.Windows.Forms.TableLayoutPanel();
            this.scAria = new System.Windows.Forms.SplitContainer();
            this.tlpBackingCam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scAria)).BeginInit();
            this.scAria.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpBackingCam
            // 
            this.tlpBackingCam.BackColor = System.Drawing.Color.Gainsboro;
            this.tlpBackingCam.ColumnCount = 1;
            this.tlpBackingCam.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBackingCam.Controls.Add(this.scAria, 0, 0);
            this.tlpBackingCam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBackingCam.Location = new System.Drawing.Point(0, 0);
            this.tlpBackingCam.Margin = new System.Windows.Forms.Padding(0);
            this.tlpBackingCam.Name = "tlpBackingCam";
            this.tlpBackingCam.RowCount = 1;
            this.tlpBackingCam.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBackingCam.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBackingCam.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBackingCam.Size = new System.Drawing.Size(288, 736);
            this.tlpBackingCam.TabIndex = 349;
            // 
            // scAria
            // 
            this.scAria.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.scAria.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scAria.Location = new System.Drawing.Point(0, 0);
            this.scAria.Margin = new System.Windows.Forms.Padding(0);
            this.scAria.Name = "scAria";
            // 
            // scAria.Panel1
            // 
            this.scAria.Panel1.BackColor = System.Drawing.Color.Silver;
            this.scAria.Panel2Collapsed = true;
            this.scAria.Panel2MinSize = 0;
            this.scAria.Size = new System.Drawing.Size(288, 736);
            this.scAria.SplitterDistance = 25;
            this.scAria.TabIndex = 344;
            // 
            // frmBackingCamera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(288, 736);
            this.ControlBox = false;
            this.Controls.Add(this.tlpBackingCam);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmBackingCamera";
            this.Text = "Backing Camera";
            this.tlpBackingCam.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scAria)).EndInit();
            this.scAria.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpBackingCam;
        public System.Windows.Forms.SplitContainer scAria;
    }
}