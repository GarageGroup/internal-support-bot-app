namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public static ChatFlowStepResult<TFlowState> NextAndAwait()
        =>
        new(ChatFlowStepResultCode.NextAndAwaiting);
}
