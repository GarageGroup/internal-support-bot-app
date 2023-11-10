using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class OwnerAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitOwner(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmOwnerApi crmOwnerApi)
        =>
        chatFlow.SetTypingStatus().AwaitLookupValue(
            crmOwnerApi.GetDefaultOwnersAsync,
            crmOwnerApi.SearchUsersOrFailureAsync,
            OwnerAwaitHelper.CreateResultMessage,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, LookupValue ownerValue)
        =>
        flowState with
        {
            OwnerId = ownerValue.Id,
            OwnerFullName = ownerValue.Name
        };
}