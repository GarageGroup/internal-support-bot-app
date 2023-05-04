using System;
using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

partial class Application
{
    internal static IBotBuilder UseAuthorizationFlow(this IBotBuilder botBuilder)
        =>
        botBuilder.UseDataverseAuthorization(
            ResolveBotAuthorizationOption,
            GetAzureUserMeGetApi,
            GetDataverseUserGetApi);

    private static IAzureUserMeGetFunc GetAzureUserMeGetApi(IBotContext botContext)
        =>
        UseHttpMessageHandlerStandard("AzureUserMeGetApi").UseAzureUserMeGetApi().Resolve(botContext.ServiceProvider);

    private static IDataverseUserGetFunc GetDataverseUserGetApi(IBotContext botContext)
        =>
        UseDataverseApiClient().UseUserGetApi().Resolve(botContext.ServiceProvider);

    private static BotAuthorizationOption ResolveBotAuthorizationOption(this IBotContext context)
    {
        var configuration = context.ServiceProvider.GetConfiguration();
        var domainName = configuration["DomainName"].OrNullIfEmpty() ?? "Garage Group";

        return new(
            oAuthConnectionName: configuration["OAuthConnectionName"].OrEmpty(),
            enterText: $"""
                Войдите в свою учетную запись {domainName}:
                1. Перейдите по ссылке
                2. Авторизуйтесь под учетной записью {domainName}
                3. Скопируйте и отправьте полученный код в этот чат
                """);
    }
}