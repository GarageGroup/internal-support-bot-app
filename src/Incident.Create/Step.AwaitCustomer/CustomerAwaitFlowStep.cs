using System;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;

internal static class CustomerAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitCustomer(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICustomerSetSearchFunc customerSetSearchFunc)
        =>
        chatFlow.SendText(
            static _ => "Нужно выбрать клиента. Введите часть названия для поиска")
        .AwaitLookupValue(
            (_, search, token) => customerSetSearchFunc.SearchCustomersOrFailureAsync(search, token),
            CustomerAwaitHelper.CreateResultMessage,
            static (flowState, customerValue) => flowState with
            {
                CustomerId = customerValue.Id,
                CustomerTitle = customerValue.Name
            });
}