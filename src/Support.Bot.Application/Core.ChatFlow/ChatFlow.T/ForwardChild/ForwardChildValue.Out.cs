using System;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TNextFlowState> ForwardChildValue<TChildFlowStateOut, TNextFlowState>(
        ChatFlowStep<TFlowState, TChildFlowStateOut> nextAsync,
        Func<TFlowState, TChildFlowStateOut, TNextFlowState> mapFlowStateOut)
        =>
        InnerForwardChildValue(
            nextAsync ?? throw new ArgumentNullException(nameof(nextAsync)),
            mapFlowStateOut ?? throw new ArgumentNullException(nameof(mapFlowStateOut)));

    private ChatFlow<TNextFlowState> InnerForwardChildValue<TChildFlowStateOut, TNextFlowState>(
        ChatFlowStep<TFlowState, TChildFlowStateOut> nextAsync,
        Func<TFlowState, TChildFlowStateOut, TNextFlowState> mapFlowStateOut)
        =>
        InnerForwardValue(
            async (flowState, token) =>
            {
                var stepResult = await nextAsync.Invoke(dialogContext, flowState, token).ConfigureAwait(false);
                return stepResult.MapFlowState(flowStateOut => mapFlowStateOut.Invoke(flowState, flowStateOut));
            });
}
