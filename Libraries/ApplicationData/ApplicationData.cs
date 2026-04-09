using CONSTANTS;
using System.Drawing;
using Serilog;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using static LVS3.Delegates;
using static LVS3.Enums;

namespace LVS3
{
    public class AppData
    {
        public static string SavedImagesPath;
        public static bool SaveImages;

    }

    public class InspectionParamDefaults
    {
        public double DebrisMinSize1 = 0;
        public double DebrisMinSize2 = 0;
        public double DebrisMinSize3 = 0;
        public double SobelAmpSize1 = 0;
        public double SobelAmpSize2 = 0;
        public double SobelAmpSize3 = 0;
        public double SobelEdge1 = 0;
        public double SobelEdge2 = 0;
        public double SobelEdge3 = 0;
        public double MeanOffset = 0;
    }

    [Serializable]
    public class InspectionParams
    {
        public bool InspectBright = false;
        public double DebrisMinSize = 50.0;
        public double DebrisMaxSize = 999999.0;
        public double MeanOffset = 0.7;
        public double SobelAmpSize = 7.0;
        public double SobelEdgeThreshold = 40.0;
        public int LabelID = 0;

        public void LoadDefaults(InspectionParamDefaults defaults)
        {            
            this.DebrisMinSize = defaults.DebrisMinSize1;
            this.SobelAmpSize = defaults.SobelAmpSize1;
            this.SobelEdgeThreshold = defaults.SobelEdge1;
            this.MeanOffset = defaults.MeanOffset;            
        }
    }

    public class AffineParams
    {
        public double RowCenter = 0;
        public double ColumnCenter = 0;
        public double PHI = 0;
        /// <summary>
        /// Midpoint to RH end of rectangle (half length)
        /// </summary>
        public double Length1 = 0;
        /// <summary>
        /// Midpoint to Top edge of rectangle (half height)
        /// </summary>
        public double Length2 = 0;
    }

    public class LabelCounts
    {
        public int CountInspected = 0;
        public int CountAccepted = 0;
        public int CountQueried = 0;
        public int CountAcceptedOp = 0;
        public int CountRejectedOp = 0;
        public int CountMissing = 0;
        public bool HasSample = false;
        public bool InspectLightArea = false;
        public int LabelType = 0;
        public VDEFilter VdeFilter = VDEFilter.Default;
    }

    public class DebrisAndErrors
    {
        public static List<DebrisAndErrors> Listing = new List<DebrisAndErrors>();
        public int ID = 0;
        public Bitmap DebrisObjects = null;

        public DebrisAndErrors(int id, Bitmap mush)
        {
            ID = id;
            DebrisObjects = mush;
            //MushList.Add(this);            
        }

        public static void Clear()
        {            
            try
            {
                if (Listing.Count > 0)
                {
                    foreach (DebrisAndErrors mush in Listing)
                        mush.cleanup();
                    Listing.Clear();
                }
            }
            catch { }            
        }

        private void cleanup()
        {            
            try
            {
                if (this.DebrisObjects != null)
                    (DebrisObjects as IDisposable)?.Dispose();
            }
            catch { }            
        }
    }


    public class LabelFixture
    {

        public int FixtureOffsetX = 0;
        public int FixtureOffsetY = 0;
        public byte[] dFixtureCenterY
        {
            get
            {
                MemoryStream ms = new MemoryStream();

                if (FixtureCenterY == null)
                    return null;
                // TODO: Serialize FixtureCenterY without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                FixtureCenterY = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]

        public object FixtureCenterY = null;
        public byte[] dFixtureCenterX
        {
            get
            {
                MemoryStream ms = new MemoryStream();

                if (FixtureCenterX == null)
                    return null;
                // TODO: Serialize FixtureCenterX without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                FixtureCenterX = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]

        public object FixtureCenterX = null;
        public byte[] dFixtureROI
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (FixtureROI == null)
                    return null;
                // TODO: Serialize FixtureROI without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                FixtureROI = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]

        public Bitmap FixtureROI = null;
        public byte[] dFixtureID
        {
            get
            {
                MemoryStream ms = new MemoryStream();

                if (FixtureID == null)
                    return null;
                // TODO: Serialize FixtureID without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                FixtureID = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]

        public object FixtureID = null;

        public int HomMatRow = 0;
        public int HomMatCol = 0;
        public double HomMatAngle = 0;

        public void Reset()
        {            
            FixtureOffsetX = 0;
            FixtureOffsetY = 0;
            FixtureCenterY = null;
            FixtureCenterX = null;
            if (FixtureROI != null)
                (FixtureROI as IDisposable)?.Dispose();
            FixtureROI = null;
            /* TODO: Replace HOperatorSet.GenEmptyObj */ //out FixtureROI);
            FixtureID = null;            
        }
    }

    public class TMParams
    {
        public byte[] dParamName
        {
            get
            {
                MemoryStream ms = new MemoryStream();

                // TODO: Serialize ParamName without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                ParamName = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public object ParamName = "";
        public byte[] dParamValue
        {
            get
            {
                MemoryStream ms = new MemoryStream();

                // TODO: Serialize ParamValue without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                ParamValue = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public object ParamValue = "";
    }

    //public class MSERParams
    //{
    //    public double MeanMultiplier = -1;

    //    public int DarkMinGray = 1;
    //    public int DarkMaxGray = 100;
    //    public int DarkSegmentContrast = 40;
    //    public int DarkMaxGrayLimit = 180;
    //    public int DarkDelta = 16;
    //    public int DarkMinDiversity = -1;
    //    public double DarkMaxVariation = 0.7;
    //    public int DarkMinSizeVAR = 50;
    //    public int DarkMinSizeMSER = 30;
    //    public int DarkAreaLarge = 999999;
    //    //public int DarkDarkLightOffset = 0;
    //    public int DarkDilationCircle = 12;

    //    public int LightMinGray = 1;
    //    public int LightMaxGray = 100;
    //    public int LightSegmentContrast = 40;
    //    public int LightMaxGrayLimit = 180;
    //    public int LightDelta = 16;
    //    public int LightMinDiversity = 7;
    //    public double LightMaxVariation = 0.7;
    //    public int LightMinSizeVAR = 50;
    //    public int LightMinSizeMSER = 30;
    //    public int LightAreaLarge = 999999;
    //    //public int LightDarkLightOffset = 0;
    //    public int LightDilationCircle = 12;
    //}

    /// <summary>
    /// Text reader configuration. Formerly used HALCON OCR model reader.
    /// Now stores configuration data only — actual OCR is handled by OpenCVComMatcherLib.
    /// </summary>
    public class TReader
    {
        public List<TMParams> tmParams = new List<TMParams>();
        public string? ReaderName = null;
        public string ModelFileName = "Pharma_NoRej.omc";
        public string MLPFileName = "Pharma_NoRej.omc";
        public object CharWidth = 0;
        public object CharHeight = 0;
        public object StrokeWidth = "";
        public string FontSize = "";
        public int SegmentContrast = 0;
        public string PartitionMethod = "";

        public TReader()
        {
        }

        public void InitReader()
        {
            // OCR initialization now handled by OpenCVComMatcherLib
        }

        public void setFontSizeSegmentation(string name, object charheight, object charwidth, object strokewidth, object segmentcontrast, object partitionmethod)
        {
            try
            {
                CharWidth = charwidth;
                CharHeight = charheight;
                StrokeWidth = strokewidth;
                SegmentContrast = Convert.ToInt32(segmentcontrast);
                FontSize = name;
                PartitionMethod = Convert.ToString(partitionmethod) ?? "";
            }
            catch (Exception ex)
            {
                string err = "setFontSizeSegmentation() err: " + ex.Message;
                Log.Logger.Error(err);
            }
        }
    }


    public class RegionFailData
    {
        public string OpZoneName = "";
        public string Filename = "";
        public List<RegionCoordPoints> failCoordsList = new List<RegionCoordPoints>();
        public byte[] dImg
        {
            get
            {
                MemoryStream ms = new MemoryStream();

                if (Img == null)
                    return null;
                // TODO: Serialize Img without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                Img = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]

        public Bitmap Img = null;
        public List<string> Reasons = new List<string>();

        public RegionFailData(string opzonename, Bitmap img)
        {
            OpZoneName = opzonename;
            Img = img;
        }
        public RegionFailData(string opzonename)
        {
            OpZoneName = opzonename;
        }
    }




    public class VDEItem
    {
        public AffineParams AffineRegion = new AffineParams();

        public int DilationCircle = 16;
        public int DarkSegmentContrast = Defaults.DarkLabelContrast;
        public int DarkMinSizeVAR = 50;
        public int DarkMaxGray = 100;
        public TReader? tr = null;
        public int RepeatType = 0;
        public string OpZoneName = "";
        public bool DarkLabel = false;
        public int CharacterContrast = 0;
        public int VdeType = (int)Enums.VDEType.OP;
        public byte[] dVDE_ROI
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (VDE_ROI != null)
                { }
                                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                VDE_ROI = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public Bitmap VDE_ROI = null;
        public bool INSPECTED = false;
        public string BarcodeNonVDEData = "";
        public OpZoneDataParams ODP = null;

        public bool DATA_FOUND = false;
        public List<string> DataFoundItems = new List<string>();
        public List<string> BarcodePlaceHolders = new List<string>();

        public string FontName { get => fontName; set => fontName = value; }
        private string fontName = "";
        private string summary = "";
        private string placeHolder = "";
        private bool isBarcode2D = false;
        private bool isBarcodeLinear = false;
        private bool isMask = false;
        private bool isOPZone = false;
        private bool isVDE = false;
        private string reelLPN = "";
        private string barcodeName = "";

        public string Placeholder { get { return placeHolder; } set { placeHolder = value; } }
        public string VDEItemName = "";
        public int[] MaskedRegion = new int[4] { 0, 0, 0, 0 };
        public int[] VDERegion = new int[4] { 0, 0, 0, 0 };
        public int[] BarcodeRegion = new int[4] { 0, 0, 0, 0 };

        public string Summary { get { return summary; } }
        public string ReelLPN { get { return reelLPN; } set { reelLPN = value; } }
        public bool IsBarcode2D { get { return isBarcode2D; } set { isBarcode2D = value; } }
        public bool IsBarcodeLinear { get { return isBarcodeLinear; } set { isBarcodeLinear = value; } }
        public bool IsMask { get { return isMask; } set { isMask = value; } }
        public bool IsOPZone { get { return isOPZone; } set { isOPZone = value; } }
        public bool IsVDE { get { return isVDE; } set { isVDE = value; } }
        public string BarcodeName { get { return barcodeName; } set { barcodeName = value; } }

        public List<string> BarcodeData = new List<string>();
        public int VDERotatedAngle = 0;
        public int VDEAngle = 0;

        public void DestroyVars()
        {
            if (VDE_ROI != null)
                try { (VDE_ROI as IDisposable)?.Dispose(); } catch { }
            if (ODP != null)
            {
                if (ODP.OPZoneIDVar != null)
                    try { (ODP.OPZoneIDVar as IDisposable)?.Dispose(); } catch { }
                if (ODP.OPVariationImages != null)
                    try { (ODP.OPVariationImages as IDisposable)?.Dispose(); } catch { }
            }
        }

        /// <summary>
        /// VDE Data
        /// </summary>
        public VDEItem(string vdeitemname, int[] vdecoords, int rotateangle, VDEType vdetype, TReader? tr)
        {
            this.tr = tr;
            VdeType = (int)vdetype;
            isVDE = true;
            VDERegion = vdecoords;
            VDEItemName = vdeitemname;
            VDERotatedAngle = rotateangle;
        }


        /// <summary>
        /// Mask or OPZone
        /// </summary>
        public VDEItem(string vdeitemname, int[] varcoords, int rotateangle, VDEType vdetype)
        {
            VdeType = (int)vdetype;
            //spaces = null;
            if (VdeType == (int)VDEType.OP)
            {
                isOPZone = true;
                if (ODP == null)
                    ODP = new OpZoneDataParams();
                ODP.OPZoneVARRegion = varcoords;
                /* TODO: Replace HOperatorSet.GenEmptyObj */ //out ODP.OPVariationImages);
            }
            else if (VdeType == (int)VDEType.MASK)
            {
                isMask = true;
                MaskedRegion = varcoords;
            }
            else if (VdeType == (int)VDEType.BARCODE_2D)
            {
                IsBarcode2D = true;
                BarcodeRegion = varcoords;
            }
            else if (VdeType == (int)VDEType.BARCODE_LINEAR)
            {
                isBarcodeLinear = true;
                BarcodeRegion = varcoords;
            }
            VDEItemName = vdeitemname;
            VDERotatedAngle = rotateangle;
            //VDEAngle = angle;
        }

        /// <summary>
        /// Barcodes all types
        /// </summary>
        public VDEItem(string vdeitemname, int[] vdecoords, object vdecolumn1, object vdecolumn2, int rotateangle, VDEType vdetype)
        {
            VdeType = (int)vdetype;
            if (vdetype == VDEType.VDE)
            {
                isVDE = true;
                VDERegion = vdecoords;
            }
            if (vdetype == VDEType.BARCODE_2D)
            {
                isBarcode2D = true;
                BarcodeRegion = vdecoords;
            }

            if (vdetype == VDEType.BARCODE_LINEAR)
            {
                isBarcodeLinear = true;
                BarcodeRegion = vdecoords;
            }

            if (vdetype == VDEType.MASK)
            {
                isMask = true;
                MaskedRegion = vdecoords;
            }
            VDEItemName = vdeitemname;
            VDERotatedAngle = rotateangle;
        }

        /// <summary>
        /// Returns the VDEItems List (all types)
        /// </summary>       
        public VDEItem(int labelid, string placeholder, int[] vdecoords, int rotateangle, VDEType vdetype, string fontname, string barcodename, string vdename)
        {
            VdeType = (int)vdetype;
            if (vdetype == VDEType.VDE)
            {
                isVDE = true;
                VDERegion = vdecoords;
                fontName = fontname;
                placeHolder = placeholder;
                //spaces = new Spaces(spacecount, avgspacewidth);
            }

            if (vdetype == VDEType.BARCODE_2D)
            {
                isBarcode2D = true;
                placeHolder = placeholder;
                barcodeName = barcodename;
                BarcodeRegion = vdecoords;
            }

            if (vdetype == VDEType.BARCODE_LINEAR)
            {
                isBarcodeLinear = true;
                placeHolder = placeholder;
                barcodeName = barcodename;
                BarcodeRegion = vdecoords;
            }
            if (vdetype == VDEType.OP)
            {
                isOPZone = true;
                if (ODP == null)
                    ODP = new OpZoneDataParams();
                ODP.OPZoneVARRegion = vdecoords;
            }
            if (vdetype == VDEType.MASK)
            {
                isMask = true;
                MaskedRegion = vdecoords;
            }
            VDERotatedAngle = rotateangle;
            VDEItemName = vdename;
        }

        public void ResetSetFound()
        {
            DATA_FOUND = false;
        }

        public List<string> CalculateBarcodeData()
        {
            List<string> retVal = new List<string>();
            string[] summary = null;
            try
            {
                if (!isBarcode2D && !isBarcodeLinear)
                    return retVal;

                StringBuilder sb = new StringBuilder();


                //VDE, VDE_NAME, LOCATION, NON_VDE_DATA

                string vde = "";
                string data = "";
                sb.Append(BarcodeName).Append('\n');
                for (int x = 0; x < BarcodePlaceHolders.Count; x++)
                {
                    vde = vde + BarcodePlaceHolders[x];
                    data = data + BarcodeData[x] + ", ";
                }
                sb.Append(vde).Append('\n');
                sb.Append(data).Append('\n');
                sb.Append("TOP: " + this.BarcodeRegion[0].ToString() + ". LEFT: " + this.BarcodeRegion[1].ToString()).Append('\n');
                if (BarcodeNonVDEData != "")
                    sb.Append("Non-VDE Data: ").Append(BarcodeNonVDEData).Append('\n');

                summary = sb.ToString().Split('\n');
                retVal.AddRange(summary);
            }
            catch (Exception ex)
            {
                string err = "CalculateBarcodeData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Setup", (int)CriticalLevels.Amber);
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public List<string> CalculateMaskRegion()
        {
            List<string> retVal = new List<string>();
            string[] summary = null;
            try
            {
                if (!isMask)
                    return retVal;
                StringBuilder sb = new StringBuilder();
                sb.Append("TOP: " + this.MaskedRegion[0].ToString() + ". LEFT: " + this.MaskedRegion[1].ToString()).Append(Environment.NewLine);
                summary = sb.ToString().Split('\n');
                retVal.AddRange(summary);
            }
            catch (Exception ex)
            {
                string err = "CalculateMaskRegion() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Setup", (int)CriticalLevels.Amber);
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public List<string> CalculateOverPrintData()
        {
            List<string> retVal = new List<string>();
            string[] summary = null;
            try
            {
                if (ODP == null)
                    return retVal;

                StringBuilder sb = new StringBuilder();
                sb.Append(VDEItemName).Append('\n');
                sb.Append("TOP: " + ODP.OPZoneVARRegion[0].ToString()).Append(", LEFT: " + ODP.OPZoneVARRegion[1].ToString()).Append('\n');
                sb.Append("Threshold: ").Append(this.DarkMaxGray.ToString()).Append('\n');
                sb.Append("Minimum Area: ").Append(this.DarkMinSizeVAR.ToString()).Append('\n');
                summary = sb.ToString().Split('\n');
                retVal.AddRange(summary);
            }
            catch (Exception ex)
            {
                string err = "CalculateOverPrintData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Setup", (int)CriticalLevels.Amber);
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public List<string> CalculateVDEData()
        {
            List<string> retVal = new List<string>();
            string[] summary = null;
            try
            {
                if (!isVDE)
                    return retVal;
                StringBuilder sb = new StringBuilder();
                sb.Append(Placeholder).Append('\n');
                sb.Append(OpZoneName).Append('\n');
                sb.Append("TOP: " + VDERegion[0].ToString()).Append(". LEFT: " + VDERegion[1].ToString()).Append('\n');
                sb.Append("Partition Method: " + tr.PartitionMethod).Append(". Contrast: " + this.tr.SegmentContrast.ToString()).Append('\n');
                sb.Append("Char Height: " + tr.CharHeight.ToString()).Append(". Char Width: " + this.tr.CharWidth.ToString()).Append('\n');
                summary = sb.ToString().Split('\n');
                retVal.AddRange(summary);
            }
            catch (Exception ex)
            {
                string err = "CalculateVDEData() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Label Setup", (int)CriticalLevels.Amber);
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static int[] RotateClockwiseCoords(int newangle, int width, int height, int[] oldxycoords)
        {
            int[] retVal = new int[] { 0, 0, 0, 0 };
            try
            {
                if (newangle == 90)
                {
                    retVal = oldxycoords;
                }
                else if (newangle == 180)
                {
                    retVal[0] = oldxycoords[1];
                    retVal[1] = height - oldxycoords[2];
                    retVal[2] = oldxycoords[3];
                    retVal[3] = height - oldxycoords[0];
                }
                else if (newangle == 270)
                {
                    retVal[0] = height - oldxycoords[2];
                    retVal[1] = width - oldxycoords[3];
                    retVal[2] = height - oldxycoords[0];
                    retVal[3] = width - oldxycoords[1];
                }
                else if (newangle == 360)
                {
                    retVal[0] = width - oldxycoords[3];
                    retVal[1] = oldxycoords[0];
                    retVal[2] = width - oldxycoords[1];
                    retVal[3] = oldxycoords[2];
                }
            }
            catch (Exception ex)
            {
                string err = "RotateClockwiseCoords() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static int[] RotateAntiClockwiseCoords(int newangle, int width, int height, int[] oldxycoords)
        {
            int[] retVal = new int[] { 0, 0, 0, 0 };
            try
            {
                if (newangle == 90)
                {
                    retVal = oldxycoords;
                }
                else if (newangle == 180)
                {
                    retVal[0] = height - oldxycoords[3];
                    retVal[1] = oldxycoords[0];
                    retVal[2] = height - oldxycoords[1];
                    retVal[3] = (oldxycoords[2] - oldxycoords[0]) + oldxycoords[0];
                }
                else if (newangle == 270)
                {
                    retVal[0] = height - oldxycoords[2];
                    retVal[1] = width - oldxycoords[3];
                    retVal[2] = height - oldxycoords[0];
                    retVal[3] = width - oldxycoords[1];
                }
                else if (newangle == 360)
                {
                    retVal[0] = oldxycoords[1];
                    retVal[1] = width - oldxycoords[2];
                    retVal[2] = oldxycoords[3];
                    retVal[3] = width - oldxycoords[0];
                }
            }
            catch (Exception ex)
            {
                string err = "RotateAntiClockwiseCoords() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        /// <summary>
        /// Reduce the domain of the image to cut-out/ignore the zone of interest
        /// note the relevant field VdeItem.isVDE/isOpZone/isMask has to be set 
        /// This is done in code by creating a new VDEItem
        /// </summary>
        /// <param name="img">The image to use</param>
        /// <param name="angle">if the label has been rotated</param>
        public void ComplementZone(ref Bitmap img, int angle)
        {
            Bitmap RegionComplement = null;
            Bitmap tmpMask = null;
            try
            {
                // TODO: Implement region masking without HALCON
                // Previously created rectangle regions and masked them out of the image
                if (RegionComplement != null)
                    (RegionComplement as IDisposable)?.Dispose();
                if (tmpMask != null)
                    (tmpMask as IDisposable)?.Dispose();
            }
            catch (Exception ex)
            {
                string err = "ComplementZone() err: " + ex.Message;
                Log.Logger.Error(err);
            }
        }
    }
    [Serializable]
    public class FontSizesSegment
    {
        public string Size = "";
        public double dCharWidth
        {
            get => (double)CharWidth; 
            set => CharWidth = value;
        }
        [XmlIgnore]
        public object CharWidth = 0;
        public double dCharHeight
        {
            get => (double)CharHeight; 
            set => CharHeight = value;
        }
        [XmlIgnore]
        public object CharHeight = 0;
        public string sSTrokeWidth
        {
            get => (string)StrokeWidth; 
            set => StrokeWidth = value;
        }
        [XmlIgnore]
        public object StrokeWidth = "medium";
        public int SegmentContrast = 15;
        public string PartitionMethod = "fixed_width";
    }


    public class RegionCoordPoints
    {
        public int[] Points = new int[4];
        public RegionCoordPoints(int[] points)
        {
            Points = points;
        }
    }

    public class FailRecord
    {
        public double ConfidenceLevel = 1;
        public int DisplayedImageIndex = 0;
        public List<RegionFailData> RegionFailDataList = new List<RegionFailData>();
        public bool LISTED_IN_PIPE = false;
        public VDEType vDEType = VDEType.VDE;
        public int Repeat = 0;
        public static List<FailRecord> FailPipes = new List<FailRecord>();
        public int LabelIndex = 1;
        public string REEL = "";
        public string MED_ID = "";
        public bool BARCODE_UNREADABLE_2D = false;
        public bool BARCODE_UNREADABLE_LINEAR = false;
        public bool SPACING_FOUND = true;
        public bool DATA_FOUND = true;
        public bool DATA_INCOMPLETE = false;
        public bool ACCEPTED = true;
        public bool ACCEPTED_BY_USER = false;
        public bool ACTIONED = false;
        public bool DUPLICATE = false;
        public bool MISSING = false;
        public bool READABLE = true;
        public bool VALID_LABEL = true;
        public bool SAMPLE = false;
        public string BackingNumber = "";
        public string CheckedItems = "";
        public string UserData = "";
        public string MissingData = "";
        public string BarcodeNonVDEData = "";
        public List<string> PlaceHolders = new List<string>();
        public List<string> Datas = new List<string>();
        //public List<RegionCoordPoints> VDECoords = new List<RegionCoordPoints>();
        //public List<RegionCoordPoints> ZoneCoords = new List<RegionCoordPoints>();
        public List<string> DatasNotFound = new List<string>();
        public FailRecord(string reel, int labelindex)
        {
            REEL = reel;
            LabelIndex = labelindex;
        }


        public void ClearData()
        {
            try
            {
                foreach (RegionFailData rfd in RegionFailDataList)
                {
                    if (rfd.Img != null)
                        (rfd.Img as IDisposable)?.Dispose();
                }
            }
            catch { }
        }

        public void Clear(string failfolder)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(failfolder);
                foreach (FileInfo fi in dir.GetFiles())
                    fi.Delete();
            }
            catch { }
        }

        public static void ClearAll(string failfolder)
        {
            if (FailPipes.Count == 0)
                return;
            for (int x = FailPipes.Count - 1; x >= 0; x--)
            {
                FailRecord.FailPipes[x].ClearData();
                FailRecord.FailPipes[x].Clear(failfolder);
                FailRecord.FailPipes.RemoveAt(x);
                if (x <= 0)
                    break;
            }
        }
    }

    public static class DateFormats
    {

        public static string LocalTimeAndZone(bool includefulldtz)
        {
            string retVal;
            try
            {
                TimeZone localZone = TimeZone.CurrentTimeZone;
                string timezone = localZone.StandardName;
                if (timezone != "")
                {
                    if (timezone.Length > 3)
                    {
                        timezone = timezone.Substring(0, 3).ToUpper();
                        if (includefulldtz == false)
                            return timezone;
                    }
                }

                DateTime ldt = DateTime.Now;
                retVal = ldt.ToString("dd-MMM-yyyy HH:mm");
                retVal = retVal + " " + timezone;
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "LocalTimeAndZone() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static string LocalTimeAndZone(DateTime date, bool includedtz)
        {
            string retVal;
            try
            {
                TimeZone localZone = TimeZone.CurrentTimeZone;
                string timezone = localZone.StandardName;
                if (timezone != "")
                {
                    if (timezone.Length > 3)
                    {
                        timezone = timezone.Substring(0, 3).ToUpper();
                        if (includedtz == false)
                            return timezone;
                    }
                }

                DateTime ldt = date;
                retVal = ldt.ToString("dd-MMM-yyyy HH:mm");
                retVal = retVal + " " + timezone;
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "LocalTimeAndZone() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }
    }

    public static class PLCFailCodes
    {
        public static List<PLCFailCode> FailCodes = new List<PLCFailCode>();

        public static string GetDescription(string failcode)
        {
            foreach (PLCFailCode fc in FailCodes)
                if (fc.FAIL_CODE == failcode)
                    return fc.FAIL_DESCRIPTION;            
            return failcode;

        }
    }
    [Serializable]
    public class PLCFailCode
    {
        public string FAIL_CODE = "";
        public string FAIL_DESCRIPTION = "";
    }
    [Serializable]
    public class FontData
    {
        public int POINT_SIZE = 0;
        public int PIXEL_HEIGHT = 0;
        public int CHARACTER_WIDTH = 0;
        public int CHARACTER_HEIGHT = 0;
        public double STROKE_WIDTH = 0;
    }
    [Serializable]
    public class OPZoneData
    {
        public int DarkMaxGray = 0;
        public int DarkMinSizeVAR = 0;
        public byte[] dAffineRegion
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (AffineRegion == null)
                    return null;
                // TODO: Serialize AffineRegion without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                AffineRegion = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]

        public Bitmap AffineRegion = null;
        public byte[] dHomMat2D
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (HomMat2D == null)
                    return null;
                // TODO: Serialize HomMat2D without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                HomMat2D = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public object HomMat2D;
        //public MSERParams MSER = new MSERParams();
        public double XOFFSET = 0;
        public double YOFFSET = 0;
        public double MeanMultiplier = 0.1;
        public int MAX_GRAY = 0;
        public int ID = 0;
        public byte[] dFIXTURE_ID
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (FIXTURE_ID == null)
                    return null;
                // TODO: Serialize FIXTURE_ID without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                FIXTURE_ID = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public object FIXTURE_ID;
        public int FIXTURE_X = 0;
        public int FIXTURE_Y = 0;
        public byte[] dVARIATION_VAM
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (VARIATION_VAM == null)
                    return null;
                // TODO: Serialize VARIATION_VAM without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                VARIATION_VAM = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public object VARIATION_VAM;
        public int OPZONE_TOP = 0;
        public int OPZONE_LEFT = 0;
        public int OPZONE_BOTTOM = 0;
        public int OPZONE_RIGHT = 0;
        
        public byte[] dRegion
        {
            get
            {
                if ( region == null ) return null;
                MemoryStream ms = new MemoryStream();
                // TODO: Serialize region without HALCON
                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                region = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public Bitmap region = null;

        public string NAME = "";
        public OPZoneData(int id)
        {
            ID = id;
        }
        public OPZoneData()
        {
            
        }
    }

    public class OpZoneDataParams
    {
        public int MinAreaVar = 50;
        public int MinAreaMSER = 50;
        public bool InspectLight = false;
        public byte[] dOPZoneIDVar
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (OPZoneIDVar != null)
                { }
                                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                OPZoneIDVar = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public object OPZoneIDVar = null;
        public byte[] dOPVariationImages
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (OPVariationImages != null)
                { }
                                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                OPVariationImages = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public Bitmap OPVariationImages = null;
        public byte[] dOPZoneIDFixture
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (OPZoneIDFixture != null)
                { }
                                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                OPZoneIDFixture = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public byte[] dOPZoneFixtureX
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (OPZoneFixtureX != null)
                { }
                                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                OPZoneFixtureX = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public object OPZoneIDFixture = null;
        public byte[] dOPZoneIDFixtureX
         {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (OPZoneFixtureX != null)
                { }
                                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                OPZoneFixtureX = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public object OPZoneFixtureX = null;
        public byte[] dOPZoneFixtureY
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (OPZoneFixtureY != null)
                { }
                                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                OPZoneFixtureY = null /* TODO: Deserialize without HALCON */;
            }
        }
        [XmlIgnore]
        public object OPZoneFixtureY = null;
        public int[] OPZoneVARRegion = new int[] { 0, 0, 0, 0 };
        public byte[] dVAR
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (VAR != null)
                { }
                                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream varms = new MemoryStream(value);
                VAR = null; // TODO: Deserialize VAR without HALCON
            }
        }
        [XmlIgnore]
        public object VAR = null;
        public byte[] dHomMat
        { 
            get
            {
                MemoryStream ms = new MemoryStream();
                if (HomMat != null)
                { }
                                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                HomMat = null /* TODO: Deserialize without HALCON */;
            }
        }

        [XmlIgnore]
        public object HomMat = null;

        public byte[] dDomainImage
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                if (DomainImage != null)
                { }
                                return Array.Empty<byte>();
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                DomainImage = null /* TODO: Deserialize without HALCON */;
            }

        }
        [XmlIgnore]
        public Bitmap DomainImage = null;

        public OpZoneDataParams()
        {

        }

        public void Clear()
        {
            if (OPVariationImages != null)
                (OPVariationImages as IDisposable)?.Dispose();
            if (OPZoneIDVar != null)
                (OPZoneIDVar as IDisposable)?.Dispose();
            // TODO: Removed HALCON operation
            try { OPZoneIDFixture = null; } catch { }
        }

        //public string AsString(VDEItem OPZ)
        //{
        //   string opZoneName = OPZ.OpZoneName + Environment.NewLine;
        //    string retVal = string.Format(opZoneName + "Fixture X {1}, Fixture Y {2}", OPZ.MSER.MinSizeVAR, Math.Round((double)OPZ.ODP.OPZoneFixtureX,2), Math.Round((double)OPZ.ODP.OPZoneFixtureY, 2));
        //    retVal = retVal + string.Format("{0}T: {1}, L: {2}, B: {3}, R: {4}", Environment.NewLine, OPZoneVARRegion[0], OPZoneVARRegion[1], OPZoneVARRegion[2], OPZoneVARRegion[3]);
        //    retVal = retVal + string.Format("{0}Debris Area(min) {1}. Threshold {2}.", Environment.NewLine, OPZ.ODP.MinAreaVar, OPZ.MSER.MaxGray);
        //    return retVal;
        //}
    }

    public static class ImageData
    {

        public static bool DeleteTrainingData(string base_folder)
        {
            bool retVal = true;
            try
            {
                Directory.Delete(base_folder, true);
            }
            catch (Exception ex)
            {
                string err = "DeleteTainingData() err: " + ex.Message;
                Log.Logger.Error(err);
                retVal = false;
            }
            return retVal;
        }

        public static bool DeleteFailData(string fail_folder)
        {
            bool retVal = true;
            try
            {
                Directory.Delete(fail_folder, true);
            }
            catch //(Exception ex)
            {
                //string err = "DeleteFailData() err: " + ex.Message;
                //MessageBox.Show(err);
                retVal = false;
            }
            return retVal;
        }

        public static bool DeleteTempFolderData(string fail_folder)
        {
            bool retVal = true;
            try
            {
                Directory.Delete(fail_folder, true);
            }
            catch //(Exception ex)
            {
                //string err = "DeleteFailData() err: " + ex.Message;
                //MessageBox.Show(err);
                retVal = false;
            }
            return retVal;
        }

        public static string CreateTrainingFilepath(string lpn)
        {
            string retVal;
            try
            {
                //string folder = Application.StartupPath;
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string applicationFolder = Path.Combine(folder, "LVS3");
                Directory.CreateDirectory(applicationFolder);
                applicationFolder = Path.Combine(applicationFolder, Defaults.StationID.ToString());
                Directory.CreateDirectory(applicationFolder);
                string datafolder = Path.Combine(applicationFolder, lpn);
                Directory.CreateDirectory(datafolder);
                datafolder = Path.Combine(datafolder, "TRG");
                Directory.CreateDirectory(datafolder);
                retVal = datafolder;
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "CreateTrainingFilepath() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static string CreateTempFilepath(string lpn)
        {
            string retVal;
            try
            {
                string folder = AppDomain.CurrentDomain.BaseDirectory;
                //string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string applicationFolder = Path.Combine(folder, "LVS3");
                Directory.CreateDirectory(applicationFolder);
                applicationFolder = Path.Combine(applicationFolder, Defaults.StationID.ToString());
                Directory.CreateDirectory(applicationFolder);
                string datafolder = Path.Combine(applicationFolder, lpn);
                Directory.CreateDirectory(datafolder);
                datafolder = Path.Combine(datafolder, "tmp");
                Directory.CreateDirectory(datafolder);
                retVal = datafolder;
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "CreateTempFilepath() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static string CreateFailFilepath(string lpn)
        {
            string retVal;
            try
            {
                //string folder = Application.StartupPath;
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string applicationFolder = Path.Combine(folder, "LVS3");
                Directory.CreateDirectory(applicationFolder);
                applicationFolder = Path.Combine(applicationFolder, Defaults.StationID.ToString());
                Directory.CreateDirectory(applicationFolder);
                string datafolder = Path.Combine(applicationFolder, lpn);
                Directory.CreateDirectory(datafolder);
                datafolder = Path.Combine(datafolder, "FAIL_DATA");
                Directory.CreateDirectory(datafolder);
                retVal = datafolder;
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "CreateFailFilepath() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static string CreateImageDumpFilepath(string lpn)
        {
            string retVal;
            try
            {
                string folder = AppDomain.CurrentDomain.BaseDirectory;
                //string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string applicationFolder = Path.Combine(folder, "LVS3");
                Directory.CreateDirectory(applicationFolder);
                applicationFolder = Path.Combine(applicationFolder, Defaults.StationID.ToString());
                Directory.CreateDirectory(applicationFolder);
                string datafolder = Path.Combine(applicationFolder, lpn);
                Directory.CreateDirectory(datafolder);
                datafolder = Path.Combine(datafolder, "IMAGE_DATA");
                Directory.CreateDirectory(datafolder);
                retVal = datafolder;
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "CreateImageDumpFilepath() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }


        public static string CreateKeyImagesFilepath(string lpn)
        {
            string retVal;
            try
            {

                string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                //string folder = Environment.SpecialFolder.CommonApplicationData;
                string applicationFolder = Path.Combine(folder, "LVS3");
                Directory.CreateDirectory(applicationFolder);
                applicationFolder = Path.Combine(applicationFolder, Defaults.StationID.ToString());
                Directory.CreateDirectory(applicationFolder);
                applicationFolder = Path.Combine(applicationFolder, "KeyImages");
                Directory.CreateDirectory(applicationFolder);
                retVal = applicationFolder;
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "CreateKeyImagesFilepath() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public static string CreateFontFilepathAllMachines()
        {
            //C:\ProgramData\LVS3\ALL_MACHINES\Fonts
            string retVal;
            try
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                //string folder = Application.StartupPath;
                string applicationFolder = Path.Combine(folder, "LVS3");
                Directory.CreateDirectory(applicationFolder);
                applicationFolder = Path.Combine(applicationFolder, "ALL_MACHINES");
                Directory.CreateDirectory(applicationFolder);
                retVal = Path.Combine(applicationFolder, "Fonts");
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "CreateFontFilepathAllMachines() err: " + ex.Message;
                Log.Logger.Error(err);
            }            
            return retVal;
        }

        public static string CreateImageFilepath1()
        {
            //C:\ProgramData\LVS3\ALL_MACHINES\KeyImages
            string retVal;
            try
            {
                string folder = AppDomain.CurrentDomain.BaseDirectory;
                //string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string applicationFolder = Path.Combine(folder, "LVS3");
                Directory.CreateDirectory(applicationFolder);
                applicationFolder = Path.Combine(applicationFolder, Defaults.StationID.ToString());
                Directory.CreateDirectory(applicationFolder);
                retVal = Path.Combine(applicationFolder, "KeyImages");
                Directory.CreateDirectory(retVal);

            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "CreateFontFilepathAllMachines() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }


        public static string CreateImageFilepath()
        {
            //C:\ProgramData\LVS3\ALL_MACHINES\KeyImages
            string retVal;
            try
            {
                //string folder = Application.StartupPath;
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string applicationFolder = Path.Combine(folder, "LVS3");
                Directory.CreateDirectory(applicationFolder);
                applicationFolder = Path.Combine(applicationFolder, Defaults.StationID.ToString());
                Directory.CreateDirectory(applicationFolder);
                retVal = Path.Combine(applicationFolder, "KeyImages");
                Directory.CreateDirectory(retVal);

            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "CreateFontFilepathAllMachines() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }


        public static string CreateReportFilepathAllMachines(string lpn)
        {
            string retVal;
            try
            {
                Uri fileUri = new Uri(new Uri("file://" + Defaults.ReportPath), lpn.ToString() + ".pdf");
                retVal = fileUri.LocalPath;
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = "CreateReportFilepathAllMachines() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

    }

    [Serializable]
    public class MedData
    {
        public string Data = "";
        public string PlaceHolder = "";
        public string VarName = "";
        public string MedSequence = "";
        public bool IsSample = false;
        public int Repeat = 0;
        public int repeatIndex = 0;
        public List<string> sequence_list = new List<string>();
        public List<string> data_list = new List<string>();
        int indexer = 0;
        public MedData(string data, string medsequence)
        {
            indexer = 0;
            Data = data;
            MedSequence = medsequence;
        }

        public MedData()
        {

        }
        public void MoveNext()
        {
            indexer++;
            if (indexer < data_list.Count - 1)
            {
                Data = data_list[indexer];
                MedSequence = sequence_list[indexer];
                if (repeatIndex < Repeat)
                    repeatIndex++;
                else
                    repeatIndex = 1;
                if (Repeat == 0)
                    repeatIndex = 0;
            }
            else
            {
                Data = data_list[data_list.Count - 1];
                MedSequence = sequence_list[data_list.Count - 1];
                if (repeatIndex < Repeat)
                    repeatIndex++;
                else
                    repeatIndex = 1;
                if (Repeat == 0)
                    repeatIndex = 0;
            }
        }

        public void MoveLast()
        {
            indexer = data_list.Count - 1;
            Data = data_list[indexer];
            MedSequence = sequence_list[indexer];
        }

        public string PeekThis()
        {
            string retVal;
            if (indexer > data_list.Count - 1)
                retVal = "";
            else
                retVal = data_list[indexer];
            return retVal;
        }

        public string PeekPrevious()
        {
            string retVal = "";
            if (indexer > 0)
                retVal = data_list[indexer - 1];
            return retVal;
        }

        public bool IsNext(string candidate, string searchdata)
        {            
            bool retVal = false;

            if (Repeat < 1)
                return retVal;

            int nextMed = indexer + 1;
            int StopAt = data_list.Count - 1;

            while (nextMed <= StopAt)
            {
                string nextval = data_list[nextMed];
                if (searchdata.Contains(nextval))
                {
                    retVal = true;
                    break;
                }
                nextMed++;
            }
            return retVal;
        }

        public bool IsValid(string candidate)
        {
            bool retVal = false;
            try
            {
                if (data_list[indexer] == candidate)
                    retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "IsValid() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public bool IsDuplicate(string candidate)
        {
            bool retVal = false;
            try
            {
                if (data_list.Count == 0)
                    return false;
                if (indexer > 0)
                {
                    if (data_list[indexer - 1] == candidate)
                        if (Repeat == 0)
                            retVal = false;

                    if (data_list[indexer - 1] == candidate)
                        if (repeatIndex % Repeat == 1)
                            retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "IsDuplicate() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public bool IsNext(string candidate)
        {
            bool retVal = false;
            try
            {
                if (data_list.Count == 0)
                    return false;

                else if (indexer >= data_list.Count - 1)
                    return false;

                else
                {
                    if (data_list[indexer] == candidate)
                        if (repeatIndex == Repeat && Repeat > 0)
                            retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "IsDuplicate() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public bool IsLast()
        {
            if (Repeat > 0)
            {
                bool retval1 = (indexer >= data_list.Count - 1);
                return retval1;
            }
            else
            {
                return false;
            }

        }
    }
    [Serializable]
    public class LabelItemAndVersion
    {
        public string REEL = "";
        public string LWO = "";
        public string LIN = "";
        public string LAF = "";
        public List<string> Versions = new List<string>();
        public LabelItemAndVersion() { }
        public int HighestVersion = 0;

        public bool DataPresent { get => LIN != "" && REEL != "" && LWO != ""; }
    }

    [Serializable]
    public class LabelItemDataSetup
    {
        public bool IsNumeric = false;
        public List<string> VDE_DATA_LIST = new List<string>();
        public List<string> VDE_SEQUENCE_LIST = new List<string>();
        public string LIN = "";
        public string FIELD_TYPE = "";
        public string PLACE_HOLDER = "";
        public string VARIABLE_NAME = "";
        public string VDE_DATA = "";
        public int Repeat = 0;
        public LabelItemDataSetup() { }
        public bool DataPresent { get => LIN != "" && PLACE_HOLDER != "" && VARIABLE_NAME != "" && Repeat >= 0; }

        public string GetDataAtIndex(int index)
        {
            if (VDE_DATA_LIST.Count == 0)
            {
                return "";
            }

            else if (index > VDE_DATA_LIST.Count - 1)
            {
                return "";
            }

            else
            {
                return VDE_DATA_LIST[index].ToString();
            }

        }
    }

    public class ReelData
    {
        public string REEL = "";
        public string LWO = "";
        public string LIN = "";
        public int LINVersion = 0;
        public int ColumnsID = 0;

        public ReelData() { }

        public bool DataPresent { get => REEL != "" && LWO != "" && LIN != "" && LINVersion > 0 && ColumnsID > 0; }
        public bool NewItem { get => REEL != "" && LWO != "" && LIN != "" && LINVersion == 0 && ColumnsID == 0; }
    }

    [Serializable]
    public class PLCRegister
    {
        public string Register = "";
        public PLCRegisterType RegisterType = 0;
        public string Description = "";
        public string ExtendedInformation = "";

        public PLCRegister()
        {

        }
        //public PLCRegister(string register, PLCRegisterType registertype, string description, string extendedinformation)
        //{
        //    this.Register = register;
        //    this.RegisterType = registertype;
        //    this.Description = description;
        //    this.ExtendedInformation = extendedinformation;
        //}
    }
    [Serializable]
    public class ADGroupData
    {
        public int ADGroupLevel = 0;
        public string ADGroupName = "";
        public string ADGroupNameFriendly = "";

        public ADGroupData(int level, string groupname, string friendlyname)
        {
            ADGroupLevel = level;
            ADGroupName = groupname;
            ADGroupNameFriendly = friendlyname;
        }

        public ADGroupData()
        {

        }
    }

    [Obsolete("Use CONSTANTS.SecretStore with DPAPI instead of DES encryption. This class will be removed in a future version.")]
    [Serializable]
    public static class ConnectionData
    {
        public static SystemMessageHandler SMH;

        public static string SetVal(string encrypt)
        {
            string _0_rr_r5 = "";
            try
            {
                string _0r_r_ = "s!a[l<s@";
                string _0r_rr = "W$@%&:q.";
                byte[] _0r = { };
                _0r = System.Text.Encoding.UTF8.GetBytes(_0r_rr);
                byte[] _00r_r = { };
                _00r_r = System.Text.Encoding.UTF8.GetBytes(_0r_r_);
                MemoryStream _0rr_ = null;
                CryptoStream _0r_r = null;
                byte[] dxx_01 = System.Text.Encoding.UTF8.GetBytes(encrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    _0rr_ = new MemoryStream();
                    _0r_r = new CryptoStream(_0rr_, des.CreateEncryptor(_00r_r, _0r), CryptoStreamMode.Write);
                    _0r_r.Write(dxx_01, 0, dxx_01.Length);
                    _0r_r.FlushFinalBlock();
                    _0_rr_r5 = Convert.ToBase64String(_0rr_.ToArray());
                }
            }
            catch (Exception ex)
            {
                string err = string.Format("SetVal() err: {0}\n", ex.Message);
                if (SMH != null)
                {
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Crypto", (int)CriticalLevels.Red);
                    SMH(smea);
                }
            }
            return _0_rr_r5;
        }

        public static string GetVal(string decrypt)
        {
            string _0_rr_r5 = "";
            try
            {
                string _f1 = decrypt;
                string _8__5_ = "s!a[l<s@";
                string _9_3 = "W$@%&:q.";
                byte[] e_400 = { };
                e_400 = System.Text.Encoding.UTF8.GetBytes(_9_3);
                byte[] tr1_0 = { };
                tr1_0 = System.Text.Encoding.UTF8.GetBytes(_8__5_);
                MemoryStream _t_00 = null;
                CryptoStream _0_rr0 = null;
                byte[] r4 = new byte[_f1.Replace(" ", "+").Length];
                r4 = Convert.FromBase64String(_f1.Replace(" ", "+"));
                using (DESCryptoServiceProvider flll2 = new DESCryptoServiceProvider())
                {
                    _t_00 = new MemoryStream();
                    _0_rr0 = new CryptoStream(_t_00, flll2.CreateDecryptor(tr1_0, e_400), CryptoStreamMode.Write);
                    _0_rr0.Write(r4, 0, r4.Length);
                    _0_rr0.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    _0_rr_r5 = encoding.GetString(_t_00.ToArray());
                }
            }
            catch (Exception ex)
            {
                string err = string.Format("GetVal() err: {0}\n", ex.Message);
                if (SMH != null)
                {
                    SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "Crypto", (int)CriticalLevels.Red);
                    SMH(smea);
                }
            }
            return _0_rr_r5;
        }
    }

    public class UserInfo
    {
        public const string name = "name";
        public const string mail = "mail";
        public const string givenname = "givenname";
        public const string sn = "sn";
        public const string userPrincipalName = "userPrincipalName";
        public const string distinguishedName = "distinguishedName";
    }

    public class MenuOptions
    {
        public const string ExitOnly = "MenuOptionExitOnly";
        public const string IDLE = "MenuOptionIdle";
        public const string ResumeInspection = "MenuOptionResume";
        public const string Start = "MenuOptionStart";
        public const string EndLPN = "MenuOptionEndLPN";
        public const string Winding = "Winding";
        public const string RUNNING = "RUNNING";
    }

    public class StatusLevel
    {
        public int Status;
        public string Details;

        public StatusLevel(StatusLevels statusLevels, string details)
        {
            Status = (int)statusLevels;
            Details = details;
        }
        public StatusLevel()
        {
            Status = (int)StatusLevels.UNASSIGNED;
            Details = "";
        }
    }

    public struct DeviceConfig
    {
        public bool SUCCESS;
        public IPAddress ip;
        public string port;
        public string name;
        public string vendor;
        public string product;
        public int group;
        public Enums.PeripheralType pt;
        public int dummy;
        public string alkerianame;

        public DeviceConfig(bool succes)
        {
            SUCCESS = succes;
            ip = null;
            port = "";
            name = "";
            port = "";
            vendor = "";
            product = "";
            group = 0;
            pt = Enums.PeripheralType.NOT_ASSIGNED;
            dummy = 0;
            alkerianame = "";
        }
    }

    public struct CameraConfig
    {
        public string AlkeriaDeviceName;
        public string HalconDeviceName;
        public string ConfigFilename;
        public string AliasName;
        public string SerialNumber;

        public CameraConfig(string init)
        {
            AlkeriaDeviceName = init;
            HalconDeviceName = init;
            ConfigFilename = init;
            AliasName = init;
            SerialNumber = init;
        }
    }

    public struct EventLogData
    {
        public string header;
        public string body;
        public string user;

        public override string ToString()
        {
            string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(dt).Append('\t').Append("user: ").Append(user).Append('\t').Append('\t').Append(header).Append('\t').Append(body);            
            return sb.ToString();
        }
    }
}