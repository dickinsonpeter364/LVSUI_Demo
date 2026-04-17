using System;
using System.Diagnostics;

namespace LVS3
{
    public static class MELSEC
    {
        public static string Errors = "";
        public static ActUtlTypeLib.ActUtlType PLC;
        private static int[] PLC_ARRAY_DATA = new int[200];

        public static bool Init()
        {
            bool retVal = true;
            Errors = "";
            try
            {
                PLC = new ActUtlTypeLib.ActUtlType();
            }
            catch(Exception ex)
            {
                Errors = "MELSEC.Init() err: " + ex.Message;
                retVal = false;
            }
            return retVal;
        }

        //public static bool loadPLCRegistersFromSpreadsheet()
        //{
        //    bool retVal = false;
        //    try
        //    {
        //        if (PLCRegisters != null)
        //            PLCRegisters.Clear();
        //        else
        //            PLCRegisters = new List<PLCRegister>();
        //        Excel.Application excelApp = new Excel.Application();
        //        string filename = UtilityFunctions.OpenFile();
        //        if (filename == "")
        //        {
        //            SL.Details = string.Format("PLC #{0}:\nloadPLCRegistersFromSpreadsheet()\nLoading PLC Registers from spreadsheet:\nxlsx workbook not ready.", PLC_ID);
        //            SL.Status = (int)(int)StatusLevels.WARNING;
        //            UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, false);
        //            return retVal;
        //        }
        //        Workbook workBook = excelApp.Workbooks.Open(@filename);
        //        Worksheet workSheet = workBook.Sheets[PLC_ID];
        //        string baseregister = "";
        //        int offset = 0;
        //        int x = 0;
        //        for (x = 2; x <= 200; x++)
        //        {
        //            PLCRegisterType registertype;
        //            string description = "";
        //            string extendedinformation = "";
        //            if (x == 2)
        //                try { baseregister = workSheet.Cells[x, 2].Text.ToString(); } catch { baseregister = ""; break; }
        //            try { offset = Convert.ToInt32(workSheet.Cells[x, 1].Text.ToString()); } catch { offset = 0; break; }
        //            try { registertype = workSheet.Cells[x, 3].Text.ToString(); } catch { continue; }
        //            try { description = workSheet.Cells[x, 4].Text.ToString(); } catch { description = ""; }
        //            try { extendedinformation = workSheet.Cells[x, 5].Text.ToString(); } catch { extendedinformation = ""; }
        //            string tmp = workSheet.Cells[x, 3].Text.ToString();
        //            switch (tmp)
        //            {
        //                case "PLC_READY":
        //                    registertype = PLCRegisterType.PLCReady;
        //                    break;
        //                case "KIT_COMPLETE":
        //                    registertype = PLCRegisterType.KitComplete;
        //                    break;
        //                case "P1_COUNT":
        //                    registertype = PLCRegisterType.ProductCount;
        //                    break;
        //                case "P2_COUNT":
        //                    registertype = PLCRegisterType.KitCount;
        //                    break;
        //                case "ERROR":
        //                    registertype = PLCRegisterType.Error;
        //                    break;
        //                case "WARNING":
        //                    registertype = PLCRegisterType.Warning;
        //                    break;
        //                case "COUNTER":
        //                    registertype = PLCRegisterType.Counter;
        //                    break;
        //            }
        //            PLCRegister plcr = new PLCRegister();
        //            plcr.BaseRegister = baseregister;
        //            plcr.Offset = offset;
        //            plcr.RegisterType = registertype;
        //            plcr.Description = description;
        //            plcr.ExtendedInformation = extendedinformation;
        //            PLCRegisters.Add(plcr);
        //        }
        //        if (PLCRegisters.Count < 4)
        //            throw new Exception(string.Format("Register array spreadsheet ({0}) has too few register entries.", filename));
        //        if (x <= 2 || offset == 0)
        //            throw new Exception(string.Format("Register array spreadsheet ({0}) is not in correct format", filename));
        //        try { workBook.Close(false); } catch { }
        //        try { workBook = null; } catch { }
        //        try { workSheet = null; } catch { }
        //        try { excelApp.Quit(); } catch { }
        //        try { excelApp = null; } catch { }
        //        retVal = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = false;
        //        SL.Details = string.Format("PLC #{0}:\nloadPLCRegistersFromSpreadsheet() err: " + ex.Message, 1);
        //        SL.Status = (int)StatusLevels.WARNING;
        //        UtilityFunctions.DoStatusGUI(SL, lblComponentName, lblStatus, false);
        //    }
        //    return retVal;
        //}

        public static bool Open(int plcid)
        {
            bool retVal = false;
            try
            {
                Errors = "";
                long lret;
                PLC.ActLogicalStationNumber = plcid;

                // Close the PLC1 Connection first
                PLC.Close();

                // Try to Open Communications to the Mits PLC 
                lret = PLC.Open();

                // Check that the PLC # Communications are OK
                if (lret != 0)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (sw.ElapsedMilliseconds < 1000)
                        ;
                    sw.Stop();
                    lret = PLC.Open();
                    //int val = 999;
                    //PLC.ReadDeviceRandom("D0", 1, out val);
                    if (lret != 0)
                    {
                        Errors = string.Format("Cannot connect to plc # {0}.\n\nSystem cannot continue", plcid);
                    }
                    else
                        retVal=true;
                }
                else
                    retVal = true;
            }
            catch (Exception ex)
            {
                Errors = string.Format("Open() err: " + ex.Message + "\nCannot connect to plc # {0}.\n\nSystem cannot continue", plcid);
                retVal = false;
            }
            return retVal;
        }


        /// <summary>
        /// Returns value at plc array[0] - PLC line is ready to start. Use ComponentReady() to check plc is online
        /// </summary>
        /// <returns></returns>
        public static bool PLCReady(int plcid, string readyregister)
        {
            bool retVal = false;
            int iVal;
            int iret;
            try
            {
                Errors = "";
                retVal = Open(plcid);
                if (retVal == false)
                {
                    Errors = "Cannot Open PLC device #" + plcid.ToString();
                    return retVal;
                }
                iret = PLC.ReadDeviceRandom(readyregister, 1, out iVal);
                if (iret != 0)
                {
                    Errors = "Cannot read data array from PLC device #" + plcid.ToString();
                    retVal = false;
                    return retVal;
                }
                else
                    retVal = true;
            }
            catch (Exception ex)
            {
                Errors = string.Format("PLCReady() err: " + ex.Message + "\nCannot connect to plc # {0}.\n\nSystem cannot continue", plcid);
                retVal = false;
            }
            return retVal;
        }

        public static bool ReadRegister(int plcid, string readregister, ref int value)
        {
            bool retVal = false;
            int iret;
            try
            {
                value = 0;
                Errors = "";
                retVal = Open(plcid);
                if (retVal == false)
                {
                    Errors = "Cannot Open PLC device #" + plcid.ToString();
                    return retVal;
                }
                iret = PLC.ReadDeviceRandom(readregister, 1, out value);
                if (iret != 0)
                {
                    Errors = "Cannot read data from PLC device #" + plcid.ToString();
                    retVal = false;
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                Errors = string.Format("ReadRegister() err: " + ex.Message + "\nCannot connect to plc # {0}.\n\nSystem cannot continue", plcid);
                retVal = false;
            }
            return retVal;
        }

        public static bool WriteToRegister(int plcid, string register, int val)
        {
            bool retVal = false;
            int iret;
            try
            {
                Errors = "";
                retVal = Open(plcid);
                if (retVal == false)
                {
                    Errors = "Cannot Open PLC device #" + plcid.ToString();
                    return retVal;
                }
                iret = PLC.WriteDeviceRandom(register,1, val);
                if (iret != 0)
                {
                    Errors = "Cannot write data to PLC device #" + plcid.ToString();
                    retVal= false;
                    return retVal;
                }
                retVal = true;
            }
            catch (Exception ex)
            {
                Errors = string.Format("WriteToRegister() err: " + ex.Message + "\nCannot connect to plc # {0}.\n\nSystem cannot continue", plcid);
                retVal = false;
            }
            return retVal;
        }

    }
}
