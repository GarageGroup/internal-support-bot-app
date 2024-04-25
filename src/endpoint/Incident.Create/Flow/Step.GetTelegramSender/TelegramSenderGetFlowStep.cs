using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class TelegramSenderGetFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> GetTelegramSender(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.Forward(
            TelegramSenderGetHelper.GetTelegramSenderOrBreak);
}