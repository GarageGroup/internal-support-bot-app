using System;
using System.Text;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support;

internal static class ConfirmCreationActivity
{
    private const string QuestionText = "Создать инцидент?";

    private const string ActionCreateText = "Создать";

    private const string ActionCancelText = "Отменить";

    private const string CanceledText = "Создание инцидента отменено";

    private static readonly Guid ActionCreateId, ActionCancelId;

    static ConfirmCreationActivity()
    {
        ActionCreateId = Guid.Parse("fb0fa54b-5235-4daa-b0cb-01731a34aa81");
        ActionCancelId = Guid.Parse("98d767c1-9c97-4772-922a-513ad387706c");
    }

    internal static ChatFlowJump<IncidentCreateFlowState> GetConfirmationResult(this IChatFlowContext<IncidentCreateFlowState> context)
        =>
        context.Activity.GetCardActionValueOrAbsent().Fold(
            actionId => actionId switch
            {
                _ when actionId == ActionCreateId => ChatFlowJump.Next(context.FlowState),
                _ when actionId == ActionCancelId => ChatFlowBreakState.From(CanceledText),
                _ => context.RepeatSameStateJump<IncidentCreateFlowState>()
            },
            context.RepeatSameStateJump<IncidentCreateFlowState>);

    internal static IActivity CreateActivity(IChatFlowContext<IncidentCreateFlowState> context)
        =>
        context.Activity.IsCardSupported()
        ? context.CreateExtendedConfirmationActivity()
        : context.CreateConfirmationActivity();

    private static IActivity CreateConfirmationActivity(this IChatFlowContext<IncidentCreateFlowState> context)
    {
        var activity = context.Activity;
        var flowState = context.FlowState;

        var summaryBuilder = new StringBuilder().AppendSummaryTextBuilder(context);
        if (string.IsNullOrEmpty(flowState.Description) is false)
        {
            summaryBuilder = summaryBuilder.AppendLine(activity).AppendRow(activity, "Описание", flowState.Description);
        }

        var card = new HeroCard
        {
            Title = QuestionText,
            Buttons = activity.CreateCardActions()
        };

        return MessageFactory.Attachment(card.ToAttachment(), summaryBuilder.ToString().ToEncodedActivityText());
    }

    private static IActivity CreateExtendedConfirmationActivity(this IChatFlowContext<IncidentCreateFlowState> context)
        =>
        new HeroCard
        {
            Title = QuestionText,
            Subtitle = new StringBuilder().AppendSummaryTextBuilder(context).ToString(),
            Text = context.FlowState.Description,
            Buttons = context.Activity.CreateCardActions()
        }
        .ToAttachment()
        .ToActivity();

    private static CardAction[] CreateCardActions(this Activity activity)
        =>
        new CardAction[]
        {
            new(ActionTypes.PostBack)
            {
                Title = ActionCreateText,
                Text = ActionCreateText,
                Value = activity.BuildCardActionValue(ActionCreateId)
            },
            new(ActionTypes.PostBack)
            {
                Title = ActionCancelText,
                Text = ActionCancelText,
                Value = activity.BuildCardActionValue(ActionCancelId)
            }
        };

    private static StringBuilder AppendSummaryTextBuilder(this StringBuilder stringBuilder, IChatFlowContext<IncidentCreateFlowState> context)
        =>
        Pipeline.Pipe(
            stringBuilder)
        .AppendRow(
            context.Activity, "Заголовок", context.FlowState.Title)
        .AppendLine(
            context.Activity)
        .AppendRow(
            context.Activity, "Клиент", context.FlowState.CustomerTitle)
        .AppendLine(
            context.Activity)
        .AppendRow(
            context.Activity, "Тип обращения", context.FlowState.CaseTypeTitle);

    private static StringBuilder AppendRow(this StringBuilder builder, Activity activity, string fieldName, string? fieldValue)
    {
        if (string.IsNullOrEmpty(fieldName) is false)
        {
            if (activity.IsTelegram())
            {
                _ = builder.Append("**").Append(fieldName).Append(':').Append("**");
            }
            else
            {
                _ = builder.Append(fieldName).Append(':');
            }
        }

        if (string.IsNullOrEmpty(fieldValue) is false)
        {
            _ = builder.Append(' ').Append(fieldValue);
        }

        return builder;
    }

    private static StringBuilder AppendLine(this StringBuilder builder, Activity activity)
        =>
        builder.Append(
            activity.InnerIsMsTeams() ? "<br>" : "\n\r\n\r");

    private static bool InnerIsMsTeams(this Activity activity)
        =>
        string.Equals(activity.ChannelId, Channels.Msteams, StringComparison.InvariantCultureIgnoreCase);
}