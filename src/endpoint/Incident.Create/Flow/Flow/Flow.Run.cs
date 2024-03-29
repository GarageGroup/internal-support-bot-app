using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateChatFlow
{
    internal static ValueTask<Unit> RunAsync(
        this IBotContext context,
        ICrmCustomerApi crmCustomerApi,
        ICrmContactApi crmContactApi,
        ICrmOwnerApi crmOwnerApi,
        ICrmIncidentApi crmIncidentApi,
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

        return context.CreateChatFlow("IncidentCreate")
        .RunFlow(
            crmCustomerApi, crmContactApi, crmOwnerApi, crmIncidentApi, supportGptApi, option)
        .CompleteValueAsync(cancellationToken);
    }
}