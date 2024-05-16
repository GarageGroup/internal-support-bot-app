using System;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GarageGroup.Internal.Support;

internal static class IncidentCreateHelper
{
    private const string TemporaryText = "Создание обращения выполняется...";

    internal static IActivity CreateTemporaryActivity(IChatFlowContext<IncidentCreateFlowState> context)
    {
        if (context.IsNotTelegramChannel())
        {
            return MessageFactory.Text(TemporaryText);
        }

        var telegramActivity = context.Activity.CreateReply();

        telegramActivity.ChannelData = new TelegramChannelData(
            parameters: new(TemporaryText)
            {
                DisableNotification = true
            })
            .ToJObject();

        return telegramActivity;
    }

    internal static ValueTask<ChatFlowJump<IncidentCreateFlowState>> CreateIncidentOrBeakAsync(
        this ICrmIncidentApi crmIncidentApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static flowState => new IncidentCreateIn(
                ownerId: flowState.Owner?.Id ?? default,
                customerId: flowState.Customer?.Id ?? default,
                contactId: flowState.Contact?.Id,
                title: flowState.Title.OrEmpty(),
                description: flowState.Description?.Value,
                caseTypeCode: flowState.CaseTypeCode.GetValueOrDefault(),
                priorityCode: flowState.PriorityCode,
                callerUserId: flowState.BotUserId.GetValueOrDefault())
            {
                SenderTelegramId = flowState.TelegramSender?.Id
            })
        .PipeValue(
            crmIncidentApi.CreateAsync)
        .Map(
            incident => context.FlowState with
            {
                Title = incident.Title,
                IncidentId = incident.Id,
            },
            ToUnexpectedBreakState)
        .Fold(
            ChatFlowJump.Next,
            ChatFlowJump.Break<IncidentCreateFlowState>);

    private static ChatFlowBreakState ToUnexpectedBreakState<TFailureCode>(Failure<TFailureCode> failure)
        where TFailureCode : struct
        =>
        ChatFlowBreakState.From(
            userMessage: "При создании обращения произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее",
            logMessage: failure.FailureMessage);
}