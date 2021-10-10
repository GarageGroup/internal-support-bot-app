namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public static implicit operator ChatFlowStepResult<TFlowState>(ChatFlowStepAlternativeCode alternativeCode)
        =>
        alternativeCode switch
        {
            ChatFlowStepAlternativeCode.RetryAndAwaiting => new(ChatFlowStepResultCode.RetryAndAwaiting),
            ChatFlowStepAlternativeCode.NextAndAwaiting => new(ChatFlowStepResultCode.NextAndAwaiting),
            _ => new(ChatFlowStepResultCode.Interruption)
        };
}
