using System;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;
using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

partial class IncidentCreateChatFlow
{
    internal static ChatFlow<Unit> InvokeFlow(
        this ChatFlow chatFlow,
        IncidentCreateBotOption option,
        ICustomerSetSearchFunc customerSetSearchFunc,
        IIncidentCreateFunc incidentCreateFunc,
        IContactSetSearchFunc contactSetSearchFunc,
        ILoggerFactory loggerFactory,
        IBotUserProvider botUserProvider)
        =>
        chatFlow.Start(
            static () => new IncidentCreateFlowState())
        .GetDescription()
        .FindCustomer(
            customerSetSearchFunc)
        .FindContcat(
            contactSetSearchFunc, loggerFactory)
        .GetTitle()
        .GetCaseType()
        .ConfirmCreation()
        .CreateIncident(
            botUserProvider,
            incidentCreateFunc,
            option)
        .MapFlowState(
            Unit.From);
}