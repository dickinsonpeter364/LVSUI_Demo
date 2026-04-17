using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
//using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using LVS3;
using static LVS3.Enums;
using System.Windows.Forms;
using CONSTANTS;
using System.Collections.Generic;

namespace mxServer
{
    public static class MXSERVER
    {
        public static ActUtlType64Lib.ActUtlType64 PLC;

        public static bool INIT()
        {
            bool retVal = false;
            try
            {
                PLC = new ActUtlType64Lib.ActUtlType64();
                retVal = true;
            }
            catch
            {
                return false;
            }
            return retVal;
        }

        public static bool WriteToRegister(int plcid, string writeregister, int val, int attemptstoconnect)
        {
            bool retVal = false;
            int iret;
            try
            {
                retVal = Open(plcid);
                if (retVal == false)
                {
                    attemptstoconnect += 1;
                    WriteToRegister(plcid, writeregister, val, attemptstoconnect);
                    if (attemptstoconnect >= 3)
                        return retVal;
                }
                iret = PLC.WriteDeviceRandom(writeregister, 1, val);
                if (iret != 0)
                {
                    retVal = false;
                    return retVal;
                }
                retVal = true;
            }
            catch (Exception ex)
            {
                string err = "WriteToRegister() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "PLC COMMS", (int)CriticalLevels.Amber);
                MessageBox.Show(err);
            }
            return retVal;
        }
        
        public static bool ReadRegister(int plcid, string readregister, ref int value, int attemptstoconnect)
        {
            bool retVal = false;
            int iret;
            try
            {
                value = 0;
                retVal = Open(plcid);
                if (retVal == false)
                {
                    attemptstoconnect += 1;
                    ReadRegister(plcid, readregister, ref value, attemptstoconnect);
                    if (attemptstoconnect >= 3)
                        return retVal;
                }
                iret = PLC.ReadDeviceRandom(readregister, 1, out value);
                if (iret != 0)
                {
                    retVal = false;
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                string err = "ReadRegister() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "PLC COMMS", (int)CriticalLevels.Amber);
                MessageBox.Show(err);
                //mxServer.doError(e.ToString());
            }

            return retVal;
        }

        public static bool Open(int plcid)
        {
            bool retVal = false;
            try
            {
                if (PLC == null)
                    PLC = new ActUtlType64Lib.ActUtlType64();
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
                    while (sw.ElapsedMilliseconds < 500)
                        ;
                    sw.Stop();
                    lret = PLC.Open();
                    if (lret != 0)
                        retVal = false;

                    else
                        retVal = true;
                }
                else
                    retVal = true;
            }
            catch (Exception ex)
            {
                string err = "Open() error opening plc:" + Environment.NewLine + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        public static bool StartForward(int plcid)
        {
            bool retVal = false;
            try
            {
                if (WriteToRegister(plcid, "M3", 1,3))
                    if (WriteToRegister(plcid, "M3", 0, 3))
                        retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("StartForward() err: " + ex.Message, "Manual Control");
                MessageBox.Show(err);
            }
            return retVal;
        }

        public static bool StartReverse(int plcid)
        {
            //string ReverseRegister = "D100";
            bool retVal = false;
            try
            {
                if (WriteToRegister(plcid, "M4", 1, 3))
                    if (WriteToRegister(plcid, "M4", 0, 3))
                        retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("StartReverse() err: " + ex.Message, "Manual Control");
                MessageBox.Show(err);
            }
            return retVal;
        }

        public static bool LabelIsAtStartPosition(int plcid)
        {
            bool retVal = true;
            try
            {
                int rewoundToStartPosition = 0;
                ReadRegister(plcid, "D120", ref rewoundToStartPosition,3);
                if (rewoundToStartPosition <= 0)
                    return true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "LabelIsAtStartPosition() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;
        }

        public static bool Stop(int plcid)
        {
            bool retVal = false;
            string EORRegister = "M10";
            try
            {
                WriteToRegister(plcid, EORRegister, 0, 3);
                WriteToRegister(plcid, EORRegister, 1, 3);
                WriteToRegister(plcid, EORRegister, 0, 3);
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "Stop() err: " + ex.Message;
                MessageBox.Show(err);
            }
            return retVal;

        }

        public static List<string> ErrorRegisters(int plcid)
        {
            List<string> retVal = new List<string>();
            string error = "";
            try
            {
                int val = 0;

                for (int range = 1001; range <= 1008; range++)
                {
                    val = 0;
                    string str = "M" + range.ToString();
                    ReadRegister(plcid, str, ref val, 3);
                    if (val != 0)
                        retVal.Add(str);
                }
            }
            catch (Exception ex)
            {
                retVal = new List<string>();
                string err = "ErrorRegisters() err: " + ex.Message;
                MessageBox.Show(err); 
            }
            return retVal;
        }

        //public static List<string> RegisterArray(int plcid, string startregister, int arraycount)
        //{
        //    List<string> retVal = new List<string>();
        //    try
        //    {  //1|ReadDeviceBlock|M1001|8<EOF>
        //        string str = plcid.ToString() + "|ReadDeviceBlock|" + startregister + "|0" + arraycount.ToString() + "<EOF>";
        //        if (StartClient(str))
        //        {
        //            object valRet = Convert.ToInt32(responseData);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = new List<string>();
        //        string err = "RegisterArray() err: " + ex.Message;
        //        MessageBox.Show(err);
        //    }
        //    return retVal;
        //}

        public static bool ResetAlarm(int plcid)
        {
            bool retVal = true;
            try
            {
                Stopwatch sw = new Stopwatch();
                WriteToRegister(plcid, "M1000", 1, 3);
                sw.Start();
                while (sw.ElapsedMilliseconds < 25)
                    ;
                sw.Stop();
                retVal = WriteToRegister(plcid, "M1000", 0, 3);
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("ResetAlarm() err: " + ex.Message, "PLC COMMS");
            }
            return retVal;
        }

        public static bool CheckAlarm(int plcid)
        {
            bool retVal = false;
            int val = 0;
            try
            {
                ReadRegister(plcid, "M1050", ref val, 3);
                if (val == 1)
                    retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("CheckAlarm() err: " + ex.Message + "\nCannot connect to plc # {0}.\n\nSystem cannot continue", plcid);
            }
            return retVal;
        }


    }

    
}


