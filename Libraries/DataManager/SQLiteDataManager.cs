using System.Drawing;
using System.Data;
using static LVS3.Delegates;
using static LVS3.Enums;

namespace LVS3
{
    public class SqLiteDataManager : IDataManager
    {
        public string ErrorDesription { get => SqLiteDatabase.ErrorDesription; set => SqLiteDatabase.ErrorDesription = value; }
        public SystemMessageHandler DataManagerSMHO { get; set; } = SqLiteDatabase.Smh;
        public SystemMessageHandler DataManagerSMHL { get; set; } = SqLiteLabelData.Smh;
        public SystemMessageEventArgs DataManagerSMEA { get; set; }
        public bool OpenConnection(DatabaseSchema dbs)
        {
            bool retVal = SqLiteDatabase.OpenSqLiteConnection();
            if (retVal == false)
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool SetReportFilePath(string path)
        {
            bool retVal = SqLiteDatabase.SetReportFilePath(path);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string GetReportFilePath()
        {
            string retVal = SqLiteDatabase.GetReportFilePath();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string GetTMFileName(ModelOCMType type)
        {
            string retVal = SqLiteDatabase.GetTmFileName(type);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<TMParams> GetTMParams()
        {
            List<TMParams> retVal = SqLiteDatabase.GetTmParams();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public (int f1, int f2, int f3, int f4) GetFilters(string filtersize)
        {
            (int f1, int f2, int f3, int f4) retVal = SqLiteDatabase.GetFilters(filtersize);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }



        public DataSet AuditLog(string username, DateTime datefrom, DateTime dateto)
        {
            DataSet retVal = SqLiteDatabase.AuditLog(username, datefrom, dateto);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<string> UserNames()
        {
            List<string> retVal = SqLiteDatabase.UserNames();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<FontSizesSegment> GetListFontSizes()
        {
            List<FontSizesSegment> retVal = SqLiteDatabase.GetListFontSizes();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public bool DeleteTrainingData(string labelitem)
        {
            bool retVal = SqLiteDatabase.DeleteTrainingData(labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool LabelIsTrained(int stationid, string labelitem)
        {
            bool retVal = SqLiteLabelData.LabelIsTrained(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool InsertInspectionParamsVDEItem(VDEItem vi, int[] coords, int labelid)
        {
            bool retVal = SqLiteLabelData.InsertInspectionParamsVdeItem(vi, coords, labelid);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public bool InspectionParamsUpdateOPZone(int id, VDEItem vdi, string tmpFolder)
        {
            bool retVal = SqLiteLabelData.InspectionParamsUpdateOpZone(id, vdi, tmpFolder);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool InspectionParamsUpdateOPZonesDeleteAll(int id)
        {
            bool retVal = SqLiteLabelData.InspectionParamsUpdateOpZonesDeleteAll(id);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool InspectionParamsUpdateVDEItemsDeleteAll(int labelid)
        {
            bool retVal = SqLiteLabelData.InspectionParamsUpdateVdeItemsDeleteAll(labelid);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<VDEItem> InspectionParamsVDEItem(int labelid, List<FontSizesSegment> fontsizessegment, LabelType labeltype)
        {
            List<VDEItem> retVal = SqLiteLabelData.InspectionParamsVdeItemsList(labelid, fontsizessegment, labeltype);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public string GetSample(string lpn)
        {
            string retVal = SqLiteDatabase.GetSample(lpn);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<FontData> LoadFontDataItems()
        {
            List<FontData> retVal = SqLiteLabelData.LoadFontDataItems();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public double GetInnerRadiusDefaultSmall()
        {
            double retVal = SqLiteLabelData.GetInnerRadiusDefaultSmall();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public double GetInnerRadiusDefaultLarge()
        {
            double retVal = SqLiteLabelData.GetInnerRadiusDefaultLarge();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public double GetConfidenceLevel()
        {
            double retVal = SqLiteLabelData.GetConfidenceLevel();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public double GetInnerRadius(int stationid, string labelitem)
        {//GetDebrisSize
            double retVal = SqLiteLabelData.GetInnerRadius(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string GetDebrisSizeLabel(int stationid, string labelitem)
        {
            string retVal = SqLiteLabelData.GetDebrisSizeLabel(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public LabelType GetLabelType(int stationid, string labelitem)
        {
            LabelType retVal = SqLiteLabelData.GetLabelType(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string GetLabelTypeAsString(int stationid, string labelitem)
        {
            string retVal = SqLiteLabelData.GetLabelTypeAsString(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public InspectionParams GeInspectionParams(int stationid, int labelid)
        {
            InspectionParams retVal = SqLiteLabelData.GeInspectionParams(stationid, labelid);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public int GetCameraGain(int stationid, string labelitem)
        {
            int retVal = SqLiteLabelData.GetCameraGain(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<OPZoneData> OPZoneData(int id, string basefolder, LabelType labeltype)
        {
            List<OPZoneData> retVal = SqLiteLabelData.OpZoneData(id, basefolder, labeltype);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<LabelItemDataSetup> LabelDataItems(string lin, string reelLpn)
        {
            List<LabelItemDataSetup> retVal = SqLiteDatabase.LabelDataItems(lin, reelLpn);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public MedData LabelDataInspection(string lwo, string reelLpn, string placeholder)
        {
            MedData retVal = SqLiteDatabase.LabelDataInspection(lwo, reelLpn, placeholder);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public int GetCountMeds(string lpn, int stationid)
        {
            int retVal = SqLiteLabelData.GetCountMeds(lpn, stationid);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public List<string> GetTrainingData(string lin, TrainingDataType tdt)
        {
            List<string> retVal = SqLiteDatabase.GetTrainingData(lin, tdt);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<string> SaveTrainingData(string lin, TrainingDataType tdt, List<string> td)
        {
            List<string> retVal = SqLiteDatabase.SaveTrainingData(lin, tdt, td);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string ApplicationSettingGet(string toget)
        {
            string retVal = SqLiteDatabase.ApplicationSettingGet(toget);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string GetReportCompiler(string lpn)
        {
            string retVal = SqLiteLabelData.GetReportCompiler(lpn);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string GetReportSummary(string lpn)
        {
            string retVal = SqLiteLabelData.GetReportSummary(lpn);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public DateTime[] GetReportSummaryDates(string lpn)
        {
            DateTime[] retVal = SqLiteLabelData.GetReportSummaryDates(lpn);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string GetCustomer(string labelitem)
        {
            string retVal = SqLiteLabelData.GetCustomer(labelitem);
            if (retVal == "")
                retVal = "Customer Not Found";
            //string retVal = "to be implemented: customer name";
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string GetLabelIssue(string lin, string lwo)
        {
            string retVal = SqLiteLabelData.GetLabelIssue(lin, lwo);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public DataSet GetSystemDevices()
        {
            DataSet retVal = SqLiteDatabase.GetSystemDevices();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public LabelItemAndVersion GoodREEL_LWO_LIN_VERSION_Data(string reel)
        {
            LabelItemAndVersion retVal = SqLiteDatabase.GoodREEL_LWO_LIN_VERSION_Data(reel);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string GetMeaning(ESigReason meaningid)
        {
            string retVal = SqLiteDatabase.GetMeaning(meaningid);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<ADGroupData> GetAllGroups()
        {
            List<ADGroupData> retVal = SqLiteDatabase.GetAllGroups();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool GetCompleteReport(string reel, string labelitem)
        {
            bool retVal = SqLiteLabelData.GetCompleteReport(reel, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<PLCRegister> LoadPLCRegisters(int plc)
        {
            List<PLCRegister> retVal = SqLiteDatabase.LoadPlcRegisters(plc);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<PLCFailCode> LoadFailCodes()
        {
            List<PLCFailCode> retVal = SqLiteDatabase.LoadFailCodes();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool SaveAction(string message, string title, string lpn, string username, string method, string refersto, string userreason)
        {
            bool retVal = SqLiteDatabase.SaveAction(message, title, lpn, username, method, refersto, userreason);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool SaveAction1(string message, string title, string lpn, string username, string method, string refersto, string operatottext, string userreason)
        {
            bool retVal = SqLiteDatabase.SaveAction1(message, title, lpn, username, method, refersto, operatottext, userreason);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public bool SummaryDataInspectionStart(string reelLpn, string labelItem, string usersaving, int stationid, bool cancellinginspection)
        {
            bool retVal = SqLiteLabelData.SummaryDataInspectionStart(reelLpn, labelItem, usersaving, stationid, cancellinginspection);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool SaveSummaryDataStart(string reelLpn, string labelItem, string usersaving, int stationid, DateTime startedAt)
        {
            bool retVal = SqLiteLabelData.SaveSummaryDataStart(reelLpn, labelItem, usersaving, stationid, startedAt);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool ClearResultData(string lpn, string lin)
        {
            bool retVal = SqLiteLabelData.ClearResultData(lpn, lin);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool SummaryDataInspectionFinish(string reelLpn, string labelItem, string usersaving, int lblcount, int missing, int accept, int reject, int acceptop, int rejectop, string opsummary)
        {
            bool retVal = SqLiteLabelData.SummaryDataInspectionFinish(reelLpn, labelItem, usersaving, lblcount, missing, accept, reject, acceptop, rejectop, opsummary);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool SaveResultData(string lpn, string lin, string acceptedbyuser, string backingnumber, string selecteditems, string operatorinfo, string reasons, string additionalinfo, int labelindex)
        {
            bool retVal = SqLiteLabelData.SaveResultData(lpn, lin, acceptedbyuser, backingnumber, selecteditems, operatorinfo, reasons, additionalinfo, labelindex);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool SaveLabelRegion(Bitmap labelvar, int stationid, string labelitem, bool inspectlightareas, int maxgray, int minarea)
        {
            bool retVal = SqLiteLabelData.SaveLabelRegion(labelvar, stationid, labelitem, inspectlightareas, maxgray, minarea);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool GetInspectLightAreas(int stationid, string labelitem)
        {
            bool retVal = SqLiteLabelData.GetInspectLightAreas(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public int[] GetVariationRegion(int stationid, string labelitem)
        {
            int[] retVal = SqLiteLabelData.GetVariationRegion(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public string GetVDEContrastAsString(int stationid, string labelitem)
        {
            string retVal = SqLiteLabelData.GetVdeContrastAsString(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public int LabelID(int stationid, string labelitem)
        {
            int retVal = SqLiteLabelData.LabelId(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public int ImageCount(VAMImageTypes vit)
        {
            int retVal = SqLiteLabelData.ImageCount(vit);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }


        public object GetFixtureID(int stationid, string labelitem, string basefolder)
        {
            object retVal = SqLiteLabelData.GetFixtureId(stationid, labelitem, basefolder);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public List<object> GetFixtureXY(int stationid, string labelitem, string basefolder)
        {
            List<object> retVal = SqLiteLabelData.GetFixtureXy(stationid, labelitem, basefolder);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool SaveVariationVAM(int stationid, string labelitem, string tmpFolder, string baseFolder)
        {
            bool retVal = SqLiteLabelData.SaveVariationVam(stationid, labelitem, tmpFolder, baseFolder);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool SaveLabelData(bool inspectLightAreas, int minarea, int stationid, string labelitem, Bitmap variationroi, double innerradius, int cameragain, LabelType labeltype)
        {
            bool retVal = SqLiteLabelData.SaveLabelData(inspectLightAreas, minarea, stationid, labelitem, variationroi, innerradius, cameragain, labeltype);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public InspectionParamDefaults GeInspectionParamDefaults()
        {
            InspectionParamDefaults retVal = SqLiteLabelData.GeInspectionParamDefaults();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

        public bool SaveInspectionParams(InspectionParams iparams)
        {
            bool retVal = SqLiteLabelData.SaveInspectionParams(iparams);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            return retVal;
        }

    }
}
