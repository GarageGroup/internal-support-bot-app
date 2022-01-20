using System;
using GGroupp.Infra.Bot.Builder;

namespace GGroupp.Internal.Support;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetSearchIn, Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>>;
using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

partial class IncidentCreateChatFlow
{
    internal static ChatFlow<Unit> InvokeFlow(
        this ChatFlow chatFlow,
        IncidentCreateBotOption option,
        ICustomerSetSearchFunc customerSetSearchFunc,
        IIncidentCreateFunc incidentCreateFunc,
        IBotUserProvider botUserProvider)
        =>
        chatFlow.Start(
            static () => new IncidentCreateFlowState())
        .GetDescription()
        .FindCustomer(
            customerSetSearchFunc)
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