using System.Globalization;
using System.Web;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GarageGroup.Internal.Support;

internal static class IncidentShowHelper
{
    internal static IActivity CreateIncidentActivity(this IncidentCreateFlowOption option, IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.IsNotTelegramChannel())
        {
            return CreateHeroCardActivity(context.FlowState, option).WithId(context.FlowState.TemporaryActivityId);
        }

        var telegramReply = context.Activity.CreateReply();
        telegramReply.ChannelData = CreateTelegramChannelData(context.FlowState, option).ToJObject();

        return telegramReply.WithId(context.FlowState.TemporaryActivityId);
    }

    private static IActivity CreateHeroCardActivity(IncidentCreateFlowState flowState, IncidentCreateFlowOption option)
        =>
        new HeroCard
        {
            Title = "Обращение было создано успешно",
            Buttons =
            [
                new(ActionTypes.OpenUrl)
                {
                    Title = flowState.Title,
                    Value = flowState.GetUrl(option)
                }
            ]
        }
        .ToAttachment()
        .ToActivity();

    private static IActivity WithId(this IActivity activity, string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return activity;
        }

        activity.Id = id;
        return activity;
    }

    private static TelegramChannelData CreateTelegramChannelData(IncidentCreateFlowState flowState, IncidentCreateFlowOption option)
        =>
        new(
            parameters: new($"Обращение <a href=\"{flowState.GetUrl(option)}\">{HttpUtility.HtmlEncode(flowState.Title)}</a> было создано успешно")
            {
                ParseMode = TelegramParseMode.Html,
                ReplyMarkup = new TelegramReplyKeyboardRemove()
            });

    private static string GetUrl(this IncidentCreateFlowState flowState, IncidentCreateFlowOption option)
        =>
        string.Format(CultureInfo.InvariantCulture, option.IncidentCardUrlTemplate, flowState.IncidentId);
}