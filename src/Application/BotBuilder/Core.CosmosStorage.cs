using System;
using System.Collections.Generic;
using System.Threading;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

partial class GSupportBotBuilder
{
    internal static IStorage ResolveCosmosStorage(IServiceProvider serviceProvider)
        =>
        lazyCosmosStorageDependency.Value.Resolve(serviceProvider);

    private static readonly Lazy<Dependency<IStorage>> lazyCosmosStorageDependency
        =
        new(CreateCosmosStorageDependency, LazyThreadSafetyMode.ExecutionAndPublication);

    private static Dependency<IStorage> CreateCosmosStorageDependency()
        =>
        CreateStandardHttpHandlerDependency("CosmosStorage")
        .UseCosmosStorage(
            sp => sp.GetRequiredService<IConfiguration>().GetCosmosStorageConfiguration());

    private static CosmosStorageConfiguration GetCosmosStorageConfiguration(this IConfiguration configuration)
        =>
        new(
            baseAddress: new(configuration.GetValue<string>("CosmosDbBaseAddressUrl")),
            masterKey: configuration.GetValue<string>("CosmosDbMasterKey"),
            databaseId: configuration.GetValue<string>("CosmosDbDatabaseId"),
            containerTtlSeconds: new Dictionary<CosmosStorageContainerType, int?>
            {
                [CosmosStorageContainerType.UserState] = configuration.GetTtlSeconds("CosmosDbUserStateContainerTtlHours"),
                [CosmosStorageContainerType.ConversationState] = configuration.GetTtlSeconds("CosmosDbConversationStateContainerTtlHours"),
                [CosmosStorageContainerType.BotStorage] = configuration.GetTtlSeconds("CosmosDbBotStorageContainerTtlHours")
            });

    private static int? GetTtlSeconds(this IConfiguration configuration, string ttlHoursKey)
    {
        var ttlHours = configuration.GetValue<int?>(ttlHoursKey);
        return ttlHours is not null ? ttlHours.Value * 3600 : null;
    }
}