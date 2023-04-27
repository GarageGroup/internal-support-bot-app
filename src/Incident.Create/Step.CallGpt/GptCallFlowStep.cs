using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support;

internal static class GptCallFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> CallGpt(this ChatFlow<IncidentCreateFlowState> chatFlow, ISupportGptApi gptApi)
        =>
        chatFlow.SendActivityOrSkip(
            GptCallHelper.CreateTemporaryActivity,
            ApplyTemporaryActivityId)
        .SetTypingStatus()
        .NextValue(
            gptApi.CompleteIncidentAsync)
        .ReplaceActivityOrSkip(
            GptCallHelper.CreateResultActivity);

    private static IncidentCreateFlowState ApplyTemporaryActivityId(IncidentCreateFlowState flowState, ResourceResponse activityResponse)
        =>
        flowState with
        {
            Gpt = new()
            {
                TemporaryActivityId = activityResponse.Id
            }
        };
}