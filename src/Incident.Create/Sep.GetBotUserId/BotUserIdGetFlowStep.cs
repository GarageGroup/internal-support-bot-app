using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class BotUserIdGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetBotUserId(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.ForwardValue(
            BotUserIdGetHelper.GetBotUserValueOrBreakAsync,
            MapFlowState);

    private static IncidentCreateFlowState MapFlowState(IncidentCreateFlowState flowState, LookupValue ownerValue)
        =>
        flowState with
        {
            BotUserId = ownerValue.Id
        };
}