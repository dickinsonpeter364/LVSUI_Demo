using CONSTANTS;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LVS3.CameraManager;
using static LVS3.Enums;

namespace LVS3
{
    public partial class  frmLabelConfiguration : Form
    {
        #region Form Variables

        private double ConfidenceLevel = DataManager.GetConfidenceLevel();
        public bool IsSmallDebrisSizeVar = true;
        //private int DarkSegmentContrast = 0;
        private InspectionParamDefaults iParamDefaults;
        private InspectionParams iParams;
        
        public LabelType LabelTYPE = LabelType.BOOKLET;
        //public int FILTER = 1;
        public int DARK_MAX_GRAY = 100;
        private List<LabelItemDataSetup> LIDs = null;
        private double InnerRadius = 0;
        private int imageIndex = 1;
        private List<string> testErrors = new List<string>();
        public bool INSPECT_LIGHT_AREAS = false;
        private HObject VariationROI = null;
        private HObject VariationImages = null;
        private HObject TrainingImages = null;
        private HObject TestImages = null;
        private HObject Backgroundimage = null;
        private HObject mouseOverImage = null;
        private List<FontSizesSegment> listFSS = new List<FontSizesSegment>();
        private LabelFixture labelFixture = new LabelFixture();
        private HTuple oldLineStyle = null;

        private bool LOADING_FORM = true;
        private int topStart = 0;
        private int leftStart = 0;
        private int bottomStart = 0;
        private int rightStart = 0;
        private DateTime startedAt = new DateTime();
        private int variationImageCount = Defaults.VAMImageCount;
        private int testImageCount = Defaults.TestImageCount;
        private int setupImagesCaptured = 0;
        private static bool formClosing = false;
        private bool ADDING_MASK = false;
        private bool ADDING_ROT_ZONE = false;
        private bool PLC_IS_REWINDING = false;
        public bool CANCELLED = false;
        protected internal CameraNecta camera;
        private bool WAITING_FOR_START = true;
        private bool ADDING_VDE = false;
        private bool ADDING_OPZONES = false;
        private bool WAITING_FOR_IMAGE_ONE = false;
        private bool EDITING_VARIATION = false;
        private bool VARIATION_COMPLETE = false;
        private double lastX = 0;
        private double lastY = 0;
        private int RotatedAngle = 90;
        public string LABEL_ITEM = "";
        public string REEL_LPN = "";
        public bool bLabelSetupComplete = false;
        public string TRG_FOLDER = "";
        public string FONT_FOLDER = "";
        public string TMP_FOLDER = "";
        private static uscMessageDisplay uscMD = null;

        ///// this to disable the control-box Close icon //////////////
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        ///////////////////////////////////////////////////////////////
        #endregion

        public frmLabelConfiguration(LabelItemAndVersion rd, uscMessageDisplay uscmd)
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.LABEL_ITEM = rd.LIN;
            lblLIN.Text = LABEL_ITEM;
            lblReel.Text = rd.REEL;
            REEL_LPN = rd.REEL;
            lblLWO.Text = rd.LWO;
            loadVDEToolsAndData(REEL_LPN);
            bLabelSetupComplete = false;
            HOperatorSet.GenEmptyObj(out VariationImages);
            uscMD = uscmd;
            InnerRadius = DataManager.GetInnerRadiusDefaultSmall();

            if (CameraManager.CamerasLoaded == false)
            {
                MessageBox.Show("Inspection camera not present", "Inspect Label", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UtilityFunctions.DoApplicationShutdown();
                return;
            }
            else if (CameraManager.NectaCameras == null)
            {
                MessageBox.Show("Inspection camera not present", "Inspect Label", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UtilityFunctions.DoApplicationShutdown();
                return;
            }
            else if (CameraManager.NectaCameras.Length == 0)
            {
                MessageBox.Show("Inspection camera not present", "Inspect Label", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UtilityFunctions.DoApplicationShutdown();
                return;
            }
            else if (CameraManager.NectaCameras[0] == null)
            {
                MessageBox.Show("Inspection camera not present", "Inspect Label", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UtilityFunctions.DoApplicationShutdown();
                return;
            }
            this.camera = CameraManager.NectaCameras[0];
        }


        protected override void OnShown(EventArgs e)
        {
            WAITING_FOR_IMAGE_ONE = true;
            ADDING_OPZONES = false;
            EDITING_VARIATION = true;
            VARIATION_COMPLETE = false;
            VARIATION_COMPLETE = false;
            CANCELLED = false;
            PLC_IS_REWINDING = false;

            TRG_FOLDER = ImageData.CreateTrainingFilepath(REEL_LPN);
            TMP_FOLDER = ImageData.CreateTempFilepath(REEL_LPN);
            ImageData.DeleteTrainingData(TMP_FOLDER);
            TMP_FOLDER = ImageData.CreateTempFilepath(REEL_LPN);
            ImageData.DeleteTrainingData(TRG_FOLDER);
            TRG_FOLDER = ImageData.CreateTrainingFilepath(REEL_LPN);
            FONT_FOLDER = ImageData.CreateFontFilepathAllMachines();
            showSpinner(false);
            HOperatorSet.SetSystem("clip_region", "false");
            startedAt = DateTime.Now;

            loadFontSizes();

            LOADING_FORM = false;
            //cboCharacterContrast.SelectedIndex = 0;
            cboLabelType.SelectedIndex = 0;
            cboDebrisSize.SelectedIndex = 0;
            cboVariationDebrisSize.SelectedIndex = 0;
            buildMedDataItemList();
            DisplayInfo("Please press the <ENTER> Key to start setup", "top");
            base.OnShown(e);

            frmLabelType frmLT = new frmLabelType();
            frmLT.ShowDialog();
            this.LabelTYPE = frmLT.label_Type;

            if (LabelTYPE == LabelType.FLATPANEL)
            {
                cboLabelType.SelectedIndex = 0;
                iParams.SobelEdgeThreshold = iParamDefaults.SobelEdge1;
                iParams.SobelAmpSize = iParamDefaults.SobelAmpSize1;
                doDarkLabelSegmentation(false);
            }
            else if (LabelTYPE == LabelType.DARK)
            {
                cboLabelType.SelectedIndex = 1;
                iParams.SobelEdgeThreshold = iParamDefaults.SobelEdge2;
                iParams.SobelAmpSize = iParamDefaults.SobelAmpSize2;
                doDarkLabelSegmentation(true);
            }
            else if (LabelTYPE == LabelType.BOOKLET)
            {
                cboLabelType.SelectedIndex = 2;
                iParams.SobelEdgeThreshold = iParamDefaults.SobelEdge3;
                iParams.SobelAmpSize = iParamDefaults.SobelAmpSize3;
                doDarkLabelSegmentation(false);
            }
            frmLT.Close();
            frmLT = null;
            doSetDebrisSize();
        }

        private bool loadFontSizes()
        {
            bool retVal = false;
            try
            {
                if(listFSS==null)
                    listFSS = new List<FontSizesSegment>();
                //cboCharacterContrast.Items.Clear(); 
                listFSS = DataManager.GetListFontSizes();
                //foreach(FontSizesSegment fss in listFSS)
                //{
                //    string fontsizeName = fss.Size;
                //    cboCharacterContrast.Items.Add(fontsizeName);
                //}
                retVal = true;
            }
            catch (Exception ex)
            {
                string err = "loadFontSizes() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            return retVal;
        }

        private bool loadVDEToolsAndData(string reel_lpn)
        {
            bool retVal = false;
            try
            {
                LIDs = DataManager.LabelDataItems(LABEL_ITEM, reel_lpn);

                //foreach(LabelItemDataSetup lid in LIDs)
                //    lid.VDE_DATA = lid.VDE_DATA_LIST[0];

                foreach (LabelItemDataSetup lid in LIDs)
                {
                    lid.VDE_DATA = lid.VDE_DATA_LIST[0];
                    if (lid.DataPresent == false)
                    {
                        string err = string.Format("Label information is missing for label item: {0}. Cannot build VDE data item model. loadVDEToolsAndData()", LABEL_ITEM);
                        SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Red);
                        uscMD.SystemMessage(smea);
                        return retVal;
                    }
                    uscVDEItem vdi = new uscVDEItem(lid);
                    //vdi.Dock = DockStyle.Fill;
                    vdi.AutoSize = false;
                    vdi.AutoSizeMode = AutoSizeMode.GrowOnly;
                    fplvdeitems.FlowDirection = FlowDirection.LeftToRight;
                    fplvdeitems.SetFlowBreak(vdi, true);
                    fplvdeitems.Controls.Add(vdi);
                    
                    //MedData meddata = DataManager.LabelDataInspection(lwo, reel_lpn, vdi.VariableName);
                    //meddata.PlaceHolder = vdi.PlaceHolder;
                    //meddata.VarName = vdi.VariableName;
                    //meddata.Repeat = vdi.Repeat;
                    //if (meddata.Repeat > 0)
                    //    meddata.repeatIndex = 1;
                    //else
                    //    meddata.repeatIndex = 0;
                    //MedDataItems.Add(meddata);
                }



                //int numRows = LIDs.Count;
                ////float height = 72;
                //for (int i = 1; i <= numRows; i++)
                //{
                //    tlpVDE.RowStyles.Insert(1, new RowStyle(SizeType.AutoSize)); // height); //);
                //}
                //int x = 1;
                //foreach (LabelItemDataSetup lid in LIDs)
                //{
                //    if (lid.DataPresent == false)
                //    {
                //        string err = "Label identification information is missing for label item: {0}\nCannot build VDE data item model.\nloadVDEToolsAndData() " + LABEL_ITEM;
                //        MessageBox.Show(this, err);
                //        return retVal;
                //    }
                //    uscVDEItem vdi = new uscVDEItem(lid);
                //    tlpVDE.Controls.Add(vdi, 0, x);
                //    vdi.Dock = DockStyle.Fill;
                //    x++;
                //}
                //tlpVDE.Controls.Add(aControl, 1, tlpVDE.RowStyles.Count - 1);
            }
            catch (Exception ex)
            {
                string err = "loadVDEToolsAndData() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            return retVal;
        }

        private bool complementZonesAndMasks(ref HObject img)
        {
            bool retVal = true;
            try
            {
                HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                hWinOCR.HalconWindow.SetColor("navy");
                hWinOCR.HalconWindow.SetLineWidth(2);
                HOperatorSet.SetDraw(hWinOCR.HalconWindow, "fill");
                foreach (VDEItem vdi in VDEReelConfig.VDEItems)
                {
                    if (vdi.IsMask)
                    {
                        int ROW1 = Convert.ToInt32(vdi.MaskedRegion[0] - 4);
                        int COL1 = Convert.ToInt32(vdi.MaskedRegion[1] - 4);
                        int ROW2 = Convert.ToInt32(vdi.MaskedRegion[2] + 4);
                        int COL2 = Convert.ToInt32(vdi.MaskedRegion[3] + 4);
                        if (ROW1 < 0)
                            ROW1 = 0;
                        if (COL1 < 0)
                            COL1 = 0;
                        if (ROW2 > h)
                            ROW2 = h;
                        if (COL2 > w)
                            COL2 = w;
                        HOperatorSet.GenRectangle1(out HObject region, ROW1, COL1, ROW2, COL2);
                        HOperatorSet.Complement(region, out HObject RegionComplement);
                        HOperatorSet.ReduceDomain(img, RegionComplement, out img);
                        region.DispObj(hWinOCR.HalconWindow);
                        if (region != null)
                            region.Dispose();
                        if (RegionComplement != null)
                            RegionComplement.Dispose();
                    }
                    else if (vdi.IsOPZone)
                    {
                        int ROW1 = Convert.ToInt32(vdi.ODP.OPZoneVARRegion[0] - 4);
                        int COL1 = Convert.ToInt32(vdi.ODP.OPZoneVARRegion[1] - 4);
                        int ROW2 = Convert.ToInt32(vdi.ODP.OPZoneVARRegion[2] + 4);
                        int COL2 = Convert.ToInt32(vdi.ODP.OPZoneVARRegion[3] + 4);
                        if (ROW1 < 0)
                            ROW1 = 0;
                        if (COL1 < 0)
                            COL1 = 0;
                        if (ROW2 > h)
                            ROW2 = h;
                        if (COL2 > w)
                            COL2 = w;
                        HOperatorSet.GenRectangle1(out HObject region, ROW1, COL1, ROW2, COL2);
                        HOperatorSet.Complement(region, out HObject RegionComplement);
                        HOperatorSet.ReduceDomain(img, RegionComplement, out img);
                        region.DispObj(hWinOCR.HalconWindow);
                        if (region != null)
                            region.Dispose();
                        if (RegionComplement != null)
                            RegionComplement.Dispose();
                    }
                    else if (vdi.IsBarcode2D || vdi.IsBarcodeLinear)
                    {
                        int ROW1 = Convert.ToInt32(vdi.BarcodeRegion[0] - 8);
                        int COL1 = Convert.ToInt32(vdi.BarcodeRegion[1] - 8);
                        int ROW2 = Convert.ToInt32(vdi.BarcodeRegion[2] + 8);
                        int COL2 = Convert.ToInt32(vdi.BarcodeRegion[3] + 8);
                        if (ROW1 < 0)
                            ROW1 = 0;
                        if (COL1 < 0)
                            COL1 = 0;
                        if (ROW2 > h)
                            ROW2 = h;
                        if (COL2 > w)
                            COL2 = w;
                        HOperatorSet.GenRectangle1(out HObject region, ROW1, COL1, ROW2, COL2);
                        HOperatorSet.Complement(region, out HObject RegionComplement);
                        HOperatorSet.ReduceDomain(img, RegionComplement, out img);
                        region.DispObj(hWinOCR.HalconWindow);
                        if (region != null)
                            region.Dispose();
                        if (RegionComplement != null)
                            RegionComplement.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "complementZonesAandMasks() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
            }
            return retVal;
        }

        private bool findAndMaskMasks(ref HObject img)
        {
            bool retVal = true;
            try
            {
                HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                hWinOCR.HalconWindow.SetColor("firebrick");
                hWinOCR.HalconWindow.SetLineWidth(8);
                foreach (VDEItem vdi in VDEReelConfig.VDEItems)
                {
                    if (vdi.IsMask)
                    {
                        int ROW1 = Convert.ToInt32(vdi.MaskedRegion[0] - 10);
                        int COL1 = Convert.ToInt32(vdi.MaskedRegion[1] - 10);
                        int ROW2 = Convert.ToInt32(vdi.MaskedRegion[2] + 10);
                        int COL2 = Convert.ToInt32(vdi.MaskedRegion[3] + 10);
                        if (ROW1 < 0)
                            ROW1 = 0;
                        if (COL1 < 0)
                            COL1 = 0;
                        if (ROW2 > h)
                            ROW2 = h;
                        if (COL2 > w)
                            COL2 = w;
                        HOperatorSet.GenRectangle1(out HObject region, ROW1, COL1, ROW2, COL2);
                        HOperatorSet.Complement(region, out HObject RegionComplement);
                        HOperatorSet.ReduceDomain(img, RegionComplement, out img);
                        if (region != null)
                            region.Dispose();
                        if (RegionComplement != null)
                            RegionComplement.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "FindAndMaskMasks() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            return retVal;
        }

        private bool getText(TReader tr, HObject img, List<uscVDEItem> vdeitems, string opzonename)
        {
            bool retVal = false;
            HTuple textresultid = null;
            HObject characters = null, rgnZone = null, imgForeground = null, connectedChars = null, connectedRgnChars = null;
            HTuple word = null;
            HTuple wordscore = null;
            HTuple classVal = null, confidence = null;
            List<string> valuesToFind = new List<string>();
            List<uscVDEItem> vdeItemsWithData = new List<uscVDEItem>();

            try
            {
                HOperatorSet.GenEmptyObj(out characters);
                HOperatorSet.SmallestRectangle1(img, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                HOperatorSet.GenRectangle1(out rgnZone, r1, c1, r2, c2);

                //HTuple charHeightValues = new HTuple();
                //charHeightValues[0] = tr.CharHeight / 2;
                //charHeightValues[1] = 16.0;
                //charHeightValues[2] = tr.CharHeight + 20;
                //HTuple charWidthValues = new HTuple();
                //charWidthValues[0] = tr.CharWidth;
                //charWidthValues[1] = tr.CharWidth * 0.5;
                //charWidthValues[2] = tr.CharWidth * 1.05;


                word = new HTuple();
                wordscore = new HTuple();
                word = null;
                if (characters != null)
                    characters.Dispose();
                HOperatorSet.GenEmptyObj(out characters);
                HOperatorSet.SegmentCharacters(rgnZone, img, out imgForeground, out characters, "local_contrast_best", "false", "false", tr.StrokeWidth, tr.CharWidth, tr.CharHeight, 0, tr.SegmentContrast, out HTuple usedThreshold);
                HOperatorSet.SelectCharacters(characters, out connectedChars, "false", tr.StrokeWidth, tr.CharWidth, tr.CharHeight, "true", "false", tr.PartitionMethod, "false", "wide", "true", 400, "completion");     // Altered Version               
                characters.Dispose();
                HOperatorSet.Connection(connectedChars, out connectedRgnChars);
                HOperatorSet.SelectShape(connectedRgnChars, out characters, "area", "and", 80, 99999);
                HOperatorSet.DoOcrWordMlp(characters, img, tr.ocrHandle, "", 5, 3, out classVal, out confidence, out word, out wordscore);

                if (classVal == null)
                    return retVal;

                if (classVal.Length < 1)
                    return retVal;

                string classValues = "";
                for (int x = 0; x <= classVal.Length - 1; x++)
                    classValues = classValues + classVal[x];

                 //int dotReplace = -1;
                for (int x = 0; x <= vdeitems.Count - 1; x++)
                {
                    vdeitems[x].VDEData = vdeitems[x].VDEData.Replace(" ", "");
                    string sVDE = vdeitems[x].VDEData;
                    sVDE = sVDE.Replace(" ", "");
                    if (sVDE.Contains("_"))
                    {
                        int indexOfUnderscore = sVDE.IndexOf("_");
                        if (indexOfUnderscore > -1)
                        {
                            if (classValues.EndsWith("-"))
                                classValues = classValues.Substring(0, indexOfUnderscore) + "_" + classValues.Substring(indexOfUnderscore, classVal.Length - (1 + indexOfUnderscore));
                            if(classValues==sVDE)
                                break;
                        }
                    }
                }

                int startIndex = -1;
                for (int x = vdeitems.Count - 1; x >= 0; x--)
                {
                    startIndex = classValues.IndexOf(vdeitems[x].VDEData);
                    if (startIndex > -1)
                    {
                        valuesToFind.Add(vdeitems[x].VDEData);
                        vdeItemsWithData.Add(vdeitems[x]);
                    }
                    startIndex = -1;
                }
                if (valuesToFind.Count == 0)
                    return retVal;
 
                if (vdeItemsWithData.Count > 0)
                {
                    string resultText = classValues;
                    if (resultText == "")
                        return retVal;
                    createVDERegions(0, ref img, vdeItemsWithData, resultText, characters, tr, valuesToFind, opzonename, tr.FontSize);
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "getText() err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (rgnZone != null)
                    rgnZone.Dispose();
                if (imgForeground != null)
                    imgForeground.Dispose();
                if (characters != null)
                    characters.Dispose();
                if (textresultid != null)
                    HOperatorSet.ClearTextResult(textresultid);
            }
            return retVal;
        }

        private bool getText(int angle, TReader tr, ref HObject imgRotated, ref HObject region, ref List<uscVDEItem> vdeitems, string opzonename, ref List<string> testerrors)
        {
            bool retVal = true;
            HTuple textresultid = null;
            HObject characters = null, imgReduced = null, rgnZone = null, imgForeground = null, connectedChars = null, connectedRgnChars = null;
            HTuple word = null;
            HTuple wordscore = null;
            HTuple classVal = null, confidence = null;
            List<string> valuesToFind = new List<string>();
            foreach(string s in valuesToFind)
            {
                //Console.WriteLine("s = " + s);
            }
            List<uscVDEItem> vdeItemsWithData = new List<uscVDEItem>();
                        
            try
            {
                HOperatorSet.GenEmptyObj(out characters);
                HOperatorSet.SmallestRectangle1(region, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                HOperatorSet.GenRectangle1(out rgnZone, r1, c1, r2, c2);
                HOperatorSet.ReduceDomain(imgRotated, region, out imgReduced);

                word = new HTuple();
                wordscore = new HTuple();
                word = null;
                if (characters != null)
                    characters.Dispose();
                HOperatorSet.GenEmptyObj(out characters);
                HOperatorSet.SegmentCharacters(rgnZone, imgReduced, out imgForeground, out characters, "local_contrast_best", "false", "false", tr.StrokeWidth, tr.CharWidth, tr.CharHeight, 0, tr.SegmentContrast, out HTuple usedThreshold);
                HOperatorSet.SelectCharacters(characters, out connectedChars, "false", tr.StrokeWidth, tr.CharWidth, tr.CharHeight, "true", "false", tr.PartitionMethod, "false", "medium", "false", 20, "completion");
                characters.Dispose();
                HOperatorSet.Connection(connectedChars, out connectedRgnChars);
                HOperatorSet.SelectShape(connectedRgnChars, out characters, "area", "and", 80, 99999);
                HOperatorSet.DoOcrWordMlp(characters, imgReduced, tr.ocrHandle, "", 5, 3, out classVal, out confidence, out word, out wordscore);

                if (classVal == null)
                {
                    testerrors.Add(string.Format("VDE {0} not found in {1}. Image #: {2}", vdeitems[0].VDEDataList[0].ToString(), opzonename, imageIndex));
                    return retVal;
                }
                if (classVal.Length < 1)
                {
                    testerrors.Add(string.Format("VDE {0} not found in {1}. Image #: {2}", vdeitems[0].VDEDataList[0].ToString(), opzonename, imageIndex));
                    return retVal;
                }
                string classValues = "";
                for (int x = 0; x <= classVal.Length - 1; x++)
                    classValues = classValues + classVal[x];

                
                for (int x = 0; x <= vdeitems.Count - 1; x++)                    
                {
                    vdeitems[x].VDEData = vdeitems[x].VDEData.Replace(" ", "");
                    vdeitems[x].VDEData = vdeitems[x].VDEData.Replace("_", "");


                    string sVDE = vdeitems[x].VDEData;
                    sVDE = sVDE.Replace(" ", "");
                    if (sVDE.Contains("_"))
                    {
                        int indexOfUnderscore = sVDE.IndexOf("_");

                        if (indexOfUnderscore > -1)
                        {
                            if (classValues.EndsWith("-"))
                                classValues = classValues.Substring(0, indexOfUnderscore) + "_" + classValues.Substring(indexOfUnderscore, classVal.Length - (1 + indexOfUnderscore));
                            if (classValues == sVDE)
                            {
                                vdeitems[x].VDEData = sVDE;
                                vdeItemsWithData.Add(vdeitems[x]);
                                break;
                            }
                        }
                    }
                }
                
                int startIndex = -1;                
                for (int x = 0; x <= vdeitems.Count - 1; x++)
                {
                    startIndex = classValues.IndexOf(vdeitems[x].VDEData);
                    
                    if (startIndex > -1)
                    {
                        valuesToFind.Add(vdeitems[x].VDEData);
                        vdeItemsWithData.Add(vdeitems[x]);
                    }
                    startIndex = -1;
                }
                if (valuesToFind.Count == 0)
                    return retVal;

                if (vdeItemsWithData.Count > 0)
                {
                    string resultText = classValues;

                    if (resultText == "")
                    {
                        testerrors.Add(string.Format("VDE {0} not found in {1}. Image #: {2}", vdeitems[0].VDEDataList[0].ToString(), opzonename, imageIndex));
                        return retVal;
                    }
                    createVDERegions(angle, ref imgRotated, vdeItemsWithData, resultText, characters, tr, valuesToFind, opzonename, tr.FontSize);
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "getText() err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (rgnZone != null)
                    rgnZone.Dispose();
                if (imgForeground != null)
                    imgForeground.Dispose();
                if (imgReduced != null)
                    imgReduced.Dispose();
                if (characters != null)
                    characters.Dispose();
                if (textresultid != null)
                    HOperatorSet.ClearTextResult(textresultid);
            }
        return retVal;
        }


        private void findVDE(HObject img, string opzonename)
        {
            List<uscVDEItem> VDEData = new List<uscVDEItem>();
            try
            {
                DisplayInfo("identifying VDE data in region: " + opzonename, "top");
                foreach (Control vdecontrol in fplvdeitems.Controls)
                {
                    if (vdecontrol is uscVDEItem)
                    {
                        uscVDEItem uscVDE = (uscVDEItem)vdecontrol;
                        if (uscVDE.VDEData.Length > 0)
                            VDEData.Add(uscVDE);
                    }
                }
                foreach (FontSizesSegment fss in this.listFSS)
                {
                    TReader textreader = new TReader();
                    textreader.InitReader();
                    HTuple CH = fss.CharHeight;
                    HTuple CW = fss.CharWidth;
                    HTuple SW = fss.StrokeWidth;
                    HTuple SC = fss.SegmentContrast;
                    HTuple nameSegment = fss.Size;
                    HTuple PM = fss.PartitionMethod;
                    textreader.setFontSizeSegmentation(nameSegment, CH, CW, SW, SC, PM);
                    if(getText(textreader, img, VDEData, opzonename)==true)
                        break;
                }
            }
            catch (Exception ex)
            {
                string err = "findVDE() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (img != null)
                    img.Dispose();
            }
        }

        private void findVDE(int rotatedangle, int angle, HObject opzoneregion, string opzonename, ref List<string> testerrors)
        {
            List<uscVDEItem> VDEData = new List<uscVDEItem>();
            HObject imgReduced = null, img = null;
            try
            {
                DisplayInfo("identifying VDE data in region: " + opzonename, "top");
                HOperatorSet.CopyObj(Backgroundimage, out img, 1, 1);
                
                if (angle != 0)
                {
                    RotatedAngle = rotatedangle;
                    rotateBackground(angle, ref img);
                    HOperatorSet.SmallestRectangle1(opzoneregion, out HTuple top, out HTuple left, out HTuple bottom, out HTuple right);
                    int[] rgnCoord = new int[] { top, left, bottom, right };
                    HOperatorSet.GetImageSize(Backgroundimage, out HTuple w, out HTuple h);
                    rgnCoord = VDEItem.RotateClockwiseCoords(rotatedangle, w, h, rgnCoord);                    
                    opzoneregion.Dispose();
                    HOperatorSet.GenRectangle1(out opzoneregion, rgnCoord[0], rgnCoord[1], rgnCoord[2], rgnCoord[3]);

                     
                }
                HOperatorSet.ReduceDomain(img, opzoneregion, out imgReduced);
                foreach (Control vdecontrol in fplvdeitems.Controls)
                {
                    if (vdecontrol is uscVDEItem)
                    {
                        uscVDEItem uscVDE = (uscVDEItem)vdecontrol;
                        if (uscVDE.VDEData.Length > 0)
                            VDEData.Add(uscVDE);
                    }
                }
                foreach (FontSizesSegment fss in this.listFSS)
                {
                    TReader textreader = new TReader();
                    textreader.InitReader();
                    HTuple CH = fss.CharHeight;                    
                    HTuple CW = fss.CharWidth;
                    HTuple SW = fss.StrokeWidth;
                    HTuple SC = fss.SegmentContrast;
                    HTuple nameSegment = fss.Size;
                    HTuple PM = fss.PartitionMethod;
                    textreader.setFontSizeSegmentation(nameSegment, CH, CW, SW, SC, PM);
                    getText(angle, textreader, ref img, ref imgReduced, ref VDEData, opzonename, ref testerrors);
                }
            }
            catch (Exception ex)
            {
                string err = "findVDE() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (imgReduced != null)
                    imgReduced.Dispose();
                if (img != null)
                    img.Dispose();
                if (opzoneregion != null)
                    opzoneregion.Dispose();
            }
        }

        private bool createVDERegions(int angle, ref HObject img, List<uscVDEItem> vdeitemswithdata, string resulttext, HObject characters, TReader tr, List<string> valuestofind, string opzonename, string size)
        {
            bool retVal = true;
            HObject region = null, imgReduced = null, RegionComplement = null;
            int firstChar = 0;
            int lastChar = 0;
            string valueToFind = "";
            uscVDEItem vdi = null;
            TReader txtrdr = tr;
            List<string> datasNotFound = new List<string>();
            try
            {
                for (int xx = 0; xx < valuestofind.Count; xx++)
                {
                    txtrdr = tr;
                    vdi = vdeitemswithdata[xx];
                    valueToFind = valuestofind[xx];
                    firstChar = 0;
                    lastChar = 0;
                    string res = valueToFind;
                    while (firstChar >= 0)
                    {
                        if (firstChar >= resulttext.Length)
                            break;
                        firstChar = (resulttext.IndexOf(res, firstChar));
                        if (firstChar == -1)
                            break;
                        HObject mask = null;
                        HOperatorSet.GenEmptyObj(out mask);
                        lastChar = valueToFind.Length;
                        HOperatorSet.CopyObj(characters, out mask, firstChar + 1, lastChar);
                        HOperatorSet.CountObj(mask, out HTuple numChars);
                        int top = 500000;
                        int left = 500000;
                        int bottom = 0;
                        int right = 0;
                        for (int x = 1; x <= numChars; x++)
                        {
                            HOperatorSet.SmallestRectangle1(mask[x], out HTuple t, out HTuple l, out HTuple b, out HTuple r);
                            if (t < top)
                                top = t;
                            if (l < left)
                                left = l;
                            if (b > bottom)
                                bottom = b;
                            if (r > right)
                                right = r;
                        }
                        int[] maskCoord = new int[] { top - 4, left - 4, bottom + 4, right + 4 };
                        HOperatorSet.GetImageSize(Backgroundimage, out HTuple w, out HTuple h);
                        int[] vdecoords = VDEItem.RotateAntiClockwiseCoords(RotatedAngle, w, h, maskCoord);
                        VDEItem vi = VDEReelConfig.VDEItemExists(vdecoords, RotatedAngle);
                        if (vi == null)
                        {
                            vi = new VDEItem(VDEReelConfig.GetNextID(vdi.PlaceHolder), vdecoords, RotatedAngle, VDEType.VDE, txtrdr);
                            VDEReelConfig.VDEItems.Add(vi);
                        }
                        vi.Placeholder = vdi.PlaceHolder;
                        vi.DarkMinSizeVAR = (IsSmallDebrisSizeVar == true ? 50 : 100);
                        if (Defaults.DarkLabel == true)
                            vi.DarkSegmentContrast = Defaults.DarkLabelContrast;
                        else
                            vi.tr.SegmentContrast = tr.SegmentContrast;
                        vi.FontName = size;
                        vi.tr.CharHeight = tr.CharHeight;
                        vi.tr.CharWidth = tr.CharWidth;
                        vi.tr.StrokeWidth = tr.StrokeWidth;
                        vi.tr.PartitionMethod = tr.PartitionMethod;
                        vi.tr.FontSize = tr.FontSize;
                        vi.tr.SegmentContrast = tr.SegmentContrast;

                        vi.OpZoneName = opzonename;
                        HTuple minContrastVDE = new HTuple();
                        HTuple usedContrastVDE = 0;
                        if (region != null)
                            region.Dispose();
                        region = null;
                        if (imgReduced != null)
                            imgReduced.Dispose();
                        imgReduced = null;
                        int[] vderegion0 = null;
                        if (angle != 0)
                        {
                            vderegion0 = VDEItem.RotateClockwiseCoords(RotatedAngle, w, h, vi.VDERegion);
                            HOperatorSet.GenRectangle1(out region, vderegion0[0] - 4, vderegion0[1] - 4, vderegion0[2] + 4, vderegion0[3] + 4);
                        }
                        else
                        {
                            HOperatorSet.GenRectangle1(out region, vi.VDERegion[0] - 4, vi.VDERegion[1] - 4, vi.VDERegion[2] + 4, vi.VDERegion[3] + 4);
                        }
                        HOperatorSet.ReduceDomain(img, region, out imgReduced);
                        optimizeMSERValue(imgReduced, vdi.VDEData.Length, ref vi);
                        if (imgReduced != null)
                            imgReduced.Dispose();

                        if (mask != null)
                            mask.Dispose();
                        if (region != null)
                            region.Dispose();
                        if (RegionComplement != null)
                            RegionComplement.Dispose();
                        if (firstChar < resulttext.Length + vdi.VDEData.Length)
                            firstChar += vdi.VDEData.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "createVDERegions() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (imgReduced != null)
                    imgReduced.Dispose();
                //if (img != null)
                //    img.Dispose();
                if (RegionComplement != null)
                    RegionComplement.Dispose();
                if (region != null)
                    region.Dispose();
                if (characters != null)
                    characters.Dispose();
            }
            return retVal;
        }

        private bool optimizeMSERValue(HObject img, int vdelength, ref VDEItem vdi)
        {
            bool retVal = true;
            HObject rgnMSERDark = null, rgnMSERLight = null, connectedRegions = null;
            HTuple mserParams, delta;
            HTuple paramValues, numBlobs;
            try
            {
                mserParams = new HTuple();
                mserParams[0] = "min_diversity";
                mserParams[1] = "max_variation";
                mserParams[2] = "min_gray";
                mserParams[3] = "max_gray";
                delta = 7.0;
                paramValues = new HTuple();
                paramValues[0] = 2;
                paramValues[1] = 0.8;
                paramValues[2] = 1;

                int[] blobsFound = new int[32];
                int y = 0;
                for (int x = 32; x < 160; x += 4)
                {
                    paramValues[3] = x;
                    DisplayInfo(string.Format("optimizing {0} MSER area contrast: {1}...", vdi.VDEItemName, x), "top");
                    HOperatorSet.SegmentImageMser(img, out rgnMSERDark, out rgnMSERLight, "dark", 5, 5000000, delta, mserParams, paramValues);
                    HOperatorSet.Connection(rgnMSERDark, out connectedRegions);
                    HOperatorSet.CountObj(connectedRegions, out numBlobs);
                    if (numBlobs == vdelength)
                        blobsFound[y] = x;
                    else
                        blobsFound[y] = -1;

                    if (connectedRegions != null)
                        connectedRegions.Dispose();
                    if (rgnMSERDark != null)
                        rgnMSERDark.Dispose();
                    if (rgnMSERLight != null)
                        rgnMSERLight.Dispose();
                    numBlobs = 0;
                    y++;
                }

                int avg = 0;
                int divideby = 0;
                for (int x = 0; x < blobsFound.Length; x++)
                {
                    if (blobsFound[x] != -1)
                    {
                        avg += blobsFound[x];
                        divideby += 1;
                    }
                }
                if (divideby == 0)
                {
                    blobsFound = new int[32];
                    y = 0;
                    for (int x = 96; x < 224; x += 4)
                    {
                        paramValues[3] = x;
                        //DisplayInfo(string.Format("optimizing {0} MSER area contrast: {1}...", vdi.VDEItemName, x), "top");
                        HOperatorSet.SegmentImageMser(img, out rgnMSERDark, out rgnMSERLight, "dark", 5, 5000000, delta, mserParams, paramValues);
                        HOperatorSet.Connection(rgnMSERDark, out connectedRegions);
                        HOperatorSet.CountObj(connectedRegions, out numBlobs);
                        if (numBlobs == vdelength)
                            blobsFound[y] = x;
                        else
                            blobsFound[y] = -1;

                        if (connectedRegions != null)
                            connectedRegions.Dispose();
                        if (rgnMSERDark != null)
                            rgnMSERDark.Dispose();
                        if (rgnMSERLight != null)
                            rgnMSERLight.Dispose();
                        numBlobs = 0;
                        y++;
                    }

                    avg = 0;
                    divideby = 0;
                    for (int x = 0; x < blobsFound.Length; x++)
                    {
                        if (blobsFound[x] != -1)
                        {
                            avg += blobsFound[x];
                            divideby += 1;
                        }
                    }
                }
                if (divideby == 0)
                    vdi.CharacterContrast = 100;
                else
                    vdi.CharacterContrast = (avg / divideby);
                DisplayInfo("", "top");
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "optimizeMSERValue() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (connectedRegions != null)
                    connectedRegions.Dispose();
                if (img != null)
                    img.Dispose();
                if (rgnMSERDark != null)
                    rgnMSERDark.Dispose();
                if (rgnMSERLight != null)
                    rgnMSERLight.Dispose();
            }
            return retVal;
        }

        private bool maskVDEByOpZone(ref HObject img, int index, HObject region, string opzonename)
        {
            bool retVal = true;
            try
            {
                retVal = maskVDE(90, 0, ref img, region, opzonename);
                if (retVal == false) return retVal;
                retVal = maskVDE(180, 90, ref img, region, opzonename);
                if (retVal == false) return retVal;
                retVal = maskVDE(270, 180, ref img, region, opzonename);
                if (retVal == false) return retVal;
                retVal = maskVDE(360, -90, ref img, region, opzonename);
                if (retVal == false) return retVal;
                RotatedAngle = 90;
                drawVDEBorders();
                DisplayInfo("", "top");
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        private bool maskVDE(int rotatedangle, int angle, ref HObject img, HObject region, string opzonename)
        {
            bool retVal = true;
            HObject RegionComplement = null, imgRotated = null;
            try
            {
                HOperatorSet.SmallestRectangle1(region, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                RotatedAngle = rotatedangle;
                HOperatorSet.GenEmptyObj(out imgRotated);
                rotateBackground(angle, ref img, ref imgRotated);
                foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(x => x.OpZoneName == opzonename && x.IsVDE == true && x.VDERotatedAngle == angle))
                {
                    foreach (Control vdecontrol in fplvdeitems.Controls)
                    {
                        if (vdecontrol is uscVDEItem)
                        {
                            uscVDEItem uscvde = (uscVDEItem)vdecontrol;
                            if (vdi.Placeholder == uscvde.PlaceHolder)
                            {
                                if (uscvde.Repeat > 0)
                                {
                                    HOperatorSet.GenRectangle1(out HObject rgn, vdi.VDERegion[0] - 4, vdi.VDERegion[1] - 8, vdi.VDERegion[2] + 4, vdi.VDERegion[3] + 8);
                                    HOperatorSet.Complement(rgn, out RegionComplement);
                                    HOperatorSet.ReduceDomain(img, RegionComplement, out img);
                                }
                                if (RegionComplement != null)
                                    RegionComplement.Dispose();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "maskVDE() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (RegionComplement != null)
                    RegionComplement.Dispose();
                if (imgRotated != null)
                    imgRotated.Dispose();
            }
            return retVal;
        }

        private void ResetSetFound()
        {
            foreach (VDEItem vdi in VDEReelConfig.VDEItems)
                if (vdi.IsVDE)
                    vdi.ResetSetFound();
        }

        private void barcode2d()
        {
            try
            {
                hWinOCR.SetFullImagePart();
                //string[] BarCodes2D = { "GS1-128", "GS1 DataBar Truncated", "GS1 DataBar Stacked", "GS1 DataBar Stacked Omnidir", "GS1 DataBar Omnidir", "GS1 DataBar Limited", "GS1 DataBar Expanded", "GS1 DataBar Expanded Stacked", "GS1 DataMatrix", "GS1 QR Code", "Data Matrix ECC 200", "Micro QR Code", "PDF417", "QR Code", "Aztec Code", "GS1 Aztec Code" };
                //string[] BarCodes2D = { "GS1 DataMatrix", "GS1 QR Code", "Data Matrix ECC 200", "Micro QR Code", "PDF417", "QR Code", "Aztec Code", "GS1 Aztec Code" };
                string[] BarCodes2D = { "GS1 DataMatrix", "GS1 QR Code", "Data Matrix ECC 200" };
                int x = 0;
                hWinOCR.HalconWindow.SetColor("magenta");
                while (x < BarCodes2D.Length)
                {
                    List<string> BarcodeDatas = new List<string>();
                    List<string> BarcodePlaceHolders = new List<string>();
                    HOperatorSet.CreateDataCode2dModel(BarCodes2D[x], "default_parameters", "standard_recognition", out HTuple DataCodeHandle);

                    HOperatorSet.FindDataCode2d(Backgroundimage, out HObject SymbolXLDs, DataCodeHandle, "stop_after_result_num", new HTuple(2), out HTuple ResultHandles, out HTuple DecodedDataStrings);
                    HOperatorSet.ClearDataCode2dModel(DataCodeHandle);
                    HOperatorSet.CountObj(SymbolXLDs, out HTuple symbalCount);
                    if (DecodedDataStrings.Length > 0)
                    {
                        for (int y = 0; y < DecodedDataStrings.Length; y++)
                        {
                            string med = DecodedDataStrings[y].S;
                            string placeholder = "";
                            string non_vde_data = med;
                            foreach (Control c in fplvdeitems.Controls)
                                if (c is uscVDEItem)
                                {
                                    uscVDEItem vdi = (uscVDEItem)c;
                                    if (vdi.VDEData.Contains(med) || med.Contains(vdi.VDEData))
                                    {
                                        BarcodePlaceHolders.Add(vdi.PlaceHolder);
                                        BarcodeDatas.Add(vdi.VDEData);
                                        placeholder = vdi.PlaceHolder;
                                        non_vde_data = non_vde_data.Replace(vdi.VDEData, "");
                                    }
                                }
                            if (BarcodeDatas.Count == 0)
                                continue;

                            HOperatorSet.SelectObj(SymbolXLDs, out HObject points, y + 1);
                            HOperatorSet.GetContourXld(points, out HTuple RowPoint, out HTuple ColPoint);
                            if (RowPoint.Length >= 4)
                            {
                                string barcodeName = BarCodes2D[x];
                                int ROW1 = Convert.ToInt32(RowPoint.TupleMin().D - Defaults.PADDING);
                                int COL1 = Convert.ToInt32(ColPoint.TupleMin().D - Defaults.PADDING);
                                int ROW2 = Convert.ToInt32(RowPoint.TupleMax().D + Defaults.PADDING);
                                int COL2 = Convert.ToInt32(ColPoint.TupleMax().D + Defaults.PADDING);
                                int[] vdecoord = new int[] { ROW1, COL1, ROW2, COL2 };
                                int[] vdecoordextended = new int[] { ROW1, COL1, ROW2, COL2 };
                                if (!VDEReelConfig.VDEItemExists(vdecoord, RotatedAngle, hWinOCR))
                                {
                                    hWinOCR.HalconWindow.SetLineWidth(3);

                                    VDEItem vi = new VDEItem(VDEReelConfig.GetNextID("Barcode2D"), vdecoord, 0, 0, RotatedAngle, VDEType.BARCODE_2D);
                                    vi.BarcodeName = barcodeName;
                                    vi.BarcodeData = BarcodeDatas;
                                    vi.IsBarcode2D = true;
                                    vi.Placeholder = placeholder;
                                    vi.BarcodePlaceHolders = BarcodePlaceHolders;
                                    if (non_vde_data.Trim() != "")
                                        vi.BarcodeNonVDEData = nonVdeGs1Text(non_vde_data, BarcodePlaceHolders, BarcodeDatas);
                                    else
                                        vi.BarcodeNonVDEData = "";
                                    VDEReelConfig.VDEItems.Add(vi);
                                    //vi.Attach(hWinOCR);
                                    vi.ComplementZone(ref Backgroundimage, RotatedAngle);
                                }
                            }
                        }
                    }
                    x++;
                }
            }
            catch (Exception ex)
            {
                string err = "barcode2D() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                DoSummary();
            }
        }

        private string nonVdeGs1Text(string nonvdedata, List<string> placeholders, List<string> datas)
        {
            List<string> vdeDataList = new List<string>();
            for (int x = 0; x < placeholders.Count; x++)
            //foreach (string sd in datas)
            {
                string tmp = placeholders[x] + " : " + datas[x];
                vdeDataList.Add(tmp);
            }
            frmGS1 frmgs = new frmGS1(nonvdedata, vdeDataList);
            frmgs.ShowDialog();
            string retVal = frmgs._DATA;
            return retVal;
        }

        private bool findAndMaskBarcode2D(ref HObject img)
        {
            bool retVal = true;
            try
            {
                HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                hWinOCR.HalconWindow.SetColor("magenta");
                foreach (VDEItem vdi in VDEReelConfig.VDEItems)
                {
                    if (vdi.IsBarcode2D)
                    {
                        int ROW1 = Convert.ToInt32(vdi.BarcodeRegion[0] - 20);
                        int COL1 = Convert.ToInt32(vdi.BarcodeRegion[1] - 20);
                        int ROW2 = Convert.ToInt32(vdi.BarcodeRegion[2] + 20);
                        int COL2 = Convert.ToInt32(vdi.BarcodeRegion[3] + 20);
                        if (ROW1 < 0)
                            ROW1 = 0;
                        if (COL1 < 0)
                            COL1 = 0;
                        if (ROW2 > h)
                            ROW2 = h;
                        if (COL2 > w)
                            COL2 = w;
                        HOperatorSet.GenRectangle1(out HObject region, ROW1, COL1, ROW2, COL2);
                        HOperatorSet.Complement(region, out HObject RegionComplement);
                        HOperatorSet.ReduceDomain(img, RegionComplement, out img);
                        if (region != null)
                            region.Dispose();
                        if (RegionComplement != null)
                            RegionComplement.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "FindAndMaskBarcode2D() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            return retVal;
        }

        public void drawVDEBorders()
        {
            HTuple old_ls = null;
            int numVDEItems = 0;
            try
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    mnuVDE.Enabled = false;
                });
                hWinOCR.HalconWindow.ClearWindow();
                hWinOCR.HalconWindow.SetColor("red");
                hWinOCR.HalconWindow.SetLineWidth(2);
                HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
                foreach (VDEItem vdi in VDEReelConfig.VDEItems)
                {
                    if (vdi != null)
                    {
                        if (vdi.IsVDE)
                        {
                            numVDEItems++;
                            hWinOCR.HalconWindow.SetLineWidth(3);
                            hWinOCR.HalconWindow.SetColor("red");
                            HOperatorSet.GenRectangle1(out HObject vderegion, vdi.VDERegion[0], vdi.VDERegion[1], vdi.VDERegion[2], vdi.VDERegion[3]);
                            hWinOCR.HalconWindow.DispObj(vderegion);
                            vderegion.Dispose();
                        }
                        else if (vdi.IsOPZone)
                        {
                            hWinOCR.HalconWindow.SetLineWidth(3);
                            hWinOCR.HalconWindow.SetColor("cyan");
                            HOperatorSet.GenRectangle1(out HObject vderegion, vdi.ODP.OPZoneVARRegion[0], vdi.ODP.OPZoneVARRegion[1], vdi.ODP.OPZoneVARRegion[2], vdi.ODP.OPZoneVARRegion[3]);
                            hWinOCR.HalconWindow.DispObj(vderegion);
                            hWinOCR.HalconWindow.DispText(returnIdFromOpzoneName(vdi.VDEItemName), "image", vdi.ODP.OPZoneVARRegion[0] + 20, vdi.ODP.OPZoneVARRegion[1] + 20, "cyan", "box", "false");
                            vderegion.Dispose();
                            this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                            {
                                mnuVDE.Enabled = true;
                            });
                        }
                        else if (vdi.IsMask)
                        {
                            HOperatorSet.SetDraw(hWinOCR.HalconWindow, "fill");
                            hWinOCR.HalconWindow.SetLineWidth(3);
                            hWinOCR.HalconWindow.SetColor("firebrick");
                            HOperatorSet.GenRectangle1(out HObject vderegion, vdi.MaskedRegion[0], vdi.MaskedRegion[1], vdi.MaskedRegion[2], vdi.MaskedRegion[3]);
                            hWinOCR.HalconWindow.DispObj(vderegion);
                            hWinOCR.HalconWindow.DispText(returnIdFromOpzoneName(vdi.VDEItemName), "image", vdi.MaskedRegion[0] + 20, vdi.MaskedRegion[1] + 20, "white", "box", "false");
                            vderegion.Dispose();
                            HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
                        }
                        else if (vdi.IsBarcode2D || vdi.IsBarcodeLinear)
                        {
                            hWinOCR.HalconWindow.SetLineWidth(3);
                            hWinOCR.HalconWindow.SetColor("magenta");
                            HOperatorSet.GenRectangle1(out HObject vderegion, vdi.BarcodeRegion[0], vdi.BarcodeRegion[1], vdi.BarcodeRegion[2], vdi.BarcodeRegion[3]);
                            hWinOCR.HalconWindow.DispObj(vderegion);
                            vderegion.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "drawVDEBorders() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (old_ls != null)
                    hWinOCR.HalconWindow.SetLineStyle(old_ls);
                if (numVDEItems == 0)
                    DisplayInfo("", "top");
            }
        }

        private string returnIdFromOpzoneName(string name)
        {
            string retVal = "";
            try
            {
                int startPoint = name.IndexOf("_");
                if (startPoint > 1)
                    retVal = name.Substring(startPoint + 1);
            }
            catch (Exception ex)
            {
                string err = "returnIdFromOpzoneName() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            return retVal;
        }

        private List<string> SummaryBarcode()
        {
            List<string> retVal = VDEReelConfig.CalculateBarcodeData();

            if (lblBarcode.InvokeRequired)
            {
                lblBarcode.Invoke((MethodInvoker)delegate
                {
                    lblBarcode.Text = "";
                    foreach (string s in retVal)
                        lblBarcode.Text = lblBarcode.Text + s;
                });
            }
            else
            {
                lblBarcode.Text = "";

                foreach (string s in retVal)
                    lblBarcode.Text = lblBarcode.Text + s;
            }
            return retVal;
        }

        private List<string> SummaryOverPrint()
        {
            //if (VARIATION_COMPLETE)
            //    hWinOCR.HalconWindow.ClearWindow();
            List<string> retVal = VDEReelConfig.CalculateOverPrintData();

            if (lblOP.InvokeRequired)
            {
                lblOP.Invoke((MethodInvoker)delegate
                {
                    lblOP.Text = "";
                    foreach (string s in retVal)
                        lblOP.Text = lblOP.Text + s + '\n';
                });
            }
            else
            {
                lblOP.Text = "";
                foreach (string s in retVal)
                    lblOP.Text = lblOP.Text + s + Environment.NewLine;
            }
            return retVal;
        }

        //public List<string> SummaryLabel()
        //{
        //    List<string> retVal = new List<string>();
        //    string[] summary = null;
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("Threshold: ").Append(this.MSERLabel.DarkMaxGray.ToString()).Append('\n');
        //        sb.Append("Minimum Area Size: ").Append(this.MSERLabel.DarkMinSizeMSER.ToString()).Append('\n');
        //        summary = sb.ToString().Split('\n');
        //        retVal.AddRange(summary);
        //    }
        //    catch (Exception ex)
        //    {
        //        string err = "SummaryLabel() err: " + ex.Message;
        //        SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Setup", (int)CriticalLevels.Amber);
        //        MessageBox.Show(err);
        //    }
        //    return retVal;
        //}

        private List<string> SummaryVDE()
        {
            List<string> retVal = VDEReelConfig.CalculateVDEData();
            if (lblVDE.InvokeRequired)
            {
                lblVDE.Invoke((MethodInvoker)delegate
                {
                    if (VARIATION_COMPLETE)
                        hWinOCR.HalconWindow.ClearWindow();
                    lblVDE.Text = "";
                    foreach (string s in retVal)
                    {                        
                        lblVDE.Text = lblVDE.Text + s + Environment.NewLine;
                    }
                });
            }
            else
            {
                if (VARIATION_COMPLETE)
                    hWinOCR.HalconWindow.ClearWindow();
                lblVDE.Text = "";
                foreach (string s in retVal)
                {
                    lblVDE.Text = lblVDE.Text + s + Environment.NewLine;
                }
            }
            return retVal;
        }

        private List<string> SummaryMasks()
        {
            List<string> retVal = VDEReelConfig.CalculateMaskData();

            if (lblMasking.InvokeRequired)
            {
                lblMasking.Invoke((MethodInvoker)delegate
                {
                    lblMasking.Text = "";
                    foreach (string s in retVal)
                        lblMasking.Text = lblMasking.Text + s + Environment.NewLine;
                });
            }
            else
            {
                lblMasking.Text = "";
                foreach (string s in retVal)
                    lblMasking.Text = lblMasking.Text + s + Environment.NewLine;
            }
            return retVal;
        }


        private bool MoveToNextVDE()
        {
            bool retVal = true;
            try
            {
                foreach (Control c in fplvdeitems.Controls)
                    if (c is uscVDEItem)
                    {
                        uscVDEItem vdi = (uscVDEItem)c;
                        retVal = vdi.MoveToNextVDE();
                        if (retVal == false)
                            break;
                    }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "MoveToNextVDE() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            return retVal;
        }

        private void barcodeLinear()
        {
            HTuple BarCodeHandle = null;
            try
            {
                HOperatorSet.GetImageSize(Backgroundimage, out HTuple Width, out HTuple Height);

                hWinOCR.SetFullImagePart();
                HOperatorSet.CreateBarCodeModel(new HTuple(), new HTuple(), out BarCodeHandle);
                HOperatorSet.SetBarCodeParam(BarCodeHandle, "stop_after_result_num", 0);
                HOperatorSet.SetBarCodeParam(BarCodeHandle, "min_identical_scanlines", 2);
                HOperatorSet.FindBarCode(Backgroundimage, out HObject SymbolRegions, BarCodeHandle, "auto", out HTuple BarcodeAsText);
                if (BarcodeAsText.Length == 0)
                    return;
                int barcodesCount = 0;
                int x = 0;
                HOperatorSet.CountObj(SymbolRegions, out HTuple barcodecount);
                barcodesCount = barcodecount.I;
                if (barcodesCount > 0)
                {
                    hWinOCR.HalconWindow.SetColor("magenta");
                    while (x < barcodesCount)
                    {
                        List<string> barcodeDatas = new List<string>();
                        List<string> barcodePlaceHolders = new List<string>();

                        string med = BarcodeAsText[x].S;
                        foreach (Control c in fplvdeitems.Controls)
                            if (c is uscVDEItem)
                                if (((uscVDEItem)c).VDEData.Contains(med) || med.Contains(((uscVDEItem)c).VDEData))
                                {
                                    barcodeDatas.Add(med);
                                    barcodePlaceHolders.Add(((uscVDEItem)c).PlaceHolder);
                                    break;
                                }
                        if (barcodeDatas.Count == 0)
                        {
                            x++;
                            continue;
                        }

                        HOperatorSet.GetBarCodeObject(out HObject regions, BarCodeHandle, "all", "symbol_regions");
                        HOperatorSet.GetBarCodeResult(BarCodeHandle, "all", "decoded_types", out HTuple DecodedDataTypes);
                        HOperatorSet.SmallestRectangle1(regions, out HTuple R1, out HTuple C1, out HTuple R2, out HTuple C2);
                        int ROW1 = R1[x].I - Defaults.PADDING;
                        int COL1 = C1[x].I - Defaults.PADDING;
                        int ROW2 = R2[x].I + Defaults.PADDING;
                        int COL2 = C2[x].I + Defaults.PADDING;
                        int[] vdecoord = new int[] { ROW1, COL1, ROW2, COL2 };
                        int[] vdecoordextended = new int[] { ROW1, COL1, ROW2, COL2 };
                        //HDrawingObject hdobj = HDrawingObject.CreateDrawingObject(HDrawingObject.HDrawingObjectType.RECTANGLE1, ROW1, COL1, ROW2, COL2);
                        //HOperatorSet.SetDrawingObjectParams(hdobj, "color", "magenta");
                        //hdobj.OnDrag(OnDragDrawingObjectBarcode);
                        //hdobj.OnResize(OnResizeDrawingObjectBarcode);
                        //HOperatorSet.AttachDrawingObjectToWindow(hWinOCR.HalconWindow, hdobj);
                        VDEItem vi = new VDEItem(VDEReelConfig.GetNextID("BarcodeLinear"), vdecoord, 0, 0, RotatedAngle, VDEType.BARCODE_LINEAR);
                        VDEReelConfig.VDEItems.Add(vi);
                        vi.BarcodeName = DecodedDataTypes[x].S;
                        vi.BarcodeData = barcodeDatas;
                        vi.IsBarcodeLinear = true;
                        vi.BarcodePlaceHolders = barcodePlaceHolders;
                        //vi.Attach(hWinOCR);
                        vi.ComplementZone(ref Backgroundimage, RotatedAngle);
                        x++;
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "barcodeLinear() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                try { HOperatorSet.ClearBarCodeModel(BarCodeHandle); } catch { }
                DoSummary();
            }
        }

        private bool findAndMaskBarcodeLinear(ref HObject img)
        {
            bool retVal = true;
            try
            {
                HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                hWinOCR.HalconWindow.SetColor("magenta");
                foreach (VDEItem vdi in VDEReelConfig.VDEItems)
                {
                    if (vdi.IsBarcodeLinear)
                    {
                        int ROW1 = Convert.ToInt32(vdi.BarcodeRegion[0] - 20);
                        int COL1 = Convert.ToInt32(vdi.BarcodeRegion[1] - 20);
                        int ROW2 = Convert.ToInt32(vdi.BarcodeRegion[2] + 20);
                        int COL2 = Convert.ToInt32(vdi.BarcodeRegion[3] + 20);
                        if (ROW1 < 0)
                            ROW1 = 0;
                        if (COL1 < 0)
                            COL1 = 0;
                        if (ROW2 > h)
                            ROW2 = h;
                        if (COL2 > w)
                            COL2 = w;
                        HOperatorSet.GenRectangle1(out HObject region, ROW1, COL1, ROW2, COL2);
                        HOperatorSet.Complement(region, out HObject RegionComplement);
                        HOperatorSet.ReduceDomain(img, RegionComplement, out img);
                        if (region != null)
                            region.Dispose();
                        if (RegionComplement != null)
                            RegionComplement.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "FindAndMaskBarcodeLinear() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            return retVal;
        }

        private void cmdGrab_Click(object sender, EventArgs e)
        {
            doStart();
        }

        private void doStart()
        {
            showSpinner(true);
            doGrabClick();
        }

        private void doGrabClick()
        {
            Task t = new Task(prepareImageCapture);
            t.Start();
            RotatedAngle = 90;
            grabSetupImage();
        }

        private void prepareImageCapture()
        {
            hWinOCR.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                hWinOCR.HalconWindow.ClearWindow();
                DisplayInfo("waiting for images...", "top");
                //cmdAddImage.Enabled = false;
                hWinOCR.SetFullImagePart();
                hWinOCR.HMoveContent = false;
                //hWinComplement.SetFullImagePart();
                //hWinComplement.HMoveContent = false;
            });

            if (CameraManager.NectaCameras[0].nectaCam.Acquire == false)
                CameraManager.NectaCameras[0].nectaCam.Acquire = true;
        }

        public void DisplayInfo(string msg, string position)
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
                MessageBox.Show(err, "Label Training", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void DisplayInfo(string msg, Control lbl)
        {
            try
            {
                if (lbl.InvokeRequired)
                {
                    lbl.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        lbl.Text = msg;
                        lbl.Refresh();
                    });
                }
                else
                {
                    lbl.Text = msg;
                    lbl.Refresh();
                }
            }
            catch (Exception ex)
            {
                string err = "displayInfo() err: " + ex.Message;
                MessageBox.Show(err, "Label Training", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public bool FindVDEInRegion(HObject rgnvde, string opzonename)
        {
            bool retVal = true;
            HObject img = null;
            try
            {
                Backgroundimage.Dispose();
                HOperatorSet.CopyObj(TrainingImages, out Backgroundimage, 1, 1);
                HOperatorSet.ReduceDomain(Backgroundimage, rgnvde, out img);
                findVDE(img, opzonename);
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "FindVDEInRegion() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (rgnvde != null)
                    rgnvde.Dispose();
                if (img != null)
                    img.Dispose();
            }
            return retVal;
        }

        private void autoFindVDEMethods(HObject opzoneregion, string opzonename)
        {
            List<string> texterrors = new List<string>();
            HObject opzoneRegion = null;
            HObject img = null;
            try
            {
                for (int x = VDEReelConfig.VDEItems.Count - 1; x >= 0; x--)
                    if (VDEReelConfig.VDEItems[x].IsVDE)
                        if (VDEReelConfig.VDEItems[x].OpZoneName == opzonename)
                        {
                            VDEItem vi = (VDEItem)VDEReelConfig.VDEItems[x];
                            try { vi.DestroyVars(); } catch { }
                            VDEReelConfig.VDEItems.RemoveAt(x);
                            vi = null;
                        }

                Backgroundimage.Dispose();
                HOperatorSet.CopyObj(TrainingImages, out Backgroundimage, 1, 1);

                HOperatorSet.ReduceDomain(Backgroundimage, opzoneregion, out img);

                HOperatorSet.CopyObj(opzoneregion, out opzoneRegion, 1, 1);
                findVDE(90, 0, opzoneRegion, opzonename, ref texterrors);
                if (opzoneRegion != null)
                    opzoneRegion.Dispose();

                HOperatorSet.CopyObj(opzoneregion, out opzoneRegion, 1, 1);
                findVDE(180, 90, opzoneRegion, opzonename, ref texterrors);
                if (opzoneRegion != null)
                    opzoneRegion.Dispose();

                HOperatorSet.CopyObj(opzoneregion, out opzoneRegion, 1, 1);
                findVDE(270, 180, opzoneRegion, opzonename, ref texterrors);
                if (opzoneRegion != null)
                    opzoneRegion.Dispose();

                HOperatorSet.CopyObj(opzoneregion, out opzoneRegion, 1, 1);
                findVDE(360, -90, opzoneRegion, opzonename, ref texterrors);
                RotatedAngle = 90;
                drawVDEBorders();
                DisplayInfo("", "top");
            }
            catch (Exception ex)
            {
                string err = "autoFindVDEMethods() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                //if (textreader != null)
                //{
                //    try { textreader.textModelReader.ClearHandle(); } catch { }
                //    try { textreader.ocrHandle.ClearHandle(); } catch { }
                //}
                if (opzoneRegion != null)
                    opzoneRegion.Dispose();
                if (opzoneregion != null)
                    opzoneregion.Dispose();
                if (img != null)
                    img.Dispose();
            }
        }

        private void clearUsedData(ref TReader tr)
        {
            tr.StrokeWidth = "medium";
            tr.CharWidth = 0;
            tr.CharHeight = 0;
            tr.SegmentContrast = 0;
            tr.FontSize = "";
        }

        private void grabSetupImage()
        {
            if (camera.CameraConnected() == false)
            {
                showSpinner(false);
                MessageBox.Show("Camera " + camera.AliasName + " is not responding.\n\nPlease check all camera connections.", "Acquire Image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            GetImageSetup();

            while (setupImagesCaptured < testImageCount)
            {
                if (setupImagesCaptured < 1)
                    hWinOCR.HMoveContent = false;
                Application.DoEvents();
                if (setupImagesCaptured >= testImageCount)
                {
                    CameraManager.NectaCameras[0].SetChannelMethodCallerFalse();
                    return;
                }
                if (formClosing)
                    return;
            }
        }

        public void do_alarm()
        {
            try
            {
                int errorspresent = 0;
                List<string> errors = new List<string>();

                mxClient.ReadRegister(1, "Errors_Present", ref errorspresent, 3);
                if (errorspresent > 0)
                    errors = mxClient.ErrorRegisters(1);
                if (errors.Count > 0)
                {
                    string title = "Alarm Status";
                    string alarmFriendlyName = "";
                    foreach (string err in errors)
                    {
                        if (err != "")
                        {
                            alarmFriendlyName = "Active Alarm: " + err + " : " + PLCFailCodes.GetDescription(err);
                            SystemMessageEventArgs smea = new SystemMessageEventArgs(err, title, (int)CriticalLevels.Red);
                            uscMD.SystemMessage(smea);
                        }
                    }
                    EndTrainingError(alarmFriendlyName);
                }
            }
            catch (Exception ex)
            {
                string err = "doAlarm() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Alarm", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
        }

        private void EndTrainingError(string error)
        {
            if (error.Trim() == "")
                error = "Label Training Error\nImage Capture Timeout";
            ESignature es = new ESignature(Enums.ESigReason.CancelTrainingError, Enums.Roles.LVSIII_USer, error);
            es.AuthenticationOnly = true;
            es.ReasonRequired = true;
            es.UseLastReason = false;
            es.DoNotAcceptLastUser = false;
            es.CaptureSignature();
            if (es.SignatureAccepted)
            {
                DataManager.SaveAction("Timeout exeeded waiting for label image", "Label Training Error", REEL_LPN, Defaults.UserLoggedIn, "monitorForGrabTimeout()", "Label training, image not received", "");
                formClosing = true;
            }
        }

        private void GetImageSetup()
        {
            try
            {
                if (camera.CameraImage != null)
                    camera.CameraImage.Dispose();
                if (camera.nectaCam.Acquire == false)
                    camera.nectaCam.Acquire = true;
                mxClient.ResetAlarm(1);

                mxClient.WriteToRegister(1, "Capture_Image", 0, 3);
                mxClient.WriteToRegister(1, "Capture_Image", 1, 3);
                mxClient.WriteToRegister(1, "Capture_Image", 0, 3);

                Task t = new Task(() => camera.GrabCameraImage(buildImageLibrary));
                t.Start();
            }
            catch (Exception ex)
            {
                showSpinner(false);
                SYSTEM_IO.PROCESSING = false;
                string err = "GetImageSetup() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
        }

        private void processSetupImages()
        {
            try
            {
                if (TrainingImages.IsInitialized() == true)
                {
                    int count = 0;
                    try { count = TrainingImages.CountObj(); } catch { }
                    if (count == 0)
                        throw new Exception("processSetupImages() err: No Training Images were captured. Please restart setup");
                }
                EDITING_VARIATION = true;
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                string err = "ProcessSetupImage() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                showSpinner(false);
                EDITING_VARIATION = true;
            }
        }
        private void buildImageLibrary()
        {
            HObject tmp1 = null;
            try
            {
                showSpinner(false);
                if (setupImagesCaptured >= testImageCount)
                    return;

                if (camera.CameraImage == null)
                {
                    displayNullImageFromCamera(camera.AliasName);
                    return;
                }
                if (PLC_IS_REWINDING)
                {
                    if (NectaCameras[0].CameraImage != null)
                        NectaCameras[0].CameraImage.Dispose();
                    return;
                }
                if (tmp1 != null)
                    tmp1.Dispose();
                HOperatorSet.CopyObj(CameraManager.NectaCameras[0].CameraImage, out tmp1, 1, 1);
                if (NectaCameras[0].CameraImage != null)
                    NectaCameras[0].CameraImage.Dispose();

                if (tmp1.CountObj() == 0)
                {
                    DisplayInfo("Invalid or no image returned from camera", "top");
                    return;
                }
                if (setupImagesCaptured < variationImageCount)
                    DisplayInfo(string.Format("building variation image library:\nimage {0} of {1}", setupImagesCaptured + 1, variationImageCount), lblImageCount);
                else if (setupImagesCaptured < testImageCount)
                    DisplayInfo(string.Format("building test image library:\nimage {0} of {1}", setupImagesCaptured + 1, testImageCount), lblImageCount);
                else
                {
                    DisplayInfo("image library complete", lblImageCount);
                    enableBackForward(true);
                }

                reduceBackground(ref tmp1);


                this.Invoke((MethodInvoker)delegate
                {
                    cmdFullScreen.Enabled = true;
                });
                

                if (setupImagesCaptured == 0)
                {
                    if (TrainingImages == null)
                        HOperatorSet.GenEmptyObj(out TrainingImages);
                    if (TestImages == null)
                        HOperatorSet.GenEmptyObj(out TestImages);
                    if (setupImagesCaptured < variationImageCount)
                        HOperatorSet.ConcatObj(TrainingImages, tmp1, out TrainingImages);
                    if (setupImagesCaptured < testImageCount)
                        HOperatorSet.ConcatObj(TestImages, tmp1, out TestImages);

                    WAITING_FOR_IMAGE_ONE = false;
                    EDITING_VARIATION = true;
                    this.Invoke((MethodInvoker)delegate
                    {
                        cmdFullScreen.Enabled = true;
                        displayFirstImage();
                        DisplayInfo("please define the label area region", "top");
                        processSetupImages();
                    });
                }
                else if (setupImagesCaptured >= 1)
                {
                    if (setupImagesCaptured < variationImageCount)
                        HOperatorSet.ConcatObj(TrainingImages, tmp1, out TrainingImages);
                    if (setupImagesCaptured < testImageCount)
                        HOperatorSet.ConcatObj(TestImages, tmp1, out TestImages);

                }
                setupImagesCaptured += 1;
                if (setupImagesCaptured >= testImageCount)
                {
                    imageIndex = 1;
                    showSpinner(false);
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                string err = "buildImageLibrary() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (tmp1 != null)
                    tmp1.Dispose();
            }
        }

        private void IO_COS_Handler(int channel, bool high)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 50)
                ;
            sw.Stop();
            if (channel == SYSTEM_IO.ALARM)
                do_alarm();
        }

        private void displayNullImageFromCamera(string devicename)
        {
            try
            {
                string noImage = string.Format("\n\n\n                 {0}\n\n\n                 No Image returned", devicename);
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                string err = "displayNullImageFromCamera() err: " + ex.Message;
                MessageBox.Show(this, err);
                //SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Image Grab", (int)CriticalLevels.Red);
                //uscMDSetup.SystemMessage(smea);
            }
        }

        private void reduceBackground(ref HObject img)
        {
            HTuple edgeThreshold = 30, ampThreshold = 20, startRow = 200, rotate = 270, labelRow = null, labelCol = null;
            HTuple msrCol1 = null;
            HTuple msrRow2 = null;
            HTuple msrAmp1 = null;
            HTuple msrAmp2 = null;
            HTuple msrDist1 = null;
            HTuple msrDist2 = null;
            HTuple msr1 = null;
            HTuple msr2 = null;
            HObject tmpObj = null;
            HObject region = null;
            try
            {
                HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                HOperatorSet.GenMeasureRectangle2(h / 2, w / 2, new HTuple(90).TupleRad(), (h - 20) / 2, 50, w, h, "nearest_neighbor", out msr1);
                HOperatorSet.MeasurePos(img, msr1, 1.0, edgeThreshold, "all", "first", out labelRow, out msrCol1, out msrAmp1, out msrDist1);
                HOperatorSet.GenMeasureRectangle2(startRow, (w / 2), new HTuple(0).TupleRad(), (w - startRow) / 2, 25, w, h, "nearest_neighbor", out msr2);
                HOperatorSet.MeasurePos(img, msr2, 1.0, ampThreshold, "all", "first", out msrRow2, out labelCol, out msrAmp2, out msrDist2);
                if (labelCol.Length == 0)
                {
                    throw new Exception("reduceBackground() err: GenMeasurePos returned no edges from label image");
                }
                else
                    HOperatorSet.GenRectangle1(out region, 0, labelCol, h, w);
                if (tmpObj != null)
                    tmpObj.Dispose();

                HOperatorSet.ReduceDomain(img, region, out tmpObj);                

                HOperatorSet.CropDomain(tmpObj, out tmpObj);                

                HOperatorSet.RotateImage(tmpObj, out tmpObj, rotate, "constant");                

                if (img != null)
                    img.Dispose();
                HOperatorSet.CopyObj(tmpObj, out img, 1, 1);
            }
            catch (Exception ex)
            {
                string err = "reduceBackground() err: " + ex.Message;
                UtilityFunctions.WriteLog(err, EventLogEntryType.FailureAudit, "Label Training");
            }
            finally
            {
                if (tmpObj != null)
                    tmpObj.Dispose();
                if (region != null)
                    region.Dispose();
                if (CameraManager.NectaCameras[0].CameraImage != null)
                    CameraManager.NectaCameras[0].CameraImage.Dispose();
            }
        }

        private void displayFirstImage()
        {
            try
            {
                foreach (uscVDEItem data in MedDataList)
                    data.Reset();

                hWinOCR.Invoke((MethodInvoker)delegate
                {
                    if (Backgroundimage != null)
                        Backgroundimage.Dispose();
                    HOperatorSet.CopyObj(TrainingImages, out Backgroundimage, 1, 1);
                    HOperatorSet.DetachBackgroundFromWindow(hWinOCR.HalconWindow);
                    HOperatorSet.AttachBackgroundToWindow(Backgroundimage, hWinOCR.HalconWindow);
                    hWinOCR.SetFullImagePart();
                    hWinOCR.Invalidate();
                    hWinOCR.Refresh();
                });
            }
            catch (Exception ex)
            {
                string err = "displayFirstImage() err: " + ex.Message;
                UtilityFunctions.WriteLog(err, EventLogEntryType.FailureAudit, "Label Training");
            }
        }

        private void hWinOCR_HMouseUp(object sender, HMouseEventArgs e)
        {
            try
            {

                if (EDITING_VARIATION)
                    setMasking(false);

                if (e.Button == MouseButtons.Right)
                {
                    MouseUpRemoveVDEItem(e);
                    setMasking(true);
                    return;
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (EDITING_VARIATION)
                        MouseUpVariation(e);
                    else if (ADDING_OPZONES && cmdAddZone.Enabled == false)
                    {
                        return;
                    }
                    else if (ADDING_MASK && cmdAddMask.Enabled == false)
                        return;
                    else if (ADDING_VDE)
                        MouseUpVDERegion();

                    if (VDEReelConfig.VDEItems.Count > 0)
                    {
                        DoSummary();
                        drawVDEBorders();
                    }
                }
            }
            catch (Exception ex)
            {
                showSpinner(false);
                string err = "hWinOCR_HMouseUp() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                showSpinner(false);
            }
        }

        private void DoSummary()
        {
            SummaryVDE();
            SummaryOverPrint();
            SummaryMasks();
            SummaryBarcode();
        }

        private void MouseUpVariation(HMouseEventArgs e)
        {
            try
            {
                if (WAITING_FOR_IMAGE_ONE)
                    return;
                if (ADDING_ROT_ZONE)
                    return;
                imageIndex = 1;
                hWinOCR.HMoveContent = true;
                //hWinComplement.HMoveContent = true;
                HTuple r1 = lastY;
                HTuple r2 = e.Y;
                HTuple c1 = lastX;
                HTuple c2 = e.X;
                int X = Convert.ToInt32(e.X);
                int Y = Convert.ToInt32(e.Y);
                if (VariationROI != null)
                {
                    if (VariationROI.CountObj() > 0)
                        if (resizeEdge(X, Y, VariationROI))
                            return;
                }
                if (((c2 + c1 + r2 + r1) > 0) && (c2 > c1) && (r2 > r1))
                {
                    hWinOCR.HalconWindow.SetColor("green");
                    hWinOCR.HalconWindow.SetLineStyle(oldLineStyle);
                    hWinOCR.HalconWindow.SetLineWidth(3);
                    if (VariationROI != null)
                        VariationROI.Dispose();
                    HOperatorSet.GenRectangle1(out VariationROI, r1, c1, r2, c2);
                    hWinOCR.HalconWindow.DispObj(VariationROI);
                    //VARTop = Convert.ToInt32(r1.D);
                    //VARLeft = Convert.ToInt32(c1.D);
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string err = "MouseUpVariation() err: " + ex.Message;
                    MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
        }


        private string getOpZoneAt(int top, int left, int bottom, int right)
        {
            string retVal = "";
            try
            {
                foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
                {
                    if (top > vdi.ODP.OPZoneVARRegion[0])
                        if (left > vdi.ODP.OPZoneVARRegion[1])
                            if (bottom < vdi.ODP.OPZoneVARRegion[2])
                                if (right < vdi.ODP.OPZoneVARRegion[3])
                                {
                                    retVal = vdi.OpZoneName;
                                    return retVal;
                                }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    retVal = "";
                    string err = "getOpZoneAt() err: " + ex.Message;
                    MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
            return retVal;
        }


        private string getOpZoneAtPoint(int row, int col)
        {
            string retVal = "";
            try
            {
                foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
                {
                    if (row > vdi.ODP.OPZoneVARRegion[0])
                        if (row < vdi.ODP.OPZoneVARRegion[2])
                            if (col > vdi.ODP.OPZoneVARRegion[1])
                                if (col < vdi.ODP.OPZoneVARRegion[3])
                                {
                                    retVal = vdi.OpZoneName;
                                    return retVal;
                                }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    retVal = "";
                    string err = "getOpZoneAtPoint() err: " + ex.Message;
                    MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
            return retVal;
        }


        private bool opZoneExists(int top, int left, int bottom, int right)
        {
            bool retVal = false;
            try
            {
                foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
                {
                    if (UtilityFunctions.AlignsWith(top, vdi.ODP.OPZoneVARRegion[0], 100))
                        if (UtilityFunctions.AlignsWith(left, vdi.ODP.OPZoneVARRegion[1], 100))
                            if (UtilityFunctions.AlignsWith(bottom, vdi.ODP.OPZoneVARRegion[2], 100))
                                if (UtilityFunctions.AlignsWith(right, vdi.ODP.OPZoneVARRegion[3], 100))
                                    if (top != vdi.ODP.OPZoneVARRegion[0] && left != vdi.ODP.OPZoneVARRegion[1] && bottom != vdi.ODP.OPZoneVARRegion[2] && right != vdi.ODP.OPZoneVARRegion[3])
                                    {
                                        MessageBox.Show("The border is too close to existing opzone: " + vdi.OpZoneName);
                                        setMasking(true);
                                        return true;
                                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string err = "opZoneExists(t,l,b,r) err: " + ex.Message;
                    MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
            return retVal;
        }


        private bool ZoneOverlap(int top, int left, int bottom, int right)
        {
            bool retVal = false;
            try
            {
                foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
                {

                    if (top != vdi.ODP.OPZoneVARRegion[0] && left != vdi.ODP.OPZoneVARRegion[1] && bottom != vdi.ODP.OPZoneVARRegion[2] && right != vdi.ODP.OPZoneVARRegion[3])
                    {
                        // top left corner is inside
                        if (vdi.ODP.OPZoneVARRegion[0] < top && vdi.ODP.OPZoneVARRegion[2] > top)
                            if (vdi.ODP.OPZoneVARRegion[1] < left && vdi.ODP.OPZoneVARRegion[3] > left)
                            {
                                MessageBox.Show("OP-Zones cannot overlap or surround eachother.\n\n" + vdi.OpZoneName + " ovelaps.");
                                setMasking(true);
                                return true;
                            }
                        // bottom left corner is inside
                        if (vdi.ODP.OPZoneVARRegion[0] < bottom && vdi.ODP.OPZoneVARRegion[2] > bottom)
                            if (vdi.ODP.OPZoneVARRegion[1] < left && vdi.ODP.OPZoneVARRegion[3] > left)
                            {
                                MessageBox.Show("OP-Zones cannot overlap or surround eachother.\n\n" + vdi.OpZoneName + " ovelaps.");
                                setMasking(true);
                                return true;
                            }
                        //top right corner is inside
                        if (vdi.ODP.OPZoneVARRegion[0] < top && vdi.ODP.OPZoneVARRegion[2] > top)
                            if (vdi.ODP.OPZoneVARRegion[1] < right && vdi.ODP.OPZoneVARRegion[3] > right)
                            {
                                MessageBox.Show("OP-Zones cannot overlap or surround eachother.\n\n" + vdi.OpZoneName + " ovelaps.");
                                setMasking(true);
                                return true;
                            }
                        //bottom right corner is inside
                        if (vdi.ODP.OPZoneVARRegion[0] < bottom && vdi.ODP.OPZoneVARRegion[2] > bottom)
                            if (vdi.ODP.OPZoneVARRegion[1] < right && vdi.ODP.OPZoneVARRegion[3] > right)
                            {
                                MessageBox.Show("OP-Zones cannot overlap or surround eachother.\n\n" + vdi.OpZoneName + " ovelaps.");
                                setMasking(true);
                                return true;
                            }
                        //bottom side is inside
                        if (vdi.ODP.OPZoneVARRegion[0] < bottom && vdi.ODP.OPZoneVARRegion[2] > bottom)
                            if (vdi.ODP.OPZoneVARRegion[1] > right && vdi.ODP.OPZoneVARRegion[3] < right)
                            {
                                MessageBox.Show("OP-Zones cannot overlap or surround eachother.\n\n" + vdi.OpZoneName + " ovelaps.");
                                setMasking(true);
                                return true;
                            }
                        //top side is inside
                        if (vdi.ODP.OPZoneVARRegion[0] < top && vdi.ODP.OPZoneVARRegion[2] > top)
                            if (vdi.ODP.OPZoneVARRegion[1] > left && vdi.ODP.OPZoneVARRegion[3] < right)
                            {
                                MessageBox.Show("OP-Zones cannot overlap or surround eachother.\n\n" + vdi.OpZoneName + " ovelaps.");
                                setMasking(true);
                                return true;
                            }
                        //left side is inside
                        if (vdi.ODP.OPZoneVARRegion[0] > top && vdi.ODP.OPZoneVARRegion[2] < bottom)
                            if (vdi.ODP.OPZoneVARRegion[1] < left && vdi.ODP.OPZoneVARRegion[3] > left)
                            {
                                MessageBox.Show("OP-Zones cannot overlap or surround eachother.\n\n" + vdi.OpZoneName + " ovelaps.");
                                setMasking(true);
                                return true;
                            }
                        //right side is inside
                        if (vdi.ODP.OPZoneVARRegion[0] > top && vdi.ODP.OPZoneVARRegion[2] < bottom)
                            if (vdi.ODP.OPZoneVARRegion[1] < right && vdi.ODP.OPZoneVARRegion[3] > right)
                            {
                                MessageBox.Show("OP-Zones cannot overlap or surround eachother.\n\n" + vdi.OpZoneName + " ovelaps.");
                                setMasking(true);
                                return true;
                            }
                        //surrounding entirely
                        if (vdi.ODP.OPZoneVARRegion[0] > top && vdi.ODP.OPZoneVARRegion[1] > left)
                            if (vdi.ODP.OPZoneVARRegion[3] < right)
                                if (vdi.ODP.OPZoneVARRegion[2] < bottom)
                                {
                                    MessageBox.Show("OP-Zones cannot overlap or surround eachother.\n\n" + vdi.OpZoneName + " is surrounded.");
                                    setMasking(true);
                                    return true;
                                }
                        //entirely inside 
                        if (vdi.ODP.OPZoneVARRegion[0] < top && vdi.ODP.OPZoneVARRegion[1] < left)
                            if (vdi.ODP.OPZoneVARRegion[3] > right)
                                if (vdi.ODP.OPZoneVARRegion[2] > bottom)
                                {
                                    MessageBox.Show("OP-Zones cannot overlap or surround eachother.\n\n" + vdi.OpZoneName + " surroundsing.");
                                    setMasking(true);
                                    return true;
                                }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string err = "ZoneOverlap(t,l,b,r) err: " + ex.Message;
                    MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
            return retVal;
        }

        private void startVDEManual()
        {
            ADDING_VDE = true;
            cmdAddMask.Enabled = false;
            cmdAddZone.Enabled = false;
            cmdFullScreen.Enabled = false;
            mnuVDE.Enabled = false;
            mnuOptions.Enabled = false;
            mnufile.Enabled = false;
            cmdGenerate.Enabled = false;
        }

        private void cancelVDEManual()
        {
            ADDING_VDE = false;
            cmdAddMask.Enabled = true;
            cmdAddZone.Enabled = true;
            cmdFullScreen.Enabled = true;
            mnuVDE.Enabled = true;
            mnuOptions.Enabled = true;
            mnufile.Enabled = true;
            cmdGenerate.Enabled = true;
        }

        private VDEItem MouseUpVDERegion()
        {
            HObject imgReduced = null, VDERegion = null;
            VDEItem vi = null;
            try
            {
                showSpinner(false);
                hWinOCR.HalconWindow.ClearWindow();
                if (bottomStart + rightStart + leftStart + topStart == 0)
                {
                    cancelVDEManual();
                    return vi;
                }
                //HOperatorSet.SmallestRectangle1(VariationROI, out HTuple t, out HTuple l, out HTuple b, out HTuple r);
                //if (topStart < t)
                //    topStart = t;
                //if (leftStart < l)
                //    leftStart = l;
                //if (bottomStart > b)
                //    bottomStart = b;
                //if (rightStart > r)
                //    rightStart = r;

                string OpzoneName = getOpZoneAt(topStart, leftStart, bottomStart, rightStart);
                if (OpzoneName == "")
                {
                    MessageBox.Show("The area drawn is not entirely within an OpZone");
                    cancelVDEManual();
                    return vi;
                }
                else
                {
                    int[] vdecoord = new int[] { topStart, leftStart, bottomStart, rightStart };
                    HOperatorSet.GenRectangle1(out VDERegion, vdecoord[0], vdecoord[1], vdecoord[2], vdecoord[3]);
                    if (FindVDEInRegion(VDERegion, OpzoneName) == false)
                    {
                        MessageBox.Show("No VDE was found within area drawn");
                    }
                    topStart = 0;
                    leftStart = 0;
                    bottomStart = 0;
                    rightStart = 0;
                    hWinOCR.HMoveContent = true;
                    DoSummary();
                    drawVDEBorders();

                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string err = "MouseUpVDERegion() err: " + ex.Message;
                    MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
            finally
            {
                if (VDERegion != null)
                    VDERegion.Dispose();
                if (imgReduced != null)
                    imgReduced.Dispose();
                ADDING_VDE = false;
                cancelVDEManual();
                showSpinner(false);
            }
            return vi;
        }

        private VDEItem MouseUpOPZoneRegion()
        {
            HObject imgReduced = null, opzoneRegion = null, region = null;
            VDEItem vi = null;
            try
            {

                showSpinner(false);
                hWinOCR.HalconWindow.ClearWindow();
                if (bottomStart +rightStart + leftStart + topStart == 0)
                {
                    mnuClose.Enabled = false;
                    return vi;
                }

                if (((bottomStart - topStart >= 60) && (rightStart - leftStart >= 60) && (bottomStart > topStart) && (rightStart > leftStart)) == false)
                {
                    mnuClose.Enabled = false;
                    MessageBox.Show(this, "The region drawn is too small");
                    return vi;
                }

                HOperatorSet.SmallestRectangle1(VariationROI, out HTuple t, out HTuple l, out HTuple b, out HTuple r);
                if (topStart < t)
                    topStart = t;
                if (leftStart < l)
                    leftStart = l;
                if (bottomStart > b)
                    bottomStart = b;
                if (rightStart > r)
                    rightStart = r;

                if (ZoneOverlap(topStart, leftStart, bottomStart, rightStart))
                {
                    hWinOCR.HalconWindow.ClearWindow();
                    hWinOCR.HMoveContent = true;
                    drawVDEBorders();
                    topStart = 0;
                    bottomStart = 0;
                    leftStart = 0;
                    rightStart = 0;
                    cmdAddZone.Enabled = true;
                    showSpinner(false);
                    return vi;
                }
                else if(opZoneExists(topStart, leftStart, bottomStart, rightStart))
                {
                    hWinOCR.HalconWindow.ClearWindow();
                    hWinOCR.HMoveContent = true;
                    drawVDEBorders();
                    topStart = 0;
                    bottomStart = 0;
                    leftStart = 0;
                    rightStart = 0;
                    setMasking(true);
                    showSpinner(false);
                    return vi;
                }
                else
                {
                    int[] vdecoordVAR = new int[] { topStart, leftStart, bottomStart, rightStart };
                    if (VDEReelConfig.VDEItemExists(vdecoordVAR, RotatedAngle, hWinOCR)==false)
                    {
                        HOperatorSet.GenRectangle1(out opzoneRegion, vdecoordVAR[0], vdecoordVAR[1], vdecoordVAR[2], vdecoordVAR[3]);
                        HOperatorSet.ReduceDomain(Backgroundimage, opzoneRegion, out imgReduced);
                        HOperatorSet.AreaCenter(opzoneRegion, out HTuple area, out HTuple OPCenterY, out HTuple OPCenterX);
                        HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
                        hWinOCR.HalconWindow.SetLineWidth(4);
                        vi = new VDEItem(VDEReelConfig.GetNextID("OPZone"), vdecoordVAR, RotatedAngle, VDEType.OP);
                        vi.DarkMinSizeVAR = (IsSmallDebrisSizeVar == true ? 50 : 100);
                        VDEReelConfig.VDEItems.Add(vi);
                        vi.OpZoneName = vi.VDEItemName;
                        HOperatorSet.CopyImage(imgReduced, out vi.ODP.DomainImage);
                        topStart = 0;
                        leftStart = 0;
                        bottomStart = 0;
                        rightStart = 0;
                        hWinOCR.HMoveContent = true;
                        autoFindVDEMethods(opzoneRegion, vi.OpZoneName);
                        DoSummary();
                        drawVDEBorders();
                        if (VDEReelConfig.HasVDE())
                            mnuVDE.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string err = "MouseUpOPZoneRegion() err: " + ex.Message;
                    MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
            finally
            {
                if (opzoneRegion != null)
                    opzoneRegion.Dispose();
                if (imgReduced != null)
                    imgReduced.Dispose();
                if (region != null)
                    region = null;
                showSpinner(false);
            }
            return vi;
        }


        private void MouseUpMaskRegion()
        {
            HObject imgReduced = null, maskRegion = null, region = null;
            try
            {
                if (ADDING_ROT_ZONE)
                    return;

                hWinOCR.HalconWindow.ClearWindow();
                HOperatorSet.SmallestRectangle1(VariationROI, out HTuple t, out HTuple l, out HTuple b, out HTuple r);
                if (topStart < t)
                    topStart = t;
                if (leftStart < l)
                    leftStart = l;
                if (bottomStart > b)
                    bottomStart = b;
                if (rightStart > r)
                    rightStart = r;

                int[] vdecoordVAR = new int[] { topStart, leftStart, bottomStart, rightStart };
                if (!VDEReelConfig.VDEItemExists(vdecoordVAR, RotatedAngle, hWinOCR))
                {
                    HOperatorSet.GenRectangle1(out maskRegion, vdecoordVAR[0], vdecoordVAR[1], vdecoordVAR[2], vdecoordVAR[3]);
                    HOperatorSet.ReduceDomain(Backgroundimage, maskRegion, out imgReduced);
                    HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
                    hWinOCR.HalconWindow.SetLineWidth(3);
                    hWinOCR.HalconWindow.SetColor("firebrick");
                    VDEItem vi = new VDEItem(VDEReelConfig.GetNextID("Mask"), vdecoordVAR, RotatedAngle, VDEType.MASK);
                    VDEReelConfig.VDEItems.Add(vi);
                    topStart = 0;
                    leftStart = 0;
                    bottomStart = 0;
                    rightStart = 0;
                    hWinOCR.HMoveContent = true;
                    DoSummary();
                    drawVDEBorders();
                    ADDING_MASK = false;
                    cmdAddMask.Enabled = true;
                    cmdAddZone.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string err = "MouseUpOPZoneRegion() err: " + ex.Message;
                    MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
            finally
            {
                if (maskRegion != null)
                    maskRegion.Dispose();
                if (imgReduced != null)
                    imgReduced.Dispose();
                if (region != null)
                    region = null;
                showSpinner(false);
            }
        }

        private bool testForZoneGenerate(VDEItem vdi)
        {
            bool retVal = true;
            HObject tmpImg = null, imgZone = null, imgReduced = null, region = null;
            try
            {
                if (vdi.ODP.OPZoneIDFixture != null)
                    try { HOperatorSet.ClearShapeModel(vdi.ODP.OPZoneIDFixture); } catch { }
                vdi.ODP.OPZoneIDFixture = null;
                int imgCount = 1;
                HTuple numImgs = new HTuple(1);
                try { HOperatorSet.CountObj(TrainingImages, out numImgs); } catch { numImgs = 0; }
                imgCount = numImgs.I;
                HOperatorSet.GenRectangle1(out region, vdi.ODP.OPZoneVARRegion[0], vdi.ODP.OPZoneVARRegion[1], vdi.ODP.OPZoneVARRegion[2], vdi.ODP.OPZoneVARRegion[3]);
                for (int x = 1; x <= imgCount; x++)
                {
                    if (imgZone != null)
                        imgZone.Dispose();
                    if (tmpImg != null)
                        tmpImg.Dispose();
                    if (imgReduced != null)
                        imgReduced.Dispose();
                    HOperatorSet.CopyObj(TrainingImages, out imgReduced, x, 1);
                    HOperatorSet.CopyObj(TrainingImages, out tmpImg, x, 1);
                    if (x == 1)
                    {
                        bool checkOK = false;
                        checkOK = maskVDEByOpZone(ref imgReduced, x - 1, region, vdi.OpZoneName);
                        if (checkOK)
                            checkOK = findAndMaskMasks(ref imgReduced);
                        if (!checkOK)
                            return false;
                        HOperatorSet.ReduceDomain(imgReduced, region, out imgZone);
                        //HOperatorSet.AreaCenter(imgZone, out HTuple a, out HTuple r, out HTuple c);
                        vdi.ODP.OPZoneFixtureY = ((vdi.ODP.OPZoneVARRegion[2] - vdi.ODP.OPZoneVARRegion[0]) / 2) + vdi.ODP.OPZoneVARRegion[0];
                        vdi.ODP.OPZoneFixtureX = ((vdi.ODP.OPZoneVARRegion[3] - vdi.ODP.OPZoneVARRegion[1]) / 2) + vdi.ODP.OPZoneVARRegion[1];
                        HOperatorSet.CreateAnisoShapeModel(imgZone, Defaults.CreateAniso, Defaults.radMinus1Point5, Defaults.rad2, Defaults.rad0Point05, 1, 1, "auto", 0.98, 1.02, "auto", "auto", "use_polarity", "auto", "auto", out vdi.ODP.OPZoneIDFixture);
                        if (vdi.ODP.OPZoneIDFixture == null)
                        {
                            MessageBox.Show("It was not possible to create a variation model for " + vdi.OpZoneName);
                            return false;
                        }
                    }
                    HOperatorSet.FindAnisoShapeModel(tmpImg, vdi.ODP.OPZoneIDFixture, Defaults.radMinus1Point5, Defaults.rad2, 1, 1, 0.98, 1.02, 0.5, 1, 0.5, "least_squares", Defaults.NumLevelsFind, Defaults.Greediness, out HTuple row, out HTuple column, out HTuple angle, out HTuple scaleR, out HTuple scaleC, out HTuple FixtureScore);
                    double score = 0;
                    if (FixtureScore.Length > 0)
                        score = FixtureScore.D;
                    else
                    {
                        HOperatorSet.FindAnisoShapeModel(tmpImg, vdi.ODP.OPZoneIDFixture, Defaults.radMinus1Point5, Defaults.rad2, 0.9, 1, 0.9, 1.0, 0.0, 1, 0.5, "least_squares", 0, 0.9, out row, out column, out angle, out scaleR, out scaleC, out FixtureScore);
                        if (FixtureScore.Length > 0)
                            score = FixtureScore.D;
                    }
                    if (FixtureScore.Length == 0)
                        testErrors.Add("Could not find Shape-Model in Op-Zone: "+ vdi.OpZoneName + " test");
;                }
            }
            catch (Exception ex)
            {
                string err = "testForZoneGenerate() err: " + ex.Message;
                if (ex.Message.ToLower().Contains("wrong number"))
                    err = "testForZoneGenerate() err: " + ex.Message + Environment.NewLine + Environment.NewLine + "Try adjusting the Contrast level";
                MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                if (imgReduced != null)
                    imgReduced.Dispose();
                if (tmpImg != null)
                    tmpImg.Dispose();
                if (region != null)
                    region.Dispose();
                if (imgZone != null)
                    imgZone.Dispose();
            }
            return retVal;
        }

        private bool generateOPZone(VDEItem vdi)
        {
            bool retVal = true;
            HObject tmpImages = null, imgReduced = null, opzROI = null, opzZone = null, tmpImg = null, imgTransform = null;
            HObject imgTransReduced = null, rgnThreshold = null, tmp = null, rgnSelected = null; 
            HObject rgnConnectedThreshold = null, imgConst = null, imgPaint = null, rgnConnectedArea = null, imgCleared = null;
            HTuple HomMat2D = null;
            HTuple cl = 0;
            HTuple rw = 0;
            HTuple w = 0;
            HTuple h = 0;
            bool checkOK = false;
            try
            {
                HOperatorSet.CopyObj(TrainingImages, out tmpImages, 1, -1);
                if (vdi.ODP.OPVariationImages != null)
                    vdi.ODP.OPVariationImages.Dispose();
                HOperatorSet.GenEmptyObj(out vdi.ODP.OPVariationImages);
                int imgCount = 1;
                HTuple numImgs = new HTuple(1);
                try { HOperatorSet.CountObj(tmpImages, out numImgs); } catch { numImgs = 0; }
                imgCount = numImgs.I;

                HOperatorSet.GenRectangle1(out opzROI, vdi.ODP.OPZoneVARRegion[0], vdi.ODP.OPZoneVARRegion[1], vdi.ODP.OPZoneVARRegion[2], vdi.ODP.OPZoneVARRegion[3]);
                HOperatorSet.SmallestRectangle1(opzROI, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                w = c2 - c1;
                h = r2 - r1;
                try { HOperatorSet.ClearTrainDataVariationModel(vdi.ODP.OPZoneIDVar); } catch { }
                for (int x = 1; x <= imgCount; x++)
                {
                    checkOK = false;
                    if (imgReduced != null) imgReduced.Dispose();
                    if (opzZone != null) opzZone.Dispose();
                    if (tmpImg != null) tmpImg.Dispose();
                    if (imgTransform != null) imgTransform.Dispose();
                    if (imgTransReduced != null) imgTransReduced.Dispose();
                    if (rgnThreshold != null) rgnThreshold.Dispose();
                    if (tmp != null) tmp.Dispose();
                    if (rgnSelected != null) rgnSelected.Dispose();
                    if (rgnConnectedThreshold != null) rgnConnectedThreshold.Dispose();
                    if (imgConst != null) imgConst.Dispose();
                    if (imgPaint != null) imgPaint.Dispose();
                    if (rgnConnectedArea != null) rgnConnectedArea.Dispose();
                    if (imgCleared != null) imgCleared.Dispose();
                    HOperatorSet.CopyObj(tmpImages, out imgReduced, x, 1);
                    HOperatorSet.CopyObj(tmpImages, out tmpImg, x, 1);
                    if (x == 1)
                    {
                        checkOK = false;
                        checkOK = maskVDEByOpZone(ref imgReduced, x - 1, opzROI, vdi.OpZoneName);
                        HOperatorSet.ReduceDomain(imgReduced, opzROI, out opzZone);
                        HOperatorSet.AreaCenter(opzZone, out HTuple a, out HTuple r, out HTuple c);
                        vdi.ODP.OPZoneFixtureX = c;
                        vdi.ODP.OPZoneFixtureY = r;
                        HOperatorSet.CreateAnisoShapeModel(opzZone, Defaults.CreateAniso, Defaults.radMinus1Point5, Defaults.rad2, Defaults.rad0Point05, 1, 1, "auto", 0.99, 1.015, "auto", "auto", "use_polarity", "auto", "auto", out vdi.ODP.OPZoneIDFixture);
                        if (vdi.ODP.OPZoneIDFixture == null)
                        {
                            MessageBox.Show("It was not possible to create the variation model for " + vdi.OpZoneName);
                            return false;
                        }
                        HOperatorSet.FindAnisoShapeModel(tmpImg, vdi.ODP.OPZoneIDFixture, Defaults.radMinus1Point5, Defaults.rad2, 1, 1, 0.98, 1.02, 0.5, 1, 0.5, "least_squares", Defaults.NumLevelsFind, Defaults.Greediness, out HTuple row, out HTuple column, out HTuple angle, out HTuple scaleR, out HTuple scaleC, out HTuple FixtureScore);
                        vdi.ODP.OPZoneFixtureY =  row;
                        vdi.ODP.OPZoneFixtureX = column;
                        HOperatorSet.ReduceDomain(imgReduced, opzROI, out imgTransReduced);
                        HOperatorSet.CropDomain(imgTransReduced, out imgReduced);
                    }
                    else
                    {
                        HOperatorSet.FindAnisoShapeModel(tmpImg, vdi.ODP.OPZoneIDFixture, Defaults.radMinus1Point5, Defaults.rad2, 1, 1, 0.98, 1.02, 0.5, 1, 0.5, "least_squares", Defaults.NumLevelsFind, Defaults.Greediness, out HTuple row, out HTuple column, out HTuple angle, out HTuple scaleR, out HTuple scaleC, out HTuple FixtureScore);
                        double score = 0;
                        if (FixtureScore.Length > 0)
                            score = FixtureScore.D;
                        else
                        {
                            HOperatorSet.FindAnisoShapeModel(tmpImg, vdi.ODP.OPZoneIDFixture, Defaults.radMinus1Point5, Defaults.rad2, 0.9, 1, 0.9, 1.0, 0.0, 1, 0.5, "least_squares", 0, 0.9, out row, out column, out angle, out scaleR, out scaleC, out FixtureScore);
                            if (FixtureScore.Length > 0)
                                score = (double)FixtureScore.D;
                        }
                        if (score < Defaults.FixtureOPZoneScoreMin || column.Length == 0 || row.Length == 0)
                        {
                            MessageBox.Show(string.Format(vdi.VDEItemName + ": Insufficient foreground detail to create a variation model for image {0}.\nPlease note also that where possible Op-Zone regions should not have similar content as other Op-Zones on the label.\n\nFixture score: {1} (too low)", x.ToString(), Math.Round(score, 2)), "Label Training", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
                            HOperatorSet.DispObj(imgReduced, hWinOCR.HalconWindow);
                            hWinOCR.HalconWindow.SetColor("red");
                            HOperatorSet.DispObj(opzROI, hWinOCR.HalconWindow);
                            retVal = false;
                            return retVal;
                        }
                        else
                        {
                            cl = new HTuple(vdi.ODP.OPZoneFixtureX - column);
                            rw = new HTuple(vdi.ODP.OPZoneFixtureY - row);
                        }
                        HOperatorSet.HomMat2dIdentity(out HomMat2D);
                        HOperatorSet.HomMat2dScale(HomMat2D, 1 / scaleR, 1 / scaleC, row, column, out HomMat2D);
                        HOperatorSet.HomMat2dRotate(HomMat2D, -angle, row, column, out HomMat2D);
                        HOperatorSet.HomMat2dTranslate(HomMat2D, rw, cl, out HomMat2D);
                        HOperatorSet.AffineTransImage(imgReduced, out imgTransform, HomMat2D, "weighted", "false");
                        checkOK = false;
                        checkOK = maskVDEByOpZone(ref imgTransform, x - 1, opzROI, vdi.OpZoneName);
                        HOperatorSet.ReduceDomain(imgTransform, opzROI, out imgReduced);
                        if (checkOK)
                            checkOK = findAndMaskBarcode2D(ref imgReduced);
                        if (checkOK)
                            checkOK = findAndMaskBarcodeLinear(ref imgReduced);
                        if (checkOK)
                            checkOK = findAndMaskMasks(ref imgReduced);
                        if (!checkOK)
                            return false;
                        HOperatorSet.CropDomain(imgReduced, out imgReduced);
                    }
                    ResetSetFound();
                    try { HOperatorSet.ConcatObj(vdi.ODP.OPVariationImages, imgReduced, out vdi.ODP.OPVariationImages); } catch (Exception ex) { string e = ex.Message; }
                }
                HOperatorSet.GetImageSize(vdi.ODP.OPVariationImages, out w, out h);
                HOperatorSet.CreateVariationModel(w[0].I, h[0].I, "byte", "robust", out vdi.ODP.OPZoneIDVar);
                HOperatorSet.TrainVariationModel(vdi.ODP.OPVariationImages, vdi.ODP.OPZoneIDVar);
                HOperatorSet.GetVariationModel(out HObject result, out HObject varim, vdi.ODP.OPZoneIDVar);
                if(Defaults.DarkLabel==false)
                    HOperatorSet.PrepareVariationModel(vdi.ODP.OPZoneIDVar, Defaults.absThreshold50, Defaults.varThreshold); 
                else
                    HOperatorSet.PrepareVariationModel(vdi.ODP.OPZoneIDVar, Defaults.absThresholdDark, Defaults.varThreshold0);
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "generateOPZone() err: ";
                if (ex.Message.ToLower().Contains("wrong number"))
                    err = "generateOPZone() err: " + ex.Message + Environment.NewLine;
                else
                    err = err + ex.Message;
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    MessageBox.Show(this, err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
            }
            finally
            {
                showSpinner(false);
                if (tmpImages != null) tmpImages.Dispose();
                if (imgReduced != null) imgReduced.Dispose();
                if (opzROI != null) opzROI.Dispose();
                if (opzZone != null) opzZone.Dispose();
                if (tmpImg != null) tmpImg.Dispose();
                if (imgTransform != null) imgTransform.Dispose();
                if (imgTransReduced != null) imgTransReduced.Dispose();
                if (rgnThreshold != null) rgnThreshold.Dispose();
                if (tmp != null) tmp.Dispose();
                if (rgnSelected != null) rgnSelected.Dispose();
                if (rgnConnectedThreshold != null) rgnConnectedThreshold.Dispose();
                if (imgConst != null) imgConst.Dispose();
                if (imgPaint != null) imgPaint.Dispose();
                if (rgnConnectedArea != null) rgnConnectedArea.Dispose();
                if (imgCleared != null) imgCleared.Dispose();
            }
            return retVal;
        }

        private bool resizeEdge(int X, int Y, HObject region)
        {
            bool retVal = false;
            HTuple r1, c1, r2, c2;
            int resizeLimit = 100;
            try
            {
                HImage img = hWinOCR.HalconWindow.GetWindowBackgroundImage();
                HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                HOperatorSet.SmallestRectangle1(region, out r1, out c1, out r2, out c2);
                int activeRowPoint = (r2 - r1) / 8;
                int activeColumnPoint = (c2 - c1) / 8;

                //extend/decrease top row of fixture
                if (Y > r1 - resizeLimit && Y < r1 + resizeLimit)
                {
                    if (X > c1 + activeColumnPoint && X < c2 - activeColumnPoint)
                    {
                        retVal = true;
                        r1 = Y;
                        if (r1 < 0)
                            r1 = 0;
                    }
                }
                //extend/decrease bottom row of fixture
                if (Y > r2 - resizeLimit && Y < r2 + resizeLimit)
                {
                    if (X > c1 + activeColumnPoint && X < c2 - activeColumnPoint)
                    {
                        retVal = true;
                        r2 = Y;
                        if (r2 > h)
                            r2 = h;
                    }
                }
                //extend/decrease left of fixture
                if (X > c1 - resizeLimit && X < c1 + resizeLimit)
                {
                    if (Y > r1 + activeRowPoint && Y < r2 - activeRowPoint)
                    {
                        retVal = true;
                        c1 = X;
                        if (c1 < 0)
                            c1 = 0;
                    }
                }

                //extend/decrease right of fixture
                if (X > c2 - resizeLimit && X < c2 + resizeLimit)
                {
                    if (Y > r1 + activeRowPoint && Y < r2 - activeRowPoint)
                    {
                        retVal = true;
                        c2 = X;
                        if (c2 > w)
                            c2 = w;
                    }
                }
                if (retVal)
                {
                    if (EDITING_VARIATION)
                    {
                        if (VariationROI != null)
                            VariationROI.Dispose();
                        VariationROI = null;
                        hWinOCR.HalconWindow.SetColor("green");
                        hWinOCR.HalconWindow.SetLineWidth(4);
                        HOperatorSet.GenRectangle1(out VariationROI, r1, c1, r2, c2);
                        hWinOCR.HalconWindow.DispObj(VariationROI);
                        DisplayInfo(Defaults.VariationMessage, "top");
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "resizeEdge() err: " + ex.Message;
                MessageBox.Show(this, err);
                retVal = false;
            }
            return retVal;
        }


        private void MouseUpRemoveVDEItem(HMouseEventArgs e)
        {
            try
            {
                int r = Convert.ToInt32(e.Y);
                int c = Convert.ToInt32(e.X);

                List<VDEItem> vdeItems = getVDEItems(r, c);
                if (vdeItems.Count == 0)
                    return;

                for(int x = 0; x < vdeItems.Count;x++) //each (VDEItem vi in vdeItems.Where(x => x.IsVDE))
                {
                    DialogResult dr = MessageBox.Show("Remove VDE item " + vdeItems[x].VDEItemName + " from the configuration?", "Label Configuration", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        if (vdeItems[x].IsVDE)
                        {
                            hWinOCR.HalconWindow.ClearRectangle(vdeItems[x].VDERegion[0], vdeItems[x].VDERegion[1], vdeItems[x].VDERegion[2], vdeItems[x].VDERegion[3]);
                            VDEReelConfig.VDEItems.Remove(vdeItems[x]);
                            hWinOCR.HalconWindow.ClearWindow();
                            vdeItems[x].DestroyVars();
                            vdeItems[x] = null;
                            doCancelZoning();
                        }
                        else if (vdeItems[x].IsMask)
                        {
                            hWinOCR.HalconWindow.ClearRectangle(vdeItems[x].MaskedRegion[0], vdeItems[x].MaskedRegion[1], vdeItems[x].MaskedRegion[2], vdeItems[x].MaskedRegion[3]);
                            VDEReelConfig.VDEItems.Remove(vdeItems[x]);
                            doCancelZoning();
                        }
                        else if (vdeItems[x].IsOPZone)
                        {

                            // ensure all VDI items that were within this zone are deleted
                            for (int y = VDEReelConfig.VDEItems.Count - 1; y >= 0; y--)
                            {
                                VDEItem vi = VDEReelConfig.VDEItems[y];
                                if (vi.IsVDE)
                                {
                                    if (vi.OpZoneName.ToLower() == vdeItems[x].OpZoneName.ToLower())
                                    {
                                        hWinOCR.HalconWindow.ClearRectangle(vi.VDERegion[0], vi.VDERegion[1], vi.VDERegion[2], vi.VDERegion[3]);
                                        hWinOCR.HalconWindow.ClearWindow();
                                        VDEReelConfig.VDEItems.Remove(vi);
                                        vi.DestroyVars();
                                        vi = null;
                                        mnuClose.Enabled = false;
                                    }
                                }
                            }
                            hWinOCR.HalconWindow.ClearRectangle(vdeItems[x].ODP.OPZoneVARRegion[0], vdeItems[x].ODP.OPZoneVARRegion[1], vdeItems[x].ODP.OPZoneVARRegion[2], vdeItems[x].ODP.OPZoneVARRegion[3]);
                            VDEReelConfig.VDEItems.Remove(vdeItems[x]);
                            vdeItems[x].ODP.Clear();
                            vdeItems[x].DestroyVars();
                            vdeItems[x] = null;
                            doCancelZoning();

                        }
                    }
                }

 
                int countOpZones = VDEReelConfig.VDEItems.Where(x => x.IsOPZone).Count();
                if (countOpZones == 0)
                    cmdGenerate.Enabled = false;
                else
                    cmdGenerate.Enabled = true;
            }
            catch (Exception ex)
            {
                string err = "MouseUpRemoveVDEItem() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                DoSummary();
                drawVDEBorders();
                showSpinner(false);
            }
        }

        private VDEItem getVDEItem(int r, int c)
        {
            try
            {
                foreach (VDEItem vi in VDEReelConfig.VDEItems.Where(x => x.IsVDE))
                {
                    if (vi.VDERegion[0] < r)
                        if (vi.VDERegion[1] < c)
                            if (vi.VDERegion[2] > r)
                                if (vi.VDERegion[3] > c)
                                    return vi;
                }
                foreach (VDEItem vi in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
                {
                    if (vi.ODP.OPZoneVARRegion[0] < r)
                        if (vi.ODP.OPZoneVARRegion[1] < c)
                            if (vi.ODP.OPZoneVARRegion[2] > r)
                                if (vi.ODP.OPZoneVARRegion[3] > c)
                                    return vi;
                }
                foreach (VDEItem vi in VDEReelConfig.VDEItems.Where(x => x.IsMask))
                {
                    if (vi.MaskedRegion[0] < r)
                        if (vi.MaskedRegion[1] < c)
                            if (vi.MaskedRegion[2] > r)
                                if (vi.MaskedRegion[3] > c)
                                    return vi;
                }
            }
            catch (Exception ex)
            {
                string err = "getVDEItem() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return null;
        }

        private List<VDEItem> getVDEItems(int r, int c)
        {
            List<VDEItem> retVal = new List<VDEItem>();
            try
            {
                foreach (VDEItem vi in VDEReelConfig.VDEItems.Where(x => x.IsVDE))
                {
                    if (vi.VDERegion[0] < r)
                        if (vi.VDERegion[1] < c)
                            if (vi.VDERegion[2] > r)
                                if (vi.VDERegion[3] > c)
                                    retVal.Add(vi);
                }
                if (retVal.Count == 0)
                {
                    foreach (VDEItem vi in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
                    {
                        if (vi.ODP.OPZoneVARRegion[0] < r)
                            if (vi.ODP.OPZoneVARRegion[1] < c)
                                if (vi.ODP.OPZoneVARRegion[2] > r)
                                    if (vi.ODP.OPZoneVARRegion[3] > c)
                                        retVal.Add(vi);
                    }
                }
                foreach (VDEItem vi in VDEReelConfig.VDEItems.Where(x => x.IsMask))
                {
                    if (vi.MaskedRegion[0] < r)
                        if (vi.MaskedRegion[1] < c)
                            if (vi.MaskedRegion[2] > r)
                                if (vi.MaskedRegion[3] > c)
                                    retVal.Add(vi);
                }
            }
            catch (Exception ex)
            {
                string err = "getVDEItems() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return retVal;
        }

        private HTuple greyVal = 0;

        private void hWinOCR_HMouseMove(object sender, HMouseEventArgs e)
        {
            try
            {
                if (ADDING_ROT_ZONE)
                    return;

                double x = e.X;
                double y = e.Y;
                try { HalconDotNet.HOperatorSet.GetGrayval(mouseOverImage, e.Y, e.X, out greyVal); } catch { }
                if (greyVal != null)
                    try { uscPixelData1.pointData(Convert.ToInt32(e.X), Convert.ToInt32(e.Y), Convert.ToInt32(greyVal.D)); } catch { }
                else
                    try { uscPixelData1.pointData(Convert.ToInt32(e.X), Convert.ToInt32(e.Y), -1); } catch { }
                if (e.Button == MouseButtons.Left)
                {
                    if (EDITING_VARIATION)
                    {
                        mouseMoveVariation(x, y);
                        return;
                    }
                    else if (ADDING_OPZONES && cmdAddZone.Enabled == false)
                    {
                        mouseMoveAddingZone(x, y);
                        return;
                    }
                    else if (ADDING_MASK && cmdAddMask.Enabled == false)
                        mouseMoveAddingMask(x, y);
                    else if (ADDING_VDE)
                        mouseMoveAddingVDE(x, y);
                }
            }
            catch { }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
        }

        private void setMasking(bool enable)
        {
            if (cmdAddZone.InvokeRequired)
            {
                cmdAddZone.Invoke((MethodInvoker)delegate
                {
                    lblOP1.Enabled = enable;
                    cmdAddZone.Enabled = enable;
                    cmdAddMask.Enabled = enable;
                });
            }
            else
            {
                lblOP1.Enabled = enable;
                cmdAddZone.Enabled = enable;
                cmdAddMask.Enabled = enable;
            }
        }

        private bool rotateBackground(int angle, ref HObject img)
        {
            bool retVal = false;

            try
            {
                HOperatorSet.RotateImage(img, out img, angle * -1, "constant");
                retVal = true;
            }
            catch (Exception ex)
            {
                string err = "rotateBackground() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return retVal;
        }

        private bool rotateBackground(int angle, ref HObject imgIn, ref HObject imgOut)
        {
            bool retVal = false;
            try
            {
                if (imgOut != null)
                    imgOut.Dispose();
                HOperatorSet.RotateImage(imgIn, out imgOut, angle * -1, "constant");
                retVal = true;
            }
            catch (Exception ex)
            {
                string err = "rotateBackground() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return retVal;
        }

        private void hWinOCR_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (ADDING_ROT_ZONE)
                return;

            if (e.Button == MouseButtons.Left)
            {
                if (EDITING_VARIATION)
                {
                    mouseDownVariation(e);
                }
                else if (cmdAddZone.Enabled == false && ADDING_OPZONES == true)
                {
                    mouseDownAddingZone(e);
                }
                else if (cmdAddMask.Enabled == false && ADDING_MASK == true)
                {
                    mouseDownAddingMask(e);
                }
                else if (cmdAddMask.Enabled == false && ADDING_VDE == true)
                {
                    mouseDownAddingVDE(e);
                }
            }
        }

        private void mouseDownAddingZone(HMouseEventArgs e)
        {
            try
            {
                if (cmdAddZone.Enabled == true)
                {
                    hWinOCR.HMoveContent = true;
                    ADDING_OPZONES = false;
                    return; //user must start operation using start button
                }
                ADDING_OPZONES = true;
                DisplayInfo("", "top");
                hWinOCR.HalconWindow.SetLineWidth(4);
                hWinOCR.HalconWindow.SetColor("cyan");
                hWinOCR.HMoveContent = false;
                HTuple new_ls = new HTuple(3, 1);
                hWinOCR.HalconWindow.SetLineStyle(new_ls);
                //doSaveEnabled(false);
                lastX = e.X;
                lastY = e.Y;
                topStart = Convert.ToInt32(lastY);
                leftStart = Convert.ToInt32(lastX);
            }
            catch (Exception ex)
            {
                string err = "mouseDownAddingZone() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private void mouseDownAddingVDE(HMouseEventArgs e)
        {
            try
            {
                ADDING_VDE = true;
                DisplayInfo("Add VDE Manually", "top");
                hWinOCR.HalconWindow.SetLineWidth(3);
                hWinOCR.HalconWindow.SetColor("red");
                hWinOCR.HMoveContent = false;
                HTuple new_ls = new HTuple(3, 1);
                hWinOCR.HalconWindow.SetLineStyle(new_ls);
                lastX = e.X;
                lastY = e.Y;
                topStart = Convert.ToInt32(lastY);
                leftStart = Convert.ToInt32(lastX);
            }
            catch (Exception ex)
            {
                string err = "mouseDownAddingMask() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void mouseDownAddingMask(HMouseEventArgs e)
        {
            try
            {
                if (cmdAddMask.Enabled == true)
                {
                    hWinOCR.HMoveContent = true;
                    ADDING_MASK = false;
                    return; //user must start operation using start button
                }
                ADDING_MASK = true;
                DisplayInfo("", "top");
                hWinOCR.HalconWindow.SetLineWidth(3);
                hWinOCR.HalconWindow.SetColor("firebrick");
                hWinOCR.HMoveContent = false;
                HTuple new_ls = new HTuple(3, 1);
                hWinOCR.HalconWindow.SetLineStyle(new_ls);
                //doSaveEnabled(false);
                lastX = e.X;
                lastY = e.Y;
                topStart = Convert.ToInt32(lastY);
                leftStart = Convert.ToInt32(lastX);
            }
            catch (Exception ex)
            {
                string err = "mouseDownAddingMask() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void mouseDownVariation(HMouseEventArgs e)
        {
            try
            {
                if (WAITING_FOR_IMAGE_ONE)
                    return;

                ADDING_OPZONES = false;
                hWinOCR.HalconWindow.ClearWindow();
                if (EDITING_VARIATION)
                {
                    //hWinOCR.HalconWindow.SetColor("blue");
                    //if (labelFixture.FixtureROI == null)
                    //    labelFixture.Reset();
                    //labelFixture.FixtureROI.DispObj(hWinOCR.HalconWindow);
                    hWinOCR.HalconWindow.SetColor("green");
                    hWinOCR.HalconWindow.SetLineWidth(2);
                }
                hWinOCR.HMoveContent = false;
                HTuple new_ls = new HTuple(3, 1);
                hWinOCR.HalconWindow.SetLineStyle(new_ls);
                //doSaveEnabled(false);
                lastX = e.X;
                lastY = e.Y;
            }
            catch (Exception ex)
            {
                string err = "mouseDownVariation() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void mouseMoveVariation(double x, double y)
        {
            HObject region = null;
            try
            {
                if (WAITING_FOR_IMAGE_ONE)
                    return;

                HTuple r1 = lastY;
                HTuple r2 = y;
                HTuple c1 = lastX;
                HTuple c2 = x;
                hWinOCR.HalconWindow.ClearWindow();
                hWinOCR.HalconWindow.SetColor("blue");
                labelFixture.FixtureROI.DispObj(hWinOCR.HalconWindow);
                if (c2 > c1 && r2 > r1)
                {
                    HOperatorSet.SmallestRectangle1(labelFixture.FixtureROI, out HTuple rf1, out HTuple rc1, out HTuple rf2, out HTuple rc2);
                    HOperatorSet.GenRectangle1(out region, r1, c1, r2, c2);
                    if (r1 != rf1 && c1 != rc1)
                    {
                        hWinOCR.HalconWindow.SetColor("green");
                        region.DispObj(hWinOCR.HalconWindow);
                        region.Dispose();
                    }
                }
                if (region != null)
                    region.Dispose();
                region = null;
            }
            catch (Exception ex)
            {
                string err = "mouseMoveVariation() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
        }

        private void mouseMoveAddingZone(double x, double y)
        {
            HObject zoneROI = null;
            try
            {
                HTuple r1 = lastY;
                HTuple r2 = y;
                HTuple c1 = lastX;
                HTuple c2 = x;

                if (c2 > c1 && r2 > r1)
                {
                    ADDING_OPZONES = true;
                    hWinOCR.HalconWindow.ClearWindow();
                    hWinOCR.HMoveContent = false;
                    hWinOCR.HalconWindow.SetColor("orange red");
                    foreach (VDEItem vi in VDEReelConfig.VDEItems.Where(z => z.IsOPZone))
                    {
                        HOperatorSet.GenRectangle1(out HObject zone, vi.ODP.OPZoneVARRegion[0], vi.ODP.OPZoneVARRegion[1], vi.ODP.OPZoneVARRegion[2], vi.ODP.OPZoneVARRegion[3]);
                        hWinOCR.HalconWindow.DispObj(zone);
                        zone.Dispose();
                    }
                    //drawVDEBorders();
                    HOperatorSet.GenRectangle1(out zoneROI, r1, c1, r2, c2);
                    hWinOCR.HalconWindow.SetColor("cyan");
                    hWinOCR.HalconWindow.DispObj(zoneROI);
                    topStart = Convert.ToInt32(r1.D);
                    bottomStart = Convert.ToInt32(r2.D);
                    leftStart = Convert.ToInt32(c1.D);
                    rightStart = Convert.ToInt32(c2.D);
                }
            }
            catch (Exception ex)
            {
                string err = "mouseMoveAddingZone() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (zoneROI != null)
                    zoneROI.Dispose();
            }
        }

        private void mouseMoveAddingVDE(double x, double y)
        {
            HObject zoneROI = null;
            try
            {
                HTuple r1 = lastY;
                HTuple r2 = y;
                HTuple c1 = lastX;
                HTuple c2 = x;

                if (c2 > c1 && r2 > r1)
                {
                    ADDING_VDE = true;
                    hWinOCR.HalconWindow.ClearWindow();
                    hWinOCR.HMoveContent = false;
                    HOperatorSet.GenRectangle1(out zoneROI, r1, c1, r2, c2);
                    hWinOCR.HalconWindow.SetColor("red");
                    hWinOCR.HalconWindow.DispObj(zoneROI);
                    topStart = Convert.ToInt32(r1.D);
                    bottomStart = Convert.ToInt32(r2.D);
                    leftStart = Convert.ToInt32(c1.D);
                    rightStart = Convert.ToInt32(c2.D);
                }
            }
            catch (Exception ex)
            {
                string err = "mouseMoveAddingVDE() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (zoneROI != null)
                    zoneROI.Dispose();
            }
        }

        private void mouseMoveAddingMask(double x, double y)
        {
            HObject zoneROI = null;
            try
            {
                HTuple r1 = lastY;
                HTuple r2 = y;
                HTuple c1 = lastX;
                HTuple c2 = x;

                if (c2 > c1 && r2 > r1)
                {
                    ADDING_MASK = true;
                    hWinOCR.HalconWindow.ClearWindow();
                    hWinOCR.HMoveContent = false;
                    HOperatorSet.GenRectangle1(out zoneROI, r1, c1, r2, c2);
                    hWinOCR.HalconWindow.SetColor("firebrick");
                    hWinOCR.HalconWindow.DispObj(zoneROI);
                    topStart = Convert.ToInt32(r1.D);
                    bottomStart = Convert.ToInt32(r2.D);
                    leftStart = Convert.ToInt32(c1.D);
                    rightStart = Convert.ToInt32(c2.D);
                }
            }
            catch (Exception ex)
            {
                string err = "mouseMoveAddingMask() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (zoneROI != null)
                    zoneROI.Dispose();
            }
        }

        private void hwinImage_Load(object sender, EventArgs e)
        {
            //winCTRL = 0;
            this.MouseWheel -= h_MouseWheel;
            this.MouseWheel += h_MouseWheel;
        }

        private void h_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - hWinOCR.Location.X, e.Y - hWinOCR.Location.Y, e.Delta);
                hWinOCR.HSmartWindowControl_MouseWheel(sender, newe);

                //if (winCTRL == 2)
                //{
                //    MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - hWinComplement.Location.X, e.Y - hWinComplement.Location.Y, e.Delta);
                //    hWinComplement.HSmartWindowControl_MouseWheel(sender, newe);
                //}
                //if (winCTRL == 3)
                //{
                //    MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks, e.X - hWinComplement1.Location.X, e.Y - hWinComplement1.Location.Y, e.Delta);
                //    hWinComplement1.HSmartWindowControl_MouseWheel(sender, newe);
                //}
            }
            catch { }
        }


        private void IO_INTERRUPT_Handler(int channel, IOEventArgs e)
        {
            try
            {
                if (PLC_IS_REWINDING == false)
                    return;
                //if (cmdAddImage.InvokeRequired)
                //{
                //    cmdAddImage.Invoke((MethodInvoker)delegate
                //    {
                //        PLC_IS_REWINDING = false;
                //        cmdAddImage.Enabled = true;
                //        if (CameraManager.NectaCameras[0].nectaCam.Acquire == false)
                //            CameraManager.NectaCameras[0].nectaCam.Acquire = true;
                //    });
                //}
                //else
                //{
                    PLC_IS_REWINDING = false;
                    if (CameraManager.NectaCameras[0].nectaCam.Acquire == false)
                        CameraManager.NectaCameras[0].nectaCam.Acquire = true;
                //}
                
                showSpinner(false);
            }
            catch (Exception ex)
            {
                string err = "IO_INTERRUPT_Handler() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
        }

        private void frmLabelConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                iParamDefaults = DataManager.GeInspectionParamDefaults();
                iParams = new InspectionParams();
                iParams.LoadDefaults(iParamDefaults);

                UtilityFunctions.SetSystemParams();
                oldLineStyle = hWinOCR.HalconWindow.GetLineStyle();
                hWinOCR.HMoveContent = false;
                if (labelFixture.FixtureROI != null)
                    labelFixture.FixtureROI.Dispose();
                if (VariationROI != null)
                    VariationROI.Dispose();

                HOperatorSet.GenEmptyObj(out labelFixture.FixtureROI);
                HOperatorSet.GenEmptyObj(out VariationROI);
                setWindowLineStyle("green");

                mxClient.WriteToRegister(1, "Capture_Image", 1, 3);
                mxClient.WriteToRegister(1, "Capture_Image", 0, 3);
                mxClient.WriteToRegister(1, "Mode_Inspect", 0, 3);
                mxClient.WriteToRegister(1, "Mode_Manual", 0, 3);
                mxClient.WriteToRegister(1, "Mode_Teach", 1, 3);
                mxClient.WriteToRegister(1, "Test_Image_Count", testImageCount, 3);
                LoadEvents();
                setHalconSystemParams();
                VDEReelConfig.VDEItems.Clear();
                VDEReelConfig.VDEItems = new List<VDEItem>();
            }
            catch (Exception ex)
            {
                string err = "frmLabelConfiguration_Load() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
        }

        private void setHalconSystemParams()
        {
            HOperatorSet.SetSystem("clip_region", "true");
            HOperatorSet.SetSystem("int_zooming", "false");
            HOperatorSet.SetSystem("pregenerate_shape_models", "false");
            HOperatorSet.GetSystem("clip_region", out HTuple TmpCtrl_ClipRegion);
        }

        public void LoadEvents()
        {
            SYSTEM_IO.IO_INTERRUPT_Handler -= IO_INTERRUPT_Handler;
            SYSTEM_IO.IO_INTERRUPT_Handler += IO_INTERRUPT_Handler;
            SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
            SYSTEM_IO.IO_CHANGE_Handler += IO_COS_Handler;
        }

        public void UnloadEvents()
        {
            SYSTEM_IO.IO_INTERRUPT_Handler -= IO_INTERRUPT_Handler;
            SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
        }

        public void UnloadData()
        {
            try
            {
                if (labelFixture.FixtureROI != null)
                    labelFixture.FixtureROI.Dispose();
                if (VariationROI != null)
                    VariationROI.Dispose();
                if (VariationImages != null)
                    VariationImages.Dispose();
                if (Backgroundimage != null)
                    Backgroundimage.Dispose();
                VDEReelConfig.ClearDataItems(hWinOCR);
                if (File.Exists(TMP_FOLDER))
                    ImageData.DeleteTrainingData(TMP_FOLDER);
                ImageData.CreateTempFilepath(REEL_LPN);
                VDEReelConfig.VDEItems.Clear();
                VDEReelConfig.VDEItems = new List<VDEItem>();

            }
            catch (Exception ex)
            {
                string err = "UnloadData() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
        }

        private void setWindowLineStyle(string color)
        {
            HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
            HOperatorSet.SetLineWidth(hWinOCR.HalconWindow, 4);
            hWinOCR.HalconWindow.SetColor(color);
            EDITING_VARIATION = true;
            mnuRestart.Enabled = true;
        }


        private void saveVariation()
        {
            cropDomainToVariationModelBorder();
            if (VariationROI == null)
                return;
            setMasking(true);
            EDITING_VARIATION = false;
            VARIATION_COMPLETE = true;
            setMasking(true);
        }

        private void cropDomainToVariationModelBorder()
        {
            HObject imgReduced = null;
            try
            {
                if (Backgroundimage != null)
                    Backgroundimage.Dispose();
                HOperatorSet.CopyObj(TrainingImages, out Backgroundimage, 1, 1);

                HOperatorSet.ReduceDomain(Backgroundimage, VariationROI, out imgReduced);
                if (imgReduced != null)
                    if (imgReduced.CountObj() > 0)
                    {
                        HOperatorSet.DetachBackgroundFromWindow(hWinOCR.HalconWindow);
                        HOperatorSet.AttachBackgroundToWindow(imgReduced, hWinOCR.HalconWindow);
                    }
            }
            catch (Exception ex)
            {
                string err = "cropDomainToVariationModelBorder() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (imgReduced != null)
                    imgReduced.Dispose();
            }
        }


        private void deleteAllLabelData()
        {
            DataManager.DeleteTrainingData(LABEL_ITEM);
        }

        private void saveAllTrainingData()
        {
            TrainingDataType tdt = TrainingDataType.BARCODE;
            saveTrainingData(tdt);
            tdt = TrainingDataType.LABEL;
            saveTrainingData(tdt);
            tdt = TrainingDataType.MASK;
            saveTrainingData(tdt);
            tdt = TrainingDataType.OPZONE;
            saveTrainingData(tdt);
            tdt = TrainingDataType.VDE;
            saveTrainingData(tdt);
            //tdt = TrainingDataType.ROT;
            //saveTrainingData(tdt);
        }

        private bool saveTrainingData(TrainingDataType tdt)
        {
            bool retVal = false;
            ITrainingData td = new TrainingData(lblLIN.Text);
            try
            {
                List<string> data = new List<string>();
                string lin = this.lblLIN.Text;

                switch (tdt)
                {
                    case TrainingDataType.BARCODE:
                        for (int x = 0; x < VDEReelConfig.VDEItems.Count; x++)
                            if (VDEReelConfig.VDEItems[x].IsBarcode2D || VDEReelConfig.VDEItems[x].IsBarcodeLinear)
                            {
                                data = VDEReelConfig.VDEItems[x].CalculateBarcodeData();
                                DataManager.SaveTrainingData(LABEL_ITEM, TrainingDataType.BARCODE, data);
                            }
                        break;
                    case TrainingDataType.MASK:
                        for (int x = 0; x < VDEReelConfig.VDEItems.Count; x++)
                            if (VDEReelConfig.VDEItems[x].IsMask)
                            {
                                data = VDEReelConfig.VDEItems[x].CalculateMaskRegion();
                                DataManager.SaveTrainingData(LABEL_ITEM, TrainingDataType.MASK, data);
                            }
                        break;
                    case TrainingDataType.OPZONE:
                        for (int x = 0; x < VDEReelConfig.VDEItems.Count; x++)
                            if (VDEReelConfig.VDEItems[x].IsOPZone)
                            {
                                data = VDEReelConfig.VDEItems[x].CalculateOverPrintData();
                                DataManager.SaveTrainingData(LABEL_ITEM, TrainingDataType.OPZONE, data);
                            }
                        break;
                    case TrainingDataType.VDE:
                        for (int x = 0; x < VDEReelConfig.VDEItems.Count; x++)
                            if (VDEReelConfig.VDEItems[x].IsVDE)
                            {
                                data = VDEReelConfig.VDEItems[x].CalculateVDEData();
                                DataManager.SaveTrainingData(LABEL_ITEM, TrainingDataType.VDE, data);
                            }
                        break;
                }
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "saveTrainingData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Save Label Training Data Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                MessageBox.Show(this, err);
            }
            return retVal;
        }

        public void createLegend()
        {
            //return;
            HObject meanVarImage = null, varImage = null, tmpObj = null, region = null, rgn = null;
            try
            {
                hWinOCR.HalconWindow.ClearWindow();
                HOperatorSet.SmallestRectangle1(VariationROI, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                HOperatorSet.GenRectangle1(out region, r1, c1, r2, c2);
                HOperatorSet.GenEmptyObj(out varImage);

                varImage = TestImages.SelectObj(1);

                HOperatorSet.ReduceDomain(varImage, VariationROI, out tmpObj);
                HOperatorSet.PaintRegion(region, tmpObj, out tmpObj, 220, "fill");
                try { HOperatorSet.DetachBackgroundFromWindow(hWinOCR.HalconWindow); } catch (Exception ex) { string err = ex.Message; }
                try { HOperatorSet.AttachBackgroundToWindow(tmpObj, hWinOCR.HalconWindow); } catch (Exception ex) { string err = ex.Message; }
                hWinOCR.SetFullImagePart();

                //tmpObj.DispObj(hWinOCR.HalconWindow);
                hWinOCR.HalconWindow.SetLineWidth(4);
                HTuple new_ls = new HTuple(1, 1);
                hWinOCR.HalconWindow.SetLineStyle(new_ls);


                hWinOCR.HalconWindow.SetColor("red");
                HOperatorSet.SetDraw(hWinOCR.HalconWindow, "fill");
                foreach (VDEItem vde in VDEReelConfig.VDEItems)
                {
                    if (vde.IsVDE)
                    {
                        HOperatorSet.GenRectangle1(out rgn, vde.VDERegion[0], vde.VDERegion[1], vde.VDERegion[2], vde.VDERegion[3]);
                        rgn.DispObj(hWinOCR.HalconWindow);
                        if (rgn != null)
                            rgn.Dispose();
                    }
                }
                hWinOCR.HalconWindow.SetColor("magenta");
                foreach (VDEItem vde in VDEReelConfig.VDEItems)
                {
                    if (vde.IsBarcode2D || vde.IsBarcodeLinear)
                    {
                        HOperatorSet.GenRectangle1(out rgn, vde.BarcodeRegion[0], vde.BarcodeRegion[1], vde.BarcodeRegion[2], vde.BarcodeRegion[3]);
                        rgn.DispObj(hWinOCR.HalconWindow);
                        if (rgn != null)
                            rgn.Dispose();
                    }
                }
                hWinOCR.HalconWindow.SetColor("firebrick");
                foreach (VDEItem vde in VDEReelConfig.VDEItems)
                {
                    if (vde.IsMask)
                    {
                        HOperatorSet.GenRectangle1(out rgn, vde.MaskedRegion[0], vde.MaskedRegion[1], vde.MaskedRegion[2], vde.MaskedRegion[3]);
                        rgn.DispObj(hWinOCR.HalconWindow);
                        if (rgn != null)
                            rgn.Dispose();
                    }
                }

                hWinOCR.HalconWindow.SetColor("cyan");
                HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
                foreach (VDEItem vde in VDEReelConfig.VDEItems)
                {
                    if (vde.IsOPZone)
                    {
                        int len = (vde.VDEItemName.Length * 15) / 2;
                        hWinOCR.HalconWindow.DispText(vde.VDEItemName, "image", (((vde.ODP.OPZoneVARRegion[2] - vde.ODP.OPZoneVARRegion[0]) / 2)) + (vde.ODP.OPZoneVARRegion[0] - 40), (((vde.ODP.OPZoneVARRegion[3] - vde.ODP.OPZoneVARRegion[1]) / 2)) + (vde.ODP.OPZoneVARRegion[1] - len), "black", "box", "false");
                        HOperatorSet.GenRectangle1(out rgn, vde.ODP.OPZoneVARRegion[0], vde.ODP.OPZoneVARRegion[1], vde.ODP.OPZoneVARRegion[2], vde.ODP.OPZoneVARRegion[3]);
                        rgn.DispObj(hWinOCR.HalconWindow);
                        if (rgn != null)
                            rgn.Dispose();
                    }
                }

                HObject tmpImg = hWinOCR.HalconWindow.DumpWindowImage();
                UtilityFunctions.RemoveBackground(ref tmpImg);
                string IMAGE_FOLDER = ImageData.CreateImageFilepath();
                string legendFile = Path.Combine(IMAGE_FOLDER, LABEL_ITEM + "_legendkey.png");
                if (File.Exists(legendFile))
                    File.Delete(legendFile);
                HOperatorSet.WriteImage(tmpImg, "png", 0, legendFile);
                if (tmpImg != null)
                    tmpImg.Dispose();
            }
            catch (Exception ex)
            {
                string err = "createLegend() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Training Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                MessageBox.Show(this, err);
            }
            finally
            {
                if (rgn != null)
                    rgn.Dispose();
                if (region != null)
                    region.Dispose();
                if (varImage != null)
                    varImage.Dispose();
                if (meanVarImage != null)
                    meanVarImage.Dispose();
                if (tmpObj != null)
                    tmpObj.Dispose();
            }
        }

        private void createLayout()
        {
            HObject rgn = null, tmpVar = null;
            try
            {
                //drawVDEBorders();

                hWinOCR.HalconWindow.ClearWindow();
                hWinOCR.HalconWindow.DetachBackgroundFromWindow();
                if (mouseOverImage != null)
                    mouseOverImage.Dispose();
                HOperatorSet.SelectObj(TestImages, out mouseOverImage, 1);
                int[] varCoords = DataManager.GetVariationRegion(Defaults.StationID, LABEL_ITEM);

                if (VariationROI.IsInitialized())
                {
                    HOperatorSet.ReduceDomain(mouseOverImage, VariationROI, out mouseOverImage);
                }
                else if (varCoords != null) 
                {
                    HOperatorSet.GenRectangle1(out tmpVar, varCoords[0], varCoords[1], varCoords[2], varCoords[3]);
                    HOperatorSet.ReduceDomain(tmpVar, tmpVar, out mouseOverImage);
                }
                HOperatorSet.AttachBackgroundToWindow(mouseOverImage, hWinOCR.HalconWindow);
                try { hWinOCR.SetFullImagePart(); } catch (Exception ex) { string err = ex.Message; }
                drawVDEBorders();

                try
                {
                    HObject tmpImg = hWinOCR.HalconWindow.DumpWindowImage();
                    UtilityFunctions.RemoveBackground(ref tmpImg);
                    string IMAGE_FOLDER = ImageData.CreateImageFilepath();
                    string outputfile = Path.Combine(IMAGE_FOLDER, LABEL_ITEM + "_layout.png");
                    if (File.Exists(outputfile))
                        File.Delete(outputfile);
                    HOperatorSet.WriteImage(tmpImg, "png", 0, outputfile);
                }
                catch (Exception ex)
                {
                    string err = "createLayout() err: " + ex.Message;
                    MessageBox.Show(this, err);
                }
            }
            catch (Exception ex)
            {
                string err = "createLayout() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (tmpVar != null)
                    tmpVar.Dispose();
                if (rgn != null)
                    rgn.Dispose();
            }
        }

        private bool doClose()
        {
            if (VARIATION_COMPLETE)
            {
                ESignature es = new ESignature(Enums.ESigReason.SaveTraining, Enums.Roles.LVSIII_USer, "Label Configuration Complete");
                es.AuthenticationOnly = true;
                es.ReasonRequired = false;
                es.UseLastReason = false;
                es.DoNotAcceptLastUser = false;
                es.CaptureSignature();
                if (es.SignatureAccepted)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    this.Cursor = Cursors.WaitCursor;
                    createLegend();
                    createLayout();
                    deleteAllLabelData();
                    if (SaveTrainedLabel(es.LastUserName))
                    {
                        saveAllTrainingData();
                        UnloadData();
                        Cursor.Current = Cursors.WaitCursor;
                        this.Cursor = Cursors.WaitCursor;
                        ImageData.DeleteTempFolderData(TMP_FOLDER);
                        return true;
                    }
                    else
                        return false;
                }
            }
            else
                MessageBox.Show("Not enough data to save this label configuration\n\nIf you wish to exit without saving please use the Menu option:\nFile =-> 'Cancel Setup'");
            return false;
        }

        public bool SaveTrainedLabel(string usersaving)
        {
            bool retVal = true;
            HObject tmpObj = null, conRegions = null, regionsShape = null;
            SystemMessageEventArgs smea = null;
            try
            {
                int camGain = CameraManager.GetGainFromCameras();
                DataManager.SaveLabelData(INSPECT_LIGHT_AREAS, Convert.ToInt32(iParams.DebrisMinSize), Defaults.StationID, LABEL_ITEM, VariationROI, InnerRadius, camGain, LabelTYPE);
                int labelID = DataManager.LabelID(Defaults.StationID, LABEL_ITEM);
                iParams.LabelID = labelID;
                DataManager.SaveInspectionParams(iParams);

                smea = new SystemMessageEventArgs("Complete. Saving inspection parameters..", "Label Training", (int)Enums.CriticalLevels.Black);
                if (uscMD != null)
                    uscMD.SystemMessage(smea);

                DataManager.InspectionParamsUpdateVDEItemsDeleteAll(labelID);
                foreach (VDEItem vdi in VDEReelConfig.VDEItems)
                {
                    smea = new SystemMessageEventArgs("Complete. Saving VDE data: " + vdi.VDEItemName + "...", "Label Training", (int)Enums.CriticalLevels.Black);
                    if (uscMD != null)
                        uscMD.SystemMessage(smea);

                    vdi.ReelLPN = REEL_LPN;
                    if (vdi.IsVDE)
                        DataManager.InsertInspectionParamsVDEItem(vdi, vdi.VDERegion, labelID);
                    else if (vdi.IsBarcode2D || vdi.IsBarcodeLinear)
                        DataManager.InsertInspectionParamsVDEItem(vdi, vdi.BarcodeRegion, labelID);
                    else if (vdi.IsMask)
                        DataManager.InsertInspectionParamsVDEItem(vdi, vdi.MaskedRegion, labelID);
                }
                DataManager.InspectionParamsUpdateOPZonesDeleteAll(labelID);
                foreach (VDEItem vdi in VDEReelConfig.VDEItems)
                    if (vdi.IsOPZone)
                    {
                        smea = new SystemMessageEventArgs("Complete. Saving Op-Zone data for " + vdi.VDEItemName + "...", "Label Training", (int)Enums.CriticalLevels.Black);
                        if (uscMD != null)
                            uscMD.SystemMessage(smea);
                        if (Defaults.DarkLabel == false)
                            HOperatorSet.PrepareVariationModel(vdi.ODP.OPZoneIDVar, Defaults.absThreshold50, Defaults.varThreshold);
                        else
                            HOperatorSet.PrepareVariationModel(vdi.ODP.OPZoneIDVar, Defaults.absThresholdDark, Defaults.varThreshold0);

                        if (IsSmallDebrisSizeVar)
                            vdi.DarkMinSizeVAR = 50;
                        else
                            vdi.DarkMinSizeVAR = 100;
                        DataManager.InspectionParamsUpdateOPZone(labelID, vdi, TMP_FOLDER);
                    }
                DataManager.SaveSummaryDataStart(REEL_LPN, LABEL_ITEM, usersaving, Defaults.StationID, startedAt);
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "SaveTrainedLabel() err: " + ex.Message;
                MessageBox.Show(this, err);
            }
            finally
            {
                if (regionsShape != null)
                    regionsShape.Dispose();
                if (conRegions != null)
                    conRegions.Dispose();
                if (tmpObj != null)
                    tmpObj.Dispose();
            }
            return retVal;
        }

        private void mnuCancel_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            dr = MessageBox.Show("Cancel label training now?", "Label Setup", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes)
                return;

            mxClient.WriteToRegister(1, "Mode_Teach", 0, 3);

            variationImageCount = 0;
            formClosing = true;
            ESignature es = new ESignature(Enums.ESigReason.CancelLabelTraining, Enums.Roles.LVSIII_USer, "Label Configuration Cancelled");
            es.AuthenticationOnly = true;
            es.ReasonRequired = true;
            es.UseLastReason = false;
            es.DoNotAcceptLastUser = false;
            es.CanCancel = true;
            es.CaptureSignature();
            if (es.SignatureAccepted)
            {
                showSpinner(false);
                UnloadData();
                DataManager.SaveAction("Training Cancelled", "Label Training", REEL_LPN, es.LastUserName, "mnuCancel_Click()", es.ErrorDescription, es.UserReason);
                Hide();
            }
        }

        public void showSpinner(bool showSpinning)
        {
            try
            {
                Invoke((MethodInvoker)delegate
                {
                    if (!showSpinning)
                    {
                        try { this.Cursor = Cursors.Default; } catch { }
                        try { Cursor.Current = Cursors.Default; } catch { }
                    }
                    else
                    {
                        try { this.Cursor = Cursors.AppStarting; } catch { }
                        try { Cursor.Current = Cursors.AppStarting; } catch { }

                    }
                    this.Refresh();
                    this.Invalidate();
                });
            }
            catch
            {
            }
        }


        private void hWinOCR_HMouseDoubleClick(object sender, HMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {                    
                    DoSummary();
                    drawVDEBorders();
                    return;
                }
                if (e.Button == MouseButtons.Left)
                {                    
                    if (EDITING_VARIATION)
                        return;
                    else if (ADDING_OPZONES)
                        return;
                    else if (ADDING_MASK)
                        return;
                    else
                    {                        
                        VDEItem opZoneItem = getVDEItem(Convert.ToInt32(e.Y), Convert.ToInt32(e.X));
                        if (opZoneItem != null)
                        {                            
                            if (opZoneItem.IsOPZone)
                            {                                
                                if (UpdateOpZoneThreshold(opZoneItem))
                                {                                    
                                    ADDING_OPZONES = false;
                                    runTest(imageIndex, opZoneItem.VDEItemName, INSPECT_LIGHT_AREAS);
                                    //if (generateOPZone(opZoneItem) == false)
                                    //{
                                    //    DisplayInfo("Op-Zone fixture could not be found; please adjust settings", "top");
                                    //    return;
                                    //}
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "hWinOCR_HMouseDoubleClick() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private bool UpdateOpZoneThreshold(VDEItem vdi)
        {
            bool retVal = false;
            HObject tmpImg = null;
            try
            {
                if (vdi.IsOPZone)
                {
                    int t = 0;
                    int l = 0;
                    int b = 0;
                    int r = 0;

                    t = vdi.ODP.OPZoneVARRegion[0];
                    l = vdi.ODP.OPZoneVARRegion[1];
                    b = vdi.ODP.OPZoneVARRegion[2];
                    r = vdi.ODP.OPZoneVARRegion[3];
                    // mouse was double-clicked within an op zone
                    HOperatorSet.CopyImage(vdi.ODP.DomainImage, out tmpImg);
                    frmEditVariation frmEV = new frmEditVariation(tmpImg, false, INSPECT_LIGHT_AREAS, InnerRadius, vdi.DarkMaxGray, iParamDefaults, LabelTYPE);
                    frmEV.ShowDialog();
                    if (frmEV.CANCELLED == false)
                    {
                        vdi.DarkMaxGray = frmEV.DarkMaxGray;
                        if (testForZoneGenerate(vdi) == true)
                            retVal = true;
                        else
                            retVal = false;
                    }
                    else
                        DisplayInfo("", "top");

                    frmEV.Close();
                    DoSummary();
                    drawVDEBorders();
                    return retVal;
                }
                retVal = true;
                return retVal;
            }
            catch (Exception ex)
            {
                string err = "UpdateOpZoneThreshold() err: " + ex.Message;
                MessageBox.Show(err, "Label Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                if (tmpImg != null)
                    tmpImg.Dispose();
            }
            return retVal;
        }

        private void doRestart()
        {
            try
            {
                enableBackForward(false);
                setMasking(false);
                mnuRunTestRT.Enabled = false;
                cmdGenerate.Enabled = false;
                mnuRestart.Enabled = false;
                if (Backgroundimage != null)
                    Backgroundimage.Dispose();
                HOperatorSet.CopyObj(TrainingImages, out Backgroundimage, 1, 1);
                hWinOCR.HalconWindow.DetachBackgroundFromWindow();
                hWinOCR.HalconWindow.ClearWindow();
                HOperatorSet.AttachBackgroundToWindow(Backgroundimage, hWinOCR.HalconWindow);
                if (VariationROI != null)
                    VariationROI.Dispose();
                VariationROI = null;
                if (labelFixture != null)
                    labelFixture.Reset();
                if (oldLineStyle != null)
                    oldLineStyle.Dispose();
                oldLineStyle = null;
                VDEReelConfig.ClearDataItems(hWinOCR);
                VDEReelConfig.VDEItems.Clear();
                VDEReelConfig.VDEItems = new List<VDEItem>();
                foreach (Control ctrl in fplvdeitems.Controls)
                {
                    if (ctrl is uscVDEItem)
                    {
                        uscVDEItem uvde = (uscVDEItem)ctrl;
                        uvde.Reset();
                    }
                }
                EDITING_VARIATION = true;
                DisplayInfo("Please define a new Label Fixture Area", "top");
                HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
                HOperatorSet.SetColor(hWinOCR.HalconWindow, "cyan");
                HOperatorSet.SetLineWidth(hWinOCR.HalconWindow, 3);
                DoSummary();
                showSpinner(false);
                hWinOCR.SetFullImagePart();
            }
            catch (Exception ex) { MessageBox.Show("Errors occured attempting to re-start:\n" + ex.Message + "\n\nPlease close and re-start this setup session"); }
        }


        private void closing()
        {
            try
            {
                CameraManager.NectaCameras[0].SetChannelMethodCallerFalse();
                if (VariationROI != null)
                    VariationROI.Dispose();
                if (VariationImages != null)
                    VariationImages.Dispose();
                if (TrainingImages != null)
                    TrainingImages.Dispose();
                if (TestImages != null)
                    TestImages.Dispose();
                if (Backgroundimage != null)
                    Backgroundimage.Dispose();
                if (listFSS != null)
                    listFSS.Clear();
                if (labelFixture != null)
                    labelFixture = null;
                if (oldLineStyle != null)
                    oldLineStyle.Dispose();
                VDEReelConfig.ClearDataItems(hWinOCR);
                UnloadEvents();
                showSpinner(false);
            }
            catch { }
        }


        private void roundTopLabel2_Click(object sender, EventArgs e)
        {
            DoSummary();
            drawVDEBorders();
            StringBuilder sb = new StringBuilder();
            sb.Append(lblVDE.Text).Append(Environment.NewLine);
            sb.Append(lblOP.Text).Append(Environment.NewLine);
            sb.Append(lblBarcode.Text).Append(Environment.NewLine);
            sb.Append(lblMasking.Text).Append(Environment.NewLine);
            Clipboard.SetText(sb.ToString());
            MessageBox.Show("VDE data has been copied to the Clipboard");
        }

        private void doSaveMask()
        {
            HObject tmpImg = null;
            try
            {
                MouseUpMaskRegion();
                ADDING_MASK = false;
                DoSummary();
                drawVDEBorders();
            }
            catch (Exception ex)
            {
                string err = "doSaveMask() err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (tmpImg != null)
                    tmpImg.Dispose();
            }
        }

        private bool doSaveOpZone()
        {
            bool retVal = false;
            HObject tmpImg = null;
            try
            {
                VDEItem opzoneItem = MouseUpOPZoneRegion();
                if (opzoneItem != null)
                {
                    opzoneItem.DestroyVars();
                    HOperatorSet.CopyImage(opzoneItem.ODP.DomainImage, out tmpImg);

                    frmEditVariation frmEV = new frmEditVariation(tmpImg, false, INSPECT_LIGHT_AREAS, InnerRadius, DARK_MAX_GRAY, iParamDefaults, LabelTYPE);
                    frmEV.ShowDialog();
                    DisplayInfo("testing OpZone fixture... ", "top");
                    if (frmEV.CANCELLED == false)
                    {
                        opzoneItem.DarkMaxGray = frmEV.DarkMaxGray;
                        bool canGenerate = testForZoneGenerate(opzoneItem);
                        if (canGenerate)
                        {
                            //cmdLabelZone.Enabled = true;
                            ADDING_OPZONES = false;
                            //assignRotZoneToOpzone(opzoneItem);
                            retVal = true;
                        }
                    }
                    frmEV.Close();
                    frmEV = null;
                    DoSummary();
                    drawVDEBorders();
                }
                setMasking(true);
            }
            catch (Exception ex)
            {
                string err = "doSaveOpZone() err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (tmpImg != null)
                    tmpImg.Dispose();
                DisplayInfo("", "top");
            }
            return retVal;
        }


        private void cmdAddZone_Click(object sender, EventArgs e)
        {
            MoveToFirstLabel();
            setMasking(false);
            enableBackForward(false);
            //cmdGenerate.Enabled = false;
            mnuRunTestRT.Enabled = false;
            //cmdLabelZone.Enabled = false;
            HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
            doAddOpZone();
            //int countOpZones = VDEReelConfig.VDEItems.Where(x => x.IsOPZone).Count();
            //if (countOpZones > 0)
            //    cmdGenerate.Enabled = true;
        }

        private void doAddOpZone()
        {
            ADDING_MASK = false;
            ADDING_ROT_ZONE = false;
            ADDING_OPZONES = true;
            hWinOCR.HMoveContent = false;
            DisplayInfo("please define an Op-Zone area", "top");
        }

        private void doCancelZoning()
        {
            ADDING_MASK = false;
            ADDING_OPZONES = false;
            ADDING_ROT_ZONE = false;
            hWinOCR.HMoveContent = true;
            setMasking(true);
            //enableBackForward(true);

            int countOpZones = VDEReelConfig.VDEItems.Where(x => x.IsOPZone).Count();
            if (countOpZones > 0)
            {
                //cmdLabelZone.Enabled = true;
                mnuRunTestRT.Enabled = true;
            }

            DisplayInfo("Define area cancelled", "top");
            DoSummary();
            drawVDEBorders();
        }

        private void doCancelMask()
        {
            ADDING_MASK = false;
            ADDING_OPZONES = false;
            ADDING_ROT_ZONE = false;
            setMasking(true);
            hWinOCR.HMoveContent = true;
            DisplayInfo("Masking define cancelled", "top");
            DoSummary();
            drawVDEBorders();
        }

        private void doMask()
        {
            MoveToFirstLabel();
            MouseUpMaskRegion();
            ADDING_MASK = false;
            setMasking(true);
        }


        private void cmdAddMask_Click(object sender, EventArgs e)
        {
            doAddMask();
        }

        private void doAddMask()
        {
            MoveToFirstLabel();
            enableBackForward(false);
            ADDING_MASK = true;
            HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
            HOperatorSet.SetLineWidth(hWinOCR.HalconWindow, 4);
            HOperatorSet.SetColor(hWinOCR.HalconWindow, "firebrick");
            setMasking(false);
            hWinOCR.SetFullImagePart();
            hWinOCR.HMoveContent = false;
            DisplayInfo("please define the masked area", "top");
        }


        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            try
            {
                
                if (LOADING_FORM)
                {
                    e.Handled = true;
                    return;
                }
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (WAITING_FOR_START)
                    {
                        WAITING_FOR_START = false;
                        WAITING_FOR_IMAGE_ONE = true;
                        doStart();
                        e.Handled = true;
                        return;
                    }
                    else if (EDITING_VARIATION)
                    {
                        if(VariationROI==null)
                        {
                            e.Handled = true;
                            return;
                        }
                        HOperatorSet.SmallestRectangle1(VariationROI, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                        if (r2 - r1 > 100 && c2 - c1 > 100)
                        {
                            DialogResult dr = new DialogResult();
                            dr = MessageBox.Show("Use the selected region to create the Variation model?", "Label Variation Model", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                saveVariation();
                                barcode2d();
                                barcodeLinear();
                                DoSummary();
                                drawVDEBorders();
                                cmdAddZone.PerformClick();
                            }
                            e.Handled = true;
                            return;
                        }
                    }
                    else if (ADDING_OPZONES)
                    {
                        DialogResult dr = new DialogResult();
                        dr = MessageBox.Show("Save Op-Zone region?", "OpZone Variation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            if (ZoneOverlap(topStart, leftStart, bottomStart, rightStart))
                                return;

                            cmdGenerate.Enabled = false;
                            //cmdLabelZone.Enabled = false;
                            cmdGenerate.Refresh();
                            if (doSaveOpZone())
                            {
                                DisplayInfo("Op-Zone is valid", "top");
                            }

                            ADDING_OPZONES = false;
                            setMasking(true);
                            DoSummary();
                            drawVDEBorders();
                            int countOpZones = VDEReelConfig.VDEItems.Where(x => x.IsOPZone).Count();
                            if (countOpZones > 0)
                            {
                                cmdGenerate.Enabled = true;
                                cmdGenerate.Refresh();
                            }
                            e.Handled = true;
                            return;
                        }
                    }
                    else if (ADDING_MASK)
                    {
                        DialogResult dr = new DialogResult();
                        dr = MessageBox.Show("Save Masked region?", "Masking Variation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            doMask();
                            setMasking(true);
                            DoSummary();
                            drawVDEBorders();
                            DisplayInfo("", "top");
                            int countOpZones = VDEReelConfig.VDEItems.Where(x => x.IsOPZone).Count();
                            if (countOpZones > 0)
                            {
                                cmdGenerate.Enabled = true;
                                cmdGenerate.Refresh();
                            }
                            e.Handled = true;
                            return;
                        }
                    }
                    //else if (ADDING_ROT_ZONE)
                    //{
                    //    DialogResult dr = new DialogResult();
                    //    dr = MessageBox.Show("Save rotated region?", "Suppress diagonal bleedthrough", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    //    if (dr == DialogResult.Yes)
                    //    {
                    //        doRotZone();
                    //        setMasking(true);
                    //        DoSummary();
                    //        drawVDEBorders();
                    //        DisplayInfo("", "top");
                    //        e.Handled = true;
                    //        return;
                    //    }
                    //}
                }
                if (e.KeyChar == (char)Keys.Escape)
                {
                    if (ADDING_OPZONES)
                    {
                        doCancelZoning();
                        e.Handled = true;
                        return;
                    }
                    if (ADDING_MASK)
                    {
                        doCancelZoning();
                        doCancelMask();
                        e.Handled = true;
                        return;
                    }
                    if (ADDING_VDE)
                    {
                        cancelVDEManual();
                        e.Handled = true;
                        return;
                    }
                }
                e.Handled = false;
            }
            catch { }
        }

        private void createOpZoneVariationModels()
        {
            showSpinner(true);
            if (createOpZoneVAMs())
            {
                DoSummary();
                drawVDEBorders();
                hWinOCR.SetFullImagePart();
                DisplayInfo("<---------    Select menu Options | Run Setup Test", "top");
            }
            showSpinner(false);
        }

        private bool createOpZoneVAMs()
        {
            HObject imgTest = null;
            bool retVal = true;
            try
            {
                int countOpZones = VDEReelConfig.VDEItems.Where(x => x.IsOPZone).Count();
                if (countOpZones == 0)
                {
                    DisplayInfo("", "top");
                    MessageBox.Show("There are no Op-Zones defined yet", "Create Variation Models");
                    return false;
                }

                VARIATION_COMPLETE = true;
                DisplayInfo("", "top");
                foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
                {
                    DisplayInfo("creating model for " + vdi.OpZoneName, "top");
                    if (generateOPZone(vdi) == false)
                    {
                        DisplayInfo("", "top");
                        return false;
                    }
                    else
                    {
                        DisplayInfo(vdi.OpZoneName + " generate Op-Zone model complete", "top");
                        //cmdGenerate.Enabled = false;
                    }
                }

            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "createVariationLabelAndAllZones() err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                //hWinOCR.HalconWindow.ClearWindow();
                //hWinOCR.HalconWindow.DetachBackgroundFromWindow();
                //if (imgTest != null)
                //    imgTest.Dispose();
                //HOperatorSet.CopyObj(TestImages, out imgTest, 1, 1);
                //HOperatorSet.ReduceDomain(imgTest, VariationROI, out imgTest);
                //try { HOperatorSet.AttachBackgroundToWindow(imgTest, hWinOCR.HalconWindow); } catch { }

                if (imgTest != null)
                    imgTest.Dispose();
            }
            return retVal;
        }

        public static List<DebrisAndErrors> DerbrisErrorList = DebrisAndErrors.Listing;

        private void runTest(bool inspectlightareas)
        {
            HObject tmpZone = null, rngComplement = null, imgTest = null, rgnZone = null, imgZoneReduced = null, rgnComplement = null, zoneStack = null;
            uscVDEItem uvde = null;
            List<uscVDEItem> MedDataList = new List<uscVDEItem>();
            string dataToFind = "";
            //MushObjects mo = null;
            try
            {
                imageIndex = 0;
                DebrisAndErrors.Clear();
                testErrors = new List<string>();
                foreach (Control ctrl in fplvdeitems.Controls)
                {
                    if (ctrl is uscVDEItem)
                    {
                        uvde = (uscVDEItem)ctrl;
                        MedDataList.Add(uvde);
                    }
                }
                SetupInspection sui = new SetupInspection(hWinOCR, labelFixture, this.DisplayInfo,VariationROI, iParams);
                int numTestImages = 0;
                try { numTestImages = TestImages.CountObj(); } catch { numTestImages = 0; }
                if (numTestImages == 0)
                {
                    MessageBox.Show("No training images are available. Expected: " + testImageCount.ToString(), "Setup Test Run", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                long timeAvg = 0;
                Stopwatch sw = new Stopwatch();
                sw.Start();

                foreach (uscVDEItem data in MedDataList)
                    data.Reset();

                for (int x = 1; x <= numTestImages; x++)
                {
                    sw.Restart();
                    if (imgTest != null)
                        imgTest.Dispose();
                    if (zoneStack != null)
                        zoneStack.Dispose();
                    HOperatorSet.GenEmptyObj(out zoneStack);

                    HOperatorSet.CopyObj(TestImages, out imgTest, x, 1);
                    sui.TransformLabel(ref imgTest, x);
                    HOperatorSet.ReduceDomain(imgTest, VariationROI, out imgTest);
                    HOperatorSet.DetachBackgroundFromWindow(hWinOCR.HalconWindow);
                    
                    HOperatorSet.AttachBackgroundToWindow(imgTest, hWinOCR.HalconWindow);

                    drawVDEBorders();
                    foreach (VDEItem opz in VDEReelConfig.VDEItems.Where(y => y.IsOPZone))
                    {
                        if (rgnZone != null)
                            rgnZone.Dispose();
                        if (imgZoneReduced != null)
                            imgZoneReduced.Dispose();
                        if (sui.TransformOPZone(ref imgTest, opz, ref testErrors, x))
                        {
                            HOperatorSet.GenRectangle1(out rgnZone, opz.ODP.OPZoneVARRegion[0], opz.ODP.OPZoneVARRegion[1], opz.ODP.OPZoneVARRegion[2], opz.ODP.OPZoneVARRegion[3]);
                            HOperatorSet.ConcatObj(rgnZone, zoneStack, out zoneStack);
                            HOperatorSet.ReduceDomain(imgTest, rgnZone, out imgZoneReduced);
                            foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(y => y.IsVDE && y.OpZoneName == opz.OpZoneName))
                            {
                                foreach (uscVDEItem data in MedDataList)
                                {
                                    if (data.PlaceHolder == vdi.Placeholder)
                                    {
                                        dataToFind = data.VDEData;
                                        break;
                                    }
                                }
                                if (sui.FindVDE(vdi.VDERotatedAngle, vdi.VDEAngle, ref imgZoneReduced, ref rgnZone, vdi, dataToFind, x, opz, false, ref testErrors) == false)
                                {
                                    MoveToNextVDE();
                                    sui.FindVDE(vdi.VDERotatedAngle, vdi.VDEAngle, ref imgZoneReduced, ref rgnZone, vdi, dataToFind, x, opz, false, ref testErrors);
                                    break;
                                }
                            }
                            if (sui.InspectOpZone(ref imgTest, ref rgnZone, opz, false, INSPECT_LIGHT_AREAS, ref testErrors, InnerRadius) == false)
                            {
                                testErrors.Add("RunTest() err: Op-Zone " + opz.OpZoneName);
                                return;
                            }
                        }
                    }
                    imgTest.Dispose();
                    HOperatorSet.CopyObj(TestImages, out imgTest, x, 1);
                    HOperatorSet.ReduceDomain(imgTest, VariationROI, out imgTest);
                    int zones = zoneStack.CountObj();
                    for (int y = 1; y <= zones; y++)
                    {
                        HOperatorSet.CopyObj(zoneStack, out tmpZone, y, 1);
                        HOperatorSet.Complement(tmpZone, out rngComplement);
                        HOperatorSet.ReduceDomain(imgTest, rngComplement, out imgTest);
                        rngComplement.Dispose();
                        tmpZone.Dispose();
                    }
                    findAndMaskBarcode2D(ref imgTest);
                    findAndMaskBarcodeLinear(ref imgTest);
                    findAndMaskMasks(ref imgTest);
                    if (sui.InspectLabel(ref imgTest, REEL_LPN, inspectlightareas) == false)
                    {
                        testErrors.Add("RunTest() err: label " + x.ToString() + " was not inspected");
                        return;
                    }
                    if (sw.IsRunning)
                    {
                        sw.Stop();
                        DisplayInfo(string.Format("label {0}", x), "top");
                        timeAvg += sw.ElapsedMilliseconds;
                    }
                    foreach (uscVDEItem data in MedDataList)
                        data.MoveToNextVDE();
                    int avgTime = Convert.ToInt32(timeAvg / numTestImages);
                    DisplayInfo(string.Format("Average time to inspect a label: {0} milliseconds", avgTime), "top");
                    foreach (uscVDEItem data in MedDataList)
                        data.Reset();
                }
            }
            catch (Exception ex)
            {
                string err = "runTest() err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (tmpZone != null) tmpZone.Dispose();
                if (imgTest != null)  imgTest.Dispose();
                if (zoneStack != null)  zoneStack.Dispose();
                if (rngComplement != null)  rngComplement.Dispose();
                if (rgnZone != null) rgnZone.Dispose();
                if (imgZoneReduced != null) imgZoneReduced.Dispose();
                if (imgTest != null) imgTest.Dispose();
                if (rgnComplement != null) rgnComplement.Dispose();
            }
        }


        private void runTest(int x, bool inspectlightareas)
        {
            HObject tmpZone = null, rngComplement = null, imgTest = null, rgnZone = null, imgZoneReduced = null, rgnComplement = null, zoneStack = null;
            uscVDEItem uvde = null;
            List<uscVDEItem> MedDataList = new List<uscVDEItem>();
            string dataToFind = "";
            try
            {
                DebrisAndErrors.Clear();
                testErrors = new List<string>();
                foreach (Control ctrl in fplvdeitems.Controls)
                {
                    if (ctrl is uscVDEItem)
                    {
                        uvde = (uscVDEItem)ctrl;
                        MedDataList.Add(uvde);
                    }
                }
                SetupInspection sui = new SetupInspection(hWinOCR, labelFixture, this.DisplayInfo, VariationROI, iParams);
                long timeAvg = 0;
                Stopwatch sw = new Stopwatch();
                sw.Start();

                sw.Restart();
                if (imgTest != null)
                    imgTest.Dispose();
                if (zoneStack != null)
                    zoneStack.Dispose();
                HOperatorSet.GenEmptyObj(out zoneStack);

                HOperatorSet.CopyObj(TestImages, out imgTest, x, 1);
                sui.TransformLabel(ref imgTest, x);
                HOperatorSet.ReduceDomain(imgTest, VariationROI, out imgTest);
                HOperatorSet.DetachBackgroundFromWindow(hWinOCR.HalconWindow);
                HOperatorSet.AttachBackgroundToWindow(imgTest, hWinOCR.HalconWindow);
                drawVDEBorders();
                foreach (VDEItem opz in VDEReelConfig.VDEItems.Where(y => y.IsOPZone))
                {
                    if (rgnZone != null)
                        rgnZone.Dispose();
                    if (imgZoneReduced != null)
                        imgZoneReduced.Dispose();
                    if (sui.TransformOPZone(ref imgTest, opz, ref testErrors, x))
                    {
                        HOperatorSet.GenRectangle1(out rgnZone, opz.ODP.OPZoneVARRegion[0], opz.ODP.OPZoneVARRegion[1], opz.ODP.OPZoneVARRegion[2], opz.ODP.OPZoneVARRegion[3]);
                        HOperatorSet.ConcatObj(rgnZone, zoneStack, out zoneStack);
                        HOperatorSet.ReduceDomain(imgTest, rgnZone, out imgZoneReduced);
                        foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(y => y.IsVDE && y.OpZoneName == opz.OpZoneName))
                        {
                            foreach (uscVDEItem data in MedDataList)
                            {
                                if (data.PlaceHolder == vdi.Placeholder)
                                {
                                    dataToFind = data.VDEData;
                                    break;
                                }
                            }
                            if (sui.FindVDE(vdi.VDERotatedAngle, vdi.VDEAngle, ref imgZoneReduced, ref rgnZone, vdi, dataToFind, x, opz, false, ref testErrors) == false)
                            {
                                MoveToNextVDE();
                                sui.FindVDE(vdi.VDERotatedAngle, vdi.VDEAngle, ref imgZoneReduced, ref rgnZone, vdi, dataToFind, x, opz, false, ref testErrors);
                                break;
                            }
                        }
                        if (sui.InspectOpZone(ref imgTest, ref rgnZone, opz, false, INSPECT_LIGHT_AREAS, ref testErrors, InnerRadius) == false)
                        {
                            testErrors.Add("RunTest() err: Op-Zone " + opz.OpZoneName);
                            return;
                        }
                    }
                }
                imgTest.Dispose();
                HOperatorSet.CopyObj(TestImages, out imgTest, x, 1);
                HOperatorSet.ReduceDomain(imgTest, VariationROI, out imgTest);
                int zones = zoneStack.CountObj();
                for (int y = 1; y <= zones; y++)
                {
                    HOperatorSet.CopyObj(zoneStack, out tmpZone, y, 1);
                    HOperatorSet.Complement(tmpZone, out rngComplement);
                    HOperatorSet.ReduceDomain(imgTest, rngComplement, out imgTest);
                    rngComplement.Dispose();
                    tmpZone.Dispose();
                }
                findAndMaskBarcode2D(ref imgTest);
                findAndMaskBarcodeLinear(ref imgTest);
                if (sui.InspectLabel(ref imgTest, REEL_LPN, inspectlightareas) == false)
                {
                    testErrors.Add("RunTest() err: label " + x.ToString() + " was not inspected");
                    return;
                }
                if (sw.IsRunning)
                {
                    sw.Stop();
                    DisplayInfo(string.Format("label {0}", x), "top");
                    timeAvg += sw.ElapsedMilliseconds;
                }
            }
            catch (Exception ex)
            {
                string err = "runTest(int) err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (tmpZone != null) tmpZone.Dispose();
                if (imgTest != null) imgTest.Dispose();
                if (zoneStack != null) zoneStack.Dispose();
                if (rngComplement != null) rngComplement.Dispose();
                if (rgnZone != null) rgnZone.Dispose();
                if (imgZoneReduced != null) imgZoneReduced.Dispose();
                if (imgTest != null) imgTest.Dispose();
                if (rgnComplement != null) rgnComplement.Dispose();
            }
        }

        private void runTest(int x, string opzonename, bool inspectlightareas)
        {
            HObject tmpZone = null, rngComplement = null, imgTest = null, rgnZone = null, imgZoneReduced = null, rgnComplement = null, zoneStack = null;
            uscVDEItem uvde = null;
            List<uscVDEItem> MedDataList = new List<uscVDEItem>();
            string dataToFind = "";
            try
            {
                DebrisAndErrors.Clear();
                testErrors = new List<string>();
                foreach (Control ctrl in fplvdeitems.Controls)
                {
                    if (ctrl is uscVDEItem)
                    {
                        uvde = (uscVDEItem)ctrl;
                        MedDataList.Add(uvde);
                    }
                }

                hWinOCR.HalconWindow.ClearWindow();
                imageIndex = x;
                foreach (uscVDEItem data in MedDataList)
                    data.Reset();
                //foreach (uscVDEItem data in MedDataList)
                //    data.MoveToIndex(x);
                DebrisAndErrors.Clear();
                testErrors = new List<string>();
                foreach (Control ctrl in fplvdeitems.Controls)
                {
                    if (ctrl is uscVDEItem)
                    {
                        uvde = (uscVDEItem)ctrl;
                        MedDataList.Add(uvde);
                    }
                }
                SetupInspection sui = new SetupInspection(hWinOCR, labelFixture, this.DisplayInfo, VariationROI, iParams);
                long timeAvg = 0;
                Stopwatch sw = new Stopwatch();
                sw.Start();

                sw.Restart();
                if (imgTest != null)
                    imgTest.Dispose();
                if (zoneStack != null)
                    zoneStack.Dispose();
                HOperatorSet.GenEmptyObj(out zoneStack);
                HOperatorSet.CopyObj(TestImages, out imgTest, x, 1);
                sui.TransformLabel(ref imgTest, x);
                HOperatorSet.ReduceDomain(imgTest, VariationROI, out imgTest);
                HOperatorSet.DetachBackgroundFromWindow(hWinOCR.HalconWindow);
                HOperatorSet.AttachBackgroundToWindow(imgTest, hWinOCR.HalconWindow);
                foreach (VDEItem opz in VDEReelConfig.VDEItems.Where(y => y.IsOPZone && y.VDEItemName == opzonename))
                {
                    if (rgnZone != null)
                        rgnZone.Dispose();
                    if (imgZoneReduced != null)
                        imgZoneReduced.Dispose();
                    if (sui.TransformOPZone(ref imgTest, opz, ref testErrors, x))
                    {
                        HOperatorSet.DetachBackgroundFromWindow(hWinOCR.HalconWindow);
                        HOperatorSet.AttachBackgroundToWindow(imgTest, hWinOCR.HalconWindow);
                        HOperatorSet.GenRectangle1(out rgnZone, opz.ODP.OPZoneVARRegion[0], opz.ODP.OPZoneVARRegion[1], opz.ODP.OPZoneVARRegion[2], opz.ODP.OPZoneVARRegion[3]);
                        HOperatorSet.ConcatObj(rgnZone, zoneStack, out zoneStack);
                        HOperatorSet.ReduceDomain(imgTest, rgnZone, out imgZoneReduced);
                        foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(y => y.IsVDE && y.OpZoneName == opz.OpZoneName))
                        {
                            foreach (uscVDEItem data in MedDataList)
                            {
                                if (data.PlaceHolder == vdi.Placeholder)
                                {
                                    dataToFind = data.VDEData;
                                    break;
                                }
                            }
                            if (sui.FindVDE(vdi.VDERotatedAngle, vdi.VDEAngle, ref imgZoneReduced, ref rgnZone, vdi, dataToFind, x, opz, false, ref testErrors) == false)
                            {
                                MoveToNextVDE();
                                sui.FindVDE(vdi.VDERotatedAngle, vdi.VDEAngle, ref imgZoneReduced, ref rgnZone, vdi, dataToFind, x, opz, false, ref testErrors);
                                break;
                            }
                        }
                        if (sui.InspectOpZone(ref imgTest, ref rgnZone, opz, false, INSPECT_LIGHT_AREAS, ref testErrors, InnerRadius) == false)
                        {
                            testErrors.Add("RunTest() err: Op-Zone " + opz.OpZoneName);
                            return;
                        }
                    }
                }
                imgTest.Dispose();
                HOperatorSet.CopyObj(TestImages, out imgTest, x, 1);
                HOperatorSet.ReduceDomain(imgTest, VariationROI, out imgTest);
                int zones = zoneStack.CountObj();
                for (int y = 1; y <= zones; y++)
                {
                    HOperatorSet.CopyObj(zoneStack, out tmpZone, y, 1);
                    HOperatorSet.Complement(tmpZone, out rngComplement);
                    HOperatorSet.ReduceDomain(imgTest, rngComplement, out imgTest);
                    rngComplement.Dispose();
                    tmpZone.Dispose();
                }
                findAndMaskBarcode2D(ref imgTest);
                findAndMaskBarcodeLinear(ref imgTest);
                if (sui.InspectLabel(ref imgTest, REEL_LPN, inspectlightareas) == false)
                {
                    testErrors.Add("RunTest() err: label " + x.ToString() + " was not inspected");
                    return;
                }
                if (sw.IsRunning)
                {
                    sw.Stop();
                    DisplayInfo(string.Format("label {0}", x), "top");
                    timeAvg += sw.ElapsedMilliseconds;
                }
            }
            catch (Exception ex)
            {
                string err = "runTest(int) err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (tmpZone != null) tmpZone.Dispose();
                if (imgTest != null) imgTest.Dispose();
                if (zoneStack != null) zoneStack.Dispose();
                if (rngComplement != null) rngComplement.Dispose();
                if (rgnZone != null) rgnZone.Dispose();
                if (imgZoneReduced != null) imgZoneReduced.Dispose();
                if (imgTest != null) imgTest.Dispose();
                if (rgnComplement != null) rgnComplement.Dispose();
            }
        }

        private void showErrors(List<string> testerrors)
        {
            string errs = "";
            foreach (string s in testerrors)
                errs = errs + s + "\n\r";
            if (errs != "")
                MessageBox.Show("Test complete with errors:\n" + errs);
            else
                MessageBox.Show("Test complete");
        }

        private void mnuClose_Click(object sender, EventArgs e)
        {
            if (doClose())
            {
                closing();
                Hide();
            }
        }

        public class SetupInspection
        {

            private InspectionParams iParams;

            private HObject variationROI = null;
            private Action<string, string> displayCaller = null;
            private HTuple OPZoneCol = 0;
            private HTuple OPZoneRow = 0;
            private LabelFixture labelFixture = new LabelFixture();
            private HSmartWindowControl HWIN = null;


            public SetupInspection(HSmartWindowControl hwin, LabelFixture labelfixture, Action<string, string> displaycaller, HObject variationroi, InspectionParams iparams)
            {
                //this.LabelBemishes = labelblemishes;
                this.displayCaller = displaycaller;
                labelFixture = labelfixture;
                HWIN = hwin;
                HWIN.HalconWindow.SetColored(12);
                HOperatorSet.SetDraw(HWIN.HalconWindow, "fill");
                variationROI = variationroi;
                iParams = iparams;
            }


            private void displayInfo(string msg, string position)
            {
                try
                {
                    HWIN.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        displayCaller(msg, position);
                    });
                }
                catch (Exception ex)
                {
                    string err = "displayInfo() err: " + ex.Message;
                    MessageBox.Show(err, "Label Training", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }


            public bool FindVDE(int rotatedangle, int angle, ref HObject img, ref HObject rgnzone, VDEItem vde, string datatofind, int imageindex, VDEItem opz, bool pausetest, ref List<string> errors)
            {
                bool retVal = true;
                HObject region = null, rgnComplement = null, imgReduced = null, imgRotated = null, characters = null;
                HTuple word = null, wordscore = null;
                try
                {
                    HOperatorSet.GenEmptyObj(out imgRotated);
                    rotateBackground(vde.VDEAngle, ref img, ref imgRotated);
                    HOperatorSet.SmallestRectangle1(rgnzone, out HTuple top, out HTuple left, out HTuple bottom, out HTuple right);
                    int[] rgnCoord = new int[] { top, left, bottom, right };
                    HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                    word = "";
                    wordscore = 0;

                    if (vde.VDEAngle != 0)
                    {
                        rotateBackground(vde.VDEAngle, ref img, ref img);
                        HOperatorSet.GetImageSize(img, out w, out h);
                        rgnCoord = VDEItem.RotateClockwiseCoords(rotatedangle, w, h, rgnCoord);
                        rgnzone.Dispose();
                        HOperatorSet.GenRectangle1(out rgnzone, rgnCoord[0], rgnCoord[1], rgnCoord[2], rgnCoord[3]);
                    }
                    int[] vdeCoord = vde.VDERegion;

                    int[] textCoords = VDEItem.RotateClockwiseCoords(vde.VDERotatedAngle, w, h, vdeCoord);
                    bool ErrorOccured = false;
                    string ErrorString = "";
                    if (getText(ref characters, ref word, ref wordscore, datatofind, ref imgRotated, textCoords, vde) == false)
                    {
                        retVal = false;
                        if (vde.RepeatType > 0)
                        {
                            top = vde.VDERegion[0] - 8;
                            left = vde.VDERegion[1] - 8;
                            bottom = vde.VDERegion[2] + 8;
                            right = vde.VDERegion[3] + 8;
                            if (top < 0)
                                top = 0;
                            if (left < 0)
                                left = 0;
                            HOperatorSet.GenRectangle1(out region, top, left, bottom, right);
                            HOperatorSet.ReduceDomain(img, region, out imgReduced);
                            HOperatorSet.Complement(region, out rgnComplement);
                            HOperatorSet.ReduceDomain(img, rgnComplement, out img);
                        }
                        HWIN.HalconWindow.SetColor("red");
                        HWIN.HalconWindow.DispObj(characters);
                        if (ErrorOccured)
                            throw new Exception(ErrorString);
                    }
                    string resultText = word.S;
                    string tmpResult = resultText;
                    HTuple matchData = new HTuple(datatofind);
                    maskComplementVDE(word.S, resultText, characters, angle, w, h, vde, ref img, ref errors);
                }
                catch (Exception ex)
                {
                    retVal = false;
                    string innerException = " " + ex.InnerException.Message;
                    string err = string.Format("VDE, image: {0}, zone {1}, searh data {2}, placeholder: {3}.", imageindex, opz.OpZoneName, datatofind, vde.Placeholder) + ex.Message + innerException.Trim();
                    errors.Add(err);
                    MessageBox.Show(err);
                }
                finally
                {
                    if (imgReduced != null)
                        imgReduced.Dispose();
                    if (rgnComplement != null)
                        rgnComplement.Dispose();
                    if (region != null)
                        region.Dispose();
                    if (characters != null)
                        characters.Dispose();
                    if (imgRotated != null)
                        imgRotated.Dispose();
                }
                return retVal;
            }

            private bool maskComplementVDE(string matcheditem, string resultText, HObject characters, int angle, int w, int h, VDEItem vde, ref HObject img, ref List<string> errors)
            {
                bool retVal = true;
                HObject region = null, mask = null, rgnComplement = null;
                int firstChar = 0;
                int lastChar = 0;
                string matchFound = matcheditem;
                string res = matcheditem;
                try
                {
                    while (firstChar >= 0)
                    {
                        firstChar = (resultText.IndexOf(matchFound, firstChar));
                        if (firstChar == -1)
                            break;
                        if (rgnComplement != null)
                            rgnComplement.Dispose();
                        rgnComplement = null;
                        if (region != null)
                            region.Dispose();
                        region = null;
                        if (mask != null)
                            mask.Dispose();
                        mask = null;
                        lastChar = matchFound.Length;
                        //if (vde.RepeatType > 0)
                        //{
                        HOperatorSet.CopyObj(characters, out mask, firstChar + 1, lastChar);
                        HOperatorSet.CountObj(mask, out HTuple numChars);
                        int top = 500000;
                        int left = 500000;
                        int bottom = 0;
                        int right = 0;
                        for (int x = 1; x <= numChars; x++)
                        {
                            HOperatorSet.SmallestRectangle1(mask[x], out HTuple t, out HTuple l, out HTuple b, out HTuple r);
                            if (t < top)
                                top = t;
                            if (l < left)
                                left = l;
                            if (b > bottom)
                                bottom = b;
                            if (r > right)
                                right = r;
                        }
                        int[] maskCoord = new int[] { top, left, bottom, right };
                        int ROW1 = Convert.ToInt32(maskCoord[0]) - 2;
                        int COL1 = Convert.ToInt32(maskCoord[1]) - 2;
                        int ROW2 = Convert.ToInt32(maskCoord[2]) + 2;
                        int COL2 = Convert.ToInt32(maskCoord[3]) + 2;
                        if (ROW1 < 0)
                            ROW1 = 0;
                        if (COL1 < 0)
                            COL1 = 0;
                        //if (ROW2 > h)
                        //    ROW2 = h;
                        if (COL2 > w)
                            COL2 = w;
                        maskCoord[0] = ROW1;
                        maskCoord[1] = COL1;
                        maskCoord[2] = ROW2;
                        maskCoord[3] = COL2;
                        int[] rotateCoords = VDEItem.RotateAntiClockwiseCoords(angle, w, h, maskCoord);
                        HOperatorSet.GenRectangle1(out region, rotateCoords[0] - 4, rotateCoords[1] - 4, rotateCoords[2] + 4, rotateCoords[3] + 4);
                        if (vde.RepeatType > 0)
                        {
                            HOperatorSet.Complement(region, out rgnComplement);
                            HOperatorSet.ReduceDomain(img, rgnComplement, out img);
                        }
                        if (rgnComplement != null)
                            rgnComplement.Dispose();

                        HWIN.HalconWindow.SetColor("red");
                        HOperatorSet.SetDraw(HWIN.HalconWindow, "margin");
                        HWIN.HalconWindow.DispObj(region);

                        if (region != null)
                            region.Dispose();
                        //}
                        if (firstChar < resultText.Length + matchFound.Length)
                            firstChar += matchFound.Length;
                        else
                            break;
                    }
                }
                catch (Exception ex)
                {
                    string err = "maskComplementVDE() err: " + ex.Message;
                    errors.Add(err);
                    retVal = false;
                    MessageBox.Show(err);
                }
                finally
                {
                    if (rgnComplement != null)
                        rgnComplement.Dispose();
                    if (region != null)
                        region.Dispose();
                    if (mask != null)
                        mask.Dispose();
                    if (characters != null)
                        characters.Dispose();
                }
                return retVal;
            }

            public bool TransformLabel(ref HObject img, int imageindex)
            {
                bool retVal = true;
                HTuple LabelCol = 0, LabelRow = 0, HomMat2D = null, scaleR = null, scaleC = null;
                HTuple row = null, column = null, angle = null, LabelScore = null;
                HObject imgTransform = null, rgnLabel = null;
                try
                {
                    LabelCol = 0;
                    LabelRow = 0;
                    if (imgTransform != null)
                        imgTransform.Dispose();
                    if (rgnLabel != null)
                        rgnLabel.Dispose();
                    HOperatorSet.ReduceDomain(img, variationROI, out img);

                    HOperatorSet.FindAnisoShapeModel(img, labelFixture.FixtureID, Defaults.radMinus1Point5, Defaults.rad2, 1, 1, 0.98, 1.02, 0.5, 1, 0.5, "least_squares", Defaults.NumLevelsFind, Defaults.Greediness, out row, out column, out angle, out scaleR, out scaleC, out LabelScore);
                    double score = 0;
                    if (LabelScore.Length > 0)
                        score = (double)LabelScore.D;
                    else
                    {
                        HOperatorSet.FindAnisoShapeModel(img, labelFixture.FixtureID, Defaults.radMinus1Point5, Defaults.rad2, 0.9, 1, 0.9, 1.0, 0.0, 1, 0.5, "least_squares", 0, 0.9, out row, out column, out angle, out scaleR, out scaleC, out LabelScore);
                        if (LabelScore.Length > 0)
                            score = (double)LabelScore.D;
                    }
                    if (score >= Defaults.FixtureOPZoneScoreMin)
                    {
                        LabelCol = labelFixture.FixtureCenterX - column;
                        LabelRow = labelFixture.FixtureCenterY - row;
                        HOperatorSet.HomMat2dIdentity(out HomMat2D);
                        HOperatorSet.HomMat2dScale(HomMat2D, 1 / scaleR, 1 / scaleC, row, column, out HomMat2D);
                        HOperatorSet.HomMat2dRotate(HomMat2D, -angle, row, column, out HomMat2D);
                        HOperatorSet.HomMat2dTranslate(HomMat2D, LabelRow, LabelCol, out HomMat2D);
                        HOperatorSet.AffineTransImage(img, out imgTransform, HomMat2D, "weighted", "false");
                        //HWIN.HalconWindow.DetachBackgroundFromWindow();
                        //HOperatorSet.AttachBackgroundToWindow(imgTransform, HWIN.HalconWindow);
                        img.Dispose();
                        HOperatorSet.CopyObj(imgTransform, out img, 1, 1);
                    }
                    else
                    {
                        retVal = false;
                    }
                }
                catch (Exception ex)
                {
                    retVal = false;
                    string err = "TransformLabel() err: label: " + imageindex.ToString() + "\n\r" + ex.Message;
                }
                finally
                {
                    if (rgnLabel != null)
                        rgnLabel.Dispose();
                    if (imgTransform != null)
                        imgTransform.Dispose();
                }
                return retVal;
            }

            public bool TransformOPZone(ref HObject img, VDEItem ozd, ref List<string> errors, int imageindex)
            {
                bool retVal = true;
                HTuple HomMat2D = null, scaleR = null, scaleC = null;
                HTuple row = null, column = null, angle = null, OPZoneScore = null;
                HObject imgTransform = null, OpZoneRegion = null;
                string opZoneName = "";
                try
                {
                    opZoneName = ozd.OpZoneName;
                    if (imgTransform != null)
                        imgTransform.Dispose();
                    if (OpZoneRegion != null)
                        OpZoneRegion.Dispose();

                    HOperatorSet.FindAnisoShapeModel(img, ozd.ODP.OPZoneIDFixture, Defaults.radMinus1Point5, Defaults.rad2, 1, 1, 0.98, 1.02, 0.5, 1, 0.5, "least_squares", Defaults.CreateAniso, Defaults.Greediness, out row, out column, out angle, out scaleR, out scaleC, out OPZoneScore);

                    double score = 0;
                    if (OPZoneScore.Length > 0)
                    {
                        score = (double)OPZoneScore.D;                        
                    }
                    else
                    {
                        object obj1 = ozd.ODP;

                        HOperatorSet.FindAnisoShapeModel(img, ozd.ODP.OPZoneIDFixture, Defaults.radMinus1Point5, Defaults.rad2, 0.9, 1, 0.9, 1.0, 0.0, 1, 0.5, "least_squares", 0, 0.9, out row, out column, out angle, out scaleR, out scaleC, out OPZoneScore);
                        if (OPZoneScore.Length > 0)
                            score = OPZoneScore.D;
                    }
                    if (score >= Defaults.FixtureOPZoneScoreMin)
                    {
                        OPZoneCol = ozd.ODP.OPZoneFixtureX - column;
                        OPZoneRow = ozd.ODP.OPZoneFixtureY - row;
                        HOperatorSet.HomMat2dIdentity(out HomMat2D);
                        HOperatorSet.HomMat2dScale(HomMat2D, 1 / scaleR, 1 / scaleC, row, column, out HomMat2D);
                        HOperatorSet.HomMat2dRotate(HomMat2D, -angle, row, column, out HomMat2D);
                        HOperatorSet.HomMat2dTranslate(HomMat2D, OPZoneRow, OPZoneCol, out HomMat2D);
                        HOperatorSet.AffineTransImage(img, out imgTransform, HomMat2D, "weighted", "false");
                        img.Dispose();
                        HOperatorSet.CopyObj(imgTransform, out img, 1, 1);
                        HWIN.Refresh();
                        ozd.ODP.HomMat = HomMat2D;
                    }
                    else
                    {
                        errors.Add(string.Format("Find Op-Zone model score too low: {0}, image: {1} zone: {2}", score, imageindex, ozd.OpZoneName));
                        retVal = false;
                    }
                }
                catch (Exception ex)
                {
                    retVal = false;
                    string err = "TransformOpZone() err: " + "region: " + opZoneName + ": " + ex.Message;
                    errors.Add(err);

                }
                finally
                {
                    if (OpZoneRegion != null)
                        OpZoneRegion.Dispose();
                    if (imgTransform != null)
                        imgTransform.Dispose();
                }
                return retVal;
            }

            private bool rotateBackground(int angle, ref HObject imgIn, ref HObject imgOut)
            {
                bool retVal = false;
                try
                {
                    if (imgOut != null)
                        imgOut.Dispose();
                    HOperatorSet.RotateImage(imgIn, out imgOut, angle * -1, "constant");
                    retVal = true;
                }
                catch (Exception ex)
                {
                    string err = "rotateBackground() err: " + ex.Message;
                    MessageBox.Show(err);
                }
                return retVal;
            }

            public int RotateAdjustment(int valuerotated)
            {
                int angle = 0;
                if (valuerotated == 0)
                    angle = 90;
                else if (valuerotated == 90)
                    angle = 180;
                else if (valuerotated == 180)
                    angle = 270;
                else if (valuerotated == 270)
                    angle = 360;
                return angle;
            }

            private bool getText(ref HObject characters, ref HTuple word, ref HTuple wordscore, string matchdata, ref HObject img, int[] region, VDEItem vde)
            {
                HObject imgReduced = null, rgnZone = null, imgForeground = null, connectedChars = null, connectedRgnChars = null;
                bool retVal = true;
                HTuple textresultid = null, classVal = null, confidence = null;
                try
                {
                    word = new HTuple();
                    wordscore = new HTuple();
                    HOperatorSet.GenEmptyObj(out characters);
                    HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                    HTuple top = region[0] - 4;
                    HTuple left = region[1];
                    HTuple bottom = region[2];
                    HTuple right = region[3] + 4;

                    HOperatorSet.GenRectangle1(out rgnZone, top, left, bottom, right);
                    HOperatorSet.ReduceDomain(img, rgnZone, out imgReduced);
                    HOperatorSet.SegmentCharacters(rgnZone, imgReduced, out imgForeground, out characters, "local_contrast_best", "false", "false", vde.tr.StrokeWidth, vde.tr.CharWidth, vde.tr.CharHeight, 0, vde.tr.SegmentContrast, out HTuple usedThreshold);
                    HOperatorSet.SelectCharacters(characters, out connectedChars, "false", vde.tr.StrokeWidth, vde.tr.CharWidth, vde.tr.CharHeight, "true", "false", vde.tr.PartitionMethod, "false", "medium", "false", 20, "completion");
                    characters.Dispose();
                    HOperatorSet.Connection(connectedChars, out connectedRgnChars);
                    HOperatorSet.SelectShape(connectedRgnChars, out characters, "area", "and", 80, 99999);
                    HOperatorSet.DoOcrWordMlp(characters, imgReduced, vde.tr.ocrHandle, "", 5, 3, out classVal, out confidence, out word, out wordscore);

                    string sWord = "";
                    if (classVal.Length > 0)
                    {
                        for (int x = 0; x < classVal.Length; x++)
                            sWord = sWord + classVal[x].S;
                        word = sWord;
                    }
                    if (word.Length == 0)
                        return true;
                    if (!sWord.Contains(matchdata))
                        return true;

                    bool CountMatchError = false;
                    string ErrorString = "";
                    bool ErrorOccured = false;
                    HObject connectedregions = null;
                    if (UtilityFunctions.MSERCheckVDE(ref imgReduced, matchdata.Length, vde.CharacterContrast, ref CountMatchError, ref ErrorString, ref ErrorOccured, ref connectedregions) == true)
                    {
                        if (CountMatchError)
                        {
                            retVal = false;
                            return retVal;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retVal = false;
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (imgReduced != null)
                        imgReduced.Dispose();
                    if (rgnZone != null)
                        rgnZone.Dispose();
                    if (textresultid != null)
                        try { HOperatorSet.ClearTextResult(textresultid); } catch { }
                }
                return retVal;
            }

            #region OPZONE INSPECT

            string TRG_FOLDER = "";

            public bool InspectLabel(ref HObject img, string reel, bool inspectlightareas)
            {
                bool retVal = true;
                HObject imgTmp = null, imgThreshold = null, connectedThreshold = null, selectedAreas = null, zoneRegion = null, regUnion = null;
                HObject regDilation = null, regFillup = null, regComplement = null;
                List<OPZoneData> OPZoneItems = new List<OPZoneData>();
                TRG_FOLDER = ImageData.CreateTrainingFilepath(reel);
                HOperatorSet.SetDraw(HWIN.HalconWindow, "fill");
                try
                {
                    foreach (OPZoneData ozd in OPZoneItems.OrderBy(x => x.NAME))
                    {
                        if (imgTmp != null) imgTmp.Dispose();
                        if (imgThreshold != null) imgThreshold.Dispose();
                        if (connectedThreshold != null) connectedThreshold.Dispose();
                        if (selectedAreas != null) selectedAreas.Dispose();
                        if (zoneRegion != null) zoneRegion.Dispose();
                        if (regUnion != null) regUnion.Dispose();
                        if (regDilation != null) regDilation.Dispose();
                        if (regFillup != null) regFillup.Dispose();
                        if (regComplement != null) regComplement.Dispose();

                        HOperatorSet.ReduceDomain(img, ozd.AffineRegion, out imgTmp);
                        HOperatorSet.Threshold(imgTmp, out imgThreshold, 1, ozd.DarkMaxGray);
                        HOperatorSet.Connection(imgThreshold, out connectedThreshold);
                        HOperatorSet.SelectShape(connectedThreshold, out selectedAreas, "area", "and", 10, 999999);
                        HOperatorSet.SmallestRectangle1(selectedAreas, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                        HOperatorSet.GenRectangle1(out zoneRegion, r1, c1, r2, c2);
                        HOperatorSet.Union1(zoneRegion, out regUnion);
                        HOperatorSet.DilationRectangle1(regUnion, out regDilation, Defaults.DilationWidth, Defaults.DilationHeight);
                        //HOperatorSet.DilationCircle(regUnion, out regDilation, ozd.MSER.DarkDilationCircle);
                        HOperatorSet.FillUp(regDilation, out regFillup);
                        HOperatorSet.Complement(regFillup, out regComplement);
                        HOperatorSet.ReduceDomain(img, regComplement, out imgTmp);

                        //MaskROTZones(ref imgTmp, ozd);
                    }

                    MaskMasks(ref img);
                    if (DebrisCheck(ref img, ref img, inspectlightareas) == false)
                        retVal = false;
                }
                catch (Exception ex)
                {
                    string err = "InspectLabel() err: " + ex.Message;
                    MessageBox.Show(err);
                }
                finally
                {
                    //HObject imgTmp = null, imgThreshold = null, connectedThreshold = null, selectedAreas = null, zoneRegion = null, regUnion = null;
                    //HObject regDilation = null, regFillup = null, regComplement = null;
                    if (imgTmp != null) imgTmp.Dispose();
                    if (imgThreshold != null) imgThreshold.Dispose();
                    if (connectedThreshold != null) connectedThreshold.Dispose();
                    if (selectedAreas != null) selectedAreas.Dispose();
                    if (zoneRegion != null) zoneRegion.Dispose();
                    if (zoneRegion != null) zoneRegion.Dispose();
                    if (regUnion != null) regUnion.Dispose();
                    if (regDilation != null) regDilation.Dispose();
                    if (regFillup != null) regFillup.Dispose();
                    if (regComplement != null) regComplement.Dispose();
                    //if (VariationImg != null) VariationImg.Dispose();
                }
                return retVal;
            }


            public bool MaskMasks(ref HObject img)
            {
                bool retVal = true;
                HObject region = null;
                int ROW1 = 0, COL1 = 0, ROW2 = 0, COL2 = 0;

                try
                {
                    HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                    foreach (VDEItem vdi in VDEReelConfig.VDEItems)
                    {
                        if (vdi.IsMask)
                        {
                            ROW1 = vdi.MaskedRegion[0] - 10;
                            COL1 = vdi.MaskedRegion[1] - 10;
                            ROW2 = vdi.MaskedRegion[2] + 10;
                            COL2 = vdi.MaskedRegion[3] + 10;
                            if (ROW1 < 0)
                                ROW1 = 0;
                            if (COL1 < 0)
                                COL1 = 0;
                            if (ROW2 > h)
                                ROW2 = h;
                            if (COL2 > w)
                                COL2 = w;

                            if (region != null)
                                region.Dispose();
                            HOperatorSet.GenRectangle1(out region, ROW1, COL1, ROW2, COL2);
                            HOperatorSet.Complement(region, out HObject RegionComplement);
                            HOperatorSet.ReduceDomain(img, RegionComplement, out img);
                            if (region != null)
                                region.Dispose();
                            if (RegionComplement != null)
                                RegionComplement.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (SYSTEM_IO.PROCESSING == false)
                        return true;
                    SYSTEM_IO.PROCESSING = false;
                    retVal = false;
                    string err = "MaskMasks() err: " + ex.Message;
                    MessageBox.Show(err);

                }
                return retVal;
            }

            private bool DebrisCheck(ref HObject maskedimage, ref HObject labelimage, bool inspectlightareas)
            {
                bool retVal = true;
                HObject imgSobel = null, rgnThreshold = null, rgnSelectedDark = null, rgnSelectedLight = null, rgnUnion = null, rgnDilation = null;
                HObject rgnFillup = null, imgReduced = null, rgnDarkObjects = null, rgnLightObjects = null, rgnConnected = null, rgnSelected = null;
                HTuple mean = 0, numDarkObjects = 0, numLightObjects = 0;
                try
                {
                    HOperatorSet.Intensity(variationROI, maskedimage, out mean, out HTuple deviation);
                    //Find Marks
                    HOperatorSet.SobelAmp(maskedimage, out imgSobel, "sum_abs", iParams.SobelAmpSize);
                    HOperatorSet.Threshold(imgSobel, out rgnThreshold, 40, 255);
                    HOperatorSet.Connection(rgnThreshold, out rgnConnected);

                    //Elininate Squid
                    HOperatorSet.SelectShape(rgnConnected, out rgnSelected, "area", "and", 10, 999999);
                    HOperatorSet.Union1(rgnSelected, out rgnUnion);

                    //Merge fragments
                    HOperatorSet.DilationCircle(rgnUnion, out rgnDilation, 3.5);
                    HOperatorSet.FillUp(rgnDilation, out rgnFillup);
                    HOperatorSet.ReduceDomain(maskedimage, rgnFillup, out imgReduced);

                    //Select Dark objects
                    HOperatorSet.Threshold(imgReduced, out rgnDarkObjects, 0, mean * iParams.MeanOffset);

                    //select bright and DO NOT merge with dark !
                    if (inspectlightareas == true)
                    {
                        HOperatorSet.Threshold(imgReduced, out rgnLightObjects, mean * iParams.MeanOffset, 255);

                        //reduce bright object size and select reject fragments
                        HOperatorSet.ErosionCircle(rgnLightObjects, out rgnLightObjects, 2.5);
                        if (rgnConnected != null)
                            rgnConnected.Dispose();
                        HOperatorSet.Connection(rgnLightObjects, out rgnConnected);
                        HOperatorSet.SelectShape(rgnConnected, out rgnSelectedLight, "area", "and", iParams.DebrisMinSize, 999999);
                        HOperatorSet.CountObj(rgnSelectedLight, out numLightObjects);
                    }

                    //reduce dark object size and select reject fragments
                    HOperatorSet.ErosionCircle(rgnDarkObjects, out rgnDarkObjects, 2.5);
                    if (rgnConnected != null)
                        rgnConnected.Dispose();
                    HOperatorSet.Connection(rgnDarkObjects, out rgnConnected);
                    HOperatorSet.SelectShape(rgnConnected, out rgnSelectedDark, "area", "and", iParams.DebrisMinSize, 999999);
                    HOperatorSet.CountObj(rgnSelectedDark, out numDarkObjects);

                    HWIN.HalconWindow.SetColor("red");
                    if (numDarkObjects > 0)
                        HWIN.HalconWindow.DispObj(rgnSelectedDark);

                    HWIN.HalconWindow.SetColor("cyan");
                    if (numLightObjects > 0)
                        HWIN.HalconWindow.DispObj(rgnSelectedLight);
                }
                catch (Exception ex)
                {
                    retVal = false;
                    if (SYSTEM_IO.PROCESSING == false)
                        return true;
                    SYSTEM_IO.PROCESSING = false;
                    string err = "DebrisCheck() err: " + ex.Message;
                    MessageBox.Show(err);
                }
                finally
                {
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

            public bool InspectOpZone(ref HObject img, ref HObject rgnZone, VDEItem ozd, bool pausetest, bool inspectlightregions, ref List<string> errors, double inneradius)
            {
                bool retVal = true;
                HObject rgnComplement = null, imgReduced = null, tmpThresholdRegion = null, connectedRegions = null, minImage = null;
                HObject maxImage = null, constImg = null, imgCleared = null, tmpObj = null, selectedRegions = null, connectedOPZoneRegions = null, imgTmp = null;

                string opZoneName = "";
                try
                {
                    if (ozd.ODP.OPZoneIDVar == null)
                        return retVal;
                    opZoneName = ozd.VDEItemName;
                    HOperatorSet.ReduceDomain(img, rgnZone, out imgReduced);
                    HOperatorSet.CropDomain(imgReduced, out tmpObj);
                    if (Defaults.DarkLabel == false)
                        HOperatorSet.PrepareVariationModel(ozd.ODP.OPZoneIDVar, Defaults.absThreshold50, Defaults.varThreshold);
                    else
                        HOperatorSet.PrepareVariationModel(ozd.ODP.OPZoneIDVar, Defaults.absThresholdDark, Defaults.varThreshold0);

                    HOperatorSet.GenRectangle1(out HObject opzROI, ozd.ODP.OPZoneVARRegion[0], ozd.ODP.OPZoneVARRegion[1], ozd.ODP.OPZoneVARRegion[2], ozd.ODP.OPZoneVARRegion[3]);

                    HOperatorSet.CopyObj(img, out imgTmp, 1, 1);
                    maskVDEByOpZone(ref img, opzROI, ozd.OpZoneName);
                    HOperatorSet.ReduceDomain(img, opzROI, out img);
                    HOperatorSet.CropDomain(img, out img);
                    compareZoneImageToModel(ozd, img, ref imgReduced, inneradius);
                    if (img != null) img.Dispose();
                    HOperatorSet.CopyObj(imgTmp, out img, 1, 1);

                }
                catch (Exception ex)
                {
                    retVal = false;
                    string err = "InspectOpZone() err: " + "region: " + opZoneName + ": " + ex.Message;
                    MessageBox.Show(err);
                }
                finally
                {
                    if (imgTmp != null) imgTmp.Dispose();
                    if (rgnComplement != null) rgnComplement.Dispose();
                    if (minImage != null) minImage.Dispose();
                    if (maxImage != null) maxImage.Dispose();
                    if (constImg != null) constImg.Dispose();
                    if (tmpObj != null) tmpObj.Dispose();
                    if (tmpObj != null) tmpObj.Dispose();
                    if (imgReduced != null) imgReduced.Dispose();
                    if (tmpThresholdRegion != null) tmpThresholdRegion.Dispose();
                    if (connectedRegions != null) connectedRegions.Dispose();
                    if (connectedOPZoneRegions != null) connectedOPZoneRegions.Dispose();
                    if (selectedRegions != null) selectedRegions.Dispose();
                    if (imgCleared != null) imgCleared.Dispose();
                }
                return retVal;
            }

            private bool maskVDEByOpZone(ref HObject img, HObject region, string opzonename)
            {
                bool retVal = true;
                try
                {
                    retVal = maskVDE(90, 0, ref img, region, opzonename);
                    if (retVal == false) return retVal;
                    retVal = maskVDE(180, 90, ref img, region, opzonename);
                    if (retVal == false) return retVal;
                    retVal = maskVDE(270, 180, ref img, region, opzonename);
                    if (retVal == false) return retVal;
                    retVal = maskVDE(360, -90, ref img, region, opzonename);
                    if (retVal == false) return retVal;
                }
                catch (Exception ex)
                {
                    string err = "maskVDEByOpZone() err: " + ex.Message;
                    MessageBox.Show(err);
                }
                return retVal;
            }

            private bool maskVDE(int rotatedangle, int angle, ref HObject img, HObject region, string opzonename)
            {
                bool retVal = true;
                HObject RegionComplement = null, imgRotated = null, rgn = null;
                try
                {
                    HOperatorSet.GenEmptyObj(out imgRotated);
                    rotateBackground(angle, ref img, ref imgRotated);
                    foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(x => x.OpZoneName.ToUpper() == opzonename.ToUpper() && x.IsVDE == true && x.VDERotatedAngle == angle))
                    {

                        if (RegionComplement != null)
                            RegionComplement.Dispose();
                        if (rgn != null)
                            rgn.Dispose();

                        //if (vdi.RepeatType > 0)
                        //{
                        HOperatorSet.GenRectangle1(out rgn, vdi.VDERegion[0] - 4, vdi.VDERegion[1] - 8, vdi.VDERegion[2] + 4, vdi.VDERegion[3] + 8);
                        HOperatorSet.Complement(rgn, out RegionComplement);
                        HOperatorSet.ReduceDomain(img, RegionComplement, out img);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    string err = "maskVDE() err: " + ex.Message;
                    MessageBox.Show(err);
                }
                finally
                {
                    if (RegionComplement != null)
                        RegionComplement.Dispose();
                    if (rgn != null)
                        rgn.Dispose();
                    if (imgRotated != null)
                        imgRotated.Dispose();
                }
                return retVal;
            }


            private bool compareZoneImageToModel(VDEItem ozd, HObject tmpobj, ref HObject img, double innerradius)
            {
                bool retVal = true;
                HObject regions = null, selectedRegions = null, connectedRegions = null;
                HTuple numDiffs = 0;
                HTuple features = new HTuple("inner_radius");
                HTuple radiusSmall = innerradius;
                HTuple radiusLarge = 500.1;
                bool varRegions = false;
                try
                {
                    HOperatorSet.CompareVariationModel(tmpobj, out regions, ozd.ODP.OPZoneIDVar);
                    if (regions.IsInitialized())
                        if (regions.CountObj() > 0)
                            varRegions = true;
                    if (varRegions)
                    {
                        HOperatorSet.Connection(regions, out regions);
                        HOperatorSet.SelectShape(regions, out selectedRegions, features, "and", radiusSmall, radiusLarge);
                        HOperatorSet.SelectShape(selectedRegions, out connectedRegions, "area", "and", ozd.DarkMinSizeVAR, 999999);
                        HOperatorSet.Connection(connectedRegions, out connectedRegions);
                        HOperatorSet.CountObj(connectedRegions, out numDiffs);
                        //if (numDiffs.Length > 0)
                        //    if (numDiffs > 0)
                        //    {
                        //        HWIN.HalconWindow.SetColor("violet");
                        //        HWIN.HalconWindow.DispObj(connectedRegions);
                        //    }
                    }
                }
                catch (Exception ex)
                {
                    retVal = false;
                    string err = "compareZoneImageToModel() err: " + ex.Message;
                    MessageBox.Show(err);
                }
                finally
                {
                    if (regions != null) regions.Dispose();
                    if (connectedRegions != null) connectedRegions.Dispose();
                    if (selectedRegions != null) selectedRegions.Dispose();
                }
                return retVal;
            }
            #endregion
        }

        private void FitScreen()
        {
            hWinOCR.SetFullImagePart();
            hWinOCR.Refresh();
        }

        private void mask_buttons_EnabledChanged(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Enabled)
                btn.BackColor = Color.Teal;
            else
                btn.BackColor = Color.LightGray;
        }

        private void mnuRestart_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            dr = MessageBox.Show("Restart Setup?", "Label Configuration", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Stop);
            if (dr == DialogResult.Yes)
                doRestart();
        }

        private void mnuRunTestRT_Click(object sender, EventArgs e)
        {
            MoveToFirstLabel();
            runTest(INSPECT_LIGHT_AREAS);
            if (testErrors.Count > 0)
                showErrors(testErrors);
            mnuClose.Enabled = true;
            MoveToFirstLabel();
            enableBackForward(true);
            enableControls();
        }

        private void enableBackForward(bool enabled)
        {
            if (cmdBack.InvokeRequired)
            {
                cmdBack.Invoke((MethodInvoker)delegate
                {
                    if (enabled)
                    {
                        cmdBack.Enabled = false;
                        cmdForward.Enabled = true;
                    }
                    else
                    {
                        cmdBack.Enabled = false;
                        cmdForward.Enabled = false;
                    }
                });
            }
            else
            {
                if (enabled)
                {
                    cmdBack.Enabled = false;
                    cmdForward.Enabled = true;
                }
                else
                {
                    cmdBack.Enabled = false;
                    cmdForward.Enabled = false;
                }
            }
        }


        private void doDarkLabelSegmentation(bool isdark)
        {
            try
            {
                if (isdark)
                {
                    INSPECT_LIGHT_AREAS = true;
                    mnuLightMarks.Text = "Light Marks/Blemish Inspection: On";
                    mnuLightMarks.CheckState = CheckState.Checked;

                    Defaults.DarkLabel = true;
                    if (VDEReelConfig.VDEItems.Count > 0)
                    {
                        foreach (VDEItem vdeItem in VDEReelConfig.VDEItems.Where(x => x.IsVDE))
                        {
                            if (vdeItem != null)
                            {
                                if (vdeItem.OpZoneName != "")
                                {
                                    vdeItem.DarkSegmentContrast = Defaults.DarkLabelContrast;
                                    vdeItem.tr.SegmentContrast = Defaults.DarkLabelContrast;
                                }
                            }
                        }
                    }
                }
                else
                {
                    INSPECT_LIGHT_AREAS = false;
                    Defaults.DarkLabel = false;
                    mnuLightMarks.Text = "Light Marks/Blemish Inspection: Off";
                    mnuLightMarks.CheckState = CheckState.Unchecked;

                    if (VDEReelConfig.VDEItems.Count > 0)
                    {
                        foreach (VDEItem vde in VDEReelConfig.VDEItems.Where(x => x.IsVDE))
                        {
                            //if (vde.OpZoneName != "")
                            //{
                            //    switch (FILTER)
                            //    {
                            //        case 1:
                            //            DarkSegmentContrast = vde.FILTER1;
                            //            break;
                            //        case 2:
                            //            DarkSegmentContrast = vde.FILTER2;
                            //            break;
                            //        case 3:
                            //            DarkSegmentContrast = vde.FILTER3;
                            //            break;
                            //    }
                            //}
                        }
                    }
                }
                if (VDEReelConfig.VDEItems.Count > 0)
                {
                    if (Defaults.DarkLabel == true)
                        foreach (VDEItem opZone in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
                            generateOPZone(opZone);
                    MoveToFirstLabel();
                }
            }
            catch (Exception ex)
            {
                string err = "doDarkLabelSegmentation() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        private void mnuDarkLabel_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsi = null;
            try
            {
                if (sender is ToolStripMenuItem)
                {
                    tsi = (ToolStripMenuItem)sender;
                    if (tsi.CheckState == CheckState.Checked)
                    {
                        doDarkLabelSegmentation(false);
                        tsi.Text = "Light Marks/Blemish Inspection: Off";
                        tsi.CheckState = CheckState.Unchecked;
                    }
                    else
                    {
                        doDarkLabelSegmentation(true);
                        tsi.Text = "Light Marks/Blemish Inspection: On";
                        tsi.CheckState = CheckState.Checked;
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "mnuDarkLabel_Click() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        //private void mnuDeformation_Click(object sender, EventArgs e)
        //{
        //    ToolStripMenuItem tsi = null;
        //    MSERParams tmpMSER = DataManager.GetMSERParams(LabelTYPE);
        //    if (sender is ToolStripMenuItem)
        //    {
        //        tsi = (ToolStripMenuItem)sender;

        //        if (tsi.CheckState == CheckState.Checked)
        //        {
        //            if (VDEReelConfig.VDEItems.Count > 0)
        //            {
        //                foreach (VDEItem deform in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
        //                {
        //                    if (deform.OpZoneName != "")
        //                    {
        //                        deform.MSER.DarkMinSizeMSER = tmpMSER.DarkMinSizeMSER;
        //                        deform.MSER.LightMinSizeMSER = tmpMSER.LightMinSizeMSER;
        //                    }
        //                }
        //            }
        //            MSERLabel.DarkMinSizeMSER = tmpMSER.DarkMinSizeMSER;
        //            MSERLabel.LightMinSizeMSER = tmpMSER.LightMinSizeMSER;
        //            tsi.Text = "Label Offset: Off";
        //            tsi.CheckState = CheckState.Unchecked;
        //        }
        //        else
        //        {
        //            if (VDEReelConfig.VDEItems.Count > 0)
        //            {

        //                foreach (VDEItem deform in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
        //                {
        //                    if (deform.OpZoneName != "")
        //                    {
        //                        deform.MSER.DarkMinSizeMSER = 200;
        //                        deform.MSER.LightMinSizeMSER = 200;
        //                    }
        //                }
        //            }
        //            MSERLabel.DarkMinSizeMSER = 200;
        //            MSERLabel.LightMinSizeMSER = 200;
        //            tsi.Text = "Label Offset: On";
        //            tsi.CheckState = CheckState.Checked;
        //        }
        //        if (VDEReelConfig.VDEItems.Count > 0)
        //        {
        //            MoveToFirstLabel();
        //            //runTest();
        //            //DoSummary();
        //        }
        //    }
        //}

        private void mnuLightMarks_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsi = null;
            if (sender is ToolStripMenuItem)
            {
                tsi = (ToolStripMenuItem)sender;
                if (tsi.CheckState == CheckState.Checked)
                {
                    INSPECT_LIGHT_AREAS = false;
                    tsi.Text = "Light Marks/Blemish Inspection: Off";
                    tsi.CheckState = CheckState.Unchecked;
                }
                else
                {
                    INSPECT_LIGHT_AREAS = true;
                    tsi.Text = "Light Marks/Blemish Inspection: On";
                    tsi.CheckState = CheckState.Checked;
                }
                if (VDEReelConfig.VDEItems.Count > 0)
                {
                    MoveToFirstLabel();
                    //runTest();
                }
            }
        }


        private void cmdGenerate_Click(object sender, EventArgs e)
        {
            disableControls();
            Task t = new Task(doGenerateModels);
            t.Start();
        }


        private void optimizeVDE()
        {
            //optimizeMSERValue(imgReduced, vdi.VDEData.Length, ref vi);
        }

        private void doGenerateModels()
        {
            try
            {
                optimizeVDE();
                MoveToFirstLabel();
                DisplayInfo("creating models..", "top");
                createOpZoneVariationModels();
                DoSummary();
                drawVDEBorders();
                enableBackForward(true);
                enableControls();
            }
            catch (Exception ex)
            {
                string err = "doGenerateModels() err: " + ex.Message + Environment.NewLine + "Please re-try the generate models option";
                MessageBox.Show(err);
            }
            finally
            {
                if (cmdForward.InvokeRequired)
                {
                    cmdGenerate.Invoke((MethodInvoker)delegate
                    {
                        cmdGenerate.Enabled = true;
                        mnuRunTestRT.Enabled = true;
                    });
                }
                else
                {
                    cmdGenerate.Enabled = true;
                    mnuRunTestRT.Enabled = true;
                }
                showSpinner(false);
            }
        }

        private void enableControls()
        {
            cmdAddMask.Invoke((MethodInvoker)delegate
            {
                //cmdLabelZone.Enabled = true;
                cmdAddMask.Enabled = true;
                cmdAddZone.Enabled = true;
                //cmdAddAffine.Enabled = true;
                cmdGenerate.Enabled = true;
                enableBackForward(true);
            });
        }

        private void disableControls()
        {
            cmdAddMask.Invoke((MethodInvoker)delegate
            {
                //cmdLabelZone.Enabled = false;
                cmdAddMask.Enabled = false;
                cmdAddZone.Enabled = false;
                //cmdAddAffine.Enabled = false;
                cmdGenerate.Enabled = false;
                enableBackForward(false);
            });
        }

        //private void cboCharacterContrast_DropDownClosed(object sender, EventArgs e)
        //{    
        //    doSelectFilter(sender, e);
        //}

        //private void doSelectFilter(object sender, EventArgs e)
        //{
        //    
        //    ToolStripComboBox cb = null;
        //    try
        //    {
        //        //cboCharacterContrast.Enabled = true;
        //        if (sender is ToolStripComboBox)
        //        {
        //            MoveToFirstLabel();
        //            cb = (ToolStripComboBox)sender;
        //            if (LabelTYPE == LabelType.DARK)
        //            {
        //                //cboCharacterContrast.SelectedIndex = -1;
        //                //cboCharacterContrast.Enabled = false;
        //            }
        //            foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(x => x.IsVDE))
        //            {
        //                if (LabelTYPE == LabelType.DARK)
        //                {
        //                    vdi.DarkSegmentContrast = Defaults.DarkLabelContrast;
        //                    vdi.tr.SegmentContrast = Defaults.DarkLabelContrast;
        //                }
        //                else
        //                    vdi.tr.SegmentContrast = getSegmentContrastFromComboItem(cb);
        //            }
        //            //mnuOptions.HideDropDown();

        //            List<VDEItem> ozdList = new List<VDEItem>();
        //            List<VDEItem> vdeList = null;
        //            foreach (VDEItem ozd in VDEReelConfig.VDEItems.Where(x => x.IsOPZone && x.OpZoneName != ""))
        //                ozdList.Add(ozd);
        //            for (int x = 0; x < ozdList.Count; x++)
        //            {
        //                vdeList = new List<VDEItem>();
        //                foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(y => y.IsVDE && y.OpZoneName == ozdList[x].OpZoneName))
        //                    vdeList.Add(vdi);

        //                for (int z = vdeList.Count - 1; z >= 0; z--)
        //                {
        //                    vdeList[z].DestroyVars();
        //                    VDEItem vdiRemove = (VDEItem)vdeList[z];
        //                    VDEReelConfig.VDEItems.Remove(vdiRemove);
        //                    vdeList.RemoveAt(z);
        //                }
        //            }
        //            foreach (VDEItem ozd in ozdList)
        //            {
        //                HObject rgnOZD = null;
        //                HOperatorSet.GenRectangle1(out rgnOZD, ozd.ODP.OPZoneVARRegion[0], ozd.ODP.OPZoneVARRegion[1], ozd.ODP.OPZoneVARRegion[2], ozd.ODP.OPZoneVARRegion[3]);
        //                autoFindVDEMethods(rgnOZD, ozd.OpZoneName);
        //                if (rgnOZD != null)
        //                    rgnOZD.Dispose();
        //            }
        //            DoSummary();
        //            drawVDEBorders();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string err = "doSelectFilter() (DropDownClosed) err: " + ex.Message;
        //        MessageBox.Show(err);
        //    }
        //}

        //private int getSegmentContrastFromComboItem(ToolStripComboBox cb)
        //{
        //    int retVal = -1;
        //    try
        //    {
        //        string size = cb.SelectedItem.ToString();
        //        foreach (FontSizesSegment fss in listFSS)
        //        {
        //            if (fss.Size.ToLower() == size.ToLower())
        //            {
        //                retVal = fss.SegmentContrast;
        //                return retVal;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string err = "getSegmentContrastFromComboItem() err: " + ex.Message;
        //        MessageBox.Show(err);
        //    }
        //    return retVal;
        //}       

        private void scroll_images_clicked(object sender, EventArgs e)
        {
            Button btn = null;
            if (sender is Button)
            {
                btn = (Button)sender;
                switch (btn.Name)
                {
                    case "cmdBack":
                        showPreviousLabel(INSPECT_LIGHT_AREAS);
                        break;
                    case "cmdForward":
                        showNextLabel(INSPECT_LIGHT_AREAS);
                        break;
                }
            }
        }

        private List<uscVDEItem> MedDataList = new List<uscVDEItem>();

        private void buildMedDataItemList()
        {
            try
            {
                MedDataList.Clear();
                uscVDEItem uvde = null;
                foreach (Control ctrl in fplvdeitems.Controls)
                {
                    if (ctrl is uscVDEItem)
                    {
                        uvde = (uscVDEItem)ctrl;
                        MedDataList.Add(uvde);
                    }
                }
            }
            catch   (Exception ex)
            {
                string err = "buildMedDataItemList() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        private void showPreviousLabel(bool inspectlightareas)
        {
            HObject tmpObj = null;
            try
            {
                if (cmdBack.InvokeRequired)
                {
                    cmdBack.Invoke((MethodInvoker)delegate
                    {
                        cmdBack.Enabled = false;
                        HTuple trgImgCount = 0;
                        HOperatorSet.CountObj(TestImages, out trgImgCount);
                        if (imageIndex > 1)
                        {
                            foreach (uscVDEItem data in MedDataList)
                                data.MoveToPreviousVDE();
                            imageIndex--;
                            cmdForward.Enabled = true;
                            cmdBack.Enabled = true;
                            if (imageIndex == 1)
                                cmdBack.Enabled = false;
                            hWinOCR.HalconWindow.ClearWindow();
                            hWinOCR.HalconWindow.DetachBackgroundFromWindow();
                            if (mouseOverImage != null)
                                mouseOverImage.Dispose();
                            HOperatorSet.SelectObj(TestImages, out mouseOverImage, imageIndex);
                            HOperatorSet.ReduceDomain(mouseOverImage, VariationROI, out mouseOverImage);
                            HOperatorSet.AttachBackgroundToWindow(mouseOverImage, hWinOCR.HalconWindow);

                            runTest(imageIndex, inspectlightareas);

                            if (DebrisAndErrors.Listing.Count > 0)
                            {
                                DebrisAndErrors mo = DebrisAndErrors.Listing[imageIndex - 1];
                                if (mo.DebrisObjects.IsInitialized())
                                {
                                    HOperatorSet.SelectObj(mo.DebrisObjects, out tmpObj, 1);
                                    HOperatorSet.DispObj(tmpObj, hWinOCR.HalconWindow);
                                }
                            }
                        }

                    });
                }
                else
                {
                    cmdBack.Enabled = false;
                    HTuple trgImgCount = 0;
                    HOperatorSet.CountObj(TestImages, out trgImgCount);
                    if (imageIndex > 1)
                    {
                        foreach (uscVDEItem data in MedDataList)
                            data.MoveToPreviousVDE();
                        imageIndex--;
                        cmdForward.Enabled = true;
                        cmdBack.Enabled = true;
                        if (imageIndex == 1)
                        {
                            MoveToFirstLabel();
                            cmdBack.Enabled = false;
                        }
                        hWinOCR.HalconWindow.ClearWindow();
                        hWinOCR.HalconWindow.DetachBackgroundFromWindow();
                        if (mouseOverImage != null)
                            mouseOverImage.Dispose();
                        HOperatorSet.SelectObj(TestImages, out mouseOverImage, imageIndex);
                        HOperatorSet.ReduceDomain(mouseOverImage, VariationROI, out mouseOverImage);
                        HOperatorSet.AttachBackgroundToWindow(mouseOverImage, hWinOCR.HalconWindow);
                        runTest(imageIndex, inspectlightareas);
                        if (DebrisAndErrors.Listing.Count > 0)
                        {
                            DebrisAndErrors mo = DebrisAndErrors.Listing[imageIndex - 1];
                            if (mo.DebrisObjects.IsInitialized())
                            {
                                HOperatorSet.SelectObj(mo.DebrisObjects, out tmpObj, 1);
                                HOperatorSet.DispObj(tmpObj, hWinOCR.HalconWindow);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "showPreviousLabel() err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (tmpObj != null)
                    tmpObj.Dispose();
            }
        }

        private void MoveToFirstLabel()
        {
            HObject tmpImg = null;
            try
            {
                if (TrainingImages.IsInitialized() == false)
                    return;
                if (TrainingImages.CountObj() == 0)
                    return;

                imageIndex = 1;
                if (cmdForward.InvokeRequired)
                {
                    cmdForward.Invoke((MethodInvoker)delegate
                    {
                        cmdBack.Enabled = false;
                        foreach (uscVDEItem data in MedDataList)
                            data.Reset();
                        hWinOCR.HalconWindow.DetachBackgroundFromWindow();
                        HOperatorSet.CopyObj(TrainingImages, out tmpImg, imageIndex, 1);
                        if (Backgroundimage != null)
                            Backgroundimage.Dispose();
                        HOperatorSet.CopyObj(TrainingImages, out Backgroundimage, imageIndex, 1);

                        if (VariationROI.IsInitialized())
                        {
                            HOperatorSet.ReduceDomain(tmpImg, VariationROI, out tmpImg);
                            HOperatorSet.ReduceDomain(Backgroundimage, VariationROI, out Backgroundimage);
                        }
                        HOperatorSet.AttachBackgroundToWindow(tmpImg, hWinOCR.HalconWindow);
                        hWinOCR.Refresh();
                    });
                }
                else
                {
                    cmdBack.Enabled = false;
                    foreach (uscVDEItem data in MedDataList)
                        data.Reset();
                    hWinOCR.HalconWindow.DetachBackgroundFromWindow();
                    HOperatorSet.CopyObj(TrainingImages, out tmpImg, imageIndex, 1);
                    if (Backgroundimage != null)
                        Backgroundimage.Dispose();
                    HOperatorSet.CopyObj(TrainingImages, out Backgroundimage, imageIndex, 1);
                    if (VariationROI.IsInitialized())
                    {
                        HOperatorSet.ReduceDomain(tmpImg, VariationROI, out tmpImg);
                        HOperatorSet.ReduceDomain(Backgroundimage, VariationROI, out Backgroundimage);
                    }
                    HOperatorSet.AttachBackgroundToWindow(tmpImg, hWinOCR.HalconWindow);
                    hWinOCR.Refresh();
                }
            }
            catch (Exception ex)
            {
                string err = "MoveToFirstLabel() err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (tmpImg != null)
                    tmpImg.Dispose();
                setMasking(true);
            }
        }

        private void showNextLabel(bool inspectlightareas)
        {
            HObject tmpObj = null;
            try
            {
                if (cmdForward.InvokeRequired)
                {
                    cmdForward.Invoke((MethodInvoker)delegate
                    {
                        cmdForward.Enabled = false;
                        HTuple trgImgCount = 0;
                        HOperatorSet.CountObj(TestImages, out trgImgCount);
                        if (imageIndex < trgImgCount.I)
                        {
                            if (imageIndex > 0)
                            {
                                foreach (uscVDEItem data in MedDataList)
                                    data.MoveToNextVDE();
                            }
                            imageIndex++;
                            cmdForward.Enabled = true;
                            cmdBack.Enabled = true;
                            if (imageIndex == trgImgCount.I)
                                cmdForward.Enabled = false;
                            hWinOCR.HalconWindow.ClearWindow();
                            hWinOCR.HalconWindow.DetachBackgroundFromWindow();
                            if (mouseOverImage != null)
                                mouseOverImage.Dispose();
                            HOperatorSet.SelectObj(TestImages, out mouseOverImage, imageIndex);
                            HOperatorSet.ReduceDomain(mouseOverImage, VariationROI, out mouseOverImage);
                            HOperatorSet.AttachBackgroundToWindow(mouseOverImage, hWinOCR.HalconWindow);
                            runTest(imageIndex, inspectlightareas);
                            if (DebrisAndErrors.Listing.Count > 0)
                            {
                                DebrisAndErrors mo = DebrisAndErrors.Listing[imageIndex - 1];
                                if (mo.DebrisObjects.IsInitialized())
                                {
                                    hWinOCR.HalconWindow.SetColor("red");
                                    HOperatorSet.SelectObj(mo.DebrisObjects, out tmpObj, 1);
                                    HOperatorSet.DispObj(tmpObj, hWinOCR.HalconWindow);
                                }
                            }
                        }
                    });
                }
                else
                {
                    cmdForward.Enabled = false;
                    HTuple trgImgCount = 0;
                    HOperatorSet.CountObj(TestImages, out trgImgCount);
                    if (imageIndex < trgImgCount.I)
                    {
                        if (imageIndex > 0)
                        {
                            foreach (uscVDEItem data in MedDataList)
                                data.MoveToNextVDE();
                        }
                        imageIndex++;
                        cmdForward.Enabled = true;
                        cmdBack.Enabled = true;
                        if (imageIndex == trgImgCount.I)
                            cmdForward.Enabled = false;
                        hWinOCR.HalconWindow.ClearWindow();
                        hWinOCR.HalconWindow.DetachBackgroundFromWindow();
                        if (mouseOverImage != null)
                            mouseOverImage.Dispose();
                        HOperatorSet.SelectObj(TestImages, out mouseOverImage, imageIndex);
                        HOperatorSet.ReduceDomain(mouseOverImage, VariationROI, out mouseOverImage);
                        HOperatorSet.AttachBackgroundToWindow(mouseOverImage, hWinOCR.HalconWindow);
                        runTest(imageIndex, inspectlightareas);
                        if (DebrisAndErrors.Listing.Count > 0)
                        {
                            DebrisAndErrors mo = DebrisAndErrors.Listing[imageIndex - 1];
                            if (mo.DebrisObjects.IsInitialized())
                            {
                                HOperatorSet.SelectObj(mo.DebrisObjects, out tmpObj, 1);
                                HOperatorSet.DispObj(tmpObj, hWinOCR.HalconWindow);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "showNextLabel() err: " + ex.Message;
                MessageBox.Show(err);
            }
            finally
            {
                if (tmpObj != null)
                    tmpObj.Dispose();
            }
        }

        private void mnuFile_Click(object sender, EventArgs e)
        {
            enableBackForward(false);
        }


        private void variationDebrisSize(bool issmall)
        {
            if (issmall)
            {
                InnerRadius = DataManager.GetInnerRadiusDefaultSmall();
            }
            else
            {
                InnerRadius = DataManager.GetInnerRadiusDefaultLarge();
            }
            foreach (VDEItem vdi in VDEReelConfig.VDEItems.Where(x => x.IsOPZone))
                vdi.DarkMinSizeVAR = (issmall == true ? 50 : 100);
        }


        //private void inner_circle_clicked(object sender, EventArgs e)
        //{
        //    ToolStripMenuItem tsi = null;
        //    if (sender is ToolStripMenuItem)
        //    {
        //        tsi = (ToolStripMenuItem)sender;
        //        if (tsi.Name == "mnuIRS")
        //        {
        //            if (tsi.CheckState == CheckState.Unchecked)
        //            {
        //                InnerRadius = DataManager.GetInnerRadiusDefaultSmall();
        //                tsi.CheckState = CheckState.Checked;
        //                mnuIRL.Checked = false;
        //            }
        //        }
        //        else if (tsi.Name == "mnuIRL")
        //        {
        //            if (tsi.CheckState == CheckState.Unchecked)
        //            {
        //                InnerRadius = DataManager.GetInnerRadiusDefaultLarge();
        //                tsi.CheckState = CheckState.Checked;
        //                mnuIRS.Checked = false;
        //            }
        //        }
        //    }
        //}

        private void cmdZoomToFit_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            FitScreen();
            Cursor.Current = Cursors.Default;
        }

        private void cboLabelType_DropDownClosed(object sender, EventArgs e)
        {
            doSelectType(sender, e);
        }

        private void doSelectType(object sender, EventArgs e)
        {
            ToolStripComboBox cb = null;
            try
            {
                if (sender is ToolStripComboBox)
                {
                    cb = (ToolStripComboBox)sender;
                    switch (cb.SelectedIndex)
                    {
                        case 0:
                            LabelTYPE = LabelType.FLATPANEL;
                            iParams.SobelEdgeThreshold = iParamDefaults.SobelEdge1;
                            iParams.SobelAmpSize = iParamDefaults.SobelAmpSize1;
                            doDarkLabelSegmentation(false);
                            break;
                        case 1:
                            LabelTYPE = LabelType.DARK;
                            iParams.SobelEdgeThreshold = iParamDefaults.SobelEdge2;
                            iParams.SobelAmpSize = iParamDefaults.SobelAmpSize2;
                            doDarkLabelSegmentation(true);
                            break;
                        case 2:
                            LabelTYPE = LabelType.BOOKLET;
                            iParams.SobelEdgeThreshold = iParamDefaults.SobelEdge3;
                            iParams.SobelAmpSize = iParamDefaults.SobelAmpSize3;
                            doDarkLabelSegmentation(false);
                            break;
                    }
                    //mnuOptions.HideDropDown();
                }
            }
            catch (Exception ex)
            {
                string err = "doSelectType() (DropDownClosed) err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        //private void cmdAddAffine_Click(object sender, EventArgs e)
        //{
        //    setMasking(false);
        //    //hWinOCR.SetFullImagePart();
        //    //hWinOCR.HMoveContent = false;
        //    MoveToFirstLabel();
        //    enableBackForward(false);
        //    ADDING_ROT_ZONE = true;
        //    HOperatorSet.SetDraw(hWinOCR.HalconWindow, "margin");
        //    HOperatorSet.SetLineWidth(hWinOCR.HalconWindow, 2);
        //    HOperatorSet.SetLineStyle(hWinOCR.HalconWindow, new HTuple());
        //    HOperatorSet.SetColor(hWinOCR.HalconWindow, "cornflower blue");
        //    Thread t = new Thread(doAddRotZone);
        //    t.Start();
        //}

        //private void doAddRotZone()
        //{
        //    HObject rotatedZone = null;
        //    try 
        //    {
        //        DisplayInfo("please define the diagonal area. Right-mouse-click anywhere on label to complete the zone", "top");
        //        hWinOCR.HalconWindow.DrawRectangle2(out double row, out double col, out double phi, out double length1, out double length2);
        //        DialogResult dr = new DialogResult();
        //        dr = MessageBox.Show("Save rotated region?", "Suppress diagonal bleedthrough", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        //        if (dr == DialogResult.Yes)
        //        {
        //            HOperatorSet.GenRectangle2(out rotatedZone, row, col, phi, length1, length2);
        //            hWinOCR.HalconWindow.DispObj(rotatedZone);
        //            AffineRegion.RowCenter = row;
        //            AffineRegion.ColumnCenter = col;
        //            AffineRegion.PHI = phi;
        //            AffineRegion.Length1 = length1;
        //            AffineRegion.Length2 = length2;
        //            createRotRegion();
        //            //setMasking(true);
        //            //DoSummary();
        //            //drawVDEBorders();
        //            //DisplayInfo("", "top");
        //            //return;
        //        }
        //        else
        //        {
        //            AffineRegion.RowCenter = 0;
        //            AffineRegion.ColumnCenter = 0;
        //            AffineRegion.PHI = 0;
        //            AffineRegion.Length1 = 0;
        //            AffineRegion.Length2 = 0;
        //        }
        //        setMasking(true);
        //        DoSummary();
        //        drawVDEBorders();
        //        DisplayInfo("", "top");

        //    }
        //    catch (Exception ex)
        //    {
        //        string err = "doAddRotZone() err: " + ex.Message;
        //        MessageBox.Show(err);
        //    }
        //    finally
        //    {
        //        if (rotatedZone != null)
        //            rotatedZone.Dispose();
        //    }
        //}



        private void doSetDebrisSize()
        {
            if (cboDebrisSize.SelectedIndex == 0)
                iParams.DebrisMinSize = iParamDefaults.DebrisMinSize1;
            else if (cboDebrisSize.SelectedIndex == 1)
                iParams.DebrisMinSize = iParamDefaults.DebrisMinSize2;
            else
                iParams.DebrisMinSize = iParamDefaults.DebrisMinSize3;

            //mnuOptions.HideDropDown();
        }

        private void cboDebrisSize_DropDownClosed(object sender, EventArgs e)
        {
            doSetDebrisSize();
        }

        private void cboVariationDebrisSize_DropDownClosed(object sender, EventArgs e)
        {
            if (cboVariationDebrisSize.SelectedIndex == 0)
            {
                variationDebrisSize(true);
                IsSmallDebrisSizeVar = true;
            }
            else
            {
                variationDebrisSize(false);
                IsSmallDebrisSizeVar = false;
            }
        }


        private void frmLabelConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Alt || Control.ModifierKeys == Keys.F4)
            {
                e.Cancel = true;
                return;
            }
        }

        private void deleteInOpZoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (VDEReelConfig.RemoveVDEFromZone())
            {
                DoSummary();
                drawVDEBorders();
                mnuVDE.Enabled = true;
            }
        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Delete all VDE items from the label?\n\n" + "Note: You can add VDE manually again by selecting menu option:\nVDE -> Manual Definition", "Delete All VDE Items", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                VDEReelConfig.RemoveAllVDE();
                DoSummary();
                drawVDEBorders();
                mnuVDE.Enabled = true;
            }
        }

        private void manualDefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startVDEManual();
        }
    }
}

