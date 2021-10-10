using System;
using System.Threading.Tasks;

namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public Task<ChatFlowStepResult<TResultFlowState>> ForwardAsync<TResultFlowState>(
        Func<TFlowState, Task<ChatFlowStepResult<TResultFlowState>>> nextAsync)
        =>
        InnerForwardAsync(
            nextAsync ?? throw new ArgumentNullException(nameof(nextAsync)));

    public Task<ChatFlowStepResult<TFlowState>> ForwardAsync(
        Func<TFlowState, Task<ChatFlowStepResult<TFlowState>>> nextAsync)
        =>
        InnerForwardAsync(
            nextAsync ?? throw new ArgumentNullException(nameof(nextAsync)));

    private Task<ChatFlowStepResult<TResultFlowState>> InnerForwardAsync<TResultFlowState>(
        Func<TFlowState, Task<ChatFlowStepResult<TResultFlowState>>> nextAsync)
        =>
        Code == ChatFlowStepResultCode.Next
            ? nextAsync.Invoke(flowState)
            : Task.FromResult(new ChatFlowStepResult<TResultFlowState>(Code));

    private Task<ChatFlowStepResult<TFlowState>> InnerForwardAsync(
        Func<TFlowState, Task<ChatFlowStepResult<TFlowState>>> nextAsync)
        =>
        Code == ChatFlowStepResultCode.Next
            ? nextAsync.Invoke(flowState)
            : Task.FromResult(this);
}
