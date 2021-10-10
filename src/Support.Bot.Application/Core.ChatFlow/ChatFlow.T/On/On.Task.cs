using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TFlowState> On(Func<TFlowState, CancellationToken, Task> onAsync)
        =>
        InnerOn(
            onAsync ?? throw new ArgumentNullException(nameof(onAsync)));

    private ChatFlow<TFlowState> InnerOn(Func<TFlowState, CancellationToken, Task> onAsync)
        =>
        InnerMapFlowStateValue(
            async (flowState, token) =>
            {
                await onAsync.Invoke(flowState, token).ConfigureAwait(false);
                return flowState;
            });
}
