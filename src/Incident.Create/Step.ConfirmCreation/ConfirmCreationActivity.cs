using System;
using System.Text;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
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
        context.GetCardActionValueOrAbsent().Fold(
            actionId => actionId switch
            {
                _ when actionId == ActionCreateId => ChatFlowJump.Next(context.FlowState),
                _ when actionId == ActionCancelId => ChatFlowBreakState.From(CanceledText),
                _ => context.RepeatSameStateJump<IncidentCreateFlowState>()
            },
            context.RepeatSameStateJump<IncidentCreateFlowState>);

    internal static IActivity CreateActivity(IChatFlowContext<IncidentCreateFlowState> context)
        =>
        context.IsCardSupported() ? context.CreateExtendedConfirmationActivity() : context.CreateConfirmationActivity();

    private static IActivity CreateConfirmationActivity(this IChatFlowContext<IncidentCreateFlowState> context)
    {
        var flowState = context.FlowState;

        var summaryBuilder = new StringBuilder().AppendSummaryTextBuilder(context, flowState);
        if (string.IsNullOrEmpty(flowState.Description) is false)
        {
            var description = context.EncodeText(flowState.Description);
            summaryBuilder = summaryBuilder.AppendLine(context).AppendRow(context, "Описание", description);
        }

        var card = new HeroCard
        {
            Title = QuestionText,
            Buttons = context.CreateCardActions()
        };

        return MessageFactory.Attachment(card.ToAttachment(), summaryBuilder.ToString());
    }

    private static IActivity CreateExtendedConfirmationActivity(this IChatFlowContext<IncidentCreateFlowState> context)
        =>
        new HeroCard
        {
            Title = QuestionText,
            Subtitle = new StringBuilder().AppendSummaryTextBuilder(context, context.FlowState).ToString(),
            Text = context.FlowState.Description,
            Buttons = context.CreateCardActions()
        }
        .ToAttachment()
        .ToActivity();

    private static CardAction[] CreateCardActions(this ITurnContext turnContext)
        =>
        new CardAction[]
        {
            new(ActionTypes.PostBack)
            {
                Title = ActionCreateText,
                Text = ActionCreateText,
                Value = turnContext.BuildCardActionValue(ActionCreateId)
            },
            new(ActionTypes.PostBack)
            {
                Title = ActionCancelText,
                Text = ActionCancelText,
                Value = turnContext.BuildCardActionValue(ActionCancelId)
            }
        };

    private static StringBuilder AppendSummaryTextBuilder(
        this StringBuilder stringBuilder, ITurnContext context, IncidentCreateFlowState flowState)
        =>
        Pipeline.Pipe(
            stringBuilder)
        .AppendRow(
            context, "Заголовок", context.EncodeText(flowState.Title))
        .AppendLine(
            context)
        .AppendRow(
            context, "Клиент", context.EncodeText(flowState.CustomerTitle))
        .AppendLine(
            context)
        .AppendRow(
            context, "Контакт", flowState.ContactFullName is not null ? context.EncodeText(flowState.ContactFullName) : "--")
        .AppendLine(
            context)
        .AppendRow(
            context, "Тип обращения", context.EncodeText(flowState.CaseTypeTitle));

    private static StringBuilder AppendRow(this StringBuilder builder, ITurnContext turnContext, string fieldName, string? fieldValue)
    {
        if (string.IsNullOrEmpty(fieldName) is false)
        {
            if (turnContext.IsTelegramChannel())
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

    private static StringBuilder AppendLine(this StringBuilder builder, ITurnContext turnContext)
        =>
        builder.Append(
            turnContext.IsMsteamsChannel() ? "<br>" : "\n\r\n\r");
}