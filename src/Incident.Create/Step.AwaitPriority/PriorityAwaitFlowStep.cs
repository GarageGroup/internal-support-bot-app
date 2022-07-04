using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class PriorityAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitPriority(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitValue(
            PriorityAwaitHelper.GetValueStepOption,
            PriorityAwaitHelper.ParseCaseTypeOrFailure,
            PriorityAwaitHelper.CreateResultMessage,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, PriorityValue priorityValue)
        =>
        flowState with
        { 
            PriorityCode = priorityValue.Code, 
            PriorityTitle = priorityValue.Name
        };
}