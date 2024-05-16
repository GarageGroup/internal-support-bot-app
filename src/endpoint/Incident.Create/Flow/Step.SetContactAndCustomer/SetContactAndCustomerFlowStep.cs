using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class SetContactAndCustomerFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> SetContactAndCustomer(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmContactApi crmContactApi)
        =>
        chatFlow.NextValue(crmContactApi.SetContactAndCustomersAsync);
}