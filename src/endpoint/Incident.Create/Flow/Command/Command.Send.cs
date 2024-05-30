using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateCommand
{
    public ValueTask<ChatCommandResult<Unit>> SendAsync(
        ChatCommandRequest<IncidentCreateCommandIn, Unit> request, CancellationToken cancellationToken)
        =>
        request.StartChatFlow(
            @in => new IncidentCreateFlowState
            {
                BotUserId = request.Context.User.Identity?.SystemId ?? default,
                BotUserName = request.Context.User.Identity?.Name,
                Description = @in.Description,
                DocumentIds = @in.DocumentIds,
                SourceSender = @in.SourceSender is null ? null : new(@in.SourceSender.Id)
            })
        .ExpectContact()
        .GetDocumentUrls()
        .CallGpt(
            gptApi)
        .ExpectTitleOrSkip()
        .ExpectCaseTypeOrSkip()
        .ExpectPriorityOrSkip()
        .ExpectOwnerOrSkip(
            ownerApi)
        .ExpectIncidentConfirmationOrSkip()
        .CreateIncident(
            incidentApi)
        .LogGptOrSkip(
            telemetryClient, option)
        .ShowIncident(
            option)
        .GetResultAsync(
            Unit.From, cancellationToken);
}