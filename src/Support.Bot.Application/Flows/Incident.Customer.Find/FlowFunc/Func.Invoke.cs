using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

partial class IncidentCustomerFindFlowFunc
{
    public ValueTask<ChatFlowStepResult<IncidentCustomerFindFlowOut>> InvokeAsync(
        DialogContext dialogContext, Unit input, CancellationToken cancellationToken = default)
    {
        _ = dialogContext ?? throw new ArgumentNullException(nameof(dialogContext));

        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<ChatFlowStepResult<IncidentCustomerFindFlowOut>>(cancellationToken);
        }

        return InnerInvokeAsync(dialogContext, cancellationToken);
    }

    private async ValueTask<ChatFlowStepResult<IncidentCustomerFindFlowOut>> InnerInvokeAsync(
        DialogContext dialogContext, CancellationToken cancellationToken)
    {
        var stubActivity = MessageFactory.Text("Поиск клиентов будет реализован позднее...");
        await dialogContext.Context.SendActivityAsync(stubActivity, cancellationToken).ConfigureAwait(false);

        return CreateStubFlowOut();
    }

    private static IncidentCustomerFindFlowOut CreateStubFlowOut()
        =>
        new(customerId: Guid.Parse("955c892f-1400-ec11-94ef-000d3a4a099f"));
}
