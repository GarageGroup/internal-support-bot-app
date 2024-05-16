using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class CaseTypeAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitCaseType(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitValue(
            CaseTypeAwaitHelper.GetValueStepOption,
            CaseTypeAwaitHelper.ParseCaseTypeOrFailure,
            CaseTypeAwaitHelper.CreateResultMessage,
            MapFlowState)
        .MapFlowState(
            CaseTypeAwaitHelper.SetCaseTypeTitle);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, CaseTypeValue caseTypeValue)
        =>
        flowState with
        { 
            CaseTypeCode = caseTypeValue.Code, 
            CaseTypeTitle = caseTypeValue.Name
        };
}