using System;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;
using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

partial class IncidentCreateChatFlow
{
    internal static ChatFlow<Unit> Start(
        this ChatFlow chatFlow,
        IncidentCreateBotOption option,
        ICustomerSetSearchFunc customerSetSearchFunc,
        IIncidentCreateFunc incidentCreateFunc,
        IContactSetSearchFunc contactSetSearchFunc)
        =>
        chatFlow.Start<IncidentCreateFlowState>(
            static () => new())
        .GetDescription()
        .GetOwnerId()
        .FindCustomer(
            customerSetSearchFunc)
        .FindContcat(
            contactSetSearchFunc)
        .GetTitle()
        .GetCaseType()
        .ConfirmIncident()
        .CreateIncident(
            incidentCreateFunc, option)
        .MapFlowState(
            Unit.From);
}