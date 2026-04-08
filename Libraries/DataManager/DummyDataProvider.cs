using System.Drawing;
using LVS3;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static LVS3.Delegates;
using static LVS3.Enums;
using System.Runtime.CompilerServices;

namespace LVS3
{
    public class DummyDataProvider : IDataManager
    {
        public string ErrorDesription
        {
            get => OracleDatabase.ErrorDesription;
            set => OracleDatabase.ErrorDesription = value;
        }

        public SystemMessageHandler DataManagerSMHO { get; set; } = OracleDatabase.SMH;
        public SystemMessageHandler DataManagerSMHL { get; set; } = OracleLabelData.SMH;
        public SystemMessageEventArgs DataManagerSMEA { get; set; }

        public string DataFolder = @"Data\";

        public void Serialize<T>(T obj, int repeat = -1, [CallerMemberName] string fileName = "")
        {
            if (obj == null)
            {
                return;
            }
            var ser = new XmlSerializer(typeof(T));
            string filename;
            if (repeat == -1)
                filename = fileName;
            else
                filename = fileName + "_" + repeat.ToString();

            using var stream = new FileStream(DataFolder + filename, FileMode.Create);
            ser.Serialize(stream, obj);

        }

        public T DeSerialize<T>(T obj,int repeat = -1, [CallerMemberName] string fileName = "")
        {
            if (obj == null)
            {
                return obj;
            }

            // Deserialize from filename into obj
            var ser = new XmlSerializer(typeof(T));
            string filename;
            if (repeat == -1)
                filename = fileName;
            else
                filename = fileName + "_" + repeat.ToString();

            using var stream = new FileStream(DataFolder + filename + ".xml", FileMode.Open);
            {
                T deserializedObj = (T)ser.Deserialize(stream);  
                return deserializedObj;
            }
            

        }
        public DummyDataProvider()
        {
            if (Directory.Exists(DataFolder) == false)
            {
                Directory.CreateDirectory(DataFolder);
            }
        }
        public bool OpenConnection(DatabaseSchema dbs)
        {
            bool retVal = OracleDatabase.OpenOracleConnection(dbs);
            if (retVal == false)
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool SetReportFilePath(string path)
        {
            bool retVal = OracleDatabase.SetReportFilePath(path);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public string GetReportFilePath()
        {
            string retVal = "";
            return DeSerialize(retVal);

           
        }

        public string GetTMFileName(ModelOCMType type)
        {
            string retVal = "";
            return DeSerialize(retVal);
        }

        public List<TMParams> GetTMParams()
        {
            List<TMParams> retVal = new();
            return DeSerialize(retVal);
        }


        public (int f1, int f2, int f3, int f4) GetFilters(string filtersize)
        {
            (int f1, int f2, int f3, int f4) retVal = new();
            return DeSerialize(retVal);
        }


        //public MSERParams GetMSERParams(LabelType labeltype)
        //{
        //    MSERParams retVal = OracleDatabase.GetMSERParams(labeltype);
        //    if (ErrorDesription.Trim() != "")
        //    {
        //        DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
        //        DataManagerSMHO?.Invoke(DataManagerSMEA);
        //    }
        //    return retVal;
        //}

        public DataSet AuditLog(string username, DateTime datefrom, DateTime dateto)
        {
            DataSet retVal = new();
            return DeSerialize(retVal);
        }

        public List<string> UserNames()
        {
            List<string> retVal = new();
            return DeSerialize(retVal);
        }

        public List<FontSizesSegment> GetListFontSizes()
        {
            List<FontSizesSegment> retVal = new();
            return DeSerialize(retVal);
        }


        public bool DeleteTrainingData(string labelitem)
        {
            bool retVal = OracleDatabase.DeleteTrainingData(labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool LabelIsTrained(int stationid, string labelitem)
        {
            bool retVal = false;
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return DeSerialize(retVal);
        }

        public bool InsertInspectionParamsVDEItem(VDEItem vi, int[] coords, int labelid)
        {
            bool retVal = false;
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return DeSerialize(retVal);
        }


        public bool InspectionParamsUpdateOPZone(int id, VDEItem vdi, string tmp_folder)
        {
            bool retVal = false;
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return DeSerialize(retVal);
        }

        public bool InspectionParamsUpdateOPZonesDeleteAll(int id)
        {
            bool retVal = false;
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool InspectionParamsUpdateVDEItemsDeleteAll(int labelid)
        {
            bool retVal = OracleLabelData.InspectionParamsUpdateVDEItemsDeleteAll(labelid);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public List<VDEItem> InspectionParamsVDEItem(int labelid, List<FontSizesSegment> fontsizessegment,
            LabelType labeltype)
        {
            List<VDEItem> retVal = OracleLabelData.InspectionParamsVDEItemsList(labelid, fontsizessegment, labeltype);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }


        public string GetSample(string lpn)
        {
            string retVal = "";

            return DeSerialize(retVal);
        }

        public List<FontData> LoadFontDataItems()
        {
            List<FontData> retVal = new();
            return DeSerialize(retVal);
        }

        public double GetInnerRadiusDefaultSmall()
        {
            double retVal = 0.0;
            return DeSerialize(retVal);
        }

        public double GetInnerRadiusDefaultLarge()
        {
            double retVal = 0.0;
            return DeSerialize(retVal);
        }

        public double GetConfidenceLevel()
        {
            double retVal = 0.0;
            return DeSerialize(retVal);
        }


        public double GetInnerRadius(int stationid, string labelitem)
        {
            //GetDebrisSize
            double retVal = 0.0;
            return DeSerialize(retVal);
        }

        public string GetDebrisSizeLabel(int stationid, string labelitem)
        {
            string retVal = "";
            return DeSerialize(retVal);
        }

        public LabelType GetLabelType(int stationid, string labelitem)
        {
            LabelType retVal = new();

            return DeSerialize(retVal);
        }

        public string GetLabelTypeAsString(int stationid, string labelitem)
        {
            string retVal = "";
            return DeSerialize(retVal);
        }

        public InspectionParams GeInspectionParams(int stationid, int labelid)
        {
            InspectionParams retVal = new();
            return DeSerialize(retVal);
        }

        public int GetCameraGain(int stationid, string labelitem)
        {
            int retVal = 0;
            return DeSerialize(retVal);
        }

        public List<OPZoneData> OPZoneData(int id, string basefolder, LabelType labeltype)
        {
            List<OPZoneData> retVal = new();
            return DeSerialize(retVal);
        }

        public List<LabelItemDataSetup> LabelDataItems(string lin, string reel_lpn)
        {
            List<LabelItemDataSetup> retVal = new();
            return DeSerialize(retVal);
        }

        public MedData LabelDataInspection(string lwo, string reel_lpn, string placeholder)
        {
            MedData retVal = new();
            return DeSerialize(retVal);
        }

        //public LabelVariationData VariationData(string lin)
        //{
        //    LabelVariationData retVal = OracleDatabase.VariationData(lin);
        //    if (ErrorDesription.Trim() != "")
        //    {
        //        DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
        //        DataManagerSMHO?.Invoke(DataManagerSMEA);
        //    }
        //    return retVal;
        //}

        public int GetCountMeds(string lpn, int stationid)
        {
            int retVal = 0;
            return DeSerialize(retVal);
        }


        public List<string> GetTrainingData(string lin, TrainingDataType tdt)
        {
            List<string> retVal = new();
            return DeSerialize(retVal, (int)tdt);
        }

        public List<string> SaveTrainingData(string lin, TrainingDataType tdt, List<string> td)
        {
            List<string> retVal = OracleDatabase.SaveTrainingData(lin, tdt, td);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        //public int SaveLINData(string lin, bool complete)
        //{
        //    int retVal = OracleDatabase.SaveLINData(lin,  complete);
        //    if (ErrorDesription.Trim() != "")
        //    {
        //        DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
        //        DataManagerSMHO?.Invoke(DataManagerSMEA);
        //    }
        //    return retVal;
        //}

        public string ApplicationSettingGet(string toget)
        {
            string retVal = "";
            return DeSerialize(retVal);
        }

        public string GetReportCompiler(string lpn)
        {
            string retVal = "";
            return DeSerialize(retVal);
        }

        public string GetReportSummary(string lpn)
        {
            string retVal = "";
            return DeSerialize(retVal);
        }

        public DateTime[] GetReportSummaryDates(string lpn)
        {
            DateTime[] retVal = [DateTime.Now];
            return DeSerialize(retVal);
        }

        public string GetCustomer(string labelitem)
        {
            string retVal = "";
            return DeSerialize(retVal);
        }

        public string GetLabelIssue(string lin, string lwo)
        {
            string retVal = "";
            return DeSerialize(retVal);
        }


        public DataSet GetSystemDevices()
        {
            DataSet retVal = new();
            return DeSerialize(retVal);
        }

        public LabelItemAndVersion GoodREEL_LWO_LIN_VERSION_Data(string reel)
        {
            LabelItemAndVersion retVal = new();
            return DeSerialize(retVal);
        }

        public string GetMeaning(ESigReason meaningid)
        {
            string retVal = "";
            return DeSerialize(retVal);
        }

        public List<ADGroupData> GetAllGroups()
        {
            List<ADGroupData> retVal = new();
            return DeSerialize(retVal);
        }

        public bool GetCompleteReport(string reel, string labelitem)
        {
            bool retVal = false;
            return DeSerialize(retVal);
        }

        public List<PLCRegister> LoadPLCRegisters(int plc)
        {
            List<PLCRegister> retVal = new();
            return DeSerialize(retVal);
        }

        public List<PLCFailCode> LoadFailCodes()
        {
            List<PLCFailCode> retVal = new();
            return DeSerialize(retVal);
        }

        public bool SaveAction(string message, string title, string lpn, string username, string method,
            string refersto, string userreason)
        {
            bool retVal = OracleDatabase.SaveAction(message, title, lpn, username, method, refersto, userreason);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool SaveAction1(string message, string title, string lpn, string username, string method,
            string refersto, string operatottext, string userreason)
        {
            bool retVal = OracleDatabase.SaveAction1(message, title, lpn, username, method, refersto, operatottext,
                userreason);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }


        public bool SummaryDataInspectionStart(string reel_lpn, string label_item, string usersaving, int stationid,
            bool cancellinginspection)
        {
            bool retVal = OracleLabelData.SummaryDataInspectionStart(reel_lpn, label_item, usersaving, stationid,
                cancellinginspection);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool SaveSummaryDataStart(string reel_lpn, string label_item, string usersaving, int stationid,
            DateTime started_at)
        {
            bool retVal = OracleLabelData.SaveSummaryDataStart(reel_lpn, label_item, usersaving, stationid, started_at);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool ClearResultData(string lpn, string lin)
        {
            bool retVal = OracleLabelData.ClearResultData(lpn, lin);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool SummaryDataInspectionFinish(string reel_lpn, string label_item, string usersaving, int lblcount,
            int missing, int accept, int reject, int acceptop, int rejectop, string opsummary)
        {
            bool retVal = OracleLabelData.SummaryDataInspectionFinish(reel_lpn, label_item, usersaving, lblcount,
                missing, accept, reject, acceptop, rejectop, opsummary);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool SaveResultData(string lpn, string lin, string acceptedbyuser, string backingnumber,
            string selecteditems, string operatorinfo, string reasons, string additionalinfo, int labelindex)
        {
            bool retVal = OracleLabelData.SaveResultData(lpn, lin, acceptedbyuser, backingnumber, selecteditems,
                operatorinfo, reasons, additionalinfo, labelindex);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool SaveLabelRegion(Bitmap labelvar, int stationid, string labelitem, bool inspectlightareas,
            int maxgray, int minarea)
        {
            bool retVal =
                OracleLabelData.SaveLabelRegion(labelvar, stationid, labelitem, inspectlightareas, maxgray, minarea);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool GetInspectLightAreas(int stationid, string labelitem)
        {
            bool retVal = false;
            return DeSerialize(retVal);
        }

        public int[] GetVariationRegion(int stationid, string labelitem)
        {
            int[] retVal = [1, 2];
            return DeSerialize(retVal);
        }

        public string GetVDEContrastAsString(int stationid, string labelitem)
        {
            string retVal = "";
            return DeSerialize(retVal);
        }


        public int LabelID(int stationid, string labelitem)
        {
            int retVal = 0;
            return DeSerialize(retVal);
        }


        public int ImageCount(VAMImageTypes vit)
        {
            int retVal = 0;
            return DeSerialize(retVal);
        }


        public object GetFixtureID(int stationid, string labelitem, string basefolder)
        {
            var retVal = 0;
            return DeSerialize(retVal);


        }

        public List<object> GetFixtureXY(int stationid, string labelitem, string basefolder)
        {
            List<object> retVal = OracleLabelData.GetFixtureXY(stationid, labelitem, basefolder);
            return DeSerialize(retVal);
        }

        public bool SaveVariationVAM(int stationid, string labelitem, string tmp_folder, string base_folder)
        {
            bool retVal = OracleLabelData.SaveVariationVAM(stationid, labelitem, tmp_folder, base_folder);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool SaveLabelData(bool inspectLightAreas, int minarea, int stationid, string labelitem,
            Bitmap variationroi, double innerradius, int cameragain, LabelType labeltype)
        {
            bool retVal = OracleLabelData.SaveLabelData(inspectLightAreas, minarea, stationid, labelitem, variationroi,
                innerradius, cameragain, labeltype);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public InspectionParamDefaults GeInspectionParamDefaults()
        {
            InspectionParamDefaults retVal = OracleLabelData.GeInspectionParamDefaults();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }

        public bool SaveInspectionParams(InspectionParams iparams)
        {
            bool retVal = OracleLabelData.SaveInspectionParams(iparams);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            return retVal;
        }
    }
}
