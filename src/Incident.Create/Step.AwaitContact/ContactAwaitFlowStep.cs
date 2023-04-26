using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class ContactAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitContact(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IContactSetSearchSupplier supportApi)
        =>
        chatFlow.AwaitLookupValue(
            supportApi.GetDefaultContactsAsync,
            supportApi.SearchContactsOrFailureAsync,
            ContactAwaitHelper.CreateResultMessage,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, LookupValue contactValue)
        =>
        contactValue.IsNotSkipValueOrAbsent().Map(flowState.WithContactValue).OrElse(flowState);

    private static IncidentCreateFlowState WithContactValue(this IncidentCreateFlowState flowState, LookupValue contactValue)
        =>
        flowState with 
        { 
            ContactId = contactValue.Id,
            ContactFullName = contactValue.Name
        };
}