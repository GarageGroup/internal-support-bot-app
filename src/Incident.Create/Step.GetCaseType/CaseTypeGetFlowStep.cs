using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class CaseTypeGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetCaseType(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitChoiceValue(
            static _ => CaseTypeGetHelper.GetCaseTypeChoiceSet(),
            CreateResultMessage,
            MapFlowState);

    private static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, LookupValue typeCodeValue)
        =>
        $"Тип обращения: {context.EncodeTextWithStyle(typeCodeValue.Name, BotTextStyle.Bold)}";

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, LookupValue caseTypeValue)
        => 
        caseTypeValue.GetCaseTypeValueOrAbsent().Map(flowState.WithCaseTypeValue).OrElse(flowState);

    private static IncidentCreateFlowState WithCaseTypeValue(this IncidentCreateFlowState flowState, CaseTypeValue caseTypeValue)
        =>
        flowState with 
        { 
            CaseTypeCode = caseTypeValue.Code, 
            CaseTypeTitle = caseTypeValue.Name
        };
}