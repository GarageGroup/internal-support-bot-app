using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class BotUserGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetBotUser(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.SetTypingStatus().GetDataverseUserOrBreak(
            BotUserGetHelper.UnexpectedErrorUserMessage,
            BotUserGetHelper.MapFlowState);
}