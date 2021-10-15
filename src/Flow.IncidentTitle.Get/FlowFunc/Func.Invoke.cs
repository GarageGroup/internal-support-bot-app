using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot;

partial class IncidentTitleGetFlowFunc
{
    public ValueTask<ChatFlowStepResult<IncidentTitleGetFlowOut>> InvokeAsync(
        DialogContext dialogContext, IncidentTitleGetFlowIn input, CancellationToken cancellationToken = default)
        =>
        ChatFlow.Start(
            dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)),
            input ?? throw new ArgumentNullException(nameof(input)))
        .MapFlowState(
            @in => CreareDefaultTitleFromDescription(@in.Description))
        .SendActivity(
            dialogContext.Context.Activity.CreateTitleHintActivity)
        .Await()
        .ForwardValue(
            CheckTitleResponseAsync)
        .CompleteValueAsync(
            cancellationToken);

    private static string CreareDefaultTitleFromDescription(string description)
        =>
        description.Trim(' ') switch
        {
            { Length: <= DefaultTitleLength } => description,
            _ => description[..DefaultTitleLength] + "..."
        };

    private static ValueTask<ChatFlowStepResult<IncidentTitleGetFlowOut>> CheckTitleResponseAsync(
        DialogContext dialogContext, string defaultTitle, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            dialogContext.Context.Activity.GetTitleValue(),
            cancellationToken)
        .Pipe(
            activityText => activityText switch
            {
                { Length: 0 } => Failure.Create("Название не указано. Повторите попытку"),
                _ => Result.Success(activityText).With<Failure<Unit>>()
            })
        .MapFailure(
            async (failure, token) =>
            {
                var failureActivity = MessageFactory.Text(failure.FailureMessage);
                await dialogContext.Context.SendActivityAsync(failureActivity, token).ConfigureAwait(false);

                return default(Unit);
            })
        .Fold(
            title => new IncidentTitleGetFlowOut(title: title),
            ChatFlowStepResult.RetryAndAwait<IncidentTitleGetFlowOut>);
}
