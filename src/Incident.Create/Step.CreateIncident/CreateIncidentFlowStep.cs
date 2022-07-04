using System;
using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GGroupp.Internal.Support;

internal static class CreateIncidentFlowStep
{
    internal static ChatFlow<IncidentCreateFlowState> CreateIncident(
        this ChatFlow<IncidentCreateFlowState> chatFlow, IQueueWriter<FlowMessage<IncidentCreateFlowMessage>> incidentCreateFlowSender)
        =>
        chatFlow.SendFlowMessage(
            CreateFlowMessage,
            incidentCreateFlowSender.SendMessageAsync,
            CreateTemporaryActivity);

    private static IncidentCreateFlowMessage CreateFlowMessage(IncidentCreateFlowState flowState)
        =>
        new(
            ownerId: flowState.OwnerId,
            customerId: flowState.CustomerId,
            contactId: flowState.ContactId,
            title: flowState.Title.OrEmpty(),
            description: flowState.Description,
            caseTypeCode: flowState.CaseTypeCode,
            priorityCode: flowState.PriorityCode,
            callerUserId: null);

    private static IActivity CreateTemporaryActivity()
        =>
        MessageFactory.Text("Создание инцидента выполняется...");
}
