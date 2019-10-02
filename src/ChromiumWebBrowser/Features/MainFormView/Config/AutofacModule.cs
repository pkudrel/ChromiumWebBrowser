using System;
using Autofac;
using ChromiumWebBrowser.Features.Chromium.Controls;
using ChromiumWebBrowser.Features.MainFormView.Views;
using ChromiumWebBrowser.Misc.Helpers;

namespace ChromiumWebBrowser.Features.MainFormView.Config
{
    public class AutofacModule : Module
    {
        //SingletonFormProvider


        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(c =>
                {
                    var cc = c.Resolve<IComponentContext>();
                    var browserFn = cc.Resolve<Func<string, AdvanceChromiumWebBrowser>>();
                    return new SingletonFormProvider<MainForm>(() =>
                            new MainForm(browserFn));
                })
                .AsSelf()
                .SingleInstance();


            builder
                .Register(c => c.Resolve<SingletonFormProvider<MainForm>>().CurrentInstance)
                .AsSelf();
        }
    }
}