using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Components;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Interfaces;
using Autofac;
using FakeGuiAndApplicationSynchronizer = Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Helpers.FakeGuiAndApplicationSynchronizer;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application;

public static class ApplicationContainerBuilder {
    public static async Task<ContainerBuilder> UseApplicationAsync(this ContainerBuilder builder,VishizhukelNetWebView2Window vishizhukelNetWebView2Window) {
        await builder.UseVishizhukelNetDvinAndPeghAsync("VishizhukelNetWeb", new DummyCsArgumentPrompter());
        if (vishizhukelNetWebView2Window == null) {
            builder.RegisterType<FakeGuiAndApplicationSynchronizer>().As(typeof(IGuiAndWebViewApplicationSynchronizer<ApplicationModel>)).SingleInstance();
        } else {
            builder.RegisterInstance(vishizhukelNetWebView2Window);
            builder.RegisterType<GuiAndApplicationSynchronizer>().As(typeof(IGuiAndWebViewApplicationSynchronizer<ApplicationModel>)).SingleInstance();
        }

        builder.RegisterType<Application.Application>().As<Application.Application>().SingleInstance();
        builder.RegisterType<ApplicationModel>().As<ApplicationModel>().As<IApplicationModel>().As<IBusy>().SingleInstance();
        builder.RegisterType<GuiToApplicationGate>().As<IGuiToWebViewApplicationGate>().SingleInstance();
        builder.RegisterType<LogicalUrlRepository>().As<ILogicalUrlRepository>().SingleInstance();
        return builder;
    }
}