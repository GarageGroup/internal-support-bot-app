namespace GGroupp.Infra;

partial class ChatFlow<TFlowState>
{
    public ChatFlow<TFlowState> Await()
        =>
        Forward(
            _ => ChatFlowStepResult.NextAndAwait<TFlowState>());
}
