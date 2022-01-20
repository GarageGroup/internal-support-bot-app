using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class ConfirmCreationFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ConfirmCreation(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.SendActivity(
            ConfirmCreationActivity.CreateActivity)
        .Forward(
            ConfirmCreationActivity.GetConfirmationResult);
}