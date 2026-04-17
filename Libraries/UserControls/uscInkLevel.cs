using System.Drawing.Drawing2D;
using static LVS3.Delegates;

namespace LVS3
{
    public partial class uscInkLevel : UserControl
    {
        public static SystemMessageHandler SMH;
        public int Head_ID { get => head_id;  }
        private int head_id = 0;
        private string drawText = "";
        private double drawPC = 100;
        ToolTip tt = new ToolTip();
        private int inkLevel = 0;

        public uscInkLevel(int level, int headid)
        {
            InitializeComponent();
            inkLevel = level;
            head_id = headid;
            tt.IsBalloon = false;
            //lblHead.Text = "Head\n# " + headid.ToString();
            lblHead.Text = "# " + headid.ToString();
        }

        private void label3_MouseHover(object sender, EventArgs e)
        {
            Label l = null;
            if (sender is Label)
                l = (Label)sender;
            tt.Show(drawText + Environment.NewLine + drawPC.ToString() + "%", l);
        }

        /// <summary>
        /// Domino ink cartidges are 42ml
        /// </summary>
        /// <param name="ml"></param>
        public void UpdateInkLevel(int ml)
        {
            try
            {
                inkLevel = ml;

                Label p = label3;
                Graphics g = p.CreateGraphics();
                int graphicHeight = (Convert.ToInt32(((double)p.Height / inkLevel) * ml));
                Brush drawBrush;
                Color gradColor = Color.Black;
                Color gradHighColor = Color.Gray;
                drawBrush = new SolidBrush(Color.LightGreen);
                //lblHead.BackColor = Color.White;
                //lblHead.ForeColor = Color.Black;
                if (ml <= 5)
                {
                    gradColor = Color.Red;
                    gradHighColor = Color.White;
                    drawBrush = new SolidBrush(Color.White);
                    //lblHead.BackColor = Color.Red;
                    //lblHead.ForeColor = Color.White;
                }
                else if (ml <= 10)
                {
                    gradColor = Color.OrangeRed;
                    gradHighColor = Color.DarkGray;
                    drawBrush = new SolidBrush(Color.White);
                    lblHead.BackColor = Color.Orange;
                }
                else if (ml < 15)
                {
                    gradHighColor = Color.LightGray;
                    drawBrush = new SolidBrush(Color.White);
                    lblHead.BackColor = Color.Orange;
                }
                RectangleF Rect = new RectangleF(0, 0, p.Width, p.Height);
                RectangleF drawRect = new RectangleF((Rect.Width / 2) - 12, ((p.Height - graphicHeight) + (graphicHeight / 2)) - 10, Rect.Width - ((Rect.Width / 2) - 12 * 2), 20);
                Region r = new Region(Rect);
                g.FillRegion(Brushes.White, r);
                Rect = new RectangleF(0, p.Height - graphicHeight, p.Width, graphicHeight);
                g.Clip = new Region(Rect);
                LinearGradientBrush linGrBrush = new LinearGradientBrush(Rect, gradHighColor, gradColor, LinearGradientMode.Vertical);
                g.FillRectangle(linGrBrush, Rect); //, 0, 155, 500, 30);
                Font drawFont = new Font("Calibri", 8.0F, FontStyle.Regular);
                string drawString = ml.ToString() + " ml";
                //g.DrawString(drawString, drawFont, drawBrush, drawRect);
                this.drawText = drawString;
                this.drawPC = Convert.ToInt32(Math.Round(((double)ml / inkLevel) * 100, 0));
            }
            catch (Exception ex)
            {
                string err = "UpdateInkLevel() err: " + ex.Message;
                 //UtilityFunctions.Notify(err, System.Diagnostics.EventLogEntryType.Error, "Printer Ink Level", true, null);
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Printer Ink Level", (int)Enums.CriticalLevels.Black);
                if (SMH != null)
                    SMH(smea);
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            UpdateInkLevel(inkLevel);
        }

    }
}
