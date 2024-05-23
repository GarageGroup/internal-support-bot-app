using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class ContactGetCommand
{
    public ValueTask<ChatCommandResult<ContactGetCommandOut>> SendAsync(
        ChatCommandRequest<ContactGetCommandIn, ContactGetCommandOut> request, CancellationToken cancellationToken)
        =>
        request.StartChatFlow(
            @in => new ContactGetFlowState
            {
                DbMinDate = DateTime.UtcNow.AddDays(-option.DbRequestPeriodInDays).Date,
                BotUserId = request.Context.User.Identity?.SystemId ?? default,
                TelegramSenderId = @in.TelegramSender?.Id,
                Customer = @in.Customer is null ? null : new(@in.Customer.Id, @in.Customer.Title),
                Contact = @in.Contact is null ? null : new(@in.Contact.Id, @in.Contact.FullName)
            })
        .FindContactBySenderIdOrSkip(
            crmContactApi)
        .ExpectCustomerOrSkip(
            crmCustomerApi)
        .ExpectContactOrSkip(
            crmContactApi)
        .GetResultAsync(
            static state => new ContactGetCommandOut
            {
                Customer = new(state.Customer?.Id ?? default, state.Customer?.Name),
                Contact = new(state.Contact?.Id, state.Contact?.FullName)
            },
            cancellationToken);
}