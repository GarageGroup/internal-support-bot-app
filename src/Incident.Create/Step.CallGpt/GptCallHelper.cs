using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support;

internal static class GptCallHelper
{
    internal static IActivity? CreateTemporaryActivity(IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.IsNotTelegramChannel())
        {
            return null;
        }

        var telegramActivity = context.Activity.CreateReply();

        telegramActivity.ChannelData = new TelegramChannelData(
            parameters: new("ИИ генерирует заголовок...")
            {
                DisableNotification = true
            })
            .ToJObject();

        return telegramActivity;
    }

    internal static IActivity CreateResultActivity(IChatFlowContext<IncidentCreateFlowState> context)
    {
        var activity = InnerCreateActivity(context);
        if (string.IsNullOrEmpty(context.FlowState.Gpt.TemporaryActivityId))
        {
            return activity;
        }

        activity.Id = context.FlowState.Gpt.TemporaryActivityId;
        return activity;

        static IActivity InnerCreateActivity(IChatFlowContext<IncidentCreateFlowState> context)
        {
            if (string.IsNullOrEmpty(context.FlowState.Gpt.Title))
            {
                var errorMessage = context.FlowState.Gpt.ErrorMessage.OrNullIfWhiteSpace() ?? "Нейросеть не смогла подобрать заголовок";
                return MessageFactory.Text(errorMessage);
            }

            if (context.IsNotTelegramChannel())
            {
                return MessageFactory.Text($"Нейросеть предлагает заголовок:\n\r{context.FlowState.Gpt.Title}");
            }

            var telegramActivity = context.Activity.CreateReply();
            var encodedText = HttpUtility.HtmlEncode(context.FlowState.Gpt.Title);

            telegramActivity.ChannelData = new TelegramChannelData(
                parameters: new($"Нейросеть предлагает заголовок:\n\r<code>{encodedText}</code>")
                {
                    ParseMode = TelegramParseMode.Html
                })
                .ToJObject();

            return telegramActivity;
        }
    }

    internal static ValueTask<IncidentCreateFlowState> CompleteIncidentAsync(
        this ISupportGptApi gptApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .HandleCancellation()
        .Pipe(
            static flowState => new IncidentCompleteIn(
                message: flowState.Description.OrEmpty()))
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
                SourceMessage = context.FlowState.Description
            }
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
            ["sourceDescription"] = context.FlowState.Description.OrEmpty()
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