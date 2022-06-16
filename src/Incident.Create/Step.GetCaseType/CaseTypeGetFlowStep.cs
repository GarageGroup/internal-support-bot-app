using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class CaseTypeGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetCaseType(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitValue(
            static _ => CaseTypeGetHelper.GetValueStepOption(),
            CaseTypeGetHelper.ParseCaseTypeOrFailure,
            CaseTypeGetHelper.CreateResultMessage,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, CaseTypeValue caseTypeValue)
        =>
        flowState with
        { 
            CaseTypeCode = caseTypeValue.Code, 
            CaseTypeTitle = caseTypeValue.Name
        };
}