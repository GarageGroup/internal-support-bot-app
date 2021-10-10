using System;
using System.Threading.Tasks;

namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TNextFlowState> Forward<TNextFlowState>(
        Func<TFlowState, ChatFlowStepResult<TNextFlowState>> next)
        =>
        InnerForward(
            next ?? throw new ArgumentNullException(nameof(next)));

    private ChatFlow<TNextFlowState> InnerForward<TNextFlowState>(
        Func<TFlowState, ChatFlowStepResult<TNextFlowState>> next)
        =>
        InnerForwardValue(
            (flowState, _) => ValueTask.FromResult(next.Invoke(flowState)));
}
