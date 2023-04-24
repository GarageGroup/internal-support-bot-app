using System;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

internal static partial class IncidentCreateChatFlow
{
    private static ChatFlow<Unit> RunFlow(
        this ChatFlow chatFlow, ISupportApi supportApi, IncidentCreateFlowOption option)
        =>
        chatFlow.Start<IncidentCreateFlowState>(
            static () => new())
        .GetDescription()
        .AwaitCustomer(
            supportApi)
        .AwaitContact(
            supportApi)
        .AwaitTitle()
        .AwaitCaseType()
        .AwaitPriority()
        .AwaitOwner(
            supportApi)
        .ConfirmIncident()
        .CreateIncident(
            supportApi)
        .ShowIncident(
            option);
}