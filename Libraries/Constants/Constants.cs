using System.Drawing;
using LVS3;

namespace CONSTANTS
{

    public static class Defaults
    {
        public static object DilationWidth = 80; 
        public static object DilationHeight = 30; 
        public static readonly int CameraGain = 1;
        public const int DarkLabelContrast = 15;
        public static bool DarkLabel = false;
        public static int TestImageCount = 6;
        public static int VAMImageCount = 4;
        public static object MSERPolarity = "dark";
        //find aniso
        public static object CreateAniso = 0;
        public static object NumLevelsFind = 0;
        public static object Greediness = 0.7;
        public static object tup0Point5 = 0.5;
        public static object tup0Point7 = 0.7;
        public static object tup1 = 1;
        public static object tup3 = 3;
        public static object tup5 = 5;
        public static object tup9 = 9;
        public static object tup0 = 0;
        public static object radMinus5 = -5.0 * Math.PI / 180.0;
        public static object radMinus2 = -2.0 * Math.PI / 180.0;
        public static object radMinus1 = -1.0 * Math.PI / 180.0;
        public static object radMinus1Point5 = -1.5 * Math.PI / 180.0;
        public static object radMinus0Point5 = -0.5 * Math.PI / 180.0;
        public static object radMinus0Point35 = -0.35 * Math.PI / 180.0;
        public static object radMinus0Point75 = -0.75 * Math.PI / 180.0;
        public static object rad0Point75 = 0.75 * Math.PI / 180.0;

        public static object rad0 = 0.0 * Math.PI / 180.0;
        public static object rad0Point05 = 0.05 * Math.PI / 180.0;
        public static object rad0Point1 = 0.1 * Math.PI / 180.0;
        public static object rad0Point5 = 0.5 * Math.PI / 180.0;
        public static object rad0Point7 = 0.7 * Math.PI / 180.0;
        public static object rad1 = (1.0) * Math.PI / 180.0;
        public static object rad2 = 2 * Math.PI / 180.0;
        public static object rad4 = (4.0) * Math.PI / 180.0;
        public static object rad3 = (3.0) * Math.PI / 180.0;
        public static object rad5 = (5.0) * Math.PI / 180.0;
        public static object rad10 = (10.0) * Math.PI / 180.0;


        // Variation Model Thresholds
        public static object absThreshold = 40;
        public static object absThreshold50 = 50;
        public static object absThresholdDark = 30;
        public static object varThreshold = 2;
        public static object varThreshold0 = 0;
        // Variation Model Thresholds

        public static string UserSigning = "";
        public static string UserLoggedIn = "";

        public static string ReportPath = @"cr-svc-fp6\LVS End of Inspection Reports";

        public static string FileType = "jpeg"; 
        public static string DumpFile = "." + FileType;
        //public static double ConfidenceLevel = 0.88;
        public static double FixtureOPZoneScoreMin = 0.3;
        public static string VariationMessage = "Draw an inspection border around the label";
        public static int PADDING = 24;
        public static int PADDING_BARCODE = 80;

        //  **  SpeedControl
        public static string SpeedControlRegister = "D700";
        public static int SpeedControlUpdateInterval = 1;
        //  **  SpeedControl

        public static string AppPath = "";
        public static int StationID = 0;

        // Siemens PLC IP Address
        public static string IPAddressPLC = "";

        // COSMOS - ORACLE
        public static string UserName = "";
        public static string DB_ServiceName = "";
        public static string DB_UserName = "";
        public static string DB_Password = "";
        public static string DB_Schema = "";
        public static string IP = "";
        public static string PORT = "";
        public static Enums.DatabaseSchema SchemaToUse = Enums.DatabaseSchema.LIVE; 

        public static string IODeviceXML = "IODevice.xml";
        public static string AppTitle = "Label Verification System III";
        public static string ProductVersion = "1.0.0.0";
        public static bool WriteToLog = false;
        public static string CurrentUser = "";
        public static bool DebugMode = false;
        public static readonly int TimeOutSeconds = 20;
        public static readonly string DataManager = "Oracle";
    }
}
