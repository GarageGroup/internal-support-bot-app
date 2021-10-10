using System;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TNextFlowState> ForwardChildValue<TChildFlowStateIn, TNextFlowState>(
        Func<TFlowState, TChildFlowStateIn> mapFlowStateIn,
        ChatFlowStep<TChildFlowStateIn, TNextFlowState> nextAsync)
        =>
        InnerForwardChildValue(
            mapFlowStateIn ?? throw new ArgumentNullException(nameof(mapFlowStateIn)),
            nextAsync ?? throw new ArgumentNullException(nameof(nextAsync)));

    private ChatFlow<TNextFlowState> InnerForwardChildValue<TChildFlowStateIn, TNextFlowState>(
        Func<TFlowState, TChildFlowStateIn> mapFlowStateIn,
        ChatFlowStep<TChildFlowStateIn, TNextFlowState> nextAsync)
        =>
        InnerForwardValue(
            (flowState, token) =>
            {
                var childFlowStateIn = mapFlowStateIn.Invoke(flowState);
                return nextAsync.Invoke(dialogContext, childFlowStateIn, token);
            });
}
