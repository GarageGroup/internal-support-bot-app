using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support;

internal static class GptCallFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> CallGpt(this ChatFlow<IncidentCreateFlowState> chatFlow, ISupportGptApi gptApi)
        =>
        chatFlow.SetTypingStatus(
            GptCallHelper.CreateTemporaryActivity,
            ApplyTemporaryActivityId)
        .NextValue(
            gptApi.CompleteIncidentAsync)
        .ReplaceActivityOrSkip(
            GptCallHelper.CreateResultActivity);

    private static IncidentCreateFlowState ApplyTemporaryActivityId(IncidentCreateFlowState flowState, ResourceResponse activityResponse)
        =>
        flowState with
        {
            Gpt = flowState.Gpt with
            {
                TemporaryActivityId = activityResponse.Id
            }
        };
}