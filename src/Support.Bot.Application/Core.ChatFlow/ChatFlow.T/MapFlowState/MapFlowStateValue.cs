using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TNextFlowState> MapFlowStateValue<TNextFlowState>(
        Func<TFlowState, CancellationToken, ValueTask<TNextFlowState>> mapFlowStateAsync)
        =>
        InnerMapFlowStateValue(
            mapFlowStateAsync ?? throw new ArgumentNullException(nameof(mapFlowStateAsync)));

    private ChatFlow<TNextFlowState> InnerMapFlowStateValue<TNextFlowState>(
        Func<TFlowState, CancellationToken, ValueTask<TNextFlowState>> mapFlowStateAsync)
        =>
        InnerForwardValue<TNextFlowState>(
            async (flowState, token) => await mapFlowStateAsync.Invoke(flowState, token).ConfigureAwait(false));
}
