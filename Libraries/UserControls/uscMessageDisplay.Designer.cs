
namespace LVS3
{
    partial class uscMessageDisplay
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
            this.tmrBoldToNormal = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lvMsg = new System.Windows.Forms.ListView();
            this.colMSG = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSubject = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDT = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmrBoldToNormal
            // 
            this.tmrBoldToNormal.Interval = 5000;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lvMsg, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(623, 175);
            this.tableLayoutPanel1.TabIndex = 322;
            // 
            // lvMsg
            // 
            this.lvMsg.BackColor = System.Drawing.Color.White;
            this.lvMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvMsg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colMSG,
            this.colSubject,
            this.colDT});
            this.lvMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMsg.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvMsg.FullRowSelect = true;
            this.lvMsg.GridLines = true;
            this.lvMsg.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvMsg.HideSelection = false;
            this.lvMsg.Location = new System.Drawing.Point(1, 1);
            this.lvMsg.Margin = new System.Windows.Forms.Padding(1);
            this.lvMsg.MultiSelect = false;
            this.lvMsg.Name = "lvMsg";
            this.lvMsg.ShowGroups = false;
            this.lvMsg.Size = new System.Drawing.Size(621, 173);
            this.lvMsg.TabIndex = 322;
            this.lvMsg.UseCompatibleStateImageBehavior = false;
            this.lvMsg.View = System.Windows.Forms.View.Details;
            this.lvMsg.DoubleClick += new System.EventHandler(this.lvMsg_DoubleClick);
            this.lvMsg.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvSysMessages_MouseUp);
            // 
            // colMSG
            // 
            this.colMSG.Text = "System Messages";
            this.colMSG.Width = 800;
            // 
            // colSubject
            // 
            this.colSubject.Text = "Subject";
            this.colSubject.Width = 180;
            // 
            // colDT
            // 
            this.colDT.Text = "Date Time:";
            this.colDT.Width = 180;
            // 
            // uscMessageDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "uscMessageDisplay";
            this.Size = new System.Drawing.Size(623, 175);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmrBoldToNormal;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        internal System.Windows.Forms.ListView lvMsg;
        internal System.Windows.Forms.ColumnHeader colMSG;
        private System.Windows.Forms.ColumnHeader colSubject;
        private System.Windows.Forms.ColumnHeader colDT;
    }
}
