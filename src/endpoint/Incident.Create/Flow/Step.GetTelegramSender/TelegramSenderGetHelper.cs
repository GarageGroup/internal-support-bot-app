using GarageGroup.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GarageGroup.Internal.Support;

internal static class TelegramSenderGetHelper
{
    internal static ChatFlowJump<IncidentCreateFlowState> GetTelegramSenderOrBreak(IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.FlowState.TelegramSender is not null)
        {
            return context.FlowState;
        }

        return context.FlowState with
        {
            TelegramSender = context.GetTelegramSender()
        };
    }

    private static TelegramSenderState? GetTelegramSender(this ITurnContext turnContext)
    {
        if (turnContext.GetCardActionValueOrAbsent().IsPresent)
        {
            return default;
        }

        if (turnContext.IsNotTelegramChannel())
        {
            return default;
        }

        if (turnContext.Activity.ChannelData is not JObject jObject)
        {
            return default;
        }

        var replyUser = jObject.SelectToken("message.forward_origin.sender_user")?.ToObject<TelegramSenderJson>();

        if (replyUser is null)
        {
            return default;
        }

        return new() 
        {
            Id = replyUser.Id,
            FirstName = replyUser.FirstName,
            LastName = replyUser.LastName,
        };
    }
}