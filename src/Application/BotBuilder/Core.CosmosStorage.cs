using System;
using System.Collections.Generic;
using System.Threading;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Configuration;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

partial class GSupportBotBuilder
{
    private const string CosmosStorageSectionName = "CosmosDb";

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
            sp => sp.GetRequiredSection(CosmosStorageSectionName).GetCosmosApiOption())
        .UseCosmosStorage(
            sp => sp.GetRequiredSection(CosmosStorageSectionName).GetCosmosStorageOption());

    private static CosmosApiOption GetCosmosApiOption(this IConfigurationSection section)
        =>
        new(
            baseAddress: new(section["BaseAddressUrl"]),
            masterKey: section["MasterKey"],
            databaseId: section["DatabaseId"]);

    private static CosmosStorageOption GetCosmosStorageOption(this IConfigurationSection section)
        =>
        new(
            containerTtlSeconds: new Dictionary<StorageItemType, int?>
            {
                [StorageItemType.UserState] = section.GetTtlSeconds("UserStateContainerTtlHours"),
                [StorageItemType.ConversationState] = section.GetTtlSeconds("ConversationStateContainerTtlHours"),
                [StorageItemType.Default] = section.GetTtlSeconds("BotStorageContainerTtlHours")
            });

    private static int? GetTtlSeconds(this IConfiguration configuration, string ttlHoursKey)
    {
        var ttlHours = configuration.GetValue<int?>(ttlHoursKey);
        return ttlHours is not null ? ttlHours.Value * 3600 : null;
    }
}