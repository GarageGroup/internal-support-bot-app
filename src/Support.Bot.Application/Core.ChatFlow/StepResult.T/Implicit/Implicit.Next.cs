namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public static implicit operator ChatFlowStepResult<TFlowState>(TFlowState flowState)
        =>
        new(flowState);
}
