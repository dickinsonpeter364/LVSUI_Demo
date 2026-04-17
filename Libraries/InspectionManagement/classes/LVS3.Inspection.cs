using AlkUSB3;
using CONSTANTS;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using static LVS3.CameraManager;
using static LVS3.Delegates;
using static LVS3.Enums;

namespace LVS3
{
    public static class Positioner
    {
        public static bool CAN_MOVE_NEXT = true;
        public static int ImageCounter = 1;

        public static void MoveNext(INSPECTION i)
        {            
            if (CAN_MOVE_NEXT == true)
            {
                i.MoveToNextVDE();
                ImageCounter++;                
            }            
        }

        public static void MoveLast(INSPECTION i)
        {            
            i.MoveLast();
            //ImageCounter++;            
        }
    }

    public static class SpeedControl
    {
        private static int speedUpdateCounter = 0;
        private static int speedControlUpdateInterval = Defaults.SpeedControlUpdateInterval;
        private static long durationAccumulater = 0;

        public static void UpdateSpeedControl(long duration)
        {            
            if (duration > 0)
                speedUpdateCounter += 1;
            durationAccumulater += duration;
            if (speedUpdateCounter % speedControlUpdateInterval == 0)
            {
                durationAccumulater /= speedControlUpdateInterval;
                long durationAverage = durationAccumulater;
                speedUpdateCounter = 0;
                durationAccumulater = 0;
                if (durationAverage > 0)
                {
                    Thread t = new Thread(() => writeSpeedToPLC(durationAverage));
                    t.Start();
                }
            }
        }

        private static void writeSpeedToPLC(long duration)
        {
            int intDuration = Convert.ToInt32(duration);            
            mxClient.WriteToRegister(1, "Speed_Control", intDuration, 3);            
        }
    }

    public class INSPECTION // : IDisposable
    {
        private frmInspect frmI = null;
        private double ConfidenceLevel = DataManager.GetConfidenceLevel();
        private InspectionParams iParams;
        private LabelType labelType;
        private string SAMPLE_LABEL = "";        
        public static bool SampleIncluded;
        public static LabelCounts Labelcounts = null;
        private double InnerRadius = 0;
        private bool INSPECT_LIGHT_AREAS = false;
        private List<FontSizesSegment> FontsizesSegment = new List<FontSizesSegment>();
        public bool ENABLE_CAPTURE = false;
        public static event ErrorMethodHandler EMH = null;
        public event AlarmMethodHandler AMH = null;
        public bool FIXED_DATA = false;
        public  Action moveNextCaller = null;
        public string TRG_FOLDER = "";
        public string FONT_FOLDER = "";
        public string FAIL_FOLDER = "";
        public int countMeds = 0;
        public string LABEL_ITEM = "";
        public string REEL_LPN = "";
        public string LWO = "";
        public Delegates.SystemMessageHandler SM;
        private bool INSPECTING = false;
        private bool REVIEWING = false;
        private  List<MedData> MedDataItems = new List<MedData>();
        private  HObject CurrentRawImage = null;
        private  HObject CurrentReducedImage = null;
        public  int LABEL_COUNT = 0;
        private  Action pauseMethodCaller = null;
        private  Action alarmMethodCaller = null;
        private  List<VDEItem> VDEItems = new List<VDEItem>();
        private  List<OPZoneData> OPZoneItems = new List<OPZoneData>();
        private System.Windows.Forms.Button cmdEndInspection = null;
        public  static uscMessageDisplay uscMD = null;
        private  CameraNecta cameraNecta;
        private  HSmartWindowControl hWin = null;
        private System.Windows.Forms.Label lblInfoText = null;
        private System.Windows.Forms.Label lblTime = null;
        private  PictureBox pbPassFail = null;
        private  HObject VariationRegion = null;
        private  HObject VariationImg = null;
        private  int[] VariationRegionCoords = new int[] { 0, 0, 0, 0 };
        public string VariableMedDataPH = "";
        private bool MultiKitsPerPatient = false;
        private int QntyKitsPerPatient = 0;
        public static bool medFailOnCurrentLabel = false;        
        private bool medFailOnPreviousLabel = false;
        public static bool medFailInCurrentPatient = false;
        private bool medFailInPreviousPatient = false;
                
        private int labelStopIndex = 0;
        private int stopAtIndex = 0;
        //private int imageIndex = 0;
        private string correctREEL_LPN = "";
        private bool reviewSampleLUIDone = false;
        private bool InhibitNextCapture = false;
        public static bool MissingLabelTrigger = false;        
        public string ReviewLUIMode = "Auto"; 
        public static int ReviewLUIModeIndex = 0;
        private bool EdgesNotDetectedError = false;
        

        public INSPECTION()
        {
            
        } 
        
        public  bool LoadInspectionDataFromDB(string lpn, string labelitem)
        {            
            bool retVal = true;
            try
            {
                labelType = DataManager.GetLabelType(Defaults.StationID, labelitem);
                //MSERLabel = DataManager.GetMSERParams(labelType);                
                REEL_LPN = lpn;

                LABEL_ITEM = labelitem;
                InnerRadius = DataManager.GetInnerRadius(Defaults.StationID, LABEL_ITEM);
                labelType = DataManager.GetLabelType(Defaults.StationID, LABEL_ITEM);
                FONT_FOLDER = ImageData.CreateFontFilepathAllMachines();
                TRG_FOLDER = ImageData.CreateTrainingFilepath(lpn);
                FAIL_FOLDER = ImageData.CreateFailFilepath(lpn);
                string IMAGE_FOLDER = ImageData.CreateImageFilepath();
                ImageData.DeleteFailData(FAIL_FOLDER);
                FAIL_FOLDER = ImageData.CreateFailFilepath(lpn);
                VariationRegionCoords = DataManager.GetVariationRegion(Defaults.StationID, LABEL_ITEM);
                INSPECT_LIGHT_AREAS = DataManager.GetInspectLightAreas(Defaults.StationID, LABEL_ITEM);
                int LabelID = DataManager.LabelID(Defaults.StationID, LABEL_ITEM);
                iParams = DataManager.GeInspectionParams(Defaults.StationID, LabelID);
                iParams.LabelID = LabelID;
                iParams.InspectBright = INSPECT_LIGHT_AREAS;

                if (VariationRegion != null)
                    VariationRegion.Dispose();
                HOperatorSet.GenRectangle1(out VariationRegion, VariationRegionCoords[0], VariationRegionCoords[1], VariationRegionCoords[2], VariationRegionCoords[3]);
                retVal = loadOPZoneData(LabelID);


                DialogResult dr = MessageBox.Show("Does this roll of labels include a sample label?", "Label Inspection", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    string sample = DataManager.GetSample(lpn);
                    SAMPLE_LABEL = sample;
                    SampleIncluded = true;                    
                    DataManager.SaveAction("Sample Label on reel: YES", REEL_LPN, "Inspection", Defaults.UserName, "SAMPLE: " + sample, "LIN: " + labelitem, "Reel includes sample label");
                }
                else if (dr == DialogResult.No)
                {
                    SAMPLE_LABEL = "";
                    SampleIncluded = false;
                    DataManager.SaveAction("Sample Label on reel: NO", REEL_LPN, "Inspection", Defaults.UserName, "LoadInspectionDataFromDB()", "LIN: " + labelitem, "Reel does not include sample label");
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "LoadInspectionDataFromDB() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Initialize Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
            return retVal;
        }

        public bool InitInspection(frmInspect frmi) // string lpn, HSmartWindowControl hwin, HSmartWindowControl hWinlayout, uscMessageDisplay uscmd, Button cmdstart, Button cmdendinspection, Label lblinfotext, Label lbltime, PictureBox pbpass)
        {
            bool retVal = true;
            try
            {
                this.frmI = frmi;
                Labelcounts = new LabelCounts();
                Labelcounts.InspectLightArea = INSPECT_LIGHT_AREAS;
                FontsizesSegment = DataManager.GetListFontSizes();
                AMH += frmInspect.AMD;
                EMH += frmInspect.EMD;
                
                INSPECTING = false;
                REVIEWING = false;
                SYSTEM_IO.PROCESSING = false;
                //labelLength = 0;
                SYSTEM_IO.IO_INTERRUPT_Handler -= IO_INTERRUPT_Handler;
                SYSTEM_IO.IO_INTERRUPT_Handler += IO_INTERRUPT_Handler;
                SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
                SYSTEM_IO.IO_CHANGE_Handler += IO_COS_Handler;
                cameraNecta = CameraManager.NectaCameras[0];
                cameraNecta.nectaCam.Acquire = false;
                cameraNecta.SetChannelMethodCallerFalse();
                uscMD = frmI.uscMD;
                cmdEndInspection = frmI.cmdEndInspection;
                Positioner.ImageCounter = 1;
                Positioner.CAN_MOVE_NEXT = false;
                hWin = frmI.hWinCurrent;
                lblInfoText = frmI.lblInfoText;
                lblTime = frmI.lblTime;
                pbPassFail = frmI.pbPass;
                LWO = "";
                REEL_LPN = frmI.REEL;
                lblInfoText.Text = REEL_LPN;
                countMeds = 0;
                ENABLE_CAPTURE = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "InitInspection() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Initialize Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
            return retVal;
        }

        public bool loadVDEToolsAndData(string reel_lpn, string lin, string lwo, ref System.Windows.Forms.FlowLayoutPanel fplvdeitems)
        {
            bool retVal = false;
            
            try
            {
                ENABLE_CAPTURE = false;
                LABEL_ITEM = lin;
                REEL_LPN = reel_lpn;
                if (MedDataItems != null)
                    MedDataItems.Clear();
                MedDataItems = new List<MedData>();

                List<LabelItemDataSetup> LIDs = DataManager.LabelDataItems(lin, reel_lpn);

                foreach (LabelItemDataSetup lid in LIDs)
                {
                    if (lid.DataPresent == false)
                    {
                        string err = string.Format("Label information is missing for label item: {0}. Cannot build VDE data item model. loadVDEToolsAndData()", lin);
                        SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Red);
                        uscMD.SystemMessage(smea);
                        return retVal;
                    }
                    uscVDEItem vdi = new uscVDEItem(lid);
                    vdi.AutoSize = false;
                    vdi.AutoSizeMode = AutoSizeMode.GrowOnly;
                    fplvdeitems.FlowDirection = FlowDirection.LeftToRight;
                    fplvdeitems.SetFlowBreak(vdi, true);
                    fplvdeitems.Controls.Add(vdi);
                    MedData meddata = DataManager.LabelDataInspection(lwo, reel_lpn, vdi.VariableName);
                    meddata.PlaceHolder = vdi.PlaceHolder;
                    meddata.VarName = vdi.VariableName;
                    meddata.Repeat = vdi.Repeat;
                    if (meddata.Repeat > 0)
                        meddata.repeatIndex = 1;
                    else
                        meddata.repeatIndex = 0;
                    MedDataItems.Add(meddata);
                }                

                VariableMedDataPH = "";
                MultiKitsPerPatient = false;
                QntyKitsPerPatient = 0;
                foreach (MedData md in MedDataItems)
                {
                    if (md.Repeat > 0)
                    {
                        VariableMedDataPH = md.PlaceHolder;  // Extract the placeholder for the varying data if it exists
                        if(md.Repeat > 1)
                        {
                            MultiKitsPerPatient = true;
                            QntyKitsPerPatient = md.Repeat;
                            break;
                        }
                    }
                }

                retVal = true;
            }
            catch (Exception ex)
            {
                string err = "loadVDEToolsAndData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Inspection", (int)CriticalLevels.Amber);
                uscMD.SystemMessage(smea);
            }
            return retVal;
        }

        

        public bool LoadMedDataList(string lin, string lpn, string lwo, FlowLayoutPanel flpvdeitem)
        //public bool LoadMedDataList(string lin, string lpn, string lwo, System.Windows.Forms.ListView lstvdeitem)
        {
            bool retVal = false;
            try
            {
                ENABLE_CAPTURE = false;
                LABEL_ITEM = lin;
                REEL_LPN = lpn;
                if (MedDataItems != null)
                    MedDataItems.Clear();
                MedDataItems = new List<MedData>();

                foreach (Control vdecontrol in flpvdeitem.Controls)
                    if (vdecontrol is uscVDEItem)
                    {
                        uscVDEItem uscvde = (uscVDEItem)vdecontrol;
                        MedData meddata = DataManager.LabelDataInspection(lwo, lpn, uscvde.VariableName);
                        meddata.PlaceHolder = uscvde.PlaceHolder;
                        meddata.VarName = uscvde.VariableName;
                        meddata.Repeat = uscvde.Repeat;
                        if (meddata.Repeat > 0)
                            meddata.repeatIndex = 1;
                        else
                            meddata.repeatIndex = 0;
                        MedDataItems.Add(meddata);                        
                    }
                LABEL_COUNT = MedDataItems[0].sequence_list.Count;
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "LoadMedDataList() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Initialize Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
            return retVal;
        }

        public bool LoadInspectionVDEItemParams(string lin)
        {
            bool retVal = false;
            try
            {
                FontsizesSegment = DataManager.GetListFontSizes();
                int labelID = DataManager.LabelID(Defaults.StationID, lin);
                labelType = DataManager.GetLabelType(Defaults.StationID, lin);
                VDEItems = DataManager.InspectionParamsVDEItem(labelID, FontsizesSegment, labelType);
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "LoadInspectionVDEItemParams() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Initialize Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
            return retVal;
        }

        private  bool loadOPZoneData(int id)
        {
            CameraNecta.imageIndexToSave = 0;
            bool retVal = false;
            try
            {
                if (OPZoneItems != null)
                    OPZoneItems.Clear();
                OPZoneItems = DataManager.OPZoneData(id, TRG_FOLDER, labelType);
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "LoadOPZoneData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Initialize Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
            return retVal;
        }

        #region Inspection

        public bool ReduceBackground(ref HObject img, ref FailRecord fp)
        {            
            bool retVal = true;
            HObject tmpObj = null, region = null;
            HTuple edgeThreshold = 30, ampThreshold = 20, startRow = 200, rotate = 270, labelRow = null, labelCol = null;
            HTuple msrCol1 = null;
            HTuple msrRow2 = null;
            HTuple msrAmp1 = null;
            HTuple msrAmp2 = null;
            HTuple msrDist1 = null;
            HTuple msrDist2 = null;
            HTuple msr1 = null;
            HTuple msr2 = null;
            try

            {                
                HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                HOperatorSet.GenMeasureRectangle2(h / 2, w / 2, new HTuple(90).TupleRad(), (h - 20) / 2, 50, w, h, "nearest_neighbor", out msr1);
                HOperatorSet.MeasurePos(img, msr1, 1.0, edgeThreshold, "positive", "first", out labelRow, out msrCol1, out msrAmp1, out msrDist1);
                HOperatorSet.GenMeasureRectangle2(startRow, (w / 2), new HTuple(0).TupleRad(), (w - startRow) / 2, 25, w, h, "nearest_neighbor", out msr2);
                HOperatorSet.MeasurePos(img, msr2, 1.0, ampThreshold, "positive", "first", out msrRow2, out labelCol, out msrAmp2, out msrDist2);
                if (labelCol.Length == 0 && labelRow.Length == 0)
                {
                    retVal = false;
                    fp.READABLE = false;
                    fp.VALID_LABEL = false;
                    RegionFailData rfd = new RegionFailData("Image Preparation");
                    if (img.IsInitialized())
                    {
                        if (rfd.Img == null)
                            HOperatorSet.GenEmptyObj(out rfd.Img);
                        HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                        HOperatorSet.DispObj(img, hWin.HalconWindow);
                    }
                    rfd.Reasons.Add("Label edges not identified / unable to determine the variation region");
                    fp.RegionFailDataList.Add(rfd);
                    fp.ACCEPTED = false;
                    fp.VALID_LABEL = false;
                    fp.ACTIONED = false;

                    EdgesNotDetectedError = true;
                    EdgeNotFoundFailDelay();
                    
                    
                    return false;
                }
                if (region != null)
                    region.Dispose();
                if (labelRow.Length == 0)
                    HOperatorSet.GenRectangle1(out region, 0, labelCol, h, w);
                else
                    HOperatorSet.GenRectangle1(out region, 0, labelCol, h, w);
                if (tmpObj != null)
                    tmpObj.Dispose();
                HOperatorSet.ReduceDomain(img, region, out tmpObj);
                HOperatorSet.CropDomain(tmpObj, out tmpObj);
                HOperatorSet.RotateImage(tmpObj, out tmpObj, rotate, "constant");
                if (CurrentRawImage != null)
                    CurrentRawImage.Dispose();
                HOperatorSet.CopyObj(tmpObj, out CurrentRawImage, 1, 1);
                img.Dispose();
                HOperatorSet.CopyObj(tmpObj, out img, 1, 1);
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                fp.READABLE = false;
                fp.ACCEPTED = false;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "ReduceBackground() err: " + ex.Message + ". Cannot distinguish label and/or web from roller background";
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            finally
            {
                if (tmpObj != null)
                    tmpObj.Dispose();
                if (region != null)
                    region.Dispose();
                if (cameraNecta.CameraImage != null)
                    cameraNecta.CameraImage.Dispose();
            }
            return retVal;
        }

        private async void EdgeNotFoundFailDelay()
        {
            await Task.Delay(500);
            SYSTEM_IO.FAIL_OCCURED();
        }

        //private (HTuple P, HTuple pA, HTuple pB) SetMSERParams(ref HObject img, HTuple mean)
        //{
        //    HTuple paramValuesA, paramValuesB, mserParams;
        //    mserParams = new HTuple();

        //    if (MSERLabel.DarkMinDiversity > -1)
        //    {
        //        //HOperatorSet.Intensity(VariationRegion, img, out HTuple mean, out HTuple deviation);
        //        mserParams[0] = "min_diversity";
        //        mserParams[1] = "max_variation";
        //        mserParams[2] = "min_gray";
        //        mserParams[3] = "max_gray";
        //        paramValuesA = new HTuple();
        //        paramValuesA[0] = MSERLabel.DarkMinDiversity;
        //        paramValuesA[1] = MSERLabel.DarkMaxVariation;
        //        paramValuesA[2] = MSERLabel.DarkMinGray;
        //        paramValuesA[3] = mean - (mean * MSERLabel.MeanMultiplier);

        //        paramValuesB = new HTuple();
        //        paramValuesB[0] = MSERLabel.LightMinDiversity;
        //        paramValuesB[1] = MSERLabel.LightMaxVariation;
        //        paramValuesB[2] = mean - 20;
        //        paramValuesB[3] = 255;
        //    }
        //    else
        //    {
        //        //HOperatorSet.Intensity(VariationRegion, img, out HTuple mean, out HTuple deviation);
        //        mserParams[0] = "max_variation";
        //        mserParams[1] = "min_gray";
        //        mserParams[2] = "max_gray";
        //        paramValuesA = new HTuple();
        //        paramValuesA[0] = MSERLabel.DarkMaxVariation;
        //        paramValuesA[1] = MSERLabel.DarkMinGray;
        //        paramValuesA[2] = mean - (mean * MSERLabel.MeanMultiplier);

        //        paramValuesB = new HTuple();
        //        paramValuesB[0] = MSERLabel.LightMaxVariation;
        //        paramValuesB[1] = mean - 20;
        //        paramValuesB[2] = 255;
        //    }
        //    return (mserParams, paramValuesA, paramValuesB);
        //}

        private void RecordFailData(HObject regiondata, ref HObject labelimage, ref FailRecord fp, bool isdark, string zonename)
        {            
            try
            {
                string result = fp.MED_ID;
                fp.VALID_LABEL = false;
                RegionFailData rfd = new RegionFailData(zonename);
                if (rfd.Img == null)
                    HOperatorSet.GenEmptyObj(out rfd.Img);
                HOperatorSet.CopyObj(labelimage, out rfd.Img, 1, 1);
                if (isdark)
                    rfd.Reasons.Add("dark debris/marks on label");
                else
                    rfd.Reasons.Add("bright areas/marks on label");
                fp.RegionFailDataList.Add(rfd);
                HOperatorSet.SmallestRectangle1(regiondata, out HTuple r1sel, out HTuple c1sel, out HTuple r2sel, out HTuple c2sel);
                if (r1sel.Length == 1)
                    rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1sel - 2, c1sel - 2, r2sel + 2, c2sel + 2 }));
                else
                {
                    if (r1sel.Length < 51)
                    {
                        for (int x = 0; x < r1sel.Length; x++)
                            rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { Convert.ToInt32(r1sel[x].D), Convert.ToInt32(c1sel[x].D), Convert.ToInt32(r2sel[x].D), Convert.ToInt32(c2sel[x].D) }));
                    }
                }
                fp.VALID_LABEL = false;
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                string sMed = "";
                foreach (MedData med in MedDataItems)
                {
                    if (med.PlaceHolder == VariableMedDataPH)
                    {
                        sMed = "label " + med.Data + ". ";
                        break;
                    }
                }
                string err = "RecordFailData() err: " + sMed + "Region: " + zonename + ": " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
        }

        private bool DebrisCheck(ref HObject maskedimage, ref HObject labelimage, bool inspectlightareas, ref FailRecord fp, string opzonename)
        {            
            bool retVal = true;
            HObject imgSobel = null, rgnThreshold = null, rgnSelectedDark = null, rgnSelectedLight = null, rgnUnion = null, rgnDilation = null;
            HObject rgnFillup = null, imgReduced = null, rgnDarkObjects = null, rgnLightObjects = null, rgnConnected = null, rgnSelected = null, rgnEroded = null; 
            HTuple mean = 0, numDarkObjects = 0, numLightObjects = 0;
            try
            {                
                HOperatorSet.Intensity(VariationRegion, maskedimage, out mean, out HTuple deviation);

                //Find Marks
                HOperatorSet.SobelAmp(maskedimage, out imgSobel, "sum_abs", iParams.SobelAmpSize);
                HOperatorSet.Threshold(imgSobel, out rgnThreshold, iParams.SobelEdgeThreshold, 255);
                HOperatorSet.Connection(rgnThreshold, out rgnConnected);

                //Elininate Small clutter pixels groups
                //HOperatorSet.SelectShape(rgnConnected, out rgnSelected, "area", "and", 10, 999999);
                HOperatorSet.Union1(rgnConnected, out rgnUnion);

                //Merge fragments
                HOperatorSet.DilationCircle(rgnUnion, out rgnDilation, 3.5);
                HOperatorSet.FillUp(rgnDilation, out rgnFillup);
                HOperatorSet.ErosionCircle(rgnFillup, out rgnEroded, 2.5);
                HOperatorSet.ReduceDomain(maskedimage, rgnEroded, out imgReduced);
                HOperatorSet.Threshold(imgReduced, out rgnDarkObjects, 0, mean * iParams.MeanOffset);

                //select bright and DO NOT merge with dark !
                if (inspectlightareas == true)
                {
                    if (imgReduced != null) imgReduced.Dispose();
                    if (rgnEroded != null) rgnEroded.Dispose();
                    HOperatorSet.ErosionCircle(rgnFillup, out rgnEroded, 3.5);
                    HOperatorSet.ReduceDomain(maskedimage, rgnEroded, out imgReduced);
                    HOperatorSet.Threshold(imgReduced, out rgnLightObjects, mean * 0.9, 255);
                    if (rgnConnected != null)
                        rgnConnected.Dispose();
                    if (rgnSelected != null)
                        rgnSelected.Dispose();
                    HOperatorSet.Connection(rgnLightObjects, out rgnConnected);
                    HOperatorSet.SelectShape(rgnConnected, out rgnSelectedLight, "area", "and", iParams.DebrisMinSize, iParams.DebrisMaxSize);
                    HOperatorSet.CountObj(rgnSelectedLight, out numLightObjects);
                }

                if (rgnConnected != null)
                    rgnConnected.Dispose();
                if (rgnSelected != null)
                    rgnSelected.Dispose();
                HOperatorSet.Connection(rgnDarkObjects, out rgnConnected);
                HOperatorSet.SelectShape(rgnConnected, out rgnConnected, "inner_radius", "and", 2.5, iParams.DebrisMaxSize);
                HOperatorSet.SelectShape(rgnConnected, out rgnSelectedDark, "area", "and", iParams.DebrisMinSize, iParams.DebrisMaxSize);
                HOperatorSet.CountObj(rgnSelectedDark, out numDarkObjects);

                if (numDarkObjects > 0)
                {
                    retVal = false;
                    RecordFailData(rgnSelectedDark, ref labelimage, ref fp, true, opzonename);
                }
                if (numLightObjects > 0)
                {
                    retVal = false;
                    RecordFailData(rgnSelectedLight, ref labelimage, ref fp, false, opzonename);
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                string sMed = "";
                foreach (MedData med in MedDataItems)
                {
                    if (med.PlaceHolder == VariableMedDataPH)
                    {
                        sMed = "label " + med.Data + ". ";
                        break;
                    }
                }
                string err = "DebrisCheck() err: " + sMed + "Region: " + opzonename + ": " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            finally
            {
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


        private void eliminateLineStretch(ref HObject labelimage, double threshold)
        {            
            HTuple darkObjectOnLine = 0;
            HTuple selectedAreaCount = 0;
            HObject selectedArea = null, rgnAreas = null, rgnComplement = null, tmpLine = null;
            int maxGray = Convert.ToInt32(threshold);
            try
            {
                if (labelimage != null)
                {
                    if (labelimage.IsInitialized())
                    {
                        HOperatorSet.Threshold(labelimage, out rgnAreas, 0, maxGray);
                        HOperatorSet.Connection(rgnAreas, out rgnAreas);
                        HOperatorSet.SelectShape(rgnAreas, out rgnAreas, "area", "and", 800, 50000);
                        HOperatorSet.Connection(rgnAreas, out rgnAreas);
                        HOperatorSet.AreaCenter(rgnAreas, out HTuple a, out HTuple YCenter, out HTuple XCenter);
                        HOperatorSet.CountObj(rgnAreas, out selectedAreaCount);
                        for (int y = 1; y <= selectedAreaCount; y++)
                        {
                            if (selectedArea != null) selectedArea.Dispose();
                            HOperatorSet.GenEmptyObj(out selectedArea);
                            HOperatorSet.SelectObj(rgnAreas, out selectedArea, y);
                            HOperatorSet.HeightWidthRatio(selectedArea, out HTuple h, out HTuple w, out HTuple ratio);
                            if (ratio.Length > 0)
                            {
                                if (ratio.D <= 0.05) 
                                {
                                    HOperatorSet.SmallestRectangle1(selectedArea, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                                    HOperatorSet.GenRectangle1(out tmpLine, r1 - 4, c1 - 4, r2 + 4, c2 + 4);
                                    HOperatorSet.Complement(tmpLine, out rgnComplement);
                                    HOperatorSet.ReduceDomain(labelimage, rgnComplement, out labelimage);
                                    if (tmpLine != null) tmpLine.Dispose();
                                    if (rgnComplement != null) rgnComplement.Dispose();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return; 
                SYSTEM_IO.PROCESSING = false;
                string err = "eliminateLineStretch() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            finally
            {
                if (selectedArea != null) selectedArea.Dispose();
                if (rgnAreas != null) rgnAreas.Dispose();
                if (rgnComplement != null) rgnComplement.Dispose();
                if (tmpLine != null) tmpLine.Dispose();
            }
        }

        public bool InspectOpZone(ref HObject img, ref FailRecord fp)
        {            
            bool bVDEFoundFalse = false;
            bool retVal = true;
            HTuple OPZoneCol = 0, OPZoneRow = 0, scaleR = null, scaleC = null, HomMat2D = null;
            HTuple row = null, column = null, angle = null, OPZoneScore = null;
            HObject rgnThreshold = null, rgnConnectedThreshold = null, rgnMSER = null, rgnUnion = null;
            HObject imgTransform = null, opzROI = null, tmp = null, rgnConnectedArea = null; 
            HObject rgnDilation = null, rgnFillup = null, rgnComplement = null, imgComplement = null, labelROI = null;
            string ozdName = "";
            try
            {
                if (VariationRegion != null)
                    VariationRegion.Dispose();
                HOperatorSet.GenRectangle1(out VariationRegion, VariationRegionCoords[0] - 4, VariationRegionCoords[1] - 4, VariationRegionCoords[2] + 4, VariationRegionCoords[3] + 4);
                HOperatorSet.ReduceDomain(img, VariationRegion, out img);
                bool bVDEFound = true;
                foreach (OPZoneData ozd in OPZoneItems.OrderBy(x => x.NAME))
                {                   
                    if (SYSTEM_IO.PROCESSING == false)
                        return true;
                    ozdName = ozd.NAME;
                    if (imgTransform != null) imgTransform.Dispose();
                    if (imgComplement != null) imgComplement.Dispose();
                    if (rgnThreshold != null) rgnThreshold.Dispose();
                    if (rgnConnectedThreshold != null) rgnConnectedThreshold.Dispose();
                    if (rgnMSER != null) rgnMSER.Dispose();
                    if (rgnUnion != null) rgnUnion.Dispose();
                    if (opzROI != null) opzROI.Dispose();
                    if (tmp != null) tmp.Dispose();
                    if (rgnConnectedArea != null) rgnConnectedArea.Dispose();
                    if (rgnDilation != null) rgnDilation.Dispose();
                    if (rgnFillup != null) rgnFillup.Dispose();
                    if (rgnComplement != null) rgnComplement.Dispose();
                    if (labelROI != null) labelROI.Dispose();
                    OPZoneCol = 0;
                    OPZoneRow = 0;
                    double score = 0;

                    HOperatorSet.FindAnisoShapeModel(img, ozd.FIXTURE_ID, Defaults.radMinus1Point5, Defaults.rad2, 1, 1, 0.98, 1.02, 0.5, 1, 0.5, "least_squares", Defaults.NumLevelsFind, Defaults.Greediness, out row, out column, out angle, out scaleR, out scaleC, out OPZoneScore);
                    if (OPZoneScore.Length > 0)
                        score = (double)OPZoneScore.D;
                    else
                    {
                        HOperatorSet.FindAnisoShapeModel(img, ozd.FIXTURE_ID, Defaults.radMinus1Point5, Defaults.rad2, 0.9, 1, 0.9, 1.0, 0.0, 1, 0.5, "least_squares", 0, 0.9, out row, out column, out angle, out scaleR, out scaleC, out OPZoneScore);
                        if (OPZoneScore.Length > 0)
                            score = (double)OPZoneScore.D;
                    }
                    if (score >= Defaults.FixtureOPZoneScoreMin)
                    {
                        OPZoneRow = ozd.FIXTURE_Y - row;
                        OPZoneCol = ozd.FIXTURE_X - column;
                        HOperatorSet.HomMat2dIdentity(out HomMat2D);
                        HOperatorSet.HomMat2dScale(HomMat2D, 1 / scaleR, 1 / scaleC, row, column, out HomMat2D);
                        HOperatorSet.HomMat2dRotate(HomMat2D, -angle, row, column, out HomMat2D);
                        HOperatorSet.HomMat2dTranslate(HomMat2D, OPZoneRow, OPZoneCol, out HomMat2D);
                        HOperatorSet.AffineTransImage(img, out imgTransform, HomMat2D, "weighted", "false");
                        HOperatorSet.ReduceDomain(img, VariationRegion, out img);
                        ozd.HomMat2D = HomMat2D;
                        HOperatorSet.GenRectangle1(out opzROI, ozd.OPZONE_TOP, ozd.OPZONE_LEFT, ozd.OPZONE_BOTTOM, ozd.OPZONE_RIGHT);
                        HOperatorSet.CopyObj(opzROI, out ozd.region, 1, 1);
                        bVDEFound = InspectVDE(ref imgTransform, ref fp, ozd);

                        if (SYSTEM_IO.PROCESSING == false)
                            return true;
                        maskVDEByOpZone(ref imgTransform, opzROI, ozd.NAME);
                        InspectAndMaskMasks(ref imgTransform, ref fp);
                        compareZoneImageToModel(ozd, imgTransform, ref fp, imgTransform);
                        HOperatorSet.GenRectangle2(out ozd.AffineRegion, row, column, angle, ((ozd.OPZONE_RIGHT - ozd.OPZONE_LEFT) / 2), (ozd.OPZONE_BOTTOM - ozd.OPZONE_TOP) / 2);

                        if (bVDEFoundFalse == false)
                        {
                            if (bVDEFound == false)
                            {
                                bVDEFoundFalse = true;
                            }
                        }                       
                    }
                    else
                    {
                        #region fail
                        retVal = false;
                        string result = fp.MED_ID;
                        fp.VALID_LABEL = false;
                        RegionFailData rfd = new RegionFailData(ozd.NAME);
                        if (rfd.Img == null)
                            HOperatorSet.GenEmptyObj(out rfd.Img);
                        HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                        rfd.Reasons.Add(string.Format("Label " + result + ". Fixture score {0} is too low in Op-Zone {1}", score, ozd.NAME));
                        rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { ozd.OPZONE_TOP, ozd.OPZONE_LEFT, ozd.OPZONE_BOTTOM, ozd.OPZONE_RIGHT }));
                        fp.RegionFailDataList.Add(rfd);
                        #endregion
                    }                    
                    if (SYSTEM_IO.PROCESSING == false)
                        return true;
                }

                if (bVDEFoundFalse == true)
                {
                    //FailRecord fpOut = null;
                    PositionCheck posCheck = PositionCheck.IS_GOOD;
                    posCheck = StatusCheck(ref fp);
                    FailRecord fpOut = null;
                    
                    if(posCheck != PositionCheck.IS_NEXT)
                    {
                        bVDEFound = setFailPipeData(ref fp, posCheck, out fpOut, ozdName);
                        if (bVDEFound)
                        {
                            fp = fpOut;
                        }
                    }
                    else
                    {
                        bVDEFound = setFailPipeData(ref fp, posCheck, out fpOut, ozdName);

                        if (bVDEFound)
                        {
                            fp = fpOut;
                        }

                        foreach (MedData md in MedDataItems)
                        {
                            fp.Datas[fp.PlaceHolders.IndexOf(md.PlaceHolder)] = md.Data;
                            if(md.PlaceHolder == VariableMedDataPH)
                            {
                                fp.MED_ID = md.Data;
                            }
                        }
                        posCheck = PositionCheck.IS_GOOD;
                        posCheck = StatusCheck(ref fp);
                        fpOut = null;
                        bVDEFound = setFailPipeData(ref fp, posCheck, out fpOut, ozdName);
                        if (bVDEFound)
                        {
                            fp = fpOut;
                        }
                    }
                }

                if(MultiKitsPerPatient == false)
                {
                    medFailOnPreviousLabel = medFailOnCurrentLabel;
                    medFailOnCurrentLabel = false;
                }
                else
                {
                    foreach (MedData md in MedDataItems)
                    {
                        if(md.PlaceHolder == VariableMedDataPH)
                        {
                            if (md.repeatIndex >= md.Repeat)
                            {
                                medFailInPreviousPatient = medFailInCurrentPatient;
                                medFailInCurrentPatient = false;                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pauseMethodCaller?.Invoke();
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                string sMed = "";
                foreach (MedData med in MedDataItems)
                {
                    if (med.PlaceHolder == VariableMedDataPH)
                    {
                        sMed = "label " + med.Data + ". ";
                        break;
                    }
                }
                string err = "InspectOpZone() err: " + sMed + "Region: " + ozdName + ": " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            finally
            {
                if (imgComplement != null) imgComplement.Dispose();
                if (rgnThreshold != null) rgnThreshold.Dispose();
                if (rgnConnectedThreshold != null) rgnConnectedThreshold.Dispose();
                if (rgnMSER != null) rgnMSER.Dispose();
                if (rgnUnion != null) rgnUnion.Dispose();
                if (imgTransform != null) imgTransform.Dispose();
                if (opzROI != null) opzROI.Dispose();
                if (tmp != null) tmp.Dispose();
                if (rgnConnectedArea != null) rgnConnectedArea.Dispose();
                if (rgnDilation != null) rgnDilation.Dispose();
                if (rgnFillup != null) rgnFillup.Dispose();
                if (rgnComplement != null) rgnComplement.Dispose();
                if (labelROI != null) labelROI.Dispose();                
            }

            return retVal;
        }

        private bool compareZoneImageToModel(OPZoneData ozd, HObject tmpobj, ref FailRecord fp, HObject rawimage)
        {
            bool retVal = true;
            HObject regions = null, rgnFail = null, failAreas = null, selectedRegions = null;
            HObject region = null, connectedRegions = null, rgnDifference = null;
            HTuple numDiffsLight = 0, numDiffs = 0;
            HTuple features = new HTuple("inner_radius");
            HTuple radiusSmall = InnerRadius;
            HTuple radiusLarge = new HTuple(500.1);
            bool varRegions = false;
            try
            {

                HOperatorSet.SmallestRectangle1(ozd.region, out HTuple r1reg, out HTuple c1reg, out HTuple r2reg, out HTuple c2reg);
                HOperatorSet.ReduceDomain(tmpobj, ozd.region, out tmpobj);
                HOperatorSet.CropDomain(tmpobj, out tmpobj);

                eliminateLineStretch(ref tmpobj, ozd.DarkMaxGray);
                HOperatorSet.CompareVariationModel(tmpobj, out regions, ozd.VARIATION_VAM);

                if (regions.IsInitialized())
                    if (regions.CountObj() > 0)
                        varRegions = true;
                if (varRegions)
                {
                    HOperatorSet.Connection(regions, out regions);
                    HOperatorSet.SelectShape(regions, out selectedRegions, features, "and", radiusSmall, radiusLarge);
                    HOperatorSet.SelectShape(selectedRegions, out connectedRegions, "area", "and", ozd.DarkMinSizeVAR*2, 999999);
                    HOperatorSet.Connection(connectedRegions, out connectedRegions);
                    HOperatorSet.CountObj(connectedRegions, out numDiffs);

                    HOperatorSet.AreaCenter(connectedRegions, out HTuple a, out HTuple c, out HTuple r);
                }

                if (numDiffs.I > 0)
                {
                    fp.VALID_LABEL = false;
                    fp.ACTIONED = false;
                    string result = fp.MED_ID;
                    RegionFailData rfd = new RegionFailData(ozd.NAME);
                    if (rfd.Img == null)
                        HOperatorSet.GenEmptyObj(out rfd.Img);
                    HOperatorSet.CopyObj(tmpobj, out rfd.Img, 1, 1);
                    rfd.Reasons.Add("Print Variation in " + ozd.NAME);
                    fp.RegionFailDataList.Add(rfd);
                    HOperatorSet.SmallestRectangle1(connectedRegions, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                    if (r1.Length == 1)
                        rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1.I, c1.I, r2.I, c2.I }));
                    else
                    {
                        for (int x = 0; x < r1.Length; x++)
                            rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1[x].I, c1[x].I, r2[x].I, c2[x].I }));
                    }
                }

            }
            catch(HalconException hex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                fp.VALID_LABEL = false;
                fp.ACTIONED = false;
                string result = fp.MED_ID;
                RegionFailData rfd = new RegionFailData(ozd.NAME);
                if (rfd.Img == null)
                    HOperatorSet.GenEmptyObj(out rfd.Img);
                HOperatorSet.CopyObj(tmpobj, out rfd.Img, 1, 1);
                rfd.Reasons.Add("compareZoneImageToModel() Halcon error in " + ozd.NAME);
                rfd.Reasons.Add("ERROR: " + hex.Message);
                fp.RegionFailDataList.Add(rfd);
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "compareZoneImageToModel() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            finally
            {
                if (rawimage != null) rawimage.Dispose();
                if (tmpobj != null) tmpobj.Dispose();
                if (rgnDifference != null) rgnDifference.Dispose();
                if (rgnFail != null) rgnFail.Dispose();
                if (failAreas != null) failAreas.Dispose();
                if (regions != null) regions.Dispose();
                if (region != null) region.Dispose();
                if (selectedRegions != null) selectedRegions.Dispose();
                if (connectedRegions != null) connectedRegions.Dispose();
            }
            return retVal;
        }

        public bool InspectLabel(HObject img, ref FailRecord fp)
        {
            bool retVal = true;
            HObject imgTmp = null, imgThreshold = null, connectedThreshold = null, selectedAreas = null, zoneRegion = null, regUnion = null;
            HObject regDilation = null, regFillup = null, regComplement = null, imgZone = null;
            HTuple countObjects = 0;
            try
            {
                InspectAndMaskMasks(ref img, ref fp);
                InspectBarcodes2D(ref img, ref fp);
                if (fp.BARCODE_UNREADABLE_2D == false)
                    MaskBarcodes2D(ref img, ref fp);
                InspectBarcodesLinear(ref img, ref fp);
                if (fp.BARCODE_UNREADABLE_LINEAR == false)
                    MaskBarcodesLinear(ref img, ref fp);
                HOperatorSet.CopyObj(img, out imgTmp, 1, 1);
                foreach (OPZoneData ozd in OPZoneItems.OrderBy(x => x.NAME))
                {
                    if (SYSTEM_IO.PROCESSING == false)
                        return true;
                    if (imgZone != null) imgZone.Dispose();
                    if (imgThreshold != null) imgThreshold.Dispose();
                    if (connectedThreshold != null) connectedThreshold.Dispose();
                    if (selectedAreas != null) selectedAreas.Dispose();
                    if (zoneRegion != null) zoneRegion.Dispose();
                    if (zoneRegion != null) zoneRegion.Dispose();
                    if (regUnion != null) regUnion.Dispose();
                    if (regDilation != null) regDilation.Dispose();
                    if (regFillup != null) regFillup.Dispose();
                    if (regComplement != null) regComplement.Dispose();
                    if (VariationImg != null) VariationImg.Dispose();
                    if (ozd.AffineRegion != null)
                    {
                        HOperatorSet.SmallestRectangle1(ozd.AffineRegion, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                        HOperatorSet.GenRectangle1(out zoneRegion, r1 - 4, c1 - 20, r2 + 4, c2 + 20);
                        HOperatorSet.ReduceDomain(imgTmp, zoneRegion, out imgZone);
                        HOperatorSet.Complement(imgZone, out regComplement);
                        HOperatorSet.ReduceDomain(imgTmp, regComplement, out imgTmp);
                    }
                    //if (ozd.AffineRegion != null)
                    //{
                    //    HOperatorSet.ReduceDomain(imgTmp, ozd.region, out imgZone);
                    //    HOperatorSet.Threshold(imgZone, out imgThreshold, 0, ozd.DarkMaxGray);
                    //    HOperatorSet.Connection(imgThreshold, out connectedThreshold);
                    //    HOperatorSet.SelectShape(connectedThreshold, out selectedAreas, "area", "and", 20, 999999);
                    //    if (selectedAreas.IsInitialized())
                    //        try { countObjects = selectedAreas.CountObj(); } catch { countObjects = 0; }
                    //    if (countObjects > 0)
                    //    {
                    //        HOperatorSet.SmallestRectangle1(selectedAreas, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                    //        HOperatorSet.GenRectangle1(out zoneRegion, r1, c1, r2, c2);
                    //        HOperatorSet.Union1(zoneRegion, out regUnion);
                    //        HOperatorSet.DilationRectangle1(regUnion, out regDilation, Defaults.DilationWidth, Defaults.DilationHeight);
                    //        HOperatorSet.FillUp(regDilation, out regFillup);
                    //        HOperatorSet.Complement(regFillup, out regComplement);
                    //        HOperatorSet.ReduceDomain(imgTmp, regComplement, out imgTmp);
                    //        eliminateLineStretch(ref imgTmp, ozd.DarkMaxGray);
                    //    }
                    //}
                }
                if (countObjects > 0)
                {
                    MaskBarcodes2D(ref imgTmp, ref fp);
                    MaskBarcodesLinear(ref imgTmp, ref fp);

                    if (DebrisCheck(ref imgTmp, ref img, INSPECT_LIGHT_AREAS, ref fp, "label") == false)
                        retVal = false;
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "InspectLabel() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            finally
            {
                if (img != null) img.Dispose();
                if (imgZone != null) imgZone.Dispose();
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
                if (VariationImg != null) VariationImg.Dispose();
            }
            return retVal;
        }



        public bool InspectAndMaskMasks(ref HObject img, ref FailRecord fp)
        {
            bool retVal = true;
            HObject region = null;
            int ROW1 = 0, COL1 = 0, ROW2 = 0, COL2 = 0;

            try
            {                
                HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);        
                foreach (VDEItem vdi in VDEItems)
                {
                    if (vdi.IsMask)
                    {
                        fp.vDEType = VDEType.MASK;
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
                string err = "InspectAndMaskMasks() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        public bool InspectBarcodes2D(ref HObject img, ref FailRecord fp)
        {
            bool retVal = true;
            HObject tmpObj = null, barcodeBin = null, SymbolXLDs = null, region = null, rgnBC = null, points = null, RegionComplement = null;
            HTuple DataCodeHandle = null;
            
            try
            {
                foreach (VDEItem vdi in VDEItems.Where(x => x.IsBarcode2D))
                {
                    if (tmpObj != null)  tmpObj.Dispose();
                    if (points != null) points.Dispose();
                    if (RegionComplement != null) RegionComplement.Dispose();
                    if (barcodeBin != null) barcodeBin.Dispose();
                    if (DataCodeHandle != null)
                        try { HOperatorSet.ClearBarCodeModel(DataCodeHandle); } catch { }
                    if (SymbolXLDs != null) SymbolXLDs.Dispose();
                    //SymbolXLDs = null;
                    if (region != null) region.Dispose();
                    //region = null;
                    DataCodeHandle = null;
                    fp.vDEType = VDEType.BARCODE_2D;
                    string barcodeType = vdi.BarcodeName;
                    HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                    int t = vdi.BarcodeRegion[0] - 160;
                    int l = vdi.BarcodeRegion[1] - 160;
                    int b = vdi.BarcodeRegion[2] + 160;
                    int r = vdi.BarcodeRegion[3] + 160;
                    if (t < 0) t = 0;
                    if (l < 0) l = 0;
                    if (b > h) b = h;
                    if (r > w) r = w;
                    HOperatorSet.GenRectangle1(out region, t, l, b, r);
                    HOperatorSet.ReduceDomain(img, region, out tmpObj);
                    HOperatorSet.GaussFilter(tmpObj, out tmpObj, 3);
                    HOperatorSet.CreateDataCode2dModel(barcodeType, "default_parameters", "standard_recognition", out DataCodeHandle);
                    //HOperatorSet.FindDataCode2d(tmpObj, out SymbolXLDs, DataCodeHandle, "stop_after_result_num", new HTuple(2), out HTuple ResultHandles, out HTuple DecodedDataStrings);
                    HOperatorSet.FindDataCode2d(tmpObj, out SymbolXLDs, DataCodeHandle, new HTuple(), new HTuple(), out HTuple ResultHandles, out HTuple DecodedDataStrings);
                    HOperatorSet.ClearDataCode2dModel(DataCodeHandle);
                    HTuple symbolCount = 0;
                    try { HOperatorSet.CountObj(SymbolXLDs, out symbolCount); } catch { symbolCount = 0; }
                    if (symbolCount > 0)
                    {
                        List<string> datas = new List<string>();
                        List<string> placeholders = new List<string>();
                        string med = "";
                        string nonVDEData = "";
                        if (DecodedDataStrings.S.Length > 0)
                        {
                            for (int x = 0; x < DecodedDataStrings.Length; x++)
                            {
                                if (DecodedDataStrings[x].S.Contains(fp.MED_ID))
                                    med = fp.MED_ID;
                                HOperatorSet.SelectObj(SymbolXLDs, out points, x + 1);
                                HOperatorSet.GetContourXld(points, out HTuple RowPoint, out HTuple ColPoint);
                                if (RowPoint.Length >= 4)
                                {
                                    int ROW1 = Convert.ToInt32(RowPoint.TupleMin().D - Defaults.PADDING);
                                    int COL1 = Convert.ToInt32(ColPoint.TupleMin().D - Defaults.PADDING);
                                    int ROW2 = Convert.ToInt32(RowPoint.TupleMax().D + Defaults.PADDING);
                                    int COL2 = Convert.ToInt32(ColPoint.TupleMax().D + Defaults.PADDING);
                                    int[] vdecoord = new int[] { ROW1, COL1, ROW2, COL2 };
                                    vdi.BarcodeRegion[0] = ROW1;
                                    vdi.BarcodeRegion[1] = COL1;
                                    vdi.BarcodeRegion[2] = ROW2;
                                    vdi.BarcodeRegion[3] = COL2;
                                    HOperatorSet.GenRectangle1(out rgnBC, ROW1, COL1, ROW2, COL2);
                                }
                                else
                                {
                                    if (x == 0)
                                    {
                                        //if (errorRaisedToPLC == false)
                                        //{
                                        //    SYSTEM_IO.FAIL_OCCURED();
                                        //    errorRaisedToPLC = true;
                                        //}
                                        int ROW1 = vdi.BarcodeRegion[0] - (Defaults.PADDING_BARCODE / 2);
                                        int COL1 = vdi.BarcodeRegion[1] - (Defaults.PADDING_BARCODE / 2);
                                        int ROW2 = vdi.BarcodeRegion[2] + (Defaults.PADDING_BARCODE / 2);
                                        int COL2 = vdi.BarcodeRegion[3] + (Defaults.PADDING_BARCODE / 2);
                                        HOperatorSet.GenRectangle1(out rgnBC, ROW1, COL1, ROW2, COL2);

                                        RegionFailData rfd = new RegionFailData(vdi.BarcodeName);
                                        if (rfd.Img == null)
                                            HOperatorSet.GenEmptyObj(out rfd.Img);
                                        HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                                        fp.RegionFailDataList.Add(rfd);
                                        rfd.Reasons.Add(string.Format("{0} barcode {1}", barcodeType, "missing and/or unreadable"));
                                        if (rgnBC != null)
                                        {
                                            HOperatorSet.SmallestRectangle1(rgnBC, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                                            rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1, c1, r2, c2 }));
                                        }
                                        else if (region != null)
                                        {
                                            rfd.Reasons.Add("A readable barcode was not found");
                                            HOperatorSet.SmallestRectangle1(region, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                                            rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1, c1, r2, c2 }));
                                        }
                                        else
                                            rfd.Reasons.Add("A readable barcode was not found");
                                        fp.ACCEPTED = false;
                                        fp.DATA_FOUND = false;
                                        fp.BARCODE_UNREADABLE_2D = true;
                                        fp.VALID_LABEL = false;
                                    }
                                }
                                nonVDEData = DecodedDataStrings[x].S;

                                foreach (MedData md in MedDataItems)
                                {
                                    if (nonVDEData == md.Data)
                                    {
                                        vdi.BarcodeData.Add(md.Data);
                                        vdi.Placeholder = md.PlaceHolder;
                                        break;
                                    }
                                }
                                
                                foreach (MedData md in MedDataItems)
                                {
                                    if (nonVDEData.Contains(md.Data))
                                    {
                                        if (datas == null)
                                            datas = new List<string>();
                                        datas.Add(md.Data);
                                        placeholders.Add(md.PlaceHolder);
                                        nonVDEData = nonVDEData.Replace(md.Data, "");
                                        if(nonVDEData!="")
                                            fp.BarcodeNonVDEData = nonVDEData;
                                    }
                                }
                            }
                            
                            if (fp.BarcodeNonVDEData != vdi.BarcodeNonVDEData && vdi.BarcodeData.Count==0)
                            {
                                //if (errorRaisedToPLC == false)
                                //{
                                //    SYSTEM_IO.FAIL_OCCURED();
                                //    errorRaisedToPLC = true;
                                //}
                                retVal = false;
                                fp.VALID_LABEL = false;
                                RegionFailData rfd = new RegionFailData(vdi.BarcodeName);
                                if (rfd.Img == null)
                                    HOperatorSet.GenEmptyObj(out rfd.Img);
                                HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                                fp.RegionFailDataList.Add(rfd);
                                if (datas.Count > 0)
                                    rfd.Reasons.Add(string.Format("{0}: Data error. Found: {1}, searching for: {2}", barcodeType, string.Join(",", datas), string.Join(",", vdi.BarcodeData)));
                                if (nonVDEData != vdi.BarcodeNonVDEData)
                                    rfd.Reasons.Add(string.Format("{0}: Barcode (non VDE) error. Found: {1}, searching for: {2}", barcodeType, nonVDEData, vdi.BarcodeNonVDEData));
                                if (rgnBC != null)
                                {
                                    HOperatorSet.SmallestRectangle1(rgnBC, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                                    rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1, c1, r2, c2 }));
                                }
                                else if (region != null)
                                {
                                    rfd.Reasons.Add("No barcode found within extended search area");
                                    HOperatorSet.SmallestRectangle1(region, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                                    rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1, c1, r2, c2 }));
                                }
                                fp.ACCEPTED = false;
                                fp.BARCODE_UNREADABLE_2D = true;
                                fp.VALID_LABEL = false;
                                HOperatorSet.ClearBarCodeModel(DataCodeHandle);
                            }
                        }
                        else
                        {
                            //if (errorRaisedToPLC == false)
                            //{
                            //    SYSTEM_IO.FAIL_OCCURED();
                            //    errorRaisedToPLC = true;
                            //}
                            if (rgnBC == null)
                            {
                                int ROW1 = vdi.BarcodeRegion[0] - Defaults.PADDING_BARCODE / 2;
                                int COL1 = vdi.BarcodeRegion[1] - Defaults.PADDING_BARCODE / 2; 
                                int ROW2 = vdi.BarcodeRegion[2] + Defaults.PADDING_BARCODE / 2;
                                int COL2 = vdi.BarcodeRegion[3] + Defaults.PADDING_BARCODE / 2;
                                HOperatorSet.GenRectangle1(out rgnBC, ROW1, COL1, ROW2, COL2);
                            }

                            RegionFailData rfd = new RegionFailData(vdi.BarcodeName);
                            if (rfd.Img == null)
                                HOperatorSet.GenEmptyObj(out rfd.Img);
                            HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                            fp.RegionFailDataList.Add(rfd);
                            rfd.Reasons.Add(string.Format("{0} barcode {1}", barcodeType, "missing and/or unreadable"));
                            if (rgnBC != null)
                            {
                                HOperatorSet.SmallestRectangle1(rgnBC, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                                rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1, c1, r2, c2 }));
                            }
                            else if (region != null)
                            {
                                rfd.Reasons.Add("A readable 2D barcode was not found");
                                HOperatorSet.SmallestRectangle1(region, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                                rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1, c1, r2, c2 }));
                            }
                            fp.ACCEPTED = false;
                            fp.DATA_FOUND = false;
                            fp.BARCODE_UNREADABLE_2D = true;
                            fp.VALID_LABEL = false;
                        }
                        HOperatorSet.ClearBarCodeModel(DataCodeHandle);
                    }
                    else
                    {
                        if (vdi.BarcodeRegion[2] > 0) // only mask if there should be a barcode here (use lowest coordinate as it must be > 0 if a barcode should exist)
                        {
                            //if (errorRaisedToPLC == false)
                            //{
                            //    SYSTEM_IO.FAIL_OCCURED();
                            //    errorRaisedToPLC = true;
                            //}
                            int ROW1 = vdi.BarcodeRegion[0] - (Defaults.PADDING_BARCODE / 2);
                            int COL1 = vdi.BarcodeRegion[1] - (Defaults.PADDING_BARCODE / 2);
                            int ROW2 = vdi.BarcodeRegion[2] + (Defaults.PADDING_BARCODE / 2);
                            int COL2 = vdi.BarcodeRegion[3] + (Defaults.PADDING_BARCODE / 2);
                            HOperatorSet.GenRectangle1(out rgnBC, ROW1, COL1, ROW2, COL2);

                            RegionFailData rfd = new RegionFailData(vdi.BarcodeName);
                            if (rfd.Img == null)
                                HOperatorSet.GenEmptyObj(out rfd.Img);
                            HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                            fp.RegionFailDataList.Add(rfd);
                            rfd.Reasons.Add(string.Format("{0} 2D barcode {1}", barcodeType, "missing and/or unreadable"));
                            if (rgnBC != null)
                            {
                                HOperatorSet.SmallestRectangle1(rgnBC, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                                rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1, c1, r2, c2 }));
                            }
                            else if (region != null)
                            {
                                //if (errorRaisedToPLC == false)
                                //{
                                //    SYSTEM_IO.FAIL_OCCURED();
                                //    errorRaisedToPLC = true;
                                //}
                                rfd.Reasons.Add("A readable barcode could not be found");
                                HOperatorSet.SmallestRectangle1(region, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                                rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1, c1, r2, c2 }));
                            }
                            fp.ACCEPTED = false;
                            fp.DATA_FOUND = false;
                            fp.BARCODE_UNREADABLE_2D = true;
                            fp.VALID_LABEL = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "InspectAndMaskBarcodes2D() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            finally
            {
                if (tmpObj != null) tmpObj.Dispose();
                if (barcodeBin != null) barcodeBin.Dispose();
                if (DataCodeHandle != null)
                    try { HOperatorSet.ClearBarCodeModel(DataCodeHandle); } catch { }
                if (SymbolXLDs != null) SymbolXLDs.Dispose();
                if (region != null) region.Dispose();
                if (rgnBC != null) rgnBC.Dispose();
                if (points != null) points.Dispose();
                if (RegionComplement != null) RegionComplement.Dispose();
            }
            return retVal;
        }

        public bool MaskBarcodes2D(ref HObject img, ref FailRecord fp)
        {
            bool retVal = true;
            HObject rgnBC = null, rgnComplement = null;

            try
            {
                foreach (VDEItem vdi in VDEItems.Where(x => x.IsBarcode2D))
                {
                    int ROW1 = vdi.BarcodeRegion[0] - (Defaults.PADDING_BARCODE / 2);
                    int COL1 = vdi.BarcodeRegion[1] - (Defaults.PADDING_BARCODE / 2);
                    int ROW2 = vdi.BarcodeRegion[2] + (Defaults.PADDING_BARCODE / 2);
                    int COL2 = vdi.BarcodeRegion[3] + (Defaults.PADDING_BARCODE / 2);
                    HOperatorSet.GenRectangle1(out rgnBC, ROW1, COL1, ROW2, COL2);
                    HOperatorSet.Complement(rgnBC, out rgnComplement);
                    HOperatorSet.ReduceDomain(img, rgnComplement, out img);
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "MaskBarcodes2D() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            finally
            {
                if (rgnBC != null) rgnBC.Dispose();
                if (rgnComplement != null) rgnComplement.Dispose();
            }
            return retVal;
        }

        public bool MaskBarcodesLinear(ref HObject img, ref FailRecord fp)
        {
            bool retVal = true;
            HObject rgnBC = null, rgnComplement = null;

            try
            {
                foreach (VDEItem vdi in VDEItems.Where(x => x.IsBarcodeLinear))
                {
                    int ROW1 = vdi.BarcodeRegion[0] - (Defaults.PADDING_BARCODE / 2);
                    int COL1 = vdi.BarcodeRegion[1] - (Defaults.PADDING_BARCODE / 2);
                    int ROW2 = vdi.BarcodeRegion[2] + (Defaults.PADDING_BARCODE / 2);
                    int COL2 = vdi.BarcodeRegion[3] + (Defaults.PADDING_BARCODE / 2);
                    HOperatorSet.GenRectangle1(out rgnBC, ROW1, COL1, ROW2, COL2);
                    HOperatorSet.Complement(rgnBC, out rgnComplement);
                    HOperatorSet.ReduceDomain(img, rgnComplement, out img);
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "MaskBarcodes2D() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            finally
            {
                if (rgnBC != null) rgnBC.Dispose();
                if (rgnComplement != null) rgnComplement.Dispose();
            }
            return retVal;
        }

        public bool InspectBarcodesLinear(ref HObject img, ref FailRecord fp)
        {
            bool retVal = true;
            HObject tmpObj = null, region = null, barcodeBin = null, barCodeObjects = null;
            HTuple BarCodeHandle = null;
            HTuple ROW1 = 0, COL1 = 0, ROW2 = 0, COL2 = 0;
            try
            {
                List<string> datas = new List<string>();
                List<string> placeholders = new List<string>();
                foreach (VDEItem vdi in VDEItems)
                {
                    if (!vdi.IsBarcodeLinear)
                        continue;
                    datas = new List<string>();
                    placeholders = new List<string>();
                    if (tmpObj != null)
                        tmpObj.Dispose();
                    if (region != null)
                        region.Dispose();
                    if (barcodeBin != null)
                        barcodeBin.Dispose();
                    if (barCodeObjects != null)
                        barCodeObjects.Dispose();
                    if (vdi.IsBarcodeLinear)
                    {
                        fp.vDEType = VDEType.BARCODE_LINEAR;
                        HOperatorSet.GetImageSize(img, out HTuple w, out HTuple h);
                        int t = vdi.BarcodeRegion[0] - 200;
                        int l = vdi.BarcodeRegion[1] - 200;
                        int b = vdi.BarcodeRegion[2] + 200;
                        int r = vdi.BarcodeRegion[3] + 200;
                        if (t < 0)
                            t = 0;
                        if (l < 0)
                            l = 0;
                        if (b > h)
                            b = h;
                        if (r > w)
                            r = w;
                        HOperatorSet.GenRectangle1(out region, t, l, b, r);
                        HOperatorSet.ReduceDomain(img, region, out tmpObj);
                        HOperatorSet.GaussFilter(tmpObj, out tmpObj, 5);
                        string barcodeType = vdi.BarcodeName;
                        HOperatorSet.CreateBarCodeModel(new HTuple("composite_code"), new HTuple("CC-A/B"), out BarCodeHandle);
                        HOperatorSet.SetBarCodeParam(BarCodeHandle, "stop_after_result_num", 0);
                        HOperatorSet.SetBarCodeParam(BarCodeHandle, "barcode_height_min", 50);
                        HOperatorSet.SetBarCodeParam(BarCodeHandle, "min_identical_scanlines", 2);
                        //set_bar_code_param
                        HOperatorSet.FindBarCode(tmpObj, out HObject SymbolRegions, BarCodeHandle, "auto", out HTuple BarcodeAsText);
                        if (BarcodeAsText.Length > 0)
                        {
                            string med = BarcodeAsText.S;
                            foreach (MedData md in MedDataItems)
                                if (md.PlaceHolder == vdi.Placeholder)
                                    if (med == md.Data)
                                    {
                                        datas.Add(med);
                                        placeholders.Add(vdi.Placeholder);
                                    }
                            if (!UtilityFunctions.AlignsWith(vdi.BarcodeData, datas))
                            {
                                //if (errorRaisedToPLC == false)
                                //{
                                //    SYSTEM_IO.FAIL_OCCURED();
                                //    errorRaisedToPLC = true;
                                //}
                                string result = fp.MED_ID;
                                fp.VALID_LABEL = false;
                                RegionFailData rfd = new RegionFailData("Barcode");
                                if (rfd.Img == null)
                                    HOperatorSet.GenEmptyObj(out rfd.Img);
                                HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                                rfd.Reasons.Add(string.Format("{0} barcode data error: Found: {1}, searching for: {2}", barcodeType, string.Join(",", datas), string.Join(",", vdi.BarcodeData)));
                                fp.RegionFailDataList.Add(rfd);
                                HOperatorSet.SmallestRectangle1(region, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                                rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1, c1, r2, c2 }));
                                fp.DATA_FOUND = false;
                                fp.ACCEPTED = false;
                                HOperatorSet.ClearBarCodeModel(BarCodeHandle);
                                fp.BARCODE_UNREADABLE_LINEAR = true;
                            }
                        }
                        else
                        {
                            //if (errorRaisedToPLC == false)
                            //{
                            //    SYSTEM_IO.FAIL_OCCURED();
                            //    errorRaisedToPLC = true;
                            //}
                            retVal = false;
                            RegionFailData rfd = new RegionFailData("Barcode");
                            if (rfd.Img == null)
                                HOperatorSet.GenEmptyObj(out rfd.Img);
                            HOperatorSet.CopyObj(img, out rfd.Img, 1, 1);
                            rfd.Reasons.Add(string.Format("{0} barcode data error: Found: {1}, searching for: {2}", barcodeType, "no data", string.Join(",", vdi.BarcodeData)));
                            rfd.Reasons.Add("No barcode found within extended search area");
                            fp.RegionFailDataList.Add(rfd);
                            HOperatorSet.SmallestRectangle1(region, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                            rfd.failCoordsList.Add(new RegionCoordPoints(new int[] { r1, c1, r2, c2 }));
                            fp.ACCEPTED = false;
                            fp.BARCODE_UNREADABLE_LINEAR = true;
                            HOperatorSet.ClearBarCodeModel(BarCodeHandle);
                        }
                        HOperatorSet.ClearBarCodeModel(BarCodeHandle);
                    }
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "InspectBarcodesLinear() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            finally
            {
                if (tmpObj != null)
                    tmpObj.Dispose();
                if (region != null)
                    region.Dispose();
                if (barcodeBin != null)
                    barcodeBin.Dispose();
                if (barCodeObjects != null)
                    barCodeObjects.Dispose();
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
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
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
                foreach (VDEItem vdi in VDEItems.Where(x => x.OpZoneName.ToUpper() == opzonename.ToUpper() && x.IsVDE == true && x.VDERotatedAngle == angle))
                {

                    if (RegionComplement != null)
                        RegionComplement.Dispose();
                    if (rgn != null)
                        rgn.Dispose();

                    foreach (MedData meddata in MedDataItems)
                        if (meddata.PlaceHolder == vdi.Placeholder)
                        {
                            vdi.RepeatType = meddata.Repeat;
                            break;
                        }

                    if (vdi.RepeatType > 0)
                    {
                        HOperatorSet.GenRectangle1(out rgn, vdi.VDERegion[0] - 4, vdi.VDERegion[1] - 8, vdi.VDERegion[2] + 4, vdi.VDERegion[3] + 8);
                        HOperatorSet.Complement(rgn, out RegionComplement);
                        HOperatorSet.ReduceDomain(img, RegionComplement, out img);
                    }
                    //break;
                }
            }
            catch (Exception ex)
            {
                string err = "maskVDE() err: " + ex.Message;
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
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
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }
        #endregion

        private bool attachBackgroundToWindow(ref HObject img, ref FailRecord fp)
        {
            bool retVal = true;
            try
            {
                if (CurrentReducedImage != null)
                    CurrentReducedImage.Dispose();
                HOperatorSet.CopyObj(img, out CurrentReducedImage, 1, 1);
                try { HOperatorSet.DetachBackgroundFromWindow(hWin.HalconWindow); } catch { }
                hWin.HalconWindow.ClearWindow();
                HOperatorSet.AttachBackgroundToWindow(CurrentReducedImage, hWin.HalconWindow);
                hWin.SetFullImagePart();
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "attachBackgroundToWindow() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        private bool attachBackgroundToWindowWithImageLibrary(ref HObject img, ref FailRecord fp)
        {
            bool retVal = true;
            try
            {
                if (CurrentReducedImage != null)
                    CurrentReducedImage.Dispose();
                if (CurrentRawImage != null)
                {
                    CurrentRawImage.Dispose();
                }
                HOperatorSet.CopyObj(img, out CurrentReducedImage, 1, 1);
                HOperatorSet.CopyObj(img, out CurrentRawImage, 1, 1);
                try { HOperatorSet.DetachBackgroundFromWindow(hWin.HalconWindow); } catch { }
                hWin.HalconWindow.ClearWindow();
                HOperatorSet.AttachBackgroundToWindow(CurrentReducedImage, hWin.HalconWindow);
                hWin.SetFullImagePart();
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "attachBackgroundToWindow() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        public void GetImageInspection()
        {
            try
            {
                clearImage();
                if (ENABLE_CAPTURE)
                {
                    mxClient.WriteToRegister(1, "Capture_Image", 1, 3);
                    if (cameraNecta.nectaCam.Acquire == false)
                        cameraNecta.nectaCam.Acquire = true;
                    Task t = new Task(() => cameraNecta.GrabCameraImage(ProcessInspectionImage));
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                string err = "GetImageInspection() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Image Capture", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
        }

        private List<string> medPlaceHolders()
        {
            List<string> retVal = new List<string>();            
            foreach (MedData md in MedDataItems)
            {                
                retVal.Add(md.PlaceHolder);
            }            
            return retVal;
        }

        private List<string> medDatas()
        {
            List<string> retVal = new List<string>();
            foreach (MedData md in MedDataItems)
            {
                retVal.Add(md.Data);
            }
            return retVal;
        }

        private string currentMed()
        {
            string retVal = "";
            List<string> meddatas = medDatas();
            foreach (MedData md in MedDataItems)
                if (md.VarName.ToUpper().Contains("MED_ID") || md.VarName.ToUpper().Contains("<MED_ID>") || md.VarName.ToUpper().Contains("<MED>") || md.PlaceHolder.ToUpper().Contains("<MED "))
                {
                    retVal = md.Data;
                    break;
                }
            return retVal;
        }

        private bool InspectionEnd(ref HObject imgreduced, ref FailRecord fp)
        {            
            SystemMessageEventArgs smea = null;
            RegionFailData rfd = null;
            HObject tmp = null;
            try
            {                
                if ((Positioner.ImageCounter == LABEL_COUNT) && (SAMPLE_LABEL != ""))
                {                                       
                    return false;
                }
                else if ((Positioner.ImageCounter == LABEL_COUNT) && (SAMPLE_LABEL == ""))
                {
                    Console.WriteLine("Count Reached Without Sample");
                    if (cameraNecta.nectaCam.Acquire == true)
                    {
                        cameraNecta.nectaCam.Acquire = false;
                    }
                    SYSTEM_IO.REEL_END();
                    return false;
                }
                else if ((Positioner.ImageCounter == LABEL_COUNT + 1) && (SAMPLE_LABEL != ""))
                {                    
                    if (cameraNecta.nectaCam.Acquire == true)
                    {
                        cameraNecta.nectaCam.Acquire = false;
                    }

                    SYSTEM_IO.FAIL_OCCURED();
                    SYSTEM_IO.REEL_END();
                    Positioner.MoveLast(this);
                    FailRecord.FailPipes.Add(fp);
                    HOperatorSet.CopyObj(imgreduced, out tmp, 1, 1);
                    ReduceBackground(ref tmp, ref fp);
                    fp.SAMPLE = true;
                    fp.ACTIONED = false;
                    fp.LabelIndex = LABEL_COUNT + 1;
                    rfd = new RegionFailData("SAMPLE LABEL");
                    fp.RegionFailDataList.Add(rfd);
                    if (rfd.Img == null)
                    {
                        HOperatorSet.GenEmptyObj(out rfd.Img);
                    }

                    HOperatorSet.CopyObj(tmp, out rfd.Img, 1, 1);
                    if (tmp != null)
                    {
                        tmp.Dispose();
                    }
                    return true;
                }
                
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                string err = "InspectionEnd() err: " + ex.Message;
                smea = new SystemMessageEventArgs(err, "Inspection Result", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
            finally
            {
                if (tmp != null)
                {
                    tmp.Dispose();
                }
            }
            return false;
        }

        private FailRecord getNewFailRecord()
        {
            FailRecord retVal = new FailRecord(REEL_LPN, Positioner.ImageCounter)
            {
                PlaceHolders = medPlaceHolders(),
                Datas = medDatas()
            };
            return retVal;
        }

        private bool InspectVDE(ref HObject img, ref FailRecord fp, OPZoneData ozd)
        {
            if (labelStopIndex == stopAtIndex)
            {
                
            }
            bool retVal = true;
            try
            {
                int angle = 0;
                VDE vde = new VDE(OPZoneItems, VDEItems, MedDataItems, uscMD, this.pauseMethodCaller);
                vde.CountMatchError = false;
                while (angle < 360)
                {
                    int usedangle = vde.RotateAdjustment(angle);
                    List<VDEItem> vdeItems = new List<VDEItem>();
                    foreach (VDEItem vdi in VDEItems.Where(x => x.OpZoneName.ToUpper() == ozd.NAME.ToUpper() && x.IsVDE == true && x.VDERotatedAngle == usedangle))
                    {
                        foreach (MedData meddata in MedDataItems)
                            if (meddata.PlaceHolder == vdi.Placeholder)
                            {
                                vdi.RepeatType = meddata.Repeat;
                                break;
                            }
                        vdeItems.Add(vdi);
                    }
                    if (vdeItems.Count == 0)
                    {
                        angle += 90;
                        continue;
                    }
                    if (retVal == true)
                    {
                        retVal = vde.FindVDE(angle, usedangle, ref img, ref fp, ozd, vdeItems, VariableMedDataPH);
                    }
                    else
                    {
                        vde.FindVDE(angle, usedangle, ref img, ref fp, ozd, vdeItems, VariableMedDataPH);

                    }                        
                    if (fp.DATA_INCOMPLETE == true || fp.DATA_FOUND == false || fp.DatasNotFound.Count > 0)
                        retVal = false;
                    if (vde.CountMatchError == true)
                        retVal = false;
                    angle += 90;
                }
                vde = null;
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "InspectVDE() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Image Capture", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                //EMH?.Invoke(err);
            }
            return retVal;
        }

        private bool _debug_inspection(FailRecord fp)
        {
            int bypassInspectionCount = 350; //change to positive value to skip nn images
            if (fp.LabelIndex <= bypassInspectionCount)
            {
                SpeedControl.UpdateSpeedControl(290);                
                dispInfoText(string.Format("debug: label index {0} not inspected", fp.LabelIndex));
                fp.VALID_LABEL = true;
                return true;
            }
            return false;
        }


        private void ProcessInspectionImage()
        {
            EdgesNotDetectedError = false;
            if (!InhibitNextCapture)
            {
                //bool errorRaisedToPLC = false;
                while (REVIEWING)
                {
                    Application.DoEvents();
                }
                INSPECTING = true;

                Stopwatch sw = new Stopwatch();
                HObject tmp1 = null;
                long ms = 0;
                SystemMessageEventArgs smea = null;
                FailRecord fp = null;
                bool inspectionPassedOK = false;
                try
                {
                    sw.Start();
                    if (CurrentReducedImage != null)
                    {
                        CurrentReducedImage.Dispose();
                    }
                    displayClearInspection();
                    Positioner.MoveNext(this);
                    Positioner.CAN_MOVE_NEXT = true;

                    fp = getNewFailRecord();
                    foreach (MedData med in MedDataItems)
                    {        
                        if (med.PlaceHolder == VariableMedDataPH || med.VarName.ToLower().Contains("med_id"))
                        {
                            fp.MED_ID = med.Data;
                            break;
                        }
                    }

                    if (!prepareLabelImageFromCameraImage(ref tmp1))
                    {
                        return;
                    }

                    if (Positioner.ImageCounter >= LABEL_COUNT)
                    {
                        if (InspectionEnd(ref tmp1, ref fp))
                        {
                            return;
                        }

                    }

                    Labelcounts.CountInspected += 1;
                    if (SAMPLE_LABEL != "")
                    {
                        Labelcounts.HasSample = true;
                    }

                    // incremented here before fails occur - value is decremented if all tests pass
                    //failCount++;

                    ////****************************************************************************************
                    //UnRem this block to fast forward through labels without inspection (qty to bypass is set in _debug_inspection() method)
                    //if (_debug_inspection(fp))
                    //    return;
                    ////****************************************************************************************

                    //mxClient.Stop(1);

                    if (ReduceBackground(ref tmp1, ref fp))
                    {
                        ////****************************************************************************************
                        //UnRem this block to get cropped labels written to disk as bitmaps - saves cropped labels without inspection
                        //var ret1 = attachBackgroundToWindow(ref tmp1, ref fp);
                        //NoInspectionSaveImagesToFile(tmp1, 50);
                        //fp.VALID_LABEL = true;
                        //return;
                        ////****************************************************************************************

                        //mxClient.Stop(1);

                        bool ret = attachBackgroundToWindow(ref tmp1, ref fp);
                        if (ret)
                        {                            
                            if (SYSTEM_IO.PROCESSING == false)
                            {
                                return;
                            }

                            InspectOpZone(ref tmp1, ref fp);
                            if (SYSTEM_IO.PROCESSING == false)
                            {
                                return;
                            }
                            InspectLabel(tmp1, ref fp);
                            if (SYSTEM_IO.PROCESSING == false)
                            {
                                return;
                            }

                            if (fp.VALID_LABEL == true && fp.ConfidenceLevel >= ConfidenceLevel)
                            {
                                inspectionPassedOK = true;
                                displayGoodInspection(fp.LabelIndex);
                            }
                        }
                    }                    
                    if (SYSTEM_IO.PROCESSING)
                    {
                        if (inspectionPassedOK)
                        {
                            Labelcounts.CountAccepted += 1;
                            if (fp.SAMPLE == false)
                            {
                                fp.ClearData();
                                fp = null;
                            }
                        }
                        else
                        {
                            AddFailAndStop(fp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (SYSTEM_IO.PROCESSING == false)
                        return;
                    SYSTEM_IO.PROCESSING = false;
                    string err = "ProcessInspectionImage() err: " + ex.Message;
                    smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                    uscMD.SystemMessage(smea);
                    EMH?.Invoke(err);
                }
                finally
                {
                    if (tmp1 != null)
                    {
                        tmp1.Dispose();
                    }
                    if (CurrentRawImage != null)
                    {
                        CurrentRawImage.Dispose();
                    }

                    INSPECTING = false;
                    sw.Stop();
                    ms = sw.ElapsedMilliseconds;
                    if (!inspectionPassedOK)
                    {
                        dispTimeText("");
                    }
                    if (ms < 100)
                        ms = 150;
                    else if (ms < 200)
                        ms = 300;
                    else if (ms < 400)
                        ms = 600;
                    else if (ms < 500)
                        ms = 800;
                    else if (ms < 1000)
                        ms = 1000;
                    SpeedControl.UpdateSpeedControl(ms);
                }
            }
            
            else
            {
                InhibitNextCapture = false;
                //Console.WriteLine("");
            }
        }


        private void NoInspectionSaveImagesToFile(HObject img, int imgcount)
        {
            try
            {
                SpeedControl.UpdateSpeedControl(650);

                if (Positioner.ImageCounter < 1)
                    return;

                if (Positioner.ImageCounter <= imgcount)
                {
                    string folder = Application.StartupPath;
                    string applicationFolder = Path.Combine(folder, "ImageDump");
                    //Directory.Delete(applicationFolder,true);
                    Directory.CreateDirectory(applicationFolder);
                    HOperatorSet.WriteImage(img, "bmp", 0, Path.Combine(applicationFolder, Positioner.ImageCounter.ToString() + ".bmp"));
                    displayImageCapture();
                }
                else
                {
                    mxClient.Stop(1);
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                string err = "image capture" + ex.Message;
                MessageBox.Show(err);
            }
        }


        private bool prepareLabelImageFromCameraImage(ref HObject img)
        {
            bool retVal = true;
            SystemMessageEventArgs smea;
            try
            {
                if (img != null)
                {
                    img.Dispose();
                }
                hWin.HalconWindow.ClearWindow();
                if (!ENABLE_CAPTURE)
                {
                    if (cameraNecta.CameraImage != null)
                    {
                        cameraNecta.CameraImage.Dispose();
                    }
                    return true;
                }
                if (cameraNecta.CameraImage == null)
                {
                    string err = "prepareLabelImageFromCameraImage() err: No image returned from Label Camera";
                    smea = new SystemMessageEventArgs("No image returned from Label Camera", "Inspection Error", (int)CriticalLevels.Red);
                    displayNullImageFromCamera(cameraNecta.AliasName);
                    uscMD.SystemMessage(smea);
                    if (SYSTEM_IO.PROCESSING == true)
                    {
                        EMH?.Invoke(err);
                    }

                    SYSTEM_IO.PROCESSING = false;
                    return false;
                }
                HOperatorSet.CopyObj(cameraNecta.CameraImage, out img, 1, 1);

                if (CurrentRawImage != null)
                {
                    CurrentRawImage.Dispose();
                }
                HOperatorSet.CopyObj(img, out CurrentRawImage, 1, 1);              

            }
            catch (Exception ex)
            {
                CurrentRawImage.Dispose();
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                string err = "prepareLabelImageFromCameraImage() err: " + ex.Message;
                smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        private void AddFailAndStop(FailRecord fp)
        {
            try
            {
                if (labelStopIndex == stopAtIndex)
                {
                    //Console.WriteLine("Stopped");
                }
                Bitmap bmp = null;
                bmp = InspectionStatus.GetImage(false);
                pbPassFail.Image = bmp;
                string sMed = "";
                
                foreach (MedData med in MedDataItems)
                {
                    if (med.PlaceHolder == VariableMedDataPH)
                    {
                        sMed = med.Data;
                        break;
                    }
                }

                string passResult = string.Format("Label index " + Positioner.ImageCounter.ToString() + " (Med {0}) ", sMed);
                if (fp.VALID_LABEL == false || fp.DUPLICATE == true || fp.READABLE == false || fp.DATA_FOUND == false || fp.DATA_INCOMPLETE == true || fp.BARCODE_UNREADABLE_LINEAR == true || fp.BARCODE_UNREADABLE_2D == true || fp.ConfidenceLevel < ConfidenceLevel)
                {
                    if (fp.MISSING == false)
                    {
                        InhibitNextCapture = true;
                        if (EdgesNotDetectedError == false)
                        {
                            SYSTEM_IO.FAIL_OCCURED();
                        }                        
                    }
                    EdgesNotDetectedError = false;
                }
                string reasons = "";               

                if (fp.RegionFailDataList.Count > 0)
                {                    
                    foreach (string r in fp.RegionFailDataList[0].Reasons)
                    {
                        if (!r.ToLower().Contains("sample"))
                        {
                            if(reasons == "")
                                reasons = reasons + r;
                            else
                                reasons = reasons + ", " + r;

                        }                        
                    }                        
                }

                SystemMessageEventArgs smea = new SystemMessageEventArgs(passResult + reasons, "Inspection Result", (int)CriticalLevels.Black, fp.LabelIndex);
                uscMD.SystemMessage(smea);
                if (fp.ACTIONED == false && fp.SAMPLE == false)
                    FailRecord.FailPipes.Add(fp);
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                string err = "AddFailAndStop() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Error", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
        }


        private bool setFailPipeData(ref FailRecord fp, PositionCheck poscheck, out FailRecord fpout, string opzonename)
        {
            if (labelStopIndex == stopAtIndex)
            {
                
            }
            
            RegionFailData rfd = null;
            bool retVal = false;
            fpout = null;
            try
            {
                //poscheck = PositionCheck.IS_UNREADABLE;
                if (poscheck == PositionCheck.IS_NEXT)
                {

                    
                }

                else if (poscheck == PositionCheck.IS_DUPLICATE)
                {
                    if (rfd == null)
                        rfd = new RegionFailData("Duplicate Label");
                    fp.RegionFailDataList.Add(rfd);
                    fp.DUPLICATE = true;
                    Positioner.CAN_MOVE_NEXT = true;
                    retVal = false;
                }
                else if (poscheck == PositionCheck.IS_INCOMPLETE)
                {
                    fp.DATA_INCOMPLETE = true;
                    retVal = false;
                }

                else if (poscheck == PositionCheck.IS_UNREADABLE)
                {
                    fp.READABLE = false;
                    Positioner.CAN_MOVE_NEXT = false;
                    rfd.Reasons.Clear();
                    rfd.Reasons.Add("Unreadable Label. Expected: " + string.Join("", fp.Datas));
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "setFailPipeData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        private PositionCheck StatusCheck(ref FailRecord fp)
        {
            PositionCheck retVal = PositionCheck.IS_GOOD;
            
            try
            {
                if (LabelIsUnReadable(ref fp))
                {
                    retVal = PositionCheck.IS_UNREADABLE;
                    return retVal;
                }
                if (LabelIsDuplicate(ref fp))
                {
                    retVal = PositionCheck.IS_DUPLICATE;
                    return retVal;
                }                

                if (fp.DatasNotFound.Count > 0 || fp.DATA_INCOMPLETE == true)
                {
                    retVal = PositionCheck.IS_INCOMPLETE;
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return PositionCheck.IS_GOOD;
                }
                SYSTEM_IO.PROCESSING = false;
                retVal = PositionCheck.ERROR;
                string err = "DoPositionChecks() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        private bool LabelIsDuplicate(ref FailRecord fp)
        {            
            bool retVal = false;
            try
            {
                for (int x = 0; x < fp.Datas.Count; x++)
                {
                    string data = fp.Datas[x];
                    string placeholder = fp.PlaceHolders[x];
                    foreach (MedData meddata in MedDataItems)
                    {
                        if (meddata.Repeat > 0)
                            if (meddata.IsDuplicate(data))
                            {
                                retVal = true;
                                break;
                            }
                    }
                    if (retVal == true)
                        break; ;
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }

                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "LabelIsDuplicate() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        private bool LabelIsNext(ref FailRecord fp)
        {            
            bool retVal = false;

            try
            {
                if (fp.DatasNotFound == null)
                {
                    return retVal;
                }
                if (fp.DatasNotFound.Count == 0)
                {
                    return retVal;
                }
                for (int x = 0; x < fp.DatasNotFound.Count; x++)
                {
                    if (retVal == true)
                        break;
                    string[] dataNotFound = fp.DatasNotFound[x].Split('|');
                    if (dataNotFound.Length >= 2)
                        foreach (MedData meddata in MedDataItems)
                            if (meddata.PlaceHolder == dataNotFound[1].Trim())
                                if (meddata.Repeat > 0)
                                {
                                    retVal = meddata.IsNext(dataNotFound[0], dataNotFound[2]);
                                    if (retVal == true)
                                        break;
                                }
                }

            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "LabelIsNext() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        private bool LabelIsLast()
        {
            bool retVal = false;
            try
            {
                foreach (MedData meddata in MedDataItems)
                    if (meddata.Repeat > 0)
                    {
                        retVal = meddata.IsLast();
                        if (retVal == true)
                            break;
                    }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "LabelIsLast() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        private bool LabelIsUnReadable(ref FailRecord fp)
        {
            bool retVal = false;
            try
            {
                foreach (MedData med in MedDataItems)
                {
                    string s = med.Data;
                    foreach (string dat in fp.Datas)
                        if (s == dat)
                            retVal = false;
                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "LabelIsUnReadable() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        public bool MoveToNextVDE()
        {
            bool retVal = true;
            try
            {
                foreach (MedData meddata in MedDataItems)
                    meddata.MoveNext();
                moveNextCaller?.Invoke();
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                string err = "MoveToNextVDE() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }


        public string PreviousMedData()
        {
            string retVal = "";
            try
            {
                foreach (MedData meddata in MedDataItems)
                    retVal = retVal + meddata.PeekPrevious();
            }
            catch (Exception ex)
            {
                string err = "PreviousMedData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                //failmethodCaller?.Invoke();
            }
            return retVal;
        }

        public void MoveLast()
        {
            try
            {
                foreach (MedData meddata in MedDataItems)
                    meddata.MoveLast();
            }
            catch (Exception ex)
            {
                string err = "MoveLast() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
            }
        }

        public string ThisMedData()
        {
            string retVal = "";
            try
            {
                foreach (MedData meddata in MedDataItems)
                    retVal = retVal + meddata.PeekThis();
            }
            catch (Exception ex)
            {
                string err = "ThisMedData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                //failmethodCaller?.Invoke();
            }
            return retVal;
        }

        private bool clearImage()
        {
            bool retVal = true;
            try
            {
                if (cameraNecta.CameraImage != null)
                    cameraNecta.CameraImage.Dispose();
                try { HOperatorSet.DetachBackgroundFromWindow(hWin.HalconWindow); } catch { }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                string err = "ClearImage()  err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Image Capture", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
            return retVal;
        }

        private void displayNullImageFromCamera(string devicename)
        {
            try
            {
                string noImage = string.Format("\n\n\n                 {0}\n\n\n                 No Image returned", devicename);
                //waitingForImage = false;
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                string err = "displayNullImageFromCamera() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Image Grab", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                EMH?.Invoke(err);
            }
        }

        private async void AutoModeDelay()
        {
            
            while (frmInspect.frmUI == null)
            {
                await Task.Delay(2000);
            }

            await Task.Delay(15000);

            if(frmInspect.frmUI != null)
            {
                frmInspect.frmUI.AutoHandle();
            }            

        }

        private void IO_INTERRUPT_Handler(int channel, IOEventArgs e)
        {
            if (SYSTEM_IO.PROCESSING == false)
                return;

            if (REVIEWING)
                return;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < 60)
                ;
            sw.Stop();

            while (INSPECTING)
                ;

            if (REVIEWING)
                return;

            if (ENABLE_CAPTURE)
            {
                REVIEWING = true;
                CloseAllReviewForms();
                doReviewLUI();

                if ((ReviewLUIMode == "Auto") && (Positioner.ImageCounter < (LABEL_COUNT - 5)))
                {
                    //AutoModeDelay();
                }
            }
        }

        private void IO_COS_Handler(int channel, bool high)
        {
            if (SYSTEM_IO.PROCESSING == false)
            {
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                while (sw1.ElapsedMilliseconds < 50)
                    ;
                sw1.Stop();
                if (channel == SYSTEM_IO.END_OF_INSPECTION)
                {
                    ReviewLUIModeIndex = 0; // Clear the auto mode index count
                    AMH -= LVS3.frmInspect.AMD;
                    EMH -= LVS3.frmInspect.EMD;
                    SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
                    SYSTEM_IO.IO_INTERRUPT_Handler -= IO_INTERRUPT_Handler;
                    SYSTEM_IO.IO_CHANGE_Handler -= frmI.IO_COS_Handler;
                }
                return;
            }
            else
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < 50)
                    ;
                sw.Stop();
                if (channel == SYSTEM_IO.END_OF_INSPECTION)
                {
                    ReviewLUIModeIndex = 0; // Clear the auto mode index count
                    SYSTEM_IO.PROCESSING = false;
                    AMH -= LVS3.frmInspect.AMD;
                    EMH -= LVS3.frmInspect.EMD;
                    SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
                    SYSTEM_IO.IO_INTERRUPT_Handler -= IO_INTERRUPT_Handler;
                    SYSTEM_IO.IO_CHANGE_Handler -= frmI.IO_COS_Handler;
                    completeInspection();
                    CloseAllInspectionForms();
                }
                else if (channel == SYSTEM_IO.ALARM)
                {
                    SYSTEM_IO.PROCESSING = false;
                    EMH -= LVS3.frmInspect.EMD;
                    SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
                    SYSTEM_IO.IO_INTERRUPT_Handler -= IO_INTERRUPT_Handler;
                    AMH?.Invoke();
                    AMH -= LVS3.frmInspect.AMD;
                }
            }
        }

        private void completeInspection()
        {
            try
            {
                SYSTEM_IO.PROCESSING = false;
                if (cameraNecta.nectaCam.Acquire == true)
                    cameraNecta.nectaCam.Acquire = false;
                mxClient.WriteToRegister(1, "Mode_Inspect", 0, 3);

                bool hasSample = SAMPLE_LABEL != "" ? true : false;
                
                if (REEL_LPN == "")
                {                    
                    REEL_LPN = FailRecord.FailPipes[0].REEL;
                }                    
                EndInspectionReel(FailRecord.FailPipes, hasSample);
            }
            catch (Exception ex)
            {
                string err = "completeInspection() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        public static void CloseAllReviewForms()
        {            
            try
            {
                int openCount = Application.OpenForms.OfType<frmUnderInvestigation>().Count();
                if (openCount > 0)
                {
                    List<frmUnderInvestigation> openList = Application.OpenForms.OfType<frmUnderInvestigation>().ToList();
                    for (int x = openCount - 1; x >= 0; x--)
                    {
                        frmUnderInvestigation f = openList[x];

                        if (f.InvokeRequired)
                        {
                            f.Invoke((System.Windows.Forms.MethodInvoker)delegate
                            {
                                try { f.Close(); } catch { }
                            });
                        }
                        else
                        {
                            try { f.Close(); } catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = "CloseAllReviewForms() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        public static void CloseAllInspectionForms()
        {
            try
            {
                int openCount = Application.OpenForms.OfType<frmInspect>().Count();
                if (openCount > 0)
                {
                    List<frmInspect> openList = Application.OpenForms.OfType<frmInspect>().ToList();
                    for (int x = openCount - 1; x >= 0; x--)
                    {
                        frmInspect f = openList[x];
                        
                        if (f.InvokeRequired)
                        {
                            f.Invoke((System.Windows.Forms.MethodInvoker)delegate
                            {
                                //try { f.Hide(); } catch { }
                                try { f.Close(); } catch { }
                            });
                        }
                        else
                        {
                            //try { f.Hide(); } catch { }
                            try { f.Close(); } catch { }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                string err = "CloseAllInspectionForms() err: " + ex.Message;
                MessageBox.Show(err);
            }
        }

        private void EndInspectionReel(List<FailRecord> faillist, bool hassample)
        {
            if (REEL_LPN == correctREEL_LPN)
            {
                
            }
            else
            {
                
            }

            ESignature es = null;
            string FAIL_FOLDER = "";
            StringBuilder sb = new StringBuilder();
            SystemMessageEventArgs smea = null;
            try
            {
                try { frmInspect.EMD -= new ErrorMethodHandler(frmI.do_error_end); } catch { }
                try { frmInspect.EMD -= frmI.do_error_end; } catch { }

                int sample = hassample == true ? 1 : 0;
                es = new ESignature(ESigReason.EndReel, Roles.LVSIII_USer, "");
                es.AuthenticationOnly = true;
                es.ReasonRequired = true;
                es.CanCancel = false;
                es.DoNotAcceptLastUser = false;
                es.CaptureSignature();
                if (es.SignatureAccepted)
                {
                    FAIL_FOLDER = ImageData.CreateFailFilepath(frmI.lblReelLPN.Text);
                    int missing = faillist.Where(x => x.MISSING == true).Count();
                    int accept = frmInspect.labelCount - faillist.Where(x => x.ACCEPTED == true).Count();
                    int reject = faillist.Count();
                    int acceptop = faillist.Where(x => x.ACCEPTED_BY_USER == true && x.SAMPLE == false).Count();
                    int rejectop = faillist.Where(x => x.ACCEPTED_BY_USER == false).Count();
                    SYSTEM_IO.PROCESSING = false;
                    if (frmI.InvokeRequired)
                    {
                        frmI.Invoke((System.Windows.Forms.MethodInvoker)delegate
                        {
                            frmI.startProgress(acceptop + rejectop + missing + sample);
                            smea = new SystemMessageEventArgs("Creating inspection report..", "Inspection Complete", (int)Enums.CriticalLevels.Black);
                            uscMD.SystemMessage(smea);
                            ENABLE_CAPTURE = false;

                            string lin = frmI.lblLIN.Text;
                            int endOfLIN = lin.IndexOf("Issue No");
                            if (endOfLIN > -1)
                                lin = lin.Substring(0, endOfLIN - 1).Trim();
                            DataManager.SummaryDataInspectionFinish(REEL_LPN, lin, es.LastUserName, frmInspect.labelCount, missing, accept, reject, acceptop, rejectop, es.UserReason);
                            InspectionReport rs = new InspectionReport(REEL_LPN, lin, frmInspect.PH, es.UserReason, es.LastUserName, INSPECTION.Labelcounts, frmInspect.labelCount, frmI.LABEL_WORK_ORDER, VariableMedDataPH);
                            if (rs.CreateDocument())
                            {
                                UtilityFunctions.ReleaseHalconMemoryVariables();
                                DataManager.SaveAction("Inspection Complete", "Inspection Completed", REEL_LPN, es.LastUserName, "EndInspectionReel()", REEL_LPN, es.UserReason);
                                CameraManager.NectaCameras[0].nectaCam.Acquire = false;
                                frmI.SetMenuOptions(MenuOptions.IDLE);
                                smea = new SystemMessageEventArgs("Inspection Completed successfully", "Inspection", (int)Enums.CriticalLevels.Black);
                                uscMD.SystemMessage(smea);
                            }
                            else
                            {
                                smea = new SystemMessageEventArgs("Inspection report creation error.", "Inspection Report", (int)Enums.CriticalLevels.Black);
                                uscMD.SystemMessage(smea);
                            }
                        });
                    }
                    else
                    {
                        frmI.startProgress(acceptop + rejectop + missing + sample);
                        smea = new SystemMessageEventArgs("Creating inspection report..", "Inspection Complete", (int)Enums.CriticalLevels.Black);
                        uscMD.SystemMessage(smea);
                        ENABLE_CAPTURE = false;
                        string lin = frmI.lblLIN.Text;
                        int endOfLIN = lin.IndexOf("Issue No");
                        if (endOfLIN > -1)
                        {
                            lin = lin.Substring(0, endOfLIN - 1).Trim();
                        }
                        DataManager.SummaryDataInspectionFinish(REEL_LPN, lin, es.LastUserName, faillist.Count, missing, accept, reject, acceptop, rejectop, es.UserReason);
                        InspectionReport rs = new InspectionReport(REEL_LPN, lin, frmInspect.PH, es.UserReason, es.LastUserName, INSPECTION.Labelcounts, frmInspect.labelCount, frmI.LABEL_WORK_ORDER, VariableMedDataPH);
                        if (rs.CreateDocument())
                        {
                            UtilityFunctions.ReleaseHalconMemoryVariables();
                            DataManager.SaveAction("Inspection Complete", "Inspection Completed", REEL_LPN, es.LastUserName, "EndInspectionReel()", REEL_LPN, es.UserReason);
                            CameraManager.NectaCameras[0].nectaCam.Acquire = false;
                            frmI.SetMenuOptions(MenuOptions.IDLE);
                            smea = new SystemMessageEventArgs("Inspection Completed successfully", "Inspection", (int)Enums.CriticalLevels.Black);
                            uscMD.SystemMessage(smea);
                        }
                        else
                        {
                            smea = new SystemMessageEventArgs("Inspection report creation error.", "Inspection Report", (int)Enums.CriticalLevels.Black);
                            uscMD.SystemMessage(smea);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                smea = new SystemMessageEventArgs("EndInspectionReel() err: " + ex.Message, "Inspection", (int)Enums.CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                frmI.EndInspectionError(ex.Message);
            }
        }

        private void doReviewLUI()
        {
            Thread t = new Thread(() => reviewLabelUnderInvestigation());
            t.IsBackground = true;
            t.Start();
        }

        private void reviewLabelUnderInvestigation()
        {
            FailRecord fpData = null;
            string IMAGE_FOLDER = ImageData.CreateImageFilepath();
            string layoutFile = Path.Combine(IMAGE_FOLDER, LABEL_ITEM + "_legendkey.png");
            try
            {
                if (SYSTEM_IO.PROCESSING)
                    if (cmdEndInspection.InvokeRequired)
                    {
                        cmdEndInspection.Invoke((System.Windows.Forms.MethodInvoker)delegate
                        {
                            cmdEndInspection.Enabled = true;
                        });
                    }
                    else
                        cmdEndInspection.Enabled = true;
                if (SYSTEM_IO.PROCESSING)
                {
                    foreach (FailRecord fr in FailRecord.FailPipes)
                    {
                        if (fr.ACTIONED == false && fr.MISSING == false)
                        {
                            fpData = fr;
                            break;
                        }
                    }
                    if (fpData != null)
                    {
                        frmInspect.frmUI = new frmUnderInvestigation(layoutFile, this.LABEL_ITEM, VariationRegion);
                        frmInspect.frmUI.TopLevel = true;
                        frmInspect.frmUI.TopMost = true;
                        frmInspect.frmUI.Refresh();
                        frmInspect.frmUI.Init(fpData, LABEL_ITEM, FAIL_FOLDER);
                        frmInspect.frmUI.Visible = false;
                        frmInspect.frmUI.ShowDialog();
                        Labelcounts.CountQueried += 1;
                        if (fpData.SAMPLE == false)
                        {
                            if (frmInspect.frmUI.AcceptedByOperator)
                                Labelcounts.CountAcceptedOp += 1;
                            else
                                Labelcounts.CountRejectedOp += 1;
                        }
                        else
                            Labelcounts.CountAcceptedOp += 1;
                        frmInspect.frmUI.Close();
                        frmInspect.frmUI = null;
                        
                        fpData.ACTIONED = true;
                        if (SYSTEM_IO.PROCESSING)
                        {
                            if (cmdEndInspection.InvokeRequired)
                            {
                                cmdEndInspection.Invoke((System.Windows.Forms.MethodInvoker)delegate
                                {
                                    cmdEndInspection.Enabled = true;
                                });
                            }
                            else
                                cmdEndInspection.Enabled = true;
                        }
                        mxClient.WriteToRegister(1, "Rewind_After_INV", 1, 3);                        
                    }

                    else if (Positioner.ImageCounter > LABEL_COUNT)
                    {
                        frmInspect.frmUI = new frmUnderInvestigation(layoutFile, this.LABEL_ITEM, VariationRegion);
                        frmInspect.frmUI.TopLevel = true;
                        frmInspect.frmUI.TopMost = true;
                        frmInspect.frmUI.Refresh();
                        frmInspect.frmUI.Visible = false;
                        frmInspect.frmUI.ShowDialog();
                        frmInspect.frmUI.Close();
                        frmInspect.frmUI = null;

                        if (SYSTEM_IO.PROCESSING)
                        {
                            if (cmdEndInspection.InvokeRequired)
                            {
                                cmdEndInspection.Invoke((System.Windows.Forms.MethodInvoker)delegate
                                {
                                    cmdEndInspection.Enabled = true;
                                });
                            }
                            else
                                cmdEndInspection.Enabled = true;
                            mxClient.WriteToRegister(1, "Capture_Image", 1, 3);                            
                        }
                    }
                }
                REVIEWING = false;

                reviewSampleLUIDone = true;

                if (MissingLabelTrigger)
                {
                    MissingLabelTriggered();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                string err = "reviewLabelUnderInvestigation() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection", (int)CriticalLevels.Red);
                uscMD.SystemMessage(smea);
                REVIEWING = false;
                EMH?.Invoke(err);
            }
            finally
            {
                if (fpData != null)
                    fpData.ClearData();
                REVIEWING = false;
            }
        }

        private void MissingLabelTriggered()
        {
            try { mxClient.WriteToRegister(1, "Missing_Label_Inhibit", 0, 3); } catch { }
            MissingLabelTrigger = false;
        }

        public void MoveNextCaller(Action movenextcaller)
        {
            string err = "";
            try
            {
                moveNextCaller = movenextcaller;
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                err = "MoveNextCaller() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Start", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                EMH?.Invoke(err);
            }
        }

        public void SetPauseCaller(Action pausemethodcaller)
        {
            string err = "";
            try
            {
                pauseMethodCaller = pausemethodcaller;
            }
            catch (Exception ex)
            {
                err = "SetPauseCaller() err: " + ex.Message;
                SYSTEM_IO.PROCESSING = false;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Start", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                EMH?.Invoke(err);
            }
        }

        public void SetAlarmCaller(Action alarmmethodcaller)
        {
            string err = "";
            try
            {
                alarmMethodCaller = alarmmethodcaller;
            }
            catch (Exception ex)
            {
                SYSTEM_IO.PROCESSING = false;
                err = "SetAlarmCaller() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Start", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                EMH?.Invoke(err);
            }
        }

        public void ClearFails()
        {
            string err = "";
            try
            {
                if (FailRecord.FailPipes.Count > 0)
                    FailRecord.FailPipes.Clear();
            }
            catch (Exception ex)
            {
                err = "ClearFails() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Start", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                EMH?.Invoke(err);
            }
        }

        public void ClearResultData(string reel_lpn, string lin)
        {
            string err = "";
            try
            {
                DataManager.ClearResultData(reel_lpn, lin);
            }
            catch (Exception ex)
            {
                err = "ClearFails() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Inspection Start", (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                EMH?.Invoke(err);
            }
        }


        private void displayGoodInspection(int labelindex)
        {
            HObject halo = null;
            try
            {
                if (hWin.InvokeRequired)
                {
                    hWin.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        if (labelindex == LABEL_COUNT + 1)
                            dispInfoText("SAMPLE LABEL - OK");
                        else
                            dispInfoText(string.Format("label {0} verified ok", labelindex));
                        Bitmap bmp = null;
                        bmp = LVS3.InspectionStatus.GetImage(true);
                        pbPassFail.Image = bmp;
                    });
                }
                else
                {
                    if (labelindex == LABEL_COUNT + 1)
                        dispInfoText("SAMPLE LABEL - OK");
                    else
                        dispInfoText(string.Format("label {0} verified ok", labelindex));
                    Bitmap bmp = null;
                    bmp = LVS3.InspectionStatus.GetImage(true);
                    pbPassFail.Image = bmp;
                }
            }
            catch { }
            finally
            {
                if (halo != null)
                    halo.Dispose();
            }
        }

        private void displayImageCapture()
        {
            HObject halo = null;
            try
            {
                if (hWin.InvokeRequired)
                {
                    hWin.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        dispInfoText("Capturing and saving images - no inspection. #  images captured: " + Positioner.ImageCounter.ToString());
                        Bitmap bmp = null;
                        bmp = LVS3.InspectionStatus.GetImage(true);
                        pbPassFail.Image = bmp;
                    });
                }
                else
                {
                    dispInfoText("Capturing and saving images - no inspection. #  images captured: " + Positioner.ImageCounter.ToString());
                    Bitmap bmp = null;
                    bmp = LVS3.InspectionStatus.GetImage(true);
                    pbPassFail.Image = bmp;
                }
            }
            catch { }
            finally
            {
                if (halo != null)
                    halo.Dispose();
            }
        }

        private void displayClearInspection()
        {
            if (lblInfoText.InvokeRequired)
            {
                lblInfoText.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    pbPassFail.Image = null;
                    hWin.HalconWindow.ClearWindow();
                    dispInfoText("");
                    dispTimeText("");
                    hWin.Invalidate();
                });
            }
            else
            {
                pbPassFail.Image = null;
                hWin.HalconWindow.ClearWindow();
                dispInfoText("");
                hWin.Invalidate();
            }
        }

        private void dispInfoText(string msg)
        {
            if (lblInfoText.InvokeRequired)
            {
                lblInfoText.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    lblInfoText.Text = msg;
                    lblInfoText.Invalidate();
                });
            }
            else
            {
                lblInfoText.Text = msg;
                lblInfoText.Invalidate();
            }
        }

        private void dispTimeText(string msg)
        {
            try
            {
                if (lblTime.InvokeRequired)
                {
                    lblTime.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        lblTime.Text = msg;
                        lblTime.Invalidate();
                    });
                }
                else
                {
                    lblTime.Text = msg;
                    lblTime.Invalidate();
                }
            }
            catch { }
        }
    }
}