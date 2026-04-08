using System.Drawing;

namespace LVS3
{

    public interface IStatusInformation
    {
         void SetStatusInformation(StatusLevel state);
    }
    
    public interface ILabelInspection
    {
        void ClearToolData();
        object Execute(Bitmap Image);
        int ltGetID();
        void ResetResultDisplay();
        void SetControlParameters(string seedtext, int top, int left, int bottom, int width, object hwin, bool darkonlight, int searchmethod);
        void SetID(int ID);
    }


    public interface IDevice
    {
        string ComponentName { get;  }
        DeviceConfig DS { get; }
        object? uscControl { get; set;  }
    }


}

