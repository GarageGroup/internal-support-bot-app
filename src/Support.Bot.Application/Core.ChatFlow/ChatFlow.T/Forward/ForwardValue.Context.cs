using System;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TNextFlowState> ForwardValue<TNextFlowState>(
        ChatFlowStep<TFlowState, TNextFlowState> nextAsync)
        =>
        InnerForwardValue(
            nextAsync ?? throw new ArgumentNullException(nameof(nextAsync)));

    private ChatFlow<TNextFlowState> InnerForwardValue<TNextFlowState>(
        ChatFlowStep<TFlowState, TNextFlowState> nextAsync)
        =>
        InnerForwardValue(
            (flowState, token) => nextAsync.Invoke(dialogContext, flowState, token));
}
