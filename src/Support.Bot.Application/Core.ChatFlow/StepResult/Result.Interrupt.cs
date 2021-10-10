namespace GGroupp.Infra;

partial class ChatFlowStepResult
{
    public static ChatFlowStepResult<TFlowState> Interrupt<TFlowState>()
        =>
        ChatFlowStepResult<TFlowState>.Interrupt();

    public static ChatFlowStepAlternativeCode Interrupt()
        =>
        ChatFlowStepAlternativeCode.Interruption;
}
