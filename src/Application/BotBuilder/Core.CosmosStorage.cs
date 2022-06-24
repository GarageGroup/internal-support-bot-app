using System;
using System.Collections.Generic;
using System.Threading;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

partial class GSupportBotBuilder
{
    internal static ICosmosStorage ResolveCosmosStorage(IServiceProvider serviceProvider)
        =>
        lazyCosmosStorageDependency.Value.Resolve(serviceProvider);

    private static readonly Lazy<Dependency<ICosmosStorage>> lazyCosmosStorageDependency
        =
        new(CreateCosmosStorageDependency, LazyThreadSafetyMode.ExecutionAndPublication);

    private static Dependency<ICosmosStorage> CreateCosmosStorageDependency()
        =>
        CreateStandardHttpHandlerDependency("CosmosStorage")
        .UseCosmosApi(
            sp => sp.GetRequiredService<IConfiguration>().GetCosmosApiOption())
        .UseCosmosStorage(
            sp => sp.GetRequiredService<IConfiguration>().GetCosmosStorageOption());

    private static CosmosApiOption GetCosmosApiOption(this IConfiguration configuration)
        =>
        new(
            baseAddress: new(configuration.GetValue<string>("CosmosDbBaseAddressUrl")),
            masterKey: configuration.GetValue<string>("CosmosDbMasterKey"),
            databaseId: configuration.GetValue<string>("CosmosDbDatabaseId"));

    private static CosmosStorageOption GetCosmosStorageOption(this IConfiguration configuration)
        =>
        new(
            containerTtlSeconds: new Dictionary<StorageItemType, int?>
            {
                [StorageItemType.UserState] = configuration.GetTtlSeconds("CosmosDbUserStateContainerTtlHours"),
                [StorageItemType.ConversationState] = configuration.GetTtlSeconds("CosmosDbConversationStateContainerTtlHours"),
                [StorageItemType.Default] = configuration.GetTtlSeconds("CosmosDbBotStorageContainerTtlHours")
            });

    private static int? GetTtlSeconds(this IConfiguration configuration, string ttlHoursKey)
    {
        var ttlHours = configuration.GetValue<int?>(ttlHoursKey);
        return ttlHours is not null ? ttlHours.Value * 3600 : null;
    }
}