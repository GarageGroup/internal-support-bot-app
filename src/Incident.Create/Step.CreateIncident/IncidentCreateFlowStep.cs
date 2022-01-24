using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

internal static class IncidentCreateFlowStep
{
    internal static ChatFlow<Unit> CreateIncident(
        this ChatFlow<IncidentCreateFlowState> chatFlow,
        IBotUserProvider botUserProvider,
        IIncidentCreateFunc incidentCreateFunc,
        IncidentCreateBotOption option)
        =>
        chatFlow.ForwardValue(
            (context, cancellationToken) => context.CreateIncidentAsync(
                botUserProvider: botUserProvider,
                incidentCreateFunc: incidentCreateFunc,
                option: option,
                cancellationToken: cancellationToken))
        .SendActivity(
            IncidentCreateActivity.CreateSuccessActivity)
        .MapFlowState(
            Unit.From);

    private static ValueTask<ChatFlowJump<IncidentLinkFlowState>> CreateIncidentAsync(
        this IChatFlowContext<IncidentCreateFlowState> context,
        IBotUserProvider botUserProvider,
        IIncidentCreateFunc incidentCreateFunc,
        IncidentCreateBotOption option,
        CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .PipeValue(
            botUserProvider.GetOwnerIdAsync)
        .Pipe(
            flowState => new IncidentCreateIn(
                ownerId: flowState.OwnerId,
                customerId: flowState.CustomerId,
                title: flowState.Title.OrEmpty(),
                description: flowState.Description,
                caseTypeCode: flowState.CaseTypeCode,
                contactId: flowState.ContactId == Guid.Empty ? null : flowState.ContactId,
                caseOriginCode: option.CaseOriginCode))
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

    private static async ValueTask<IncidentCreateFlowState> GetOwnerIdAsync(
        this IBotUserProvider botUserProvider, IncidentCreateFlowState flowState, CancellationToken cancellationToken)
    {
        var currentUser = await botUserProvider.GetCurrentUserAsync(cancellationToken);
        if (currentUser is null)
        {
            return flowState;
        }

        return flowState with
        {
            OwnerId = currentUser.Claims.GetValueOrAbsent("DataverseSystemUserId").FlatMap(ParseOrAbsent).OrDefault()
        };

        static Optional<Guid> ParseOrAbsent(string value)
            =>
            Guid.TryParse(value, out var guid) ? Optional.Present(guid) : default;
    }

    private static ChatFlowBreakState ToUnexpectedBreakState<TFailureCode>(Failure<TFailureCode> failure)
        where TFailureCode : struct
        =>
        ChatFlowBreakState.From(
            userMessage: "При создании инцидента произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее",
            logMessage: failure.FailureMessage);
}