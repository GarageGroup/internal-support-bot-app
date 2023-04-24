using System;
using GGroupp.Infra.Bot.Builder;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

partial class Application
{
    internal static ICosmosStorage ResolveCosmosStorage(IServiceProvider serviceProvider)
        =>
        UseCosmosStorage().Resolve(serviceProvider);

    private static Dependency<ICosmosStorage> UseCosmosStorage()
        =>
        UseHttpMessageHandlerStandard("CosmosStorage")
        .UseCosmosApi("CosmosDb")
        .UseCosmosStorageStandard("CosmosDb");
}