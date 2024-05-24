using System;
using System.Collections.Generic;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ExpectCaseTypeOrSkip(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.ExpectValueOrSkip(
            GetCaseTypeStepOption);

    private static ValueStepOption<IncidentCreateFlowState, CaseTypeState>? GetCaseTypeStepOption(
        IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.FlowState.CaseType is not null)
        {
            return null;
        }

        return new(
            text: context.Localizer[ChooseCaseTypeText],
            parse: GetDefault,
            forward: MapFlowState)
        {
            Suggestions =
            [
                [
                    CreateCaseTypeSuggestion(IncidentCaseTypeCode.Question),
                    CreateCaseTypeSuggestion(IncidentCaseTypeCode.Problem),
                    CreateCaseTypeSuggestion(IncidentCaseTypeCode.Request)
                ]
            ]
        };

        static Result<CaseTypeState, ChatRepeatState> GetDefault(string _)
            =>
            default;

        Result<IncidentCreateFlowState, ChatRepeatState> MapFlowState(CaseTypeState caseType)
            =>
            context.FlowState with
            {
                CaseType = caseType
            };

        KeyValuePair<string, CaseTypeState> CreateCaseTypeSuggestion(IncidentCaseTypeCode code)
        {
            var name = context.GetDisplayName(code);
            return new(name, new(code, name));
        }
    }
}