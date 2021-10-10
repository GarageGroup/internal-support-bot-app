namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public static ChatFlowStepResult<TFlowState> Interrupt()
        =>
        new(ChatFlowStepResultCode.Interruption);
}
