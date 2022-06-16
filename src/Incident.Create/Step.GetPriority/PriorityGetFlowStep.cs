using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class PriorityGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetPriority(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitValue(
            static _ => PriorityGetHelper.GetValueStepOption(),
            PriorityGetHelper.ParseCaseTypeOrFailure,
            PriorityGetHelper.CreateResultMessage,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, PriorityValue priorityValue)
        =>
        flowState with
        { 
            PriorityCode = priorityValue.Code, 
            PriorityTitle = priorityValue.Name
        };
}