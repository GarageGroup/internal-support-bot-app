using System;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;

internal static class CustomerFindFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> FindCustomer(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICustomerSetSearchFunc customerSetSearchFunc)
        =>
        chatFlow.SendText(
            static _ => "Нужно выбрать клиента. Введите часть названия для поиска")
        .AwaitLookupValue(
            (_, search, token) => customerSetSearchFunc.SearchCustomersAsync(search, token),
            CreateResultMessage,
            static (flowState, customerValue) => flowState with
            {
                CustomerId = customerValue.Id,
                CustomerTitle = customerValue.Name
            });

    private static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, LookupValue customerValue)
        =>
        $"Клиент: {context.EncodeTextWithStyle(customerValue.Name, BotTextStyle.Bold)}";
}