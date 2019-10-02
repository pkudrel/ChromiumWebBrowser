namespace ChromiumWebBrowser.Core.Features.WebProxies.Models
{
    public interface IWebProxyService
    {
        bool IsEnable { get; }
        System.Net.WebProxy GetWebProxy();
        ProxySettingsExtend GetProxySettings();
    }
}