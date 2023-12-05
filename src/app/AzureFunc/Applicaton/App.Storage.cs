using System;
using GarageGroup.Infra;
using GarageGroup.Infra.Bot.Builder;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    internal static ICosmosStorage ResolveCosmosStorage(IServiceProvider serviceProvider)
        =>
        UseCosmosStorage().Resolve(serviceProvider);

    private static Dependency<ICosmosStorage> UseCosmosStorage()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("CosmosStorage")
        .UseTokenCredentialResource()
        .UsePollyStandard()
        .UseCosmosStorage("CosmosDb");
}