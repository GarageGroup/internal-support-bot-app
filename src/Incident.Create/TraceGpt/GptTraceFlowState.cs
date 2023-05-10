using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class GptTraceFlowState
{
    internal static ChatFlow<IncidentLinkFlowState> TraceGpt(
        this ChatFlow<IncidentLinkFlowState> chatFlow, IncidentCreateFlowOption option)
        =>
        chatFlow.On(option.TraceGpt);
}