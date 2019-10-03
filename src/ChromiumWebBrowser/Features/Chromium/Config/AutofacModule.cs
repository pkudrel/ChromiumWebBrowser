using System;
using System.Windows.Forms;
using Autofac;
using ChromiumWebBrowser.Core.Features.Projects.Services;
using ChromiumWebBrowser.Core.Features.ResourceRequest.Default.Services;
using ChromiumWebBrowser.Features.Chromium.Controls;
using ChromiumWebBrowser.Features.Chromium.Handlers;
using ChromiumWebBrowser.Features.Chromium.Others;

namespace ChromiumWebBrowser.Features.Chromium.Config
{
    public class AutofacModule : Module
    {
        //SingletonFormProvider
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<Func<string, AdvanceChromiumWebBrowser>>(
                c =>
                {
                    var cc = c.Resolve<IComponentContext>();
                    var pm = cc.Resolve<IProjectManager>();
                    var drrhf = cc.Resolve<DefaultResourceRequestHandlerFactory>();

                    return address =>
                    {
                        var browser = new AdvanceChromiumWebBrowser(address)
                        {
                            Dock = DockStyle.Fill
                        };

                        browser.RequestHandler = new ExampleRequestHandler();
                        browser.LifeSpanHandler = new LifeSpanHandler();
                        browser.ResourceRequestHandlerFactory = drrhf;
                        return browser;
                    };
                });


            builder.Register(
                c =>
                {
                    var cc = c.Resolve<IComponentContext>();
                    var func = cc.Resolve<Func<string, AdvanceChromiumWebBrowser>>();
                    var browser = func("http://test.hornetpk.lan/");
                    return browser;
                }).AsSelf();
        }
    }
}