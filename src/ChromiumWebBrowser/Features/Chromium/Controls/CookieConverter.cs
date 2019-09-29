

using System.Collections.Generic;
using ChromiumWebBrowser.Core.Features.Chromium.Models;

namespace ChromiumWebBrowser.Features.Chromium.Controls
{
    internal static class CookieConverter
    {
        public static List<Cookie> ConvertList(List<CefSharp.Cookie> listCefSharpCookie)
        {
            var list = new List<Cookie>();
            foreach (var cookie in listCefSharpCookie)
                list.Add(new Cookie(cookie.Domain, cookie.Path, cookie.Name, cookie.Value));
            return list;
        }

        public static Cookie Convert(CefSharp.Cookie cookie)
        {
            return new Cookie(cookie.Domain, cookie.Path, cookie.Name, cookie.Value);
        }
    }
}