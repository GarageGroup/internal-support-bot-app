using System;
using System.Collections.Generic;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static class TitleGetFlowStep
{
    private const int MaxTitleLength = 200;

    private const int DefaultTitleLength = 130;

    internal static ChatFlow<IncidentCreateFlowState> GetTitle(this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.AwaitValue(
            GetStepOption,
            ValidateText,
            CreateResultMessage,
            static (flowState, title) => flowState with
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

    private static ValueStepOption<string> GetStepOption(IChatFlowContext<IncidentCreateFlowState> context)
        =>
        new(
            messageText: "Укажите или выберите заголовок",
            suggestions: new[]
            {
                new[]
                {
                    GetTitleSuggestion(context.FlowState.Description)
                }
            });

    private static KeyValuePair<string, string> GetTitleSuggestion(string? description)
        =>
        new(
            key: StringUtils.Ellipsis(description.OrEmpty(), DefaultTitleLength),
            value: GetMaxLengthTitle(description));

    private static string GetMaxLengthTitle(string? description)
        =>
        description switch
        {
            { Length: > MaxTitleLength } => description[..MaxTitleLength],
            _ => description.OrEmpty()
        };
}