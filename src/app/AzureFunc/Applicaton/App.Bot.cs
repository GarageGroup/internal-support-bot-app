using System;
using GarageGroup.Infra;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support;

partial class Application
{
    [HttpFunction("HandleHttpBotMessage", HttpMethodName.Post, Route = "messages", AuthLevel = HttpAuthorizationLevel.Function)]
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
        .Build(true);
}