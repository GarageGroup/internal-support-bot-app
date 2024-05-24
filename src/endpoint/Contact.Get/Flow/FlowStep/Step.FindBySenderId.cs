using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

partial class ContactGetFlowStep
{
    internal static ChatFlow<ContactGetFlowState> FindContactBySenderIdOrSkip(
        this ChatFlow<ContactGetFlowState> chatFlow, ICrmContactApi crmContactApi)
        =>
        chatFlow.ForwardValue(
            crmContactApi.FindContactBySenderIdOrSkipAsync);

    private static async ValueTask<ChatFlowJump<ContactGetFlowState>> FindContactBySenderIdOrSkipAsync(
        this ICrmContactApi crmContactApi, IChatFlowContext<ContactGetFlowState> context, CancellationToken cancellationToken)
    {
        if (context.FlowState.TelegramSenderId is null)
        {
            return context.FlowState;
        }

        var contactTask = crmContactApi.InnerFindContactBySenderIdAsync(context, cancellationToken);
        var typingTask = context.Api.SendChatActionAsync(BotChatAction.Typing, cancellationToken);

        await Task.WhenAll(contactTask, typingTask).ConfigureAwait(false);
        var contact = contactTask.Result;

        if (contact is null)
        {
            return context.FlowState;
        }

        var customerMessage = context.BuildCustomerResultMessage(contact.CustomerName);
        var contactMessage = context.BuildContactResultMessage(contact.ContactName);

        var resultMessage = string.Format("{0}\n\r{1}", customerMessage, contactMessage);
        _ = await context.Api.SendHtmlModeTextAndRemoveReplyKeyboardAsync(resultMessage, cancellationToken).ConfigureAwait(false);

        return context.FlowState with
        {
            Customer = new(contact.CustomerId, contact.CustomerName),
            Contact = new(contact.ContactId, contact.ContactName)
        };
    }

    private static Task<ContactGetOut?> InnerFindContactBySenderIdAsync(
        this ICrmContactApi crmContactApi, IChatFlowContext<ContactGetFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState.TelegramSenderId, cancellationToken)
        .Pipe(
            static id => new ContactGetIn(id.GetValueOrDefault()))
        .PipeValue(
            crmContactApi.GetAsync)
        .OnFailure(
            context.LogFailure)
        .Fold<ContactGetOut?>(
            static success => success,
            _ => null);

    private static void LogFailure(this IChatFlowContextBase context, Failure<ContactGetFailureCode> failure)
    {
        if (failure.FailureCode is ContactGetFailureCode.NotFound)
        {
            return;
        }

        context.Logger.LogError(failure.SourceException, "FindContactBySenderId failure: {message}", failure.FailureMessage);
    }
}