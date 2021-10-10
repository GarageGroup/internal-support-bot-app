namespace GGroupp.Infra;

partial class ChatFlowStepResult
{
    public static ChatFlowStepResult<TFlowState> RetryAndAwait<TFlowState>()
        =>
        ChatFlowStepResult<TFlowState>.RetryAndAwait();

    public static ChatFlowStepAlternativeCode RetryAndAwait()
        =>
        ChatFlowStepAlternativeCode.RetryAndAwaiting;
}
