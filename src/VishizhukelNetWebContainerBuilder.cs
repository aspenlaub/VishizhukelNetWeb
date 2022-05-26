using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb;

public static class VishizhukelNetWebContainerBuilder {
    public static async Task<IServiceCollection> UseVishizhukelNetWebAndPeghAsync(this IServiceCollection services, ICsArgumentPrompter csArgumentPrompter) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(services, csArgumentPrompter, false);
    }

    public static async Task<IServiceCollection> UseVishizhukelNetWebDvinAndPeghAsync(this IServiceCollection services, ICsArgumentPrompter csArgumentPrompter) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(services, csArgumentPrompter, true);
    }

    private static async Task<IServiceCollection> UseVishizhukelNetWebAndPeghOptionallyDvinAsync(IServiceCollection services, ICsArgumentPrompter csArgumentPrompter, bool useDvin) {
        if (useDvin) {
            await services.UseVishizhukelNetDvinAndPeghAsync(csArgumentPrompter);
        } else {
            await services.UseVishizhukelNetAndPeghAsync(csArgumentPrompter);
        }
        return services;
    }

    public static async Task<ContainerBuilder> UseVishizhukelNetWebAndPeghAsync(this ContainerBuilder builder, ICsArgumentPrompter csArgumentPrompter, ILogConfiguration logConfiguration) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(builder, csArgumentPrompter, false, logConfiguration);
    }

    public static async Task<ContainerBuilder> UseVishizhukelNetWebDvinAndPeghAsync(this ContainerBuilder builder, ICsArgumentPrompter csArgumentPrompter, ILogConfiguration logConfiguration) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(builder, csArgumentPrompter, true, logConfiguration);
    }

    private static async Task<ContainerBuilder> UseVishizhukelNetWebAndPeghOptionallyDvinAsync(ContainerBuilder builder, ICsArgumentPrompter csArgumentPrompter, bool useDvin, ILogConfiguration logConfiguration) {
        if (useDvin) {
            await builder.UseVishizhukelNetDvinAndPeghAsync(csArgumentPrompter, logConfiguration);
        } else {
            await builder.UseVishizhukelNetAndPeghAsync(csArgumentPrompter, logConfiguration);
        }

        return builder;
    }
}