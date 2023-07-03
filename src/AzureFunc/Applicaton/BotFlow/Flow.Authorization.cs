using System;
using GarageGroup.Infra;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

partial class Application
{
    internal static IBotBuilder UseAuthorizationFlow(this IBotBuilder botBuilder)
        =>
        botBuilder.UseDataverseAuthorization(
            ResolveBotAuthorizationOption,
            GetAzureUserApi,
            GetDataverseUserApi);

    private static IAzureUserApi GetAzureUserApi(IBotContext botContext)
        =>
        UseHttpMessageHandlerStandard("AzureUserApi").UseAzureUserApi().Resolve(botContext.ServiceProvider);

    private static IDataverseUserApi GetDataverseUserApi(IBotContext botContext)
        =>
        UseDataverseApiClient().UseUserApi().Resolve(botContext.ServiceProvider);

    private static BotAuthorizationOption ResolveBotAuthorizationOption(this IBotContext context)
        =>
        new(
            oAuthConnectionName: context.ServiceProvider.GetConfiguration()["OAuthConnectionName"].OrEmpty(),
            enterText: $"""
                Войдите в свою учетную запись Garage Group:
                1. Перейдите по ссылке
                2. Авторизуйтесь под учетной записью Garage Group
                3. Скопируйте и отправьте полученный код в этот чат
                """);
}