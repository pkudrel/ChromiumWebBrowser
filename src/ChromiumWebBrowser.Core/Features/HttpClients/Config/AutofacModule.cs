using Autofac;
using ChromiumWebBrowser.Core.Features.WebProxies.Models;

namespace ChromiumWebBrowser.Core.Features.HttpClients.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // WebProxyService
            // IWebProxyService
            builder.Register(c =>
                {
                    var cc = c.Resolve<IComponentContext>();

                    var webProxy = WebProxyProvider.FromString("None");
                    var webProxyServiceBuilder = cc.ResolveNamed<IWebProxyServiceBuilder>(webProxy.Name);
                    var webProxyService = webProxyServiceBuilder.CreateWebProxyService();

                    return webProxyService;
                }).As<IWebProxyService>()
                .SingleInstance();
        }
    }
}