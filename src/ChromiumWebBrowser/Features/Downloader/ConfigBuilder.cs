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
                Name = "cefsharp_86.0.24_x64",
                Nugets = new List<NugetInfo>
                {
                    new NugetInfo("CefSharp.Common", "86.0.241",
                        new List<CopyInfo> {new CopyInfo("/CefSharp/x64", "/")}),

                    new NugetInfo("CefSharp.WinForms", "86.0.241",
                        new List<CopyInfo> {new CopyInfo("/CefSharp/x64", "/")}),

                    new NugetInfo("cef.redist.x64", "86.0.24",
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