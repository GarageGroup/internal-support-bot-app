using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class OwnerAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitOwner(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IUserSetSearchSupplier supportApi)
        =>
        chatFlow.SendText(
            _ => "Нужно выбрать ответственного")
        .AwaitLookupValue(
            OwnerAwaitHelper.GetDefaultOwnerAsync,
            supportApi.SearchUsersOrFailureAsync,
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