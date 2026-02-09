using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Components;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Interfaces;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb;

public static class VishizhukelNetWebContainerBuilder {
    public static async Task<IServiceCollection> UseVishizhukelNetWebAndPeghAsync(this IServiceCollection services, string applicationName) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(services, applicationName, false);
    }

    public static async Task<IServiceCollection> UseVishizhukelNetWebDvinAndPeghAsync(this IServiceCollection services, string applicationName) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(services, applicationName, true);
    }

    private static async Task<IServiceCollection> UseVishizhukelNetWebAndPeghOptionallyDvinAsync(IServiceCollection services, string applicationName, bool useDvin) {
        if (useDvin) {
            await services.UseVishizhukelNetDvinAndPeghAsync(applicationName);
        } else {
            await services.UseVishizhukelNetAndPeghAsync(applicationName);
        }

        services.AddTransient<ILogicalUrlRepository, LogicalUrlRepository>();
        services.AddTransient<IOucidLogAccessor, OucidLogAccessor>();
        return services;
    }

    public static async Task<ContainerBuilder> UseVishizhukelNetWebAndPeghAsync(this ContainerBuilder builder, string applicationName) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(builder, applicationName, false);
    }

    public static async Task<ContainerBuilder> UseVishizhukelNetWebDvinAndPeghAsync(this ContainerBuilder builder, string applicationName) {
        return await UseVishizhukelNetWebAndPeghOptionallyDvinAsync(builder, applicationName, true);
    }

    private static async Task<ContainerBuilder> UseVishizhukelNetWebAndPeghOptionallyDvinAsync(ContainerBuilder builder, string applicationName, bool useDvin) {
        if (useDvin) {
            await builder.UseVishizhukelNetDvinAndPeghAsync(applicationName);
        } else {
            await builder.UseVishizhukelNetAndPeghAsync(applicationName);
        }

        builder.RegisterType<LogicalUrlRepository>().As<ILogicalUrlRepository>();
        builder.RegisterType<OucidLogAccessor>().As<IOucidLogAccessor>();
        return builder;
    }
}