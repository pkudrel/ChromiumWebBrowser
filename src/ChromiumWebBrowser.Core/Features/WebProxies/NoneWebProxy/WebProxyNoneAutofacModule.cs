using Autofac;
using ChromiumWebBrowser.Core.Features.WebProxies.Models;
using NLog;

namespace ChromiumWebBrowser.Core.Features.WebProxies.NoneWebProxy
{
    public class WebProxyNoneAutofacModule : Module
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly WebProxyProvider _provider = WebProxyProvider.None;

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<IWebProxyServiceBuilder>(
                    c => new NoneWebProxyServiceBuilder())
                .Named<IWebProxyServiceBuilder>(_provider.Name);
        }
    }
}