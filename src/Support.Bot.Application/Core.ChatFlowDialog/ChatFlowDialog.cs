using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace GGroupp.Infra;

public sealed class ChatFlowDialog<TFlowOut> : Dialog
{
    private readonly ChatFlowStep<Unit, TFlowOut> flowStep;

    internal ChatFlowDialog(string dialogId, ChatFlowStep<Unit, TFlowOut> flowStep) : base(dialogId)
        =>
        this.flowStep = flowStep;

    public override Task<DialogTurnResult> BeginDialogAsync(
        DialogContext dialogContext, object options, CancellationToken cancellationToken = default)
    {
        _ = dialogContext ?? throw new ArgumentNullException(nameof(dialogContext));

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<DialogTurnResult>(cancellationToken);
        }

        return ExecuteDialogAsync(dialogContext, cancellationToken);
    }

    public override Task<DialogTurnResult> ContinueDialogAsync(DialogContext dialogContext, CancellationToken cancellationToken = default)
    {
        _ = dialogContext ?? throw new ArgumentNullException(nameof(dialogContext));

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<DialogTurnResult>(cancellationToken);
        }

        if (dialogContext.Context.Activity.Type != ActivityTypes.Message)
        {
            return Task.FromResult(EndOfTurn);
        }

        return ExecuteDialogAsync(dialogContext, cancellationToken);
    }

    public override Task<DialogTurnResult> ResumeDialogAsync(
        DialogContext dialogContext, DialogReason reason, object? result = null, CancellationToken cancellationToken = default)
    {
        _ = dialogContext ?? throw new ArgumentNullException(nameof(dialogContext));

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<DialogTurnResult>(cancellationToken);
        }

        if (dialogContext.Context.Activity.Type != ActivityTypes.Message)
        {
            return Task.FromResult(EndOfTurn);
        }

        return ExecuteDialogAsync(dialogContext, cancellationToken);
    }

    private async Task<DialogTurnResult> ExecuteDialogAsync(DialogContext dialogContext, CancellationToken cancellationToken)
    {
        _ = dialogContext.ClearChatFlowLevel();
        var flowStepResult = await flowStep.Invoke(dialogContext, default, cancellationToken).ConfigureAwait(false);

        if (flowStepResult.Code == ChatFlowStepResultCode.Next)
        {
            return await dialogContext.EndDialogAsync(flowStepResult.FlowStateOrThrow(), cancellationToken).ConfigureAwait(false);
        }

        if (flowStepResult.Code == ChatFlowStepResultCode.Interruption)
        {
            return await dialogContext.EndDialogAsync(null, cancellationToken).ConfigureAwait(false);
        }

        return EndOfTurn;
    }
}
