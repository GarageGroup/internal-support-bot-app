using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class OwnerAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitOwner(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmUserApi crmUserApi)
        =>
        chatFlow.SendText(
            _ => "Нужно выбрать ответственного")
        .AwaitLookupValue(
            OwnerAwaitHelper.GetDefaultOwnerAsync,
            crmUserApi.SearchUsersOrFailureAsync,
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