using System;
using GarageGroup.Infra;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

internal static partial class Application
{
    private const string BotEntityName = "BotRequest";

    private static Dependency<IDataverseApiClient> UseDataverseApi()
        =>
        Dependency.From(
            ServiceProviderServiceExtensions.GetRequiredService<IDataverseApiClient>);

    private static Dependency<ISqlApi> UseSqlApi()
        =>
        Dependency.From(
            ServiceProviderServiceExtensions.GetRequiredService<ISqlApi>);

    private static Dependency<ICosmosStorage> UseCosmosStorage()
        =>
        Dependency.From(
            ServiceProviderServiceExtensions.GetRequiredService<ICosmosStorage>);

    private static IConfiguration GetConfiguration(this IServiceProvider serviceProvider)
        =>
        serviceProvider.GetRequiredService<IConfiguration>();
}