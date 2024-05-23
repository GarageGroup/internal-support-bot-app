using GarageGroup.Infra;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    [HttpFunction("HandleBotHttp", HttpMethodName.Post, Route = "message", AuthLevel = HttpAuthorizationLevel.Function)]
    internal static Dependency<IBotSignalHandler> UseBotSignal([DurableClient] this DurableTaskClient client)
        =>
        Dependency.Of(
            client)
        .UseOrchestrationEntityApi()
        .UseBotSignalHandler(
            BotEntityName);

    [EntityFunction("HandleBotEntity", EntityName = BotEntityName)]
    internal static Dependency<IBotWebHookHandler> UseBot()
        =>
        Dependency.From(
            ServiceProviderServiceExtensions.GetRequiredService<BotProvider>)
        .GetBotBuilder()
        .UseAuthorization()
        .UseCommands()
        .WithBotInfoCommand()
        .WithBotStopCommand()
        .WithLogoutCommand()
        .WithContactGetCommand()
        .WithIncidentCreateCommand()
        .WithBotMenuCommand()
        .BuildWebHookHandler();
}