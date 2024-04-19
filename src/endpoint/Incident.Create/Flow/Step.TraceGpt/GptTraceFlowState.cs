using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class GptTraceFlowState
{
    internal static ChatFlow<IncidentCreateFlowState> TraceGpt(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IncidentCreateFlowOption option)
        =>
        chatFlow.On(option.TraceGpt);
}