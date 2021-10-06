using System;
using System.Threading;
using GGroupp.Infra;
using Microsoft.AspNetCore.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot
{
    internal static class BotAppBuilderExtensions
    {
        private static Lazy<IStorage> lazyStorage;

        private static Lazy<ConversationState> lazyConversationState;

        private static Lazy<UserState> lazyUserState;

        static BotAppBuilderExtensions()
        {
            lazyStorage = new(CreateStorage, LazyThreadSafetyMode.ExecutionAndPublication);
            lazyConversationState = new(CreateConversationState, LazyThreadSafetyMode.ExecutionAndPublication);
            lazyUserState = new(CreateUserState, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public static IApplicationBuilder UseGSupportBot(this IApplicationBuilder app)
            =>
            app.UseBot(Resolve);

        private static IBot Resolve(IServiceProvider serviceProvider)
            =>
            DialogBot.Create(
                conversationState: lazyConversationState.Value,
                userState: lazyUserState.Value,
                dialog: UseDialog().Resolve(serviceProvider));

        private static Dependency<Dialog> UseDialog()
            =>
            throw new NotImplementedException();
            /*PrimaryHandler.UseStandardSocketsHttpHandler()
            .With(
                _ => lazyUserState.Value)
            .Fold(
                (h, s) => new UserDataFlowGetFunc(h, s))
            .With(
                _ => new IncidentTitleFlowCreateFunc())
            .Fold(
                (userData, title) => new IncidentFlowCreateFunc(userData, title))
            .Map<Dialog>(
                func => func.ToChatFlowDialog("IncidentFlowCreate"));*/

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
}