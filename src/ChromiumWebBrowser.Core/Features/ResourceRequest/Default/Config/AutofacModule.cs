using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using CefSharp;
using ChromiumWebBrowser.Core.Features.HttpClients;
using ChromiumWebBrowser.Core.Features.ResourceRequest.Default.Services;
using ChromiumWebBrowser.Core.Features.ResourceRequest.Models;
using ChromiumWebBrowser.Core.Features.ResponseProcessor.Processors.Default.Services;
using ChromiumWebBrowser.Core.Features.ResponseProcessor.Services;
using ChromiumWebBrowser.Features.Projects.Models;
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


            builder.Register<Func<ProjectRegistry, List<string>, DefaultResourceRequestHandler>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                var browser = cc.Resolve<IProxyBrowser>();
                var processorManager = cc.Resolve<IResponseProcessorManager>();
                var list = cc.ResolveNamed<IEnumerable<IResponseAction>>(Processor.Default.Name);


                return (projectRegistry, actions) =>
                {
                    var responseActions = list.Where(x => actions.Contains(x.Name)).ToList();  
                    var res = new DefaultResourceRequestHandler(new DefaultResourceHandler(browser, processorManager, projectRegistry, responseActions));
                    return res;
                };
            });




        }
    }
}