using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support;

internal static class IncidentCreateActivity
{
    internal static IActivity CreateSuccessActivity(IChatFlowContext<IncidentLinkFlowState> context)
        =>
        new HeroCard
        {
            Title = "Инцидент был создан успешно",
            Buttons = new CardAction[]
            {
                new(ActionTypes.OpenUrl)
                {
                    Title = context.FlowState.Title,
                    Value = context.FlowState.Url
                }
            }
        }
        .ToAttachment()
        .ToActivity();
}