using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;

internal static class CustomerFindHelper
{
    private const int MaxCustomerSetCount = 6;

    internal static ValueTask<Result<LookupValueSetOption, BotFlowFailure>> SearchCustomersOrFailureAsync(
        this ICustomerSetSearchFunc customerSetSearchFunc, string seachText, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            seachText, cancellationToken)
        .HandleCancellation()
        .Pipe(
            static text => new CustomerSetSearchIn(
                searchText: text,
                top: MaxCustomerSetCount))
        .PipeValue(
            customerSetSearchFunc.InvokeAsync)
        .MapFailure(
            MapToFlowFailure)
        .Filter(
            static @out => @out.Customers.Any(),
            static _ => BotFlowFailure.From("Не удалось найти ни одного клиента. Попробуйте уточнить запрос"))
        .MapSuccess(
            static @out => new LookupValueSetOption(
                items: @out.Customers.Select(MapCustomerItem).ToArray(),
                choiceText: "Выберите клиента"));

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, LookupValue customerValue)
        =>
        $"Клиент: {context.EncodeTextWithStyle(customerValue.Name, BotTextStyle.Bold)}";

    private static LookupValue MapCustomerItem(CustomerItemSearchOut item)
        =>
        new(item.Id, item.Title);

    private static BotFlowFailure MapToFlowFailure(Failure<CustomerSetSearchFailureCode> failure)
        =>
        (failure.FailureCode switch
        {
            CustomerSetSearchFailureCode.NotAllowed
                => "При поиске клиентов произошла ошибка. У вашей учетной записи не достаточно разрешений. Обратитесь к администратору приложения",
            CustomerSetSearchFailureCode.TooManyRequests
                => "Слишком много обращений к сервису. Попробуйте повторить попытку через несколько секунд",
            _
                => "При поиске клиентов произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее"
        })
        .Pipe(
            message => BotFlowFailure.From(message, failure.FailureMessage));
}