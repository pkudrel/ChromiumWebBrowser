using System;
using System.Diagnostics;
using System.Text;
using ChromiumWebBrowser.Core.Misc.Bootstrap;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Windows.Forms;

namespace ChromiumWebBrowser.Misc.Helpers
{
    public static class NlogFeature
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static void AddNLog(string logDirectory, string appName, LogLevel logLevel, AppVersion appVersion)
        {
            var config = Part1(logDirectory, appName, logLevel);

            LogVersion(appVersion);

            // winforms logger 

            Part2(config);
        }

        private static void Part2(LoggingConfiguration config)
        {
            ConfigurationItemFactory
                .Default
                .Targets
                .RegisterDefinition("FormControl", typeof(FormControlTarget));


            var formControlTarget = new FormControlTarget
            {
                Name = "form-winform",
                Layout = "${message}${newline}",
                Append = true,
                ReverseOrder = false,
                ControlName = "textInfo",
                FormName = "MainForm"
            };
            config.AddTarget(formControlTarget);
            config.AddRule(LogLevel.Info, LogLevel.Info, formControlTarget, "formInfo");

            LogManager.Configuration = config;
        }

        private static LoggingConfiguration Part1(string logDirectory, string appName, LogLevel logLevel)
        {
            var name = appName;
            var config = new LoggingConfiguration();
            config.Variables["logDirectory"] = logDirectory;
            config.Variables["appName"] = name;

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = "${var:logDirectory}/${var:appName}.log.txt";
            fileTarget.ArchiveFileName = "${var:logDirectory}/archives//${var:appName}.{#}.log.txt";
            fileTarget.Layout = "${longdate}|${level:uppercase=true}|${threadid}|${logger}|${message}";
            fileTarget.ArchiveEvery = FileArchivePeriod.Day;
            fileTarget.Encoding = Encoding.UTF8;
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.Rolling;
            fileTarget.MaxArchiveFiles = 12;
            var ruleFile = new LoggingRule("*", logLevel, fileTarget);
            config.LoggingRules.Add(ruleFile);

            LogManager.Configuration = config;
            return config;
        }

        private static void LogVersion(AppVersion ver)
        {

            var sem = ver.SemVer ?? ver.MainVersion;
            var full = ver.FullInfo;
            var isX64 = Environment.Is64BitProcess;
            _log.Info(
                $"Application version; Sem: {sem}; Full: {full} ; " +
                $"Up time: {GetUpTime()} ; " +
                $"Is64Bit: {isX64}");
            var aa = 1;
        }

        public static TimeSpan GetUpTime()
        {
            using (var uptime = new PerformanceCounter("System", "System Up Time"))
            {
                uptime.NextValue(); //Call this an extra time before reading its value
                return TimeSpan.FromSeconds(uptime.NextValue());
            }
        }
    }
}