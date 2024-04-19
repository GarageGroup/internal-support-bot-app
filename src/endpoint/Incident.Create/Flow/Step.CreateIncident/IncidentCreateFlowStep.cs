using GarageGroup.Infra.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GarageGroup.Internal.Support;

internal static class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> CreateIncident(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmIncidentApi crmIncidentApi)
        =>
        chatFlow.SetTypingStatus(
            IncidentCreateHelper.CreateTemporaryActivity,
            MapFlowState)
        .ForwardValue(
            crmIncidentApi.CreateIncidentOrBeakAsync);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, ResourceResponse activityResponse)
        =>
        flowState with
        {
            TemporaryActivityId = activityResponse.Id
        };
}