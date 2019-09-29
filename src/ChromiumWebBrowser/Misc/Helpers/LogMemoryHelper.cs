using System.Diagnostics;
using NLog;

namespace ChromiumWebBrowser.Misc.Helpers
{
   public static class LogMemoryHelper
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();


        public static void LogMemoryUsage(string msg)
        {
            var currentProcess = Process.GetCurrentProcess();
            var processAllocatedMemory = currentProcess.WorkingSet64;
            _log.Debug($"Memoru usage: {processAllocatedMemory / (1024 * 1024)}MB; Check: {msg}");
        }
    }
}
