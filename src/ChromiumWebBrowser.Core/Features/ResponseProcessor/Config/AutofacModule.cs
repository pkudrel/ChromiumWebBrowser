using System;
using System.Collections.Generic;
using Autofac;
using ChromiumWebBrowser.Core.Features.HttpClients;
using ChromiumWebBrowser.Core.Features.HttpClients.Models;
using ChromiumWebBrowser.Core.Features.ResourceRequest.Models;
using ChromiumWebBrowser.Core.Features.ResponseProcessor.Processors.Default.Services;
using ChromiumWebBrowser.Core.Features.WebProxies.Models;

namespace ChromiumWebBrowser.Core.Features.ResponseProcessor.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UrlRuleService>().As<IUrlRuleService>().SingleInstance();

           

            builder.Register<Func<string, List<IResponseAction>>>(c =>
                {
                    var cc = c.Resolve<IComponentContext>();
                    var list = cc.ResolveNamed<IEnumerable<IResponseAction>>(Processor.Default.Name);
                    return nam =>
                    {
                        var res = new List<IResponseAction>();
                        return res;
                    };
                })
              
              ;
        }
    }
}