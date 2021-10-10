using System;

namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public ChatFlowStepResult<TResultFlowState> Forward<TResultFlowState>(
        Func<TFlowState, ChatFlowStepResult<TResultFlowState>> next)
        =>
        InnerForward(
            next ?? throw new ArgumentNullException(nameof(next)));

    public ChatFlowStepResult<TFlowState> Forward(
        Func<TFlowState, ChatFlowStepResult<TFlowState>> next)
        =>
        InnerForward(
            next ?? throw new ArgumentNullException(nameof(next)));

    private ChatFlowStepResult<TResultFlowState> InnerForward<TResultFlowState>(
        Func<TFlowState, ChatFlowStepResult<TResultFlowState>> next)
        =>
        Code == ChatFlowStepResultCode.Next
            ? next.Invoke(flowState)
            : new(Code);

    private ChatFlowStepResult<TFlowState> InnerForward(
        Func<TFlowState, ChatFlowStepResult<TFlowState>> next)
        =>
        Code == ChatFlowStepResultCode.Next
            ? next.Invoke(flowState)
            : this;
}
