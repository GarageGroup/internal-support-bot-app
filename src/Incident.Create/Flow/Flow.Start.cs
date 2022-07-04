using System;
using GGroupp.Infra;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;
using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;
using IIncidentCreateFlowSender = IQueueWriter<FlowMessage<IncidentCreateFlowMessage>>;

partial class IncidentCreateChatFlow
{
    internal static ChatFlow<Unit> Start(
        this ChatFlow chatFlow,
        ICustomerSetSearchFunc customerSetSearchFunc,
        IContactSetSearchFunc contactSetSearchFunc,
        IIncidentCreateFlowSender incidentCreateFlowSender)
        =>
        chatFlow.Start<IncidentCreateFlowState>(
            static () => new())
        .GetDescription()
        .GetBotUserId()
        .AwaitCustomer(
            customerSetSearchFunc)
        .AwaitContact(
            contactSetSearchFunc)
        .AwaitTitle()
        .AwaitCaseType()
        .AwaitPriority()
        .GetOwner()
        .ConfirmIncident()
        .CreateIncident(
            incidentCreateFlowSender)
        .MapFlowState(
            Unit.From);
}