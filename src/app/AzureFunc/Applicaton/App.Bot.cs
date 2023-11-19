using System;
using GarageGroup.Infra;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Bot.Builder;
using Microsoft.DurableTask.Client;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    [HttpFunction("HandleHttpBotMessage", HttpMethodName.Post, Route = "messages", AuthLevel = HttpAuthorizationLevel.Function)]
    internal static Dependency<IBotSignalHandler> UseBotSignal([DurableClient] DurableTaskClient client)
        =>
        Dependency.Of(
            client)
        .UseOrchestrationEntityApi()
        .UseBotSignalHandler(
            BotEntityName);

    [EntityFunction("HandleBotRequest", EntityName = BotEntityName)]
    internal static Dependency<IBotRequestHandler> UseBot()
        =>
        Dependency.From(
            ResolveBot,
            StandardCloudAdapter.Resolve)
        .UseBotRequestHandler();

    private static IBot ResolveBot(this IServiceProvider serviceProvider)
        =>
        BotBuilder.Resolve(serviceProvider)
        .UseLogoutFlow()
        .UseBotStopFlow()
        .UseAuthorizationFlow()
        .UseBotInfoFlow()
        .UseIncidentCreateFlow()
        .Build();
}