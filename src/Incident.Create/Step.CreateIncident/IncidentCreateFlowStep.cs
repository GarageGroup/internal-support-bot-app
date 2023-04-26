using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentLinkFlowState> CreateIncident(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IIncidentCreateSupplier supportApi)
        =>
        chatFlow.SetTypingStatus().ForwardValue(
            supportApi.CreateIncidentOrBeakAsync);
}