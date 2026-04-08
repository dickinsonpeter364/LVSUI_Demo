using System.Drawing;
using LVS3;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static LVS3.Delegates;
using static LVS3.Enums;
using Microsoft.Data.Sqlite;

namespace LVS3
{
    public class CapturingDataManager : IDataManager
    {
        private static string databaseFile = "c:/dev1sqlite/dev1.db";
        private static string sqliteConnectionString = $"Data Source={databaseFile};";

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
            using var stream = new FileStream(DataFolder + filename  + ".xml", FileMode.Create);
            ser.Serialize(stream, obj);

        }
        public CapturingDataManager()
        {
            if ( Directory.Exists(DataFolder) == false)
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
            string retVal = OracleDatabase.GetReportFilePath();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            Serialize(retVal);

            return retVal;
        }

        public string GetTMFileName(ModelOCMType type)
        {
            string retVal = OracleDatabase.GetTMFileName(type);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);

            return retVal;
        }

        public List<TMParams> GetTMParams()
        {
            List<TMParams> retVal = OracleDatabase.GetTMParams();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            Serialize(retVal);

            return retVal;
        }


        public (int f1, int f2, int f3, int f4) GetFilters(string filtersize)
        {
            (int f1, int f2, int f3, int f4) retVal = OracleDatabase.GetFilters(filtersize);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);
            return retVal;
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
            DataSet retVal = OracleDatabase.AuditLog(username, datefrom, dateto);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);

            return retVal;
        }

        public List<string> UserNames()
        {
            List<string> retVal = OracleDatabase.UserNames();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);

            return retVal;
        }

        public List<FontSizesSegment> GetListFontSizes()
        {
            List<FontSizesSegment> retVal = OracleDatabase.GetListFontSizes();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);

            return retVal;
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
            bool retVal = OracleLabelData.LabelIsTrained(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);
            return retVal;
        }

        public bool InsertInspectionParamsVDEItem(VDEItem vi, int[] coords, int labelid)
        {
            bool retVal = OracleLabelData.InsertInspectionParamsVDEItem(vi, coords, labelid);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);

            return retVal;
        }


        public bool InspectionParamsUpdateOPZone(int id, VDEItem vdi, string tmp_folder)
        {
            bool retVal = OracleLabelData.InspectionParamsUpdateOPZone(id, vdi, tmp_folder);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            Serialize(retVal);
            return retVal;
        }

        public bool InspectionParamsUpdateOPZonesDeleteAll(int id)
        {
            bool retVal = OracleLabelData.InspectionParamsUpdateOPZonesDeleteAll(id);
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
            string retVal = OracleDatabase.GetSample(lpn);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            
            Serialize(retVal);


            return retVal;
        }

        public List<FontData> LoadFontDataItems()
        {
            List<FontData> retVal = OracleLabelData.LoadFontDataItems();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            Serialize(retVal);


            return retVal;
        }

        public double GetInnerRadiusDefaultSmall()
        {
            double retVal = OracleLabelData.GetInnerRadiusDefaultSmall();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public double GetInnerRadiusDefaultLarge()
        {
            double retVal = OracleLabelData.GetInnerRadiusDefaultLarge();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);

            return retVal;
        }

        public double GetConfidenceLevel()
        {
            double retVal = OracleLabelData.GetConfidenceLevel();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }


        public double GetInnerRadius(int stationid, string labelitem)
        {
            //GetDebrisSize
            double retVal = OracleLabelData.GetInnerRadius(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);

            return retVal;
        }

        public string GetDebrisSizeLabel(int stationid, string labelitem)
        {
            string retVal = OracleLabelData.GetDebrisSizeLabel(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);

            return retVal;
        }

        public LabelType GetLabelType(int stationid, string labelitem)
        {
            LabelType retVal = OracleLabelData.GetLabelType(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            Serialize(retVal);

            return retVal;
        }

        public string GetLabelTypeAsString(int stationid, string labelitem)
        {
            string retVal = OracleLabelData.GetLabelTypeAsString(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public InspectionParams GeInspectionParams(int stationid, int labelid)
        {
            InspectionParams retVal = OracleLabelData.GeInspectionParams(stationid, labelid);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            Serialize(retVal);

            return retVal;
        }

        public int GetCameraGain(int stationid, string labelitem)
        {
            int retVal = OracleLabelData.GetCameraGain(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            Serialize(retVal);

            return retVal;
        }

        public List<OPZoneData> OPZoneData(int id, string basefolder, LabelType labeltype)
        {
            List<OPZoneData> retVal = OracleLabelData.OPZoneData(id, basefolder, labeltype);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);



            return retVal;
        }

        public List<LabelItemDataSetup> LabelDataItems(string lin, string reel_lpn)
        {
            List<LabelItemDataSetup> retVal = OracleDatabase.LabelDataItems(lin, reel_lpn);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            //of ApplicationDat0a.LabelItemDataSetup with the VDE_DATA_LIST and VDE_SEQUENCE_LIST stored in separate tables
            using var sqliteConn = new SqliteConnection(sqliteConnectionString);
            sqliteConn.Open();

            // Insert the label items
            foreach (var item in retVal)
            {
                string insertLabelItemSql =
                    $"INSERT INTO LABEL_ITEM (LIN, FIELD_TYPE, PLACE_HOLDER, VARIABLE_NAME, VDE_DATA, Repeat) VALUES (@lin, @fieldType, @placeHolder, @variableName, @vdeData, @repeat)";
                using var insertCmd = new SqliteCommand(insertLabelItemSql, sqliteConn);
                insertCmd.Parameters.AddWithValue("@lin", item.LIN);
                insertCmd.Parameters.AddWithValue("@fieldType", item.FIELD_TYPE);
                insertCmd.Parameters.AddWithValue("@placeHolder", item.PLACE_HOLDER);
                insertCmd.Parameters.AddWithValue("@variableName", item.VARIABLE_NAME);
                insertCmd.Parameters.AddWithValue("@vdeData", item.VDE_DATA);                
                insertCmd.Parameters.AddWithValue("@repeat", item.Repeat);
                insertCmd.ExecuteNonQuery();
                // Insert VDE data
                if (item.VDE_DATA_LIST.Count != 0)
                {
                    foreach (var vdeData in item.VDE_DATA_LIST)
                    {
                        string vdeDataSql = "INSERT INTO VDE_DATA_LIST (LABEL_ID, VALUE) VALUES ((SELECT ID FROM LABEL_ITEM WHERE LIN = @lin), @value)";
                        using var dataCmd = new SqliteCommand(vdeDataSql, sqliteConn);
                        dataCmd.Parameters.AddWithValue("@lin", item.LIN);
                        dataCmd.Parameters.AddWithValue("@value", vdeData);
                        dataCmd.ExecuteNonQuery();
                    }
                }

                // Insert VDE sequence
                if (item.VDE_SEQUENCE_LIST.Count != 0)
                {
                    foreach (var vdeSequence in item.VDE_SEQUENCE_LIST)
                    {
                        string vdeSequenceSql = "INSERT INTO VDE_SEQUENCE_LIST (LABEL_ID, VALUE) VALUES ((SELECT ID FROM LABEL_ITEM WHERE LIN = @lin), @value)";
                        using var vdeSequenceCmd = new SqliteCommand(vdeSequenceSql, sqliteConn);
                        vdeSequenceCmd.Parameters.AddWithValue("@lin", item.LIN);
                        vdeSequenceCmd.Parameters.AddWithValue("@value", vdeSequence);
                        vdeSequenceCmd.ExecuteNonQuery();
                    }
                }
            }

            sqliteConn.Close();
        


            return retVal;
        }

        public MedData LabelDataInspection(string lwo, string reel_lpn, string placeholder)
        {
            MedData retVal = OracleDatabase.LabelDataInspection(lwo, reel_lpn, placeholder);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
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
            int retVal = OracleLabelData.GetCountMeds(lpn, stationid);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }


        public List<string> GetTrainingData(string lin, TrainingDataType tdt)
        {
            List<string> retVal = OracleDatabase.GetTrainingData(lin, tdt);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal, (int)tdt);


            return retVal;
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
            string retVal = OracleDatabase.ApplicationSettingGet(toget);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            Serialize(retVal);

            return retVal;
        }

        public string GetReportCompiler(string lpn)
        {
            string retVal = OracleLabelData.GetReportCompiler(lpn);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public string GetReportSummary(string lpn)
        {
            string retVal = OracleLabelData.GetReportSummary(lpn);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public DateTime[] GetReportSummaryDates(string lpn)
        {
            DateTime[] retVal = OracleLabelData.GetReportSummaryDates(lpn);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public string GetCustomer(string labelitem)
        {
            string retVal = OracleLabelData.GetCustomer(labelitem);
            if (retVal == "")
                retVal = "Customer Not Found";
            //string retVal = "to be implemented: customer name";
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public string GetLabelIssue(string lin, string lwo)
        {
            string retVal = OracleLabelData.GetLabelIssue(lin, lwo);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }


        public DataSet GetSystemDevices()
        {
            DataSet retVal = OracleDatabase.GetSystemDevices();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }

            Serialize(retVal);

            return retVal;
        }

        public LabelItemAndVersion GoodREEL_LWO_LIN_VERSION_Data(string reel)
        {
            LabelItemAndVersion retVal = OracleDatabase.GoodREEL_LWO_LIN_VERSION_Data(reel);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public string GetMeaning(ESigReason meaningid)
        {
            string retVal = OracleDatabase.GetMeaning(meaningid);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public List<ADGroupData> GetAllGroups()
        {
            List<ADGroupData> retVal = OracleDatabase.GetAllGroups();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public bool GetCompleteReport(string reel, string labelitem)
        {
            bool retVal = OracleLabelData.GetCompleteReport(reel, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public List<PLCRegister> LoadPLCRegisters(int plc)
        {
            List<PLCRegister> retVal = OracleDatabase.LoadPLCRegisters(plc);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public List<PLCFailCode> LoadFailCodes()
        {
            List<PLCFailCode> retVal = OracleDatabase.LoadFailCodes();
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHO?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
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
            bool retVal = OracleLabelData.GetInspectLightAreas(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public int[] GetVariationRegion(int stationid, string labelitem)
        {
            int[] retVal = OracleLabelData.GetVariationRegion(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public string GetVDEContrastAsString(int stationid, string labelitem)
        {
            string retVal = OracleLabelData.GetVDEContrastAsString(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }


        public int LabelID(int stationid, string labelitem)
        {
            int retVal = OracleLabelData.LabelID(stationid, labelitem);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }

            Serialize(retVal);

            return retVal;
        }


        public int ImageCount(VAMImageTypes vit)
        {
            int retVal = OracleLabelData.ImageCount(vit);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }


        public object GetFixtureID(int stationid, string labelitem, string basefolder)
        {
            object retVal = OracleLabelData.GetFixtureID(stationid, labelitem, basefolder);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
        }

        public List<object> GetFixtureXY(int stationid, string labelitem, string basefolder)
        {
            List<object> retVal = OracleLabelData.GetFixtureXY(stationid, labelitem, basefolder);
            if (ErrorDesription.Trim() != "")
            {
                DataManagerSMEA = new SystemMessageEventArgs(ErrorDesription, "Data Access", (int)CriticalLevels.Red);
                DataManagerSMHL?.Invoke(DataManagerSMEA);
            }
            Serialize(retVal);


            return retVal;
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
