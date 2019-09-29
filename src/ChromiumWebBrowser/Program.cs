using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using ChromiumWebBrowser.Core;
using ChromiumWebBrowser.Core.Misc.Bootstrap;
using ChromiumWebBrowser.Features.Downloader;
using ChromiumWebBrowser.Features.Downloader.Models;
using ChromiumWebBrowser.Misc.Helpers;
using NLog;

namespace ChromiumWebBrowser
{
    static class Program
    {

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            try
            {
                var env = MainPart1();
                var reg = MainPart2(env);
                MainConfigureCef(env, reg);
            }
            catch (Exception e)
            {
                _log.Error(e);
            }

            
            Application.Run(new Form1());
        }



        private static AppEnvironment MainPart1()
        {
            var asm = typeof(Program).Assembly;
            var env = AppEnvironmentBuilder.Instance.GetAppEnvironment(asm);
            NlogFeature.AddNLog(env.LogDir, env.AppNameSlug, LogLevel.Debug, env.AppVersion);
            Boot.Instance.AddAssembly(asm, AssemblyInProject.Main);
            Boot.Instance.AddAssembly(typeof(AssemblyCore).Assembly, AssemblyInProject.Core);
            
            return env;
        }

        private static CefRegistry MainPart2(AppEnvironment env)
        {
            var cnf = ConfigBuilder.Create();
            var reg = new CefRegistry(env, cnf);

            // It must be first thing here 
            DpiAwareness.SetProcessDpiAwareness(DpiAwareness.ProcessDpiAwareness.ProcessSystemDpiAware);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ProgramDownloader.DownloadCefSharpEnvIfNeeded(reg);
            return reg;
        }

        private static void MainConfigureCef(AppEnvironment env, CefRegistry reg)
        {
            //Monitor parent process exit and close subprocesses if parent process exits first
            //This will at some point in the future becomes the default
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            //For Windows 7 and above, best to include relevant app.manifest entries as well
            Cef.EnableHighDPISupport();

            var settings = new CefSettings
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(env.MiscDir, "chromium", "cache")
            };

            //Example of setting a command line argument
            //Enables WebRTC
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, false, browserProcessHandler: null);
        }

    }
}
