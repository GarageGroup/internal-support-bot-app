using GarageGroup.Infra;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    private static Dependency<IUserAuthorizationApi> UseUserAuthorizationApi()
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging(
            "AzureAuthorizationApi")
        .UsePollyStandard()
        .UseHttpApi()
        .With(
            ServiceProviderServiceExtensions.GetRequiredService<IDataverseApiClient>)
        .UseUserAuthorizationApi();
}