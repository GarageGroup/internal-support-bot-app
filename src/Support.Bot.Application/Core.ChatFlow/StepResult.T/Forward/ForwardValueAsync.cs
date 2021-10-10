using System;
using System.Threading.Tasks;

namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public ValueTask<ChatFlowStepResult<TResultFlowState>> ForwardValueAsync<TResultFlowState>(
        Func<TFlowState, ValueTask<ChatFlowStepResult<TResultFlowState>>> nextAsync)
        =>
        InnerForwardValueAsync(
            nextAsync ?? throw new ArgumentNullException(nameof(nextAsync)));

    public ValueTask<ChatFlowStepResult<TFlowState>> ForwardValueAsync(
        Func<TFlowState, ValueTask<ChatFlowStepResult<TFlowState>>> nextAsync)
        =>
        InnerForwardValueAsync(
            nextAsync ?? throw new ArgumentNullException(nameof(nextAsync)));

    private ValueTask<ChatFlowStepResult<TResultFlowState>> InnerForwardValueAsync<TResultFlowState>(
        Func<TFlowState, ValueTask<ChatFlowStepResult<TResultFlowState>>> nextAsync)
        =>
        Code == ChatFlowStepResultCode.Next
            ? nextAsync.Invoke(flowState)
            : ValueTask.FromResult(new ChatFlowStepResult<TResultFlowState>(Code));

    private ValueTask<ChatFlowStepResult<TFlowState>> InnerForwardValueAsync(
        Func<TFlowState, ValueTask<ChatFlowStepResult<TFlowState>>> nextAsync)
        =>
        Code == ChatFlowStepResultCode.Next
            ? nextAsync.Invoke(flowState)
            : ValueTask.FromResult(this);
}
