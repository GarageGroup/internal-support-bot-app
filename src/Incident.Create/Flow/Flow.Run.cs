using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

partial class IncidentCreateChatFlow
{
    internal static ValueTask<Unit> RunAsync(
        this IBotContext context,
        ISupportApi supportApi,
        ISupportGptApi supportGptApi,
        IncidentCreateFlowOption option,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(supportGptApi);
        ArgumentNullException.ThrowIfNull(option);

        var turnContext = context.TurnContext;
        if (turnContext.IsNotMessageType())
        {
            return context.BotFlow.NextAsync(cancellationToken);
        }

        if (turnContext.IsTelegramChannel() && string.Equals(turnContext.Activity.Text, "/start", StringComparison.InvariantCultureIgnoreCase))
        {
            return context.BotFlow.NextAsync(cancellationToken);
        }

        return context.CreateChatFlow("IncidentCreate").RunFlow(supportApi, supportGptApi, option).CompleteValueAsync(cancellationToken);
    }
}