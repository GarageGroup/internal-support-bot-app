using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

partial class Application
{
    internal static IBotBuilder UseAuthorizationFlow(this IBotBuilder botBuilder)
        =>
        botBuilder.UseDataverseAuthorization(
            AuthorizationBotBuilder.ResolveStandardOption,
            GetAzureUserMeGetApi,
            GetDataverseUserGetApi);

    private static IAzureUserMeGetFunc GetAzureUserMeGetApi(IBotContext botContext)
        =>
        UseHttpMessageHandlerStandard("AzureUserMeGetApi").UseAzureUserMeGetApi().Resolve(botContext.ServiceProvider);

    private static IDataverseUserGetFunc GetDataverseUserGetApi(IBotContext botContext)
        =>
        UseDataverseApiClient().UseUserGetApi().Resolve(botContext.ServiceProvider);
}