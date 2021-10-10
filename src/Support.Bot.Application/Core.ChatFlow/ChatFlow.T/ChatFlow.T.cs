using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Infra;

public sealed partial class ChatFlow<TFlowState>
{
    private readonly int flowLevel;

    private readonly int flowPosition;

    private readonly DialogContext dialogContext;

    private readonly Func<CancellationToken, ValueTask<ChatFlowStepResult<TFlowState>>> flowStep;

    private ChatFlow(
        int flowLevel,
        int flowPosition,
        DialogContext dialogContext,
        Func<CancellationToken, ValueTask<ChatFlowStepResult<TFlowState>>> flowStep)
    {
        this.flowLevel = flowLevel;
        this.flowPosition = flowPosition;
        this.dialogContext = dialogContext;
        this.flowStep = flowStep;
    }

    private async ValueTask<ChatFlowStepResult<TNextFlowState>> InnerGetNextStepResultAsync<TNextFlowState>(
        Func<TFlowState, CancellationToken, ValueTask<ChatFlowStepResult<TNextFlowState>>> nextAsync, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return await ValueTask.FromCanceled<ChatFlowStepResult<TNextFlowState>>(token);
        }

        var stepResult = await InnerGetStepResultAsync(token).ConfigureAwait(false);
        return await stepResult.ForwardValueAsync(
            async flowState =>
            {
                _ = dialogContext.SetChatFlowState(flowLevel, flowState);
                var nextResult = await nextAsync.Invoke(flowState, token).ConfigureAwait(false);

                if (nextResult.Code != ChatFlowStepResultCode.RetryAndAwaiting)
                {
                    _ = dialogContext.SetChatFlowPosition(flowLevel: flowLevel, position: flowPosition + 1);
                }

                return nextResult;
            })
            .ConfigureAwait(false);
    }

    private async ValueTask<ChatFlowStepResult<TFlowState>> InnerGetStepResultAsync(CancellationToken cancellationToken)
        =>
        dialogContext.GetChatFlowPosition(flowLevel) >= flowPosition
            ? dialogContext.GetChatFlowState<TFlowState>(flowLevel)
            : await flowStep.Invoke(cancellationToken).ConfigureAwait(false);
}
