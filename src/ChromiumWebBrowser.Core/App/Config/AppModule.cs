using Autofac;
using ChromiumWebBrowser.Core.Features.HttpClients;
using ChromiumWebBrowser.Core.Features.HttpClients.Models;
using ChromiumWebBrowser.Core.Features.WebProxies.Models;

namespace ChromiumWebBrowser.Core.App.Config
{
    public class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // ProxyBrowser
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
            }).As<IProxyBrowser>();


            // WebProxyService
            // IWebProxyService
            builder.Register(c =>
            {
                var cc = c.Resolve<IComponentContext>();

                var webProxy = WebProxyProvider.FromString("None");
                var webProxyServiceBuilder = cc.ResolveNamed<IWebProxyServiceBuilder>(webProxy.Name);
                var webProxyService = webProxyServiceBuilder.CreateWebProxyService();

                return webProxyService;
            }).As<IWebProxyService>().SingleInstance();
        }
    }
}