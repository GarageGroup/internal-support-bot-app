using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Newtonsoft.Json.Linq;

namespace GGroupp.Internal.Support;

internal static class DescriptionGetHelper
{
    internal static ChatFlowJump<IncidentCreateFlowState> GetDescriptionOrBreak(IChatFlowContext<IncidentCreateFlowState> context)
    {
        var description = context.GetDescription();

        if (string.IsNullOrEmpty(description))
        {
            return default;
        }

        return context.FlowState with
        {
            Description = description
        };
    }

    private static string? GetDescription(this ITurnContext turnContext)
    {
        var text = turnContext.Activity.Text;

        if (turnContext.GetCardActionValueOrAbsent().IsPresent)
        {
            return default;
        }

        if (string.IsNullOrEmpty(text) is false)
        {
            return text;
        }

        if (turnContext.IsNotTelegramChannel())
        {
            return default;
        }

        if (turnContext.Activity.ChannelData is not JObject jObject)
        {
            return default;
        }

        return jObject.SelectToken("message.caption")?.ToString();
    }
}