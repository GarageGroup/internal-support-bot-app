using System;
using System.Collections.Generic;
using System.Linq;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class PriorityGetHelper
{
    private static readonly ValueStepOption<PriorityValue> valueStepOption;

    static PriorityGetHelper()
    {
        var caseTypes = new PriorityValue[]
        {
            new(IncidentPriorityCode.Hight, "Высокий"),
            new(IncidentPriorityCode.Normal, "Обычный"),
            new(IncidentPriorityCode.Low, "Низкий")
        };

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

    internal static ValueStepOption<PriorityValue> GetValueStepOption()
        =>
        valueStepOption;

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, PriorityValue priorityValue)
        =>
        $"Приоритет: {context.EncodeHtmlTextWithStyle(priorityValue.Name, BotTextStyle.Bold)}";

    internal static Result<PriorityValue, BotFlowFailure> ParseCaseTypeOrFailure(string text)
        =>
        valueStepOption.Suggestions.SelectMany(Pipeline.Pipe).GetValueOrAbsent(text).Fold<Result<PriorityValue, BotFlowFailure>>(
            static value => Result.Success(value),
            static () => BotFlowFailure.From(userMessage: "Выберите один из вариантов"));
}