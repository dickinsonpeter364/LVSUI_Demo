using System.Diagnostics;
//using static LVS3.Enums;

namespace LVS3
{
    public static class Delegates
    {
        public delegate void ProgressHandler(int progress);
        public delegate void AlarmMethodHandler();
        public delegate void ErrorMethodHandler(string error);
        public delegate void ShowBackingCameraHandler();
        public delegate void HideBackingCameraHandler();
        public delegate void DeviceConfigMessageHandler(string text, string title);
        public delegate void SystemMessageHandler(SystemMessageEventArgs smea);
        public delegate void MSGHandler(MSGEventArgs e);
        public delegate void SysMessageDelegate(string message, bool bold, bool critical);
        public delegate void UpdateInterfaceDelegate();
        public delegate void DoWarningsDelegate();
        public delegate void DoAlarmsDelegate(bool running);
        public delegate void DoPKDelegate();
        public delegate void DoTimeoutDelegate();

        public delegate void ComponentDataHandler(object sender, ComponentDataArgs e);
        public delegate void AlarmHandlerInvoker(bool running);
        public delegate void UpdateInterfaceInvoker();
        public delegate void MenuHandlerInvoker(string menuoption);
        public delegate void ShowHandlerInvoker(bool show);
        public delegate void EventMessageHandler(string e);
        public delegate void IO_INTERRUPT_Handler(int channel, IOEventArgs e);
        public delegate void IO_CHANGE_Handler(int channel, bool channelstate);
    }


    //ISetupForm
    public class SystemMessageEventArgs : EventArgs
    {
        public string TITLE = "";
        public string MSG = "";
        public int CL;
        public int FailID;

        public SystemMessageEventArgs(string msg, string title, int cl, int failid = -1)
        {
            TITLE = title;
            MSG = msg;
            CL = cl;
            FailID = failid;
        }
    }

    public class ComponentDataArgs : EventArgs
    {
        public string RecievedData = "";

        public ComponentDataArgs(string receiveddata)
        {
            this.RecievedData = receiveddata;
        }
    }

    public class MSGEventArgs : EventArgs
    {
        public bool ShowDialog = false;
        public string Message = "";
        public EventLogEntryType ErrorLevel = EventLogEntryType.Information;

        public MSGEventArgs(string message, EventLogEntryType errorlevel, bool showdialog)
        {
            this.Message = message;
            this.ErrorLevel = errorlevel;
            this.ShowDialog = showdialog;
        }
    }

    public class IOEventArgs : EventArgs
    {
        public int Channel = 0;
        public bool IsHigh = false;

        public IOEventArgs(int channel, bool IsHigh)
        {
            this.Channel = channel;
            this.IsHigh = IsHigh;
        }
    }
}