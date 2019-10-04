using Autofac;
using ChromiumWebBrowser.Core.Features.HttpClients.Models;
using ChromiumWebBrowser.Core.Features.WebProxies.Models;

namespace ChromiumWebBrowser.Core.Features.HttpClients.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(c =>
                {
                    var cc = c.Resolve<IComponentContext>();
                    var webProxyService = cc.Resolve<IWebProxyService>();

                    var handler = new DefaultHttpClientHandler
                    {
                        AllowAutoRedirect = true
                    };

                    var settings = new AdvanceBrowserSettings
                    {
                        Headers = DefaultHeader.Value
                    };

                    var httpClient = new DenebLabHttpClient(
                        handler,
                        new DefaultOptions());

                    var browser = new ProxyBrowser(
                        httpClient,
                        webProxyService,
                        settings);
                    return browser;
                })
                .As<IProxyBrowser>()
                .SingleInstance();
        }
    }
}