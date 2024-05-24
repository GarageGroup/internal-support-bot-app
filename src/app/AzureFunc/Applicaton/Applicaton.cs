using System;
using GarageGroup.Infra;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

internal static partial class Application
{
    private const string BotEntityName = "TelegramBotRequest";

    private const string BotAuthorizationSectionName = "Bot:Authorization";

    private static IBotApi ResolveBotApi(IServiceProvider serviceProvider)
        =>
        serviceProvider.GetRequiredService<BotProvider>().BotApi;

    private static IBotStorage ResolveBotStorage(IServiceProvider serviceProvider)
        =>
        serviceProvider.GetRequiredService<BotProvider>().BotStorage;

    private static Dependency<IDataverseApiClient> UseDataverseApi()
        =>
        Dependency.From(
            ServiceProviderServiceExtensions.GetRequiredService<IDataverseApiClient>);

    private static Dependency<ISqlApi> UseSqlApi()
        =>
        Dependency.From(
            ServiceProviderServiceExtensions.GetRequiredService<ISqlApi>);

    private static IConfiguration GetConfiguration(this IServiceProvider serviceProvider)
        =>
        serviceProvider.GetRequiredService<IConfiguration>();
}