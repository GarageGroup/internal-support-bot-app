using System.Text;
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
        telegramReply.ChannelData = CreateTelegramChannelData(context, context.FlowState);

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

    private static JObject CreateTelegramChannelData(this ITurnContext turnContext, IncidentLinkFlowState flowState)
    {
        var encodedTitle = turnContext.EncodeText(flowState.Title);
        var messageBuilder = new StringBuilder();

        if (string.IsNullOrEmpty(encodedTitle))
        {
            messageBuilder = messageBuilder.Append($"<a href=\"{flowState.Url}\">Инцидент</a>");
        }
        else
        {
            messageBuilder = messageBuilder.Append($"Инцидент <a href=\"{flowState.Url}\">{encodedTitle}</a>");
        }

        var text = messageBuilder.Append(' ').Append("был создан успешно").ToString();
        return new TelegramChannelData(
            parameters: new(text)
            {
                ParseMode = TelegramParseMode.Html,
                ReplyMarkup = new TelegramReplyKeyboardRemove()
            })
        .ToJObject();
    }
}