using System;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TFlowState> On(Action<TFlowState> on)
        =>
        InnerOn(
            on ?? throw new ArgumentNullException(nameof(on)));

    private ChatFlow<TFlowState> InnerOn(Action<TFlowState> on)
        =>
        InnerMapFlowState(
            flowState =>
            {
                on.Invoke(flowState);
                return flowState;
            });
}
