using GarageGroup.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Newtonsoft.Json.Linq;

namespace GarageGroup.Internal.Support;

internal static class DescriptionGetHelper
{
    internal static ChatFlowJump<IncidentCreateFlowState> GetDescriptionOrBreak(IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.FlowState.Description is not null)
        {
            return context.FlowState;
        }

        var description = context.GetDescription();

        if (string.IsNullOrEmpty(description))
        {
            return default;
        }

        return context.FlowState with
        {
            Description = new(description)
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