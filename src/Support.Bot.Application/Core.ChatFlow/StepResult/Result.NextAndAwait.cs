namespace GGroupp.Infra;

partial class ChatFlowStepResult
{
    public static ChatFlowStepResult<TFlowState> NextAndAwait<TFlowState>()
        =>
        ChatFlowStepResult<TFlowState>.NextAndAwait();

    public static ChatFlowStepAlternativeCode NextAndAwait()
        =>
        ChatFlowStepAlternativeCode.NextAndAwaiting;
}
