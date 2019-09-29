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
                Name = "cefsharp_73.1.13_x64",
                Nugets = new List<NugetInfo>
                {
                    new NugetInfo("CefSharp.Common", "73.1.130",
                        new List<CopyInfo> {new CopyInfo("/CefSharp/x64", "/")}),

                    new NugetInfo("CefSharp.WinForms", "73.1.130",
                        new List<CopyInfo> {new CopyInfo("/CefSharp/x64", "/")}),

                    new NugetInfo("cef.redist.x64", "73.1.13",
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