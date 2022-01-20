using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;

internal static class CustomerFindFlowStep
{
    private const int MaxCustomerSetCount = 5;

    internal static ChatFlow<IncidentCreateFlowState> FindCustomer(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICustomerSetSearchFunc customerSetSearchFunc)
        =>
        chatFlow.SendText(
            static _ => "Нужно выбрать клиента. Введите часть названия для поиска")
        .AwaitLookupValue(
            (_, search, token) => customerSetSearchFunc.SearchCustomersAsync(search, token),
            static (flowState, customerValue) => flowState with
            {
                CustomerId = customerValue.Id,
                CustomerTitle = customerValue.Name
            });

    private static ValueTask<Result<LookupValueSetSeachOut, BotFlowFailure>> SearchCustomersAsync(
        this ICustomerSetSearchFunc customerSetSearchFunc, LookupValueSetSeachIn seachInput, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            seachInput, cancellationToken)
        .Pipe(
            static @in => new CustomerSetSearchIn(
                searchText: @in.Text,
                top: MaxCustomerSetCount))
        .PipeValue(
            customerSetSearchFunc.InvokeAsync)
        .MapFailure(
            MapToFlowFailure)
        .Filter(
            static @out => @out.Customers.Any(),
            static _ => BotFlowFailure.From("Не удалось найти ни одного клиента. Попробуйте уточнить запрос"))
        .MapSuccess(
            static @out => new LookupValueSetSeachOut(
                items: @out.Customers.Select(MapCustomerItem).ToArray(),
                choiceText: "Выберите клиента"));

    private static LookupValue MapCustomerItem(CustomerItemSearchOut item)
        =>
        new(item.Id, item.Title);

    private static BotFlowFailure MapToFlowFailure(Failure<CustomerSetSearchFailureCode> failure)
        =>
        new(
            userMessage: "При выполнении запроса произошла непредвиденная ошибка. Обратитесь к администратору и повторите попытку позднее",
            logMessage: failure.FailureMessage);
}