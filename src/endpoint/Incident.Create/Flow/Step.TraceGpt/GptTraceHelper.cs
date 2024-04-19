using System;
using System.Collections.Generic;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class GptTraceHelper
{
    internal static void TraceGpt(this IncidentCreateFlowOption option, IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (string.IsNullOrEmpty(context.FlowState.Gpt.Title))
        {
            return;
        }

        var isGptUsed = string.Equals(context.FlowState.Gpt.Title, context.FlowState.Title, StringComparison.InvariantCulture);
        var properties = new Dictionary<string, string>(option.GptTraceData.AsEnumerable())
        {
            ["flowId"] = context.ChatFlowId,
            ["event"] = "GptUsage",
            ["isGptUsed"] = isGptUsed ? "true" : "false",
            ["gptTitle"] = context.FlowState.Gpt.Title,
            ["selectedTitle"] = context.FlowState.Title.OrEmpty(),
            ["sourceDescription"] = context.FlowState.Gpt.SourceMessage.OrEmpty(),
        };

        context.BotTelemetryClient.TrackEvent("CompleteIncidentGptUsage", properties);
    }
}