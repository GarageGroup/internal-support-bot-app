using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

internal static class ContactFindFlowStep
{
    private const string ChooseOrPass = "Выберите контакт или пропустите";
    private const string PassButtonText = "ПРОПУСТИТЬ";
    private const int MaxCustomerSetCount = 5;

    private static Guid PassId;

    static ContactFindFlowStep()
        =>
        PassId = Guid.Parse("6e271f6a-07b2-4887-af8d-938c66300387");

    internal static ChatFlow<IncidentCreateFlowState> FindContcat(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IContactSetSearchFunc contactSetSearchFunc, ILoggerFactory loggerFactory)
        =>
        chatFlow.SendText(
            static _ => "Нужно выбрать контакт. Введите часть имени для поиска")
        .AwaitLookupValue(
            (flowState, token) => SetDefaultLookUpValue(flowState, contactSetSearchFunc, loggerFactory.CreateLogger("ContactFind"), token),
            (flowState, search, token) => contactSetSearchFunc.SearchContactsAsync(flowState, search, token),
            MapFlowState);

    private static ValueTask<Result<LookupValueSetSeachOut, BotFlowFailure>> SearchContactsAsync(
        this IContactSetSearchFunc contactSetSearchFunc,
        IncidentCreateFlowState flowState,
        LookupValueSetSeachIn seachInput,
        CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            seachInput, cancellationToken)
        .Pipe(
            @in => new ContactSetSearchIn(
                searchText: @in.Text,
                customerId: flowState.CustomerId,
                MaxCustomerSetCount))
        .PipeValue(
            contactSetSearchFunc.InvokeAsync)
        .MapFailure(
            MapToFlowFailure)
        .MapSuccess(
            static @out => new LookupValueSetSeachOut(
                items: @out.Contacts.Select(MapContactItem).ToList().AddPassValue(),
                choiceText: ChooseOrPass));

    private static ValueTask<LookupValueSetSeachOut> SetDefaultLookUpValue(
        IncidentCreateFlowState flowState, 
        IContactSetSearchFunc contactSetSearchFunc, 
        ILogger logger,
        CancellationToken token)
        =>
        AsyncPipeline.Pipe(
            token)
        .Pipe(
            _ => new ContactSetSearchIn(
                searchText: string.Empty,
                customerId: flowState.CustomerId,
                top: MaxCustomerSetCount))
        .PipeValue(
            contactSetSearchFunc.InvokeAsync)
        .Fold(
            static @out => new LookupValueSetSeachOut(
                items: @out.Contacts.Select(MapContactItem).ToList().AddPassValue(),
                choiceText: ChooseOrPass),
            failure => MapSearchFailure(failure, logger));

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, LookupValue contactValue)
        => 
        contactValue.Id == PassId ?
        flowState :
        flowState with 
        { 
            ContactId = contactValue.Id, 
            ContactFullName = contactValue.Name
        };

    private static IReadOnlyCollection<LookupValue> AddPassValue(this List<LookupValue> lookupValues)
    {
        if (lookupValues == null)
            return new[] { new LookupValue(PassId, PassButtonText) };

        lookupValues.Add(new(PassId, PassButtonText));
        return lookupValues;
    }

    private static BotFlowFailure MapToFlowFailure(Failure<ContactSetSearchFailureCode> failure)
        =>
        new(
            userMessage: "При выполнении запроса произошла непредвиденная ошибка. Обратитесь к администратору и повторите попытку позднее",
            logMessage: failure.FailureMessage);

    private static LookupValue MapContactItem(ContactItemSearchOut item)
        =>
        new(item.Id, item.FullName);

    private static LookupValueSetSeachOut MapSearchFailure(Failure<ContactSetSearchFailureCode> failure, ILogger logger)
    {
        logger.LogError(failure.FailureMessage, failure.FailureCode);
        return new LookupValueSetSeachOut(
            items: new List<LookupValue>().AddPassValue(),
            choiceText: ChooseOrPass);
    }
}

