
namespace LVS3
{
    partial class frmLabelType
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
            this.tTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.tlpOptions = new System.Windows.Forms.TableLayoutPanel();
            this.optType2 = new System.Windows.Forms.RadioButton();
            this.optType1 = new System.Windows.Forms.RadioButton();
            this.optType0 = new System.Windows.Forms.RadioButton();
            this.cmdOK = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tlpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkGray;
            this.panel1.Controls.Add(this.tlpOptions);
            this.panel1.Controls.Add(this.cmdOK);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(454, 297);
            this.panel1.TabIndex = 0;
            // 
            // tlpOptions
            // 
            this.tlpOptions.BackColor = System.Drawing.Color.DarkGray;
            this.tlpOptions.ColumnCount = 1;
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpOptions.Controls.Add(this.optType2, 0, 2);
            this.tlpOptions.Controls.Add(this.optType1, 0, 1);
            this.tlpOptions.Controls.Add(this.optType0, 0, 0);
            this.tlpOptions.Location = new System.Drawing.Point(25, 56);
            this.tlpOptions.Margin = new System.Windows.Forms.Padding(0);
            this.tlpOptions.Name = "tlpOptions";
            this.tlpOptions.RowCount = 3;
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpOptions.Size = new System.Drawing.Size(255, 192);
            this.tlpOptions.TabIndex = 6;
            // 
            // optType2
            // 
            this.optType2.Appearance = System.Windows.Forms.Appearance.Button;
            this.optType2.AutoSize = true;
            this.optType2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.optType2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optType2.FlatAppearance.CheckedBackColor = System.Drawing.Color.RoyalBlue;
            this.optType2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SkyBlue;
            this.optType2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SkyBlue;
            this.optType2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optType2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optType2.Location = new System.Drawing.Point(3, 131);
            this.optType2.Name = "optType2";
            this.optType2.Size = new System.Drawing.Size(249, 58);
            this.optType2.TabIndex = 6;
            this.optType2.TabStop = true;
            this.optType2.Tag = "";
            this.optType2.Text = "Booklet";
            this.optType2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optType2.UseVisualStyleBackColor = false;
            this.optType2.Click += new System.EventHandler(this.optClicked_Click);
            // 
            // optType1
            // 
            this.optType1.Appearance = System.Windows.Forms.Appearance.Button;
            this.optType1.AutoSize = true;
            this.optType1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.optType1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optType1.FlatAppearance.CheckedBackColor = System.Drawing.Color.RoyalBlue;
            this.optType1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SkyBlue;
            this.optType1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SkyBlue;
            this.optType1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optType1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optType1.Location = new System.Drawing.Point(3, 67);
            this.optType1.Name = "optType1";
            this.optType1.Size = new System.Drawing.Size(249, 58);
            this.optType1.TabIndex = 5;
            this.optType1.TabStop = true;
            this.optType1.Text = "Dark";
            this.optType1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optType1.UseVisualStyleBackColor = false;
            this.optType1.Click += new System.EventHandler(this.optClicked_Click);
            // 
            // optType0
            // 
            this.optType0.Appearance = System.Windows.Forms.Appearance.Button;
            this.optType0.AutoSize = true;
            this.optType0.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.optType0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optType0.FlatAppearance.CheckedBackColor = System.Drawing.Color.RoyalBlue;
            this.optType0.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SkyBlue;
            this.optType0.FlatAppearance.MouseOverBackColor = System.Drawing.Color.SkyBlue;
            this.optType0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optType0.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optType0.Location = new System.Drawing.Point(3, 3);
            this.optType0.Name = "optType0";
            this.optType0.Size = new System.Drawing.Size(249, 58);
            this.optType0.TabIndex = 4;
            this.optType0.TabStop = true;
            this.optType0.Text = "Flat Panel";
            this.optType0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optType0.UseVisualStyleBackColor = false;
            this.optType0.Click += new System.EventHandler(this.optClicked_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Enabled = false;
            this.cmdOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.cmdOK.Location = new System.Drawing.Point(300, 59);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(0);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(128, 186);
            this.cmdOK.TabIndex = 5;
            this.cmdOK.TabStop = false;
            this.cmdOK.Text = "Continue...";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // frmLabelType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkBlue;
            this.ClientSize = new System.Drawing.Size(478, 321);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmLabelType";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Label Type";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.tlpOptions.ResumeLayout(false);
            this.tlpOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip tTip;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tlpOptions;
        private System.Windows.Forms.RadioButton optType2;
        private System.Windows.Forms.RadioButton optType1;
        private System.Windows.Forms.RadioButton optType0;
        private System.Windows.Forms.Button cmdOK;
    }
}