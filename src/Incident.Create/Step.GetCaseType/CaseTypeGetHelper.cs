using System;
using System.Collections.Generic;
using System.Linq;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class CaseTypeGetHelper
{
    private static readonly IReadOnlyDictionary<string, CaseTypeValue> caseTypeValues;

    private static readonly ValueStepOption valueStepOption;

    static CaseTypeGetHelper()
    {
        var caseTypes = new CaseTypeValue[]
        {
            new(IncidentCaseTypeCode.Question, "Вопрос"),
            new(IncidentCaseTypeCode.Problem, "Проблема"),
            new(IncidentCaseTypeCode.Request, "Запрос")
        };

        caseTypeValues = caseTypes.ToDictionary(GetName, StringComparer.InvariantCultureIgnoreCase);

        valueStepOption = new ValueStepOption(
            messageText: "Выберите тип обращения",
            suggestions: new[]
            {
                caseTypes.Select(GetName).ToArray()
            });

        static string GetName(CaseTypeValue value) => value.Name;
    }

    internal static ValueStepOption GetValueStepOption()
        =>
        valueStepOption;

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, string caseTypeName)
        =>
        $"Тип обращения: {context.EncodeTextWithStyle(caseTypeName, BotTextStyle.Bold)}";

    internal static Result<CaseTypeValue, BotFlowFailure> ParseCaseTypeOrFailure(string text)
        =>
        caseTypeValues.TryGetValue(text, out var value)
            ? Result.Success(value)
            : BotFlowFailure.From(userMessage: "Выберите один из вариантов");
}