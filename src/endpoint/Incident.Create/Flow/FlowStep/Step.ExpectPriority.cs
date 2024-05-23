using System;
using System.Collections.Generic;
using GarageGroup.Infra.Telegram.Bot;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ExpectPriorityOrSkip(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.ExpectValueOrSkip(
            GetPriorityStepOption);

    private static ValueStepOption<IncidentCreateFlowState, PriorityState>? GetPriorityStepOption(
        IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.FlowState.Priority is not null)
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
                    CreateCaseTypeSuggestion(IncidentPriorityCode.High),
                    CreateCaseTypeSuggestion(IncidentPriorityCode.Normal),
                    CreateCaseTypeSuggestion(IncidentPriorityCode.Low)
                ]
            ]
        };

        static Result<PriorityState, ChatRepeatState> GetDefault(string _)
            =>
            default;

        Result<IncidentCreateFlowState, ChatRepeatState> MapFlowState(PriorityState priority)
            =>
            context.FlowState with
            {
                Priority = priority
            };

        KeyValuePair<string, PriorityState> CreateCaseTypeSuggestion(IncidentPriorityCode code)
        {
            var name = context.GetDisplayName(code);
            return new(name, new(code, name));
        }
    }
}