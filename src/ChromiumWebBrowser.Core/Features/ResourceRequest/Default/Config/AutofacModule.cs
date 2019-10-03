using System.Collections.Generic;
using Autofac;
using CefSharp;
using ChromiumWebBrowser.Core.Features.ResourceRequest.Default.Services;
using Newtonsoft.Json;

namespace ChromiumWebBrowser.Core.Features.ResourceRequest.Default
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultResourceRequestHandlerFactory>()
                .AsSelf()
                .As<IResourceRequestHandlerFactory>();

            builder
                .RegisterType<DefaultResourceRequestHandler>()
                .AsSelf();

            builder
                .RegisterType<DefaultResourceHandler>()
                .AsSelf();



            var l = new List<ResourceType>();
            l.Add(ResourceType.MainFrame);
            l.Add(ResourceType.SubFrame);
            var json2 = JsonConvert.SerializeObject(l);


        }
    }
}