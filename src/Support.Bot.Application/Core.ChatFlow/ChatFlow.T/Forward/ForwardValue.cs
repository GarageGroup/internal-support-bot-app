using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TNextFlowState> ForwardValue<TNextFlowState>(
        Func<TFlowState, CancellationToken, ValueTask<ChatFlowStepResult<TNextFlowState>>> nextAsync)
        =>
        InnerForwardValue(
            nextAsync ?? throw new ArgumentNullException(nameof(nextAsync)));

    private ChatFlow<TNextFlowState> InnerForwardValue<TNextFlowState>(
        Func<TFlowState, CancellationToken, ValueTask<ChatFlowStepResult<TNextFlowState>>> nextAsync)
        =>
        new(
            flowLevel: flowLevel,
            flowPosition: flowPosition + 1,
            dialogContext: dialogContext,
            flowStep: token => GetNextStepResultAsync(nextAsync, token));
}
