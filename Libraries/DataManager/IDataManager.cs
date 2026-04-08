using System.Data;
using System.Drawing;

namespace LVS3;

public interface IDataManager
{
    string ErrorDesription { get; set; }
    Delegates.SystemMessageHandler DataManagerSMHO { get; set; }
    Delegates.SystemMessageHandler DataManagerSMHL { get; set; }
    SystemMessageEventArgs DataManagerSMEA { get; set; }
    bool OpenConnection(Enums.DatabaseSchema dbs);
    bool SetReportFilePath(string path);
    string GetReportFilePath();
    string GetTMFileName(ModelOCMType type);
    List<TMParams> GetTMParams();
    (int f1, int f2, int f3, int f4) GetFilters(string filtersize);
    DataSet AuditLog(string username, DateTime datefrom, DateTime dateto);
    List<string> UserNames();
    List<FontSizesSegment> GetListFontSizes();
    bool DeleteTrainingData(string labelitem);
    bool LabelIsTrained(int stationid, string labelitem);
    bool InsertInspectionParamsVDEItem(VDEItem vi, int[] coords, int labelid);
    bool InspectionParamsUpdateOPZone(int id, VDEItem vdi, string tmp_folder);
    bool InspectionParamsUpdateOPZonesDeleteAll(int id);
    bool InspectionParamsUpdateVDEItemsDeleteAll(int labelid);
    List<VDEItem> InspectionParamsVDEItem(int labelid, List<FontSizesSegment> fontsizessegment, LabelType labeltype);
    string GetSample(string lpn);
    List<FontData> LoadFontDataItems();
    double GetInnerRadiusDefaultSmall();
    double GetInnerRadiusDefaultLarge();
    double GetConfidenceLevel();
    double GetInnerRadius(int stationid, string labelitem);
    string GetDebrisSizeLabel(int stationid, string labelitem);
    LabelType GetLabelType(int stationid, string labelitem);
    string GetLabelTypeAsString(int stationid, string labelitem);
    InspectionParams GeInspectionParams(int stationid, int labelid);
    int GetCameraGain(int stationid, string labelitem);
    List<OPZoneData> OPZoneData(int id, string basefolder, LabelType labeltype);
    List<LabelItemDataSetup> LabelDataItems(string lin, string reel_lpn);
    MedData LabelDataInspection(string lwo, string reel_lpn, string placeholder);
    int GetCountMeds(string lpn, int stationid);
    List<string> GetTrainingData(string lin, TrainingDataType tdt);
    List<string> SaveTrainingData(string lin, TrainingDataType tdt, List<string> td);
    string ApplicationSettingGet(string toget);
    string GetReportCompiler(string lpn);
    string GetReportSummary(string lpn);
    DateTime[] GetReportSummaryDates(string lpn);
    string GetCustomer(string labelitem);
    string GetLabelIssue(string lin, string lwo);
    DataSet GetSystemDevices();
    LabelItemAndVersion GoodREEL_LWO_LIN_VERSION_Data(string reel);
    string GetMeaning(Enums.ESigReason meaningid);
    List<ADGroupData> GetAllGroups();
    bool GetCompleteReport(string reel, string labelitem);
    List<PLCRegister> LoadPLCRegisters(int plc);
    List<PLCFailCode> LoadFailCodes();
    bool SaveAction(string message, string title, string lpn, string username, string method, string refersto, string userreason);
    bool SaveAction1(string message, string title, string lpn, string username, string method, string refersto, string operatottext, string userreason);
    bool SummaryDataInspectionStart(string reel_lpn, string label_item, string usersaving, int stationid, bool cancellinginspection);
    bool SaveSummaryDataStart(string reel_lpn, string label_item, string usersaving, int stationid, DateTime started_at);
    bool ClearResultData(string lpn, string lin);
    bool SummaryDataInspectionFinish(string reel_lpn, string label_item, string usersaving, int lblcount, int missing, int accept, int reject, int acceptop, int rejectop, string opsummary);
    bool SaveResultData(string lpn, string lin, string acceptedbyuser, string backingnumber, string selecteditems, string operatorinfo, string reasons, string additionalinfo, int labelindex);
    bool SaveLabelRegion(Bitmap labelvar, int stationid, string labelitem, bool inspectlightareas, int maxgray, int minarea);
    bool GetInspectLightAreas(int stationid, string labelitem);
    int[] GetVariationRegion(int stationid, string labelitem);
    string GetVDEContrastAsString(int stationid, string labelitem);
    int LabelID(int stationid, string labelitem);
    int ImageCount(VAMImageTypes vit);
    object GetFixtureID(int stationid, string labelitem, string basefolder);
    List<object> GetFixtureXY(int stationid, string labelitem, string basefolder);
    bool SaveVariationVAM(int stationid, string labelitem, string tmp_folder, string base_folder);
    bool SaveLabelData(bool inspectLightAreas, int minarea, int stationid, string labelitem, Bitmap variationroi, double innerradius, int cameragain, LabelType labeltype);
    InspectionParamDefaults GeInspectionParamDefaults();
    bool SaveInspectionParams(InspectionParams iparams);
}