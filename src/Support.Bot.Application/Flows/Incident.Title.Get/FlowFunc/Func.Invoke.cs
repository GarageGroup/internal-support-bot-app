using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GGroupp.Internal.Support.Bot
{
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
                title => MessageFactory.Text(
                    $"Теперь нужно указать название.\nПредлагаю: '{title}'.\n'{Yes}' для согласия или свое название."))
            .Await()
            .ForwardValue(
                CheckTitleResponseAsync)
            .CompleteValueAsync(
                cancellationToken);

        private static string CreareDefaultTitleFromDescription(string description)
            =>
            description switch
            {
                { Length: <= DefaultTitleLength } => description,
                _ => description.Substring(0, DefaultTitleLength) + "..."
            };

        private static ValueTask<ChatFlowStepResult<IncidentTitleGetFlowOut>> CheckTitleResponseAsync(
            DialogContext dialogContext, string defaultTitle, CancellationToken cancellationToken)
            =>
            AsyncPipeline.Start(
                dialogContext.Context.Activity.Text ?? string.Empty,
                cancellationToken)
            .Pipe(
                activityText => activityText switch
                {
                    { Length: 0 }               => Failure.Create("Название не указано. Повторите попытку"),
                    _ when IsYes(activityText)  => Result.Success(defaultTitle).With<Failure<Unit>>(),
                    _                           => Result.Success(activityText).With<Failure<Unit>>()
                })
            .MapFailure(
                async (failure, token) =>
                {
                    var failureActivity = MessageFactory.Text(failure.FailureMessage);
                    await dialogContext.Context.SendActivityAsync(failureActivity, token).ConfigureAwait(false);

                    return default(Unit);
                })
            .Fold<ChatFlowStepResult<IncidentTitleGetFlowOut>>(
                title => new IncidentTitleGetFlowOut(title: title),
                _ => ChatFlowStepResult.RetryAndAwait());

        private static bool IsYes(string text)
            =>
            string.Equals(text, Yes, StringComparison.InvariantCultureIgnoreCase);
    }
}