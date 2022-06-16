using System;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;
using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

partial class IncidentCreateChatFlow
{
    internal static ChatFlow<Unit> Start(
        this ChatFlow chatFlow,
        IncidentCreateBotOption option,
        ICustomerSetSearchFunc customerSetSearchFunc,
        IContactSetSearchFunc contactSetSearchFunc,
        IIncidentCreateFunc incidentCreateFunc)
        =>
        chatFlow.Start<IncidentCreateFlowState>(
            static () => new())
        .GetDescription()
        .FindCustomer(
            customerSetSearchFunc)
        .FindContcat(
            contactSetSearchFunc)
        .GetTitle()
        .GetCaseType()
        .GetPriority()
        .GetOwner()
        .ConfirmIncident()
        .CreateIncident(
            incidentCreateFunc, option)
        .MapFlowState(
            Unit.From);
}