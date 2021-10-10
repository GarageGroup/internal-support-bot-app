namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public static ChatFlowStepResult<TFlowState> RetryAndAwait()
        =>
        new(ChatFlowStepResultCode.RetryAndAwaiting);
}
