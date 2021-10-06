using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support.Bot
{
    partial class IncidentCreateFlowFunc
    {
        public ValueTask<ChatFlowStepResult<Unit>> InvokeAsync(
            DialogContext dialogContext, IncidentCreateFlowIn input, CancellationToken cancellationToken = default)
            =>
            ChatFlow.Start(
                dialogContext ?? throw new ArgumentNullException(nameof(dialogContext)),
                input ?? throw new ArgumentNullException(nameof(input)))
            .SendActivity(
                @in => MessageFactory.Text($"Создать инцидент '{@in.ToString()}'?"))
            .SendActivity(
                _ => MessageFactory.Text($"'{Yes}' для подтверждения, любой другой ответ - отказ"))
            .Await()
            .ForwardValue(
                CheckResponseAsync)
            .MapFlowState(
                @in => new IncidentCreateIn(
                    ownerId: @in.OwnerId,
                    customerId: @in.CustomerId,
                    title: @in.Title,
                    description: @in.Description))
            .ForwardValue(
                CreateIncidentAsync)
            .CompleteValueAsync(
                cancellationToken);

        private ValueTask<ChatFlowStepResult<IncidentCreateFlowIn>> CheckResponseAsync(
            DialogContext dialogContext, IncidentCreateFlowIn input, CancellationToken cancellationToken)
            =>
            AsyncPipeline.Start(
                dialogContext.Context.Activity.Text ?? string.Empty,
                cancellationToken)
            .Pipe(
                activityText => IsYes(activityText) switch
                {
                    true    => Result.Present(input),
                    _       => default
                })
            .MapFailure(
                async (failure, token) =>
                {
                    var failureActivity = MessageFactory.Text("Создание инцидента отменено");
                    await dialogContext.Context.SendActivityAsync(failureActivity, token).ConfigureAwait(false);

                    return failure;
                })
            .Fold(
                ChatFlowStepResult.Next,
                _ => ChatFlowStepResult.Interrupt());

        private ValueTask<ChatFlowStepResult<Unit>> CreateIncidentAsync(
            DialogContext dialogContext, IncidentCreateIn incident, CancellationToken cancellationToken)
            =>
            AsyncPipeline.Start(
                incident, cancellationToken)
            .PipeValue(
                incidentCreateFunc.InvokeAsync)
            .Fold<ChatFlowStepResult<Unit>>(
                async (createdIncident, token) =>
                {
                    var failureActivity = MessageFactory.Text("Создание инцидента отменено");
                    await dialogContext.Context.SendActivityAsync(failureActivity, token).ConfigureAwait(false);

                    return default(Unit);
                },
                async (failure, token) =>
                {
                    logger.LogError(failure.FailureMessage);

                    var failureActivity = MessageFactory.Text(UnexpectedIncidentCreateFailureMessage);
                    await dialogContext.Context.SendActivityAsync(failureActivity, token).ConfigureAwait(false);

                    return ChatFlowStepResult.Interrupt();
                });

        private static bool IsYes(string text)
            =>
            string.Equals(text, Yes, StringComparison.InvariantCultureIgnoreCase);

        private const string UnexpectedIncidentCreateFailureMessage =
            "Не удалось создать инцидент. Возможно сервис не доступен. Обратитесь к администратору или повторите попытку позже";
    }
}