using System;
using System.Collections.Generic;
using System.Text;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class TitleAwaitHelper
{
    private const int MaxTitleLength = 200;

    internal static Result<string, BotFlowFailure> ValidateText(string text)
        =>
        text.Length switch
        {
            <= MaxTitleLength => Result.Success(text),
            _ => BotFlowFailure.From($"Длина заголовка не может быть больше {MaxTitleLength}")
        };

    internal static ValueStepOption<string> GetStepOption(IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (string.IsNullOrEmpty(context.FlowState.Gpt.Title))
        {
            return new("Укажите заголовок");
        }

        var messageBuilder = new StringBuilder("Укажите заголовок или используйте сгенерированный вариант:\n\r");

        if (context.IsNotTelegramChannel())
        {
            messageBuilder = messageBuilder.Append(context.FlowState.Gpt.Title);
        }
        else
        {
            messageBuilder = messageBuilder.Append("<code>").Append(context.FlowState.Gpt.Title).Append("</code>");
        }

        return new(
            messageText: messageBuilder.ToString(),
            suggestions: new KeyValuePair<string, string>[][]
            {
                [
                    new("Использовать сгенерированный заголовок", context.FlowState.Gpt.Title.TruncateTitle())
                ]
            });
    }

    private static string TruncateTitle(this string title)
        =>
        title.Length switch
        {
            > MaxTitleLength => title[..MaxTitleLength],
            _ => title
        };
}