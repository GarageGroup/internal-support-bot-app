using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class OwnerGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetOwner(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.ForwardValue(
            OwnerGetHelper.GetOwnerValueOrBreakAsync,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, LookupValue ownerValue)
        =>
        flowState with
        { 
            OwnerId = ownerValue.Id, 
            OwnerFullName = ownerValue.Name
        };
}