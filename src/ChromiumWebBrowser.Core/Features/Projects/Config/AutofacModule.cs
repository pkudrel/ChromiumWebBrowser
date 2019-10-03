using Autofac;
using ChromiumWebBrowser.Core.Features.HttpClients;
using ChromiumWebBrowser.Core.Features.HttpClients.Models;
using ChromiumWebBrowser.Core.Features.Projects.Services;
using ChromiumWebBrowser.Core.Features.WebProxies.Models;

namespace ChromiumWebBrowser.Core.Features.Projects.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjectManager>().As<IProjectManager>().SingleInstance();
        }
    }
}