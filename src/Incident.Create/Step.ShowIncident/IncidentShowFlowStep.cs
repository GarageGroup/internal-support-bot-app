using System;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class IncidentShowFlowStep
{
    internal static ChatFlow<Unit> ShowIncident(
        this ChatFlow<IncidentLinkFlowState> chatFlow, IncidentCreateFlowOption option)
        =>
        chatFlow.ReplaceActivityOrSkip(
            option.CreateIncidentActivity)
        .MapFlowState(
            Unit.From);
}