using System;
using System.Collections.Generic;
using System.Linq;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class PriorityGetHelper
{
    private static readonly IReadOnlyDictionary<string, PriorityValue> priorityValues;

    private static readonly ValueStepOption valueStepOption;

    static PriorityGetHelper()
    {
        var caseTypes = new PriorityValue[]
        {
            new(IncidentPriorityCode.Hight, "Высокий"),
            new(IncidentPriorityCode.Normal, "Обычный"),
            new(IncidentPriorityCode.Low, "Низкий")
        };

        priorityValues = caseTypes.ToDictionary(GetName, StringComparer.InvariantCultureIgnoreCase);

        valueStepOption = new ValueStepOption(
            messageText: "Выберите приоритет",
            suggestions: new[]
            {
                caseTypes.Select(GetName).ToArray()
            });

        static string GetName(PriorityValue value) => value.Name;
    }

    internal static ValueStepOption GetValueStepOption()
        =>
        valueStepOption;

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, string caseTypeName)
        =>
        $"Приоритет: {context.EncodeTextWithStyle(caseTypeName, BotTextStyle.Bold)}";

    internal static Result<PriorityValue, BotFlowFailure> ParseCaseTypeOrFailure(string text)
        =>
        priorityValues.TryGetValue(text, out var value)
            ? Result.Success(value)
            : BotFlowFailure.From(userMessage: "Выберите один из вариантов");
}