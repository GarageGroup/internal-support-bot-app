using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class BotUserGetHelper
{
    internal static ValueTask<ChatFlowJump<DataverseUserValue>> GetBotUserOrBreakAsync(
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
            GetDataverseUserValueOrFailure)
        .MapFailure(
            ToBreakState)
        .Fold(
            ChatFlowJump.Next,
            ChatFlowJump.Break<DataverseUserValue>);

    private static Result<DataverseUserValue, Failure<Unit>> GetDataverseUserValueOrFailure(BotUser botUser)
    {
        return botUser.Claims.AsEnumerable()
            .GetValueOrAbsent("DataverseSystemUserId")
            .Fold(
                ParseOrFailure,
                CreateUserIdClaimMustBeSpecifiedFailure)
            .MapSuccess(
                CreateValue);
            

        static Result<Guid, Failure<Unit>> ParseOrFailure(string value)
            =>
            Guid.TryParse(value, out var guid) ? guid : Failure.Create($"DataverseUserId Claim {value} is not a Guid");

        static Result<Guid, Failure<Unit>> CreateUserIdClaimMustBeSpecifiedFailure()
            =>
            Failure.Create("Dataverse user claim must be specified");

        DataverseUserValue CreateValue(Guid userId)
        {
            var dataverseUserFullName = botUser.Claims.AsEnumerable().GetValueOrAbsent("DataverseSystemUserFullName").OrDefault();
            if (string.IsNullOrEmpty(dataverseUserFullName) is false)
            {
                return new()
                {
                    Id = userId,
                    Name = dataverseUserFullName
                };
            }

            if (string.IsNullOrEmpty(botUser.DisplayName) is false)
            {
                return new()
                {
                    Id = userId,
                    Name = botUser.DisplayName
                };
            }

            return new()
            {
                Id = userId
            };
        }
    }

    private static ChatFlowBreakState ToBreakState(Failure<Unit> failure)
        =>
        ChatFlowBreakState.From(
            userMessage: "Произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее",
            logMessage: failure.FailureMessage);
}