using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Components;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Integration.Test;

public static class IntegrationTestContainerBuilder {
    public static ContainerBuilder RegisterForIntegrationTest(this ContainerBuilder builder) {
        builder.UseDvinAndPegh("VishizhukelNetWeb");
        builder.RegisterType<CanvasAndImageAndImageSizeAdjuster>().As<ICanvasAndImageSizeAdjuster>().SingleInstance();
        builder.RegisterType<StarterAndStopper>().As<IStarterAndStopper>();
        builder.RegisterType<WindowUnderTest>();
        builder.RegisterType<TashAccessor>().As<ITashAccessor>();
        return builder;
    }
}