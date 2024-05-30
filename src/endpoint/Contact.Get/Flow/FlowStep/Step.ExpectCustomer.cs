using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

using static ContactGetResource;

partial class ContactGetFlowStep
{
    internal static ChatFlow<ContactGetFlowState> ExpectCustomerOrSkip(
        this ChatFlow<ContactGetFlowState> chatFlow, ICrmCustomerApi crmCustomerApi)
        =>
        chatFlow.ExpectChoiceValueOrSkip(
            crmCustomerApi.CreateCustomerStepOption);

    private static ChoiceStepOption<ContactGetFlowState, CustomerState>? CreateCustomerStepOption(
        this ICrmCustomerApi crmCustomerApi, IChatFlowContext<ContactGetFlowState> context)
    {
        if (context.FlowState.Customer is not null)
        {
            return null;
        }

        return new(
            choiceSetFactory: GetCustomersAsync,
            resultMessageFactory: CreateResultMessage,
            selectedItemMapper: MapFlowState);

        ValueTask<Result<ChoiceStepSet<CustomerState>, ChatRepeatState>> GetCustomersAsync(
            ChoiceStepRequest request, CancellationToken cancellationToken)
            =>
            string.IsNullOrEmpty(request.Text) switch
            {
                true => crmCustomerApi.GetLastCustomersAsync(context, cancellationToken),
                _ => crmCustomerApi.SearchCustomersAsync(context, request, cancellationToken)
            };

        ChatMessageSendRequest CreateResultMessage(ChoiceStepItem<CustomerState> item)
            =>
            new(context.BuildCustomerResultMessage(item.Title))
            {
                ReplyMarkup = new BotReplyKeyboardRemove()
            };

        ContactGetFlowState MapFlowState(ChoiceStepItem<CustomerState> item)
            =>
            context.FlowState with
            {
                Customer = item.Value
            };
    }

    private static ValueTask<Result<ChoiceStepSet<CustomerState>, ChatRepeatState>> GetLastCustomersAsync(
        this ICrmCustomerApi crmCustomerApi, IChatFlowContext<ContactGetFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static state => new LastCustomerSetGetIn(
                userId: state.BotUserId,
                minCreationTime: state.DbMinDate,
                top: MaxCustomerSetCount))
        .PipeValue(
            crmCustomerApi.GetLastAsync)
        .Map(
            success => new ChoiceStepSet<CustomerState>(
                choiceText: success.Customers.IsEmpty ? context.Localizer[SearchCustomerText] : context.Localizer[ChooseOrSearchCustomerText])
            {
                Items = success.Customers.Map(context.MapCustomer)
            },
            failure => failure.ToChatRepeatState(context.Localizer[SearchCustomerText]));

    private static ValueTask<Result<ChoiceStepSet<CustomerState>, ChatRepeatState>> SearchCustomersAsync(
        this ICrmCustomerApi crmCustomerApi, IChatFlowContextBase context, ChoiceStepRequest request, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            request.Text, cancellationToken)
        .Pipe(
            static text => new CustomerSetSearchIn(text)
            {
                Top = MaxCustomerSetCount
            })
        .PipeValue(
            crmCustomerApi.SearchAsync)
        .MapFailure(
            context.MapFailure)
        .MapSuccess(
            @out => new ChoiceStepSet<CustomerState>(
                choiceText: @out.Customers.IsEmpty ? context.Localizer[NotFoundCustomersText] : context.Localizer[ChooseOrSearchCustomerText])
            {
                Items = @out.Customers.Map(context.MapCustomer)
            });

    private static ChoiceStepItem<CustomerState> MapCustomer(this IChatFlowContextBase context, CustomerItemOut item)
        =>
        new(
            id: item.Id.ToString(),
            title: item.Title,
            value: new(item.Id, item.Title));

    private static string BuildCustomerResultMessage(this IChatFlowContextBase context, string customerTitle)
        =>
        string.Format("{0}: <b>{1}</b>", context.Localizer[CustomerFieldName], HttpUtility.HtmlEncode(customerTitle));

    private static ChatRepeatState MapFailure(this IChatFlowContextBase context, Failure<CustomerSetGetFailureCode> failure)
        =>
        failure.ToChatRepeatState(
            userMessage: failure.FailureCode switch
            {
                CustomerSetGetFailureCode.NotAllowed => context.Localizer[NotAllowedText],
                _ => throw failure.ToException()
            });
}