using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

partial class ConversationFlowOnUpdateFunc
{
    public ValueTask<ChatFlowStepResult<Unit>> InvokeAsync(DialogContext dialogContext, Unit _, CancellationToken cancellationToken = default)
    {
        var context = dialogContext ?? throw new ArgumentNullException(nameof(dialogContext));
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<ChatFlowStepResult<Unit>>(cancellationToken);
        }

        return InnerInvokeAsync(context, cancellationToken);
    }

    private async ValueTask<ChatFlowStepResult<Unit>> InnerInvokeAsync(DialogContext dialogContext, CancellationToken cancellationToken = default)
    {
        foreach (var member in dialogContext.Context.Activity.MembersAdded.Where(m => m.Id != dialogContext.Context.Activity.Recipient.Id))
        {
            var reply = MessageFactory.Text("Привет! Это G-Support бот!");
            _ = await dialogContext.Context.SendActivityAsync(reply, cancellationToken).ConfigureAwait(false);
        }

        return default(Unit);
    }
}
