using CONSTANTS;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LVS3.Enums;

namespace LVS3
{
    public partial class frmEditVariation : Form
    {
        private List<FontSizesSegment> FontsizesSegmentation = new List<FontSizesSegment>();

        private LabelType labelType = LabelType.FLATPANEL;
        private InspectionParamDefaults iParamDefaults = new InspectionParamDefaults();
        public int DarkMaxGray = 100;
        bool INSPECT_LIGHT_AREAS = false;
        public bool EDITING_LABEL_VARIATION = false;
        public bool CREATE_VARIATION_LABEL = false;
        public bool CANCELLED = false;
        private HObject zoneImage = null;
        //private MSERParams MSER = null;
        private int oldHigh = 0;
        double innerRadius = 4;

        public frmEditVariation(HObject img, bool labelvariation, bool inspectlightareas, double innerradius, int darkmaxgray, InspectionParamDefaults iparams, LabelType labeltype)
        {
            InitializeComponent();
            DarkMaxGray = darkmaxgray;
            iParamDefaults = iparams;
            labelType = labeltype;
            innerRadius = innerradius;
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 100;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            zoneImage = img;
            tbDark.Maximum = 180; // DataManager.get iparams. mser.DarkMaxGrayLimit;
            tbDark.Value = darkmaxgray; // Convert.ToInt32(mser.DarkMaxGray);
            DarkMaxGray = darkmaxgray;
            oldHigh = DarkMaxGray;
            lblDark.Text = tbDark.Value.ToString();
            EDITING_LABEL_VARIATION = labelvariation;
            INSPECT_LIGHT_AREAS = inspectlightareas;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Up)
            {
                if (tbDark.Value < tbDark.Maximum)
                    tbDark.Value += 1;
                e.Handled = true;
                DebrisCheck(ref zoneImage, ref zoneImage, INSPECT_LIGHT_AREAS);
                return;
            }
            if (e.KeyValue == (char)Keys.Down)
            {
                if (tbDark.Value > tbDark.Minimum)
                    tbDark.Value -= 1;
                e.Handled = true;
                DebrisCheck(ref zoneImage, ref zoneImage, INSPECT_LIGHT_AREAS);
                return;
            }
            e.Handled = true;
        }

        private bool DebrisCheck(ref HObject maskedimage, ref HObject labelimage, bool inspectlightareas)
        {
            bool retVal = true;
            HObject imgSobel = null, rgnThreshold = null, rgnSelectedDark = null, rgnSelectedLight = null, rgnUnion = null, rgnDilation = null, rgnImage=null;
            HObject rgnFillup = null, imgReduced = null, rgnDarkObjects = null, rgnLightObjects = null, rgnConnected = null, rgnSelected = null, rgnEroded = null;
            HTuple mean, numDarkObjects, numLightObjects = 0, sobelAmp = 0, edgeThreshold = 0, minSize = 50;
            try
            {
                HOperatorSet.SmallestRectangle1(maskedimage, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                HOperatorSet.GenRectangle1(out rgnImage, r1, c1, r2, c2);
                HOperatorSet.Intensity(rgnImage, maskedimage, out mean, out HTuple deviation);

                //Find Marks
                if (labelType == LabelType.FLATPANEL)
                {
                    sobelAmp = iParamDefaults.SobelAmpSize1;
                    edgeThreshold = tbDark.Value; //  iParamDefaults.SobelEdge1;
                }
                else if (labelType == LabelType.DARK)
                {
                    sobelAmp = iParamDefaults.SobelAmpSize2;
                    edgeThreshold = tbDark.Value; // iParamDefaults.SobelEdge2;
                }
                else if (labelType == LabelType.BOOKLET)
                {
                    sobelAmp = iParamDefaults.SobelAmpSize3;
                    edgeThreshold = tbDark.Value; // iParamDefaults.SobelEdge3;
                }
                HOperatorSet.SobelAmp(maskedimage, out imgSobel, "sum_abs", sobelAmp);
                HOperatorSet.Threshold(imgSobel, out rgnThreshold, edgeThreshold, 255);
                HOperatorSet.Connection(rgnThreshold, out rgnConnected);

                //Elininate Squid
                //HOperatorSet.SelectShape(rgnConnected, out rgnSelected, "area", "and", 10, 999999);
                HOperatorSet.Union1(rgnConnected, out rgnUnion);

                //Merge fragments
                HOperatorSet.DilationCircle(rgnUnion, out rgnDilation, 3.5);
                HOperatorSet.FillUp(rgnDilation, out rgnFillup);
                HOperatorSet.ErosionCircle(rgnFillup, out rgnEroded, 2.5);
                HOperatorSet.ReduceDomain(maskedimage, rgnEroded, out imgReduced);
                HOperatorSet.Threshold(imgReduced, out rgnDarkObjects, 1, DarkMaxGray); // tbDark.Value);
                HOperatorSet.CountObj(rgnDarkObjects, out numDarkObjects);
                hWinOCR.HalconWindow.ClearWindow();
                if (numDarkObjects > 0)
                {
                    HOperatorSet.SetColor(hWinOCR.HalconWindow, "red");
                    hWinOCR.HalconWindow.DispObj(rgnDarkObjects);
                }

                //HOperatorSet.Threshold(imgReduced, out rgnDarkObjects, 0, mean * iParamDefaults.MeanOffset);

                if (inspectlightareas == true)
                {
                    if (imgReduced != null) imgReduced.Dispose();
                    if (rgnEroded != null) rgnEroded.Dispose();
                    HOperatorSet.ErosionCircle(rgnFillup, out rgnEroded, 3.5);
                    HOperatorSet.ReduceDomain(maskedimage, rgnEroded, out imgReduced);
                    HOperatorSet.Threshold(imgReduced, out rgnLightObjects, mean + (mean * 0.1), 255);
                    if (rgnConnected != null)
                        rgnConnected.Dispose();
                    if (rgnSelected != null)
                        rgnSelected.Dispose();
                    HOperatorSet.Connection(rgnLightObjects, out rgnConnected);
                    HOperatorSet.SelectShape(rgnConnected, out rgnSelectedLight, "area", "and", minSize, 999999);
                    HOperatorSet.CountObj(rgnSelectedLight, out numLightObjects);
                }

                //if (rgnConnected != null)
                //    rgnConnected.Dispose();
                //if (rgnSelected != null)
                //    rgnSelected.Dispose();
                //HOperatorSet.Connection(rgnDarkObjects, out rgnConnected);
                //HOperatorSet.SelectShape(rgnConnected, out rgnConnected, "inner_radius", "and", 2.5, 55.5);
                //HOperatorSet.SelectShape(rgnConnected, out rgnSelectedDark, "area", "and", minSize, 999999);
                //HOperatorSet.CountObj(rgnSelectedDark, out numDarkObjects);

                
                //if (numDarkObjects > 0)
                //{
                //    HOperatorSet.SetColor(hWinOCR.HalconWindow, "red");
                //    hWinOCR.HalconWindow.DispObj(rgnSelectedDark);
                //}
                if (numLightObjects > 0 && INSPECT_LIGHT_AREAS)
                {
                    HOperatorSet.SetColor(hWinOCR.HalconWindow, "cyan");
                    hWinOCR.HalconWindow.DispObj(rgnSelectedLight);
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                string err = "DebrisCheck() err: " + ex.Message;
            }
            finally
            {
                if (rgnImage != null) rgnImage.Dispose();
                if (rgnEroded != null) rgnEroded.Dispose();
                if (imgSobel != null) imgSobel.Dispose();
                if (rgnThreshold != null) rgnThreshold.Dispose();
                if (rgnSelectedDark != null) rgnSelectedDark.Dispose();
                if (rgnSelectedLight != null) rgnSelectedLight.Dispose();
                if (rgnUnion != null) rgnUnion.Dispose();
                if (rgnDilation != null) rgnDilation.Dispose();
                if (rgnFillup != null) rgnFillup.Dispose();
                if (imgReduced != null) imgReduced.Dispose();
                if (rgnDarkObjects != null) rgnDarkObjects.Dispose();
                if (rgnLightObjects != null) rgnLightObjects.Dispose();
                if (rgnConnected != null) rgnConnected.Dispose();
                if (rgnSelected != null) rgnSelected.Dispose();
            }
            return retVal;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                doSave();
                e.Handled = true;
                return;
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                doCancel();
                e.Handled = true;
                return;
            }
            //if (e.KeyChar == (char)Keys.Up)
            //{
            //    if (tbDark.Value < tbDark.Maximum)
            //        tbDark.Value += 1;
            //    e.Handled = true;
            //    visualizeMSERValues();
            //    return;
            //}
            //if (e.KeyChar == (char)Keys.Down)
            //{
            //    if (tbDark.Value > tbDark.Minimum)
            //        tbDark.Value -= 1;
            //    e.Handled = true;
            //    visualizeMSERValues();
            //    return;
            //}
            e.Handled = false;
        }

        protected override void OnShown(EventArgs e)
        {
            tbDark.Enabled = true;
            cropAndDisplayZone();
            DebrisCheck(ref zoneImage, ref zoneImage, INSPECT_LIGHT_AREAS);
            hWinOCR.SetFullImagePart();
            base.OnShown(e);
        }

        public void cropAndDisplayZone()
        {
            HObject tmpImg = null;
            try
            {
                HOperatorSet.SmallestRectangle1(zoneImage, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                HOperatorSet.AttachBackgroundToWindow(zoneImage, hWinOCR.HalconWindow);
                hWinOCR.SetFullImagePart(); 
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string err = "cropAndDisplayZone() err: " + ex.Message;
                    displayInfo(err, "top");
                    //MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
            finally
            {
                if (tmpImg != null)
                    tmpImg.Dispose();
            }
        }

        private void hwinImage_Load(object sender, EventArgs e)
        {
            this.MouseWheel -= h_MouseWheel;
            this.MouseWheel += h_MouseWheel;
        }

        private void h_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - hWinOCR.Location.X, e.Y - hWinOCR.Location.Y, e.Delta);
                hWinOCR.HSmartWindowControl_MouseWheel(sender, newe);
            }
            catch { }
        }

        private void sliderValueChangeDark(object sender, EventArgs e)
        {
            string err = "";
            displayInfo(err, "top");

            if (tbDark.Enabled == false)
                return;

            TrackBar t = null;
            if (sender is TrackBar)
            {
                t = (TrackBar)sender;
                string barName = t.Name;
                switch (barName)
                {
                    case "tbDark":
                        lblDark.Text = tbDark.Value.ToString();
                        DarkMaxGray = tbDark.Value;
                        break;
                    case "tbBright":
                        break;
                }
                DebrisCheck(ref zoneImage, ref zoneImage, INSPECT_LIGHT_AREAS);
            }
        }

        private void displayInfo(string msg, string position)
        {
            try
            {
                if (lblMessage.InvokeRequired)
                {
                    lblMessage.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        lblMessage.Text = msg;
                        lblMessage.Refresh();
                    });
                }
                else
                {
                    lblMessage.Text = msg;
                    lblMessage.Refresh();
                }
            }
            catch (Exception ex)
            {
                string err = "displayInfo() err: " + ex.Message;
                displayInfo(err, "top");
                //MessageBox.Show(err, "Label Training", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private void doSave()
        {
            if (EDITING_LABEL_VARIATION)
                CREATE_VARIATION_LABEL = true;
            DarkMaxGray = tbDark.Value;
            this.Hide();
        }

        private void doCancel()
        {
            CANCELLED = true;
            DarkMaxGray = oldHigh;
            this.Hide();
        }

        private void lblDark_Click(object sender, EventArgs e)
        {
            tbDark.Focus();
        }

        private void cmdKeepAspect_Clicked(object sender, EventArgs e)
        {
            if (hWinOCR.HKeepAspectRatio == false)
            {
                hWinOCR.HKeepAspectRatio = true;
                hWinOCR.SetFullImagePart();
                hWinOCR.Refresh();
            }
        }

        private void cmdFillScreen_Clicked(object sender, EventArgs e)
        {
            if (hWinOCR.HKeepAspectRatio == true)
            {
                hWinOCR.HKeepAspectRatio = false;
                HOperatorSet.SmallestRectangle1(zoneImage, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                HOperatorSet.AttachBackgroundToWindow(zoneImage, hWinOCR.HalconWindow);
                hWinOCR.HalconWindow.SetPart(0, 0, Convert.ToInt32(r2.D), Convert.ToInt32(c2.D));
                hWinOCR.Refresh();
            }
        }


        private void hWinOCR_HMouseMove(object sender, HMouseEventArgs e)
        {
            try
            {
                double x = e.X;
                double y = e.Y;
                HTuple greyVal = 0;
                try { HalconDotNet.HOperatorSet.GetGrayval(zoneImage, e.Y, e.X, out greyVal); } catch { }
                if (greyVal != null)
                    try { uscPixelData1.pointData(Convert.ToInt32(e.X), Convert.ToInt32(e.Y), Convert.ToInt32(greyVal.D)); } catch { }
                else
                    try { uscPixelData1.pointData(Convert.ToInt32(e.X), Convert.ToInt32(e.Y), -1); } catch { }
            }
            catch { }
        }

        private void WinControls_MouseEnter(object sender, EventArgs e)
        {
            if (sender is HSmartWindowControl)
            {
                tbDark.BackColor = Color.LightGray;
                return;
            }
            if (sender is TrackBar)
            {
                tbDark.BackColor = Color.White;
                tbDark.Focus();
            }
            if (sender is TableLayoutPanel)
            {
                tbDark.BackColor = Color.White;
                tbDark.Focus();
            }
        }

        private void WinControls_MouseLeave(object sender, EventArgs e)
        {
            if (sender is HSmartWindowControl)
            {
                tbDark.BackColor = Color.White;
                tbDark.Focus();
            }
            if (sender is TableLayoutPanel)
            {
                tbDark.BackColor = Color.White;
                tbDark.Focus();
            }
        }

        private void tbDark_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = null;
            tb = (TrackBar)sender;
            toolTip1.SetToolTip(tb, tb.Value.ToString());
            DarkMaxGray = (int)tb.Value;
        }
    }
}