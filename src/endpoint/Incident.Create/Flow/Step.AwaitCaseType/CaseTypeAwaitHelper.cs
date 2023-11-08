using System;
using System.Collections.Generic;
using System.Linq;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class CaseTypeAwaitHelper
{
    private static readonly ValueStepOption<CaseTypeValue> valueStepOption;

    static CaseTypeAwaitHelper()
    {
        var caseTypes = new CaseTypeValue[]
        {
            new(IncidentCaseTypeCode.Question, "Вопрос"),
            new(IncidentCaseTypeCode.Problem, "Проблема"),
            new(IncidentCaseTypeCode.Request, "Запрос")
        };

        valueStepOption = new(
            messageText: "Выберите тип обращения",
            suggestions: new[]
            {
                caseTypes.Select(GetSuggestion).ToArray()
            });

        static KeyValuePair<string, CaseTypeValue> GetSuggestion(CaseTypeValue value)
            =>
            new(value.Name, value);
    }

    internal static ValueStepOption<CaseTypeValue> GetValueStepOption()
        =>
        valueStepOption;

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, CaseTypeValue caseType)
        =>
        $"Тип обращения: {context.EncodeHtmlTextWithStyle(caseType.Name, BotTextStyle.Bold)}";

    internal static Result<CaseTypeValue, BotFlowFailure> ParseCaseTypeOrFailure(string text)
        =>
        valueStepOption.Suggestions.SelectMany(Pipeline.Pipe).GetValueOrAbsent(text).Fold<Result<CaseTypeValue, BotFlowFailure>>(
            static value => Result.Success(value),
            static () => BotFlowFailure.From(userMessage: "Выберите один из вариантов"));
}