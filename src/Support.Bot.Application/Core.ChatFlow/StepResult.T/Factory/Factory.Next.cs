namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public static ChatFlowStepResult<TFlowState> Next(TFlowState flowState)
        =>
        new(flowState);
}
