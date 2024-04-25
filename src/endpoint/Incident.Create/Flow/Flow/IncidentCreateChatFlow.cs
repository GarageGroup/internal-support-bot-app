using System;
using GarageGroup.Infra.Bot.Builder;

namespace GarageGroup.Internal.Support;

internal static partial class IncidentCreateChatFlow
{
    private static ChatFlow<IncidentCreateFlowState> RunFlow(
        this ChatFlowStarter<IncidentCreateFlowState> chatFlow,
        ICrmCustomerApi crmCustomerApi,
        ICrmContactApi crmContactApi,
        ICrmOwnerApi crmOwnerApi,
        ICrmIncidentApi crmIncidentApi,
        ISupportGptApi supportGptApi,
        IncidentCreateFlowOption option)
        =>
        chatFlow.Start(
            () => new()
            {
                DbMinDate = DateTime.UtcNow.AddDays(-option.DbRequestPeriodInDays).Date,
                UrlWebApp = option.WebAppUrl,
            })
        .GetBotUser()
        .GetDescription()
        .GetTelegramSender()
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
            crmOwnerApi)
        .ConfirmIncident()
        .CreateIncident(
            crmIncidentApi)
        .TraceGpt(
            option)
        .ShowIncident(
            option);
}