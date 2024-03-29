using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static class TitleAwaitFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> AwaitTitle(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitValue(
            TitleAwaitHelper.GetStepOption,
            TitleAwaitHelper.ValidateText,
            static (flowState, title) => flowState with
            {
                Title = title
            });
}