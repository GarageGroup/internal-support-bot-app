using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Internal.Support;

internal static class GptCallHelper
{
    internal static ValueTask<IncidentCreateFlowState> CompleteIncidentAsync(
        this ISupportGptApi gptApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(context.FlowState.Title) is false)
        {
            return new(context.FlowState);
        }

        return gptApi.InnerCompleteIncidentAsync(context, cancellationToken);
    }

    private static ValueTask<IncidentCreateFlowState> InnerCompleteIncidentAsync(
        this ISupportGptApi gptApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .HandleCancellation()
        .Pipe(
            static flowState => new IncidentCompleteIn(
                message: (flowState.Description?.Value).OrEmpty()))
        .PipeValue(
            gptApi.CompleteIncidentAsync)
        .Fold(
            context.ApplyGptValue,
            context.LogGptFailure);

    private static IncidentCreateFlowState ApplyGptValue(this IChatFlowContext<IncidentCreateFlowState> context, IncidentCompleteOut @out)
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
                SourceMessage = context.FlowState.Description?.Value
            },
            CaseTypeCode = @out.CaseTypeCode
        };
    }

    private static IncidentCreateFlowState LogGptFailure(
        this IChatFlowContext<IncidentCreateFlowState> context, Failure<IncidentCompleteFailureCode> failure)
    {
        context.Logger.LogWarning("GptFailure: {gptFailure}", failure.FailureMessage);

        context.BotTelemetryClient.TrackEvent("CompleteIncidentGptFailure", new Dictionary<string, string>
        {
            ["flowId"] = context.ChatFlowId,
            ["event"] = "GptFailure",
            ["message"] = failure.FailureMessage,
            ["sourceDescription"] = (context.FlowState.Description?.Value).OrEmpty()
        });

        return context.FlowState with
        {
            Gpt = context.FlowState.Gpt with
            {
                Title = null,
                ErrorMessage = failure.FailureCode switch
                {
                    IncidentCompleteFailureCode.TooManyRequests => "Нейросеть в настоящий момент перегружена",
                    _ => null
                }
            }
        };
    }
}