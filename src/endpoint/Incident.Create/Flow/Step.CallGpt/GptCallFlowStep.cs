using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class GptCallFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> CallGpt(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ISupportGptApi gptApi)
        =>
        chatFlow.SetTypingStatus().NextValue(
            gptApi.CompleteIncidentAsync);
}