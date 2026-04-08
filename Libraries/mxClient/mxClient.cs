using CONSTANTS;
using System.Diagnostics;
using S7.Net;
using Serilog;
using static LVS3.Enums;

namespace LVS3
{

    public class mxClient : ImxClient
    {
        public Plc PLC = null;

        public bool INIT()
        {
            bool retVal = false;
            try
            {
                PLC = new Plc(CpuType.S71500, Defaults.IPAddressPLC, 0, 1);
                retVal = true;
            }
            catch
            {
                return false;
            }
            return retVal;
        }

        public void InspectionLampOn()
        {
            //WriteToRegister(1, "M0", 1, 3);
        }

        public void InspectionLampOff()
        {
            //WriteToRegister(1, "M0", 0, 3);
        }

        public bool WriteToRegister(int plcid, string writeregister, int val, int attemptstoconnect)
        {
            bool retVal = false;
            try
            {
                string dataType = "";
                string siemens_Register = PLCIOMapping(writeregister, out dataType);

                retVal = Open(plcid); //
                if (retVal == false)
                {
                    return retVal;
                }

                if (dataType == "Bool")
                {
                    bool state = false;
                    if (val == 0)
                    {
                        state = false;
                    }
                    else if (val == 1)
                    {
                        state = true;
                    }
                    PLC.Write(siemens_Register, state);
                }
                else if (dataType == "Int")
                {
                    PLC.Write(siemens_Register, (short)val);
                }



                PLC.Close();
                retVal = true;
            }
            catch (Exception ex)
            {
                string err = "WriteToRegister() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "PLC COMMS", (int)CriticalLevels.Amber);
                Log.Logger.Error(err);
                return false;
            }
            return retVal;
        }

        public bool ReadRegister(int plcid, string readregister, ref int value, int attemptstoconnect)
        {
            bool retVal = false;
            try
            {
                string siemens_Register = "";
                string dataType = "";
                if (readregister.StartsWith("DB8"))
                {
                    siemens_Register = readregister;
                }
                else
                {
                    siemens_Register = PLCIOMapping(readregister, out dataType);
                }
                value = 0;
                retVal = Open(plcid);
                if (retVal == false)
                {
                    return retVal;
                }

                value = Convert.ToInt32(PLC.Read(siemens_Register));

                if (value == 0)
                {
                    retVal = true;
                    return retVal;
                }
                else if (value == 1)
                {
                    retVal = false;
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                string err = "ReadRegister() err: " + ex.Message;
                SystemMessageEventArgs smea = new SystemMessageEventArgs(err, "PLC COMMS", (int)CriticalLevels.Amber);
                Log.Logger.Error(err);
                //mxServer.doError(e.ToString());
            }
            return retVal;
        }

        public bool Open(int plcid)
        {

            bool retVal = false;
            try
            {
                if (PLC == null)
                    PLC = new Plc(CpuType.S71500, Defaults.IPAddressPLC, 0, 1);

                try { PLC.Close(); } catch { }

                try
                {
                    if (!PLC.IsConnected)
                    {
                        try { PLC.Open(); retVal = true; } catch { retVal = false; }
                    }
                }
                catch { }

                if (retVal == false)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (sw.ElapsedMilliseconds < 1000)
                        ;
                    sw.Stop();

                    PLC.Open();
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "Open() error opening plc:" + Environment.NewLine + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public bool StartForward(int plcid)
        {
            bool retVal = false;
            try
            {
                if (WriteToRegister(plcid, "Manual_FWD", 1, 3))
                    if (WriteToRegister(plcid, "Manual_FWD", 0, 3))
                        retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("StartForward() err: " + ex.Message, "Manual Control");
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public bool StartReverse(int plcid)
        {
            bool retVal = false;
            try
            {
                if (WriteToRegister(plcid, "Manual_RWD", 1, 3))
                    retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("StartReverse() err: " + ex.Message, "Manual Control");
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public bool LabelIsAtStartPosition(int plcid)
        {
            bool retVal = true;
            try
            {
                int rewoundToStartPosition = 0;
                ReadRegister(plcid, "Rewound_To_Start_Pos", ref rewoundToStartPosition, 3);
                if (rewoundToStartPosition != 0)
                    return true;
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "LabelIsAtStartPosition() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }

        public bool Stop(int plcid)
        {
            bool retVal = false;
            try
            {
                WriteToRegister(plcid, "Stop_UI", 1, 3);

            }
            catch (Exception ex)
            {
                retVal = false;
                string err = "Stop() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;

        }

        public List<string> ErrorRegisters(int plcid)
        {
            List<string> retVal = new List<string>();
            try
            {
                int val = 0;
                string dataType = "";
                string Alarms = PLCIOMapping("Get_Alarm_Word", out dataType);

                for (int range = 0; range <= 7; range++)
                {
                    val = 0;
                    string str = Alarms + range.ToString();
                    ReadRegister(plcid, str, ref val, 3);
                    if (val != 0)
                        retVal.Add(str);
                }
            }
            catch (Exception ex)
            {
                retVal = new List<string>();
                string err = "ErrorRegisters() err: " + ex.Message;
                Log.Logger.Error(err);
            }
            return retVal;
        }


        public bool ResetAlarm(int plcid)
        {
            bool retVal = true;
            try
            {
                retVal = WriteToRegister(plcid, "Reset_Alarms", 1, 3);
            }
            catch (Exception ex)
            {
                retVal = false;
                string err = string.Format("ResetAlarm() err: " + ex.Message, "PLC COMMS");
            }
            return retVal;
        }

        public bool CheckAlarm(int plcid)
        {
            bool retVal = false;
            int val = 0;
            try
            {
                ReadRegister(plcid, "Errors_Present", ref val, 3);
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

        private static string PLCIOMapping(string PLCTag, out string DataType)
        {
            string retVal = "";
            string Alarms = "DB8.DBX22.";
            switch (PLCTag)
            {
                case "Get_Alarm_Word": { retVal = Alarms; DataType = "Part_Address"; } break;

                case "Mode_Inspect": { retVal = "DB8.DBX0.0"; DataType = "Bool"; } break;
                case "Mode_Manual": { retVal = "DB8.DBX0.1"; DataType = "Bool"; } break;
                case "Mode_Teach": { retVal = "DB8.DBX0.2"; DataType = "Bool"; } break;

                case "Manual_FWD": { retVal = "DB8.DBX2.0"; DataType = "Bool"; } break;
                case "Manual_RWD": { retVal = "DB8.DBX2.1"; DataType = "Bool"; } break;

                case "Stop": { retVal = "DB8.DBX4.0"; DataType = "Bool"; } break;
                case "Stop_UI": { retVal = "DB8.DBX4.1"; DataType = "Bool"; } break;
                case "Stop_Mode": { retVal = "DB8.DBX4.2"; DataType = "Bool"; } break;
                case "Reset_Alarms": { retVal = "DB8.DBX4.3"; DataType = "Bool"; } break;
                case "Capture_Image": { retVal = "DB8.DBX4.4"; DataType = "Bool"; } break;
                case "Rewind_After_INV": { retVal = "DB8.DBX4.5"; DataType = "Bool"; } break;
                case "REV_Override": { retVal = "DB8.DBX4.6"; DataType = "Bool"; } break;
                case "Missing_Label_Inhibit": { retVal = "DB8.DBX4.7"; DataType = "Bool"; } break;
                case "Doc_Sample_Included": { retVal = "DB8.DBX5.0"; DataType = "Bool"; } break;
                case "Doc_Sample_Not_Included": { retVal = "DB8.DBX5.1"; DataType = "Bool"; } break;
                case "FWD_Small_Core_Selected": { retVal = "DB8.DBX5.2"; DataType = "Bool"; } break;
                case "FWD_Large_Core_Selected": { retVal = "DB8.DBX5.3"; DataType = "Bool"; } break;
                case "RWD_Small_Core_Selected": { retVal = "DB8.DBX5.4"; DataType = "Bool"; } break;
                case "RWD_Large_Core_Selected": { retVal = "DB8.DBX5.5"; DataType = "Bool"; } break;
                case "Inspection_Cancelled": { retVal = "DB8.DBX5.6"; DataType = "Bool"; } break;

                // Ints output to PLC
                case "Speed_Top_Limit": { retVal = "DB8.DBW6"; DataType = "Int"; } break;
                case "Label_Count": { retVal = "DB8.DBW8"; DataType = "Int"; } break;
                case "RWD_Radius": { retVal = "DB8.DBW10"; DataType = "Int"; } break;
                case "Test_Image_Count": { retVal = "DB8.DBW12"; DataType = "Int"; } break;
                case "Speed_Control": { retVal = "DB8.DBW14"; DataType = "Int"; } break;
                case "FWD_Manual_Speed_SP": { retVal = "DB8.DBW16"; DataType = "Int"; } break;
                case "RWD_Manual_Speed_SP": { retVal = "DB8.DBW18"; DataType = "Int"; } break;

                // Bits Input from PLC
                case "At_Investigation": { retVal = "DB8.DBX20.0"; DataType = "Bool"; } break;
                case "Errors_Present": { retVal = "DB8.DBX20.1"; DataType = "Bool"; } break;
                case "Alarms_Estop_Activated": { retVal = Alarms + "0"; DataType = "Bool"; } break;
                case "Alarms_LHS_Door_Open": { retVal = Alarms + "1"; DataType = "Bool"; } break;
                case "Alarms_RHS_Door_Open": { retVal = Alarms + "2"; DataType = "Bool"; } break;
                case "Alarms_Air_Pressure_Low": { retVal = Alarms + "3"; DataType = "Bool"; } break;
                case "Alarms_8U1_Common_Fault": { retVal = Alarms + "4"; DataType = "Bool"; } break;
                case "Alarms_9U1_Common_Fault": { retVal = Alarms + "5"; DataType = "Bool"; } break;
                case "Alarms_Guider_Not_OK": { retVal = Alarms + "6"; DataType = "Bool"; } break;
                case "Alarms_Web_Loss": { retVal = Alarms + "7"; DataType = "Bool"; } break;

                // Ints Input from PLC
                case "Rewound_To_Start_Pos": { retVal = "DB8.DBW24"; DataType = "Int"; } break;

                default: { retVal = ""; DataType = "Bool"; } break;
            }

            return retVal;
        }
    }

    public class InspectionModes
    {
        private static string m_Manual = "MANUAL MODE";
        private static string m_Auto = "INSPECTION MODE";
        private static string m_Teach = "TEACH MODE";

        public string MANUAL
        {
            get { return m_Manual; }
        }
        public string AUTO
        {
            get { return m_Auto; }
        }
        public string TEACH
        {
            get { return m_Teach; }
        }
    }
}

