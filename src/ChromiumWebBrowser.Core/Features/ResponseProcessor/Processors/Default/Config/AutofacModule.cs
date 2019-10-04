using Autofac;
using ChromiumWebBrowser.Core.Features.ResourceRequest.Models;
using ChromiumWebBrowser.Core.Features.ResponseProcessor.Processors.Default.Services;
using ChromiumWebBrowser.Core.Features.ResponseProcessor.Services;

namespace ChromiumWebBrowser.Core.Features.ResponseProcessor.Processors.Default.Config
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            var currentProcessor = Processor.Default;

            builder.RegisterType<MakeBrakesOnTagEnd>()
                .As<IResponseAction>()
                .Named<IResponseAction>(currentProcessor.Name);

            builder.RegisterType<ResponseProcessorManager>()
                .As<IResponseProcessorManager>()
               ;

        }
    }
}