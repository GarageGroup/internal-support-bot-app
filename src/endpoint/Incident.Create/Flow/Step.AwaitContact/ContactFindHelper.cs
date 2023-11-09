using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

internal static class ContactAwaitHelper
{
    private const string ChooseOrSkip = "Выберите контакт или введите часть имени для поиска. Этот шаг можно пропустить";

    private const string UnsuccessfulDefaultResultText = "Клиенты для данного контакта еще не добавлены. Этот шаг можно пропустить";

    private const string UnsuccessfulSearchResultText = "Не удалось найти ни одного контакта. Попробуйте уточнить запрос";

    private const string SkipButtonText = "ПРОПУСТИТЬ";

    private const int MaxCustomerSetCount = 5;

    private static readonly Guid SkipId;

    private static readonly LookupValue SkipValue;

    static ContactAwaitHelper()
    {
        SkipId = Guid.Parse("6e271f6a-07b2-4887-af8d-938c66300387");
        SkipValue = new(SkipId, SkipButtonText);
    }

    internal static ValueTask<LookupValueSetOption> GetDefaultContactsAsync(
        this ICrmContactApi crmContactApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken token)
        =>
        AsyncPipeline.Pipe(
            context.FlowState.CustomerId, token)
        .Pipe(
            static customerId => new ContactSetSearchIn(
                searchText: string.Empty,
                customerId: customerId)
            {
                Top = MaxCustomerSetCount
            })
        .PipeValue(
            crmContactApi.SearchAsync)
        .Fold(
            static @out => new(
                items: @out.Contacts.AsEnumerable().Select(MapContactItem).ToList().AddSkipValue(),
                choiceText: @out.Contacts.IsNotEmpty ? ChooseOrSkip : UnsuccessfulDefaultResultText),
            failure => MapSearchFailure(failure, context.Logger));

    internal static ValueTask<Result<LookupValueSetOption, BotFlowFailure>> SearchContactsOrFailureAsync(
        this ICrmContactApi crmContactApi,
        IChatFlowContext<IncidentCreateFlowState> context,
        string seachText,
        CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .HandleCancellation()
        .Pipe(
            flowState => new ContactSetSearchIn(
                searchText: seachText,
                customerId: flowState.CustomerId)
            {
                Top = MaxCustomerSetCount
            })
        .PipeValue(
            crmContactApi.SearchAsync)
        .MapFailure(
            MapToFlowFailure)
        .MapSuccess(
            static @out => new LookupValueSetOption(
                items: @out.Contacts.AsEnumerable().Select(MapContactItem).ToList().AddSkipValue(),
                choiceText: @out.Contacts.IsNotEmpty ? ChooseOrSkip : UnsuccessfulSearchResultText));

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, LookupValue contactValue)
        =>
        $"Контакт: {context.EncodeHtmlTextWithStyle(contactValue.Name, BotTextStyle.Bold)}";

    internal static Optional<LookupValue> IsNotSkipValueOrAbsent(this LookupValue contactValue)
        => 
        contactValue.Id != SkipId ? new(contactValue) : default;

    private static FlatArray<LookupValue> AddSkipValue(this List<LookupValue> lookupValues)
    {
        lookupValues.Add(SkipValue);
        return lookupValues;
    }

    private static LookupValue MapContactItem(ContactItemOut item)
        =>
        new(item.Id, item.FullName);

    private static LookupValueSetOption MapSearchFailure(Failure<ContactSetSearchFailureCode> failure, ILogger logger)
    {
        logger.LogError("Search contacts failure: {failureCode} {failureMessage}", failure.FailureCode, failure.FailureMessage);
        return new(items: new[] { SkipValue }, choiceText: ChooseOrSkip);
    }

    private static BotFlowFailure MapToFlowFailure(Failure<ContactSetSearchFailureCode> failure)
        =>
        (failure.FailureCode switch
        {
            ContactSetSearchFailureCode.NotAllowed
                => "При поиске контактов произошла ошибка. У вашей учетной записи не достаточно разрешений. Обратитесь к администратору приложения",
            ContactSetSearchFailureCode.TooManyRequests
                => "Слишком много обращений к сервису. Попробуйте повторить попытку через несколько секунд",
            _
                => "При поиске контактов произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее"
        })
        .Pipe(
            message => BotFlowFailure.From(message, failure.FailureMessage));
}