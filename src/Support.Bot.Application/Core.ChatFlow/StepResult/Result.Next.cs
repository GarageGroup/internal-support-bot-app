namespace GGroupp.Infra;

partial class ChatFlowStepResult
{
    public static ChatFlowStepResult<TFlowState> Next<TFlowState>(TFlowState flowState)
        =>
        ChatFlowStepResult<TFlowState>.Next(flowState);
}
