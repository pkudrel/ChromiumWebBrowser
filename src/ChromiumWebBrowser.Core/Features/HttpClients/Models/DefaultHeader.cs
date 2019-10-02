using System.Collections.Generic;

namespace ChromiumWebBrowser.Core.Features.HttpClients.Models
{
    public static class DefaultHeader
    {
        public static List<KeyValuePair<string, string>> Value { get; } = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>(
                "Accept",
                "image/gif, image/jpeg, image/pjpeg, application/x-ms-application, application/xaml+xml, application/x-ms-xbap, */*"),
            new KeyValuePair<string, string>(
                "Accept-Encoding",
                "gzip, deflate"),
            new KeyValuePair<string, string>(
                "User-Agent",
                Consts.HEADER_USER_AGENT)
        };
    }
}