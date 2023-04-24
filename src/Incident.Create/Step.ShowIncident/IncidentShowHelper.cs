using System.Globalization;
using System.Web;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support;

internal static class IncidentShowHelper
{
    internal static IActivity CreateIncidentActivity(this IncidentCreateFlowOption option, IChatFlowContext<IncidentLinkFlowState> context)
    {
        if (context.IsNotTelegramChannel())
        {
            return CreateHeroCardActivity(context.FlowState, option);
        }

        var telegramReply = MessageFactory.Text(default);
        telegramReply.ChannelData = CreateTelegramChannelData(context.FlowState, option).ToJObject();

        return telegramReply;
    }

    private static IActivity CreateHeroCardActivity(IncidentLinkFlowState flowState, IncidentCreateFlowOption option)
        =>
        new HeroCard
        {
            Title = "Инцидент был создан успешно",
            Buttons = new CardAction[]
            {
                new(ActionTypes.OpenUrl)
                {
                    Title = flowState.Title,
                    Value = flowState.GetUrl(option)
                }
            }
        }
        .ToAttachment()
        .ToActivity();

    private static TelegramChannelData CreateTelegramChannelData(IncidentLinkFlowState flowState, IncidentCreateFlowOption option)
        =>
        new(
            parameters: new($"Инцидент <a href=\"{flowState.GetUrl(option)}\">{HttpUtility.HtmlEncode(flowState.Title)}</a> был создан успешно")
            {
                ParseMode = TelegramParseMode.Html,
                ReplyMarkup = new TelegramReplyKeyboardRemove()
            });

    private static string GetUrl(this IncidentLinkFlowState flowState, IncidentCreateFlowOption option)
        =>
        string.Format(CultureInfo.InvariantCulture, option.IncidentCardUrlTemplate, flowState.Id);
}