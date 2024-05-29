using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ExpectContact(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.ForwardValue(
            ExpectContactAsync);

    private static async ValueTask<ChatFlowJump<IncidentCreateFlowState>> ExpectContactAsync(
        IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        var stepState = context.StepState.Get<ContactGetStepState>();
        if (stepState is null)
        {
            stepState = new()
            {
                MessageStatus = GetMessageStatus(context.Update.Message),
                Description = context.FlowState.Description,
                DocumentIds = context.FlowState.DocumentIds
            };

            context.StepState.Set(stepState);
        }
        else
        {
            stepState = UpdateStepState(stepState, context.Update.Message);
            context.StepState.Set(stepState);

            if (stepState.MessageStatus is not MessageStatus.Collected)
            {
                return default;
            }
        }

        var @in = context.CreateContactGetInput();
        var result = await context.Command.RunAsync<ContactGetCommandIn, ContactGetCommandOut>(@in, cancellationToken).ConfigureAwait(false);

        if (result.State is TurnState.Complete)
        {
            var contact = result.CompleteValueOrThrow();
            return context.FlowState with
            {
                Customer = new(contact.Customer.Id, contact.Customer.Title),
                Contact = new(contact.Contact?.Id, contact.Contact?.FullName),
                Description = stepState.Description,
                DocumentIds = stepState.DocumentIds
            };
        }

        if (result.State is TurnState.Cancelled)
        {
            return default(ChatBreakState);
        }

        return default;
    }

    private static ContactGetCommandIn CreateContactGetInput(this IChatFlowContext<IncidentCreateFlowState> context)
    {
        var state = context.FlowState;
        if (state.IsRepeated is false)
        {
            return new(
                telegramSender: state.SourceSender is null ? null : new(state.SourceSender.UserId));
        }

        return new(
            customer: state.Customer is null ? null : new(state.Customer.Id, state.Customer.Title),
            contact: state.Contact is null ? null : new(state.Contact.Id, state.Contact.FullName));
    }

    private static MessageStatus GetMessageStatus(BotMessage? message)
    {
        if (message?.ForwardOrigin is not null)
        {
            return MessageStatus.Forwarding;
        }

        if (string.IsNullOrWhiteSpace(message?.MediaGroupId) is false)
        {
            return MessageStatus.MediaGroup;
        }

        return MessageStatus.Collected;
    }

    private static ContactGetStepState UpdateStepState(ContactGetStepState state, BotMessage? message)
    {
        if (state.MessageStatus is MessageStatus.Collected)
        {
            return state;
        }

        if (message is null || IsCollected(state, message))
        {
            return state with
            {
                MessageStatus = MessageStatus.Collected
            };
        }

        var description = message.GetDescription();
        var documentIds = message.GetDocumentIds();

        return state with
        {
            Description = JoinDescriptions(state.Description, description),
            DocumentIds = state.DocumentIds.Concat(documentIds)
        };

        static bool IsCollected(ContactGetStepState state, BotMessage message)
            =>
            (state.MessageStatus is MessageStatus.Forwarding && message.ForwardOrigin is null) ||
            (state.MessageStatus is MessageStatus.MediaGroup && string.IsNullOrWhiteSpace(message.MediaGroupId));

        static string? JoinDescriptions(string? current, string? additional)
        {
            if (string.IsNullOrEmpty(current))
            {
                return additional;
            }

            if (string.IsNullOrEmpty(additional))
            {
                return current;
            }

            return string.Format("{0}\n\r{1}", current, additional);
        }
    }

    private sealed record class ContactGetStepState
    {
        public MessageStatus MessageStatus { get; init; }

        public string? Description { get; init; }

        public FlatArray<string> DocumentIds { get; init; }
    }

    private enum MessageStatus
    {
        Collected,

        Forwarding,

        MediaGroup
    }
}