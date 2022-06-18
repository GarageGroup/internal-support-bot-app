using System.Web;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace GGroupp.Internal.Support;

internal static class IncidentCreateActivity
{
    internal static IActivity CreateSuccessActivity(IChatFlowContext<IncidentLinkFlowState> context)
    {
        if (context.IsNotTelegramChannel())
        {
            return CreateHeroCardActivity(context.FlowState);
        }

        var telegramReply = MessageFactory.Text(default);
        telegramReply.ChannelData = CreateTelegramChannelData(context.FlowState);

        return telegramReply;
    }

    private static IActivity CreateHeroCardActivity(IncidentLinkFlowState flowState)
        =>
        new HeroCard
        {
            Title = "Инцидент был создан успешно",
            Buttons = new CardAction[]
            {
                new(ActionTypes.OpenUrl)
                {
                    Title = flowState.Title,
                    Value = flowState.Url
                }
            }
        }
        .ToAttachment()
        .ToActivity();

    private static JObject CreateTelegramChannelData(IncidentLinkFlowState flowState)
        =>
        new TelegramChannelData(
            parameters: new($"Инцидент <a href=\"{flowState.Url}\">{HttpUtility.HtmlEncode(flowState.Title)}</a> был создан успешно")
            {
                ParseMode = TelegramParseMode.Html,
                ReplyMarkup = new TelegramReplyKeyboardRemove()
            })
        .ToJObject();
}