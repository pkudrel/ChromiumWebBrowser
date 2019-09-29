using System;
using System.Runtime.InteropServices;

namespace ChromiumWebBrowser.Misc.Helpers
{
    public static class DpiAwareness
    {

        [DllImport("SHCore.dll", SetLastError = true)]
        public static extern bool SetProcessDpiAwareness(ProcessDpiAwareness awareness);

        [DllImport("SHCore.dll", SetLastError = true)]
        public static extern void GetProcessDpiAwareness(IntPtr hprocess, out ProcessDpiAwareness awareness);

        public enum ProcessDpiAwareness
        {
            ProcessDpiUnaware = 0,
            ProcessSystemDpiAware = 1,
            ProcessPerMonitorDpiAware = 2
        }
    }
}