using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

partial class IncidentCreateChatFlow
{
    internal static ValueTask<Unit> RunAsync(
        this IBotContext context, ISupportApi supportApi, IncidentCreateFlowOption option, CancellationToken cancellationToken)
    {
        var turnContext = context.TurnContext;
        if (turnContext.IsNotMessageType())
        {
            return context.BotFlow.NextAsync(cancellationToken);
        }

        if (turnContext.IsTelegramChannel() && string.Equals(turnContext.Activity.Text, "/start", StringComparison.InvariantCultureIgnoreCase))
        {
            return context.BotFlow.NextAsync(cancellationToken);
        }

        return context.CreateChatFlow("IncidentCreate").RunFlow(supportApi, option).CompleteValueAsync(cancellationToken);
    }
}