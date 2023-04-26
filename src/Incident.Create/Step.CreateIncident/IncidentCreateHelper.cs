using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support;

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
            parameters: new TelegramParameters(TemporaryText)
            {
                DisableNotification = true
            })
            .ToJObject();

        return telegramActivity;
    }

    internal static ValueTask<ChatFlowJump<IncidentCreateOut>> CreateIncidentOrBeakAsync(
        this IIncidentCreateSupplier supportApi, IChatFlowContext<IncidentCreateFlowState> context, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            context.FlowState, cancellationToken)
        .Pipe(
            static flowState => new IncidentCreateIn(
                ownerId: flowState.OwnerId,
                customerId: flowState.CustomerId,
                contactId: flowState.ContactId,
                title: flowState.Title.OrEmpty(),
                description: flowState.Description,
                caseTypeCode: flowState.CaseTypeCode,
                priorityCode: flowState.PriorityCode))
        .PipeValue(
            supportApi.CreateIncidentAsync)
        .MapFailure(
            ToUnexpectedBreakState)
        .Fold(
            ChatFlowJump.Next,
            ChatFlowJump.Break<IncidentCreateOut>);

    private static ChatFlowBreakState ToUnexpectedBreakState<TFailureCode>(Failure<TFailureCode> failure)
        where TFailureCode : struct
        =>
        ChatFlowBreakState.From(
            userMessage: "При создании обращения произошла непредвиденная ошибка. Обратитесь к администратору или повторите попытку позднее",
            logMessage: failure.FailureMessage);
}