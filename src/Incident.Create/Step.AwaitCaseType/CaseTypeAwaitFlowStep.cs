using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class CaseTypeAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitCaseType(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitValue(
            CaseTypeAwaitHelper.GetValueStepOption,
            CaseTypeAwaitHelper.ParseCaseTypeOrFailure,
            CaseTypeAwaitHelper.CreateResultMessage,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, CaseTypeValue caseTypeValue)
        =>
        flowState with
        { 
            CaseTypeCode = caseTypeValue.Code, 
            CaseTypeTitle = caseTypeValue.Name
        };
}