using CONSTANTS;
using System.Drawing;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static LVS3.CameraManager;
using static LVS3.Delegates;
using static LVS3.Enums;
using ILogger = Serilog.ILogger;
// ReSharper disable UnusedMember.Local
namespace LVS3
{
    class HalconImageConverter
    {
        /// <summary>
        /// Converts a HALCON Bitmap (image) to a System.Drawing.Bitmap.
        /// Supports both grayscale and RGB images.
        /// </summary>
        public static Bitmap HObjectToBitmap( Bitmap hObj )
        {
            if (hObj == null || !hObj.IsInitialized())
                throw new ArgumentException("Invalid Bitmap image.");

            // Ensure we have an Bitmap
            Bitmap hImage = new Bitmap(hObj);

            // Get image type
            string type = hImage.GetImageType().ToString().ToLower();

            if (type == "\"byte\"" || type == "\"uint2\"" || type == "\"int2\"") // Grayscale
            {
                object ptr;
                object width, height, imgType;

                ptr = hImage.GetImagePointer1(out imgType, out width, out height);

                // Create bitmap from raw grayscale data
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

                // Set grayscale palette
                ColorPalette palette = bmp.Palette;
                for (int i = 0 ; i < 256 ; i++)
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                bmp.Palette = palette;

                // Copy data into bitmap
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height),
                                                  ImageLockMode.WriteOnly, bmp.PixelFormat);
                int bytes = height * bmpData.Stride;
                byte[] raw = new byte[bytes];

                // Copy row by row (account for stride padding)
                for (int y = 0 ; y < height ; y++)
                    Marshal.Copy(ptr + y * width, raw, y * bmpData.Stride, width);

                Marshal.Copy(raw, 0, bmpData.Scan0, bytes);
                bmp.UnlockBits(bmpData);

                return bmp;
            }
            else if (type == "rgb" || type == "byte" && hImage.CountChannels() == 3) // RGB
            {
                object rPtr, gPtr, bPtr;
                object width, height;
                object tType;

                hImage.GetImagePointer3(out rPtr, out gPtr, out bPtr,
                                        out tType,
                                        out width, out height);

                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height),
                                                  ImageLockMode.WriteOnly, bmp.PixelFormat);

                byte[] raw = new byte[height * bmpData.Stride];

                for (int y = 0 ; y < height ; y++)
                {
                    for (int x = 0 ; x < width ; x++)
                    {
                        int idx = y * bmpData.Stride + x * 3;
                        raw[idx + 2] = Marshal.ReadByte(rPtr, y * width + x); // R
                        raw[idx + 1] = Marshal.ReadByte(gPtr, y * width + x); // G
                        raw[idx] = Marshal.ReadByte(bPtr, y * width + x); // B
                    }
                }

                Marshal.Copy(raw, 0, bmpData.Scan0, raw.Length);
                bmp.UnlockBits(bmpData);

                return bmp;
            }
            else
            {
                throw new NotSupportedException($"Unsupported image type: {type}");
            }
        }
    }


        public static class Positioner
    {
        public static bool CanMoveNext = true;
        public static int ImageCounter = 1;

        public static void MoveNext( Inspection i )
        {
            if (CanMoveNext)
            {
                i.MoveToNextVde();
                ImageCounter++;
            }
        }

        public static void MoveLast( Inspection i )
        {
            i.MoveLast();
            //ImageCounter++;            
        }
    }

    public static class SpeedControl
    {
        private static int _speedUpdateCounter;
        private static int _speedControlUpdateInterval = Defaults.SpeedControlUpdateInterval;
        private static long _durationAccumulater;

        public static void UpdateSpeedControl( long duration )
        {
            if (duration > 0)
                _speedUpdateCounter += 1;
            _durationAccumulater += duration;
            if (_speedUpdateCounter % _speedControlUpdateInterval == 0)
            {
                _durationAccumulater /= _speedControlUpdateInterval;
                var durationAverage = _durationAccumulater;
                _speedUpdateCounter = 0;
                _durationAccumulater = 0;
                if (durationAverage > 0)
                {
                    var t = new Thread(() => WriteSpeedToPlc(durationAverage));
                    t.Start();
                }
            }
        }

        private static void WriteSpeedToPlc( long duration )
        {
            var intDuration = Convert.ToInt32(duration);
            _ctx?.MxClient?.WriteToRegister(1, "Speed_Control", intDuration, 3);
        }
    }

    public class Inspection // : IDisposable
        : IInspection
    {
        protected InspectionContext _ctx = null!;
        private InspectionParams _iParams;
        private LabelType _labelType;
        private int imageNum;
        protected string _sampleLabel = "";
        public bool SampleIncluded { get; set; }
        private Bitmap bmpRes;
        public static LabelCounts Labelcounts;
        private double _innerRadius;
        private bool _inspectLightAreas;
        private List<FontSizesSegment> _fontsizesSegment = [];
        public bool EnableCapture { get; set; }
        public static event ErrorMethodHandler Emh;
        public event AlarmMethodHandler Amh;
        public bool FixedData { get; set; }
        public Action moveNextCaller;
        public string TrgFolder = "";
        public string FontFolder = "";
        public string FailFolder { get; set; } = "";

        public int CountMeds;
        public string LabelItem = "";
        public string ReelLpn = "";
        public string Lwo = "";
        public SystemMessageHandler Sm;
        protected bool _inspecting;
        protected bool _reviewing;
        protected List<MedData> _medDataItems = [];
        protected Bitmap _currentRawImage;
        protected Bitmap _currentReducedImage;
        public int LabelCount { get; set; }
        private Action _pauseMethodCaller;
        private List<VDEItem> _vdeItems = [];
        private List<OPZoneData> _opZoneItems = [];
        protected CameraNecta _cameraNecta;
        protected Bitmap _variationRegion;
        private Bitmap _variationImg = null;
        private int[] _variationRegionCoords = [0, 0, 0, 0];
        public string VariableMedDataPh = "";
        private bool _multiKitsPerPatient;
        public static bool MedFailOnCurrentLabel;
        public static bool MedFailInCurrentPatient;

        private int _labelStopIndex = 0;
        private int _stopAtIndex = 0;
        protected bool _inhibitNextCapture;
        public static bool MissingLabelTrigger;
        public string ReviewLuiMode = "Auto";
        public static int ReviewLuiModeIndex;
        protected bool _edgesNotDetectedError;
        private readonly IDataManager DataManager;
        private readonly IImageMatcher ImageMatcher;
        public readonly IUtilityFunctions UtilityFunctions;
        public IPeripherals Peripherals;
        protected readonly double _confidenceLevel;
        protected readonly ILogger _logger = Log.ForContext<Inspection>();

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static extern void CopyMemory( IntPtr dest, IntPtr src, uint count );

        public Inspection( IDataManager dataManager, IUtilityFunctions utilityFunctions, IPeripherals peripherals, IImageMatcher imageMatcher )
        {
            DataManager = dataManager;
            UtilityFunctions = utilityFunctions;
            Peripherals = peripherals;
            ImageMatcher = imageMatcher;
            _confidenceLevel = DataManager.GetConfidenceLevel();
            string labelDefPath = @"C:\1404 labels\LabelDefinition.xml";
            string labelDefXml = File.ReadAllText(labelDefPath);
            imageNum = 1;

            ImageMatcher.SetLabelDefinition(labelDefXml);


        }

        public bool LoadInspectionDataFromDb( string lpn, string labelitem )
        {
            _logger.Information("Entering LoadInspectionDataFromDb(lpn: {lpn}, labelitem: {labelitem})",
                lpn,
                labelitem);
            bool retVal;
            try
            {
                _labelType = DataManager.GetLabelType(Defaults.StationID,
                    labelitem);
                //MSERLabel = DataManager.GetMSERParams(labelType);                
                ReelLpn = lpn;

                LabelItem = labelitem;
                _innerRadius = DataManager.GetInnerRadius(Defaults.StationID,
                    LabelItem);
                _labelType = DataManager.GetLabelType(Defaults.StationID,
                    LabelItem);
                FontFolder = ImageData.CreateFontFilepathAllMachines();
                TrgFolder = ImageData.CreateTrainingFilepath(lpn);
                FailFolder = ImageData.CreateFailFilepath(lpn);
                ImageData.CreateImageFilepath();
                ImageData.DeleteFailData(FailFolder);
                FailFolder = ImageData.CreateFailFilepath(lpn);
                _variationRegionCoords = DataManager.GetVariationRegion(Defaults.StationID,
                    LabelItem);
                _inspectLightAreas = DataManager.GetInspectLightAreas(Defaults.StationID,
                    LabelItem);
                var labelId = DataManager.LabelID(Defaults.StationID,
                    LabelItem);
                _iParams = DataManager.GeInspectionParams(Defaults.StationID,
                    labelId);
                _iParams.LabelID = labelId;
                _iParams.InspectBright = _inspectLightAreas;

                _variationRegion?.Dispose();
                /* TODO: Replace Halcon
                HOperatorSet.GenRectangle1(out _variationRegion,
                    _variationRegionCoords[0],
                    _variationRegionCoords[1],
                    _variationRegionCoords[2],
                    _variationRegionCoords[3]);
                */
                retVal = LoadOpZoneData(labelId);


                var dr = MessageBox.Show(@"Does this roll of labels include a sample label?",
                    @"Label Inspection",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (dr == /*DialogResult.*/ Yes)
                {
                    var sample = DataManager.GetSample(lpn);
                    _sampleLabel = sample;
                    SampleIncluded = true;
                    DataManager.SaveAction("Sample Label on reel: YES",
                        ReelLpn,
                        "Inspection",
                        Defaults.UserName,
                        "SAMPLE: " + sample,
                        "LIN: " + labelitem,
                        "Reel includes sample label");
                }
                else if (dr == /*DialogResult.*/ No)
                {
                    _sampleLabel = "";
                    SampleIncluded = false;
                    DataManager.SaveAction("Sample Label on reel: NO",
                        ReelLpn,
                        "Inspection",
                        Defaults.UserName,
                        "LoadInspectionDataFromDB()",
                        "LIN: " + labelitem,
                        "Reel does not include sample label");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in LoadInspectionDataFromDb: {Message}",
                    ex.Message);
                retVal = false;
                var err = "LoadInspectionDataFromDB() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Initialize Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
            }
            return retVal;
        }

        public bool InitInspection( InspectionContext context )
        {
            _logger.Information("Entering InitInspection");
            var retVal = true;
            try
            {
                _ctx = context;
                Labelcounts = new LabelCounts();
                Labelcounts.InspectLightArea = _inspectLightAreas;
                _fontsizesSegment = DataManager.GetListFontSizes();
                if (_ctx.OnAlarm != null) Amh += _ctx.OnAlarm;
                if (_ctx.OnError != null) Emh += _ctx.OnError;

                _inspecting = false;
                _reviewing = false;
                SYSTEM_IO.PROCESSING = false;
                SYSTEM_IO.IO_INTERRUPT_Handler -= IO_INTERRUPT_Handler;
                SYSTEM_IO.IO_INTERRUPT_Handler += IO_INTERRUPT_Handler;
                SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
                SYSTEM_IO.IO_CHANGE_Handler += IO_COS_Handler;
                _cameraNecta = NectaCameras[0];
                _cameraNecta.nectaCam.Acquire = false;
                _cameraNecta.SetChannelMethodCallerFalse();
                Positioner.ImageCounter = 1;
                Positioner.CanMoveNext = false;
                Lwo = "";
                ReelLpn = _ctx.ReelLpn;
                _ctx.OnInfoTextChanged?.Invoke(ReelLpn);
                CountMeds = 0;
                EnableCapture = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in InitInspection: {Message}",
                    ex.Message);
                retVal = false;
                var err = "InitInspection() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Initialize Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
            }
            return retVal;
        }

        private List<VdeDisplayItem> _vdeDisplayItems = new();
        private List<MedDataDisplayItem> _medDisplayItems = new();

        public List<VdeDisplayItem> GetVdeDisplayItems() => _vdeDisplayItems;
        public List<MedDataDisplayItem> GetMedDisplayItems() => _medDisplayItems;

        public bool LoadVdeToolsAndData( string reelLpn, string lin, string lwo )
        {
            _logger.Information("Entering LoadVdeToolsAndData(reelLpn: {reelLpn}, lin: {lin}, lwo: {lwo})",
                reelLpn,
                lin,
                lwo);

            var retVal = false;
            _vdeDisplayItems.Clear();
            try
            {
                EnableCapture = false;
                LabelItem = lin;
                ReelLpn = reelLpn;
                _medDataItems?.Clear();
                _medDataItems = [];

                var liDs = DataManager.LabelDataItems(lin,
                    reelLpn);

                foreach (var lid in liDs)
                {
                    if (lid.DataPresent == false)
                    {
                        var err = string.Format(
                            "Label information is missing for label item: {0}. Cannot build VDE data item model. loadVDEToolsAndData()",
                            lin);
                        var smea = new SystemMessageEventArgs(err,
                            "Label Inspection",
                            (int)CriticalLevels.Red);
                        _ctx?.OnSystemMessage?.Invoke(smea);
                        return false;
                    }
                    // Build display item for the UI (replaces uscVDEItem UserControl)
                    _vdeDisplayItems.Add(new VdeDisplayItem
                    {
                        Name = lid.VariableName,
                        Placeholder = lid.PlaceHolder,
                        OpZoneName = lid.OpZoneName,
                        IsVDE = lid.IsVDE,
                        IsBarcode2D = lid.IsBarcode2D,
                        IsBarcodeLinear = lid.IsBarcodeLinear
                    });
                    var meddata = DataManager.LabelDataInspection(lwo,
                        reelLpn,
                        lid.VariableName);
                    meddata.PlaceHolder = lid.PlaceHolder;
                    meddata.VarName = lid.VariableName;
                    meddata.Repeat = lid.Repeat;
                    if (meddata.Repeat > 0)
                        meddata.repeatIndex = 1;
                    else
                        meddata.repeatIndex = 0;
                    _medDataItems.Add(meddata);
                }

                VariableMedDataPh = "";
                _multiKitsPerPatient = false;
                foreach (var md in _medDataItems)
                {
                    if (md.Repeat > 0)
                    {
                        VariableMedDataPh = md.PlaceHolder;  // Extract the placeholder for the varying data if it exists
                        if (md.Repeat > 1)
                        {
                            _multiKitsPerPatient = true;
                            break;
                        }
                    }
                }

                retVal = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in LoadVdeToolsAndData: {Message}",
                    ex.Message);
                var err = "loadVDEToolsAndData() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Label Inspection",
                    (int)CriticalLevels.Amber);
                _ctx?.OnSystemMessage?.Invoke(smea);
            }
            return retVal;
        }



        public bool LoadMedDataList( string lin, string lpn, string lwo )
        {
            _logger.Information("Entering LoadMedDataList(lin: {lin}, lpn: {lpn}, lwo: {lwo})",
                lin,
                lpn,
                lwo);
            bool retVal;
            try
            {
                EnableCapture = false;
                LabelItem = lin;
                ReelLpn = lpn;
                _medDataItems?.Clear();
                _medDataItems = [];
                _medDisplayItems.Clear();

                foreach (var vdi in _vdeDisplayItems)
                {
                    var meddata = DataManager.LabelDataInspection(lwo,
                        lpn,
                        vdi.Name);
                    meddata.PlaceHolder = vdi.Placeholder;
                    meddata.VarName = vdi.Name;
                    // meddata.Repeat is set from the display item if available
                    _medDataItems.Add(meddata);
                    _medDisplayItems.Add(new MedDataDisplayItem
                    {
                        Placeholder = vdi.Placeholder,
                        Data = meddata.Data
                    });
                }
                LabelCount = _medDataItems[0].sequence_list.Count;
                retVal = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in LoadMedDataList: {Message}",
                    ex.Message);
                retVal = false;
                var err = "LoadMedDataList() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Initialize Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
            }
            return retVal;
        }

        public bool LoadInspectionVdeItemParams( string lin )
        {
            _logger.Information("Entering LoadInspectionVDEItemParams(lin: {lin})",
                lin);
            bool retVal;
            try
            {
                _fontsizesSegment = DataManager.GetListFontSizes();
                var labelId = DataManager.LabelID(Defaults.StationID,
                    lin);
                _labelType = DataManager.GetLabelType(Defaults.StationID,
                    lin);
                _vdeItems = DataManager.InspectionParamsVDEItem(labelId,
                    _fontsizesSegment,
                    _labelType);
                retVal = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in LoadInspectionVdeItemParams: {Message}",
                    ex.Message);
                retVal = false;
                var err = "LoadInspectionVDEItemParams() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Initialize Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
            }
            return retVal;
        }

        private bool LoadOpZoneData( int id )
        {
            _logger.Information("Entering LoadOpZoneData(id: {id})",
                id);
            CameraNecta.imageIndexToSave = 0;
            bool retVal;
            try
            {
                _opZoneItems?.Clear();
                _opZoneItems = DataManager.OPZoneData(id,
                    TrgFolder,
                    _labelType);
                retVal = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in LoadOpZoneData: {Message}",
                    ex.Message);
                retVal = false;
                var err = "LoadOPZoneData() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Initialize Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
            }
            return retVal;
        }

        #region Inspection

        /// <summary>
        /// Purpose of this function is to trim out the label only from the whole image acquired from 
        /// the camera. This is done using edge tools to detect a contrast change for the edge of the label 
        /// against the background
        /// </summary>
        /// <param name="img">The Camera image comes in and gets modified to the label image</param>
        /// <param name="fp">Record structure</param>
        /// <returns>True if sucessful</returns>
        protected bool ReduceBackground( ref Bitmap img, ref FailRecord fp )
        {
            _logger.Information("Entering ReduceBackground(img, fp)");
            var retVal = true;
            Bitmap tmpObj = null, region = null;
            object edgeThreshold = 30, ampThreshold = 20, startRow = 200, rotate = 270;
            try

            {
                #region HDEVProcedure
                //Detect the size of the image from the camera
                /* TODO: Replace HOperatorSet.GetImageSize */ //img,
                    out var w,
                    out var h);
                //Create a region in order to do the edge detection (Halcon refer to this as am measurement tool)
                //Defined in size automatically by the image dimensions
                /* TODO: Replace Halcon
                HOperatorSet.GenMeasureRectangle2(h / 2,
                    w / 2,
                    new object(90).TupleRad(),
                    (h - 20) / 2,
                    50,
                    w,
                    h,
                    "nearest_neighbor",
                    out var msr1);
                */
                //Find the edge of the label
                //Sigma indicates the sharpness of the edge, the threshold determines the acceptable limit
                //Positive indicates a dark->Light transition and first is the choice of edge found.
                /* TODO: Replace HOperatorSet.MeasurePos */ //img,
                    msr1,
                    1.0,
                    edgeThreshold,
                    "positive",
                    "first",
                    out var labelRow,
                    out _,
                    out _,
                    out _);
                //repeat this to find the top edge of the label as bottom is always ref. edge?
                /* TODO: Replace Halcon
                HOperatorSet.GenMeasureRectangle2(startRow,
                    (w / 2),
                    new object(0).TupleRad(),
                    (w - startRow) / 2,
                    25,
                    w,
                    h,
                    "nearest_neighbor",
                    out var msr2);
                */
                /* TODO: Replace HOperatorSet.MeasurePos */ //img,
                    msr2,
                    1.0,
                    ampThreshold,
                    "positive",
                    "first",
                    out _,
                    out var labelCol,
                    out _,
                    out _);
                #endregion
                //Verify we have x(Col) and y(row) results 
                if (labelCol.Length == 0 && labelRow.Length == 0)
                {
                    fp.READABLE = false;
                    fp.VALID_LABEL = false;
                    var rfd = new RegionFailData("Image Preparation");
                    if (img.IsInitialized())
                    {
                        if (rfd.Img == null)
                            /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                        /* TODO: Replace HOperatorSet.CopyObj */ //img,
                            out rfd.Img,
                            1,
                            1);
                        /* TODO: Replace HOperatorSet.DispObj */ //img,
                            IntPtr.Zero /* TODO: HalconWindow */);
                    }
                    rfd.Reasons.Add("Label edges not identified / unable to determine the variation region");
                    fp.RegionFailDataList.Add(rfd);
                    fp.ACCEPTED = false;
                    fp.VALID_LABEL = false;
                    fp.ACTIONED = false;

                    _edgesNotDetectedError = true;
                    EdgeNotFoundFailDelay();


                    return false;
                }

                //With Valid edges found now the lable can be cropped -out of the larger image
                //Label is always cropped from left edge only
                #region CommonCode - no params required          
                if (labelRow.Length == 0) //we didn't find the row-edge - code is the same in each section of the if!
                    /* TODO: Replace Halcon
                    HOperatorSet.GenRectangle1(out region,
                        0,
                        labelCol,
                        h,
                        w);
                    */
                else
                    /* TODO: Replace Halcon
                    HOperatorSet.GenRectangle1(out region,
                        0,
                        labelCol,
                        h,
                        w);
                    */

                //trim the image smaller
                /* TODO: Replace HOperatorSet.ReduceDomain */ //img,
                    region,
                    out tmpObj);
                /* TODO: Replace HOperatorSet.CropDomain */ //tmpObj,
                    out tmpObj);
                //change orientation for ease of display/processing
                /* TODO: Replace HOperatorSet.RotateImage */ //tmpObj,
                    out tmpObj,
                    rotate,
                    "constant");
                _currentRawImage?.Dispose();
                //Copy image for output
                /* TODO: Replace HOperatorSet.CopyObj */ //tmpObj,
                    out _currentRawImage,
                    1,
                    1);
                img.Dispose();
                /* TODO: Replace HOperatorSet.CopyObj */ //tmpObj,
                    out img,
                    1,
                    1);
                #endregion
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in ReduceBackground: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                fp.READABLE = false;
                fp.ACCEPTED = false;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                var err = "ReduceBackground() err: " + ex.Message + ". Cannot distinguish label and/or web from roller background";
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                tmpObj?.Dispose();
                region?.Dispose();
                _cameraNecta.CameraImage?.Dispose();
            }
            return retVal;
        }

        private async void EdgeNotFoundFailDelay()
        {
            _logger.Information("Entering EdgeNotFoundFailDelay()");
            await Task.Delay(500);
            SYSTEM_IO.FAIL_OCCURED();
        }

        //private (object P, object pA, object pB) SetMSERParams(ref Bitmap img, object mean)
        //{
        //    object paramValuesA, paramValuesB, mserParams;
        //    mserParams = new object();

        //    if (MSERLabel.DarkMinDiversity > -1)
        //    {
        //        ///* TODO: Replace HOperatorSet.Intensity */ //VariationRegion, img, out object mean, out object deviation);
        //        mserParams[0] = "min_diversity";
        //        mserParams[1] = "max_variation";
        //        mserParams[2] = "min_gray";
        //        mserParams[3] = "max_gray";
        //        paramValuesA = new object();
        //        paramValuesA[0] = MSERLabel.DarkMinDiversity;
        //        paramValuesA[1] = MSERLabel.DarkMaxVariation;
        //        paramValuesA[2] = MSERLabel.DarkMinGray;
        //        paramValuesA[3] = mean - (mean * MSERLabel.MeanMultiplier);

        //        paramValuesB = new object();
        //        paramValuesB[0] = MSERLabel.LightMinDiversity;
        //        paramValuesB[1] = MSERLabel.LightMaxVariation;
        //        paramValuesB[2] = mean - 20;
        //        paramValuesB[3] = 255;
        //    }
        //    else
        //    {
        //        ///* TODO: Replace HOperatorSet.Intensity */ //VariationRegion, img, out object mean, out object deviation);
        //        mserParams[0] = "max_variation";
        //        mserParams[1] = "min_gray";
        //        mserParams[2] = "max_gray";
        //        paramValuesA = new object();
        //        paramValuesA[0] = MSERLabel.DarkMaxVariation;
        //        paramValuesA[1] = MSERLabel.DarkMinGray;
        //        paramValuesA[2] = mean - (mean * MSERLabel.MeanMultiplier);

        //        paramValuesB = new object();
        //        paramValuesB[0] = MSERLabel.LightMaxVariation;
        //        paramValuesB[1] = mean - 20;
        //        paramValuesB[2] = 255;
        //    }
        //    return (mserParams, paramValuesA, paramValuesB);
        //}

        private void RecordFailData( Bitmap regiondata, ref Bitmap labelimage, ref FailRecord fp, bool isdark, string zonename )
        {
            _logger.Information(
                "Entering RecordFailData(regiondata, labelimage, fp, isdark: {isdark}, zonename: {zonename})",
                isdark,
                zonename);
            try
            {
                fp.VALID_LABEL = false;
                var rfd = new RegionFailData(zonename);
                if (rfd.Img == null)
                    /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                /* TODO: Replace HOperatorSet.CopyObj */ //labelimage,
                    out rfd.Img,
                    1,
                    1);
                if (isdark)
                    rfd.Reasons.Add("dark debris/marks on label");
                else
                    rfd.Reasons.Add("bright areas/marks on label");
                fp.RegionFailDataList.Add(rfd);
                /* TODO: Replace Halcon
                HOperatorSet.SmallestRectangle1(regiondata,
                    out var r1Sel,
                    out var c1Sel,
                    out var r2Sel,
                    out var c2Sel);
                */
                if (r1Sel.Length == 1)
                    rfd.failCoordsList.Add(new RegionCoordPoints([r1Sel - 2, c1Sel - 2, r2Sel + 2, c2Sel + 2]));
                else
                {
                    if (r1Sel.Length < 51)
                    {
                        for (var x = 0 ; x < r1Sel.Length ; x++)
                            rfd.failCoordsList.Add(new RegionCoordPoints([Convert.ToInt32(r1Sel[x].D), Convert.ToInt32(c1Sel[x].D), Convert.ToInt32(r2Sel[x].D), Convert.ToInt32(c2Sel[x].D)
                            ]));
                    }
                    //What happens if there are >=51 Defects? they don't go in the fail coords list????
                }
                fp.VALID_LABEL = false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in RecordFailData: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                var sMed = "";
                foreach (var med in _medDataItems)
                {
                    if (med.PlaceHolder == VariableMedDataPh)
                    {
                        sMed = "label " + med.Data + ". ";
                        break;
                    }
                }
                var err = "RecordFailData() err: " + sMed + "Region: " + zonename + ": " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }

        private bool DebrisCheck( ref Bitmap maskedimage, ref Bitmap labelimage, bool inspectlightareas, ref FailRecord fp, string opzonename )
        {
            _logger.Information(
                "Entering DebrisCheck(maskedimage, labelimage, inspectlightareas: {inspectlightareas}, fp, opzonename: {opzonename})",
                inspectlightareas,
                opzonename);

            #region HDEV_Proc
            var retVal = true;
            Bitmap imgSobel = null, rgnThreshold = null, rgnSelectedDark = null, rgnSelectedLight = null, rgnUnion = null, rgnDilation = null;
            Bitmap rgnFillup = null, imgReduced = null, rgnDarkObjects = null, rgnLightObjects = null, rgnConnected = null, rgnSelected = null, rgnEroded = null;
            object numLightObjects = 0;
            try
            {
                //Used to estimate the threshold on the image, just the mean intesity of the pixels in this region is used
                /* TODO: Replace HOperatorSet.Intensity */ //_variationRegion,
                    maskedimage,
                    out var mean,
                    out _);

                //Find Marks
                //Sobel filter basically finds edges by detecting contrast / change in brightness only these areas with sufficent contrast change will be considered debris
                /* TODO: Replace HOperatorSet.SobelAmp */ //maskedimage,
                    out imgSobel,
                    "sum_abs",
                    _iParams.SobelAmpSize);
                /* TODO: Replace HOperatorSet.Threshold */ //imgSobel,
                    out rgnThreshold,
                    _iParams.SobelEdgeThreshold,
                    255);
                //Splits all non-touching regions into individual regions
                /* TODO: Replace HOperatorSet.Connection */ //rgnThreshold,
                    out rgnConnected); //this line is currently redundant

                //Elininate Small clutter pixels groups
                ///* TODO: Replace HOperatorSet.SelectShape */ //rgnConnected, out rgnSelected, "area", "and", 10, 999999);
                //Join all disconnected areas into 1 region
                /* TODO: Replace Halcon
                HOperatorSet.Union1(rgnConnected,
                    out rgnUnion);//this line is currently redundant
                */

                //Merge fragments by growing the region to join stuff together (Dilation) then  fill in any gaps (FillUp) then shrink back again to re-separate (Erosion)
                /* TODO: Replace HOperatorSet.DilationCircle */ //rgnUnion,
                    out rgnDilation,
                    3.5);
                /* TODO: Replace HOperatorSet.FillUp */ //rgnDilation,
                    out rgnFillup);
                /* TODO: Replace HOperatorSet.ErosionCircle */ //rgnFillup,
                    out rgnEroded,
                    2.5);

                //We are only workimng in the areas considered to be faults
                /* TODO: Replace HOperatorSet.ReduceDomain */ //maskedimage,
                    rgnEroded,
                    out imgReduced);
                // This will only find DARK debris within the areas prepared as potential debris
                /* TODO: Replace HOperatorSet.Threshold */ //imgReduced,
                    out rgnDarkObjects,
                    0,
                    mean * _iParams.MeanOffset);

                //Just clears out variables before reuse

                //Split areas up
                /* TODO: Replace HOperatorSet.Connection */ //rgnDarkObjects,
                    out rgnConnected);
                //Select/sotr based on attributes
                //file:///C:/Program%20Files/MVTec/HALCON-22.11-Steady/doc/html/reference/operators/toc_regions_features.html
                //anything smaller than 2.5px radius of inner circel willbe discarded
                /* TODO: Replace HOperatorSet.SelectShape */ //rgnConnected,
                    out rgnConnected,
                    "inner_radius",
                    "and",
                    2.5,
                    _iParams.DebrisMaxSize);
                //Areas < DebrisMinSize will be discarded
                /* TODO: Replace HOperatorSet.SelectShape */ //rgnConnected,
                    out rgnSelectedDark,
                    "area",
                    "and",
                    _iParams.DebrisMinSize,
                    _iParams.DebrisMaxSize);
                //Counts how many defects
                /* TODO: Replace HOperatorSet.CountObj */ //rgnSelectedDark,
                    out var numDarkObjects);

                //select bright and DO NOT merge with dark !
                //Roughly a repeat of above but for the defects that are brighter than the mean rather than darker
                if (inspectlightareas)
                {
                    imgReduced?.Dispose();
                    rgnEroded?.Dispose();
                    /* TODO: Replace HOperatorSet.ErosionCircle */ //rgnFillup,
                        out rgnEroded,
                        3.5);
                    /* TODO: Replace HOperatorSet.ReduceDomain */ //maskedimage,
                        rgnEroded,
                        out imgReduced);
                    /* TODO: Replace HOperatorSet.Threshold */ //imgReduced,
                        out rgnLightObjects,
                        mean * 0.9,
                        255); //note there may be a gap between dark and light thresholds if _iParams.MeanOffset != 0.9
                    rgnConnected?.Dispose();
                    /* TODO: Replace HOperatorSet.Connection */ //rgnLightObjects,
                        out rgnConnected);
                    //No check for inner radius?? So different criteria for light defects
                    /* TODO: Replace HOperatorSet.SelectShape */ //rgnConnected,
                        out rgnSelectedLight,
                        "area",
                        "and",
                        _iParams.DebrisMinSize,
                        _iParams.DebrisMaxSize);
                    /* TODO: Replace HOperatorSet.CountObj */ //rgnSelectedLight,
                        out numLightObjects);
                }
                #endregion
                //Decision making
                if (numDarkObjects > 0)
                {
                    retVal = false;
                    RecordFailData(rgnSelectedDark,
                        ref labelimage,
                        ref fp,
                        true,
                        opzonename);
                }
                if (numLightObjects > 0)
                {
                    retVal = false;
                    RecordFailData(rgnSelectedLight,
                        ref labelimage,
                        ref fp,
                        false,
                        opzonename);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in DebrisCheck: {Message}",
                    ex.Message);
                retVal = false;
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                var sMed = "";
                foreach (var med in _medDataItems)
                {
                    if (med.PlaceHolder == VariableMedDataPh)
                    {
                        sMed = "label " + med.Data + ". ";
                        break;
                    }
                }
                var err = "DebrisCheck() err: " + sMed + "Region: " + opzonename + ": " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                rgnEroded?.Dispose();
                imgSobel?.Dispose();
                rgnThreshold?.Dispose();
                rgnSelectedDark?.Dispose();
                rgnSelectedLight?.Dispose();
                rgnUnion?.Dispose();
                rgnDilation?.Dispose();
                rgnFillup?.Dispose();
                imgReduced?.Dispose();
                rgnDarkObjects?.Dispose();
                rgnLightObjects?.Dispose();
                rgnConnected?.Dispose();
                rgnSelected?.Dispose();
            }
            return retVal;
        }

        /// <summary>
        /// This finds dark areas bigger than 800 px in area and masks them and the surrounding 4px from the image
        /// if they only have a width less than 5% of the height, i.e. lines
        /// </summary>
        /// <param name="labelimage"></param>
        /// <param name="threshold"></param>
        private void EliminateLineStretch( ref Bitmap labelimage, double threshold )
        {
            _logger.Information("Entering EliminateLineStretch(labelimage, threshold: {threshold})",
                threshold);
            Bitmap selectedArea = null, rgnAreas = null, rgnComplement = null, tmpLine = null;
            var maxGray = Convert.ToInt32(threshold);
            try
            {
                if (labelimage != null)
                {
                    if (labelimage.IsInitialized())
                    {
                        /* TODO: Replace HOperatorSet.Threshold */ //labelimage,
                            out rgnAreas,
                            0,
                            maxGray);
                        /* TODO: Replace HOperatorSet.Connection */ //rgnAreas,
                            out rgnAreas);
                        /* TODO: Replace HOperatorSet.SelectShape */ //rgnAreas,
                            out rgnAreas,
                            "area",
                            "and",
                            800,
                            50000);
                        /* TODO: Replace HOperatorSet.Connection */ //rgnAreas,
                            out rgnAreas);
                        /* TODO: Replace HOperatorSet.AreaCenter */ //rgnAreas,
                            out _,
                            out _,
                            out _);
                        /* TODO: Replace HOperatorSet.CountObj */ //rgnAreas,
                            out var selectedAreaCount);
                        for (var y = 1 ; y <= selectedAreaCount ; y++)
                        {
                            selectedArea?.Dispose();
                            /* TODO: Replace HOperatorSet.GenEmptyObj */ //out selectedArea);
                            /* TODO: Replace HOperatorSet.SelectObj */ //rgnAreas,
                                out selectedArea,
                                y);
                            /* TODO: Replace HOperatorSet.HeightWidthRatio */ //selectedArea,
                                out _,
                                out _,
                                out var ratio);
                            if (ratio.Length > 0)
                            {
                                if (ratio.D <= 0.05)
                                {
                                    /* TODO: Replace Halcon
                                    HOperatorSet.SmallestRectangle1(selectedArea,
                                        out var r1,
                                        out var c1,
                                        out var r2,
                                        out var c2);
                                    */
                                    /* TODO: Replace Halcon
                                    HOperatorSet.GenRectangle1(out tmpLine,
                                        r1 - 4,
                                        c1 - 4,
                                        r2 + 4,
                                        c2 + 4);
                                    */
                                    /* TODO: Replace HOperatorSet.Complement */ //tmpLine,
                                        out rgnComplement);
                                    /* TODO: Replace HOperatorSet.ReduceDomain */ //labelimage,
                                        rgnComplement,
                                        out labelimage);
                                    tmpLine?.Dispose();
                                    rgnComplement?.Dispose();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in EliminateLineStretch: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                var err = "eliminateLineStretch() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                selectedArea?.Dispose();
                rgnAreas?.Dispose();
                rgnComplement?.Dispose();
                tmpLine?.Dispose();
            }
        }

        protected void InspectOpZone( ref Bitmap img, ref FailRecord fp )
        {
            _logger.Information("Entering InspectOpZone(img, fp)");
            var bVdeFoundFalse = false;
            Bitmap imgTransform = null, opzRoi = null;
            var ozdName = "";
            try
            {
                _variationRegion?.Dispose();
                //The image is prepared by making it 8px bigger than the variation region - not sure why at the moment. may just be to allow edge effects or maybe to allow a small amount of rotation/translation?
                /* TODO: Replace Halcon
                HOperatorSet.GenRectangle1(out _variationRegion,
                    _variationRegionCoords[0] - 4,
                    _variationRegionCoords[1] - 4,
                    _variationRegionCoords[2] + 4,
                    _variationRegionCoords[3] + 4);
                */
                /* TODO: Replace HOperatorSet.ReduceDomain */ //img,
                    _variationRegion,
                    out img);
                bool bVdeFound;
                foreach (var ozd in _opZoneItems.OrderBy(x => x.NAME))
                {
                    if (SYSTEM_IO.PROCESSING == false) return;
                    ozdName = ozd.NAME;
                    imgTransform?.Dispose();
                    opzRoi?.Dispose();
                    double score = 0;
                    //Locate the opzone shape model
                    //Will cope with some amount of rotation from the defaults.radxxx and some amount of Scale in the column direction of the image but none in the row direction
                    /* TODO: Replace HOperatorSet.FindAnisoShapeModel */ //img,
                        ozd.FIXTURE_ID,
                        Defaults.radMinus1Point5,
                        Defaults.rad2,
                        1,
                        1,
                        0.98,
                        1.02,
                        0.5,
                        1,
                        0.5,
                        "least_squares",
                        Defaults.NumLevelsFind,
                        Defaults.Greediness,
                        out var row,
                        out var column,
                        out var angle,
                        out var scaleR,
                        out var scaleC,
                        out var opZoneScore);
                    if (opZoneScore.Length > 0)
                        score = opZoneScore.D;
                    else
                    {
                        //I think the intention here is to find any match - min score = 0 so there is an indication of how much below the Score threshold it has been found at
                        //However there are different scal settings 0.9-1 for R and C compared to first attmept above? Maybe not deliberate just main call edited and values not copied below?
                        /* TODO: Replace HOperatorSet.FindAnisoShapeModel */ //img,
                            ozd.FIXTURE_ID,
                            Defaults.radMinus1Point5,
                            Defaults.rad2,
                            0.9,
                            1,
                            0.9,
                            1.0,
                            0.0,
                            1,
                            0.5,
                            "least_squares",
                            0,
                            0.9,
                            out row,
                            out column,
                            out angle,
                            out scaleR,
                            out scaleC,
                            out opZoneScore);
                        if (opZoneScore.Length > 0)
                            score = opZoneScore.D;
                    }
                    if (score >= Defaults.FixtureOPZoneScoreMin)
                    {
                        //This now rectifies the opzone based on the shape model's resulting x/y/angle
                        var opZoneRow = ozd.FIXTURE_Y - row;
                        var opZoneCol = ozd.FIXTURE_X - column;
                        //Create the transformation matrix to describe the move
                        /* TODO: Replace Halcon
                        HOperatorSet.HomMat2dIdentity(out var homMat2D);
                        */
                        //The image will be stretches/shrunk based on the shape model...not sure if this will be helpful for the variation model
                        /* TODO: Replace Halcon
                        HOperatorSet.HomMat2dScale(homMat2D,
                            1 / scaleR,
                            1 / scaleC,
                            row,
                            column,
                            out homMat2D);
                        */
                        //Rotate by -angle to make the image 'square' again
                        /* TODO: Replace Halcon
                        HOperatorSet.HomMat2dRotate(homMat2D,
                            -angle,
                            row,
                            column,
                            out homMat2D);
                        */
                        /* TODO: Replace Halcon
                        HOperatorSet.HomMat2dTranslate(homMat2D,
                            opZoneRow,
                            opZoneCol,
                            out homMat2D);
                        */
                        //Moves the image back into a 'square' position
                        /* TODO: Replace HOperatorSet.AffineTransImage */ //img,
                            out imgTransform,
                            homMat2D,
                            "weighted",
                            "false");
                        /* TODO: Replace HOperatorSet.ReduceDomain */ //img,
                            _variationRegion,
                            out img); //Not sure this line is doing anything useful
                        ozd.HomMat2D = homMat2D;
                        /* TODO: Replace Halcon
                        HOperatorSet.GenRectangle1(out opzRoi,
                            ozd.OPZONE_TOP,
                            ozd.OPZONE_LEFT,
                            ozd.OPZONE_BOTTOM,
                            ozd.OPZONE_RIGHT);
                        */
                        /* TODO: Replace HOperatorSet.CopyObj */ //opzRoi,
                            out ozd.region,
                            1,
                            1);

                        //Inspection is now done on the rotated/scaled/rectified image - this should put the image in the exact same position as the variation model
                        //This is where the OCR/text reading is done
                        bVdeFound = InspectVde(ref imgTransform,
                            ref fp,
                            ozd);

                        if (SYSTEM_IO.PROCESSING == false) return;
                        //Think this masks the text from the rest of the opzone
                        _ = MaskVdeByOpZone(ref imgTransform,
                            ozd.NAME);
                        //Makes the masked areas 10px bigger and removes from the image checking edges of image 
                        InspectAndMaskMasks(ref imgTransform,
                            ref fp);
                        //Find print defects using variation model comparison
                        CompareZoneImageToModel(ozd,
                            imgTransform,
                            ref fp,
                            imgTransform);
                        //creates a rotated rectangle that should outline the opzone as per the corrected orientation from the shape match, assume for display purposes and not sure if used.
                        /* TODO: Replace Halcon
                        HOperatorSet.GenRectangle2(out ozd.AffineRegion,
                            row,
                            column,
                            angle,
                            ((ozd.OPZONE_RIGHT - ozd.OPZONE_LEFT) / 2),
                            (ozd.OPZONE_BOTTOM - ozd.OPZONE_TOP) / 2);
                        */

                        if (bVdeFoundFalse == false)
                        {
                            if (bVdeFound == false)
                            {
                                bVdeFoundFalse = true;
                            }
                        }
                    }
                    else
                    {
                        #region fail

                        var result = fp.MED_ID;
                        fp.VALID_LABEL = false;
                        var rfd = new RegionFailData(ozd.NAME);
                        if (rfd.Img == null)
                            /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                        /* TODO: Replace HOperatorSet.CopyObj */ //img,
                            out rfd.Img,
                            1,
                            1);
                        rfd.Reasons.Add(string.Format(
                            "Label " + result + ". Fixture score {0} is too low in Op-Zone {1}",
                            score,
                            ozd.NAME));
                        rfd.failCoordsList.Add(new RegionCoordPoints([ozd.OPZONE_TOP, ozd.OPZONE_LEFT, ozd.OPZONE_BOTTOM, ozd.OPZONE_RIGHT
                        ]));
                        fp.RegionFailDataList.Add(rfd);
                        #endregion
                    }
                    if (SYSTEM_IO.PROCESSING == false) return;
                }

                if (bVdeFoundFalse)
                {
                    //FailRecord fpOut = null;
                    var posCheck = StatusCheck(ref fp);
                    FailRecord fpOut;

                    if (posCheck != PositionCheck.IS_NEXT)
                    {
                        bVdeFound = SetFailPipeData(ref fp,
                            posCheck,
                            out fpOut,
                            ozdName);
                        if (bVdeFound)
                        {
                            fp = fpOut;
                        }
                    }
                    else
                    {
                        bVdeFound = SetFailPipeData(ref fp,
                            posCheck,
                            out fpOut,
                            ozdName);

                        if (bVdeFound)
                        {
                            fp = fpOut;
                        }

                        foreach (var md in _medDataItems)
                        {
                            fp.Datas[fp.PlaceHolders.IndexOf(md.PlaceHolder)] = md.Data;
                            if (md.PlaceHolder == VariableMedDataPh)
                            {
                                fp.MED_ID = md.Data;
                            }
                        }

                        posCheck = StatusCheck(ref fp);
                        bVdeFound = SetFailPipeData(ref fp,
                            posCheck,
                            out fpOut,
                            ozdName);
                        if (bVdeFound)
                        {
                            fp = fpOut;
                        }
                    }
                }

                if (_multiKitsPerPatient == false)
                {
                    MedFailOnCurrentLabel = false;
                }
                else
                {
                    foreach (var md in _medDataItems)
                    {
                        if (md.PlaceHolder == VariableMedDataPh)
                        {
                            if (md.repeatIndex >= md.Repeat)
                            {
                                MedFailInCurrentPatient = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in InspectOpZone: {Message}",
                    ex.Message);
                _pauseMethodCaller?.Invoke();
                if (SYSTEM_IO.PROCESSING == false) return;
                SYSTEM_IO.PROCESSING = false;
                var sMed = "";
                foreach (var med in _medDataItems)
                {
                    if (med.PlaceHolder == VariableMedDataPh)
                    {
                        sMed = "label " + med.Data + ". ";
                        break;
                    }
                }
                var err = "InspectOpZone() err: " + sMed + "Region: " + ozdName + ": " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                imgTransform?.Dispose();
                opzRoi?.Dispose();
            }
        }

        private void CompareZoneImageToModel( OPZoneData ozd, Bitmap tmpobj, ref FailRecord fp, Bitmap rawimage )
        {
            _logger.Information("Entering CompareZoneImageToModel(ozd: {ozd}, tmpobj, fp, rawimage)",
                ozd);
            Bitmap regions = null, selectedRegions = null;
            Bitmap connectedRegions = null;
            object numDiffs = 0;
            var features = new object("inner_radius");
            object radiusSmall = _innerRadius;
            var radiusLarge = new object(500.1);
            var varRegions = false;
            try
            {
                //Unused - HOperatorSet.SmallestRectangle1(ozd.region, out var r1Reg, out var c1Reg, out var r2Reg, out var c2Reg);

                //Reduce image to just the region size of the variation model
                //ozd region is fixed the tmpobj (affine-translated-image) should be correctly placed 'under' the fixed region
                /* TODO: Replace HOperatorSet.ReduceDomain */ //tmpobj,
                    ozd.region,
                    out tmpobj);
                /* TODO: Replace HOperatorSet.CropDomain */ //tmpobj,
                    out tmpobj); //get rid of edge pixels so image is size of region
                //Mask-out thin lines? We have witnessed this removing long underscores/lines from images which is not expected - not sure of the intention of this
                //Maybe pre-printed separators that don't align with the over-printed text?? 
                EliminateLineStretch(ref tmpobj,
                    ozd.DarkMaxGray);

                //Find defects from variation model
                /* TODO: Replace HOperatorSet.CompareVariationModel */ //tmpobj,
                    out regions,
                    ozd.VARIATION_VAM);
                //Check we have some defects to work with
                if (regions.IsInitialized())
                    if (regions.CountObj() > 0)
                        varRegions = true;
                if (varRegions)
                {
                    #region HDevProcedure
                    //Split the defect pixels into disconneced regions
                    /* TODO: Replace HOperatorSet.Connection */ //regions,
                        out regions);
                    //Only select ones with a thickness
                    /* TODO: Replace HOperatorSet.SelectShape */ //regions,
                        out selectedRegions,
                        features,
                        "and",
                        radiusSmall,
                        radiusLarge);
                    //Now only select ones with an large enough area - don't know why DarkminSizeVar is doubled here
                    /* TODO: Replace HOperatorSet.SelectShape */ //selectedRegions,
                        out connectedRegions,
                        "area",
                        "and",
                        ozd.DarkMinSizeVAR * 2,
                        999999);
                    //Connect again -don't think this does anything at all as the regions have not been manipulated above
                    /* TODO: Replace HOperatorSet.Connection */ //connectedRegions,
                        out connectedRegions);
                    //count how many faults we have
                    /* TODO: Replace HOperatorSet.CountObj */ //connectedRegions,
                        out numDiffs);
                    //get the sizes/areas and centres of  each fault
                    /* TODO: Replace HOperatorSet.AreaCenter */ //connectedRegions,
                        out _,
                        out _,
                        out _);
                    #endregion
                }

                if (numDiffs.I > 0)
                {
                    fp.VALID_LABEL = false;
                    fp.ACTIONED = false;
                    var rfd = new RegionFailData(ozd.NAME);
                    if (rfd.Img == null)
                        /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                    /* TODO: Replace HOperatorSet.CopyObj */ //tmpobj,
                        out rfd.Img,
                        1,
                        1);
                    rfd.Reasons.Add("Print Variation in " + ozd.NAME);
                    fp.RegionFailDataList.Add(rfd);
                    //create and record boxes around each fault
                    /* TODO: Replace Halcon
                    HOperatorSet.SmallestRectangle1(connectedRegions,
                        out var r1,
                        out var c1,
                        out var r2,
                        out var c2);
                    */
                    if (r1.Length == 1)
                        rfd.failCoordsList.Add(new RegionCoordPoints([r1.I, c1.I, r2.I, c2.I]));
                    else
                    {
                        for (var x = 0 ; x < r1.Length ; x++)
                            rfd.failCoordsList.Add(new RegionCoordPoints([r1[x].I, c1[x].I, r2[x].I, c2[x].I]));
                    }
                }

            }
            catch (HalconException hex)
            {
                _logger.Error(hex,
                    "HalconException in CompareZoneImageToModel: {Message}",
                    hex.Message);
                if (SYSTEM_IO.PROCESSING == false) return;
                fp.VALID_LABEL = false;
                fp.ACTIONED = false;
                var rfd = new RegionFailData(ozd.NAME);
                if (rfd.Img == null)
                    /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                /* TODO: Replace HOperatorSet.CopyObj */ //tmpobj,
                    out rfd.Img,
                    1,
                    1);
                rfd.Reasons.Add("compareZoneImageToModel() Halcon error in " + ozd.NAME);
                rfd.Reasons.Add("ERROR: " + hex.Message);
                fp.RegionFailDataList.Add(rfd);
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in CompareZoneImageToModel: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false) return;
                SYSTEM_IO.PROCESSING = false;
                var err = "compareZoneImageToModel() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                rawimage?.Dispose();
                tmpobj?.Dispose();
                regions?.Dispose();
                selectedRegions?.Dispose();
                connectedRegions?.Dispose();
            }
        }

        /// <summary>
        /// This reads the barcodes on the label
        /// </summary>
        /// <param name="img"></param>
        /// <param name="fp"></param>
        /// <returns></returns>
        public bool InspectLabel( Bitmap img, ref FailRecord fp )
        {
            _logger.Information("Entering InspectLabel(img, fp)");
            var retVal = true;
            Bitmap imgTmp = null, zoneRegion = null;
            Bitmap regComplement = null, imgZone = null;
            object countObjects = 0;
            try
            {
                InspectAndMaskMasks(ref img,
                    ref fp);
                InspectBarcodes2D(ref img,
                    ref fp);
                //if we can read the barcode then mask the area from debris checks
                if (fp.BARCODE_UNREADABLE_2D == false)
                    MaskBarcodes2D(ref img,
                        ref fp);
                InspectBarcodesLinear(ref img,
                    ref fp);
                if (fp.BARCODE_UNREADABLE_LINEAR == false)
                    MaskBarcodesLinear(ref img,
                        ref fp);
                /* TODO: Replace HOperatorSet.CopyObj */ //img,
                    out imgTmp,
                    1,
                    1);
                foreach (var ozd in _opZoneItems.OrderBy(x => x.NAME))
                {
                    if (SYSTEM_IO.PROCESSING == false)
                        return true;
                    imgZone?.Dispose();
                    zoneRegion?.Dispose();
                    zoneRegion?.Dispose();
                    regComplement?.Dispose();
                    _variationImg?.Dispose();
                    if (ozd.AffineRegion != null)
                    {
                        /* TODO: Replace Halcon
                        HOperatorSet.SmallestRectangle1(ozd.AffineRegion,
                            out var r1,
                            out var c1,
                            out var r2,
                            out var c2);
                        */
                        /* TODO: Replace Halcon
                        HOperatorSet.GenRectangle1(out zoneRegion,
                            r1 - 4,
                            c1 - 20,
                            r2 + 4,
                            c2 + 20);
                        */
                        /* TODO: Replace HOperatorSet.ReduceDomain */ //imgTmp,
                            zoneRegion,
                            out imgZone);
                        /* TODO: Replace HOperatorSet.Complement */ //imgZone,
                            out regComplement);
                        /* TODO: Replace HOperatorSet.ReduceDomain */ //imgTmp,
                            regComplement,
                            out imgTmp);
                    }
                    //if (ozd.AffineRegion != null)
                    //{
                    //    /* TODO: Replace HOperatorSet.ReduceDomain */ //imgTmp, ozd.region, out imgZone);
                    //    /* TODO: Replace HOperatorSet.Threshold */ //imgZone, out imgThreshold, 0, ozd.DarkMaxGray);
                    //    /* TODO: Replace HOperatorSet.Connection */ //imgThreshold, out connectedThreshold);
                    //    /* TODO: Replace HOperatorSet.SelectShape */ //connectedThreshold, out selectedAreas, "area", "and", 20, 999999);
                    //    if (selectedAreas.IsInitialized())
                    //        try { countObjects = selectedAreas.CountObj(); } catch { countObjects = 0; }
                    //    if (countObjects > 0)
                    //    {
                    //        HOperatorSet.SmallestRectangle1(selectedAreas, out object r1, out object c1, out object r2, out object c2);
                    //        HOperatorSet.GenRectangle1(out zoneRegion, r1, c1, r2, c2);
                    //        HOperatorSet.Union1(zoneRegion, out regUnion);
                    //        HOperatorSet.DilationRectangle1(regUnion, out regDilation, Defaults.DilationWidth, Defaults.DilationHeight);
                    //        /* TODO: Replace HOperatorSet.FillUp */ //regDilation, out regFillup);
                    //        /* TODO: Replace HOperatorSet.Complement */ //regFillup, out regComplement);
                    //        /* TODO: Replace HOperatorSet.ReduceDomain */ //imgTmp, regComplement, out imgTmp);
                    //        eliminateLineStretch(ref imgTmp, ozd.DarkMaxGray);
                    //    }
                    //}
                }
                //redundant as countobjects never set in active code
                if (countObjects > 0)
                {
                    MaskBarcodes2D(ref imgTmp,
                        ref fp);
                    MaskBarcodesLinear(ref imgTmp,
                        ref fp);

                    if (DebrisCheck(ref imgTmp,
                            ref img,
                            _inspectLightAreas,
                            ref fp,
                            "label") == false)
                        retVal = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in InspectLabel: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                var err = "InspectLabel() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                img?.Dispose();
                imgZone?.Dispose();
                imgTmp?.Dispose();
                zoneRegion?.Dispose();
                zoneRegion?.Dispose();
                regComplement?.Dispose();
                _variationImg?.Dispose();
            }
            return retVal;
        }


        /// <summary>
        /// Finds the masked regions, makes them 10px bigger each way and then checks they don't exceed the image size
        /// custs the mask data out of the image leaving the non-masked areas of the image live for inspection
        /// </summary>
        /// <param name="img">Inspetcion image which will be manipulated to remove the masked areas</param>
        /// <param name="fp">fail record set to mask if there is a maske area </param>
        /// <returns></returns>
        public bool InspectAndMaskMasks( ref Bitmap img, ref FailRecord fp )
        {
            _logger.Information("Entering InspectAndMaskMasks(img, fp)");
            var retVal = true;
            Bitmap region = null;

            try
            {
                /* TODO: Replace HOperatorSet.GetImageSize */ //img,
                    out var w,
                    out var h);
                foreach (var vdi in _vdeItems)
                {
                    if (vdi.IsMask)
                    {
                        fp.vDEType = VDEType.MASK;
                        var row1 = vdi.MaskedRegion[0] - 10;
                        var col1 = vdi.MaskedRegion[1] - 10;
                        var row2 = vdi.MaskedRegion[2] + 10;
                        var col2 = vdi.MaskedRegion[3] + 10;
                        if (row1 < 0)
                            row1 = 0;
                        if (col1 < 0)
                            col1 = 0;
                        if (row2 > h)
                            row2 = h;
                        if (col2 > w)
                            col2 = w;

                        region?.Dispose();
                        /* TODO: Replace Halcon
                        HOperatorSet.GenRectangle1(out region,
                            row1,
                            col1,
                            row2,
                            col2);
                        */
                        /* TODO: Replace HOperatorSet.Complement */ //region,
                            out var regionComplement);
                        /* TODO: Replace HOperatorSet.ReduceDomain */ //img,
                            regionComplement,
                            out img);
                        region?.Dispose();
                        regionComplement?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in InspectAndMaskMasks: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                var err = "InspectAndMaskMasks() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return retVal;
        }

        public bool InspectBarcodes2D( ref Bitmap img, ref FailRecord fp )
        {
            _logger.Information("Entering InspectBarcodes2D(img, fp)");
            var retVal = true;
            Bitmap tmpObj = null;
            using Bitmap barcodeBin = null;
            Bitmap symbolXlDs = null, region = null, rgnBc = null, points = null, regionComplement = null;
            object dataCodeHandle = null;

            try
            {
                foreach (var vdi in _vdeItems.Where(x => x.IsBarcode2D))
                {
                    tmpObj?.Dispose();
                    points?.Dispose();
                    regionComplement?.Dispose();
                    barcodeBin?.Dispose();
                    if (dataCodeHandle != null)
                        try { /* TODO: Replace HOperatorSet.ClearBarCodeModel */ //dataCodeHandle); }
                        catch
                        {
                            // ignored
                        }

                    symbolXlDs?.Dispose();
                    //SymbolXLDs = null;
                    region?.Dispose();
                    //region = null;
                    dataCodeHandle = null;
                    fp.vDEType = VDEType.BARCODE_2D;
                    var barcodeType = vdi.BarcodeName;
                    /* TODO: Replace HOperatorSet.GetImageSize */ //img,
                        out var w,
                        out var h);
                    //Expand the Region to search for the 2D code. Doesn't matter if it includes other text etc
                    var t = vdi.BarcodeRegion[0] - 160;
                    var l = vdi.BarcodeRegion[1] - 160;
                    var b = vdi.BarcodeRegion[2] + 160;
                    var r = vdi.BarcodeRegion[3] + 160;
                    if (t < 0) t = 0;
                    if (l < 0) l = 0;
                    if (b > h) b = h;
                    if (r > w) r = w;
                    //Create the region to search in
                    /* TODO: Replace Halcon
                    HOperatorSet.GenRectangle1(out region,
                        t,
                        l,
                        b,
                        r);
                    */
                    /* TODO: Replace HOperatorSet.ReduceDomain */ //img,
                        region,
                        out tmpObj);
                    //Blur the image before reading? not sure why this is needed - can comnent out and code will still execute without other modification
                    /* TODO: Replace HOperatorSet.GaussFilter */ //tmpObj,
                        out tmpObj,
                        3);
                    //Create the barcode reader - this ocld be done outside of the inspection to save time (if it takes much time)
                    //read barcode with default  params - you can restrict the parameters for a faster read
                    /* TODO: Replace Halcon
                    HOperatorSet.CreateDataCode2dModel(barcodeType,
                        "default_parameters",
                        "standard_recognition",
                        out dataCodeHandle);
                    */
                    //HOperatorSet.FindDataCode2d(tmpObj, out SymbolXLDs, DataCodeHandle, "stop_after_result_num", new object(2), out object ResultHandles, out object DecodedDataStrings);
                    //read barcode 
                    /* TODO: Replace Halcon
                    HOperatorSet.FindDataCode2d(tmpObj,
                        out symbolXlDs,
                        dataCodeHandle,
                        new object(),
                        new object(),
                        out _,
                        out var decodedDataStrings);
                    */
                    /* TODO: Replace Halcon
                    HOperatorSet.ClearDataCode2dModel(dataCodeHandle);
                    */
                    object symbolCount;
                    try { /* TODO: Replace HOperatorSet.CountObj */ //symbolXlDs,
                        out symbolCount); } catch { symbolCount = 0; }
                    if (symbolCount > 0)
                    {
                        var datas = new List<string>();
                        var nonVdeData = "";
                        if (decodedDataStrings.S.Length > 0)
                        {
                            for (var x = 0 ; x < decodedDataStrings.Length ; x++)
                            {
                                /* TODO: Replace HOperatorSet.SelectObj */ //symbolXlDs,
                                    out points,
                                    x + 1);
                                /* TODO: Replace HOperatorSet.GetContourXld */ //points,
                                    out var rowPoint,
                                    out var colPoint);
                                if (rowPoint.Length >= 4)
                                {
                                    var row1 = Convert.ToInt32(rowPoint.TupleMin().D - Defaults.PADDING);
                                    var col1 = Convert.ToInt32(colPoint.TupleMin().D - Defaults.PADDING);
                                    var row2 = Convert.ToInt32(rowPoint.TupleMax().D + Defaults.PADDING);
                                    var col2 = Convert.ToInt32(colPoint.TupleMax().D + Defaults.PADDING);
                                    vdi.BarcodeRegion[0] = row1;
                                    vdi.BarcodeRegion[1] = col1;
                                    vdi.BarcodeRegion[2] = row2;
                                    vdi.BarcodeRegion[3] = col2;
                                    /* TODO: Replace Halcon
                                    HOperatorSet.GenRectangle1(out rgnBc,
                                        row1,
                                        col1,
                                        row2,
                                        col2);
                                    */
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
                                        var row1 = vdi.BarcodeRegion[0] - (Defaults.PADDING_BARCODE / 2);
                                        var col1 = vdi.BarcodeRegion[1] - (Defaults.PADDING_BARCODE / 2);
                                        var row2 = vdi.BarcodeRegion[2] + (Defaults.PADDING_BARCODE / 2);
                                        var col2 = vdi.BarcodeRegion[3] + (Defaults.PADDING_BARCODE / 2);
                                        /* TODO: Replace Halcon
                                        HOperatorSet.GenRectangle1(out rgnBc,
                                            row1,
                                            col1,
                                            row2,
                                            col2);
                                        */

                                        var rfd = new RegionFailData(vdi.BarcodeName);
                                        if (rfd.Img == null)
                                            /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                                        /* TODO: Replace HOperatorSet.CopyObj */ //img,
                                            out rfd.Img,
                                            1,
                                            1);
                                        fp.RegionFailDataList.Add(rfd);
                                        rfd.Reasons.Add(string.Format("{0} barcode {1}",
                                            barcodeType,
                                            "missing and/or unreadable"));
                                        if (rgnBc != null)
                                        {
                                            /* TODO: Replace Halcon
                                            HOperatorSet.SmallestRectangle1(rgnBc,
                                                out var r1,
                                                out var c1,
                                                out var r2,
                                                out var c2);
                                            */
                                            rfd.failCoordsList.Add(new RegionCoordPoints([r1, c1, r2, c2]));
                                        }
                                        else if (region != null)
                                        {
                                            rfd.Reasons.Add("A readable barcode was not found");
                                            /* TODO: Replace Halcon
                                            HOperatorSet.SmallestRectangle1(region,
                                                out var r1,
                                                out var c1,
                                                out var r2,
                                                out var c2);
                                            */
                                            rfd.failCoordsList.Add(new RegionCoordPoints([r1, c1, r2, c2]));
                                        }
                                        else
                                            rfd.Reasons.Add("A readable barcode was not found");
                                        fp.ACCEPTED = false;
                                        fp.DATA_FOUND = false;
                                        fp.BARCODE_UNREADABLE_2D = true;
                                        fp.VALID_LABEL = false;
                                    }
                                }
                                nonVdeData = decodedDataStrings[x].S;

                                foreach (var md in _medDataItems)
                                {
                                    if (nonVdeData == md.Data)
                                    {
                                        vdi.BarcodeData.Add(md.Data);
                                        vdi.Placeholder = md.PlaceHolder;
                                        break;
                                    }
                                }

                                foreach (var md in _medDataItems)
                                {
                                    if (nonVdeData.Contains(md.Data))
                                    {
                                        if (datas == null)
                                            datas = [];
                                        datas.Add(md.Data);
                                        nonVdeData = nonVdeData.Replace(md.Data,
                                            "");
                                        if (nonVdeData != "")
                                            fp.BarcodeNonVDEData = nonVdeData;
                                    }
                                }
                            }

                            if (fp.BarcodeNonVDEData != vdi.BarcodeNonVDEData && vdi.BarcodeData.Count == 0)
                            {
                                //if (errorRaisedToPLC == false)
                                //{
                                //    SYSTEM_IO.FAIL_OCCURED();
                                //    errorRaisedToPLC = true;
                                //}
                                retVal = false;
                                fp.VALID_LABEL = false;
                                var rfd = new RegionFailData(vdi.BarcodeName);
                                if (rfd.Img == null)
                                    /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                                /* TODO: Replace HOperatorSet.CopyObj */ //img,
                                    out rfd.Img,
                                    1,
                                    1);
                                fp.RegionFailDataList.Add(rfd);
                                if (datas.Count > 0)
                                    rfd.Reasons.Add(string.Format("{0}: Data error. Found: {1}, searching for: {2}",
                                        barcodeType,
                                        string.Join(",",
                                            datas),
                                        string.Join(",",
                                            vdi.BarcodeData)));
                                if (nonVdeData != vdi.BarcodeNonVDEData)
                                    rfd.Reasons.Add(string.Format(
                                        "{0}: Barcode (non VDE) error. Found: {1}, searching for: {2}",
                                        barcodeType,
                                        nonVdeData,
                                        vdi.BarcodeNonVDEData));
                                if (rgnBc != null)
                                {
                                    /* TODO: Replace Halcon
                                    HOperatorSet.SmallestRectangle1(rgnBc,
                                        out var r1,
                                        out var c1,
                                        out var r2,
                                        out var c2);
                                    */
                                    rfd.failCoordsList.Add(new RegionCoordPoints([r1, c1, r2, c2]));
                                }
                                else if (region != null)
                                {
                                    rfd.Reasons.Add("No barcode found within extended search area");
                                    /* TODO: Replace Halcon
                                    HOperatorSet.SmallestRectangle1(region,
                                        out var r1,
                                        out var c1,
                                        out var r2,
                                        out var c2);
                                    */
                                    rfd.failCoordsList.Add(new RegionCoordPoints([r1, c1, r2, c2]));
                                }
                                else
                                    rfd.Reasons.Add("A readable barcode was not found");
                                fp.ACCEPTED = false;
                                fp.BARCODE_UNREADABLE_2D = true;
                                fp.VALID_LABEL = false;
                            }
                        }
                        else
                        {
                            //if (errorRaisedToPLC == false)
                            //{
                            //    SYSTEM_IO.FAIL_OCCURED();
                            //    errorRaisedToPLC = true;
                            //}
                            if (rgnBc == null)
                            {
                                var row1 = vdi.BarcodeRegion[0] - Defaults.PADDING_BARCODE / 2;
                                var col1 = vdi.BarcodeRegion[1] - Defaults.PADDING_BARCODE / 2;
                                var row2 = vdi.BarcodeRegion[2] + Defaults.PADDING_BARCODE / 2;
                                var col2 = vdi.BarcodeRegion[3] + Defaults.PADDING_BARCODE / 2;
                                /* TODO: Replace Halcon
                                HOperatorSet.GenRectangle1(out rgnBc,
                                    row1,
                                    col1,
                                    row2,
                                    col2);
                                */
                            }

                            var rfd = new RegionFailData(vdi.BarcodeName);
                            if (rfd.Img == null)
                                /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                            /* TODO: Replace HOperatorSet.CopyObj */ //img,
                                out rfd.Img,
                                1,
                                1);
                            fp.RegionFailDataList.Add(rfd);
                            rfd.Reasons.Add(string.Format("{0} barcode {1}",
                                barcodeType,
                                "missing and/or unreadable"));
                            if (rgnBc != null)
                            {
                                /* TODO: Replace Halcon
                                HOperatorSet.SmallestRectangle1(rgnBc,
                                    out var r1,
                                    out var c1,
                                    out var r2,
                                    out var c2);
                                */
                                rfd.failCoordsList.Add(new RegionCoordPoints([r1, c1, r2, c2]));
                            }
                            else if (region != null)
                            {
                                rfd.Reasons.Add("A readable 2D barcode was not found");
                                /* TODO: Replace Halcon
                                HOperatorSet.SmallestRectangle1(region,
                                    out var r1,
                                    out var c1,
                                    out var r2,
                                    out var c2);
                                */
                                rfd.failCoordsList.Add(new RegionCoordPoints([r1, c1, r2, c2]));
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
                _logger.Error(ex,
                    "Exception in InspectAndMaskBarcodes2D: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                var err = "InspectAndMaskBarcodes2D() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                tmpObj?.Dispose();
                barcodeBin?.Dispose();
                if (dataCodeHandle != null)
                    try { /* TODO: Replace HOperatorSet.ClearBarCodeModel */ //dataCodeHandle); }
                    catch
                    {
                        // ignored
                    }

                symbolXlDs?.Dispose();
                region?.Dispose();
                rgnBc?.Dispose();
                points?.Dispose();
                regionComplement?.Dispose();
            }
            return retVal;
        }

        public bool MaskBarcodes2D( ref Bitmap img, ref FailRecord fp )
        {
            _logger.Information("Entering MaskBarcodes2D(img, fp)");
            var retVal = true;
            Bitmap rgnBc = null, rgnComplement = null;

            try
            {
                foreach (var vdi in _vdeItems.Where(x => x.IsBarcode2D))
                {
                    var row1 = vdi.BarcodeRegion[0] - (Defaults.PADDING_BARCODE / 2);
                    var col1 = vdi.BarcodeRegion[1] - (Defaults.PADDING_BARCODE / 2);
                    var row2 = vdi.BarcodeRegion[2] + Defaults.PADDING_BARCODE / 2;
                    var col2 = vdi.BarcodeRegion[3] + Defaults.PADDING_BARCODE / 2;
                    /* TODO: Replace Halcon
                    HOperatorSet.GenRectangle1(out rgnBc,
                        row1,
                        col1,
                        row2,
                        col2);
                    */
                    /* TODO: Replace HOperatorSet.Complement */ //rgnBc,
                        out rgnComplement);
                    /* TODO: Replace HOperatorSet.ReduceDomain */ //img,
                        rgnComplement,
                        out img);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in MaskBarcodes2D: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                var err = "MaskBarcodes2D() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                rgnBc?.Dispose();
                rgnComplement?.Dispose();
            }
            return retVal;
        }

        public bool MaskBarcodesLinear( ref Bitmap img, ref FailRecord fp )
        {
            _logger.Information("Entering MaskBarcodesLinear(img, fp)");
            var retVal = true;
            Bitmap rgnBc = null, rgnComplement = null;

            try
            {
                foreach (var vdi in _vdeItems.Where(x => x.IsBarcodeLinear))
                {
                    var row1 = vdi.BarcodeRegion[0] - (Defaults.PADDING_BARCODE / 2);
                    var col1 = vdi.BarcodeRegion[1] - (Defaults.PADDING_BARCODE / 2);
                    var row2 = vdi.BarcodeRegion[2] + Defaults.PADDING_BARCODE / 2;
                    var col2 = vdi.BarcodeRegion[3] + Defaults.PADDING_BARCODE / 2;
                    /* TODO: Replace Halcon
                    HOperatorSet.GenRectangle1(out rgnBc,
                        row1,
                        col1,
                        row2,
                        col2);
                    */
                    /* TODO: Replace HOperatorSet.Complement */ //rgnBc,
                        out rgnComplement);
                    /* TODO: Replace HOperatorSet.ReduceDomain */ //img,
                        rgnComplement,
                        out img);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in MaskBarcodes2D: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                var err = "MaskBarcodes2D() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                rgnBc?.Dispose();
                rgnComplement?.Dispose();
            }
            return retVal;
        }

        public bool InspectBarcodesLinear( ref Bitmap img, ref FailRecord fp )
        {
            _logger.Information("Entering InspectBarcodesLinear(img, fp)");
            var retVal = true;
            Bitmap tmpObj = null, region = null;
            try
            {
                foreach (var vdi in _vdeItems)
                {
                    if (!vdi.IsBarcodeLinear)
                        continue;
                    List<string> datas = [];
                    tmpObj?.Dispose();
                    region?.Dispose();
                    if (vdi.IsBarcodeLinear)
                    {
                        fp.vDEType = VDEType.BARCODE_LINEAR;
                        /* TODO: Replace HOperatorSet.GetImageSize */ //img,
                            out var w,
                            out var h);
                        var t = vdi.BarcodeRegion[0] - 200;
                        var l = vdi.BarcodeRegion[1] - 200;
                        var b = vdi.BarcodeRegion[2] + 200;
                        var r = vdi.BarcodeRegion[3] + 200;
                        if (t < 0)
                            t = 0;
                        if (l < 0)
                            l = 0;
                        if (b > h)
                            b = h;
                        if (r > w)
                            r = w;
                        /* TODO: Replace Halcon
                        HOperatorSet.GenRectangle1(out region,
                            t,
                            l,
                            b,
                            r);
                        */
                        /* TODO: Replace HOperatorSet.ReduceDomain */ //img,
                            region,
                            out tmpObj);
                        /* TODO: Replace HOperatorSet.GaussFilter */ //tmpObj,
                            out tmpObj,
                            5);
                        var barcodeType = vdi.BarcodeName;
                        /* TODO: Replace HOperatorSet.CreateBarCodeModel */ //new object("composite_code"),
                            new object("CC-A/B"),
                            out var barCodeHandle);
                        /* TODO: Replace HOperatorSet.SetBarCodeParam */ //barCodeHandle,
                            "stop_after_result_num",
                            0);
                        /* TODO: Replace HOperatorSet.SetBarCodeParam */ //barCodeHandle,
                            "barcode_height_min",
                            50);
                        /* TODO: Replace HOperatorSet.SetBarCodeParam */ //barCodeHandle,
                            "min_identical_scanlines",
                            2);
                        //set_bar_code_param
                        /* TODO: Replace HOperatorSet.FindBarCode */ //tmpObj,
                            out _,
                            barCodeHandle,
                            "auto",
                            out var barcodeAsText);
                        if (barcodeAsText.Length > 0)
                        {
                            var med = barcodeAsText.S;
                            foreach (var md in _medDataItems)
                                if (md.PlaceHolder == vdi.Placeholder)
                                    if (med == md.Data)
                                    {
                                        datas.Add(med);
                                    }
                            if (!UtilityFunctions.AlignsWith(vdi.BarcodeData,
                                    datas))
                            {
                                //if (errorRaisedToPLC == false)
                                //{
                                //    SYSTEM_IO.FAIL_OCCURED();
                                //    errorRaisedToPLC = true;
                                //}
                                fp.VALID_LABEL = false;
                                var rfd = new RegionFailData("Barcode");
                                if (rfd.Img == null)
                                    /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                                /* TODO: Replace HOperatorSet.CopyObj */ //img,
                                    out rfd.Img,
                                    1,
                                    1);
                                rfd.Reasons.Add(string.Format("{0} barcode data error: Found: {1}, searching for: {2}",
                                    barcodeType,
                                    string.Join(",",
                                        datas),
                                    string.Join(",",
                                        vdi.BarcodeData)));
                                fp.RegionFailDataList.Add(rfd);
                                /* TODO: Replace Halcon
                                HOperatorSet.SmallestRectangle1(region,
                                    out var r1,
                                    out var c1,
                                    out var r2,
                                    out var c2);
                                */
                                rfd.failCoordsList.Add(new RegionCoordPoints([r1, c1, r2, c2]));
                                fp.DATA_FOUND = false;
                                fp.ACCEPTED = false;
                                /* TODO: Replace HOperatorSet.ClearBarCodeModel */ //barCodeHandle);
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
                            var rfd = new RegionFailData("Barcode");
                            if (rfd.Img == null)
                                /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                            /* TODO: Replace HOperatorSet.CopyObj */ //img,
                                out rfd.Img,
                                1,
                                1);
                            rfd.Reasons.Add(string.Format("{0} barcode data error: Found: {1}, searching for: {2}",
                                barcodeType,
                                "no data",
                                string.Join(",",
                                    vdi.BarcodeData)));
                            rfd.Reasons.Add("No barcode found within extended search area");
                            fp.RegionFailDataList.Add(rfd);
                            /* TODO: Replace Halcon
                            HOperatorSet.SmallestRectangle1(region,
                                out var r1,
                                out var c1,
                                out var r2,
                                out var c2);
                            */
                            rfd.failCoordsList.Add(new RegionCoordPoints([r1, c1, r2, c2]));
                            fp.ACCEPTED = false;
                            fp.BARCODE_UNREADABLE_LINEAR = true;
                            /* TODO: Replace HOperatorSet.ClearBarCodeModel */ //barCodeHandle);
                        }
                        /* TODO: Replace HOperatorSet.ClearBarCodeModel */ //barCodeHandle);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in InspectBarcodesLinear: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                var err = "InspectBarcodesLinear() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                tmpObj?.Dispose();
                region?.Dispose();
            }
            return retVal;
        }

        private bool MaskVdeByOpZone( ref Bitmap img, string opzonename )
        {
            _logger.Information("Entering MaskVdeByOpZone(img, region, opzonename: {opzonename})",
                opzonename);
            bool retVal;
            try
            {
                retVal = MaskVde(rotatedAngle: 90,
                    0,
                    ref img,
                    opzonename);
                if (retVal == false) return false;
                retVal = MaskVde(rotatedAngle: 180,
                    90,
                    ref img,
                    opzonename);
                if (retVal == false) return false;
                retVal = MaskVde(rotatedAngle: 270,
                    180,
                    ref img,
                    opzonename);
                if (retVal == false) return false;
                retVal = MaskVde(rotatedAngle: 360,
                    -90,
                    ref img,
                    opzonename);
                if (retVal == false) return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in MaskVdeByOpZone: {Message}",
                    ex.Message);

                var err = "maskVDEByOpZone() err: " + ex.Message;
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return retVal;
        }

        private bool MaskVde( int rotatedAngle, int angle, ref Bitmap img, string opzonename )
        {
            _logger.Information(
                "Entering MaskVde(rotatedangle: {rotatedangle}, angle: {angle}, img, region, opzonename: {opzonename})",
                rotatedAngle,
                angle,
                opzonename);
            var retVal = true;
            Bitmap regionComplement = null, imgRotated = null, rgn = null;
            try
            {
                /* TODO: Replace HOperatorSet.GenEmptyObj */ //out imgRotated);
                RotateBackground(angle,
                    ref img,
                    ref imgRotated);
                foreach (var vdi in _vdeItems.Where(x => x.OpZoneName.ToUpper() == opzonename.ToUpper() && x.IsVDE && x.VDERotatedAngle == angle))
                {
                    regionComplement?.Dispose();
                    rgn?.Dispose();

                    foreach (var meddata in _medDataItems)
                        if (meddata.PlaceHolder == vdi.Placeholder)
                        {
                            vdi.RepeatType = meddata.Repeat;
                            break;
                        }

                    if (vdi.RepeatType > 0)
                    {
                        /* TODO: Replace Halcon
                        HOperatorSet.GenRectangle1(out rgn,
                            vdi.VDERegion[0] - 4,
                            vdi.VDERegion[1] - 8,
                            vdi.VDERegion[2] + 4,
                            vdi.VDERegion[3] + 8);
                        */
                        /* TODO: Replace HOperatorSet.Complement */ //rgn,
                            out regionComplement);
                        /* TODO: Replace HOperatorSet.ReduceDomain */ //img,
                            regionComplement,
                            out img);
                    }
                    //break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in MaskVde: {Message}",
                    ex.Message);
                var err = "maskVDE() err: " + ex.Message;
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            finally
            {
                regionComplement?.Dispose();
                rgn?.Dispose();
                imgRotated?.Dispose();
            }
            return retVal;
        }

        private void RotateBackground( int angle, ref Bitmap imgIn, ref Bitmap imgOut )
        {
            _logger.Information("Entering RotateBackground(angle: {angle}, imgIn, imgOut)",
                angle);
            try
            {
                imgOut?.Dispose();
                /* TODO: Replace HOperatorSet.RotateImage */ //imgIn,
                    out imgOut,
                    angle * -1,
                    "constant");
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in RotateBackground: {Message}",
                    ex.Message);
                var err = "rotateBackground() err: " + ex.Message;
                if (SYSTEM_IO.PROCESSING == false) return;
                SYSTEM_IO.PROCESSING = false;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }
        #endregion
        static Bitmap BitmapToHImage( Bitmap bmp )
        {
            {
                Bitmap ret = new();
                var bInfo = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                ret.GenImageInterleaved(bInfo.Scan0, "rgbx", bmp.Width, bmp.Height, 0, "int4", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(bInfo);

                return ret;
            }
        }
        protected bool AttachBackgroundToWindow( ref Bitmap img )
        {
            _logger.Information("Entering AttachBackgroundToWindow(img, fp)");
            var retVal = true;
            try
            {
                _currentReducedImage?.Dispose();
                /* TODO: Replace HOperatorSet.CopyObj */ //img,
                    out _currentReducedImage,
                    1,
                    1);

                // TODO: detach/attach background - handled by WPF display
                _ctx?.OnImageClear?.Invoke();
                _ctx?.OnImageDisplay?.Invoke(_currentReducedImage);
                // SetFullImagePart - handled by WPF
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in AttachBackgroundToWindow: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                var err = "attachBackgroundToWindow() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return retVal;
        }

        public virtual void GetImageInspection()
        {
            _logger.Information("Entering GetImageInspection()");
            try
            {
                ClearImage();
                if (EnableCapture)
                {
                    _ctx?.MxClient?.WriteToRegister(1,
                        "Capture_Image",
                        1,
                        3);
                    if (_cameraNecta.nectaCam.Acquire == false)
                        _cameraNecta.nectaCam.Acquire = true;
                    var t = new Task(() => _cameraNecta.GrabCameraImage(ProcessInspectionImage));
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in GetImageInspection: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                var err = "GetImageInspection() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Image Capture",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }

        private List<string> MedPlaceHolders()
        {
            _logger.Information("Entering MedPlaceHolders()");
            var retVal = new List<string>();
            foreach (var md in _medDataItems)
            {
                retVal.Add(md.PlaceHolder);
            }
            return retVal;
        }

        private List<string> MedDatas()
        {
            _logger.Information("Entering MedDatas()");
            var retVal = new List<string>();
            foreach (var md in _medDataItems)
            {
                retVal.Add(md.Data);
            }
            return retVal;
        }

        /*
                private string CurrentMed()
                {
                    _logger.Information("Entering CurrentMed()");
                    var retVal = "";
                    try
                    {
                        MedDatas();
                        foreach (var md in _medDataItems)
                            if (md.VarName.ToUpper().Contains("MED_ID") || md.VarName.ToUpper().Contains("<MED_ID>") || md.VarName.ToUpper().Contains("<MED>") || md.PlaceHolder.ToUpper().Contains("<MED "))
                            {
                                retVal = md.Data;
                                break;
                            }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex,
                            "Exception in CurrentMed: {Message}",
                            ex.Message);
                        var err = "CurrentMed() err: " + ex.Message;
                        var smea = new SystemMessageEventArgs(err,
                            "Inspection",
                            (int)CriticalLevels.Red);
                        _ctx?.OnSystemMessage?.Invoke(smea);
                        //failmethodCaller?.Invoke();
                    }
                    return retVal;
                }
        */

        protected bool InspectionEnd( ref Bitmap imgreduced, ref FailRecord fp )
        {
            _logger.Information("Entering InspectionEnd(imgreduced, fp)");
            Bitmap tmp = null;
            try
            {
                if ((Positioner.ImageCounter == LabelCount) && (_sampleLabel != ""))
                {
                    return false;
                }
                else if ((Positioner.ImageCounter == LabelCount) && (_sampleLabel == ""))
                {
                    Console.WriteLine(@"Count Reached Without Sample");
                    if (_cameraNecta.nectaCam.Acquire)
                    {
                        _cameraNecta.nectaCam.Acquire = false;
                    }
                    SYSTEM_IO.REEL_END();
                    return false;
                }
                else if ((Positioner.ImageCounter == LabelCount + 1) && (_sampleLabel != ""))
                {
                    if (_cameraNecta.nectaCam.Acquire)
                    {
                        _cameraNecta.nectaCam.Acquire = false;
                    }

                    SYSTEM_IO.FAIL_OCCURED();
                    SYSTEM_IO.REEL_END();
                    Positioner.MoveLast(this);
                    FailRecord.FailPipes.Add(fp);
                    /* TODO: Replace HOperatorSet.CopyObj */ //imgreduced,
                        out tmp,
                        1,
                        1);
                    ReduceBackground(ref tmp,
                        ref fp);
                    fp.SAMPLE = true;
                    fp.ACTIONED = false;
                    fp.LabelIndex = LabelCount + 1;
                    var rfd = new RegionFailData("SAMPLE LABEL");
                    fp.RegionFailDataList.Add(rfd);
                    if (rfd.Img == null)
                    {
                        /* TODO: Replace HOperatorSet.GenEmptyObj */ //out rfd.Img);
                    }

                    /* TODO: Replace HOperatorSet.CopyObj */ //tmp,
                        out rfd.Img,
                        1,
                        1);
                    tmp?.Dispose();
                    return true;
                }

                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in InspectionEnd: {Message}",
                    ex.Message);
                var smea = new SystemMessageEventArgs("EndInspectionReel() err: " + ex.Message,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
            }
            finally
            {
                tmp?.Dispose();
            }
            return false;
        }

        protected FailRecord GetNewFailRecord()
        {
            _logger.Information("Entering GetNewFailRecord()");
            var retVal = new FailRecord(ReelLpn,
                Positioner.ImageCounter)
            {
                PlaceHolders = MedPlaceHolders(),
                Datas = MedDatas()
            };
            return retVal;
        }

        private bool InspectVde( ref Bitmap img, ref FailRecord fp, OPZoneData ozd )
        {
            _logger.Information("Entering InspectVde(img, fp, ozd: {ozd})",
                ozd);
            if (_labelStopIndex == _stopAtIndex)
            {

            }
            var retVal = true;
            try
            {
                var angle = 0;
                var vde = new Vde(_opZoneItems,
                    _vdeItems,
                    _medDataItems,
                    UscMd,
                    _pauseMethodCaller,
                    UtilityFunctions);
                vde.CountMatchError = false;
                while (angle < 360)
                {
                    var usedangle = vde.RotateAdjustment(angle);
                    var vdeItems = new List<VDEItem>();
                    foreach (var vdi in _vdeItems.Where(x => x.OpZoneName.ToUpper() == ozd.NAME.ToUpper() && x.IsVDE && x.VDERotatedAngle == usedangle))
                    {
                        foreach (var meddata in _medDataItems)
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
                    if (retVal)
                    {
                        retVal = vde.FindVde(angle,
                            usedangle,
                            ref img,
                            ref fp,
                            ozd,
                            vdeItems,
                            VariableMedDataPh);
                    }
                    else
                    {
                        vde.FindVde(angle,
                            usedangle,
                            ref img,
                            ref fp,
                            ozd,
                            vdeItems,
                            VariableMedDataPh);

                    }
                    if (fp.DATA_INCOMPLETE || fp.DATA_FOUND == false || fp.DatasNotFound.Count > 0)
                        retVal = false;
                    if (vde.CountMatchError)
                        retVal = false;
                    angle += 90;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in InspectVde: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }
                SYSTEM_IO.PROCESSING = false;
                var err = "InspectVDE() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Image Capture",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                retVal = false;
            }
            return retVal;
        }


        private bool _debug_inspection( FailRecord fp )
        {
            _logger.Information("Entering _debug_inspection(fp)");
            bool errorRaisedToPLC = false;
            var bypassInspectionCount = 350; //change to positive value to skip nn images
            if (fp.LabelIndex <= bypassInspectionCount)
            {
                SpeedControl.UpdateSpeedControl(290);
                DispInfoText("debug: label index " + fp.LabelIndex.ToString() + " not inspected");
                fp.VALID_LABEL = true;
                return true;
            }
            return false;
        }

        static void SetSourceFromBitmap( IImageMatcher matcher, Bitmap bmp, string imageDefXml )
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int channels = 3; // Enforcing 24bpp RGB

            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb
            );

            int stride = data.Stride;
            int totalBytes = Math.Abs(stride) * height;
            byte[] rawBytes = new byte[totalBytes];
            Marshal.Copy(data.Scan0, rawBytes, 0, totalBytes);
            bmp.UnlockBits(data);

            byte[] compactBytes;
            if (stride == width * channels)
            {
                compactBytes = rawBytes;
            }
            else
            {
                compactBytes = new byte[width * height * channels];
                for (int y = 0 ; y < height ; y++)
                {
                    Array.Copy(rawBytes, y * stride, compactBytes, y * width * channels, width * channels);
                }
            }

            matcher.SetSourceImage(compactBytes, width, height, channels, imageDefXml);
        }
        /// <summary>
        /// Removes all non-alphanumeric characters from a string.
        /// </summary>
        public static string RemoveNonAlphaNumeric( string input )
        {
            if (string.IsNullOrEmpty(input)) return input;
            char[] arr = input.ToCharArray();
            char[] result = new char[arr.Length];
            int count = 0;
            foreach (char c in arr)
            {
                if (char.IsLetterOrDigit(c))
                {
                    result[count++] = c;
                }
            }
            return new string(result, 0, count);
        }
        public static Bitmap BitmapToHObject( Bitmap bmp )
        {
            if (bmp == null)
                throw new ArgumentNullException(nameof(bmp));

            // Lock bitmap data for direct memory access
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            try
            {
                IntPtr ptr = bmpData.Scan0;
                int width = bmp.Width;
                int height = bmp.Height;

                Bitmap hImage;

                switch (bmp.PixelFormat)
                {
                    case PixelFormat.Format8bppIndexed:
                        // Grayscale image
                        /* TODO: Replace Halcon
                        HOperatorSet.GenImage1(out hImage, "byte", width, height, ptr);
                        */
                        break;

                    case PixelFormat.Format24bppRgb:
                        // 24-bit RGB image (BGR order in memory)
                        /* TODO: Replace HOperatorSet.GenImageInterleaved */ //
                            out hImage,
                            ptr,
                            "bgr", // Halcon expects "bgr" for 24-bit
                            width,
                            height,
                            -1, // Default row stride
                            "byte",
                            width,
                            height,
                            0, 0, -1, 0
                        );
                        break;

                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppRgb:
                        // 32-bit image (BGRX in memory)
                        /* TODO: Replace HOperatorSet.GenImageInterleaved */ //
                            out hImage,
                            ptr,
                            "bgrx", // Ignore alpha channel
                            width,
                            height,
                            -1,
                            "byte",
                            width,
                            height,
                            0, 0, -1, 0
                        );
                        break;

                    default:
                        throw new NotSupportedException($"Unsupported PixelFormat: {bmp.PixelFormat}");
                }

                return hImage;
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
        }

        protected virtual void ProcessInspectionImage()
        {

            _logger.Information("Entering ProcessInspectionImage()");
            _edgesNotDetectedError = false;
            if (!_inhibitNextCapture)
            {
                //bool errorRaisedToPLC = false;
                while (_reviewing)
                {
                    // DoEvents - not needed in WPF
                }
                _inspecting = true;

                var sw = new Stopwatch();
                Bitmap tmp1 = null;
                var inspectionPassedOk = false;
                try
                {

                    sw.Start();
                    _currentReducedImage?.Dispose();
                    //DisplayClearInspection();
                    Positioner.MoveNext(this);
                    Positioner.CanMoveNext = true;

                    var fp = GetNewFailRecord();
                    foreach (var med in _medDataItems)
                    {
                        if (med.PlaceHolder == VariableMedDataPh || med.VarName.ToLower().Contains("med_id"))
                        {
                            fp.MED_ID = med.Data;
                            break;
                        }
                    }

                    //get the image from the cameras
                    if (!PrepareLabelImageFromCameraImage(ref tmp1))
                    {
                        return;
                    }

                    using (Bitmap bmpProcess = HalconImageConverter.HObjectToBitmap(tmp1))
                    {
                        SetSourceFromBitmap(ImageMatcher, bmpProcess, "");
                    }
                    if (AppData.SaveImages)
                        SaveImage(tmp1);
                    string[] keys = new string[_medDataItems.Count];
                    string[] values = new string[_medDataItems.Count];
                    int idx = 0;
                    foreach (var med in _medDataItems)
                    {

                        keys[idx] = RemoveNonAlphaNumeric(med.PlaceHolder.ToString());
                        values[idx] = med.Data.ToString();
                        idx++;
                    }
                    var ret = ImageMatcher.PerformMatch(keys, values);
                    Array imgSafeArray;
                    int w, h, c;
                    
                    // Call COM Method
                    ImageMatcher.GetMarkedImage(out imgSafeArray, out w, out h, out c);

                    byte[] imgData = (byte[])imgSafeArray;
                    if (imgData != null && imgData.Length > 0 && w > 0 && h > 0)
                    {
                        string outputDir = @"C:\tmp";
                        if (!Directory.Exists(outputDir))
                        {
                            Directory.CreateDirectory(outputDir);
                        }
                        string outputPath = Path.Combine(outputDir, "marked_result" + imageNum.ToString() + ".bmp");
                        imageNum++;

                        // Create Bitmap from raw bytes
                        Bitmap bmpRes = new Bitmap(w, h, PixelFormat.Format24bppRgb);
                        BitmapData data = bmpRes.LockBits(
                            new Rectangle(0, 0, w, h),
                            ImageLockMode.WriteOnly,
                            PixelFormat.Format24bppRgb
                        );

                        // Calculate bytes per row (BGR = 3 channels)
                        int rowBytes = w * 3;
                        int stride = data.Stride;

                        // Copy data line by line to handle padding (stride)
                        for (int y = 0 ; y < h ; y++)
                        {
                            int sourceIndex = y * rowBytes;
                            IntPtr destPtr = IntPtr.Add(data.Scan0, y * stride);
                            Marshal.Copy(imgData, sourceIndex, destPtr, rowBytes);
                        }

                        bmpRes.UnlockBits(data);
                        bmpRes.Save(outputPath, ImageFormat.Bmp);
                        _frmI.Invoke(() =>
                        {
                            _frmI.FrmOpenCV.resultImage.Image = bmpRes;
                            _frmI.FrmOpenCV.Text = $"Marked Image {imageNum}";
                            _frmI.FrmOpenCV.Invalidate();
                        } );


                        Console.WriteLine($"Marked image written to: {outputPath}");

                    } 
                    if (Positioner.ImageCounter >= LabelCount)
                    {
                        if (InspectionEnd(ref tmp1,
                                ref fp))
                        {
                            return;
                        }

                    }

                    Labelcounts.CountInspected += 1;
                    if (_sampleLabel != "")
                    {
                        Labelcounts.HasSample = true;
                    }

                    // incremented here before fails occur - value is decremented if all tests pass
                    //failCount++;

                    ////****************************************************************************************
                    //UnRem this block to fast forward through labels without inspection (qty to bypass is set in _debug_inspection() method)
                    if (_debug_inspection(fp))
                        return;
                    ////****************************************************************************************

                    //_ctx?.MxClient?.Stop(1);


                }
                catch (Exception ex)
                {
                    _logger.Error(ex,
                        "Exception in ProcessInspectionImage: {Message}",
                        ex.Message);
                    if (SYSTEM_IO.PROCESSING == false)
                        return;
                    SYSTEM_IO.PROCESSING = false;
                    var err = "ProcessInspectionImage() err: " + ex.Message;
                    var smea = new SystemMessageEventArgs(err,
                        "Inspection Error",
                        (int)CriticalLevels.Red);
                    _ctx?.OnSystemMessage?.Invoke(smea);
                    Emh?.Invoke(err);

                }
                finally
                {
                    tmp1?.Dispose();
                    _currentRawImage?.Dispose();

                    _inspecting = false;
                    
                    sw.Stop();
             
                }
            }
    

            else
            {
                _inhibitNextCapture = false;
                //Console.WriteLine("");
            }
        }


        protected void SaveImage( Bitmap img )
        {
            return;
            string savePath;
            if (string.IsNullOrEmpty(AppData.SavedImagesPath))
            {
                var folder = Application.StartupPath;
                var applicationFolder = Path.Combine(folder,
                "ImageDump");
                //Directory.Delete(applicationFolder,true);
                if (!Directory.Exists(applicationFolder))
                    Directory.CreateDirectory(applicationFolder);
                savePath = applicationFolder;
            }
            else
                savePath = AppData.SavedImagesPath;

            savePath = Path.Combine(savePath, this.ReelLpn);

            if ( !Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            if (File.Exists(Path.Combine(savePath,
                    Positioner.ImageCounter.ToString() + ".bmp")))
            File.Delete(Path.Combine(savePath,
                Positioner.ImageCounter.ToString() + ".bmp"));
            /* TODO: Replace HOperatorSet.WriteImage */ //img,
                "bmp",
                0,
                Path.Combine(savePath,
                    Positioner.ImageCounter.ToString() + ".bmp"));
        }

        // ReSharper disable once UnusedMember.Local
        private void NoInspectionSaveImagesToFile( Bitmap img, int imgcount )
        {
            _logger.Information("Entering NoInspectionSaveImagesToFile(img, imgcount: {imgcount})",
                imgcount);
            try
            {
                SpeedControl.UpdateSpeedControl(650);

                if (Positioner.ImageCounter < 1)
                    return;

                if (Positioner.ImageCounter <= imgcount)
                {
                    var folder = Application.StartupPath;
                    var applicationFolder = Path.Combine(folder,
                        "ImageDump");
                    //Directory.Delete(applicationFolder,true);
                    Directory.CreateDirectory(applicationFolder);
                    /* TODO: Replace HOperatorSet.WriteImage */ //img,
                        "bmp",
                        0,
                        Path.Combine(applicationFolder,
                            Positioner.ImageCounter.ToString() + ".bmp"));
                    DisplayImageCapture();
                }
                else
                {
                    _ctx?.MxClient?.Stop(1);
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                var err = "image capture" + ex.Message;
                Log.Logger.Error(err);
            }
        }


        protected virtual bool  PrepareLabelImageFromCameraImage( ref Bitmap img )
        {
            _logger.Information("Entering PrepareLabelImageFromCameraImage(img)");
            var retVal = true;
            SystemMessageEventArgs smea;
            try
            {
                img?.Dispose();
                _ctx?.OnImageClear?.Invoke();
                if (!EnableCapture)
                {
                    _cameraNecta.CameraImage?.Dispose();
                    return true;
                }
                if (_cameraNecta.CameraImage == null)
                {
                    var err = "prepareLabelImageFromCameraImage() err: No image returned from Label Camera";
                    smea = new SystemMessageEventArgs("No image returned from Label Camera",
                        "Inspection Error",
                        (int)CriticalLevels.Red);
                    DisplayNullImageFromCamera(_cameraNecta.AliasName);
                    _ctx?.OnSystemMessage?.Invoke(smea);
                    if (SYSTEM_IO.PROCESSING)
                    {
                        Emh?.Invoke(err);
                    }

                    SYSTEM_IO.PROCESSING = false;
                    return false;
                }
                /* TODO: Replace HOperatorSet.CopyObj */ //_cameraNecta.CameraImage,
                    out img,
                    1,
                    1);

                _currentRawImage?.Dispose();
                /* TODO: Replace HOperatorSet.CopyObj */ //img,
                    out _currentRawImage,
                    1,
                    1);

            }
            catch (Exception ex)
            {
                _currentRawImage?.Dispose();
                _logger.Error(ex,
                    "Exception in PrepareLabelImageFromCameraImage: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                var err = "prepareLabelImageFromCameraImage() err: " + ex.Message;
                smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return retVal;
        }
        public static Image GetPassFailImage( bool pass )
        {
            // return pass.png from Resources if as pass is true and fail.png if pass is false
            return pass ? null /* TODO: Resource */ : null /* TODO: Resource */;
        }

        protected void AddFailAndStop( FailRecord fp )
        {
            _logger.Information("Entering AddFailAndStop(fp)");
            try
            {
                if (_labelStopIndex == _stopAtIndex)
                {
                    //Console.WriteLine("Stopped");
                }
                var bmp = GetPassFailImage(false);
                _ctx?.OnPassFailImageChanged?.Invoke(bmp);
                var sMed = "";

                foreach (var med in _medDataItems)
                {
                    if (med.PlaceHolder == VariableMedDataPh)
                    {
                        sMed = med.Data;
                        break;
                    }
                }

                var passResult = string.Format("Label index " + Positioner.ImageCounter.ToString() + " (Med {0}) ",
                    sMed);
                if (fp.VALID_LABEL == false || fp.DUPLICATE || fp.READABLE == false || fp.DATA_FOUND == false || fp.DATA_INCOMPLETE || fp.BARCODE_UNREADABLE_LINEAR || fp.BARCODE_UNREADABLE_2D || fp.ConfidenceLevel < _confidenceLevel)
                {
                    if (fp.MISSING == false)
                    {
                        _inhibitNextCapture = true;
                        if (_edgesNotDetectedError == false)
                        {
                            SYSTEM_IO.FAIL_OCCURED();
                        }
                    }
                    _edgesNotDetectedError = false;
                }
                var reasons = "";

                if (fp.RegionFailDataList.Count > 0)
                {
                    foreach (var r in fp.RegionFailDataList[0].Reasons)
                    {
                        if (!r.ToLower().Contains("sample"))
                        {
                            if (reasons == "")
                                reasons = reasons + r;
                            else
                                reasons = reasons + ", " + r;

                        }
                    }
                }

                var smea = new SystemMessageEventArgs(passResult + reasons,
                    "Inspection Result",
                    (int)CriticalLevels.Black,
                    fp.LabelIndex);
                _ctx?.OnSystemMessage?.Invoke(smea);
                if (fp.ACTIONED == false && fp.SAMPLE == false)
                    FailRecord.FailPipes.Add(fp);
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in AddFailAndStop: {Message}",
                    ex.Message);
                SYSTEM_IO.PROCESSING = false;
                var smea = new SystemMessageEventArgs("AddFailAndStop() err: " + ex.Message,
                    "Inspection",
                    (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                Emh?.Invoke(ex.Message);
            }
        }


        private bool SetFailPipeData( ref FailRecord fp, PositionCheck poscheck, out FailRecord fpout, string opzonename )
        {
            _logger.Information("Entering SetFailPipeData(fp, poscheck: {poscheck}, fpout, opzonename: {opzonename})",
                poscheck,
                opzonename);
            if (_labelStopIndex == _stopAtIndex)
            {

            }

            fpout = null;
            try
            {
                //poscheck = PositionCheck.IS_UNREADABLE;
                if (poscheck == PositionCheck.IS_NEXT)
                {


                }

                else if (poscheck == PositionCheck.IS_DUPLICATE)
                {
                    var rfd = new RegionFailData("Duplicate Label");
                    fp.RegionFailDataList.Add(rfd);
                    fp.DUPLICATE = true;
                    Positioner.CanMoveNext = true;
                }
                else if (poscheck == PositionCheck.IS_INCOMPLETE)
                {
                    fp.DATA_INCOMPLETE = true;
                }

                else if (poscheck == PositionCheck.IS_UNREADABLE)
                {
                    fp.READABLE = false;
                    Positioner.CanMoveNext = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in SetFailPipeData: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }
                SYSTEM_IO.PROCESSING = false;
                var err = "setFailPipeData() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return false;
        }

        private PositionCheck StatusCheck( ref FailRecord fp )
        {
            _logger.Information("Entering StatusCheck(fp)");
            var retVal = PositionCheck.IS_GOOD;

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

                if (fp.DatasNotFound.Count > 0 || fp.DATA_INCOMPLETE)
                {
                    retVal = PositionCheck.IS_INCOMPLETE;
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in StatusCheck: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return PositionCheck.IS_GOOD;
                }

                SYSTEM_IO.PROCESSING = false;
                retVal = PositionCheck.ERROR;
                var err = "DoPositionChecks() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return retVal;
        }

        private bool LabelIsDuplicate( ref FailRecord fp )
        {
            _logger.Information("Entering LabelIsDuplicate(fp)");
            var retVal = false;
            try
            {
                for (var x = 0 ; x < fp.Datas.Count ; x++)
                {
                    var data = fp.Datas[x];
                    foreach (var meddata in _medDataItems)
                    {
                        if (meddata.Repeat > 0)
                            if (meddata.IsDuplicate(data))
                            {
                                retVal = true;
                                break;
                            }
                    }
                    if (retVal)
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in LabelIsDuplicate: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }

                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                var err = "LabelIsDuplicate() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return retVal;
        }

        // ReSharper disable once UnusedMember.Local
        private bool LabelIsNext( ref FailRecord fp )
        {
            _logger.Information("Entering LabelIsNext(fp)");
            var retVal = false;

            try
            {
                if (fp.DatasNotFound == null)
                {
                    return false;
                }
                if (fp.DatasNotFound.Count == 0)
                {
                    return false;
                }
                for (var x = 0 ; x < fp.DatasNotFound.Count ; x++)
                {
                    if (retVal)
                        break;
                    var dataNotFound = fp.DatasNotFound[x].Split('|');
                    if (dataNotFound.Length >= 2)
                        foreach (var meddata in _medDataItems)
                            if (meddata.PlaceHolder == dataNotFound[1].Trim())
                                if (meddata.Repeat > 0)
                                {
                                    retVal = meddata.IsNext(dataNotFound[0],
                                        dataNotFound[2]);
                                    if (retVal)
                                        break;
                                }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in LabelIsNext: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                {
                    return true;
                }
                SYSTEM_IO.PROCESSING = false;
                retVal = false;
                var err = "LabelIsNext() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return retVal;
        }

        // ReSharper disable once UnusedMember.Local
        private bool LabelIsLast()
        {
            _logger.Information("Entering LabelIsLast()");
            var retVal = false;
            try
            {
                foreach (var meddata in _medDataItems)
                    if (meddata.Repeat > 0)
                    {
                        retVal = meddata.IsLast();
                        if (retVal)
                            break;
                    }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in LabelIsLast: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                retVal = false;
                SYSTEM_IO.PROCESSING = false;
                var err = "LabelIsLast() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return retVal;
        }

        private bool LabelIsUnReadable( ref FailRecord fp )
        {
            _logger.Information("Entering LabelIsUnReadable(fp)");
            try
            {
                foreach (var med in _medDataItems)
                {
                    var s = med.Data;
                    foreach (var dat in fp.Datas)
                        if (s == dat)
                        {
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in LabelIsUnReadable: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;

                var err = "LabelIsUnReadable() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return false;
        }

        public bool MoveToNextVde()
        {
            _logger.Information("Entering MoveToNextVde()");
            var retVal = true;
            try
            {
                foreach (var meddata in _medDataItems)
                    meddata.MoveNext();
                moveNextCaller?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in MoveToNextVDE: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return true;
                SYSTEM_IO.PROCESSING = false;
                retVal = false;

                var err = "MoveToNextVDE() err: " + ex.Message;
                SYSTEM_IO.PROCESSING = false;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
            return retVal;
        }


        public string PreviousMedData()
        {
            _logger.Information("Entering PreviousMedData()");
            var retVal = "";
            try
            {
                foreach (var meddata in _medDataItems)
                    retVal = retVal + meddata.PeekPrevious();
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in PreviousMedData: {Message}",
                    ex.Message);
                var err = "PreviousMedData() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                //failmethodCaller?.Invoke();
            }
            return retVal;
        }

        public void MoveLast()
        {
            _logger.Information("Entering MoveLast()");
            try
            {
                foreach (var meddata in _medDataItems)
                    meddata.MoveLast();
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in MoveLast: {Message}",
                    ex.Message);
                var err = "MoveLast() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
            }
        }

        public string ThisMedData()
        {
            _logger.Information("Entering ThisMedData()");
            var retVal = "";
            try
            {
                foreach (var meddata in _medDataItems)
                    retVal = retVal + meddata.PeekThis();
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in ThisMedData: {Message}",
                    ex.Message);
                var err = "ThisMedData() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                //failmethodCaller?.Invoke();
            }
            return retVal;
        }

        protected void ClearImage()
        {
            _logger.Information("Entering ClearImage()");
            try
            {
                _cameraNecta.CameraImage?.Dispose();
                // TODO: detach background - handled by WPF display
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in ClearImage: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false) return;
                SYSTEM_IO.PROCESSING = false;
                var err = "ClearImage()  err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Image Capture",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }

        private void DisplayNullImageFromCamera( string devicename )
        {
            _logger.Information("Entering DisplayNullImageFromCamera(devicename: {devicename})",
                devicename);
            try
            {
                //waitingForImage = false;
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                var err = "displayNullImageFromCamera() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Image Grab",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }

        private async void AutoModeDelay()
        {
            _logger.Information("Entering AutoModeDelay()");
            try
            {
                while (FrmInspect.FrmUi == null)
                {
                    await Task.Delay(2000);
                }

                await Task.Delay(15000);

                if (FrmInspect.FrmUi != null)
                {
                    FrmInspect.FrmUi.AutoHandle();
                }
            }
            catch (Exception ex)
            {
                var err = "AutoModeDelay() err: " + ex.Message;
                Log.Logger.Error(err);
            }
        }

        private void IO_INTERRUPT_Handler( int channel, IOEventArgs e )
        {
            _logger.Information("Entering IO_INTERRUPT_Handler(channel: {channel}, e)",
                channel);
            if (SYSTEM_IO.PROCESSING == false)
                return;

            if (_reviewing)
                return;

            var sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < 60)
                ;
            sw.Stop();

            while (_inspecting)
                ;

            if (_reviewing)
                return;

            if (EnableCapture)
            {
                _reviewing = true;
                CloseAllReviewForms();
                DoReviewLui();

                if ((ReviewLuiMode == "Auto") && (Positioner.ImageCounter < (LabelCount - 5)))
                {
                    //AutoModeDelay();
                }
            }
        }

        private void IO_COS_Handler( int channel, bool high )
        {
            _logger.Information("Entering IO_COS_Handler(channel: {channel}, high: {high})",
                channel,
                high);
            if (SYSTEM_IO.PROCESSING == false)
            {
                var sw1 = new Stopwatch();
                sw1.Start();
                while (sw1.ElapsedMilliseconds < 50)
                    ;
                sw1.Stop();
                if (channel == SYSTEM_IO.END_OF_INSPECTION)
                {
                    ReviewLuiModeIndex = 0; // Clear the auto mode index count
                    Amh -= FrmInspect.Amd;
                    Emh -= FrmInspect.Emd;
                    SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
                    SYSTEM_IO.IO_INTERRUPT_Handler -= IO_INTERRUPT_Handler;
                    SYSTEM_IO.IO_CHANGE_Handler -= _frmI.IO_COS_Handler;
                }
            }
            else
            {
                var sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < 50)
                    ;
                sw.Stop();
                if (channel == SYSTEM_IO.END_OF_INSPECTION)
                {
                    ReviewLuiModeIndex = 0; // Clear the auto mode index count
                    SYSTEM_IO.PROCESSING = false;
                    Amh -= FrmInspect.Amd;
                    Emh -= FrmInspect.Emd;
                    SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
                    SYSTEM_IO.IO_INTERRUPT_Handler -= IO_INTERRUPT_Handler;
                    SYSTEM_IO.IO_CHANGE_Handler -= _frmI.IO_COS_Handler;
                    CompleteInspection();
                }
                else if (channel == SYSTEM_IO.ALARM)
                {
                    SYSTEM_IO.PROCESSING = false;
                    Emh -= FrmInspect.Emd;
                    SYSTEM_IO.IO_CHANGE_Handler -= IO_COS_Handler;
                    SYSTEM_IO.IO_INTERRUPT_Handler -= IO_INTERRUPT_Handler;
                    Amh?.Invoke();
                    Amh -= FrmInspect.Amd;
                }
            }
        }

        protected void CompleteInspection()
        {
            _logger.Information("Entering CompleteInspection()");
            try
            {
                SYSTEM_IO.PROCESSING = false;
                if (_cameraNecta.nectaCam.Acquire)
                    _cameraNecta.nectaCam.Acquire = false;
                _ctx?.MxClient?.WriteToRegister(1,
                    "Mode_Inspect",
                    0,
                    3);

                var hasSample = _sampleLabel != "";

                if (ReelLpn == "")
                {
                    ReelLpn = FailRecord.FailPipes[0].REEL;
                }
                EndInspectionReel(FailRecord.FailPipes,
                    hasSample);
            }
            catch (Exception ex)
            {
                var err = "completeInspection() err: " + ex.Message;
                Log.Logger.Error(err);
            }
        }

        public static void CloseAllReviewForms()
        {
            // Static method, _logger not available
            try
            {
                var openCount = 0;
                if (openCount > 0)
                {
                    var openList = new List<object>();
                    for (var x = openCount - 1 ; x >= 0 ; x--)
                    {
                        var f = openList[x];

                        if (f.InvokeRequired)
                        {
                            f.Invoke((Action)delegate
                            {
                                try { f.Close(); }
                                catch
                                {
                                    // ignored
                                }
                            });
                        }
                        else
                        {
                            try { f.Close(); }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var err = "CloseAllReviewForms() err: " + ex.Message;
                Log.Logger.Error(err);
            }
        }

        public static void CloseAllInspectionForms()
        {
            // Static method, _logger not available
            try
            {
                var openCount = 0;
                if (openCount > 0)
                {
                    var openList = new List<object>();
                    for (var x = openCount - 1 ; x >= 0 ; x--)
                    {
                        var f = openList[x];
                        if (!f.IsDisposed)
                        {
                            if (f.InvokeRequired)
                            {
                                f.Invoke((Action)delegate
                                {
                                    //try { f.Hide(); } catch { }
                                    try { f.Close(); }
                                    catch
                                    {
                                        // ignored
                                    }
                                });
                            }
                            else
                            {
                                //try { f.Hide(); } catch { }
                                try { f.Close(); }
                                catch
                                {
                                    // ignored
                                }
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                var err = "CloseAllInspectionForms() err: " + ex.Message;
                Log.Logger.Error(err);
            }
        }

        private void EndInspectionReel( List<FailRecord> faillist, bool hassample )
        {
            _logger.Information("Entering EndInspectionReel(faillist, hassample: {hassample})",
                hassample);
            SystemMessageEventArgs smea = null;
            try
            {
                try { FrmInspect.Emd -= _frmI.do_error_end; }
                catch
                {
                    // ignored
                }

                try { FrmInspect.Emd -= _frmI.do_error_end; }
                catch
                {
                    // ignored
                }

                var sample = hassample ? 1 : 0;
                var es = new ESignature(ESigReason.EndReel,
                    Roles.LVSIII_USer,
                    "",
                    DataManager);
                es.AuthenticationOnly = true;
                es.ReasonRequired = true;
                es.CanCancel = false;
                es.DoNotAcceptLastUser = false;
                es.CaptureSignature();
                if (es.SignatureAccepted)
                {
                    ImageData.CreateFailFilepath(ReelLpn);
                    var missing = faillist.Where(x => x.MISSING).Count();
                    var accept = LabelCount - faillist.Where(x => x.ACCEPTED).Count();
                    var reject = faillist.Count();
                    var acceptop = faillist.Where(x => x.ACCEPTED_BY_USER && x.SAMPLE == false).Count();
                    var rejectop = faillist.Where(x => x.ACCEPTED_BY_USER == false).Count();
                    SYSTEM_IO.PROCESSING = false;
                    if (_frmI.InvokeRequired)
                    {
                        _ = _frmI.Invoke((Action)delegate
                        {
                            _ctx?.OnEndInspectionEnabled?.Invoke(true);
                            
                            _frmI.StartProgress(acceptop + rejectop + missing + sample);
                            smea = new SystemMessageEventArgs("Creating inspection report..",
                                "Inspection Complete",
                                (int)CriticalLevels.Black);
                            _ctx?.OnSystemMessage?.Invoke(smea);
                            EnableCapture = false;

                            var lin = LabelItem;
                            var endOfLin = lin.IndexOf("Issue No", StringComparison.Ordinal);
                            if (endOfLin > -1)
                                lin = lin.Substring(0,
                                    endOfLin - 1).Trim();
                            DataManager.SummaryDataInspectionFinish(ReelLpn,
                                lin,
                                es.LastUserName,
                                LabelCount,
                                missing,
                                accept,
                                reject,
                                acceptop,
                                rejectop,
                                es.UserReason);
                            var rs = new InspectionReport(ReelLpn,
                                lin,
                                VariableMedDataPh,
                                es.UserReason,
                                es.LastUserName,
                                Labelcounts,
                                LabelCount,
                                _frmI.LabelWorkOrder,
                                VariableMedDataPh,
                                FrmSelect.DataManager);
                            if (rs.CreateDocument())
                            {
                                UtilityFunctions.ReleaseHalconMemoryVariables();
                                DataManager.SaveAction("Inspection Complete",
                                    "Inspection Completed",
                                    ReelLpn,
                                    es.LastUserName,
                                    "EndInspectionReel()",
                                    ReelLpn,
                                    es.UserReason);
                                NectaCameras[0].nectaCam.Acquire = false;
                                _frmI.SetMenuOptions(MenuOptions.IDLE);
                                smea = new SystemMessageEventArgs("Inspection Completed successfully",
                                    "Inspection",
                                    (int)CriticalLevels.Black);
                                _ctx?.OnSystemMessage?.Invoke(smea);
                            }
                            else
                            {
                                smea = new SystemMessageEventArgs("Inspection report creation error.",
                                    "Inspection Report",
                                    (int)CriticalLevels.Black);
                                _ctx?.OnSystemMessage?.Invoke(smea);
                            }
                            _frmI.Hide();

                        });
                    }
                    else
                    {
                        
                        _frmI.StartProgress(acceptop + rejectop + missing + sample);
                        smea = new SystemMessageEventArgs("Creating inspection report..",
                            "Inspection Complete",
                            (int)CriticalLevels.Black);
                        _ctx?.OnSystemMessage?.Invoke(smea);
                        EnableCapture = false;
                        var lin = LabelItem;
                        var endOfLin = lin.IndexOf("Issue No", StringComparison.Ordinal);
                        if (endOfLin > -1)
                        {
                            lin = lin.Substring(0,
                                endOfLin - 1).Trim();
                        }
                        DataManager.SummaryDataInspectionFinish(ReelLpn,
                            lin,
                            es.LastUserName,
                            faillist.Count,
                            missing,
                            accept,
                            reject,
                            acceptop,
                            rejectop,
                            es.UserReason);
                        var rs = new InspectionReport(ReelLpn,
                            lin,
                            VariableMedDataPh,
                            es.UserReason,
                            es.LastUserName,
                            Labelcounts,
                            LabelCount,
                            _frmI.LabelWorkOrder,
                            VariableMedDataPh,
                            FrmSelect.DataManager);
                        if (rs.CreateDocument())
                        {
                            UtilityFunctions.ReleaseHalconMemoryVariables();
                            DataManager.SaveAction("Inspection Complete",
                                "Inspection Completed",
                                ReelLpn,
                                es.LastUserName,
                                "EndInspectionReel()",
                                ReelLpn,
                                es.UserReason);
                            NectaCameras[0].nectaCam.Acquire = false;
                            _frmI.SetMenuOptions(MenuOptions.IDLE);
                            smea = new SystemMessageEventArgs("Inspection Completed successfully",
                                "Inspection",
                                (int)CriticalLevels.Black);
                            _ctx?.OnSystemMessage?.Invoke(smea);
                        }
                        else
                        {
                            smea = new SystemMessageEventArgs("Inspection report creation error.",
                                "Inspection Report",
                                (int)CriticalLevels.Black);
                            _ctx?.OnSystemMessage?.Invoke(smea);
                        }
                        _frmI.Hide();
                    }
                }
                else
                    _frmI.Hide();
            }
            catch (Exception ex)
            {
                smea = new SystemMessageEventArgs("EndInspectionReel() err: " + ex.Message,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                _frmI.EndInspectionError(ex.Message);
            }
        }

        protected void DoReviewLui()
        {
            _logger.Information("Entering DoReviewLui()");
            var t = new Thread(() => ReviewLabelUnderInvestigation());
            t.IsBackground = true;
            t.Start();
        }

        protected virtual void ReviewLabelUnderInvestigation()
        {
            _logger.Information("Entering ReviewLabelUnderInvestigation()");
            FailRecord fpData = null;
            var imageFolder = ImageData.CreateImageFilepath();
            var layoutFile = Path.Combine(imageFolder,
                LabelItem + "_legendkey.png");
            try
            {
                if (SYSTEM_IO.PROCESSING)
                    if (_cmdEndInspection.InvokeRequired)
                    {
                        _cmdEndInspection.Invoke((Action)delegate
                        {
                            _ctx?.OnEndInspectionEnabled?.Invoke(true);
                        });
                    }
                    else
                        _ctx?.OnEndInspectionEnabled?.Invoke(true);
                if (SYSTEM_IO.PROCESSING)
                {
                    foreach (var fr in FailRecord.FailPipes)
                    {
                        if (fr.ACTIONED == false && fr.MISSING == false)
                        {
                            fpData = fr;
                            break;
                        }
                    }
                    if (fpData != null)
                    {
                        FrmInspect.FrmUi = new FrmUnderInvestigation(layoutFile,
                            LabelItem,
                            _variationRegion,
                            UtilityFunctions);
                        FrmInspect.FrmUi.TopLevel = true;
                        FrmInspect.FrmUi.TopMost = true;
                        FrmInspect.FrmUi.Refresh();
                        FrmInspect.FrmUi.Init(fpData,
                            LabelItem,
                            FailFolder);
                        FrmInspect.FrmUi.Visible = false;
                        FrmInspect.FrmUi.ShowDialog();
                        Labelcounts.CountQueried += 1;
                        if (fpData.SAMPLE == false)
                        {
                            if (FrmInspect.FrmUi.AcceptedByOperator)
                                Labelcounts.CountAcceptedOp += 1;
                            else
                                Labelcounts.CountRejectedOp += 1;
                        }
                        else
                            Labelcounts.CountAcceptedOp += 1;
                        FrmInspect.FrmUi.Close();
                        FrmInspect.FrmUi = null;

                        fpData.ACTIONED = true;
                        if (SYSTEM_IO.PROCESSING)
                        {
                            if (_cmdEndInspection.InvokeRequired)
                            {
                                _cmdEndInspection.Invoke((Action)delegate
                                {
                                    _ctx?.OnEndInspectionEnabled?.Invoke(true);
                                });
                            }
                            else
                                _ctx?.OnEndInspectionEnabled?.Invoke(true);
                        }
                        _ctx?.MxClient?.WriteToRegister(1,
                            "Rewind_After_INV",
                            1,
                            3);
                    }

                    else if (Positioner.ImageCounter > LabelCount)
                    {
                        FrmInspect.FrmUi = new FrmUnderInvestigation(layoutFile,
                            LabelItem,
                            _variationRegion,
                            UtilityFunctions);
                        FrmInspect.FrmUi.TopLevel = true;
                        FrmInspect.FrmUi.TopMost = true;
                        FrmInspect.FrmUi.Refresh();
                        FrmInspect.FrmUi.Visible = false;
                        FrmInspect.FrmUi.ShowDialog();
                        FrmInspect.FrmUi.Close();
                        FrmInspect.FrmUi = null;

                        if (SYSTEM_IO.PROCESSING)
                        {
                            if (_cmdEndInspection.InvokeRequired)
                            {
                                _cmdEndInspection.Invoke((Action)delegate
                                {
                                    _ctx?.OnEndInspectionEnabled?.Invoke(true);
                                });
                            }
                            else
                                _ctx?.OnEndInspectionEnabled?.Invoke(true);
                            _ctx?.MxClient?.WriteToRegister(1,
                                "Capture_Image",
                                1,
                                3);
                        }
                    }
                }
                _reviewing = false;


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
                _logger.Error(ex,
                    "Exception in ReviewLabelUnderInvestigation: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                var err = "reviewLabelUnderInvestigation() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                _reviewing = false;
                Emh?.Invoke(err);
            }
            finally
            {
                fpData?.ClearData();
                _reviewing = false;
            }
        }

        protected void MissingLabelTriggered()
        {
            _logger.Information("Entering MissingLabelTriggered()");
            try { _ctx?.MxClient?.WriteToRegister(1,
                "Missing_Label_Inhibit",
                0,
                3); }
            catch
            {
                // ignored
            }

            MissingLabelTrigger = false;
        }

        public void MoveNextCaller( Action movenextcaller )
        {
            _logger.Information("Entering MoveNextCaller(movenextcaller)");
            try
            {
                moveNextCaller = movenextcaller;
            }
            catch (Exception ex)
            {
                if (SYSTEM_IO.PROCESSING == false)
                    return;

                var err = "MoveNextCaller() err: " + ex.Message;
                SYSTEM_IO.PROCESSING = false;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Start",
                    (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }

        public void SetPauseCaller( Action pausemethodcaller )
        {
            _logger.Information("Entering SetPauseCaller(pausemethodcaller)");
            try
            {
                _pauseMethodCaller = pausemethodcaller;
            }
            catch (Exception ex)
            {
                var err = "SetPauseCaller() err: " + ex.Message;
                SYSTEM_IO.PROCESSING = false;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Start",
                    (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }

        public void SetAlarmCaller( Action alarmmethodcaller )
        {
            _logger.Information("Entering SetAlarmCaller(alarmmethodcaller)");
            try
            {
            }
            catch (Exception ex)
            {
                SYSTEM_IO.PROCESSING = false;
                var err = "SetAlarmCaller() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Start",
                    (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }

        public void ClearFails()
        {
            _logger.Information("Entering ClearFails()");
            try
            {
                if (FailRecord.FailPipes.Count > 0)
                    FailRecord.FailPipes.Clear();
            }
            catch (Exception ex)
            {
                var err = "ClearFails() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Start",
                    (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }

        public void ClearResultData( string reelLpn, string lin )
        {
            _logger.Information("Entering ClearResultData(reelLpn: {reelLpn}, lin: {lin})",
                reelLpn,
                lin);
            try
            {
                DataManager.ClearResultData(reelLpn,
                    lin);
            }
            catch (Exception ex)
            {
                var err = "ClearFails() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Start",
                    (int)CriticalLevels.Red);
                SMH?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }


        protected void DisplayGoodInspection( int labelindex )
        {
            _logger.Information("Entering DisplayGoodInspection(labelindex: {labelindex})",
                labelindex);
            try
            {
                if (_hWin.InvokeRequired)
                {
                    _hWin.Invoke((Action)delegate
                    {
                        if (labelindex == LabelCount + 1)
                            DispInfoText("SAMPLE LABEL - OK");
                        else
                            DispInfoText(string.Format("label {0} verified ok",
                                labelindex));
                        var bmp = GetPassFailImage(true);
                        _ctx?.OnPassFailImageChanged?.Invoke(bmp);
                    });
                }
                else
                {
                    if (labelindex == LabelCount + 1)
                        DispInfoText("SAMPLE LABEL - OK");
                    else
                        DispInfoText(string.Format("label {0} verified ok",
                            labelindex));
                    var bmp = GetPassFailImage(true);
                    _ctx?.OnPassFailImageChanged?.Invoke(bmp);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in DisplayGoodInspection: {Message}",
                    ex.Message);
            }
        }

        private void DisplayImageCapture()
        {
            _logger.Information("Entering DisplayImageCapture()");
            try
            {
                if (_hWin.InvokeRequired)
                {
                    _hWin.Invoke((Action)delegate
                    {
                        DispInfoText("Capturing and saving images - no inspection. #  images captured: " + Positioner.ImageCounter.ToString());
                        var bmp = GetPassFailImage(true);
                        _ctx?.OnPassFailImageChanged?.Invoke(bmp);
                    });
                }
                else
                {
                    DispInfoText("Capturing and saving images - no inspection. #  images captured: " + Positioner.ImageCounter.ToString());
                    var bmp = GetPassFailImage(true);
                    _ctx?.OnPassFailImageChanged?.Invoke(bmp);
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Clears the display window of anything previuosly shown (to avoid edges of larger images remaining visible around current display
        /// </summary>
        protected void DisplayClearInspection()
        {
            _logger.Information("Entering DisplayClearInspection()");
            try
            {
                if (_hWin.InvokeRequired)
                {
                    _hWin.Invoke((Action)delegate
                    {
                        _ctx?.OnPassFailImageChanged?.Invoke(null);
                        _ctx?.OnImageClear?.Invoke();
                        DispInfoText("");
                        DispTimeText("");
                        // Invalidate - handled by WPF
                    });
                }
                else
                {
                    _ctx?.OnPassFailImageChanged?.Invoke(null);
                    _ctx?.OnImageClear?.Invoke();
                    DispInfoText("");
                    // Invalidate - handled by WPF
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception in DisplayClearInspection: {Message}",
                    ex.Message);
                if (SYSTEM_IO.PROCESSING == false)
                    return;
                SYSTEM_IO.PROCESSING = false;
                var err = "displayClearInspection() err: " + ex.Message;
                var smea = new SystemMessageEventArgs(err,
                    "Inspection Error",
                    (int)CriticalLevels.Red);
                _ctx?.OnSystemMessage?.Invoke(smea);
                Emh?.Invoke(err);
            }
        }

        private void DispInfoText( string msg )
        {
            _logger.Information("Entering DispInfoText(msg: {msg})",
                msg);
            if (_lblInfoText.InvokeRequired)
            {
                _lblInfoText.Invoke((Action)delegate
                {
                    _ctx?.OnInfoTextChanged?.Invoke(msg);
                    _lblInfoText.Invalidate();
                });
            }
            else
            {
                _ctx?.OnInfoTextChanged?.Invoke(msg);
                _lblInfoText.Invalidate();
            }
        }

        protected void DispTimeText( string msg )
        {
            _logger.Information("Entering DispTimeText(msg: {msg})",
                msg);
            try
            {
                if (_lblTime.InvokeRequired)
                {
                    _lblTime.Invoke((Action)delegate
                    {
                        _ctx?.OnTimeTextChanged?.Invoke(msg);
                        _lblTime.Invalidate();
                    });
                }
                else
                {
                    _ctx?.OnTimeTextChanged?.Invoke(msg);
                    _lblTime.Invalidate();
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}