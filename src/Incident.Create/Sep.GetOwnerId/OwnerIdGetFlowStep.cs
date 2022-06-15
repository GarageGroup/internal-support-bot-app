using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class OwnerIdGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetOwnerId(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.ForwardValue(
            GetOwnerIdOrBreakAsync,
            static (flowState, ownerId) => flowState with
            {
                OwnerId = ownerId
            });

    private static ValueTask<ChatFlowJump<Guid>> GetOwnerIdOrBreakAsync(
        IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            default(Unit), cancellationToken)
        .HandleCancellation()
        .PipeValue(
            context.BotUserProvider.InvokeAsync)
        .MapFailure(
            static _ => Failure.Create("Bot user must be specified"))
        .Forward(
            static user => user.Claims.GetValueOrAbsent("DataverseSystemUserId").Fold(ParseOrFailure, CreateClaimMustBeSpecifiedFailure))
        .MapFailure(
            ToBreakState)
        .Fold(
            ChatFlowJump.Next,
            ChatFlowJump.Break<Guid>);

    private static Result<Guid, Failure<Unit>> ParseOrFailure(string value)
        =>
        Guid.TryParse(value, out var guid) ? guid : Failure.Create($"DataverseUserId Claim {value} is not a Guid");

    private static Result<Guid, Failure<Unit>> CreateClaimMustBeSpecifiedFailure()
        =>
        Failure.Create("Dataverse user claim must be specified");

    private static ChatFlowBreakState ToBreakState(Failure<Unit> failure)
        =>
        ChatFlowBreakState.From(
            userMessage: "Произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее",
            logMessage: failure.FailureMessage);
}