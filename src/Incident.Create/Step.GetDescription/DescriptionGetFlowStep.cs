using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class DescriptionGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetDescription(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.Forward(GetDescriptionOrFailure);

    private static ChatFlowJump<IncidentCreateFlowState> GetDescriptionOrFailure(
        IChatFlowContext<IncidentCreateFlowState> context)
    {
        var text = context.Activity.Text;

        if (string.IsNullOrEmpty(text) || context.GetCardActionValueOrAbsent().IsPresent)
        {
            return default;
        }

        return context.FlowState with
        {
            Description = context.Activity.Text
        };
    }
}