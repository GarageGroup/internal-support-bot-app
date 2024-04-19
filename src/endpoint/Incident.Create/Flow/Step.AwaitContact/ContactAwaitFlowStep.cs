using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class ContactAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitContact(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmContactApi crmContactApi)
        =>
        chatFlow.AwaitLookupValue(
            crmContactApi.GetDefaultContactsAsync,
            crmContactApi.SearchContactsOrFailureAsync,
            ContactAwaitHelper.CreateResultMessage,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, LookupValue contactValue)
        =>
        contactValue.IsNotSkipValueOrAbsent().Map(flowState.WithContactValue).OrElse(flowState.WithEmptyContactValue());

    private static IncidentCreateFlowState WithContactValue(this IncidentCreateFlowState flowState, LookupValue contactValue)
        =>
        flowState with 
        {
            Contact = new()
            {
                Id = contactValue.Id,
                FullName = contactValue.Name
            }            
        };

    private static IncidentCreateFlowState WithEmptyContactValue(this IncidentCreateFlowState flowState)
        =>
        flowState with
        {
            Contact = new()
        };
}