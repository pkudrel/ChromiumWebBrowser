using System.Net;
using ChromiumWebBrowser.Core.Features.WebProxies.Models;

namespace ChromiumWebBrowser.Core.Features.WebProxies.NoneWebProxy
{
    public class NoneWebProxyService : IWebProxyService
    {
        public WebProxy GetWebProxy()
        {
            return null;
        }

        public ProxySettingsExtend GetProxySettings()
        {
            return null;
        }

        public bool IsEnable => false;
    }
}