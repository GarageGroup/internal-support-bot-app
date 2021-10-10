using System;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TNextFlowState> MapFlowState<TNextFlowState>(
        Func<TFlowState, TNextFlowState> mapFlowState)
        =>
        InnerMapFlowState(
            mapFlowState ?? throw new ArgumentNullException(nameof(mapFlowState)));

    private ChatFlow<TNextFlowState> InnerMapFlowState<TNextFlowState>(
        Func<TFlowState, TNextFlowState> mapFlowState)
        =>
        InnerForward<TNextFlowState>(
            flowState => mapFlowState.Invoke(flowState));
}
