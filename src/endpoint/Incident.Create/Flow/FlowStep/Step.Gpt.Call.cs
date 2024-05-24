using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> CallGpt(
        this ChatFlow<IncidentCreateFlowState> chatFlow, ISupportGptApi gptApi)
        =>
        chatFlow.NextValue(
            gptApi.CompleteIncidentAsync);

    private static async ValueTask<IncidentCreateFlowState> CompleteIncidentAsync(
        this ISupportGptApi gptApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(context.FlowState.Title) is false)
        {
            return context.FlowState;
        }

        // to do: remove before release
        context.Logger.LogInformation("SenderId: {senderId}", context.FlowState.SourceSender?.UserId);
        context.Logger.LogInformation("PhotoIds: {photoIds}", string.Join(',', context.FlowState.PhotoIdSet.AsEnumerable()));

        var gptTask = gptApi.InnerCompleteIncidentAsync(context, cancellationToken);
        var typingTask = context.Api.SendChatActionAsync(BotChatAction.Typing, cancellationToken);

        await Task.WhenAll(gptTask, typingTask).ConfigureAwait(false);

        return gptTask.Result;
    }

    private static Task<IncidentCreateFlowState> InnerCompleteIncidentAsync(
        this ISupportGptApi gptApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static flowState => new IncidentCompleteIn(
                message: flowState.Description.OrEmpty()))
        .PipeValue(
            gptApi.CompleteIncidentAsync)
        .Fold(
            context.ApplyGptValue,
            context.LogGptFailure);

    private static IncidentCreateFlowState ApplyGptValue(
        this IChatFlowContext<IncidentCreateFlowState> context, IncidentCompleteOut @out)
    {
        if (string.IsNullOrEmpty(@out.Title))
        {
            return context.FlowState;
        }

        return context.FlowState with
        {
            Gpt = context.FlowState.Gpt with
            {
                Title = @out.Title,
                CaseTypeCode = @out.CaseTypeCode,
                SourceMessage = context.FlowState.Description
            },
            CaseType = new(
                code: @out.CaseTypeCode,
                title: context.GetDisplayName(@out.CaseTypeCode))
        };
    }

    private static IncidentCreateFlowState LogGptFailure(
        this IChatFlowContext<IncidentCreateFlowState> context, Failure<IncidentCompleteFailureCode> failure)
    {
        context.Logger.LogWarning(
            "GptFailure: {gptFailure}. Source description: '{sourceDescription}'", failure.FailureMessage, context.FlowState.Description);

        return context.FlowState with
        {
            Gpt = context.FlowState.Gpt with
            {
                Title = null
            }
        };
    }
}