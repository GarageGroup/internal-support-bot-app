using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> CreateIncident(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ICrmIncidentApi crmIncidentApi)
        =>
        chatFlow.SendChatAction(
            BotChatAction.Typing, GetTemporaryMessage, SaveTemporaryMessageId)
        .ForwardValue(
            crmIncidentApi.CreateIncidentOrThrowAsync);

    private static ValueTask<ChatFlowJump<IncidentCreateFlowState>> CreateIncidentOrThrowAsync(
        this ICrmIncidentApi crmIncidentApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static flowState => new IncidentCreateIn(
                ownerId: flowState.Owner?.Id ?? default,
                customerId: flowState.Customer?.Id ?? default,
                contactId: flowState.Contact?.Id,
                title: flowState.Title.OrEmpty(),
                description: flowState.Description,
                caseTypeCode: flowState.CaseType?.Code ?? default,
                priorityCode: flowState.Priority?.Code ?? default,
                callerUserId: flowState.BotUserId)
            {
                SenderTelegramId = flowState.SourceSender?.UserId
            })
        .PipeValue(
            crmIncidentApi.CreateAsync)
        .MapSuccess(
            incident => context.FlowState with
            {
                Title = incident.Title,
                IncidentId = incident.Id,
            })
        .MapSuccess(
            ChatFlowJump.Next)
        .SuccessOrThrow(
            static failure => failure.ToException());

    private static ChatMessageSendRequest? GetTemporaryMessage(IChatFlowContextBase context)
        =>
        new(context.Localizer[IncidentCreationTemporaryText])
        {
            DisableNotification = true,
            ReplyMarkup = new BotReplyKeyboardRemove()
        };

    private static IncidentCreateFlowState SaveTemporaryMessageId(
        IChatFlowContext<IncidentCreateFlowState> context, BotMessage message)
        =>
        context.FlowState with
        {
            TemporaryMessageId = message.MessageId
        };
}