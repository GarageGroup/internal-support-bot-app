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
    private const string TemporaryText = "Обращение к ИИ для создания заголовка...";

    internal static IActivity CreateTemporaryActivity(IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.IsNotTelegramChannel())
        {
            return MessageFactory.Text(TemporaryText);
        }

        var telegramActivity = context.Activity.CreateReply();

        telegramActivity.ChannelData = new TelegramChannelData(
            parameters: new TelegramParameters(TemporaryText)
            {
                DisableNotification = true
            })
            .ToJObject();

        return telegramActivity;
    }

    internal static IActivity CreateResultActivity(IChatFlowContext<IncidentCreateFlowState> context)
    {
        var activity = InnerCreateActivity(context);
        if (string.IsNullOrEmpty(context.FlowState.Gpt?.TemporaryActivityId))
        {
            return activity;
        }

        activity.Id = context.FlowState.Gpt.TemporaryActivityId;
        return activity;

        static IActivity InnerCreateActivity(IChatFlowContext<IncidentCreateFlowState> context)
        {
            if (string.IsNullOrEmpty(context.FlowState.Gpt?.Title))
            {
                return MessageFactory.Text("Нейросеть не смогла подобрать заголовок");
            }

            if (context.IsNotTelegramChannel())
            {
                return MessageFactory.Text($"Нейросеть предлагает заголовок:\n\r{context.FlowState.Gpt.Title}");
            }

            var telegramActivity = context.Activity.CreateReply();
            var encodedText = HttpUtility.HtmlEncode(context.FlowState.Gpt.Title);

            telegramActivity.ChannelData = new TelegramChannelData(
                parameters: new TelegramParameters($"Нейросеть предлагает заголовок:\n\r<code>{encodedText}</code>")
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

        var gptFlowState = context.FlowState.Gpt ?? new();

        return context.FlowState with
        {
            Gpt = gptFlowState with
            {
                Title = @out.Title
            }
        };
    }

    private static IncidentCreateFlowState LogGptFailure(this IChatFlowContext<IncidentCreateFlowState> context, Failure<Unit> failure)
    {
        context.Logger.LogWarning("GptFailure: {gptFailure}", failure.FailureMessage);

        context.BotTelemetryClient.TrackEvent("CompleteIncidentGptFailure", new Dictionary<string, string>
        {
            ["FlowId"] = context.ChatFlowId,
            ["Event"] = "GptFailure",
            ["Message"] = failure.FailureMessage,
            ["SourceDescription"] = context.FlowState.Description.OrEmpty()
        });

        return context.FlowState;
    }
}