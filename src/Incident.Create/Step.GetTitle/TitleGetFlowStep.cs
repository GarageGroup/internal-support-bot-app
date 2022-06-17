using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class TitleGetFlowStep
{
    private const int MaxTitleLength = 200;

    private const int DefaultTitleLength = 30;

    internal static ChatFlow<IncidentCreateFlowState> GetTitle(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitValue(
            GetStepOption,
            ValidateText,
            CreateResultMessage,
            (flowState, title) => flowState with
            {
                Title = title
            });

    private static Result<string, BotFlowFailure> ValidateText(string text)
        =>
        text.Length switch
        {
            <= MaxTitleLength => Result.Success(text),
            _ => BotFlowFailure.From($"Длина заголовка не может быть больше {MaxTitleLength}")
        };

    private static string CreateResultMessage(IChatFlowContext<IncidentCreateFlowState> context, string suggestion)
        =>
        $"Заголовок: {context.EncodeTextWithStyle(suggestion, BotTextStyle.Bold)}";

    private static ValueStepOption GetStepOption(IChatFlowContext<IncidentCreateFlowState> context)
        =>
        new(
            messageText: "Укажите или выберите заголовок",
            suggestions: new[]
            {
                new[]
                {
                    StringUtils.Ellipsis(context.FlowState.Description.OrEmpty(), DefaultTitleLength)
                }
            });
}
