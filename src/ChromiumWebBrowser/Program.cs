using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autofac;
using CefSharp;
using CefSharp.WinForms;
using ChromiumWebBrowser.Core;
using ChromiumWebBrowser.Core.App.Bootstrap;
using ChromiumWebBrowser.Core.App.Models;
using ChromiumWebBrowser.Core.App.ReqRes;
using ChromiumWebBrowser.Core.Features.Projects.Models;
using ChromiumWebBrowser.Features.Downloader;
using ChromiumWebBrowser.Features.Downloader.Models;
using ChromiumWebBrowser.Features.MainFormView.Views;
using ChromiumWebBrowser.Features.Projects.Models;
using ChromiumWebBrowser.Misc.Helpers;
using MediatR;
using Newtonsoft.Json;
using NLog;

namespace ChromiumWebBrowser
{
    internal static class Program
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            try
            {
                var env = MainPart1();
                var reg = MainPart2(env);
                MainConfigureCef(env, reg);

                var builder = new ContainerBuilder();
                builder.RegisterAssemblyModules(Boot.Instance.GetAssemblies());
                builder.RegisterInstance(env);
                builder.RegisterInstance(reg);

                using (var container = builder.Build())
                {
                    using (var scope = container.BeginLifetimeScope())
                    {
                        var mediator = scope.Resolve<IMediator>();
                        _log.Debug($"Rise 'AppStartingEvent' event");
                        await mediator.Publish(new AppStartingEvent());
                        _log.Debug($"Rise 'AppStartedEvent' event");
                        await mediator.Publish(new AppStartedEvent());
                        var form = scope.Resolve<MainForm>();
                        Application.Run(form);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        private static AppEnvironment MainPart1()
        {
            var asm = typeof(Program).Assembly;
            var env = Boot.Instance.Start(asm);
            NlogFeature.AddNLog(env.LogDir, env.AppNameSlug, LogLevel.Debug, env.AppVersion);
            Boot.Instance.AddAssembly(asm, AssemblyInProject.Main);
            Boot.Instance.AddAssembly(typeof(AssemblyCore).Assembly, AssemblyInProject.Core);

            return env;
        }

        private static AppRegistry MainPart2(AppEnvironment env)
        {
            // get config 
            var pathToConfig = Path.Combine(env.ConfigDir, Consts.APP_CONFIG_FILE);
            var json = File.ReadAllText(pathToConfig);
            var config = JsonConvert.DeserializeObject<AppConfig>(json);


            var cnf = ConfigBuilder.Create();
            var reg = new CefRegistry(env, cnf);

            // It must be first thing here 
            DpiAwareness.SetProcessDpiAwareness(DpiAwareness.ProcessDpiAwareness.ProcessSystemDpiAware);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ProgramDownloader.DownloadCefSharpEnvIfNeeded(reg);
            var registry = new AppRegistry(env, config);
            return registry;
        }

        private static void MainConfigureCef(AppEnvironment env, AppRegistry reg)
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