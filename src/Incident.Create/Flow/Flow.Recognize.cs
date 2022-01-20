using System;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

partial class IncidentCreateChatFlow
{
    internal static Result<ChatFlow, Unit> InternalRecoginzeOrFailure(this IBotContext context)
    {
        var activity = context.TurnContext.Activity;
        if (activity.IsNotMessageType())
        {
            return default;
        }

        if (activity.IsTelegram() && string.Equals(activity.Text, "/start", StringComparison.InvariantCultureIgnoreCase))
        {
            return default;
        }

        return context.CreateChatFlow("IncidentCreate");
    }
}