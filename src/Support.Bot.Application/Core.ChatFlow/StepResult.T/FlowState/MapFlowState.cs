using System;

namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public ChatFlowStepResult<TResultFlowState> MapFlowState<TResultFlowState>(Func<TFlowState, TResultFlowState> mapFlowState)
        =>
        InnerMapFlowState(
            mapFlowState ?? throw new ArgumentNullException(nameof(mapFlowState)));

    private ChatFlowStepResult<TResultFlowState> InnerMapFlowState<TResultFlowState>(Func<TFlowState, TResultFlowState> mapFlowState)
        =>
        Code == ChatFlowStepResultCode.Next
            ? new(mapFlowState.Invoke(flowState))
            : new(Code);
}
