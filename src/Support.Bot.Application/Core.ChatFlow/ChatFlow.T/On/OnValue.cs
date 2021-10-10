using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TFlowState> OnValue(Func<TFlowState, CancellationToken, ValueTask> onAsync)
        =>
        InnerOnValue(
            onAsync ?? throw new ArgumentNullException(nameof(onAsync)));

    private ChatFlow<TFlowState> InnerOnValue(Func<TFlowState, CancellationToken, ValueTask> onAsync)
        =>
        InnerOn(
            async (flowState, token) => await onAsync.Invoke(flowState, token).ConfigureAwait(false));
}
