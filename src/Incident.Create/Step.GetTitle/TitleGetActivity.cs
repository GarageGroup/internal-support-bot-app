using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support;

internal static class TitleGetActivity
{
    private const string CardTitle = "Укажите заголовок";

    private const string CardText = "Можно воспользовать предложенным или ввести свой";

    internal static IActivity CreateTitleHintActivity(this ITurnContext context, TitleGetFlowStepState stepState)
    {
        var card = context.Activity.CreateTitleHintCard(stepState);
        if (context.Activity.IsCardSupported())
        {
            return card.ToAttachment().ToActivity();
        }

        if (context.Activity.IsTelegram())
        {
            var activity = MessageFactory.Text($"{CardTitle}. {CardText}");
            var channelData = CreateTelegramChannelData(stepState.OfferedTitle.OrEmpty());

            return activity.SetTelegramChannelData(channelData);
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
        if (context.Activity.IsNotMessageType())
        {
            return default;
        }

        var buttonResult = context.Activity.GetCardActionValueOrAbsent();
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

    private static HeroCard CreateTitleHintCard(this Activity activity, TitleGetFlowStepState stepState)
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
                    Value = activity.BuildCardActionValue(stepState.ButtonId)
                }
            }
        };

    private static TelegramChannelData CreateTelegramChannelData(string offeredTitle)
        =>
        new()
        {
            Method = TelegramMethod.SendMessage,
            Parameters = new()
            {
                ReplyMarkup = new TelegramReplyKeyboardMarkup
                {
                    Keyboard = new[]
                    {
                        new TelegramKeyboardButton[]
                        {
                            new()
                            {
                                Text = offeredTitle
                            }
                        }
                    },
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true,
                    InputFieldPlaceholder = CardTitle
                }
            }
        };
}