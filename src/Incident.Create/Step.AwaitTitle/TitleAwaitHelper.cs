using System;
using System.Collections.Generic;
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

        return new(
            messageText: "Укажите заголовок или подтвердите вариант, сгенерированный нейросетью",
            suggestions: new[]
            {
                new KeyValuePair<string, string>[]
                {
                    new("Подтвердить предложение нейросети", context.FlowState.Gpt.Title.TruncateTitle())
                }
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