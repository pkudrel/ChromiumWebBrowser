using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using ChromiumWebBrowser.Core.App.Bootstrap;
using MediatR;
using MediatR.Pipeline;
using Module = Autofac.Module;

namespace ChromiumWebBrowser.Core.App.Config
{
    /// <summary>
    /// Mediator module
    /// </summary>
    /// <remarks>
    /// Links:
    /// https://lostechies.com/jimmybogard/2014/09/09/tackling-cross-cutting-concerns-with-a-mediator-pipeline/
    /// https://gist.github.com/NotMyself/07ea9ac7884d8d29e147
    /// https://gist.github.com/NotMyself/579f94e1aad6a022ddb9
    /// </remarks>
    public class MediatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var bly = Boot.Instance.GetAssemblies();

            //  builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(bly)
                .Where(t =>
                    t.GetInterfaces().Any(i => i.IsClosedTypeOf(typeof(IRequestHandler<>))
                                               || i.IsClosedTypeOf(typeof(IRequestHandler<,>))
                                               || i.IsClosedTypeOf(typeof(INotificationHandler<>))

                    )
                )
                .AsImplementedInterfaces();
            ;

            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).AsImplementedInterfaces();

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

        }
    }
}