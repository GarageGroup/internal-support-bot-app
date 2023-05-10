using System;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static partial class IncidentCreateChatFlow
{
    private static ChatFlow<Unit> RunFlow(
        this ChatFlow chatFlow, ISupportApi supportApi, ISupportGptApi supportGptApi, IncidentCreateFlowOption option)
        =>
        chatFlow.Start<IncidentCreateFlowState>(
            static () => new())
        .GetBotUser()
        .GetDescription()
        .AwaitCustomer(
            supportApi)
        .AwaitContact(
            supportApi)
        .CallGpt(
            supportGptApi)
        .AwaitTitle()
        .AwaitCaseType()
        .AwaitPriority()
        .AwaitOwner(
            supportApi)
        .ConfirmIncident()
        .CreateIncident(
            supportApi)
        .TraceGpt(
            option)
        .ShowIncident(
            option);
}