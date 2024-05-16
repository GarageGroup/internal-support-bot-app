using System;
using System.Collections.Generic;
using System.Linq;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class CaseTypeAwaitHelper
{
    private static readonly ValueStepOption<CaseTypeValue> valueStepOption;

    private static readonly FlatArray<CaseTypeValue> caseTypes;

    static CaseTypeAwaitHelper()
    {
        caseTypes =
        [
            new(IncidentCaseTypeCode.Question, "Вопрос"),
            new(IncidentCaseTypeCode.Problem, "Проблема"),
            new(IncidentCaseTypeCode.Request, "Запрос")
        ];

        valueStepOption = new(
            messageText: "Выберите тип обращения",
            suggestions: new[]
            {
                caseTypes.AsEnumerable().Select(GetSuggestion).ToArray()
            });

        static KeyValuePair<string, CaseTypeValue> GetSuggestion(CaseTypeValue value)
            =>
            new(value.Name, value);
    }

    internal static ValueStepOption<CaseTypeValue> GetValueStepOption(IChatFlowContext<IncidentCreateFlowState> context)
        =>
        context.FlowState.CaseTypeCode is null ? valueStepOption : new() 
        { 
            SkipStep = true 
        };

    internal static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, CaseTypeValue caseType)
        =>
        $"Тип обращения: {context.EncodeHtmlTextWithStyle(caseType.Name, BotTextStyle.Bold)}";

    internal static Result<CaseTypeValue, BotFlowFailure> ParseCaseTypeOrFailure(string text)
        =>
        valueStepOption.Suggestions.SelectMany(Pipeline.Pipe).GetValueOrAbsent(text).Fold<Result<CaseTypeValue, BotFlowFailure>>(
            static value => Result.Success(value),
            static () => BotFlowFailure.From(userMessage: "Выберите один из вариантов"));

    internal static IncidentCreateFlowState SetCaseTypeTitle(IncidentCreateFlowState state)
    {
        if (state.CaseTypeCode is null || string.IsNullOrEmpty(state.CaseTypeTitle) is false)
        {
            return state;
        }

        return state with
        {
            CaseTypeTitle = caseTypes.AsEnumerable().FirstOrDefault(IsCaseTypeMatched)?.Name
        };

        bool IsCaseTypeMatched(CaseTypeValue caseType)
            =>
            caseType.Code == state.CaseTypeCode;
    }
}