using System.Collections.Generic;
using ChromiumWebBrowser.Features.Downloader.Models;

namespace ChromiumWebBrowser.Features.Downloader
{
    public static class ConfigBuilder
    {
        public static CefConfig Create()
        {
            var ret = new CefConfig {PackageConfig = GetPackageConfig()};
            return ret;
        }

        private static PackageConfig GetPackageConfig()
        {
            return new PackageConfig
            {
                Name = "cefsharp_85.3.13_x64",
                Nugets = new List<NugetInfo>
                {
                    new NugetInfo("CefSharp.Common", "85.3.130",
                        new List<CopyInfo> {new CopyInfo("/CefSharp/x64", "/")}),

                    new NugetInfo("CefSharp.WinForms", "85.3.130",
                        new List<CopyInfo> {new CopyInfo("/CefSharp/x64", "/")}),

                    new NugetInfo("cef.redist.x64", "85.3.13",
                        new List<CopyInfo>
                        {
                            new CopyInfo("/CEF", "/"),
                            new CopyInfo("/CEF/locales", "/locales"),
                            new CopyInfo("/CEF/swiftshader", "/swiftshader")
                        })
                }
            };
        }
    }
}