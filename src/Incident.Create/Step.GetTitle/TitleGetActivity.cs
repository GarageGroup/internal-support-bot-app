using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace GGroupp.Internal.Support;

internal static class TitleGetActivity
{
    private const string CardTitle = "Укажите заголовок";

    private const string CardText = "Можно воспользовать предложенным или ввести свой";

    internal static IActivity CreateTitleHintActivity(this ITurnContext context, TitleGetFlowStepState stepState)
    {
        if (context.IsTelegramChannel())
        {
            var telegramReply = MessageFactory.Text($"{CardTitle}. {CardText}");
            telegramReply.ChannelData = CreateTelegramChannelData(stepState.OfferedTitle.OrEmpty());

            return telegramReply;
        }

        var card = context.CreateTitleHintCard(stepState);
        if (context.IsCardSupported())
        {
            return card.ToAttachment().ToActivity();
        }

        var cardTitle = card.Title;

        card.Title = card.Text;
        card.Text = null;

        return MessageFactory.Attachment(card.ToAttachment(), cardTitle);
    }

    internal static IActivity CreateTitleMustBeSpecifiedActivity()
        =>
        MessageFactory.Text("Название не указано. Повторите попытку");

    internal static Result<string, Unit> GetTitleValueOrFailure(this ITurnContext context, TitleGetFlowStepState stepState)
    {
        if (context.IsNotMessageType())
        {
            return default;
        }

        var buttonResult = context.GetCardActionValueOrAbsent();
        if (buttonResult.IsPresent)
        {
            return buttonResult.OrThrow().Equals(stepState.ButtonId) switch
            {
                true => Result.Present(stepState.OfferedTitle.OrEmpty()),
                _ => default
            };
        }

        return context.Activity.Text.OrEmpty();
    }

    private static HeroCard CreateTitleHintCard(this ITurnContext context, TitleGetFlowStepState stepState)
        =>
        new()
        {
            Title = CardTitle,
            Text = CardText,
            Buttons = new CardAction[]
            {
                new(ActionTypes.PostBack)
                {
                    Title = stepState.OfferedTitle.OrEmpty(),
                    Text = stepState.OfferedTitle.OrEmpty(),
                    Value = context.BuildCardActionValue(stepState.ButtonId)
                }
            }
        };

    private static JObject CreateTelegramChannelData(string offeredTitle)
        =>
        new TelegramChannelData(
            parameters: new()
            {
                ReplyMarkup = new TelegramReplyKeyboardMarkup(
                    keyboard: new[]
                    {
                        new TelegramKeyboardButton[]
                        {
                            new(offeredTitle)
                        }
                    })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true,
                    InputFieldPlaceholder = CardTitle
                }
            })
        .ToJObject();
}