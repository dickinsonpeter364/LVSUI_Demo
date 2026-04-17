namespace LVS3
{
    public interface IReview
    {
        void DISPLAY_REVIEW();
    }

    public delegate void ReviewClickedHandler();
    public delegate void GoToProductionClickedHandler(object sender, EventArgs e);
    public partial class uscExpander : UserControl, IReview
    {
        public event ReviewClickedHandler ReviewOpened;
        public event GoToProductionClickedHandler GoToProductionClicked;
        private ToolTip tt = new ToolTip();
        private bool maskOpen = false;
        private bool reviewOpen = false;

         public bool ReviewOpen
        {
            get { return reviewOpen;  }
        }

        public uscExpander()
        {
            InitializeComponent();
        tt.ToolTipTitle = "Product Setup";
            tt.IsBalloon = true;
            tt.SetToolTip(lblMasking, "Click to add masks");
            tt.SetToolTip(lblReview, "Click to review all settings");
        }

    #region Masking
    private void Masks_MouseDown(object sender, MouseEventArgs e)
    {
        try
        {
                lblTT.Text = "";

                if (e.Button == MouseButtons.Left)
                {
                    RadioButton mask = (RadioButton)sender;
                    Bitmap bmp;

                    int w = 50;
                    int h = 40;
                    bmp = new Bitmap(w, h);
                    Graphics g = Graphics.FromImage(bmp);
                    Brush brshBlack = new SolidBrush(Color.LightCyan);
                    Brush brshWhite = new SolidBrush(Color.White);
                    Pen whitePen = new Pen(Color.White);
                    Pen blackPen = new Pen(Color.Black);
                    Pen XHairsPen = new Pen(Color.Cyan);
                    g.DrawRectangle(blackPen, 1, 1, w - 1, h - 1);
                    g.FillRectangle(brshBlack, 1, 1, w - 1, h - 1);
                    g.DrawRectangle(whitePen, 3, 3, w - 6, h - 6);
                    g.FillRectangle(brshWhite, 3, 3, w - 6, h - 6);
                    g.DrawRectangle(XHairsPen, (w / 2) - 5, 20, 11, 1);
                    g.DrawRectangle(XHairsPen, 25, (h / 2) - 5, 1, 11);

                    bmp.MakeTransparent(Color.White);
                    Cursor cur = new Cursor(bmp.GetHicon());
                    Cursor.Current = cur;
                    DoDragDrop(mask, DragDropEffects.Copy);
                }
            }
            catch (Exception ex)
            {
                string err = "Masks_MouseDown() err: " + ex.Message;
                MessageBox.Show(err, "Mask Definition", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Mask_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                RadioButton rb = (RadioButton)sender;
                lblTT.Text = "Drag over label to mask out an area from investigation";
            }
            catch
            {
                lblTT.Text = "";
            }
        }

        private void ClearText_MouseMove(object sender, MouseEventArgs e)
        {
            lblTT.Text = "";
        }

        private void MaskButton_Click(object sender, EventArgs e)
        {
            TableLayoutRowStyleCollection styles = tlpMask.RowStyles;
            if (maskOpen == false)
            {
                lblMasking.Image = imageList1.Images[0];
                lblReview.Image = imageList1.Images[1];
                styles[2].SizeType = SizeType.Percent;
                styles[3].SizeType = SizeType.Percent;
                styles[4].SizeType = SizeType.Percent;
                styles[2].Height = 100;
                styles[3].Height = 0;
                styles[4].Height = 0;
                maskOpen = true;
                reviewOpen = false;
                if (ReviewOpened != null)
                    ReviewOpened();
            }
            else
            {
                lblMasking.Image = imageList1.Images[1];
                lblReview.Image = imageList1.Images[1];
                styles[2].SizeType = SizeType.Percent;
                styles[3].SizeType = SizeType.Percent;
                styles[4].SizeType = SizeType.Percent;
                styles[2].Height = 0;
                styles[3].Height = 100;
                styles[4].Height = 0;
                reviewOpen = false;
                maskOpen = false;
            }
        }
        #endregion

        #region Review
        private void ReviewButton_Click(object sender, EventArgs e)
        {
            openCloseReviewTab();
        }

        public void openCloseReviewTab()
        {
            TableLayoutRowStyleCollection styles = tlpMask.RowStyles;
            if (reviewOpen == false)
            {
                tlpMask.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    lblMasking.Image = imageList1.Images[1];
                    lblReview.Image = imageList1.Images[0];
                    styles[2].SizeType = SizeType.Percent;
                    styles[3].SizeType = SizeType.Percent;
                    styles[4].SizeType = SizeType.Percent;
                    styles[2].Height = 0;
                    styles[3].Height = 0;
                    styles[4].Height = 100;
                    maskOpen = false;
                    reviewOpen = true;
                    DISPLAY_REVIEW();
                    if (ReviewOpened != null)
                        ReviewOpened();
                });
            }
            else
            {
                tlpMask.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    lblMasking.Image = imageList1.Images[1];
                    lblReview.Image = imageList1.Images[1];
                    styles[2].SizeType = SizeType.Percent;
                    styles[3].SizeType = SizeType.Percent;
                    styles[4].SizeType = SizeType.Percent;
                    styles[2].Height = 0;
                    styles[3].Height = 100;
                    styles[4].Height = 0;
                    reviewOpen = false;
                    maskOpen = false;
                });
            }
        }

        public void ProductionEnabled(bool enabled)
        {
            this.cmdGoToProduction.Enabled = enabled;
        }

        private void DISPLAY_REVIEW()
        {
            string strPHText = "...";


            //lblProductName.Text = LVS3.ProductManager.ProductName;
            //double PWidthAbove = Convert.ToInt32(((ProductSetup.PS_HAE.RightEdgeXAbove - ProductSetup.PS_HAE.LeftEdgeXAbove) / ProductManager.PPMAbove));
            //if (Defaults.MetricSystem == false)
            //    PWidthAbove = Math.Round(PWidthAbove / 25.4, 2);
            //double PWidthBelow = 0;
            //if (ProductManager.DualSideInspection)
            //{
            //    PWidthBelow = Convert.ToInt32(((ProductSetup.PS_HAE.RightEdgeXBelow - ProductSetup.PS_HAE.LeftEdgeXBelow) / ProductManager.PPMBelow));
            //    if (Defaults.MetricSystem == false)
            //        PWidthBelow = Math.Round(PWidthBelow / 25.4, 2);
            //    lblPWidth.Text = string.Format("Above: {0} {1}", PWidthAbove, Defaults.MetricSystem == true ? "mm" : "in");
            //    lblPWidth.Text += '\n' + string.Format("\nBelow: {0} {1}", PWidthBelow, Defaults.MetricSystem == true ? "mm" : "in");
            //}
            //else
            //    lblPWidth.Text = string.Format("Above: {0} {1}", PWidthAbove, Defaults.MetricSystem == true ? "mm" : "in");

            //double HWidth = Convert.ToInt32((ProductSetup.PS_HAE.RightEdgeInnerX - ProductSetup.PS_HAE.LeftEdgeInnerX) / ProductManager.PPMAbove);
            //if (Defaults.MetricSystem == false)
            //    HWidth = Math.Round(HWidth / 25.4, 2);
            //lblHWidth.Text = string.Format("{0} {1}", HWidth, Defaults.MetricSystem == true ? "mm" : "in");
            //lblPHData.Text = strPHText;
            //lblMRData.Text = ProductSetup.MaskDefinition.buildAcceptArgs();
        }

        void IReview.DISPLAY_REVIEW()
        {
            DISPLAY_REVIEW();
        }

        public void CloseUp()
        {
            TableLayoutRowStyleCollection styles = tlpMask.RowStyles;
            lblMasking.Image = imageList1.Images[1];
            lblReview.Image = imageList1.Images[1];
            styles[2].SizeType = SizeType.Percent;
            styles[3].SizeType = SizeType.Percent;
            styles[4].SizeType = SizeType.Percent;
            styles[2].Height = 0;
            styles[3].Height = 100;
            styles[4].Height = 0;
            reviewOpen = false;
            maskOpen = false;
        }

        #endregion

        private void cmdGoToProduction_Click(object sender, EventArgs e)
        {
            if (GoToProductionClicked != null)
                GoToProductionClicked(sender, e);
        }
    }
}
