namespace GGroupp.Infra;

partial struct ChatFlowStepResult<TFlowState>
{
    public static bool Equals(ChatFlowStepResult<TFlowState> left, ChatFlowStepResult<TFlowState> right)
        =>
        left.Equals(right);

    public static bool operator ==(ChatFlowStepResult<TFlowState> left, ChatFlowStepResult<TFlowState> right)
        =>
        left.Equals(right);

    public static bool operator !=(ChatFlowStepResult<TFlowState> left, ChatFlowStepResult<TFlowState> right)
        =>
        left.Equals(right) is false;

    public override bool Equals(object? obj)
        =>
        obj is ChatFlowStepResult<TFlowState> other &&
        Equals(other);
}
