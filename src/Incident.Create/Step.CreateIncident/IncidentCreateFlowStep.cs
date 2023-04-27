using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support;

internal static class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentLinkFlowState> CreateIncident(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IIncidentCreateSupplier supportApi)
        =>
        chatFlow.SendActivityOrSkip(
            IncidentCreateHelper.CreateTemporaryActivity,
            MapFlowState)
        .SetTypingStatus()
        .ForwardValue(
            supportApi.CreateIncidentOrBeakAsync,
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
            TemporaryActivityId = flowState.TemporaryActivityId
        };
}