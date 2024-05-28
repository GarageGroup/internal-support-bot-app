using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Logging;

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
                SenderTelegramId = flowState.SourceSender?.UserId,
                Documents = flowState.Documents.Map(MapDocument)
            })
        .PipeValue(
            crmIncidentApi.CreateAsync)
        .OnSuccess(
            context.LogAnnotationFailures)
        .MapSuccess(
            incident => context.FlowState with
            {
                Title = incident.Title,
                IncidentId = incident.Id,
                AnnotationFailureFileNames = incident.Failures.Map(GetFileName)
            })
        .MapSuccess(
            ChatFlowJump.Next)
        .SuccessOrThrow(
            static failure => failure.ToException());

    private static void LogAnnotationFailures(this IChatFlowContextBase context, IncidentCreateOut incidentCreateOut)
    {
        if (incidentCreateOut.Failures.IsEmpty)
        {
            return;
        }

        foreach (var failure in incidentCreateOut.Failures)
        {
            context.Logger.LogError(
                failure.SourceException, 
                "Annotation error. FileName: {fileName}. Message: {failureMessage}", failure.FileName, failure.FailureMessage);
        }
    }

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

    private static DocumentModel MapDocument(DocumentState document)
        =>
        new(fileName: document.FileName, url: document.Url)
        {
            Type = document.Type
        };

    private static AnnotationFailureState GetFileName(AnnotationCreateFailure failure)
        =>
        new(failure.FileName)
        {
            FailureCode = failure.FailureCode
        };
}