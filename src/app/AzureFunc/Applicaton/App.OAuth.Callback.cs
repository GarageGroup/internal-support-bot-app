using GarageGroup.Infra;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    [EndpointFunction("AuthorizeUser", IsSwaggerHidden = true)]
    [EndpointFunctionSecurity(FunctionAuthorizationLevel.Function)]
    internal static Dependency<CallbackEndpoint> UseUserAuthorizeEndpoint([DurableClient] DurableTaskClient client)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("MsLoginHttpApi")
        .UsePollyStandard()
        .UseHttpApi()
        .With(
            ResolveBotStorage)
        .With(
            client.UseBotSignal())
        .UseAuthorizationCallbackEndpoint(
            BotAuthorizationSectionName);
}