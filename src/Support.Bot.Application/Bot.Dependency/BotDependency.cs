using GGroupp.Infra;
using Microsoft.Bot.Builder;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.Bot;

internal static class BotDependency
{
    public static Dependency<IBot> UseGSupportBot(this Dependency<ConversationState, UserState> dependency)
        =>
        dependency.With(
            dependency.GetSecond().UseIncidentCreateDialog())
        .With(
            dependency.GetSecond().UseConversationOnUpdateDialog())
        .Fold<IBot>(
            DialogBot.Create);
}
