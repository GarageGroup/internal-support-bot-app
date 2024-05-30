using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

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

        if (string.IsNullOrWhiteSpace(context.FlowState.Description) && context.FlowState.PhotoUrls.IsEmpty)
        {
            return context.FlowState;
        }

        var gptTask = gptApi.InnerCompleteIncidentAsync(context, cancellationToken);
        var messageTask = context.SendTemporaryMessageAsync(context.Localizer[GptTempMessage], cancellationToken);

        await Task.WhenAll(gptTask, messageTask).ConfigureAwait(false);

        await context.Api.DeleteMessageAsync(messageTask.Result.MessageId, cancellationToken).ConfigureAwait(false);

        return gptTask.Result;
    }

    private static Task<IncidentCreateFlowState> InnerCompleteIncidentAsync(
        this ISupportGptApi gptApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static flowState => new IncidentCompleteIn(
                message: flowState.Description,
                imageUrls: flowState.PhotoUrls))
        .PipeValue(
            gptApi.CompleteIncidentAsync)
        .OnFailure(
            (_, cancellationToken) => context.Api.SendHtmlModeTextAndRemoveReplyKeyboardAsync(
                context.Localizer[GptErrorMessage], cancellationToken))
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