using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace GGroupp.Internal.Support;

internal static class IncidentCreateActivity
{
    private const string SuccessMessage = "Инцидент был создан успешно";

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
            Title = SuccessMessage,
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

    private static JObject CreateTelegramChannelData(ITurnContext turnContext, IncidentLinkFlowState flowState)
        =>
        new TelegramChannelData(
            parameters: new(SuccessMessage)
            {
                ReplyMarkup = new TelegramInlineKeyboardMarkup(
                    keyboard: new[]
                    {
                        new TelegramInlineKeyboardButton[]
                        {
                            new(turnContext.EncodeText(flowState.Title))
                            {
                                Url = flowState.Url
                            }
                        }
                    })
            })
        .ToJObject();
}