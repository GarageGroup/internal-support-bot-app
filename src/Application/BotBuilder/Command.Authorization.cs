using System;
using System.Threading;
using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;
using GGroupp.Platform;
using PrimeFuncPack;

namespace GGroupp.Internal.Support;

using IAzureUserGetFunc = IAsyncValueFunc<AzureUserMeGetIn, Result<AzureUserGetOut, Failure<AzureUserGetFailureCode>>>;
using IDataverseUserGetFunc = IAsyncValueFunc<DataverseUserGetIn, Result<DataverseUserGetOut, Failure<DataverseUserGetFailureCode>>>;

partial class GSupportBotBuilder
{
    internal static IBotBuilder UseGSupportAuthorization(this IBotBuilder botBuilder)
        =>
        botBuilder.UseDataverseAuthorization(
            AuthorizationBotBuilder.ResolveStandardOption,
            GetAzureUserGetApi,
            GetDataverseUserGetApi);

    private static readonly Lazy<Dependency<IAzureUserGetFunc>> azureUserGetApiDependency
        =
        new(CreateAzureUserGetDependency, LazyThreadSafetyMode.ExecutionAndPublication);

    private static IAzureUserGetFunc GetAzureUserGetApi(IBotContext botContext)
        =>
        azureUserGetApiDependency.Value.Resolve(botContext.ServiceProvider);

    private static IDataverseUserGetFunc GetDataverseUserGetApi(IBotContext botContext)
        =>
        CreateStandardHttpHandlerDependency("DataverseUserGetApi")
        .UseDataverseApiClient()
        .UseUserGetApi()
        .Resolve(botContext.ServiceProvider);

    private static Dependency<IAzureUserGetFunc> CreateAzureUserGetDependency()
        =>
        CreateStandardHttpHandlerDependency("AzureUserGetApi")
        .UseAzureUserMeGetApi();
}