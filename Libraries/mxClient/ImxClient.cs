namespace LVS3;

public interface ImxClient
{
    bool INIT();
    void InspectionLampOn();
    void InspectionLampOff();
    bool WriteToRegister(int plcid, string writeregister, int val, int attemptstoconnect);
    bool ReadRegister(int plcid, string readregister, ref int value, int attemptstoconnect);
    bool Open(int plcid);
    bool StartForward(int plcid);
    bool StartReverse(int plcid);
    bool LabelIsAtStartPosition(int plcid);
    bool Stop(int plcid);
    List<string> ErrorRegisters(int plcid);
    bool ResetAlarm(int plcid);
    bool CheckAlarm(int plcid);
}