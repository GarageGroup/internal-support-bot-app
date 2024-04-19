using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

internal static class CustomerAwaitHelper
{
    private const string DefaultText = "Нужно выбрать клиента. Введите часть названия для поиска";

    private const string CustomerChoiceText = "Выберите клиента или введите часть названия для поиска";

    private const string CustomerNotFoundText = "Не удалось найти ни одного клиента. Попробуйте уточнить запрос";

    private const int MaxCustomerSetCount = 6;

    internal static ValueTask<LookupValueSetOption> GetLastCustomersAsync(
        this ICrmCustomerApi crmCustomerApi,
        IChatFlowContext<IncidentCreateFlowState> context,
        CancellationToken cancellationToken)
    {
        if (context.FlowState.Customer is not null)
        {
            return new(
                result: new(default)
                {
                    SkipStep = true,
                });
        }

        return crmCustomerApi.InnerGetLastCustomersAsync(context, cancellationToken);
    }

    private static ValueTask<LookupValueSetOption> InnerGetLastCustomersAsync(
        this ICrmCustomerApi crmCustomerApi,
        IChatFlowContext<IncidentCreateFlowState> context,
        CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static state => new LastCustomerSetGetIn(
                userId: state.BotUserId.GetValueOrDefault(),
                minCreationTime: state.DbMinDate,
                top: MaxCustomerSetCount))
        .PipeValue(
            crmCustomerApi.GetLastAsync)
        .Fold(
            static @out => new(
                items: @out.Customers.Map(MapCustomerItem),
                choiceText: @out.Customers.IsNotEmpty ? CustomerChoiceText : DefaultText),
            context.LogFailure);

    internal static ValueTask<Result<LookupValueSetOption, BotFlowFailure>> SearchCustomersAsync(
        this ICrmCustomerApi crmCustomerApi,
        IChatFlowContext<IncidentCreateFlowState> _,
        string searchText,
        CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            searchText, cancellationToken)
        .Pipe(
            static text => new CustomerSetSearchIn(text)
            {
                Top = MaxCustomerSetCount
            })
        .PipeValue(
            crmCustomerApi.SearchAsync)
        .MapFailure(
            MapToFlowFailure)
        .Filter(
            static @out => @out.Customers.IsNotEmpty,
            static _ => BotFlowFailure.From(CustomerNotFoundText))
        .MapSuccess(
            static @out => new LookupValueSetOption(
                items: @out.Customers.Map(MapCustomerItem),
                choiceText: CustomerChoiceText));

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, LookupValue customerValue)
        =>
        $"Клиент: {context.EncodeHtmlTextWithStyle(customerValue.Name, BotTextStyle.Bold)}";

    private static LookupValue MapCustomerItem(CustomerItemOut item)
        =>
        new(item.Id, item.Title);

    private static BotFlowFailure MapToFlowFailure(Failure<CustomerSetGetFailureCode> failure)
        =>
        (failure.FailureCode switch
        {
            CustomerSetGetFailureCode.NotAllowed
                => "При поиске клиентов произошла ошибка. У вашей учетной записи не достаточно разрешений. Обратитесь к администратору приложения",
            CustomerSetGetFailureCode.TooManyRequests
                => "Слишком много обращений к сервису. Попробуйте повторить попытку через несколько секунд",
            _
                => "При поиске клиентов произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее"
        })
        .Pipe(
            message => BotFlowFailure.From(message, failure.FailureMessage));

    private static LookupValueSetOption LogFailure(this ILoggerSupplier context, Failure<CustomerSetGetFailureCode> failure)
    {
        context.Logger.LogError("Get last customers failure: {failureCode} {failureMessage}", failure.FailureCode, failure.FailureMessage);
        return new(default, DefaultText, default);
    }
}
