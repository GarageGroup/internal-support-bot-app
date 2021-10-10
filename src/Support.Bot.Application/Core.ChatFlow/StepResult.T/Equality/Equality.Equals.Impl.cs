namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public bool Equals(ChatFlowStepResult<TFlowState> other)
        =>
        CodeComparer.Equals(Code, other.Code) &&
        FlowStateComparer.Equals(flowState, other.flowState);
}
