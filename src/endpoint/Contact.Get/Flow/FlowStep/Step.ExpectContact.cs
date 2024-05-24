using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

using static ContactGetResource;

partial class ContactGetFlowStep
{
    internal static ChatFlow<ContactGetFlowState> ExpectContactOrSkip(
        this ChatFlow<ContactGetFlowState> chatFlow, ICrmContactApi crmContactApi)
        =>
        chatFlow.ExpectChoiceValueOrSkip(
            crmContactApi.CreateContactStepOption);

    private static ChoiceStepOption<ContactGetFlowState, ContactState>? CreateContactStepOption(
        this ICrmContactApi crmContactApi, IChatFlowContext<ContactGetFlowState> context)
    {
        if (context.FlowState.Contact is not null)
        {
            return null;
        }

        return new(
            choiceSetFactory: GetContactsAsync,
            resultMessageFactory: CreateResultMessage,
            selectedItemMapper: MapFlowState);

        ValueTask<Result<ChoiceStepSet<ContactState>, ChatRepeatState>> GetContactsAsync(
            ChoiceStepRequest request, CancellationToken cancellationToken)
            =>
            string.IsNullOrEmpty(request.Text) switch
            {
                true => crmContactApi.GetLastContactsAsync(context, cancellationToken),
                _ => crmContactApi.SearchContactsAsync(context, request, cancellationToken)
            };

        ChatMessageSendRequest CreateResultMessage(ChoiceStepItem<ContactState> item)
            =>
            new(context.BuildContactResultMessage(item.Title))
            {
                ReplyMarkup = new BotReplyKeyboardRemove()
            };

        ContactGetFlowState MapFlowState(ChoiceStepItem<ContactState> item)
            =>
            context.FlowState with
            {
                Contact = item.Value
            };
    }

    private static ValueTask<Result<ChoiceStepSet<ContactState>, ChatRepeatState>> GetLastContactsAsync(
        this ICrmContactApi crmContactApi, IChatFlowContext<ContactGetFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static state => new LastContactSetGetIn(
                customerId: state.Customer?.Id ?? default,
                userId: state.BotUserId,
                top: MaxContactSetCount))
        .PipeValue(
            crmContactApi.GetLastAsync)
        .Map(
            success => new ChoiceStepSet<ContactState>(
                choiceText: success.Contacts.IsEmpty ? context.Localizer[NotExistContactsText] : context.Localizer[ChooseOrSearchContactText])
            {
                Items = success.Contacts.Map(context.MapContact).Concat(context.GetSkipButton())
            },
            failure => failure.ToChatRepeatState(context.Localizer[NotExistContactsText]));

    private static ValueTask<Result<ChoiceStepSet<ContactState>, ChatRepeatState>> SearchContactsAsync(
        this ICrmContactApi crmContactApi, IChatFlowContext<ContactGetFlowState> context, ChoiceStepRequest request, CancellationToken token)
        =>
        AsyncPipeline.Pipe(
            context.FlowState.Customer?.Id, token)
        .Pipe(
            customerId => new ContactSetSearchIn(
                searchText: request.Text,
                customerId: customerId.GetValueOrDefault())
            {
                Top = MaxContactSetCount
            })
        .PipeValue(
            crmContactApi.SearchAsync)
        .MapFailure(
            context.MapFailure)
        .MapSuccess(
            @out => new ChoiceStepSet<ContactState>(
                choiceText: @out.Contacts.IsEmpty ? context.Localizer[NotFoundContactsText] : context.Localizer[ChooseOrSearchContactText])
            {
                Items = @out.Contacts.Map(context.MapContact).Concat(context.GetSkipButton())
            });

    private static ChoiceStepItem<ContactState> MapContact(this IChatFlowContextBase context, ContactItemOut item)
        =>
        new(
            id: item.Id.ToString(),
            title: item.FullName,
            value: new(item.Id, item.FullName));

    private static ChoiceStepItem<ContactState> GetSkipButton(this IChatFlowContextBase context)
        =>
        new(
            id: SkipButtonId,
            title: context.Localizer[SkipButtonText],
            value: new(default, default));

    private static ChatRepeatState MapFailure(this IChatFlowContextBase context, Failure<ContactSetGetFailureCode> failure)
        =>
        failure.ToChatRepeatState(
            userMessage: failure.FailureCode switch
            {
                ContactSetGetFailureCode.NotAllowed => context.Localizer[NotAllowedText],
                _ => throw failure.ToException()
            });
}