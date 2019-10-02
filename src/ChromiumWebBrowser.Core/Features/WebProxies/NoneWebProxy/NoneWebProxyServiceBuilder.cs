using ChromiumWebBrowser.Core.Features.WebProxies.Models;

namespace ChromiumWebBrowser.Core.Features.WebProxies.NoneWebProxy
{
    public class NoneWebProxyServiceBuilder : IWebProxyServiceBuilder
    {
        public IWebProxyService CreateWebProxyService()
        {
            return new NoneWebProxyService();
        }
    }
}