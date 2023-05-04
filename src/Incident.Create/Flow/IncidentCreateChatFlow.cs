using System;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

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
        .TraceGpt(
            option)
        .CreateIncident(
            supportApi)
        .ShowIncident(
            option);
}