using System;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TNextFlowState> ForwardChildValue<TChildFlowStateIn, TChildFlowStateOut, TNextFlowState>(
        Func<TFlowState, TChildFlowStateIn> mapFlowStateIn,
        ChatFlowStep<TChildFlowStateIn, TChildFlowStateOut> nextAsync,
        Func<TFlowState, TChildFlowStateOut, TNextFlowState> mapFlowStateOut)
        =>
        InnerForwardChildValue(
            mapFlowStateIn ?? throw new ArgumentNullException(nameof(mapFlowStateIn)),
            nextAsync ?? throw new ArgumentNullException(nameof(nextAsync)),
            mapFlowStateOut ?? throw new ArgumentNullException(nameof(mapFlowStateOut)));

    private ChatFlow<TNextFlowState> InnerForwardChildValue<TChildFlowStateIn, TChildFlowStateOut, TNextFlowState>(
        Func<TFlowState, TChildFlowStateIn> mapFlowStateIn,
        ChatFlowStep<TChildFlowStateIn, TChildFlowStateOut> nextAsync,
        Func<TFlowState, TChildFlowStateOut, TNextFlowState> mapFlowStateOut)
        =>
        InnerForwardValue(
            async (flowState, token) =>
            {
                var childFlowStateIn = mapFlowStateIn.Invoke(flowState);
                var stepResult = await nextAsync.Invoke(dialogContext, childFlowStateIn, token).ConfigureAwait(false);
                return stepResult.MapFlowState(flowStateOut => mapFlowStateOut.Invoke(flowState, flowStateOut));
            });
}
