using GGroupp.Infra.Bot.Builder;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

internal static class OwnerGetHelper
{
    internal static ValueTask<ChatFlowJump<LookupValue>> GetOwnerValueOrBreakAsync(
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
            GetLookupValueOrFailure)
        .MapFailure(
            ToBreakState)
        .Fold(
            ChatFlowJump.Next,
            ChatFlowJump.Break<LookupValue>);

    private static Result<LookupValue, Failure<Unit>> GetLookupValueOrFailure(BotUser botUser)
    {
        return botUser.Claims
            .GetValueOrAbsent("DataverseSystemUserId")
            .Fold(
                ParseOrFailure,
                CreateUserIdClaimMustBeSpecifiedFailure)
            .MapSuccess(
                CreateLookupValue);
            

        static Result<Guid, Failure<Unit>> ParseOrFailure(string value)
            =>
            Guid.TryParse(value, out var guid) ? guid : Failure.Create($"DataverseUserId Claim {value} is not a Guid");

        static Result<Guid, Failure<Unit>> CreateUserIdClaimMustBeSpecifiedFailure()
            =>
            Failure.Create("Dataverse user claim must be specified");

        LookupValue CreateLookupValue(Guid userId)
        {
            var dataverseUserFullName = botUser.Claims.GetValueOrAbsent("DataverseSystemUserFullName").OrDefault();
            if (string.IsNullOrEmpty(dataverseUserFullName) is false)
            {
                return new(userId, dataverseUserFullName);
            }

            if (string.IsNullOrEmpty(botUser.DisplayName) is false)
            {
                return new(userId, botUser.DisplayName);
            }

            return new(userId, "Я (по умолчанию)");
        }
    }

    private static ChatFlowBreakState ToBreakState(Failure<Unit> failure)
        =>
        ChatFlowBreakState.From(
            userMessage: "Произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее",
            logMessage: failure.FailureMessage);
}