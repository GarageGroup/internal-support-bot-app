using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class TitleGetFlowStep
{
    private const int DefaultTitleLength = 30;

    internal static ChatFlow<IncidentCreateFlowState> GetTitle(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.ForwardValue(
            GetTitleOrBreakAsync,
            (flowState, title) => flowState with
            {
                Title = title
            });

    private static async ValueTask<ChatFlowJump<string>> GetTitleOrBreakAsync(
        this IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        if (context.StepState is not TitleGetFlowStepState stepState)
        {
            var state = new TitleGetFlowStepState
            {
                ButtonId = Guid.NewGuid(),
                OfferedTitle = CreareDefaultTitleFromDescription(context.FlowState.Description.OrEmpty())
            };
            var offeredTitleActivity = context.CreateTitleHintActivity(state);

            await context.SendActivityAsync(offeredTitleActivity, cancellationToken);
            return ChatFlowJump.Repeat<string>(state);
        }

        var titleResult = context.GetTitleValueOrFailure(stepState);
        if (titleResult.IsFailure)
        {
            return context.RepeatSameStateJump<string>();
        }

        var title = titleResult.SuccessOrThrow();
        if (string.IsNullOrEmpty(title))
        {
            var retryActivity = TitleGetActivity.CreateTitleMustBeSpecifiedActivity();
            await context.SendActivityAsync(retryActivity, cancellationToken);

            return context.RepeatSameStateJump<string>();
        }

        return title;
    }

    private static string CreareDefaultTitleFromDescription(string description)
        =>
        description.Trim(' ') switch
        {
            { Length: <= DefaultTitleLength } => description,
            _ => description[..DefaultTitleLength] + "..."
        };
}