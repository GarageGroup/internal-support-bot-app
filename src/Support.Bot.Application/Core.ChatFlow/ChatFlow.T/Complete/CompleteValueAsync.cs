using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ValueTask<ChatFlowStepResult<TFlowState>> CompleteValueAsync(CancellationToken cancellationToken)
        =>
        cancellationToken.IsCancellationRequested
            ? ValueTask.FromCanceled<ChatFlowStepResult<TFlowState>>(cancellationToken)
            : InnerCompleteAsync(cancellationToken);

    private async ValueTask<ChatFlowStepResult<TFlowState>> InnerCompleteAsync(CancellationToken cancellationToken)
    {
        var result = await InnerGetStepResultAsync(cancellationToken).ConfigureAwait(false);

        if (result.Code == ChatFlowStepResultCode.NextAndAwaiting)
        {
            return ChatFlowStepResult.RetryAndAwait();
        }

        if (result.Code == ChatFlowStepResultCode.Next || result.Code == ChatFlowStepResultCode.Interruption)
        {
            _ = dialogContext.ClearChatFlowLevelResources(flowLevel);
            _ = dialogContext.SetChatFlowLevel(flowLevel - 1);
        }

        return result;
    }
}
