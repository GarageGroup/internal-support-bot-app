using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class TitleGetFlowStep
{
    private const int DefaultTitleLength = 30;

    internal static ChatFlow<IncidentCreateFlowState> GetTitle(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitText(
            GetStepOption,
            CreateResultMessage,
            (flowState, title) => flowState with
            {
                Title = title
            });

    private static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, string suggestion)
        =>
        $"Заголовок: {context.EncodeTextWithStyle(suggestion, BotTextStyle.Bold)}";

    private static ValueStepOption GetStepOption(IChatFlowContext<IncidentCreateFlowState> context)
        =>
        new(
            messageText: "Укажите заголовок. Можно воспользовать предложенным или ввести свой",
            suggestions: new[]
            {
                new[]
                {
                    StringUtils.Ellipsis(context.FlowState.Description.OrEmpty(), DefaultTitleLength)
                }
            });
}