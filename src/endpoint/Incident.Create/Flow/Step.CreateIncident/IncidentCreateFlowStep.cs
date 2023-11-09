using GarageGroup.Infra.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GarageGroup.Internal.Support;

internal static class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentLinkFlowState> CreateIncident(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmIncidentApi crmIncidentApi)
        =>
        chatFlow.SetTypingStatus(
            IncidentCreateHelper.CreateTemporaryActivity,
            MapFlowState)
        .ForwardValue(
            crmIncidentApi.CreateIncidentOrBeakAsync,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, ResourceResponse activityResponse)
        =>
        flowState with
        {
            TemporaryActivityId = activityResponse.Id
        };

    private static IncidentLinkFlowState MapFlowState(IncidentCreateFlowState flowState, IncidentCreateOut incident)
        =>
        new()
        {
            Title = incident.Title,
            Id = incident.Id,
            TemporaryActivityId = flowState.TemporaryActivityId,
            Gpt = flowState.Gpt
        };
}