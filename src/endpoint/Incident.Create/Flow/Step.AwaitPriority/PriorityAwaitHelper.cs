using System;
using System.Collections.Generic;
using System.Linq;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class PriorityAwaitHelper
{
    private static readonly ValueStepOption<PriorityValue> valueStepOption;

    static PriorityAwaitHelper()
    {
        PriorityValue[] caseTypes =
        [
            new(IncidentPriorityCode.Hight, "Высокий"),
            new(IncidentPriorityCode.Normal, "Обычный"),
            new(IncidentPriorityCode.Low, "Низкий")
        ];

        valueStepOption = new(
            messageText: "Выберите приоритет",
            suggestions: new[]
            {
                caseTypes.Select(GetSuggestion).ToArray()
            });

        static KeyValuePair<string, PriorityValue> GetSuggestion(PriorityValue value)
            =>
            new(value.Name, value);
    }

    internal static ValueStepOption<PriorityValue> GetValueStepOption(IChatFlowContext<IncidentCreateFlowState> context)
        =>
        string.IsNullOrEmpty(context.FlowState.PriorityTitle) ? valueStepOption : new()
        {
            SkipStep = true
        };

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, PriorityValue priorityValue)
        =>
        $"Приоритет: {context.EncodeHtmlTextWithStyle(priorityValue.Name, BotTextStyle.Bold)}";

    internal static Result<PriorityValue, BotFlowFailure> ParseCaseTypeOrFailure(string text)
        =>
        valueStepOption.Suggestions.SelectMany(Pipeline.Pipe).GetValueOrAbsent(text).Fold<Result<PriorityValue, BotFlowFailure>>(
            static value => Result.Success(value),
            static () => BotFlowFailure.From(userMessage: "Выберите один из вариантов"));
}