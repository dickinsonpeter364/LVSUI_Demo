using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LVS3;

namespace mxClient
{
    public class DummyPLCClient : ImxClient
    {
        public bool INIT()
        {
            return true;
        }

        public void InspectionLampOn()
        {
        }

        public void InspectionLampOff()
        {
        }

        public bool WriteToRegister(int plcid, string writeregister, int val, int attemptstoconnect)
        {
            return true;
        }

        public bool ReadRegister(int plcid, string readregister, ref int value, int attemptstoconnect)
        {
            if ( readregister == "At_Investigation")
                return false;
            value = 0;
            return true;
        }

        public bool Open(int plcid)
        {
            return true;
        }

        public bool StartForward(int plcid)
        {
            return true;
        }

        public bool StartReverse(int plcid)
        {
            return true;
        }

        public bool LabelIsAtStartPosition(int plcid)
        {
            return true;
        }

        public bool Stop(int plcid)
        {
            return true;
        }

        public List<string> ErrorRegisters(int plcid)
        {
            return new List<string>();
        }

        public bool ResetAlarm(int plcid)
        {
            return true;
        }

        public bool CheckAlarm(int plcid)
        {
            return false;
        }
    }
}
