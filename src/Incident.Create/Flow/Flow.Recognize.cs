using System;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

partial class IncidentCreateChatFlow
{
    internal static Result<ChatFlow, Unit> InternalRecoginzeOrFailure(this IBotContext context)
    {
        var turnContext = context.TurnContext;
        if (turnContext.IsNotMessageType())
        {
            return default;
        }

        if (turnContext.IsTelegramChannel() && string.Equals(turnContext.Activity.Text, "/start", StringComparison.InvariantCultureIgnoreCase))
        {
            return default;
        }

        return context.CreateChatFlow("IncidentCreate");
    }
}