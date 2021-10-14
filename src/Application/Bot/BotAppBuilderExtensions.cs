using System;
using System.Threading;
using GGroupp.Infra;
using Microsoft.AspNetCore.Builder;
using Microsoft.Bot.Builder;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

internal static class BotAppBuilderExtensions
{
    private static readonly Lazy<IStorage> lazyStorage;

    private static readonly Lazy<ConversationState> lazyConversationState;

    private static readonly Lazy<UserState> lazyUserState;

    private static readonly Dependency<IBot> botDependency;

    static BotAppBuilderExtensions()
    {
        lazyStorage = new(CreateStorage, LazyThreadSafetyMode.ExecutionAndPublication);
        lazyConversationState = new(CreateConversationState, LazyThreadSafetyMode.ExecutionAndPublication);
        lazyUserState = new(CreateUserState, LazyThreadSafetyMode.ExecutionAndPublication);
        botDependency = CreateBotDependency();
    }

    public static IApplicationBuilder UseGSupportBot(this IApplicationBuilder app)
        =>
        app.UseBot(botDependency.Resolve);

    private static Dependency<IBot> CreateBotDependency()
        =>
        Dependency.From(
            () => lazyConversationState.Value,
            () => lazyUserState.Value)
        .UseGSupportBot();

    private static ConversationState CreateConversationState()
        =>
        new(lazyStorage.Value);

    private static UserState CreateUserState()
        =>
        new(lazyStorage.Value);

    private static IStorage CreateStorage()
        =>
        new MemoryStorage();
}
