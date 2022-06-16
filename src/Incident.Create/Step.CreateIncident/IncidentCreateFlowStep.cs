using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

internal static class IncidentCreateFlowStep
{
    internal static ChatFlow<Unit> CreateIncident(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IIncidentCreateFunc incidentCreateFunc, IncidentCreateBotOption option)
        =>
        chatFlow.ForwardValue(
            (context, cancellationToken) => context.CreateIncidentAsync(
                incidentCreateFunc: incidentCreateFunc,
                option: option,
                cancellationToken: cancellationToken))
        .SendActivity(
            IncidentCreateActivity.CreateSuccessActivity)
        .MapFlowState(
            Unit.From);

    private static ValueTask<ChatFlowJump<IncidentLinkFlowState>> CreateIncidentAsync(
        this IChatFlowContext<IncidentCreateFlowState> context,
        IIncidentCreateFunc incidentCreateFunc,
        IncidentCreateBotOption option,
        CancellationToken cancellationToken)
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
            incidentCreateFunc.InvokeAsync)
        .Map(
            incident => new IncidentLinkFlowState
            {
                Title = incident.Title,
                Url = string.Format(CultureInfo.InvariantCulture, option.IncidentCardUrlTemplate, incident.Id)
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