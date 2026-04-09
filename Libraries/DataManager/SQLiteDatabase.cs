using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Serilog;
using CONSTANTS;
using System.Drawing;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using static LVS3.Enums;
using static LVS3.Delegates;

namespace LVS3
{

    public static class SqLiteLabelData
    {
        public static SystemMessageHandler Smh;
        private static OracleConnection _conn1 = null;

        private static void NotifyError(string msg, string title, bool isError)
        {
            try
            {
                //raise event instead of displaying a dialog
                if (Smh != null)
                {
                    CriticalLevels cl = Enums.CriticalLevels.Black;
                    if (isError)
                        cl = Enums.CriticalLevels.Red;

                    SystemMessageEventArgs smea = new SystemMessageEventArgs(msg, title, (int)cl);
                    OracleDatabase.SMH(smea);
                }

            }
            catch (Exception ex)
            {
                string err = "notifyError() Err: " + ex.Message + "Originating error: " + msg;
                Log.Logger.Error(err);
            }
        }


        public static bool InsertInspectionParamsVdeItem(VDEItem vi, int[] coords, int labelid)
        {
            bool retVal = false;
            string sqlText = "";
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    VDEType vtype = VDEType.VDE;
                    if (vi.IsBarcode2D)
                        vtype = VDEType.BARCODE_2D;
                    else if (vi.IsBarcodeLinear)
                        vtype = VDEType.BARCODE_LINEAR;
                    else if (vi.IsMask)
                        vtype = VDEType.MASK;
                    else if (vi.IsOPZone)
                        vtype = VDEType.OP;

                    retVal = true;
                    //int filterMinSize = 0;
                    int t = coords[0];
                    int l = coords[1];
                    int b = coords[2];
                    int r = coords[3];
                    string bph = string.Join(",", vi.BarcodePlaceHolders);
                    string vdename = vi.VDEItemName;
                    string opzonename = vi.OpZoneName;
                    string nonvdedata = vi.BarcodeNonVDEData;
                    int segmentContrast = 0;
                    if (vi.tr == null)
                        segmentContrast = 0;
                    double charWidth = 0;
                    double charHeight = 0;
                    string strokeWidth = "";
                    if (vi.IsVDE)
                    {
                        charWidth = Convert.ToDouble(vi.tr.CharWidth);
                        charHeight = Convert.ToDouble(vi.tr.CharHeight);
                        strokeWidth = vi.tr?.StrokeWidth?.ToString() ?? "";
                        //filterMinSize = vi.FILTER_TO_USE;
                        //switch (vi.FILTER_TO_USE)
                        //{
                        //    case 1:
                        //        filterMinSize = vi.FILTER1;
                        //        break;
                        //    case 2:
                        //        filterMinSize = vi.FILTER2;
                        //        break;
                        //    case 3:
                        //        filterMinSize = vi.FILTER3;
                        //        break;
                        //    case 4:
                        //        filterMinSize = vi.FILTER4;
                        //        break;
                        //}
                    }
                    if (vtype == VDEType.VDE)
                    {
                        segmentContrast = vi.tr.SegmentContrast;
                        sqlText = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_VDE_ITEM_PARAMS (STATION_ID, LABEL_ID, PLACE_HOLDER, VDE_TYPE, TOP, LEFT, BOTTOM, RIGHT, FONT_NAME, BARCODE_NAME, ROTATE_ANGLE, BARCODE_PLACEHOLDERS, BARCODE_NON_VDE_DATA, VDE_NAME, OPZONE_NAME, CHARACTER_CONTRAST, PARTITION_METHOD, SEGMENT_CONTRAST, CHARACTER_WIDTH, CHARACTER_HEIGHT, STROKE_WIDTH) VALUES ({0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7}, '{8}', '{9}', {10}, '{11}', '{12}', '{13}', '{14}', {15}, '{16}', {17}, {18}, {19}, '{20}') ", Defaults.StationID, labelid, vi.Placeholder, (int)vtype, t, l, b, r, vi.FontName, vi.BarcodeName == null ? "" : vi.BarcodeName, vi.VDERotatedAngle, bph, nonvdedata, vdename, opzonename, vi.CharacterContrast, vi.tr.PartitionMethod, vi.tr.SegmentContrast, charWidth, charHeight, strokeWidth);
                    }
                    else
                        sqlText = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_VDE_ITEM_PARAMS (STATION_ID, LABEL_ID, PLACE_HOLDER, VDE_TYPE, TOP, LEFT, BOTTOM, RIGHT, FONT_NAME, BARCODE_NAME, ROTATE_ANGLE, BARCODE_PLACEHOLDERS, BARCODE_NON_VDE_DATA, VDE_NAME, OPZONE_NAME, CHARACTER_CONTRAST, SEGMENT_CONTRAST, CHARACTER_WIDTH, CHARACTER_HEIGHT, STROKE_WIDTH) VALUES( {0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7}, '{8}', '{9}', {10}, '{11}', '{12}', '{13}', '{14}', {15}, {16}, {17}, {18}, '{19}') ", Defaults.StationID, labelid, vi.Placeholder, (int)vtype, t, l, b, r, vi.FontName, vi.BarcodeName == null ? "" : vi.BarcodeName, vi.VDERotatedAngle, bph, nonvdedata, vdename, opzonename, vi.CharacterContrast, segmentContrast, charWidth, charHeight, strokeWidth);

                    OracleCommand oCmd = new OracleCommand(sqlText, conn);
                    int ret = oCmd.ExecuteNonQuery();
                    retVal = (ret == 1 ? true : false);
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("InsertInspectionParamsVDEItem() err: {0}. UPDATE|INSERT:SQL: {1}", ex.Message, sqlText);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static bool InspectionParamsUpdateVdeItemsDeleteAll(int labelid)
        {
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_VDE_ITEM_PARAMS WHERE LABEL_ID = {0} AND STATION_ID = {1} ", labelid, Defaults.StationID);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            bool retVal;
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    oCmd.ExecuteNonQuery();
                }
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("InspectionParamsUpdateVDEItemsDeleteAll() err: {0}.DELETE:SQL: {1}", ex.Message, sqlText);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static bool SaveResultData(string lpn, string lin, string acceptedbyuser, string backingnumber, string selecteditems, string operatorinfo, string reasons, string additionalinfo, int labelindex)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            bool retVal = false;
            try
            {
                if (_conn1 == null)
                    _conn1 = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                if (_conn1.State == ConnectionState.Open)
                {
                    string dtz = DateFormats.LocalTimeAndZone(false);

                    //string Sql = "INSERT INTO " + Defaults.DB_Schema + ".XX_INSPECTION_RESULT_DATA (REEL_LPN, LIN, ACCEPTED_USER, BACKING_NUMBER, SELECTED_ITEMS, INSPECTION_TIME, OPERATOR_INFO, REASONS, ADDITIONAL_INFO, LABEL_INDEX) ";
                    string sql = "INSERT INTO " + Defaults.DB_Schema + ".XX_INSPECTION_RESULT_DATA (REEL_LPN, LIN, ACCEPTED_USER, BACKING_NUMBER, SELECTED_ITEMS, DTZ, OPERATOR_INFO, REASONS, ADDITIONAL_INFO, LABEL_INDEX, STATION_ID) ";
                    sql = sql + string.Format("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10})", lpn, lin, acceptedbyuser, backingnumber, selecteditems, dtz, operatorinfo, reasons, additionalinfo, labelindex, Defaults.StationID);
                    oCmd = new OracleCommand(sql, _conn1);
                    oCmd.CommandType = CommandType.Text;
                    oCmd.ExecuteNonQuery();
                    retVal = true;
                }
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "SaveResultData() err: " + ex.Message;
                NotifyError(err, "Label Inspection", true);
            }
            return retVal;
        }

        public static bool ClearResultData(string lpn, string lin)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            bool retVal;
            try
            {
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                if (conn.State == ConnectionState.Open)
                {
                    string sql = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_INSPECTION_RESULT_DATA WHERE REEL_LPN = '{0}' AND LIN = '{1}'", lpn, lin);
                    oCmd = new OracleCommand(sql, conn);
                    oCmd.CommandType = CommandType.Text;
                    oCmd.ExecuteNonQuery();
                    retVal = true;
                }
                else
                    throw new Exception(Environment.NewLine + "COSMOS connection has been disconnected" + Environment.NewLine + "Cannot update inspection status to complete");
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "ClearResultData() err: " + ex.Message;
                NotifyError(err, "Label Inspection", true);
            }
            return retVal;
        }


        //public static bool SaveLabelConfiguration(string LABEL_ITEM, string REEL_LPN, int varThreshold, int countImages, int clutter, DateTime startedAt, string username)
        //{
        //    OracleCommand oCmd = new OracleCommand();
        //    OracleDataAdapter da = new OracleDataAdapter(oCmd);
        //    bool retVal;
        //    try
        //    {
        //        OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            if (username.Trim() == "")
        //                username = "test_user";
        //            string startDate = " TO_DATE('" + startedAt.ToString("MM/dd/yyyy 00:00:00") + "', 'mm/dd/yyyy HH24:MI:SS')";
        //            string endDate = " TO_DATE(TO_CHAR(SYSDATE,'DD-MON-YYYY HH24:MI:SS'),'DD-MON-YYYY HH24:MI:SS')";

        //            string Sql = "UPDATE " + Defaults.DB_Schema + ".XX_LABEL_CONFIGURATION SET ";
        //            Sql = Sql + string.Format("LIN='{0}', REEL_USED='{1}', VARIATION_CONTRAST= {2}, VARIATION_IMAGE_COUNT={3}, STARTED_AT= {4}, COMPLETE_AT={5}, CLUTTER= {6}, USER_NAME= '{7}' WHERE LIN = '{0}'", LABEL_ITEM, REEL_LPN, varThreshold, countImages, startDate, endDate, clutter, username);
        //            oCmd = new OracleCommand(Sql, conn);
        //            oCmd.CommandType = CommandType.Text;
        //            da.UpdateCommand = oCmd;
        //            int res = -1;
        //            try { res = oCmd.ExecuteNonQuery(); } catch { res = -1; }
        //            if (res <= 0)
        //            {
        //                Sql = "INSERT INTO " + Defaults.DB_Schema + ".XX_LABEL_CONFIGURATION (LIN, REEL_USED, VARIATION_CONTRAST, VARIATION_IMAGE_COUNT, STARTED_AT, COMPLETE_AT, CLUTTER, USER_NAME) ";
        //                Sql = Sql + string.Format("VALUES ('{0}', '{1}', {2}, {3}, {4}, {5} , {6} , '{7}')", LABEL_ITEM, REEL_LPN, varThreshold, countImages, startDate, endDate, clutter, username);
        //                oCmd = new OracleCommand(Sql, conn);
        //                oCmd.CommandType = CommandType.Text;
        //                oCmd.ExecuteNonQuery();
        //            }
        //            retVal = true;
        //        }
        //        else
        //            throw new Exception(Environment.NewLine + "COSMOS connection has been disconnected" + Environment.NewLine + "Cannot update inspection status to complete");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        retVal = false;
        //        string err = "SaveLabelConfiguration() err: " + ex.Message;
        //        notifyError(err, "Label Inspection", true);
        //    }
        //    return retVal;
        //}

        public static bool SaveSummaryDataStart(string reelLpn, string labelItem, string usersaving, int stationid, DateTime startedAt)
        {//SaveSummaryDataInspectionStart
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            bool retVal;
            try
            {
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                if (conn.State == ConnectionState.Open)
                {
                    if (usersaving.Trim() == "")
                        usersaving = "test_user";
                    string startDate = " TO_DATE('" + startedAt.ToString("MM/dd/yyyy HH:mm:00") + "', 'mm/dd/yyyy HH24:MI:SS')";
                    string endDate = " TO_DATE(TO_CHAR(SYSDATE,'DD-MON-YYYY HH24:MI:SS'),'DD-MON-YYYY HH24:MI:SS')";

                    string sql = "UPDATE " + Defaults.DB_Schema + ".XX_INSPECTION_REPORT_SUMMARY SET ";
                    sql = sql + string.Format("REEL_LPN='{0}', LABEL_ITEM ='{1}', OPERATOR_NAME_SETUP='{2}', MACHINE_NUMBER= {3}, TRAIN_LABEL_START= {4}, TRAIN_LABEL_FINISH={5} WHERE REEL_LPN = '{0}' AND LABEL_ITEM = '{1}'", reelLpn, labelItem, usersaving, stationid, startDate, endDate);
                    oCmd = new OracleCommand(sql, conn);
                    oCmd.CommandType = CommandType.Text;
                    da.UpdateCommand = oCmd;
                    int res = -1;
                    try { res = oCmd.ExecuteNonQuery(); } catch { res = -1; }
                    if (res <= 0)
                    {
                        sql = "INSERT INTO " + Defaults.DB_Schema + ".XX_INSPECTION_REPORT_SUMMARY (REEL_LPN, LABEL_ITEM, OPERATOR_NAME_SETUP, MACHINE_NUMBER, TRAIN_LABEL_START, TRAIN_LABEL_FINISH) ";
                        sql = sql + string.Format("VALUES ('{0}', '{1}', '{2}', {3}, {4}, {5})", reelLpn, labelItem, usersaving, stationid, startDate, endDate);
                        oCmd = new OracleCommand(sql, conn);
                        oCmd.CommandType = CommandType.Text;
                        oCmd.ExecuteNonQuery();
                    }
                    retVal = true;
                }
                else
                    throw new Exception(Environment.NewLine + "COSMOS connection has been disconnected" + Environment.NewLine + "Cannot update inspection completion status");
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "SaveSummaryDataStart() err: " + ex.Message;
                NotifyError(err, "Label Inspection", true);
            }
            return retVal;
        }

        public static bool SummaryDataInspectionStart(string reelLpn, string labelItem, string userstarting, int stationid, bool cancellinginspection)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            bool retVal;
            try
            {
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                if (conn.State == ConnectionState.Open)
                {
                    string startInspectionDate = "";

                    //startDate =" TO_DATE('" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:00") + "', 'mm/dd/yyyy HH24:MI:SS')";
                    if (cancellinginspection)
                        startInspectionDate = "''";
                    else
                        startInspectionDate = " TO_DATE(TO_CHAR(SYSDATE,'DD-MON-YYYY HH24:MI:SS'),'DD-MON-YYYY HH24:MI:SS')";

                    if (labelItem.Contains("Issue"))
                    {
                        int endOfLin = labelItem.IndexOf("Issue");
                        labelItem = labelItem.Substring(0, endOfLin - 1).Trim();
                    }

                    string sql = "UPDATE " + Defaults.DB_Schema + ".XX_INSPECTION_REPORT_SUMMARY SET ";
                    sql = sql + string.Format("INSPECTION_LABEL_START= {0}, OPERATOR_NAME_INSPECTION = '{1}' WHERE REEL_LPN = '{2}' AND LABEL_ITEM = '{3}' AND MACHINE_NUMBER = {4} ", startInspectionDate, userstarting, reelLpn, labelItem, stationid);
                    oCmd = new OracleCommand(sql, conn);
                    oCmd.CommandType = CommandType.Text;
                    da.UpdateCommand = oCmd;
                    int res = -1;
                    try { res = oCmd.ExecuteNonQuery(); } catch (Exception ex) { string err = ex.Message; }
                    retVal = true;
                }
                else
                    throw new Exception(Environment.NewLine + "COSMOS connection has been disconnected" + Environment.NewLine + "Cannot update inspection status to complete");
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "SummaryDataInspectionStart() err: " + ex.Message;
                NotifyError(err, "Label Inspection", true);
            }
            return retVal;
        }

        public static bool SummaryDataInspectionFinish(string reelLpn, string labelItem, string usersaving, int lblcount, int missing, int accept, int reject, int acceptop, int rejectop, string opsummary)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            bool retVal;
            try
            {
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                if (conn.State == ConnectionState.Open)
                {
                    if (labelItem.Contains("Issue"))
                    {
                        int endOfLin = labelItem.IndexOf("Issue");
                        labelItem = labelItem.Substring(0, endOfLin - 1).Trim();
                    }

                    string endDate = " TO_DATE(TO_CHAR(SYSDATE,'DD-MON-YYYY HH24:MI:SS'),'DD-MON-YYYY HH24:MI:SS')";
                    string sql = "UPDATE " + Defaults.DB_Schema + ".XX_INSPECTION_REPORT_SUMMARY SET ";
                    sql = sql + string.Format("OPERATOR_NAME_INSPECTION='{0}', INSPECTION_LABEL_FINISH= {1}, LABEL_COUNT = {2},  ", usersaving, endDate, lblcount);
                    sql = sql + string.Format("COUNT_MISSING = {0}, COUNT_ACCEPTED = {1}, COUNT_REJECTED = {2}, COUNT_ACCEPTED_OP = {3}, COUNT_REJECTED_OP = {4}, OPERATOR_REPORT_SUMMARY = '{5}' WHERE REEL_LPN = '{6}' AND LABEL_ITEM = '{7}'", missing, accept, reject, acceptop, rejectop, opsummary, reelLpn, labelItem);

                    oCmd = new OracleCommand(sql, conn);
                    oCmd.CommandType = CommandType.Text;
                    da.UpdateCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    retVal = true;
                }
                else
                    throw new Exception(Environment.NewLine + "COSMOS connection has been disconnected" + Environment.NewLine + "Cannot update inspection status to complete");
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "SummaryDataInspectionFinish() err: " + ex.Message;
                NotifyError(err, "Label Inspection", true);
            }
            return retVal;
        }

        public static bool InspectionParamsUpdateOpZonesDeleteAll(int id)
        {
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS_ZONES WHERE ID = {0} ", id);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            bool retVal;
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    oCmd.ExecuteNonQuery();
                }
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("InspectionParamsUpdateOPZonesDeleteAll() err: {0}. DELETE:SQL: {1}", ex.Message, sqlText);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static bool InspectionParamsUpdateOpZone(int id, VDEItem vdi, string tmpFolder)
        {
            bool retVal = false;
            Byte[] blob1 = new Byte[0];
            try
            {
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                if (conn.State == ConnectionState.Open)
                {
                    retVal = true;
                    int t = vdi.ODP.OPZoneVARRegion[0];
                    int l = vdi.ODP.OPZoneVARRegion[1];
                    int b = vdi.ODP.OPZoneVARRegion[2];
                    int r = vdi.ODP.OPZoneVARRegion[3];

                    int x = Convert.ToInt32(vdi.ODP.OPZoneFixtureX);
                    int y = Convert.ToInt32(vdi.ODP.OPZoneFixtureY);
                    string zonename = vdi.VDEItemName;
                    vdi.OpZoneName = vdi.VDEItemName;
                    string tmpfilename = Path.Combine(tmpFolder, "fix.shm");
                    System.IO.File.Delete(tmpfilename);
                    /* TODO: Replace HOperatorSet.WriteShapeModel */ //vdi.ODP.OPZoneIDFixture, tmpfilename);
                    FileStream fs = new FileStream(tmpfilename, FileMode.OpenOrCreate, FileAccess.Read);
                    Byte[] blob = new Byte[fs.Length];
                    fs.Read(blob, 0, blob.Length);
                    fs.Close();

                    //string mserFields = "MSER_MIN_GRAY, MSER_MAX_GRAY, MSER_DELTA, MSER_MIN_DIVERSITY, MSER_MAX_VARIATION, DEBRIS_MIN_SIZE_VAR, DEBRIS_MIN_SIZE_MSER, AREA_LARGE, DARK_TO_LIGHT_OFFSET, DILATION_CIRCLE,MAX_GRAY_LIMIT,SEGMENT_CONTRAST";
                    //string sqlText = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_LABEL_ITEMS_ZONES (ID, TOP, LEFT, BOTTOM, RIGHT, FIXTURE_X, FIXTURE_Y, ZONE_NAME," + mserFields + ") VALUES({0},{1},{2},{3},{4},{5},{6},'{7}',{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}) ", id, t, l, b, r, X, Y, zonename, vdi.MSER.DarkMinGray, vdi.MSER.DarkMaxGray, vdi.MSER.DarkDelta, vdi.MSER.DarkMinDiversity, vdi.MSER.DarkMaxVariation, vdi.MSER.DarkMinSizeVAR, vdi.MSER.DarkMinSizeMSER, vdi.MSER.DarkAreaLarge, 0, vdi.MSER.DarkDilationCircle, vdi.MSER.DarkMaxGrayLimit, vdi.MSER.DarkSegmentContrast);
                    string dbFields = "DEBRIS_MIN_SIZE_VAR, AREA_LARGE, MAX_GRAY_LIMIT";
                    string sqlText = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_LABEL_ITEMS_ZONES (ID, TOP, LEFT, BOTTOM, RIGHT, FIXTURE_X, FIXTURE_Y, ZONE_NAME, " + dbFields + ") VALUES({0},{1},{2},{3},{4},{5},{6},'{7}',{8},{9},{10}) ", id, t, l, b, r, x, y, zonename, vdi.DarkMinSizeVAR, 999999, vdi.DarkMaxGray);
                    OracleCommand oCmd = new OracleCommand(sqlText, conn);
                    int ret = oCmd.ExecuteNonQuery();
                    if (ret == 1)
                    {

                        string sql = "UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS_ZONES SET FIXTURE_ID = :FIXTURE_ID WHERE ID = : LI_ID AND FIXTURE_X = : FIXX AND FIXTURE_Y = : FIXY ";
                        using (OracleCommand cmd = new OracleCommand(sql, conn))
                        {
                            cmd.Parameters.Add("FIXTURE_ID", OracleDbType.Blob).Value = blob1;
                            cmd.Parameters.Add("LI_ID", OracleDbType.Decimal, 1).Value = id;
                            cmd.Parameters.Add("FIXX", OracleDbType.Decimal, 1).Value = x;
                            cmd.Parameters.Add("FIXY", OracleDbType.Decimal, 1).Value = y;
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message + ". SQL: " + sql);
                            }
                        }
                        sql = "UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS_ZONES SET FIXTURE_ID = :FIXTURE_ID WHERE ID = : LI_ID AND FIXTURE_X = : FIXX AND FIXTURE_Y = : FIXY ";
                        using (OracleCommand cmd = new OracleCommand(sql, conn))
                        {
                            cmd.Parameters.Add("FIXTURE_ID", OracleDbType.Blob).Value = blob;
                            cmd.Parameters.Add("LI_ID", OracleDbType.Decimal, 1).Value = id;
                            cmd.Parameters.Add("FIXX", OracleDbType.Decimal, 1).Value = x;
                            cmd.Parameters.Add("FIXY", OracleDbType.Decimal, 1).Value = y;
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message + ". SQL: " + sql);
                            }
                        }
                        File.Delete(tmpfilename);
                    }
                    if (ret == 1)
                    {
                        tmpfilename = Path.Combine(tmpFolder, vdi.VDEItemName + "_OPZONE_VAM.vam");
                        File.Delete(tmpfilename);
                        /* TODO: Replace HOperatorSet.WriteVariationModel */ //vdi.ODP.OPZoneIDVar, tmpfilename);
                        fs = new FileStream(tmpfilename, FileMode.OpenOrCreate, FileAccess.Read);
                        blob = new Byte[fs.Length];
                        fs.Read(blob, 0, blob.Length);
                        fs.Close();
                        string sql = "UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS_ZONES SET VARIATION_VAM = :VARIATION_VAM WHERE ID = : LI_ID AND FIXTURE_X = : FIXX AND FIXTURE_Y = : FIXY ";
                        using (OracleCommand cmd = new OracleCommand(sql, conn))
                        {
                            cmd.Parameters.Add("VARIATION_VAM", OracleDbType.Blob).Value = blob1;
                            cmd.Parameters.Add("LI_ID", OracleDbType.Decimal, 1).Value = id;
                            cmd.Parameters.Add("FIXX", OracleDbType.Decimal, 1).Value = x;
                            cmd.Parameters.Add("FIXY", OracleDbType.Decimal, 1).Value = y;
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message + ". SQL: " + sql);
                            }
                        }
                        sql = "UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS_ZONES SET VARIATION_VAM = :VARIATION_VAM WHERE ID = : LI_ID AND FIXTURE_X = : FIXX AND FIXTURE_Y = : FIXY ";
                        using (OracleCommand cmd = new OracleCommand(sql, conn))
                        {
                            cmd.Parameters.Add("VARIATION_VAM", OracleDbType.Blob).Value = blob;
                            cmd.Parameters.Add("LI_ID", OracleDbType.Decimal, 1).Value = id;
                            cmd.Parameters.Add("FIXX", OracleDbType.Decimal, 1).Value = x;
                            cmd.Parameters.Add("FIXY", OracleDbType.Decimal, 1).Value = y;
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message + ". SQL: " + sql);
                            }
                        }
                        File.Delete(tmpfilename);
                    }
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("InspectionParamsUpdateOPZone() err: {0}", ex.Message + ". " + ex.InnerException?.InnerException.InnerException);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static List<VDEItem> InspectionParamsVdeItemsList(int labelid, List<FontSizesSegment> fontsizessegment, LabelType labeltype)
        {
            List<VDEItem> retVal = new List<VDEItem>();
            SQLiteCommand oCmd;
            SQLiteDataAdapter da;
            string sSql = "";
            try
            {
                SQLiteConnection conn = SqLiteDatabase.GetSqLiteConnection(SqLiteDatabase.ConnectionString);
                sSql = string.Format("SELECT * FROM " + "XX_VDE_ITEM_PARAMS WHERE LABEL_ID = {0} AND STATION_ID = {1}", labelid, Defaults.StationID);
                oCmd = new SQLiteCommand(sSql, conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new SQLiteDataAdapter(oCmd);
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        VDEType vdeType;
                        vdeType = (VDEType)Convert.ToInt32(dr["VDE_TYPE"].ToString());
                        int row1 = Convert.ToInt32(dr["TOP"].ToString());
                        int col1 = Convert.ToInt32(dr["LEFT"].ToString());
                        int row2 = Convert.ToInt32(dr["BOTTOM"].ToString());
                        int col2 = Convert.ToInt32(dr["RIGHT"].ToString());
                        int[] vdecoord = new int[] { row1, col1, row2, col2 };
                        string fontname = dr["FONT_NAME"].ToString();
                        int angle = Convert.ToInt32(dr["ROTATE_ANGLE"].ToString());
                        string barcodename = dr["BARCODE_NAME"].ToString();
                        string barcodePlaceholders = dr["BARCODE_PLACEHOLDERS"].ToString();
                        string barcodeNonVdeData = dr["BARCODE_NON_VDE_DATA"].ToString();
                        string vdename = dr["VDE_NAME"].ToString();
                        string opzonename = dr["OPZONE_NAME"].ToString();
                        string placeHolder = dr["PLACE_HOLDER"].ToString();
                        VDEItem vdi = new VDEItem(labelid, placeHolder, vdecoord, angle, vdeType, fontname, barcodename, vdename);
                        vdi.OpZoneName = opzonename;
                        if (vdeType == VDEType.VDE)
                        {
                            //vdi.MSER = OracleDatabase.GetMSERParams(labeltype);
                            vdi.CharacterContrast = Convert.ToInt32(dr["CHARACTER_CONTRAST"].ToString());
                            vdi.tr = new TReader();
                            vdi.tr.InitReader();
                            vdi.tr.SegmentContrast = Convert.ToInt32(dr["SEGMENT_CONTRAST"].ToString());
                            vdi.tr.CharHeight = Convert.ToInt32(dr["CHARACTER_HEIGHT"].ToString());
                            vdi.tr.CharWidth = Convert.ToInt32(dr["CHARACTER_WIDTH"].ToString());
                            vdi.tr.StrokeWidth = dr["STROKE_WIDTH"].ToString();
                            vdi.tr.PartitionMethod = dr["PARTITION_METHOD"].ToString();
                        }
                        if (vdeType == VDEType.BARCODE_2D)
                        {
                            vdi.BarcodePlaceHolders = new List<string>(barcodePlaceholders.Split(','));
                            vdi.BarcodeNonVDEData = barcodeNonVdeData;
                        }
                        //if (vdeType == VDEType.OP)
                        //    vdi.MSER = OracleDatabase.GetMSERParams(labeltype);
                        retVal.Add(vdi);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                string err = string.Format("InspectionParamsVDEItem() err: {0}\nSQL: {1}", ex.Message, sSql);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static List<OPZoneData> OpZoneData(int id, string basefolder, LabelType labeltype)
        {
            List<OPZoneData> retVal = new List<OPZoneData>();
            string s1 = "SELECT FIXTURE_ID, VARIATION_VAM, TOP, LEFT, BOTTOM, RIGHT, FIXTURE_X, FIXTURE_Y, ZONE_NAME, DEBRIS_MIN_SIZE_VAR, AREA_LARGE, MAX_GRAY_LIMIT FROM ";
            string sqlText = string.Format(s1 + Defaults.DB_Schema + ".XX_LABEL_ITEMS_ZONES WHERE ID = {0}", id);
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            OracleCommand cmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    using (OracleDataReader odr = cmd.ExecuteReader())
                    {
                        while (odr.Read())
                        {
                            OPZoneData ozd = new OPZoneData(id);
                            Byte[] blob = null;
                            string tempFile;
                            if (!odr.IsDBNull(0))
                            {
                                OracleBlob blob1 = odr.GetOracleBlob(0);
                                blob = new Byte[blob1.Length];
                                int i = blob1.Read(blob, 0, System.Convert.ToInt32(blob.Length));
                                tempFile = Path.Combine(basefolder, "tmpID.shm");
                                File.Delete(tempFile);
                                using (FileStream fs = new FileStream(tempFile, FileMode.Create, System.IO.FileAccess.Write))
                                {
                                    MemoryStream ms = new MemoryStream(blob);
                                    byte[] bytes = new byte[ms.Length];
                                    ms.Read(bytes, 0, (int)ms.Length);
                                    fs.Write(bytes, 0, bytes.Length);
                                    ms.Close();
                                    if (File.Exists(tempFile))
                                    {
                                        /* TODO: Replace HOperatorSet.ReadShapeModel */ //tempFile, out object fixtureid);
                                        // TODO: load FIXTURE_ID
                                        
                                    }
                                }
                                File.Delete(tempFile);
                            }
                            blob = null;
                            if (!odr.IsDBNull(1))
                            {
                                OracleBlob blob1 = odr.GetOracleBlob(1);
                                blob = new Byte[blob1.Length];
                                int i = blob1.Read(blob, 0, System.Convert.ToInt32(blob.Length));
                                tempFile = Path.Combine(basefolder, "tmpVAM.vam");
                                File.Delete(tempFile);
                                using (FileStream fs = new FileStream(tempFile, FileMode.Create, System.IO.FileAccess.Write))
                                {
                                    MemoryStream ms = new MemoryStream(blob);
                                    byte[] bytes = new byte[ms.Length];
                                    ms.Read(bytes, 0, (int)ms.Length);
                                    fs.Write(bytes, 0, bytes.Length);
                                    ms.Close();
                                    if (File.Exists(tempFile))
                                    {
                                        /* TODO: Replace HOperatorSet.ReadVariationModel */ //tempFile, out object vam);
                                        // TODO: load VARIATION_VAM
                                        
                                    }
                                }
                                File.Delete(tempFile);
                            }
                            // FIXTURE_ID, VARIATION_VAM, TOP, LEFT, BOTTOM, RIGHT, FIXTURE_X, FIXTURE_Y, ZONE_NAME, DEBRIS_MIN_SIZE_VAR, AREA_LARGE, MAX_GRAY_LIMIT

                            ozd.OPZONE_TOP = odr.GetInt32(2);
                            ozd.OPZONE_LEFT = odr.GetInt32(3);
                            ozd.OPZONE_BOTTOM = odr.GetInt32(4);
                            ozd.OPZONE_RIGHT = odr.GetInt32(5);
                            ozd.FIXTURE_X = Convert.ToInt32(odr.GetString(6));
                            ozd.FIXTURE_Y = Convert.ToInt32(odr.GetString(7));
                            ozd.NAME = odr.GetString(8);
                            ozd.DarkMinSizeVAR = odr.GetInt32(9);
                            ozd.MAX_GRAY = odr.GetInt32(11);
                            ozd.DarkMaxGray = odr.GetInt32(11);
                            retVal.Add(ozd);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                string err = string.Format("GetOPZoneData() err: {0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static object GetFixtureId(int stationid, string labelitem, string basefolder)
        {
            object retVal = null;
            string sqlText = string.Format("SELECT FIXTURE_ID FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE STATION_ID = {0} AND LABEL_ITEM = '{1}' ", stationid, labelitem);
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            OracleCommand cmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        Byte[] blob = null;
                        if (!dr.IsDBNull(0))
                        {
                            OracleBlob blob1 = dr.GetOracleBlob(0);
                            blob = new Byte[blob1.Length];
                            int i = blob1.Read(blob, 0, System.Convert.ToInt32(blob.Length));

                            string tempFile = Path.Combine(basefolder, "FixtureModel.fix");
                            File.Delete(tempFile);
                            using (FileStream fs = new FileStream(tempFile, FileMode.Create, System.IO.FileAccess.Write))
                            {
                                MemoryStream ms = new MemoryStream(blob);
                                byte[] bytes = new byte[ms.Length];
                                ms.Read(bytes, 0, (int)ms.Length);
                                fs.Write(bytes, 0, bytes.Length);
                                ms.Close();
                                // TODO: ReadShapeModel/GetShapeModelContours without HALCON
                            }
                            File.Delete(tempFile);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                string err = string.Format("GetFixtureData() err: {0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static List<object> GetFixtureXy(int stationid, string labelitem, string basefolder)
        {
            List<object> retVal = new List<object>();
            string sqlText = string.Format("SELECT FIXTURE_X, FIXTURE_Y FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE STATION_ID = {0} AND LABEL_ITEM = '{1}' ", stationid, labelitem);
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            OracleDataAdapter da;
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd = new OracleCommand(sqlText, conn);
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    da = new OracleDataAdapter(oCmd);
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        object x = Convert.ToInt32(dr["FIXTURE_X"].ToString());
                        object y = Convert.ToInt32(dr["FIXTURE_Y"].ToString());
                        retVal.Add(x);
                        retVal.Add(y);
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                string err = string.Format("GetFixtureXY() err: {0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static bool LabelIsTrained(int stationid, string labelitem)
        {
            bool retVal = false;
            string sqlText = string.Format("SELECT ID FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE STATION_ID = {0} AND LABEL_ITEM = '{1}' ", stationid, labelitem);
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            OracleDataAdapter da;
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd = new OracleCommand(sqlText, conn);
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    da = new OracleDataAdapter(oCmd);
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 1)
                        retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("LabelIsTrained() err: {0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static string GetLabelIssue(string lin, string lwo)
        {
            string retVal = "";

            //return retVal;
            //string sqlText = string.Format("SELECT DISTINCT FIELD FROM " + Defaults.DB_Schema + ".xx_variable_data_extract_vw WHERE CLAUSE_FIELD = '{0}' ", lpn);
            string sqlText = string.Format("select wdj.attribute10 as ISSUE_NUMBER from apps.wip_entities we, apps.wip_discrete_jobs wdj where 1=1 and we.wip_entity_id = wdj.wip_entity_id and we.wip_entity_name = '{0}'", lwo);
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            OracleDataAdapter da;
            DataSet ds;
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    ds = new DataSet();
                    da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        retVal = dr["ISSUE_NUMBER"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = string.Format("GetCustomer() err: {0}. SQL: {1}", ex.Message, sqlText);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }


        public static string GetCustomer(string labelitem)
        {
            string retVal = "";
            //string sqlText = string.Format("SELECT DISTINCT SPONSOR_PARTY_NAME FROM " + Defaults.DB_Schema + ".xx_variable_data_extract_vw WHERE reel_lpn = '{0}' ", lpn);
            //string sqlText = string.Format("SELECT distinct(Customer) FROM apps.xxac_vde_v WHERE LABEL_ITEM = '{0}'", labelitem);
            //select distinct Customer, label_item from apps.xxac_vde_v where label_item = 'L10002361'
            string sqlText = string.Format("SELECT distinct(CUSTOMER) FROM apps.xxac_vde_v WHERE LABEL_ITEM = '{0}'", labelitem);
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            OracleDataAdapter da;
            DataSet ds;
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    ds = new DataSet();
                    da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        retVal = dr["CUSTOMER"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = string.Format("GetCustomer() err: {0}. SQL: {1}", ex.Message, sqlText);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static int GetCountMeds(string lpn, int stationid)
        {
            int retVal = 0;
            string sqlText = string.Format("SELECT COUNT(label_item) AS COUNTED FROM " + Defaults.DB_Schema + ".xx_variable_data_extract_vw WHERE reel_lpn = '{0}' ", lpn);
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            OracleDataAdapter da;
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        retVal = Convert.ToInt32(dr["COUNTED"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = 0;
                string err = string.Format("GetCountMeds() err: {0}. SQL: {1}", ex.Message, sqlText);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static List<FontData> LoadFontDataItems()
        {
            List<FontData> retVal = new List<FontData>();
            string sqlText = "SELECT POINT_SIZE, PIXEL_HEIGHT, CHARACTER_WIDTH, CHARACTER_HEIGHT, STROKE_WIDTH FROM " + Defaults.DB_Schema + ".XX_FONT_DATA ORDER BY PIXEL_HEIGHT ASC";
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            OracleDataAdapter da;
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd = new OracleCommand(sqlText, conn);
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    da = new OracleDataAdapter(oCmd);
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            FontData fd = new FontData();
                            fd.POINT_SIZE = Convert.ToInt32(dr["POINT_SIZE"].ToString());
                            fd.PIXEL_HEIGHT = Convert.ToInt32(dr["PIXEL_HEIGHT"].ToString());
                            fd.CHARACTER_WIDTH = Convert.ToInt32(dr["CHARACTER_WIDTH"].ToString());
                            fd.CHARACTER_HEIGHT = Convert.ToInt32(dr["CHARACTER_HEIGHT"].ToString());
                            fd.STROKE_WIDTH = Convert.ToDouble(dr["STROKE_WIDTH"].ToString());
                            retVal.Add(fd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                string err = string.Format("LoadFontDataItems() err: {0}. SQL: {1}", ex.Message, sqlText);
                NotifyError(err, "Data Access", true);
            }
            return retVal;
        }

        public static bool GetInspectLightAreas(int stationid, string labelitem)
        {
            bool retVal = false;

            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("SELECT INSPECT_LIGHT_AREAS FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE STATION_ID = {0} AND LABEL_ITEM = '{1}' ", stationid, labelitem);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        int tmp = Convert.ToInt32(dr["INSPECT_LIGHT_AREAS"].ToString());
                        retVal = (tmp == 0 ? false : true);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("GetVariationRegion() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }

        public static string GetVdeContrastAsString(int stationid, string labelitem)
        {
            string retVal = "";

            int labelId = LabelId(stationid, labelitem);
            DataSet dsFilters = OracleDatabase.GetFilters();

            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("SELECT SEGMENT_CONTRAST, CHARACTER_WIDTH, CHARACTER_HEIGHT FROM " + Defaults.DB_Schema + ".XX_VDE_ITEM_PARAMS WHERE STATION_ID = {0} AND LABEL_ID = {1} AND VDE_TYPE = 0", stationid, labelId);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        int cw = Convert.ToInt32(dr["CHARACTER_WIDTH"].ToString());
                        int ch = Convert.ToInt32(dr["CHARACTER_HEIGHT"].ToString());
                        int sc = Convert.ToInt32(dr["SEGMENT_CONTRAST"].ToString());
                        string pm = dr["PARTITION_METHOD"].ToString();

                        int rows = dsFilters.Tables[0].Rows.Count;
                        for (int x = 0; x < rows; x++)
                        {
                            DataRow dr1 = dsFilters.Tables[0].Rows[x];
                            //int F1 = Convert.ToInt32(dr1["FILTER1"].ToString());
                            //int F2 = Convert.ToInt32(dr1["FILTER2"].ToString());
                            //int F3 = Convert.ToInt32(dr1["FILTER3"].ToString());

                            if (cw == Convert.ToInt32(dr1["CHARACTER_WIDTH"].ToString()))
                                if (ch == Convert.ToInt32(dr1["CHARACTER_HEIGHT"].ToString()))
                                {
                                    if (sc == Convert.ToInt32(dr1["FILTER1"].ToString()) || sc == Convert.ToInt32(dr1["FILTER4"].ToString()))
                                    {
                                        retVal = "Low";
                                        break;
                                    }
                                    else if (sc == Convert.ToInt32(dr1["FILTER2"].ToString()))
                                    {
                                        retVal = "Medium";
                                        break;
                                    }

                                    if (sc == Convert.ToInt32(dr1["FILTER3"].ToString()))
                                    {
                                        retVal = "High";
                                        break;
                                    }
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = "";
                string err = string.Format("GetVDEContrastAsString() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }

        public static double GetInnerRadius(int stationid, string labelitem)
        {
            double retVal = 0;

            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("SELECT INNER_RADIUS FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE STATION_ID = {0} AND LABEL_ITEM = '{1}' ", stationid, labelitem);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        double tmp = Convert.ToDouble(dr["INNER_RADIUS"].ToString());
                        retVal = (tmp == 0 ? 0 : tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = 4.0;
                string err = string.Format("GetInnerRadius() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }

        public static string GetDebrisSizeLabel(int stationid, string labelitem)
        {
            string retVal = "";
            (double min1, double min2, double min3) tmpVals;

            int labelid = LabelId(stationid, labelitem);

            (tmpVals) = GetDebrisSizeDefaults();

            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("SELECT DEBRIS_SIZE_MIN FROM " + Defaults.DB_Schema + ".XX_DEBRIS_CHECK_PARAMS WHERE STATION_ID = {0} AND LABEL_ID = {1} ", stationid, labelid);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        double tmp = Convert.ToDouble(dr["DEBRIS_SIZE_MIN"].ToString());
                        if (tmp == tmpVals.min1)
                            retVal = "Small";
                        if (tmp == tmpVals.min2)
                            retVal = "Medium";
                        if (tmp == tmpVals.min3)
                            retVal = "Large";
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = "-";
                string err = string.Format("GetInnerRadius() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }

        public static (double min1, double min2, double min3) GetDebrisSizeDefaults()
        {
            (double, double, double) retVal = (0, 0, 0);

            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = "SELECT DEBRIS_SIZE_MIN1, DEBRIS_SIZE_MIN2, DEBRIS_SIZE_MIN3 FROM " + Defaults.DB_Schema + ".XX_DEBRIS_CHECK_DEFAULTS";
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        retVal.Item1 = Convert.ToDouble(dr["DEBRIS_SIZE_MIN1"].ToString());
                        retVal.Item2 = Convert.ToDouble(dr["DEBRIS_SIZE_MIN2"].ToString());
                        retVal.Item3 = Convert.ToDouble(dr["DEBRIS_SIZE_MIN3"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                retVal.Item1 = 0;
                retVal.Item2 = 0;
                retVal.Item3 = 0;
                string err = string.Format("GetDebrisSizeDefaults() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }


        public static InspectionParamDefaults GeInspectionParamDefaults()
        {
            InspectionParamDefaults retVal = new InspectionParamDefaults();
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = "SELECT * FROM " + Defaults.DB_Schema + ".XX_DEBRIS_CHECK_DEFAULTS ";
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        retVal.DebrisMinSize1 = Convert.ToDouble(dr["DEBRIS_SIZE_MIN1"].ToString());
                        retVal.DebrisMinSize2 = Convert.ToDouble(dr["DEBRIS_SIZE_MIN2"].ToString());
                        retVal.DebrisMinSize3 = Convert.ToDouble(dr["DEBRIS_SIZE_MIN3"].ToString());
                        retVal.SobelEdge1 = Convert.ToDouble(dr["SOBEL_EDGE1"].ToString());
                        retVal.SobelEdge2 = Convert.ToDouble(dr["SOBEL_EDGE2"].ToString());
                        retVal.SobelEdge3 = Convert.ToDouble(dr["SOBEL_EDGE3"].ToString());
                        retVal.SobelAmpSize1 = Convert.ToDouble(dr["SOBEL_AMP_SIZE1"].ToString());
                        retVal.SobelAmpSize2 = Convert.ToDouble(dr["SOBEL_AMP_SIZE2"].ToString());
                        retVal.SobelAmpSize3 = Convert.ToDouble(dr["SOBEL_AMP_SIZE3"].ToString());
                        retVal.MeanOffset = Convert.ToDouble(dr["MEAN_OFFSET"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                string err = string.Format("GeInspectionParamDefaults() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }

        public static InspectionParams GeInspectionParams(int stationid, int labelid)
        {
            InspectionParams retVal = new InspectionParams();

            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("SELECT * FROM " + Defaults.DB_Schema + ".XX_DEBRIS_CHECK_PARAMS WHERE STATION_ID = {0} AND LABEL_ID = {1} ", stationid, labelid);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        retVal.DebrisMinSize = Convert.ToDouble(dr["DEBRIS_SIZE_MIN"].ToString());
                        retVal.DebrisMaxSize = Convert.ToDouble(dr["DEBRIS_SIZE_MAX"].ToString());
                        retVal.MeanOffset = Convert.ToDouble(dr["MEAN_OFFSET"].ToString());
                        retVal.SobelAmpSize = Convert.ToDouble(dr["SOBEL_AMP_SIZE"].ToString());
                        retVal.SobelEdgeThreshold = Convert.ToDouble(dr["SOBEL_EDGE_THRESHOLD"].ToString());
                        retVal.LabelID = Convert.ToInt32(dr["LABEL_ID"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                string err = string.Format("GeInspectionParams() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }

        public static LabelType GetLabelType(int stationid, string labelitem)
        {
            //LabelType retVal = LabelType.PANEL;
            LabelType retVal = LabelType.FLATPANEL;

            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("SELECT LABEL_TYPE FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE STATION_ID = {0} AND LABEL_ITEM = '{1}' ", stationid, labelitem);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        int tmp = Convert.ToInt32(dr["LABEL_TYPE"].ToString());
                        switch (tmp)
                        {
                            case 0:
                                //retVal = LabelType.PANEL;
                                retVal = LabelType.FLATPANEL;
                                break;
                            case 1:
                                //retVal = LabelType.BOOKLET;
                                retVal = LabelType.DARK;
                                break;
                            case 2:
                                //retVal = LabelType.BOOKLET_BLEEDTHROUGH;
                                retVal = LabelType.BOOKLET;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = string.Format("GetLabelType() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }

        public static string GetLabelTypeAsString(int stationid, string labelitem)
        {
            string retVal = "";

            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("SELECT LABEL_TYPE FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE STATION_ID = {0} AND LABEL_ITEM = '{1}' ", stationid, labelitem);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        int tmp = Convert.ToInt32(dr["LABEL_TYPE"].ToString());
                        switch (tmp)
                        {
                            case 0:
                                retVal = "Flat Panel";
                                break;
                            case 1:
                                retVal = "Dark";
                                break;
                            case 2:
                                retVal = "Booklet";
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = string.Format("GetLabelTypeAsString() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }

        public static int GetCameraGain(int stationid, string labelitem)
        {
            int retVal = 0;

            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("SELECT CAMERA_GAIN FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE STATION_ID = {0} AND LABEL_ITEM = '{1}' ", stationid, labelitem);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        int tmp = Convert.ToInt32(dr["CAMERA_GAIN"].ToString());
                        retVal = (tmp == 0 ? 2 : tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = 6;
                string err = string.Format("GetCameraGain() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }


        public static int ImageCount(VAMImageTypes vit)
        {
            int retVal = 0;
            string sqlText = "";
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    if (vit == VAMImageTypes.VAM)
                        sqlText = "SELECT VAM_IMAGE_COUNT FROM " + Defaults.DB_Schema + ".XX_VARIATION_DEFAULTS WHERE STATION_ID = " + Defaults.StationID;
                    else
                        sqlText = "SELECT TEST_IMAGE_COUNT FROM " + Defaults.DB_Schema + ".XX_VARIATION_DEFAULTS WHERE STATION_ID = " + Defaults.StationID;

                    oCmd = new OracleCommand(sqlText, conn);
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        int tmp = Convert.ToInt32(dr[0].ToString());
                        retVal = tmp;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = 0;
                string err = string.Format("ImageCount() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }

        public static double GetInnerRadiusDefaultLarge()
        {
            double retVal = 0;
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = "SELECT INNER_RADIUS_LARGE FROM " + Defaults.DB_Schema + ".XX_VARIATION_DEFAULTS WHERE STATION_ID = " + Defaults.StationID;
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        double tmp = Convert.ToDouble(dr["INNER_RADIUS_LARGE"].ToString());
                        retVal = tmp;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = 4.0;
                string err = string.Format("GetInnerRadiusDefaultLarge() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }

        public static double GetConfidenceLevel()
        {
            double retVal = 0;
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = "SELECT CONFIDENCE_LEVEL FROM " + Defaults.DB_Schema + ".XX_VARIATION_DEFAULTS WHERE STATION_ID = " + Defaults.StationID;
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        double tmp = Convert.ToDouble(dr["CONFIDENCE_LEVEL"].ToString());
                        retVal = tmp;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = 4.0;
                string err = string.Format("GetConfidenceLevel() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }


        public static double GetInnerRadiusDefaultSmall()
        {
            double retVal = 0;

            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = "SELECT INNER_RADIUS_SMALL FROM " + Defaults.DB_Schema + ".XX_VARIATION_DEFAULTS WHERE STATION_ID = " + Defaults.StationID;
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        double tmp = Convert.ToDouble(dr["INNER_RADIUS_SMALL"].ToString());
                        retVal = tmp;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = 4.0;
                string err = string.Format("GetInnerRadiusDefaultSmall() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }


        public static int[] GetVariationRegion(int stationid, string labelitem)
        {
            int[] retVal = new int[] { 0, 0, 0, 0 };
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("SELECT TOP,LEFT,BOTTOM,RIGHT FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE STATION_ID = {0} AND LABEL_ITEM = '{1}' ", stationid, labelitem);
            OracleCommand oCmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    OracleDataAdapter da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        retVal[0] = Convert.ToInt32(dr["TOP"].ToString());
                        retVal[1] = Convert.ToInt32(dr["LEFT"].ToString());
                        retVal[2] = Convert.ToInt32(dr["BOTTOM"].ToString());
                        retVal[3] = Convert.ToInt32(dr["RIGHT"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                string err = string.Format("GetVariationRegion() err:\n{0}", ex.Message);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }


        public static int LabelId(int stationid, string labelitem)
        {
            int retVal = -1;
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sqlText = string.Format("SELECT ID FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE STATION_ID = {0} AND LABEL_ITEM = '{1}' ", stationid, labelitem);
            OracleCommand cmd = new OracleCommand(sqlText, conn);
            try
            {
                if (conn.State != ConnectionState.Open)
                    return retVal;
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        retVal = Convert.ToInt32(dr[0].ToString());
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
                string err = string.Format("OPZonesID() err: {0}", ex.Message + ". " + sqlText);
                NotifyError(err, "Data Access", true);
            }
            conn.Close();
            return retVal;
        }


        public static bool SaveFixtureId(object model, int stationid, string labelitem, string modelfilename)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            bool retVal;
            try
            {
                /* TODO: Replace HOperatorSet.WriteShapeModel */ //model, modelfilename);

                FileStream fs = new FileStream(modelfilename, FileMode.OpenOrCreate, FileAccess.Read);
                Byte[] blob = new Byte[fs.Length];
                fs.Read(blob, 0, blob.Length);
                fs.Close();
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                string sSql = string.Format("SELECT ID FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE LABEL_ITEM = '{0}' AND STATION_ID = {1}", labelitem, Defaults.StationID);
                oCmd = new OracleCommand(sSql, conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow d = ds.Tables[0].Rows[0];
                    object tmp = d["ID"].ToString();
                    int res;
                    int.TryParse(tmp.ToString(), out res);
                    if (res > 0)
                    {
                        Byte[] blob1 = new Byte[0];
                        string sql = "UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS SET FIXTURE_ID = :FIXTURE_ID WHERE ID = : LI_ID";
                        using (OracleCommand cmd = new OracleCommand(sql, conn))
                        {
                            cmd.Parameters.Add("FIXTURE_ID", OracleDbType.Blob).Value = blob1;
                            cmd.Parameters.Add("LI_ID", OracleDbType.Decimal, 1).Value = res;
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                        sql = "UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS SET FIXTURE_ID = :FIXTURE_ID WHERE ID = : LI_ID";
                        using (OracleCommand cmd = new OracleCommand(sql, conn))
                        {
                            cmd.Parameters.Add("FIXTURE_ID", OracleDbType.Blob).Value = blob;
                            cmd.Parameters.Add("LI_ID", OracleDbType.Decimal, 1).Value = res;
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                }
                retVal = true;
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "SaveModelFixture() err: " + ex.Message;
                NotifyError(err, "Label Training", true);
                return retVal;
            }
            return retVal;
        }

        public static bool SaveLabelData(bool inspectlightareas, int minarea, int stationid, string labelitem, Bitmap variationroi, double innerradius, int cameragain, LabelType labeltype)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            bool retVal = false;
            try
            {
                object r1 = 0, c1 = 0, r2 = 0, c2 = 0; // TODO: extract rectangle from variationroi
                int inspectLightAreas = 0;
                if (inspectlightareas)
                    inspectLightAreas = 1;

                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                string sSql = string.Format("UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS SET MIN_AREA = {0}, TOP = {1}, LEFT = {2}, BOTTOM = {3}, RIGHT = {4}, INNER_RADIUS = {5}, CAMERA_GAIN = {6} , LABEL_TYPE = {7}, INSPECT_LIGHT_AREAS = {8} WHERE LABEL_ITEM = '{9}' AND STATION_ID = {10}", minarea, r1, c1, r2, c2, innerradius, cameragain, (int)labeltype, inspectLightAreas, labelitem, Defaults.StationID);
                OracleCommand cmd = new OracleCommand(sSql, conn);
                int res = cmd.ExecuteNonQuery();
                if (res != 1)
                {
                    string strSql = "SELECT MAX(ID) FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS ";
                    oCmd = new OracleCommand(strSql, conn);
                    oCmd.CommandType = CommandType.Text;
                    DataSet ds = new DataSet();
                    da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        DataRow d = ds.Tables[0].Rows[0];
                        object tmp = d[0].ToString();
                        int maxId = -1;
                        int.TryParse(tmp.ToString(), out maxId);
                        if (maxId > 0)
                            maxId += 1;
                        else
                            maxId = 1;
                        sSql = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_LABEL_ITEMS (ID, STATION_ID, LABEL_ITEM, TOP, LEFT, BOTTOM, RIGHT, MIN_AREA, INNER_RADIUS, CAMERA_GAIN, LABEL_TYPE, INSPECT_LIGHT_AREAS) VALUES ({0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}) ", maxId, Defaults.StationID, labelitem, Convert.ToInt32(r1), Convert.ToInt32(c1), Convert.ToInt32(r2), Convert.ToInt32(c2), minarea, innerradius, cameragain, (int)labeltype, inspectLightAreas);
                        oCmd = new OracleCommand(sSql, conn);
                        oCmd.CommandType = CommandType.Text;
                        res = oCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "SaveLabelData() err: " + ex.Message;
                NotifyError(err, "Label Training", true);
                return retVal;
            }
            return retVal;
        }


        public static bool SaveInspectionParams(InspectionParams iparams)
        {
            bool retVal = true;
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            string sSql = "";

            try
            {
                DeleteInpectionParams(iparams.LabelID);
                OracleDatabase.ErrorDesription = "";
                if (conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                sSql = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_DEBRIS_CHECK_PARAMS (STATION_ID, LABEL_ID, DEBRIS_SIZE_MIN, DEBRIS_SIZE_MAX, MEAN_OFFSET, SOBEL_AMP_SIZE, SOBEL_EDGE_THRESHOLD) VALUES({0},{1},{2},{3},{4},{5},{6})", Defaults.StationID, iparams.LabelID, iparams.DebrisMinSize, iparams.DebrisMaxSize, iparams.MeanOffset, iparams.SobelAmpSize, iparams.SobelEdgeThreshold);

                oCmd = new OracleCommand(sSql, conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                retVal = false;
                OracleDatabase.ErrorDesription = string.Format("SaveInspectionParams() err: {0}\tSELECT\tSQL: {1}", ex.Message, sSql);
                NotifyError(OracleDatabase.ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static bool DeleteInpectionParams(int labelid)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
            bool retVal = true;
            try
            {
                string sSql = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_DEBRIS_CHECK_PARAMS WHERE LABEL_ID = {0} AND STATION_ID = {1}", labelid, Defaults.StationID);
                oCmd = new OracleCommand(sSql, conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "deleteInpectionParams() err: " + ex.Message;
                NotifyError(err, "Label Inspection", true);
            }
            return retVal;
        }


        public static bool SaveLabelRegion(Bitmap labelvar, int stationid, string labelitem, bool inspectlightareas, int maxgray, int minarea)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            bool retVal;
            try
            {
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                string sSql = string.Format("SELECT ID FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE LABEL_ITEM = '{0}' AND STATION_ID = {1}", labelitem, Defaults.StationID);
                oCmd = new OracleCommand(sSql, conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow d = ds.Tables[0].Rows[0];
                    object tmp = d["ID"].ToString();
                    int res;
                    int.TryParse(tmp.ToString(), out res);
                    int inspectLight = (inspectlightareas == true ? 1 : 0);
                    if (res > 0)
                    {
                        object row1 = 0, col1 = 0, row2 = 0, col2 = 0; // TODO: extract rectangle from labelvar
                        labelvar.Dispose();
                        string sql = string.Format("UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS SET TOP = {0}, LEFT = {1}, BOTTOM = {2}, RIGHT = {3}, INSPECT_LIGHT_AREAS = {4}, MAX_GRAY = {5}, MIN_AREA = {6}  WHERE ID = {7}", row1, col1, row2, col2, inspectLight, maxgray, minarea, res);
                        oCmd = new OracleCommand(sql, conn);
                        oCmd.CommandType = CommandType.Text;
                        oCmd.ExecuteNonQuery();
                    }
                }
                retVal = true;
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "SaveLabelRegion() err: " + ex.Message;
                NotifyError(err, "Label Training", true);
            }
            return retVal;
        }

        public static string GetReportCompiler(string reel)
        {
            OracleCommand oCmd;
            OracleDataAdapter da;
            DataSet ds;
            string retVal = "";
            try
            {
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                if (conn.State == ConnectionState.Open)
                {
                    string sql = string.Format("SELECT OPERATOR_NAME_INSPECTION FROM " + Defaults.DB_Schema + ".XX_INSPECTION_REPORT_SUMMARY WHERE REEL_LPN = '{0}'", reel);
                    oCmd = new OracleCommand(sql, conn);
                    oCmd.CommandType = CommandType.Text;
                    ds = new DataSet();
                    da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 1)
                        retVal = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                }
                else
                    throw new Exception(Environment.NewLine + "COSMOS connection has been lost" + Environment.NewLine + "Cannot update inspection status to complete");
            }
            catch (System.Exception ex)
            {
                retVal = "";
                string err = "GetCompiler() err: " + ex.Message;
                NotifyError(err, "Label Inspection", true);
            }
            return retVal;
        }

        public static string GetReportSummary(string reel)
        {
            OracleCommand oCmd;
            OracleDataAdapter da;
            DataSet ds;
            string retVal = "";
            try
            {
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                if (conn.State == ConnectionState.Open)
                {
                    string sql = string.Format("SELECT OPERATOR_REPORT_SUMMARY FROM " + Defaults.DB_Schema + ".XX_INSPECTION_REPORT_SUMMARY WHERE REEL_LPN = '{0}'", reel);
                    oCmd = new OracleCommand(sql, conn);
                    oCmd.CommandType = CommandType.Text;
                    ds = new DataSet();
                    da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 1)
                        retVal = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                }
                else
                    throw new Exception(Environment.NewLine + "COSMOS connection has been disconnected" + Environment.NewLine + "Cannot update inspection status to complete");
            }
            catch (System.Exception ex)
            {
                retVal = "";
                string err = "GetReportSummary() err: " + ex.Message;
                NotifyError(err, "Label Inspection", true);
            }
            return retVal;
        }

        public static DateTime[] GetReportSummaryDates(string reel)
        {
            OracleCommand oCmd;
            OracleDataAdapter da;
            DataSet ds;
            DateTime[] retVal = new DateTime[2];
            try
            {
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                if (conn.State == ConnectionState.Open)
                {
                    string sql = string.Format("SELECT INSPECTION_LABEL_START, INSPECTION_LABEL_FINISH FROM " + Defaults.DB_Schema + ".XX_INSPECTION_REPORT_SUMMARY WHERE REEL_LPN = '{0}'", reel);
                    oCmd = new OracleCommand(sql, conn);
                    oCmd.CommandType = CommandType.Text;
                    ds = new DataSet();
                    da = new OracleDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        retVal[0] = Convert.ToDateTime(ds.Tables[0].Rows[0].ItemArray[0].ToString());
                        retVal[1] = Convert.ToDateTime(ds.Tables[0].Rows[0].ItemArray[1].ToString());
                    }
                }
                else
                    throw new Exception(Environment.NewLine + "COSMOS connection has been disconnected" + Environment.NewLine + "Cannot update inspection status to complete");
            }
            catch (System.Exception ex)
            {
                retVal = null;
                string err = "GetReportSummary() err: " + ex.Message;
                NotifyError(err, "Label Inspection", true);
            }
            return retVal;
        }


        public static bool GetCompleteReport(string reel, string labelitem)
        {
            OracleCommand oCmd;
            OracleDataAdapter da;
            DataSet ds;
            bool retVal = false;
            try
            {
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                string sql = string.Format("SELECT REEL_LPN FROM " + Defaults.DB_Schema + ".XX_INSPECTION_REPORT_SUMMARY WHERE REEL_LPN = '{0}' AND MACHINE_NUMBER = {1} AND LABEL_ITEM = '{2}' AND INSPECTION_LABEL_FINISH IS NOT NULL ", reel, Defaults.StationID, labelitem);
                oCmd = new OracleCommand(sql, conn);
                oCmd.CommandType = CommandType.Text;
                ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 1)
                    retVal = true;
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "CompleteReport() err: " + ex.Message;
                NotifyError(err, "Label Inspection", true);
            }
            return retVal;
        }

        public static bool SaveVariationVam(int stationid, string labelitem, string foldernametmp, string foldernamebase)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            string filename = Path.Combine(foldernametmp, "variationModel.vam");
            string filename1 = Path.Combine(foldernamebase, "variationModel.vam");
            bool retVal;
            try
            {
                FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read);
                Byte[] blob = new Byte[fs.Length];
                Byte[] blob1 = new Byte[0];
                fs.Read(blob, 0, blob.Length);
                fs.Close();
                File.Copy(filename, filename1, true);
                try { File.Delete(filename); } catch { }
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                string sSql = string.Format("SELECT ID FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE LABEL_ITEM = '{0}' AND STATION_ID = {1}", labelitem, Defaults.StationID);
                oCmd = new OracleCommand(sSql, conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow d = ds.Tables[0].Rows[0];
                    object tmp = d["ID"].ToString();
                    int res;
                    int.TryParse(tmp.ToString(), out res);
                    if (res > 0)
                    {
                        string sql = "UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS SET VARIATION_VAM = :VARIATION_VAM WHERE ID = : LI_ID";
                        using (OracleCommand cmd = new OracleCommand(sql, conn))
                        {
                            cmd.Parameters.Add("VARIATION_VAM", OracleDbType.Blob).Value = blob1;
                            cmd.Parameters.Add("LI_ID", OracleDbType.Decimal, 1).Value = res;
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                        sql = "UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS SET VARIATION_VAM = :VARIATION_VAM WHERE ID = : LI_ID";
                        using (OracleCommand cmd = new OracleCommand(sql, conn))
                        {
                            cmd.Parameters.Add("VARIATION_VAM", OracleDbType.Blob).Value = blob;
                            cmd.Parameters.Add("LI_ID", OracleDbType.Decimal, 1).Value = res;
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                }
                try { File.Delete(filename1); } catch { }
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "SaveVariationVAM() err: " + ex.Message;
                NotifyError(err, "Label Training", true);
            }
            return retVal;
        }

        public static bool SaveFontFile(string fontfoldername, string fontfilename, FontType ftype)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            string filename = Path.Combine(fontfoldername, fontfilename);
            string columnName = "";
            bool retVal;
            try
            {

                switch (ftype)
                {
                    case FontType.FONT:
                        columnName = "FONT_SIZE";
                        break;
                    default:
                        break;
                }

                //"find_text_support.hotc"
                FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read);
                Byte[] blob = new Byte[fs.Length];
                Byte[] blob1 = new Byte[0];
                fs.Read(blob, 0, blob.Length);
                fs.Close();
                OracleConnection conn = SqLiteDatabase.GetOracleConnection(Defaults.SchemaToUse);
                if (conn == null)
                    throw new Exception("COSMOS connection failure");
                string sql = string.Format("UPDATE " + Defaults.DB_Schema + ".XX_FONTS SET {0} = :COLUMN_NAME", columnName);
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add("COLUMN_NAME", OracleDbType.Blob).Value = blob1;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                sql = string.Format("UPDATE " + Defaults.DB_Schema + ".XX_FONTS SET {0} = :COLUMN_NAME", columnName);
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add("COLUMN_NAME", OracleDbType.Blob).Value = blob;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                //                try { File.Delete(filename); } catch { }
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "SaveFontFile() err: " + ex.Message;
                NotifyError(err, "Label Training", true);
            }
            return retVal;
        }
    }


    public static class SqLiteDatabase
    {
        public static SystemMessageHandler Smh;
        public static string ErrorDesription = "";
        private static OracleConnection m_Conn;
        private static SQLiteConnection _mConn;
        public static string ConnectionString => $"Data Source = {Defaults.SqlitePath}";


        public static string GoodLIN(string lin)
        {//L30002822
            string retVal = "";
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                string sSQL = string.Format("SELECT DISTINCT LABEL_ITEM FROM " + Defaults.DB_Schema + ".XX_VARIABLE_DATA_EXTRACT_VW WHERE LABEL_ITEM = '{0}'", lin);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    try { retVal = dr[0].ToString(); } catch { retVal = ""; }
                    retVal = retVal.Trim();
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GoodLIN() err:\n{0}", ex.Message);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static LabelItemAndVersion GoodREEL_LWO_LIN_VERSION_Data(string reel)
        {
            LabelItemAndVersion retVal = new LabelItemAndVersion();
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                string sSQL = string.Format("SELECT DISTINCT REEL_LPN FROM " + Defaults.DB_Schema + ".XX_VARIABLE_DATA_EXTRACT_VW WHERE REEL_LPN = '{0}'", reel);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                DataRow dr;
                if (ds.Tables[0].Rows.Count == 1)
                {
                    dr = ds.Tables[0].Rows[0];
                    try { retVal.REEL = dr[0].ToString(); } catch { retVal.REEL = ""; }
                }
                if (retVal.REEL != "")
                {
                    ds = new DataSet();
                    dr = null;
                    sSQL = string.Format("SELECT DISTINCT LABEL_WORK_ORDER FROM " + Defaults.DB_Schema + ".XX_VARIABLE_DATA_EXTRACT_VW WHERE REEL_LPN = '{0}'", retVal.REEL);
                    oCmd = new OracleCommand(sSQL, m_Conn);
                    oCmd.CommandType = CommandType.Text;
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        dr = ds.Tables[0].Rows[0];
                        try { retVal.LWO = dr[0].ToString(); } catch { retVal.LWO = ""; }
                    }
                }
                if (retVal.LWO != "")
                {
                    ds = new DataSet();
                    dr = null;
                    sSQL = string.Format("SELECT DISTINCT LABEL_ITEM FROM " + Defaults.DB_Schema + ".XX_VARIABLE_DATA_EXTRACT_VW WHERE REEL_LPN = '{0}' AND LABEL_WORK_ORDER = '{1}'", retVal.REEL, retVal.LWO);
                    oCmd = new OracleCommand(sSQL, m_Conn);
                    oCmd.CommandType = CommandType.Text;
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        dr = ds.Tables[0].Rows[0];
                        try { retVal.LIN = dr[0].ToString(); } catch { retVal.LIN = ""; }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GoodREEL_LWO_LIN_VERSION_Data() err:\n{0}", ex.Message);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static string GetSample(string lpn)
        {
            string retVal = "";
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            List<string> sampleMeds = new List<string>();
            string strSQL = "";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                strSQL = String.Format("SELECT MED_ID, MED_SEQUENCE_NUMBER FROM " + Defaults.DB_Schema + ".XX_VARIABLE_DATA_EXTRACT_VW WHERE REEL_LPN = '{0}' ORDER BY MED_SEQUENCE_NUMBER DESC", lpn);

                oCmd = new OracleCommand();
                oCmd = new OracleCommand(strSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count >= 1)
                {
                    int count = ds.Tables[0].Rows.Count;
                    DataRow row = ds.Tables[0].Rows[count - 1];
                    retVal = row["MED_ID"].ToString();
                }
            }
            catch (Exception ex)
            {
                retVal = "";
                ErrorDesription = string.Format("GetSample() err:\n{0}\n{1}", ex.Message, strSQL);
            }
            return retVal;
        }


        internal static void notifyError(string msg, string title, bool IsError)
        {
            try
            {
                //raise event instead of displaying a dialog
                if (Smh != null)
                {
                    CriticalLevels cl = Enums.CriticalLevels.Black;
                    if (IsError)
                        cl = Enums.CriticalLevels.Red;

                    SystemMessageEventArgs smea = new SystemMessageEventArgs(msg, title, (int)cl);
                    Smh(smea);
                }
            }
            catch (Exception ex)
            {
                try { Log.Logger.Error("notifyError() Err: {Error} Originating error: {Msg}", ex.Message, msg); } catch { }
            }
        }

        public static List<FontSizesSegment> GetListFontSizes()
        {
            List<FontSizesSegment> retVal = new List<FontSizesSegment>();
            DataTable dt = new DataTable();
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);

            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                string sSQL = string.Format("SELECT * FROM " + Defaults.DB_Schema + ".XX_FONT_DATA_SEGMENT WHERE STATION_ID = {0} ORDER BY ORDER_BY_SIZE", Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        FontSizesSegment fs = new FontSizesSegment();
                        string s = dr[0].ToString();
                        fs.Size = dr["SIZE"].ToString();
                        fs.CharHeight = Convert.ToDouble(dr["CHARACTER_HEIGHT"].ToString());
                        fs.CharWidth = Convert.ToDouble(dr["CHARACTER_WIDTH"].ToString());
                        fs.StrokeWidth = dr["STROKE_WIDTH"].ToString();
                        fs.SegmentContrast = Convert.ToInt32(dr["SEGMENT_CONTRAST"].ToString());
                        fs.PartitionMethod = dr["PARTITION_METHOD"].ToString();
                        retVal.Add(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = "GetFontSizesSegmentation() err: " + ex.Message;
                notifyError(ErrorDesription, "VDE DATA", true);
            }
            return retVal;
        }


        public static List<string> UserNames()
        {
            List<string> retVal = new List<string>();
            DataTable dt = new DataTable();
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);

            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                string sSQL = "SELECT DISTINCT ACTION_USERNAME FROM " + Defaults.DB_Schema + ".XX_LVS3_ACTIONS WHERE ACTION_USERNAME IS NOT NULL ORDER BY ACTION_USERNAME";
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string s = dr[0].ToString();
                        retVal.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = "AuditLog() err: " + ex.Message;
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static DataSet AuditLog(string username, DateTime datefrom, DateTime dateto)
        {
            DataSet retVal = new DataSet();
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);

            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                string startDate = " AND (ACTION_DATE >= TO_DATE('" + datefrom.ToString("MM/dd/yyyy 00:00:00") + "', 'mm/dd/yyyy HH24:MI:SS')";
                string endDate = " AND ACTION_DATE <= TO_DATE('" + dateto.ToString("MM/dd/yyyy 23:59:59") + "', 'mm/dd/yyyy HH24:MI:SS'))";

                string sSQL = string.Format("SELECT ACTION_USERNAME, ACTION_DATE, DTZ, ACTION_TITLE, ACTION_MESSAGE, ACTION_REFERS_TO, ACTION_USER_REASON, ACTION_MACHINE FROM " + Defaults.DB_Schema + ".XX_LVS3_ACTIONS WHERE (ACTION_USERNAME {0} ", username);
                sSQL = sSQL + startDate;
                sSQL = sSQL + endDate;
                sSQL = sSQL + " ORDER BY ACTION_DATE DESC";
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(retVal);
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = "AuditLog() err: " + ex.Message;
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static bool SaveAction(string message, string title, string lpn, string username, string method, string refersto, string userreason)
        {
            OracleCommand oCmd = new OracleCommand();
            bool retVal;
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                string dtz = DateFormats.LocalTimeAndZone(false);
                string uName = username;

                string strSQL = "INSERT INTO " + Defaults.DB_Schema + ".XX_LVS3_ACTIONS (STATION_ID, ACTION_MESSAGE, ACTION_SOURCE, ACTION_USERNAME, ACTION_CURRENTMETHOD, ACTION_MACHINE, ACTION_REFERS_TO, ACTION_USER_REASON, DTZ) ";
                string strValues = string.Format("VALUES({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')", Defaults.StationID, message, title, uName, method, Environment.MachineName, refersto, userreason, dtz);
                strSQL = strSQL + strValues;
                oCmd = new OracleCommand(strSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                int res = oCmd.ExecuteNonQuery();
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                ErrorDesription = "SaveAction()" + ex.Message;
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }


        public static bool SaveAction1(string message, string title, string lpn, string username, string method, string refersto, string operatortext, string userreason)
        {
            OracleCommand oCmd = new OracleCommand();
            bool retVal;
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                string dtz = DateFormats.LocalTimeAndZone(false);
                string uName = username;

                operatortext = operatortext.Replace("'", "");
                operatortext = operatortext.Replace("/", "|");
                message = message.Replace("'", "");
                message = message.Replace("/", "|");

                string strSQL = "INSERT INTO " + Defaults.DB_Schema + ".XX_LVS3_ACTIONS (STATION_ID, ACTION_MESSAGE, ACTION_SOURCE, ACTION_USERNAME, ACTION_CURRENTMETHOD, ACTION_MACHINE, ACTION_REFERS_TO, ACTION_USER_REASON, DTZ, ACTION_TITLE) ";
                string strValues = string.Format("VALUES({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')", Defaults.StationID, message, title, uName, method, Environment.MachineName, refersto, userreason, dtz, operatortext);
                strSQL = strSQL + strValues;
                oCmd = new OracleCommand(strSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                int res = oCmd.ExecuteNonQuery();
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                ErrorDesription = "SaveAction1()" + ex.Message;
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }



        public static List<ADGroupData> GetAllGroups()
        {
            List<ADGroupData> retVal = new List<ADGroupData>();
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();

            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                string sSQL = "SELECT * FROM XX_AD_USER_GROUPS ORDER BY GROUP_LEVEL";
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ADGroupData agd = new ADGroupData();
                        agd.ADGroupLevel = Convert.ToInt32(dr["GROUP_LEVEL"].ToString());
                        agd.ADGroupName = dr["GROUP_NAME"].ToString();
                        agd.ADGroupNameFriendly = dr["GROUP_FRIENDLY_NAME"].ToString();
                        retVal.Add(agd);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal.Clear();
                ErrorDesription = string.Format("GetAllGroups() err:\n{0}", ex.Message);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static List<LabelItemDataSetup> LabelDataItems(string lin, string reelLpn)
        {
            var retVal = new List<LabelItemDataSetup>();
            var sqliteCmd = new SQLiteCommand();
            var da = new SQLiteDataAdapter(sqliteCmd);
            var sSql = "";
            try
            {

                var endOfLin = lin.IndexOf("Issue No", StringComparison.Ordinal);
                if (endOfLin > -1)
                {
                    lin = lin.Substring(0, endOfLin - 1).Trim();
                    lin = lin.Trim();
                }
                ErrorDesription = "";
                if (_mConn == null)
                    OpenSqLiteConnection();
                else if (_mConn.State == ConnectionState.Closed)
                    OpenSqLiteConnection();
                if (_mConn == null)
                    throw new Exception("Cannot open connection to Database schema");
                if (_mConn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema");

                sSql = string.Format("SELECT distinct(VARIABLE_NAME), ID, PLACE_HOLDER, FIELD_TYPE FROM LABEL_ITEM WHERE LIN = '{0}'", lin);
                sqliteCmd = new SQLiteCommand(sSql, _mConn);
                sqliteCmd.CommandType = CommandType.Text;
                var ds = new DataSet();
                da = new SQLiteDataAdapter();
                da.SelectCommand = sqliteCmd;
                sqliteCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var lid = new LabelItemDataSetup();
                        lid.LIN = lin;
                        //lid.Version = version;
                        lid.FIELD_TYPE = dr["FIELD_TYPE"].ToString();
                        lid.PLACE_HOLDER = dr["PLACE_HOLDER"].ToString();
                        lid.VARIABLE_NAME = dr["VARIABLE_NAME"].ToString();
                        lid.VARIABLE_NAME = lid.VARIABLE_NAME.Replace(" ", "_");
                        lid.Repeat = GetRepeat(lid.VARIABLE_NAME, reelLpn);

                        var ID = dr["ID"];
                        int id;
                        if (ID != null && ID != DBNull.Value)
                        {
                            id = Convert.ToInt32(ID);
                        }
                        else
                        {
                            id = 0;
                        }
                        lid.VDE_DATA_LIST = GetAllMedData(id);
                        lid.VDE_SEQUENCE_LIST = GetAllMedSequenceData(id);
                        if (lid.DataPresent)
                        {
                            lid.VDE_DATA = GetFirstMedData(id);
                            retVal.Add(lid);
                            lid.IsNumeric = IsDigitsOnly(lid.VDE_DATA_LIST);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("LabelDataItems() err: {0}\nSQL: {1}", ex.Message, sSql);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }


        private static bool IsDigitsOnly(List<string> list)
        {
            bool retVal = true;
            foreach (string s in list)
            {
                foreach (char c in s)
                {
                    if (c < '0' || c > '9')
                        if (c != '/')
                        {
                            retVal = false;
                            break;
                        }
                }
            }
            return retVal;
        }

        public static MedData LabelDataInspection(string lwo, string reel_lpn, string placeholder)
        {
            MedData retVal = null;
            string lin = "";

            var sSql = string.Format("SELECT LIN  FROM LABEL_ITEM");
            var sqliteCmd = new SQLiteCommand(sSql, _mConn);
            sqliteCmd.CommandType = CommandType.Text;
            var ds = new DataSet();
            var da = new SQLiteDataAdapter(sqliteCmd);
            sqliteCmd.ExecuteNonQuery();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                var dr = ds.Tables[0].Rows[0];
                lin = dr["LIN"].ToString();
            }

            var medSql = string.Format("Select VALUE FROM VDE_DATA_LIST ORDER BY ID");
            var medCmd = new SQLiteCommand(medSql, _mConn);
            medCmd.CommandType = CommandType.Text;
            var medDs = new DataSet();
            var medDa = new SQLiteDataAdapter(medCmd);
            medCmd.ExecuteNonQuery();
            medDa.Fill(medDs);
            string medId = string.Empty;
            if (medDs.Tables[0].Rows.Count > 0)
            {
                var dr = medDs.Tables[0].Rows[0];
                medId = dr["VALUE"].ToString();
            }


            var labelDataItem = LabelDataItems(lin, reel_lpn);


            retVal = new(medId, labelDataItem[0].VDE_SEQUENCE_LIST[0])
            {
                data_list = labelDataItem[0].VDE_DATA_LIST,
                sequence_list = labelDataItem[0].VDE_SEQUENCE_LIST,
                PlaceHolder = placeholder
            };


            return retVal;
        }


        public static bool SetReportFilePath(string path)
        {
            bool retVal = true;
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            string sSQL = "";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                sSQL = string.Format("UPDATE " + Defaults.DB_Schema + ".XX_REPORT_FILE_PATH SET FILE_PATH = '{0}' ", path);
                sSQL += string.Format("WHERE STATION_ID = {0}", Defaults.StationID);

                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.UpdateCommand = oCmd;
                int res = oCmd.ExecuteNonQuery();

                if (res <= 0)
                {
                    sSQL = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_REPORT_FILE_PATH (FILE_PATH, STATION_ID) VALUES('{0}',{1})", path, Defaults.StationID);
                    oCmd = new OracleCommand(sSQL, m_Conn);
                    oCmd.CommandType = CommandType.Text;
                    da.SelectCommand = oCmd;
                    oCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                ErrorDesription = string.Format("SetReportFilePath() err: {0}\nUPDATE/INSERT\nSQL: {1}", ex.Message, sSQL);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }




        public static string GetReportFilePath()
        {
            string retVal = "";
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            string sSQL = "";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                sSQL = string.Format("SELECT FILE_PATH FROM " + Defaults.DB_Schema + " .XX_REPORT_FILE_PATH WHERE STATION_ID = {0}", Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1];
                    retVal = dr["FILE_PATH"].ToString();
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GetReportFilePath() err: {0}\nSQL: {1}", ex.Message, sSQL);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static string GetTmFileName(ModelOCMType type)
        {
            string retVal = "";
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            string sSQL = "";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                sSQL = string.Format("SELECT OCM_FILE_NAME FROM " + Defaults.DB_Schema + " .XX_TEXT_MODEL_OCM WHERE READER_TYPE = {0}", (int)type);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1];
                    retVal = dr["OCM_FILE_NAME"].ToString();
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GetTMParams() err: {0}\nSQL: {1}", ex.Message, sSQL);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static List<TMParams> GetTmParams()
        {
            List<TMParams> retVal = new List<TMParams>();
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            string sSQL = "";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                sSQL = "SELECT * FROM " + Defaults.DB_Schema + " .XX_TEXT_MODEL_PARAMS";
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TMParams tmp = new TMParams();
                        tmp.ParamName = dr["PARAM_NAME"].ToString();
                        tmp.ParamValue = dr["PARAM_VALUE"].ToString();
                        retVal.Add(tmp);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GetTMParams() err: {0}\nSQL: {1}", ex.Message, sSQL);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static (int min_area, int max_gray) GetMSERParamsLabel(string labelitem)
        {
            (int min_area, int max_gray) retVal = (50, 100);
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            string sSQL = "";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                sSQL = string.Format("SELECT MAX_GRAY, MIN_AREA FROM " + Defaults.DB_Schema + " .XX_LABEL_ITEMS WHERE LABEL_ITEM = '{0}' AND STATION_ID = {1}", labelitem, Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1];
                    retVal.min_area = Convert.ToInt32(dr["MIN_AREA"].ToString());
                    retVal.max_gray = Convert.ToInt32(dr["MAX_GRAY"].ToString());
                }
            }
            catch (Exception ex)
            {
                retVal.min_area = 50;
                retVal.max_gray = 100;
                ErrorDesription = string.Format("GetMSERParams() err: {0}\nSQL: {1}", ex.Message, sSQL);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static (int f1, int f2, int f3, int f4) GetFilters(string filtersize)
        {
            (int, int, int, int) retVal = (0, 0, 0, 0);
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            string sSQL = "";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                sSQL = String.Format("SELECT FILTER1, FILTER2, FILTER3, FILTER4 FROM " + Defaults.DB_Schema + " .XX_FONT_DATA_SEGMENT WHERE STATION_ID = {0} AND  UPPER(\"SIZE\")= '{1}'", Defaults.StationID, filtersize.ToUpper());
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1];
                    retVal = (Convert.ToInt32(dr["FILTER1"].ToString()), Convert.ToInt32(dr["FILTER2"].ToString()), Convert.ToInt32(dr["FILTER3"].ToString()), Convert.ToInt32(dr["FILTER4"].ToString()));
                }
            }
            catch (Exception ex)
            {
                retVal = (0, 0, 0, 0);
                ErrorDesription = string.Format("GetFilters() err: {0}\nSQL: {1}", ex.Message, sSQL);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }


        public static DataSet GetFilters()
        {
            DataSet retVal = null;
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            string sSQL = "";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                sSQL = String.Format("SELECT * FROM " + Defaults.DB_Schema + ".XX_FONT_DATA_SEGMENT WHERE STATION_ID = {0} ORDER BY ORDER_BY_SIZE", Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                    retVal = ds;
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GetFilters() err: {0}\nSQL: {1}", ex.Message, sSQL);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static int GetRepeat(string varname, string reel_lpn)
        {
            // return -1 on exceptions 
            // return 0 where all labels are identical 
            // return nn for repeat: 
            // eg: a value of 1 would indicate every med was unique, 
            // eg: a value of 27 indicates there are 27 identical labels per repeat. this number will divide exactly into the number of labels on the reel

            int retVal = -1;
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            string sSQL = "";
            int divider = 1;
            int rowcount = 0;
            try
            {
                ErrorDesription = "";
                varname = varname.Replace(" ", "_");
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                sSQL = string.Format("SELECT COUNT(DISTINCT({0})) AS DIVIDER FROM " + Defaults.DB_Schema + ".XX_VARIABLE_DATA_EXTRACT_VW WHERE REEL_LPN = '{1}'", varname, reel_lpn);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    divider = Convert.ToInt32(dr["DIVIDER"].ToString());
                }
                sSQL = string.Format("SELECT COUNT({0}) AS VDE_ROW_COUNT FROM " + Defaults.DB_Schema + ".XX_VARIABLE_DATA_EXTRACT_VW WHERE REEL_LPN = '{1}'", varname, reel_lpn);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                ds = new DataSet();
                da = new OracleDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    rowcount = Convert.ToInt32(dr["VDE_ROW_COUNT"].ToString());
                }
                if (rowcount > 0 && divider > 0)
                    retVal = rowcount / divider;
                if (retVal == rowcount)
                    retVal = 0; // return 0 where all labels are identical 
            }
            catch (Exception ex)
            {
                retVal = -1;
                ErrorDesription = string.Format("GetFirstMedData() err: {0}\n\nSQL: {1}", ex.Message, sSQL);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static string GetFirstMedData(int id)
        {
            var sqliteCmd = new SQLiteCommand();
            var sSql = "";
            string retVal;
            try
            {
                ErrorDesription = "";
                if (_mConn == null)
                    OpenSqLiteConnection();
                else if (_mConn.State == ConnectionState.Closed)
                    OpenSqLiteConnection();
                if (_mConn == null)
                    throw new Exception("Cannot open connection to Database schema");
                if (_mConn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema");

                sSql = string.Format("SELECT DISTINCT(VALUE)  FROM VDE_SEQUENCE_LIST WHERE LABEL_ID = '{0}' ORDER BY VALUE DESC", id);

                sqliteCmd = new SQLiteCommand(sSql, _mConn);
                sqliteCmd.CommandType = CommandType.Text;
                var ds = new DataSet();
                var da = new SQLiteDataAdapter(sqliteCmd);
                sqliteCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var dr = ds.Tables[0].Rows[0];
                    retVal = dr["VALUE"].ToString();
                }
                else
                    throw new Exception(string.Format("Unable to extract {0} VDE data for setup. ", id));
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GetFirstMedData() err: {0}\n\nSQL: {1}", ex.Message, sSql);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static List<string> GetAllMedData(int id)
        {
            var retVal = new List<string>();
            var sqliteCmd = new SQLiteCommand();
            var sSql = "";
            try
            {
                ErrorDesription = "";
                if (_mConn == null)
                    OpenSqLiteConnection();
                else if (_mConn.State == ConnectionState.Closed)
                    OpenSqLiteConnection();
                if (_mConn == null)
                    throw new Exception("Cannot open connection to Database schema");
                if (_mConn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema");

                sSql = string.Format("SELECT ID, VALUE AS medSeqNum FROM VDE_DATA_LIST  WHERE LABEL_ID = '{0}' ORDER BY ID", id);

                sqliteCmd = new SQLiteCommand(sSql, _mConn);
                sqliteCmd.CommandType = CommandType.Text;
                var ds = new DataSet();
                var da = new SQLiteDataAdapter(sqliteCmd);
                sqliteCmd.ExecuteNonQuery();
                da.Fill(ds);
                var y = ds.Tables[0].Rows.Count;
                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (var x = 0; x < y; x++)
                    {
                        var dr = ds.Tables[0].Rows[x];
                        retVal.Add(dr["medSeqNum"].ToString());
                    }
                }
                else
                    throw new Exception(string.Format("Unable to extract {0} VDE data for setup. ", id));
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GetAllMedData() err: {0}\n\nSQL: {1}", ex.Message, sSql);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static List<string> GetAllMedSequenceData(int id)
        {
            var retVal = new List<string>();
            var sqliteCmd = new SQLiteCommand();
            var sSql = "";
            try
            {
                ErrorDesription = "";
                if (_mConn == null)
                    OpenSqLiteConnection();
                else if (_mConn.State == ConnectionState.Closed)
                    OpenSqLiteConnection();
                if (_mConn == null)
                    throw new Exception("Cannot open connection to Database schema");
                if (_mConn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema");

                sSql = string.Format("SELECT VALUE FROM VDE_SEQUENCE_LIST WHERE LABEL_ID = '{0}' ORDER BY VALUE DESC", id);

                sqliteCmd = new SQLiteCommand(sSql, _mConn);
                sqliteCmd.CommandType = CommandType.Text;
                var ds = new DataSet();
                var da = new SQLiteDataAdapter(sqliteCmd);
                sqliteCmd.ExecuteNonQuery();
                da.Fill(ds);
                var y = ds.Tables[0].Rows.Count;
                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (var x = 0; x < y; x++)
                    {
                        var dr = ds.Tables[0].Rows[x];
                        retVal.Add(dr["VALUE"].ToString());
                    }
                }
                else
                    throw new Exception("Unable to extract Med Sequence Numbers data!");
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GetAllMedSequenceData() err: {0}. SQL: {1}", ex.Message, sSql);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }


        public static List<string> SaveTrainingData(string lin, TrainingDataType tdt, List<string> td)
        {
            List<string> retVal = new List<string>();
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            string sSQL = "";
            string sTmp = "";
            try
            {
                if (td.Count == 0)
                    return retVal;

                foreach (string s in td)
                {
                    sTmp = sTmp + s + '\n';
                }
                //return null;
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                switch (tdt)
                {
                    case TrainingDataType.BARCODE:
                        sSQL = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_TRAINING_DATA_BARCODE (LIN, VDE, VDE_NAME, LOCATION, NON_VDE_DATA, STATION_ID) VALUES('{0}','{1}','{2}','{3}','{4}',{5})", lin, td[0] + " | " + td[1], td[2], td[3], td[4], Defaults.StationID);
                        break;
                    case TrainingDataType.LABEL:
                        sSQL = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_TRAINING_DATA_LABEL (LIN, THRESHOLD, MIN_AREA, STATION_ID) VALUES('{0}','{1}','{2}',{3})", lin, td[0], td[1], Defaults.StationID);
                        break;
                    case TrainingDataType.MASK:
                        td[0] = td[0].Replace('\r', ' ');
                        sSQL = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_TRAINING_DATA_MASK (LIN, LOCATION, STATION_ID) VALUES('{0}','{1}',{2})", lin, td[0], Defaults.StationID);
                        break;
                    case TrainingDataType.ROT:
                        td[0] = td[0].Replace('\r', ' ');
                        sSQL = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_TRAINING_DATA_ROT (LIN, LOCATION, STATION_ID) VALUES('{0}','{1}',{2})", lin, td[0], Defaults.StationID);
                        break;
                    case TrainingDataType.OPZONE:
                        sSQL = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_TRAINING_DATA_OPZONE (LIN, LOCATION, THRESHOLD, MIN_AREA, OPZONE_NAME, STATION_ID) VALUES('{0}','{1}','{2}','{3}','{4}',{5})", lin, td[1], td[2], td[3], td[0], Defaults.StationID);
                        break;
                    case TrainingDataType.VDE:
                        sSQL = string.Format("INSERT INTO " + Defaults.DB_Schema + ".XX_TRAINING_DATA_VDE (LIN, PLACE_HOLDER, OPZONE, LOCATION, STATION_ID) VALUES('{0}','{1}','{2}','{3}',{4})", lin, td[0], td[1], td[2], Defaults.StationID);
                        break;
                }

                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GetTrainingData() err: {0}\tSELECT\tSQL: {1}", ex.Message, sSQL);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static bool DeleteTrainingData(string lin)
        {
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            bool retVal = true;
            string sSQL = "";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                sSQL = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_BARCODE WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.ExecuteNonQuery();

                sSQL = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_LABEL WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.ExecuteNonQuery();

                sSQL = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_MASK WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.ExecuteNonQuery();

                sSQL = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_OPZONE WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.ExecuteNonQuery();

                sSQL = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_VDE WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.ExecuteNonQuery();

                //sSQL = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_ROT WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                //oCmd = new OracleCommand(sSQL, m_Conn);
                //oCmd.CommandType = CommandType.Text;
                //oCmd.ExecuteNonQuery();

                int labelID = OracleLabelData.LabelID(Defaults.StationID, lin);

                sSQL = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS WHERE LABEL_ITEM = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.ExecuteNonQuery();

                sSQL = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_LABEL_ITEMS_ZONES WHERE ID = '{0}' ", labelID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.ExecuteNonQuery();

                sSQL = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_VDE_ITEM_PARAMS WHERE LABEL_ID = {0} AND STATION_ID = {1}", labelID, Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.ExecuteNonQuery();

                //sSQL = string.Format("DELETE FROM " + Defaults.DB_Schema + ".XX_VDE_ITEM_ROT_PARAMS WHERE LABEL_ID = {0} AND STATION_ID = {1}", labelID, Defaults.StationID);
                //oCmd = new OracleCommand(sSQL, m_Conn);
                //oCmd.CommandType = CommandType.Text;
                //oCmd.ExecuteNonQuery();

                retVal = true;
            }
            catch (System.Exception ex)
            {
                retVal = false;
                string err = "DeleteTrainingData() err: " + ex.Message;

                notifyError(err, "Label Training", true);
                return retVal;
            }
            return retVal;
        }


        public static List<string> GetTrainingData(string lin, TrainingDataType tdt)
        {
            List<string> retVal = new List<string>();
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            string sSQL = "";
            try
            {
                //return null;
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                switch (tdt)
                {
                    case TrainingDataType.BARCODE:
                        sSQL = string.Format("SELECT VDE, VDE_NAME, LOCATION, NON_VDE_DATA FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_BARCODE WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                        break;
                    case TrainingDataType.LABEL:
                        //sSQL = string.Format("SELECT THRESHOLD, MIN_AREA FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_LABEL WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                        sSQL = string.Format("SELECT THRESHOLD FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_LABEL WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                        break;
                    case TrainingDataType.MASK:
                        sSQL = string.Format("SELECT LOCATION FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_MASK WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                        break;
                    case TrainingDataType.OPZONE:
                        //sSQL = string.Format("SELECT LOCATION, THRESHOLD, MIN_AREA, OPZONE_NAME FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_OPZONE WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                        sSQL = string.Format("SELECT LOCATION, THRESHOLD, OPZONE_NAME FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_OPZONE WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                        break;
                    case TrainingDataType.VDE:
                        sSQL = string.Format("SELECT PLACE_HOLDER, OPZONE, LOCATION FROM " + Defaults.DB_Schema + ".XX_TRAINING_DATA_VDE WHERE LIN = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);
                        break;
                }


                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count >= 1)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        switch (tdt)
                        {
                            case TrainingDataType.BARCODE:
                                retVal.Add(dr["VDE"].ToString());
                                retVal.Add(dr["VDE_NAME"].ToString());
                                retVal.Add(dr["LOCATION"].ToString());
                                retVal.Add(dr["NON_VDE_DATA"].ToString() + Environment.NewLine);
                                break;

                            case TrainingDataType.LABEL:
                                retVal.Add("THRESHOLD: " + dr["THRESHOLD"].ToString() + Environment.NewLine);
                                break;

                            case TrainingDataType.MASK:
                                retVal.Add("LOCATION: " + dr["LOCATION"].ToString() + Environment.NewLine);
                                break;

                            case TrainingDataType.OPZONE:
                                retVal.Add(dr["OPZONE_NAME"].ToString());
                                retVal.Add("LOCATION: " + dr["LOCATION"].ToString() + Environment.NewLine);
                                retVal.Add("THRESHOLD: " + dr["THRESHOLD"].ToString() + Environment.NewLine);
                                break;

                            case TrainingDataType.VDE:
                                retVal.Add("PLACE HOLDER:" + dr["PLACE_HOLDER"].ToString());
                                retVal.Add(dr["OPZONE"].ToString());
                                retVal.Add(dr["LOCATION"].ToString() + Environment.NewLine);
                                break;
                        }
                    }
                }
                //else
                //{
                //    ErrorDesription = string.Format("GetTrainingData(): LIN {0} produced no data:\tSELECT SQL: {1}", lin, sSQL);
                //    //retVal = null;
                //    return retVal;
                //}
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GetTrainingData() err: {0}\tSELECT\tSQL: {1}", ex.Message, sSQL);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }


        //public static int SaveLINData(string lin, bool complete)
        //{
        //    int retVal = 0;
        //    OracleCommand oCmd = new OracleCommand();
        //    OracleDataAdapter da = new OracleDataAdapter(oCmd);
        //    string sSQL = "";
        //    try
        //    {
        //        ErrorDesription = "";
        //        if (m_Conn == null)
        //            SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
        //        else if (m_Conn.State == ConnectionState.Closed)
        //            SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
        //        if (m_Conn == null)
        //            throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
        //        else if (m_Conn.State == ConnectionState.Closed)
        //            throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

        //        sSQL = "UPDATE " + Defaults.DB_Schema + ".XX_LABEL_ITEMS SET ";
        //        sSQL += string.Format("LABEL_ITEM = '{0}' ", lin);
        //        sSQL += string.Format("WHERE LABEL_ITEM = '{0}' AND STATION_ID = {1}", lin, Defaults.StationID);

        //        oCmd = new OracleCommand(sSQL, m_Conn);
        //        oCmd.CommandType = CommandType.Text;
        //        da.UpdateCommand = oCmd;
        //        int res = oCmd.ExecuteNonQuery();

        //        if (res <= 0)
        //        {
        //            //1. create new label
        //            oCmd = new OracleCommand("", m_Conn);
        //            oCmd.CommandType = CommandType.StoredProcedure;
        //            da.InsertCommand = oCmd;
        //            oCmd.CommandText = "XX_SP_INSERT";
        //            oCmd.Parameters.Add(new OracleParameter("STATION_ID", OracleDbType.Int16)).Value = Defaults.StationID;
        //            oCmd.Parameters.Add(new OracleParameter("LABEL_ITEM", OracleDbType.Varchar2, 20)).Value = lin;
        //            oCmd.Parameters.Add(new OracleParameter("LI_ID", OracleDbType.Int16)).Direction = ParameterDirection.Output;
        //            res = oCmd.ExecuteNonQuery();
        //            object tmp = oCmd.Parameters["LI_ID"].Value;
        //            Int16 itmp;
        //            Int16.TryParse(tmp.ToString(), out itmp);

        //            if (itmp > 0)
        //            {
        //                //2. get each place holder and variable name for the LIN
        //                sSQL = string.Format("SELECT VARIABLE_NAME, PLACE_HOLDER FROM " + Defaults.DB_Schema + ".XX_APP_LABEL_ITEM_VAR_NAMES_VW WHERE LABEL_ITEM = '{0}'", lin);
        //                oCmd = new OracleCommand(sSQL, m_Conn);
        //                oCmd.CommandType = CommandType.Text;
        //                DataSet ds = new DataSet();
        //                da = new OracleDataAdapter();
        //                da.SelectCommand = oCmd;
        //                oCmd.ExecuteNonQuery();
        //                da.Fill(ds);
        //                if (ds.Tables[0].Rows.Count > 0)
        //                {
        //                    foreach (DataRow dr in ds.Tables[0].Rows)
        //                    {
        //                        string varName = dr["VARIABLE_NAME"].ToString();
        //                        varName = varName.ToUpper().Trim();
        //                        varName = varName.Replace(" ", "_");

        //                        //3. iterate thru place holders and insert a row for each VDE item
        //                        oCmd = new OracleCommand("", m_Conn);
        //                        oCmd.CommandType = CommandType.StoredProcedure;
        //                        da.InsertCommand = oCmd;
        //                        oCmd.CommandText = "XX_SP_INSERT_LABEL_COLUMNS";
        //                        oCmd.Parameters.Add(new OracleParameter("LI_ID", OracleDbType.Int16)).Value = itmp;
        //                        oCmd.Parameters.Add(new OracleParameter("VARIABLE_NAME", OracleDbType.Varchar2, 50)).Value = varName;
        //                        oCmd.Parameters.Add(new OracleParameter("PLACE_HOLDER", OracleDbType.Varchar2, 50)).Value = dr["PLACE_HOLDER"].ToString();
        //                        oCmd.Parameters.Add(new OracleParameter("STATION_ID", OracleDbType.Int16)).Value = Defaults.StationID;
        //                        res = oCmd.ExecuteNonQuery();
        //                    }
        //                }
        //                retVal = itmp;
        //            }
        //            else
        //                throw new Exception("SaveLINData() err: Unable to update LIN VDE place-holders");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = 0;
        //        ErrorDesription = string.Format("SaveLINData() err: {0}\nUPDATE/INSERT\nSQL: {1}", ex.Message, sSQL);
        //        notifyError(ErrorDesription, "Data Access", true);
        //    }
        //    return retVal;
        //}

        public static List<PLCRegister> LoadPlcRegisters(int plc)
        {
            List<PLCRegister> retVal = new List<PLCRegister>();
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            string sSQL = "SELECT * FROM " + Defaults.DB_Schema + ".XX_FAIL_CODES ORDER BY FAIL_CODE ASC";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        //PLCRegisterType registertype = PLCRegisterType.UNASSIGNED;
                        //registertype = (PLCRegisterType)Convert.ToInt32(dr["REGISTER_TYPE"].ToString());
                        PLCRegister plcr = new PLCRegister();
                        plcr.Register = dr["FAIL_CODE"].ToString();
                        //plcr.Offset = Convert.ToInt32(dr["OFFSET"].ToString());
                        plcr.RegisterType = Enums.PLCRegisterType.Error;
                        plcr.Description = dr["FAIL_DESCRIPTION"].ToString();
                        //plcr.ExtendedInformation = dr["EXTENDED_INFORMATION"].ToString();
                        retVal.Add(plcr);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = "LoadPLCRegisters() err: " + ex.Message + "\n" + sSQL;
            }
            return retVal;
        }

        public static List<PLCFailCode> LoadFailCodes()
        {
            List<PLCFailCode> retVal = new List<PLCFailCode>();
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            string sSQL = "SELECT FAIL_CODE, FAIL_DESCRIPTION FROM XX_FAIL_CODES ORDER BY FAIL_CODE";
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        PLCFailCode fc = new PLCFailCode();
                        fc.FAIL_CODE = dr["FAIL_CODE"].ToString();
                        fc.FAIL_DESCRIPTION = dr["FAIL_DESCRIPTION"].ToString();
                        retVal.Add(fc);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = "LoadFailCodes() err: " + ex.Message + "\n" + sSQL;
            }
            return retVal;
        }

        public static int GetNextLPNSigID()
        {
            int retVal = -1;
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                OracleCommand oCmd = new OracleCommand();
                oCmd.Connection = m_Conn;
                oCmd.CommandText = "SELECT XX_GET_NEXT_LPN('XX_SQ_LPN_ESIGN') FROM DUAL";
                oCmd.CommandType = CommandType.Text;
                var result = oCmd.ExecuteScalar();
                if (result != null)
                {
                    string res = result.ToString();
                    int.TryParse(res, out retVal);
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
                ErrorDesription = "GetNextLPNSigID() err: " + ex.Message;
            }
            return retVal;
        }

        public static string GetMeaning(ESigReason meaningid)
        {
            string retVal = "";
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                string sSQL = string.Format("SELECT PROMPTMEANING FROM " + Defaults.DB_Schema + ".XX_LVS3_MEANINGS WHERE MEANING_ID = {0}", (int)meaningid);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count >= 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    try { retVal = dr["PROMPTMEANING"].ToString(); } catch (Exception ex) { ErrorDesription = ex.Message; retVal = ""; }
                }
            }
            catch (Exception ex)
            {
                ErrorDesription = "GetMeaning() err: " + ex.Message;
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static string getDWO(string lpn)
        {
            string retVal = "";
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                //select distinct detail_work_order FROM xx_variable_data_extract_vw WHERE reel_lpn = 'E163329-0018L-LP003-0006'
                string sSQL = string.Format("SELECT DETAIL_WORK_ORDER FROM " + Defaults.DB_Schema + ".XX_VARIABLE_DATA_EXTRACT_VW WHERE REEL_LPN = '{0}'", lpn);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count >= 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    try { retVal = dr[0].ToString(); } catch { retVal = ""; }
                    retVal = retVal.Trim();
                }
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("getDWO() err:\n{0}", ex.Message);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static DataSet GetSystemDevices()
        {
            DataSet retVal = null;
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                //string sSQL = "SELECT DEVICENAME, IP_ADDRESS, PORT_NUMBER, TYPE, VENDOR_ID, PRODUCT_ID, GROUP_ID FROM XX_SYSTEM_DEVICES ORDER BY TYPE, GROUP_ID";
                string sSQL = string.Format("SELECT * FROM XX_SYSTEM_DEVICES WHERE STATION_ID = {0} ORDER BY TYPE, GROUP_ID", Defaults.StationID);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count >= 1)
                    retVal = ds;
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = string.Format("GetSystemDevices() err:\n{0}", ex.Message);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static DeviceConfig GetScannerDeviceData()
        {
            DeviceConfig retVal = new DeviceConfig(true);
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                string sSQL = string.Format("SELECT DEVICENAME, PRODUCT_ID, VENDOR_ID FROM XX_SYSTEM_DEVICES WHERE TYPE = {0}", PeripheralType.Scanner);
                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count >= 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    retVal.name = dr["DEVICENAME"].ToString();
                    retVal.product = dr["PRODUCT_ID"].ToString();
                    retVal.vendor = dr["VENDOR_ID"].ToString();
                }
            }
            catch (Exception ex)
            {
                retVal.SUCCESS = false;
                ErrorDesription = string.Format("GetScannerDeviceData() err:\n{0}", ex.Message);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static string ApplicationSettingGet(string valuename)
        {
            string retVal = null;
            OracleCommand oCmd = new OracleCommand();
            OracleDataAdapter da = new OracleDataAdapter(oCmd);
            DataSet ds = new DataSet();
            try
            {
                ErrorDesription = "";
                if (m_Conn == null)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                else if (m_Conn.State == ConnectionState.Closed)
                    SqLiteDatabase.OpenOracleConnection(Defaults.SchemaToUse);
                if (m_Conn == null)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);
                else if (m_Conn.State == ConnectionState.Closed)
                    throw new Exception("Cannot open connection to Database schema: " + Defaults.DB_Schema);

                string sSQL = string.Format("SELECT SETTING_VALUE FROM XXACS_LVSIII.XX_APPLICATION_SETTINGS WHERE 1=1 AND SETTING_NAME = '{0};", valuename);

                oCmd = new OracleCommand(sSQL, m_Conn);
                oCmd.CommandType = CommandType.Text;
                da.SelectCommand = oCmd;
                oCmd.ExecuteNonQuery();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    if (dr != null)
                    {
                        retVal = dr[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = "";
                ErrorDesription = string.Format("ApplicationSettingGet({0}) err:\n{1}", valuename, ex.Message);
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static bool OpenOracleConnection(DatabaseSchema dbs)
        {
            bool retVal = true;

            try
            {
                string datasource = BuildOracleConnectionString();
                m_Conn = new OracleConnection(datasource);
                m_Conn.Open();
            }
            catch (Exception ex)
            {
                retVal = false;
                ErrorDesription = "OpenOracleConnection() err:\n" + ex.Message;
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static OracleConnection GetOracleConnection(DatabaseSchema dbs)
        {
            OracleConnection retVal;
            try
            {
                string datasource = BuildOracleConnectionString();
                retVal = new OracleConnection(datasource);
                retVal.Open();
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = "GetOracleConnection() err:\n" + ex.Message;
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        private static string BuildOracleConnectionString()
        {
            return string.Format(
                "user id={0};password={1};data source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST={2})(PORT={3}))(CONNECT_DATA=(SERVICE_NAME={4})))",
                Defaults.DB_UserName, Defaults.DB_Password, Defaults.IP, Defaults.PORT, Defaults.DB_ServiceName);
        }
        public static bool OpenSqLiteConnection()
        {
            var retVal = true;

            try
            {
                _mConn = new SQLiteConnection(ConnectionString);
                _mConn.Open();
            }
            catch (Exception ex)
            {
                retVal = false;
                ErrorDesription = "OpenSQLiteConnection() err:\n" + ex.Message;
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }

        public static SQLiteConnection GetSqLiteConnection(string connectionString)
        {
            SQLiteConnection retVal;
            try
            {
                retVal = new SQLiteConnection(connectionString);
                retVal.Open();
            }
            catch (Exception ex)
            {
                retVal = null;
                ErrorDesription = "GetSQLiteConnection() err:\n" + ex.Message;
                notifyError(ErrorDesription, "Data Access", true);
            }
            return retVal;
        }
    }

}

