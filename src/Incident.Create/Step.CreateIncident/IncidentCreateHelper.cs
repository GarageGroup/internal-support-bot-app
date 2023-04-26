using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class IncidentCreateHelper
{
    internal static ValueTask<ChatFlowJump<IncidentLinkFlowState>> CreateIncidentOrBeakAsync(
        this IIncidentCreateSupplier supportApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static flowState => new IncidentCreateIn(
                ownerId: flowState.OwnerId,
                customerId: flowState.CustomerId,
                contactId: flowState.ContactId,
                title: flowState.Title.OrEmpty(),
                description: flowState.Description,
                caseTypeCode: flowState.CaseTypeCode,
                priorityCode: flowState.PriorityCode))
        .PipeValue(
            supportApi.CreateIncidentAsync)
        .Map(
            incident => new IncidentLinkFlowState
            {
                Title = incident.Title,
                Id = incident.Id
            },
            ToUnexpectedBreakState)
        .Fold(
            ChatFlowJump.Next,
            ChatFlowJump.Break<IncidentLinkFlowState>);

    private static ChatFlowBreakState ToUnexpectedBreakState<TFailureCode>(Failure<TFailureCode> failure)
        where TFailureCode : struct
        =>
        ChatFlowBreakState.From(
            userMessage: "При создании инцидента произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее",
            logMessage: failure.FailureMessage);
}