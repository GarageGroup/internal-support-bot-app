using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class BotUserGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetBotUser(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.SetTypingStatus().ForwardValue(
            BotUserGetHelper.GetBotUserOrBreakAsync,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, DataverseUserValue userValue)
        =>
        flowState with
        { 
            BotUserId = userValue.Id, 
            BotUserName = userValue.Name
        };
}