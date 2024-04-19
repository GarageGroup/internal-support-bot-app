using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class CustomerAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitCustomer(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmCustomerApi crmCustomerApi)
        =>
        chatFlow.SetTypingStatus().AwaitLookupValue(
            crmCustomerApi.GetLastCustomersAsync,
            crmCustomerApi.SearchCustomersAsync,
            CustomerAwaitHelper.CreateResultMessage,
            static (flowState, customerValue) => flowState with
            {
                Customer = new()
                {
                    Id = customerValue.Id,
                    Title = customerValue.Name
                }
            });
}