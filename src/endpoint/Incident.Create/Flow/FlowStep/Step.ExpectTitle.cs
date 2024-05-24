using System;
using System.Text;
using System.Web;
using GarageGroup.Infra.Telegram.Bot;
using Microsoft.Extensions.Localization;

namespace GarageGroup.Internal.Support;

using static IncidentCreateResource;

partial class IncidentCreateFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> ExpectTitleOrSkip(
        this ChatFlow<IncidentCreateFlowState> chatFlow)
        =>
        chatFlow.ExpectTextOrSkip(
            GetTitleStepOption);

    private static TextStepOption<IncidentCreateFlowState>? GetTitleStepOption(
        IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (string.IsNullOrEmpty(context.FlowState.Title) is false)
        {
            return null;
        }

        if (string.IsNullOrEmpty(context.FlowState.Gpt.Title))
        {
            return new(
                text: context.Localizer[ProvideTitleText],
                forward: MapTitleOrRepeat);
        }

        var encodedTitle = HttpUtility.HtmlEncode(context.FlowState.Gpt.Title);
        var text = string.Format("{0}\n\r<code>{1}</code>", context.Localizer[ProvideOrUseGeneratedTitleText], encodedTitle);

        return new(
            text: text,
            forward: MapTitleOrRepeat)
        {
            Suggestions =
            [
                [
                    new(context.Localizer[UseGeneratedTitleText], context.FlowState.Gpt.Title.CutOff(MaxTitleLength))
                ]
            ]
        };

        Result<IncidentCreateFlowState, ChatRepeatState> MapTitleOrRepeat(string title)
        {
            if (title.Length <= MaxTitleLength)
            {
                return context.FlowState with
                {
                    Title = title
                };
            }

            return ChatRepeatState.From(context.Localizer.GetString(ExcededLengthTitleTemplate, MaxTitleLength));
        }
    }
}