using System.Runtime.InteropServices;

namespace LVS3.External
{

    public class TaskBar
    {
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);
        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        public static void ShowTaskBar()
        {
            int hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_SHOW);
        }
        public static void HideTaskBar()
        {
            int hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_HIDE);
        }
    }
}
