using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class CustomerAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitCustomer(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmCustomerApi crmCustomerApi)
        =>
        chatFlow.SendText(
            static _ => "Нужно выбрать клиента. Введите часть названия для поиска")
        .AwaitLookupValue(
            (_, search, token) => crmCustomerApi.SearchCustomersOrFailureAsync(search, token),
            CustomerAwaitHelper.CreateResultMessage,
            static (flowState, customerValue) => flowState with
            {
                CustomerId = customerValue.Id,
                CustomerTitle = customerValue.Name
            });
}