using System.IO;
using ChromiumWebBrowser.Core.Misc.Bootstrap;

namespace ChromiumWebBrowser.Features.Downloader.Models
{
    public class CefRegistry
    {
        public CefRegistry(AppEnvironment env, CefConfig config)
        {
            PackageConfig = config.PackageConfig;
            CefSharpPackageName = config.PackageConfig.Name;
            CefSharpEnvStorePath = Path.Combine(env.CefBinariesDir, "CefSharp", "packages");
            GlobalCefSharpEnvPath = Path.Combine(CefSharpEnvStorePath, config.PackageConfig.Name);
            CefSharpLocalePath = Path.Combine(GlobalCefSharpEnvPath, "locales");
            GlobalBrowserSubprocessPath = Path.Combine(GlobalCefSharpEnvPath, "CefSharp.BrowserSubprocess.exe");
            LocalCefSharpEnvPath = env.ExeFileDir;
            LocalBrowserSubprocessPath = Path.Combine(LocalCefSharpEnvPath, "CefSharp.BrowserSubprocess.exe");
            Core.Common.Io.Misc.CreateDirIfNotExist(CefSharpEnvStorePath);
        }

        public string CefSharpLocalePath { get; }
        public PackageConfig PackageConfig { get; }
        public string GlobalBrowserSubprocessPath { get; }
        public string LocalBrowserSubprocessPath { get; }
        public string GlobalCefSharpEnvPath { get; }
        public string LocalCefSharpEnvPath { get; }
        public string CefSharpPackageName { get; }
        public string CefSharpEnvStorePath { get; }
    }
}