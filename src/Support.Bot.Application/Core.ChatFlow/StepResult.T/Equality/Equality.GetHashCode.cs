using System;

namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public override int GetHashCode()
        =>
        Code == ChatFlowStepResultCode.Next ? PresentFlowStateHashCode() : AbsentFlowStateHashCode();

    private int PresentFlowStateHashCode()
        =>
        flowState is not null
            ? HashCode.Combine(EqualityContract, true, CodeComparer.GetHashCode(Code), FlowStateComparer.GetHashCode(flowState))
            : HashCode.Combine(EqualityContract, true, CodeComparer.GetHashCode(Code));

    private int AbsentFlowStateHashCode()
        =>
        HashCode.Combine(EqualityContract, CodeComparer.GetHashCode(Code));
}
