using System;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static partial class IncidentCreateChatFlow
{
    private static ChatFlow<Unit> RunFlow(
        this ChatFlow chatFlow,
        ICrmCustomerApi crmCustomerApi,
        ICrmContactApi crmContactApi,
        ICrmUserApi crmUserApi,
        ICrmIncidentApi crmIncidentApi,
        ISupportGptApi supportGptApi,
        IncidentCreateFlowOption option)
        =>
        chatFlow.Start<IncidentCreateFlowState>(
            () => new()
            {
                DbMinDate = DateTime.UtcNow.AddDays(-option.DbRequestPeriodInDays).Date
            })
        .GetBotUser()
        .GetDescription()
        .AwaitCustomer(
            crmCustomerApi)
        .AwaitContact(
            crmContactApi)
        .CallGpt(
            supportGptApi)
        .AwaitTitle()
        .AwaitCaseType()
        .AwaitPriority()
        .AwaitOwner(
            crmUserApi)
        .ConfirmIncident()
        .CreateIncident(
            crmIncidentApi)
        .TraceGpt(
            option)
        .ShowIncident(
            option);
}