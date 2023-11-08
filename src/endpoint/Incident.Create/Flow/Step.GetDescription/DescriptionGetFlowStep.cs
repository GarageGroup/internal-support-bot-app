using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class DescriptionGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetDescription(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.Forward(
            DescriptionGetHelper.GetDescriptionOrBreak);
}