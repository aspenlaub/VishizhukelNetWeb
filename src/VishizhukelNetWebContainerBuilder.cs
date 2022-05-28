using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb;

public static class VishizhukelNetWebContainerBuilder {
    public static async Task<IServiceCollection> UseVishizhukelNetWebAndPeghAsync(this IServiceCollection services, string applicationName, ICsArgumentPrompter csArgumentPrompter) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(services, applicationName, csArgumentPrompter, false);
    }

    public static async Task<IServiceCollection> UseVishizhukelNetWebDvinAndPeghAsync(this IServiceCollection services, string applicationName, ICsArgumentPrompter csArgumentPrompter) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(services, applicationName, csArgumentPrompter, true);
    }

    private static async Task<IServiceCollection> UseVishizhukelNetWebAndPeghOptionallyDvinAsync(IServiceCollection services, string applicationName, ICsArgumentPrompter csArgumentPrompter, bool useDvin) {
        if (useDvin) {
            await services.UseVishizhukelNetDvinAndPeghAsync(applicationName, csArgumentPrompter);
        } else {
            await services.UseVishizhukelNetAndPeghAsync(applicationName, csArgumentPrompter);
        }

        services.AddTransient<IOucidLogAccessor, OucidLogAccessor>();
        return services;
    }

    public static async Task<ContainerBuilder> UseVishizhukelNetWebAndPeghAsync(this ContainerBuilder builder, string applicationName, ICsArgumentPrompter csArgumentPrompter) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(builder, applicationName, csArgumentPrompter, false);
    }

    public static async Task<ContainerBuilder> UseVishizhukelNetWebDvinAndPeghAsync(this ContainerBuilder builder, string applicationName, ICsArgumentPrompter csArgumentPrompter) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(builder, applicationName, csArgumentPrompter, true);
    }

    private static async Task<ContainerBuilder> UseVishizhukelNetWebAndPeghOptionallyDvinAsync(ContainerBuilder builder, string applicationName, ICsArgumentPrompter csArgumentPrompter, bool useDvin) {
        if (useDvin) {
            await builder.UseVishizhukelNetDvinAndPeghAsync(applicationName, csArgumentPrompter);
        } else {
            await builder.UseVishizhukelNetAndPeghAsync(applicationName, csArgumentPrompter);
        }

        builder.RegisterType<OucidLogAccessor>().As<IOucidLogAccessor>();
        return builder;
    }
}