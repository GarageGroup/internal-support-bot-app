using System;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class IncidentShowFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ShowIncident(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IncidentCreateFlowOption option)
        =>
        chatFlow.ReplaceActivityOrSkip(
            option.CreateIncidentActivity);
}